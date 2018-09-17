using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class FormActionCmd : OutCmd
    {
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Form Action")]
        protected bool Remove { get; set; }
        [Argument(0, Description = "Control Id")]
        public (bool HasValue, string Parameter) ControlID { get; private set; }
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
        public (bool HasValue, string Parameter) Command { get; private set; }
        [Argument(2, Description = "Value")]
        public (bool HasValue, string Parameter) Value { get; private set; }
        public List<AIS.Action> FormActions { get; set; }
        protected virtual int OnExecute()
        {
            if (ControlID.HasValue && Command.HasValue)
            {
                FormActions.Add(new AIS.FormAction()
                {
                    controlID = ControlID.Parameter,
                    command = Command.Parameter,
                    value = Value.Parameter
                });
            }
            else if (ControlID.HasValue)
            {
                var fa = FormActions.Find(e =>
                {
                    return e.GetType() == typeof(AIS.FormAction) && (e as AIS.FormAction).controlID.Equals(ControlID.Parameter);
                });
                if (fa is null) Warning("ControlID {0} not found!", ControlID.Parameter);
                else if (Remove) FormActions.Remove(fa);
            }

            foreach (var fa in FormActions)
            {
                var cmd = new FormActionCmd(fa as AIS.FormAction);
                cmd.Display(OutFile, false);
            }

            return 1;
        }
        public FormActionCmd(AIS.FormAction a)
        {
            ControlID = (false, a.controlID);
            Command = (false, a.command);
            Value = (false, a.value);
        }
        public FormActionCmd()
        {
        }
    }
}
