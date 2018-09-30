using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class ConditionCmd : BaseCmd
    {
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Condition")]
        protected bool Remove { get; }
        [Argument(0, Description = "Control Id")]
        protected (bool HasValue, string Parameter) ControlId { get; }
        [Argument(1, Description = "Operator")]
        [AllowedValues(new string[]
        {
                "BETWEEN",
                "LIST",
                "EQUAL",
                "NOT_EQUAL",
                "LESS",
                "LESS_EQUAL",
                "GREATER",
                "GREATER_EQUAL",
                "STR_START_WITH",
                "STR_END_WITH",
                "STR_CONTAIN",
                "STR_BLANK",
                "STR_NOT_BLANK"
        }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) Operator { get; }
        [Argument(2, Description = "Value Type")]
        [AllowedValues(new string[] {
            "LITERAL",
            "TODAY",
            "TODAY_PLUS_DAY",
            "TODAY_MINUS_DAY",
            "TODAY_PLUS_MONTH",
            "TODAY_MINUS_MONTH",
            "TODAY_PLUS_YEAR",
            "TODAY_MINUS_YEAR"
        }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) ValueType { get; }
        [Argument(3, Description = "Value  (separate multple values with ';')")]
        protected (bool HasValue, string Parameter) Value { get; }
        protected AIS.Request Request { get; set; }
        protected List<string> RemainingArguments { get; }
        protected virtual int OnExecute()
        {
            if (ControlId.HasValue && Operator.HasValue && ValueType.HasValue)
            {
                var value = (Value.HasValue ? RemainingArguments.Count > 0
                             ? Value.Parameter + " " + RemainingArguments.Aggregate((a, s) => a + " " + s)
                             : Value.Parameter : "").Split(';');
                Request.query.condition.Add(new AIS.Condition()
                {
                    controlId = ControlId.Parameter.ToUpper(),
                    @operator = Operator.Parameter.ToUpper(),
                    value = value.Select(v =>
                    {
                        return new AIS.Value()
                        {
                            content = v,
                            specialValueId = ValueType.Parameter.ToUpper()
                        };
                    }).ToArray()
                });
            }
            else if (ControlId.HasValue)
            {
                var cnd = Request.query.condition.Find(e => e.controlId.Equals(ControlId.Parameter, StringComparison.OrdinalIgnoreCase));
                if (cnd is null) Error("ControlId {0} not found!", ControlId.Parameter);
                else if (Remove) Request.query.condition.Remove(cnd);
                return 0;
            }

            return 1;
        }
    }
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
        protected AIS.Request Request { get; set; }
        protected virtual int OnExecute()
        {
            if (Request.query is null)
            {
                if (MatchType.HasValue) Request.query = new AIS.Query();
                else
                {
                    Error("Match type required!");
                    return 0;
                }
            }
            var qr = Request.query;
            if (MatchType.HasValue) qr.matchType = MatchType.Parameter.ToUpper();
            if (AutoFind.HasValue) qr.autoFind = AutoFind.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (AutoClear.HasValue) qr.autoClear = AutoClear.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);

            return 1;
        }
    }
}
