using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class ResponseCmd<T> : JObjectCmd where T : AIS.Service
    {
        [Option("-i|--index", CommandOptionType.SingleValue, Description = "Zero Based Index")]
        protected (bool HasValue, int Parameter) Index { get; }
        protected List<Response<T>> Responses { get; set; }
        protected override int OnExecute()
        {
            base.OnExecute();
            try
            {
                var res = Index.HasValue ? Responses[Index.Parameter] : Responses.Last();
                JToken = res.Result.Clone();
                if (!Iter) Dump();
            }
            catch (Exception e)
            {
                Error("Response Error!\n{0}", e.Message);
            }
            return 1;
        }
    }
}
