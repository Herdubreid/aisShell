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
    public class GridConverter : CustomCreationConverter<AIS.Grid>
    {
        public override AIS.Grid Create(Type objectType)
        {
            return null;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.ContainsKey("gridRowInsertEvents"))
            {
                return jsonObject.ToObject<AIS.GridInsert>();
            }
            return jsonObject.ToObject<AIS.GridUpdate>();
        }
    }
    public class ActionConverter : CustomCreationConverter<AIS.Action>
    {
        public override AIS.Action Create(Type objectType)
        {
            return null;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            if (jsonObject.ContainsKey("controlID"))
            {
                return jsonObject.ToObject<AIS.FormAction>();
            }
            var ga = JsonConvert.DeserializeObject<AIS.GridAction>(jsonObject.ToString(), new GridConverter());
            return ga;
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
                if (typeof(T) == typeof(AIS.StackFormRequest))
                {
                    var rq = Request as AIS.StackFormRequest;
                    return String.Format("fs_{0}", rq.formRequest.formName);
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
    }
    public interface ICtxId
    {
        string Cmd { get; set; }
        string Id { get; set; }
    }
    public abstract class CtxId<T> : ICtxId
        where T : ICtxId
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
        public string Cmd { get; set; }
        protected CtxId(string cmd, string id)
        {
            Cmd = cmd;
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
        protected RequestCtx(string cmd, string id) : base(cmd, id)
        {
            Request.maxPageSize = "10";
        }
    }
    public class DataCtx : RequestCtx<AIS.DatabrowserRequest, DataCtx>
    {
        public DataCtx(string id) : base("dt", id)
        {
            Request.findOnEntry = "TRUE";
        }
    }
    public class FormCtx : RequestCtx<AIS.FormRequest, FormCtx>
    {
        public FormCtx(string id) : base("fm", id)
        {
            Request.formServiceAction = "R";
        }
    }
    public class StackFormCtx : RequestCtx<AIS.StackFormRequest, StackFormCtx>
    {
        public StackFormCtx(string id) : base("sfm", id)
        { }
    }
    public class ServerCtx : CtxId<ServerCtx>
    {
        public AIS.Server Server { get; set; }
        public ServerCtx(string id, string baseUrl) : base("sv", id)
        {
            Server = new AIS.Server(baseUrl);
        }
    }
}
