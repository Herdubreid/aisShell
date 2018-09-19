using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class FormRequestCmd : RequestCmd<AIS.FormRequest>
    {
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
        protected override int OnExecute()
        {
            base.OnExecute();
            var rq = Request;
            rq.formName = FormName.HasValue ? FormName.Parameter.ToUpper() : rq.formName;
            rq.version = Version.HasValue ? Version.Parameter : rq.version;
            rq.formServiceAction = FormServiceAction.HasValue ? FormServiceAction.Parameter : rq.formServiceAction;
            rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;
            rq.queryObjectName = QueryObjectName.HasValue ? QueryObjectName.Parameter : rq.queryObjectName;

            var cmd = new FormRequestCmd(Request);
            cmd.Display(OutFile, false);

            return 1;
        }
        public FormRequestCmd(AIS.FormRequest rq) : base(rq)
        {
            FormName = (false, rq.formName);
            Version = (false, rq.version);
            FormServiceAction = (false, rq.formServiceAction);
            StopOnWarning = (false, rq.stopOnWarning);
            QueryObjectName = (false, rq.queryObjectName);
        }
        public FormRequestCmd()
        {
        }
    }
}
