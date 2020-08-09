using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
namespace Celin
{
    public abstract class RequestCtx<T1, T2> : BaseCtx<T2>
        where T1 : AIS.Service, new()
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
                try
                {
                    var t = ServerCtx.Current.Server.RequestAsync<JsonElement>(Request, cancel);
                    if (Wait(t, cancel))
                    {
                        Responses.Add(new Response<T1>() { Request = Request, Result = t.Result });
                        BaseCmd.Success("Responses {0}.", Responses.Count);
                    }
                }
                catch (Exception ex)
                {
                    BaseCmd.Error("Request failed!\n{0}", ex.Message);
                }
            }
        }
        protected RequestCtx(string cmd, string id) : base(cmd, id)
        {
        }
    }
}
