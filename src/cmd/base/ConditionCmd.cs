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
            "LOGIN_USER",
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
        protected AIS.Query Query { get; set; }
        protected List<string> RemainingArguments { get; }
        protected virtual int OnExecute()
        {
            if (Operator.HasValue)
            {
                var cnd = new AIS.Condition()
                {
                    controlId = ControlId.Parameter.ToUpper(),
                    @operator = Operator.Parameter.ToUpper()
                };
                if (ValueType.HasValue)
                {
                    var value = (Value.HasValue ? RemainingArguments.Count > 0
                                 ? Value.Parameter + " " + RemainingArguments.Aggregate((a, s) => a + " " + s)
                                 : Value.Parameter : "").Split(';');
                    cnd.value = value.Select(v =>
                    {
                        return new AIS.Value()
                        {
                            content = v,
                            specialValueId = ValueType.Parameter.ToUpper()
                        };
                    }).ToArray();
                }
                if (Query.condition is null) Query.condition = new List<AIS.Condition>();
                Query.condition.Add(cnd);
            }
            else if (ControlId.HasValue && Query.condition != null)
            {
                var cnd = Query.condition.Find(e => e.controlId.Equals(ControlId.Parameter, StringComparison.OrdinalIgnoreCase));
                if (cnd is null) Error("ControlId {0} not found!", ControlId.Parameter);
                else if (Remove) Query.condition.Remove(cnd);
                return 0;
            }

            return 1;
        }
    }
}
