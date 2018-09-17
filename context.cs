using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
namespace Celin
{
    public class ActionConverter : CustomCreationConverter<AIS.Action>
    {
        public override AIS.Action Create(Type objectType)
        {
            return new AIS.FormAction();
        }
    }
    public class Response<T> where T : AIS.Request
    {
        public T Request { get; set; }
        public JObject Result { get; set; }
        public string ResultKey
        {
            get
            {
                if (typeof(T) == typeof(AIS.FormRequest)) return String.Format("fs_{0}", Request.formName);
                if (typeof(T) == typeof(AIS.DatabrowserRequest))
                {
                    var rq = Request as AIS.DatabrowserRequest;
                    return String.Format("fs_DATABROWSE_{0}", rq.targetName);
                }
                return null;
            }
        }
        public List<ValueTuple<string, JToken>> GridMembers
        {
            get
            {
                var result = new List<ValueTuple<string, JToken>>();
                var grid = (JArray)Result.SelectToken(String.Format("{0}.data.gridData.rowset", ResultKey));
                if (grid is null) BaseCmd.Error("No Grid Member!");
                else if (grid.Count == 0) BaseCmd.Warning("Grid is Empty!");
                else foreach (var e in (JObject)grid[0]) result.Add(new ValueTuple<string, JToken>(e.Key, e.Value));
                return result;
            }
        }
        JToken FindKey(JArray json, string key)
        {
            foreach (var e in json)
            {
                var res = e.Children().Any() ?
                            e.GetType() == typeof(JArray) ?
                            FindKey((JArray)e, key) :
                            FindKey((JObject)e, key) : null;
                if (res != null) return res;
            }
            return null;
        }
        JToken FindKey(JObject json, string key)
        {
            var res = json[key];
            if (res is null)
            {
                foreach (var e in json)
                {
                    res = e.Value.Children().Any() ?
                                e.Value.GetType() == typeof(JArray) ?
                                FindKey((JArray)e.Value, key) :
                                FindKey((JObject)e.Value, key) : null;
                    if (res != null) return res;
                }
            }
            return res;
        }
        public JToken this[string key]
        {
            get => FindKey(Result, key);
        }
    }
    public interface ICtxId
    {
        string Id { get; set; }
    }
    public abstract class CtxId<T> : ICtxId where T : ICtxId
    {
        public static List<T> List { get; set; } = new List<T>();
        public static T Current { get; set; }
        public static bool Select(string id)
        {
            if (List.Exists(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                Current = List.Find(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                return true;
            }
            return false;
        }
        public static void Load(string fname)
        {
            try
            {
                using (StreamReader sr = File.OpenText(fname))
                {
                    var list = JsonConvert.DeserializeObject<List<T>>(sr.ReadToEnd(), new ActionConverter());
                    if (list != null && list.Count > 0)
                    {
                        List = list;
                        Current = List.First();
                    }
                    else BaseCmd.Warning("No data to load!");
                }
            }
            catch (Exception e)
            {
                BaseCmd.Error(e.Message);
            }
        }
        public static void Save(string fname)
        {
            if (!File.Exists(fname) || Prompt.GetYesNo("Do you want to overwrite existing file?", false))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(fname))
                    {
                        sw.Write(JsonConvert.SerializeObject(List));
                    }
                }
                catch (Exception e)
                {
                    BaseCmd.Error(e.Message);
                }
            }
        }
        public string Id { get; set; }
        protected CtxId(string id)
        {
            Id = id;
        }
    }
    public abstract class RequestCtx<T1, T2> : CtxId<T2>
        where T1 : AIS.Request, new()
        where T2 : ICtxId
    {
        public static List<Response<T1>> Responses { get; } = new List<Response<T1>>();
        public T1 Request { get; set; } = new T1();
        public void Submit()
        {
            if (ServerCtx.Current is null)
            {
                BaseCmd.Error("No Server Context!");
            }
            else if (ServerCtx.Current.Server.AuthResponse is null)
            {
                BaseCmd.Error("{0} not connected!", ServerCtx.Current.Id);
            }
            else
            {
                var t = new Task<Tuple<bool, JObject>>(() => ServerCtx.Current.Server.Request<JObject>(Request));
                t.Start();
                while (!t.IsCompleted)
                {
                    Thread.Sleep(500);
                    Console.Write('.');
                }
                if (t.Result.Item1)
                {
                    Responses.Add(new Response<T1>() { Request = Request, Result = t.Result.Item2 });
                    BaseCmd.Success("Responses {0}.", Responses.Count);
                }
                else
                {
                    BaseCmd.Error("Request failed!");
                }
            }
        }
        protected RequestCtx(string id) : base(id)
        {
            Request.maxPageSize = "10";
        }
    }
    public class DataCtx : RequestCtx<AIS.DatabrowserRequest, DataCtx>
    {
        public DataCtx(string id) : base(id)
        {
            Request.findOnEntry = "TRUE";
        }
    }
    public class FormCtx : RequestCtx<AIS.FormRequest, FormCtx>
    {
        public FormCtx(string id) : base(id)
        {
            Request.formServiceAction = "R";
        }
    }
    public class StackFormCtx : RequestCtx<AIS.StackFormRequest, StackFormCtx>
    {
        public StackFormCtx(string id) : base(id)
        { }
    }
    public class ServerCtx : CtxId<ServerCtx>
    {
        public AIS.Server Server { get; set; }
        public ServerCtx(string id, string baseUrl) : base(id)
        {
            Server = new AIS.Server(baseUrl);
        }
    }
}
