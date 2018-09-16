using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class RequestCmd<T1> : OutCmd
        where T1 : AIS.Request, new()
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
        protected T1 _rq;
        public T1 Request
        {
            get { return _rq; }
            set
            {
                _rq = value;
                if (FindOnEntry.HasValue) _rq.findOnEntry = FindOnEntry.Parameter.ToUpper();
                if (ReturnControlIDs.HasValue) _rq.returnControlIDs = ReturnControlIDs.Parameter;
                if (MaxPageSize.HasValue) _rq.maxPageSize = MaxPageSize.Parameter.ToString();
                if (AliasNaming.HasValue) _rq.aliasNaming = AliasNaming.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
                if (OutputType.HasValue) _rq.outputType = OutputType.Parameter;
            }
        }
        protected RequestCmd(T1 rq)
        {
            FindOnEntry = (false, rq.findOnEntry);
            ReturnControlIDs = (false, rq.returnControlIDs);
            MaxPageSize = (false, Convert.ToInt32(rq.maxPageSize));
            AliasNaming = (false, rq.aliasNaming ? "TRUE" : "FALSE");
            OutputType = (false, rq.outputType);
        }
        protected RequestCmd() { }
    }
}
