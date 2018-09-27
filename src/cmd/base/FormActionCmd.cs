using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class FormActionCmd : BaseCmd
    {
        [Option("-i|--index", CommandOptionType.SingleValue, Description = "Add or Remove Zero-based Index")]
        protected (bool HasValue, int Parameter) Index { get; }
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Form Action")]
        protected bool Remove { get; }
        [Argument(0, Description = "Control Id")]
        protected (bool HasValue, string Parameter) ControlID { get; }
        [Argument(1, Description = "Command")]
        [AllowedValues(new string[]
        {
                "SetControlValue",
                "SetQBEValue",
                "DoAction",
                "SetRadioButton",
                "SetComboValue",
                "SetCheckboxValue",
                "SelectRow",
                "UnSelectRow",
                "UnSelectAllRows",
                "SelectAllRows",
                "ClickGridCell",
                "ClickGridColumnAggregate"
        })]
        protected (bool HasValue, string Parameter) Command { get; }
        [Argument(2, Description = "Value")]
        protected (bool HasValue, string Parameter) Value { get; }
        public List<AIS.Action> FormActions { get; set; }
        protected List<string> RemainingArguments { get; }
        protected virtual int OnExecute()
        {
            try
            {

                if (ControlID.HasValue && Command.HasValue)
                {
                    var value = Value.HasValue ? RemainingArguments.Count > 0
                                     ? Value.Parameter + " " + RemainingArguments.Aggregate((a, s) => a + " " + s)
                                     : Value.Parameter : "";
                    var fa = new AIS.FormAction()
                    {
                        controlID = ControlID.Parameter,
                        command = Command.Parameter,
                        value = value
                    };
                    if (Index.HasValue)
                    {
                        FormActions.Insert(Index.Parameter, fa);
                    }
                    else FormActions.Add(fa);
                }
                else if (ControlID.HasValue || Index.HasValue)
                {
                    var fa = Index.HasValue
                                  ? FormActions[Index.Parameter]
                                  : FormActions.Find(e =>
                                  {
                                      return e.GetType() == typeof(AIS.FormAction) && (e as AIS.FormAction).controlID.Equals(ControlID.Parameter);
                                  });
                    if (fa is null) Error("ControlID {0} not found!", ControlID.Parameter);
                    else if (Remove) FormActions.Remove(fa);
                }
            }
            catch (Exception e) { Error(e.Message); }

            return 1;
        }
        public FormActionCmd(AIS.FormAction a)
        {
            ControlID = (false, a.controlID);
            Command = (false, a.command);
            Value = (false, a.value);
        }
        public FormActionCmd() { }
    }
}
