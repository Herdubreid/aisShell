using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class QueryCmd : BaseCmd
    {
        [Option("-mt|--matchType", CommandOptionType.SingleValue, Description = "Match Type")]
        [AllowedValues(new string[] { "match_all", "match_any" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) MatchType { get; }
        [Option("-af|--autoFind", CommandOptionType.SingleValue, Description = "Automatically Find")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) AutoFind { get; }
        [Option("-ac|--autoClear", CommandOptionType.SingleValue, Description = "Clear Other Fields")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) AutoClear { get; }
        protected AIS.Query Query { get; set; }
        protected virtual int OnExecute()
        {
            if (MatchType.HasValue) Query.matchType = MatchType.Parameter.ToUpper();
            if (AutoFind.HasValue) Query.autoFind = AutoFind.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (AutoClear.HasValue) Query.autoClear = AutoClear.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);

            return 1;
        }
    }
}
