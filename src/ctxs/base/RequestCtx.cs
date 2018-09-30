using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public abstract class RequestCtx<T1, T2> : BaseCtx<T2>
        where T1 : AIS.Request, new()
        where T2 : IBaseCtx
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
                var cancel = new CancellationTokenSource();
                var t = new Task<Tuple<bool, JObject>>(() => ServerCtx.Current.Server.Request<JObject>(Request, cancel));
                if (Wait(t, cancel))
                {

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
        }
        protected RequestCtx(string cmd, string id) : base(cmd, id)
        {
            Request.maxPageSize = "10";
        }
    }
}
