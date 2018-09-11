using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command(Description = "Define")]
    [Subcommand("a", typeof(ActCmd))]
    public class FormDefCmd : RequestCmd<AIS.FormRequest, FormCtx>
    {
        [Command(Description = "Form Action")]
        public class ActCmd : BaseCmd
        {
            [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Form Action")]
            [SuppressDisplay]
            public bool Remove { get; private set; }
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
            FormDefCmd FormDefCmd { get; set; }
            int OnExecute()
            {
                if (FormDefCmd.OnExecute() == 0) return 0;

                var fas = FormDefCmd.FormCtx.Request.formActions;
                if (ControlID.HasValue && Command.HasValue)
                {
                    fas.Add(new AIS.FormAction()
                    {
                        controlID = ControlID.Parameter,
                        command = Command.Parameter,
                        value = Value.Parameter
                    });
                }
                else if (ControlID.HasValue)
                {
                    var fa = fas.Find(e => e.controlID.Equals(ControlID.Parameter));
                    if (fa is null) Warning("ControlID {0} not found!", ControlID.Parameter);
                    else if (Remove) fas.Remove(fa);
                    else
                    {
                        var cmd = new ActCmd(fa);
                        cmd.Display(OutFile, true);
                    }
                }
                else
                {
                    foreach (var fa in fas)
                    {
                        var cmd = new ActCmd(fa);
                        cmd.Display(OutFile, true);
                    }
                }

                return 1;
            }
            public ActCmd(AIS.FormAction fa)
            {
                ControlID = (false, fa.controlID);
                Command = (false, fa.command);
                Value = (false, fa.value);
            }
            public ActCmd(FormDefCmd formDefCmd)
            {
                FormDefCmd = formDefCmd;
            }
        }
        [Option("-fn|--formName", CommandOptionType.SingleValue, Description = "Form Name")]
        [PromptOption]
        public (bool HasValue, string Parameter) FormName { get; private set; }
        [Option("-v|--version", CommandOptionType.SingleValue, Description = "Version Name")]
        public (bool HasValue, string Parameter) Version { get; private set; }
        [Option("-fa|--formAction", CommandOptionType.SingleValue, Description = "Form Service Action")]
        public (bool HasValue, string Parameter) FormServiceAction { get; private set; }
        [Option("-sw|--stopOnWarning", CommandOptionType.SingleValue, Description = "Stop on Warning")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        public (bool HasValue, string Parameter) StopOnWarning { get; private set; }
        [Option("-qn|--queryName", CommandOptionType.SingleValue, Description = "Query Object Name")]
        public (bool HasValue, string Parameter) QueryObjectName { get; private set; }
        FormCmd FormCmd { get; set; }
        int OnExecute()
        {
            if (FormCmd.Id.HasValue && !FormCtx.Select(FormCmd.Id.Parameter))
            {
                if (Prompt.GetYesNo("New Form Definition?", true))
                {
                    PromptOptions();
                    FormCtx.Current = new FormCtx(FormCmd.Id.Parameter);
                    FormCtx.List.Add(FormCtx.Current);
                }
                return 0;
            }
            if (FormCtx.Current is null)
            {
                Error("No Form Context!");
                return 0;
            }
            RequestCtx = FormCtx.Current;
            var rq = FormCtx.Current.Request;
            rq.formName = FormName.HasValue ? FormName.Parameter : rq.formName;
            rq.version = Version.HasValue ? Version.Parameter : rq.version;
            rq.formServiceAction = FormServiceAction.HasValue ? FormServiceAction.Parameter : rq.formServiceAction;
            rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;
            rq.queryObjectName = QueryObjectName.HasValue ? QueryObjectName.Parameter : rq.queryObjectName;

            var cmd = new FormDefCmd(FormCtx.Current);
            cmd.Display(OutFile, false);
            OutputLine(OutFile);

            return 1;
        }
        public FormDefCmd(FormCtx formCtx) : base(formCtx)
        {
            var rq = formCtx.Request;
            FormName = (false, rq.formName);
            Version = (false, rq.version);
            FormServiceAction = (false, rq.formServiceAction);
            StopOnWarning = (false, rq.stopOnWarning);
            QueryObjectName = (false, rq.queryObjectName);
        }
        public FormDefCmd(FormCmd formCmd)
        {
            FormCmd = formCmd;
        }
    }
}
