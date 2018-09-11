using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class RequestCmd<T1, T2> : BaseCmd
        where T1 : AIS.Request, new()
        where T2 : ICtxId
    {
        [Option("-f|--find", CommandOptionType.SingleValue, Description = "Find on Entry")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        public (bool HasValue, string Parameter) FindOnEntry { get; private set; }
        [Option("-ri|--returnIds", CommandOptionType.SingleValue, Description = "Return Control IDs")]
        public (bool HasValue, string Parameter) ReturnControlIDs { get; private set; }
        [Option("-mp|--maxPage", CommandOptionType.SingleValue, Description = "Max Page Size")]
        public (bool HasValue, int Parameter) MaxPageSize { get; private set; }
        [Option("-an|--aliasNaming", CommandOptionType.SingleValue, Description = "Alias Naming")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        public (bool HasValue, string Parameter) AliasNaming { get; private set; }
        [Option("-ot|--outputType", CommandOptionType.SingleValue, Description = "Output Type")]
        public (bool HasValue, string Parameter) OutputType { get; private set; }
        RequestCtx<T1, T2> _requestCtx;
        public RequestCtx<T1, T2> RequestCtx
        {
            get { return _requestCtx; }
            set
            {
                _requestCtx = value;
                var rq = value.Request;
                rq.findOnEntry = FindOnEntry.HasValue ? FindOnEntry.Parameter.ToUpper() : rq.findOnEntry;
                rq.returnControlIDs = ReturnControlIDs.HasValue ? ReturnControlIDs.Parameter : rq.returnControlIDs;
                rq.maxPageSize = MaxPageSize.HasValue ? MaxPageSize.Parameter.ToString() : rq.maxPageSize;
                rq.aliasNaming = AliasNaming.HasValue ? AliasNaming.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase) : rq.aliasNaming;
                rq.outputType = OutputType.HasValue ? OutputType.Parameter : rq.outputType;
            }
        }
        public RequestCmd(RequestCtx<T1, T2> requestCtx)
        {
            _requestCtx = requestCtx;
            var rq = requestCtx.Request;
            FindOnEntry = (false, rq.findOnEntry);
            ReturnControlIDs = (false, rq.returnControlIDs);
            MaxPageSize = (false, Convert.ToInt32(rq.maxPageSize));
            AliasNaming = (false, rq.aliasNaming ? "TRUE" : "FALSE");
            OutputType = (false, rq.outputType);
        }
        public RequestCmd() { }
    }
}
