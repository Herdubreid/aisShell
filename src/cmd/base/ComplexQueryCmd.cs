using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class ComplexQueryCmd : QueryCmd
    {
        [Option("-o|--operation", CommandOptionType.SingleValue, Description = "And/Or Operation")]
        [AllowedValues(new string[] { "AND", "OR" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) Operation { get; }
        protected AIS.Request Request { get; set; }
        protected AIS.Query ComplexQuery { get; set; }
        protected override int OnExecute()
        {
            if (Request.query != null && Request.query.condition != null)
            {
                if (!Prompt.GetYesNo("Do you want to change to Complex Query?", false)) return 0;
                Request.query = null;
            }
            if (Request.query is null)
            {
                Request.query = new AIS.Query()
                {
                    complexQuery = new List<AIS.ComplexQuery>()
                };
            }
            if (Operation.HasValue)
            {
                Request.query.complexQuery.Add(new AIS.ComplexQuery()
                {
                    andOr = Operation.Parameter.ToUpper(),
                    query = new AIS.Query()
                    {
                        matchType = MatchType.HasValue
                            ? MatchType.Parameter.ToUpper()
                            : null
                    }
                });
            }
            else if (Request.query.complexQuery.Count == 0)
            {
                Error("And/Or Operator required");
                return 0;
            }

            Query = Request.query;

            ComplexQuery = Request.query.complexQuery[Request.query.complexQuery.Count - 1].query;

            return 1;
        }
    }
}
