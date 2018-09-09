using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public class Response<T>
    {
        public T Request { get; set; }
        public JObject Result { get; set; }
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
                    var data = (JArray)JsonConvert.DeserializeObject(sr.ReadToEnd());
                    if (data.Count > 0)
                    {
                        List = data.ToObject<List<T>>();
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
        public static List<Response<T2>> Responses { get; } = new List<Response<T2>>();
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
                    Responses.Add(new Response<T2>() { Request = Current, Result = t.Result.Item2 });
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
        public DataCtx(string id) : base(id) { }
    }
    public class FormCtx : RequestCtx<AIS.FormRequest, FormCtx>
    {
        public FormCtx(string id) : base(id)
        {
            Request.formServiceAction = "R";
        }
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
