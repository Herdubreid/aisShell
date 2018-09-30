using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class RequestCmd<T1> : BaseCmd
        where T1 : AIS.Request, new()
    {
        [Option("-rc|--returnControlIds", CommandOptionType.SingleValue, Description = "Return Control IDs")]
        protected (bool HasValue, string Parameter) ReturnControlIDs { get; private set; }
        [Option("-mp|--maxPage", CommandOptionType.SingleValue, Description = "Max Page Size")]
        protected (bool HasValue, int Parameter) MaxPageSize { get; private set; }
        [Option("-ot|--outputType", CommandOptionType.SingleValue, Description = "Output Type")]
        protected (bool HasValue, string Parameter) OutputType { get; private set; }
        protected T1 _rq;
        public T1 Request
        {
            get { return _rq; }
            set
            {
                _rq = value;
                if (ReturnControlIDs.HasValue) _rq.returnControlIDs = ReturnControlIDs.Parameter.ToUpper();
                if (MaxPageSize.HasValue) _rq.maxPageSize = MaxPageSize.Parameter.ToString();
                if (OutputType.HasValue) _rq.outputType = OutputType.Parameter;
            }
        }
    }
}
