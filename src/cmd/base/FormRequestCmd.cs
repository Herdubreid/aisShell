using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class FormRequestCmd : RequestCmd<AIS.FormRequest>
    {
        [Option("-fn|--formName", CommandOptionType.SingleValue, Description = "Form Name")]
        [PromptOption]
        public (bool HasValue, string Parameter) FormName { get; set; }
        [Option("-v|--version", CommandOptionType.SingleValue, Description = "Version Name")]
        protected (bool HasValue, string Parameter) Version { get; private set; }
        [Option("-fs|--formServicAction", CommandOptionType.SingleValue, Description = "Form Service Action")]
        [AllowedValues(new string[] { "r", "c", "u", "d" })]
        protected (bool HasValue, string Parameter) FormServiceAction { get; private set; }
        [Option("-f|--find", CommandOptionType.SingleValue, Description = "Find on Entry")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) FindOnEntry { get; private set; }
        [Option("-sw|--stopOnWarning", CommandOptionType.SingleValue, Description = "Stop on Warning")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) StopOnWarning { get; private set; }
        [Option("-qn|--queryName", CommandOptionType.SingleValue, Description = "Query Object Name")]
        protected (bool HasValue, string Parameter) QueryObjectName { get; private set; }
        [Option("-an|--aliasNaming", CommandOptionType.SingleValue, Description = "Alias Naming")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) AliasNaming { get; private set; }
        [Option("-be|--bypassER", CommandOptionType.SingleValue, Description = "Bypass Form Service ER Event")]
        [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) BypassER { get; set; }
        protected virtual int OnExecute()
        {
            var rq = Request;
            rq.formName = FormName.HasValue ? FormName.Parameter.ToUpper() : rq.formName;
            rq.version = Version.HasValue ? Version.Parameter.ToUpper() : rq.version;
            rq.formServiceAction = FormServiceAction.HasValue ? FormServiceAction.Parameter.ToUpper() : rq.formServiceAction;
            if (FindOnEntry.HasValue) _rq.findOnEntry = FindOnEntry.Parameter.ToUpper();
            rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;
            rq.queryObjectName = QueryObjectName.HasValue ? QueryObjectName.Parameter : rq.queryObjectName;
            if (AliasNaming.HasValue) _rq.aliasNaming = AliasNaming.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (BypassER.HasValue) _rq.bypassFormServiceEREvent = BypassER.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase);

            return 1;
        }
    }
}
