using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("sfm", Description = "Stack Form Context")]
    [Subcommand(typeof(FormReqCmd))]
    [Subcommand(typeof(StackActCmd))]
    [Subcommand(typeof(OpenCmd))]
    [Subcommand(typeof(ExecuteCmd))]
    [Subcommand(typeof(CloseCmd))]
    [Subcommand(typeof(ExpCmd))]
    [Subcommand(typeof(ResCmd))]
    [Subcommand(typeof(SaveCmd))]
    [Subcommand(typeof(LoadCmd))]
    public class StackFormCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        protected (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        bool List { get; }
        [Command("exp", Description = "Export Request")]
        [Subcommand(typeof(IterCmd))]
        class ExpCmd : JObjectCmd
        {
            [Command("it", Description = "Iterate")]
            class IterCmd : JArrayCmd
            {
                protected override int OnExecute()
                {
                    ExpCmd.Iter = true;
                    if (ExpCmd.OnExecute() == 0 || ExpCmd.NullJToken) return 0;
                    JToken = ExpCmd.JToken.Value;
                    return base.OnExecute();
                }
                ExpCmd ExpCmd { get; set; }
                public IterCmd(ExpCmd expCmd)
                {
                    ExpCmd = expCmd;
                }
            }
            StackFormCmd StackFormCmd { get; set; }
            protected override int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;
                Object = StackFormCtx.Current.Request;
                if (!Iter) Dump();

                return base.OnExecute();
            }
            public ExpCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }

        }
        [Command("save", Description = "Save Definition")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; set; }
            int OnExecute()
            {
                PromptOptions();
                StackFormCtx.Save(FileName.Parameter + ".sfctx");
                return 1;
            }
        }
        [Command("load", Description = "Load Definition")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; set; }
            int OnExecute()
            {
                PromptOptions();
                StackFormCtx.Load(FileName.Parameter + ".sfctx");
                Context = StackFormCtx.Current;
                return 1;
            }
        }
        [Command("fr", Description = "Form Request")]
        [Subcommand(typeof(FormActCmd))]
        [Subcommand(typeof(FormInpCmd))]
        [Subcommand(typeof(GridUpdCmd))]
        [Subcommand(typeof(GridInsCmd))]
        [Subcommand(typeof(QryCmd))]
        [Subcommand(typeof(CpqCmd))]
        class FormReqCmd : FormRequestCmd
        {
            [Command("cpq", Description = "Complex Query")]
            [Subcommand(typeof(ConCmd))]
            class CpqCmd : ComplexQueryCmd
            {
                [Command("cn", Description = "Condition", ThrowOnUnexpectedArgument = false)]
                class ConCmd : ConditionCmd
                {
                    protected override int OnExecute()
                    {
                        if (CpqCmd.OnExecute() == 0) return 0;
                        Query = CpqCmd.ComplexQuery;

                        return base.OnExecute();
                    }
                    CpqCmd CpqCmd { get; set; }
                    public ConCmd(CpqCmd cpqCmd)
                    {
                        CpqCmd = cpqCmd;
                    }
                }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    Request = StackFormCtx.Current.Request.formRequest;

                    return base.OnExecute();
                }
                FormReqCmd FormReqCmd { get; set; }
                public CpqCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            [Command("qry", Description = "Query")]
            [Subcommand(typeof(CondCmd))]
            class QryCmd : QueryCmd
            {
                [Command("cn", Description = "Condition", ThrowOnUnexpectedArgument = false)]
                class CondCmd : ConditionCmd
                {
                    protected override int OnExecute()
                    {
                        if (QryCmd.OnExecute() == 0) return 0;
                        var qry = StackFormCtx.Current.Request.formRequest.query;
                        if (qry != null && qry.complexQuery != null)
                        {
                            if (!Prompt.GetYesNo("Do you want to hange to normal Query", false)) return 0;
                            StackFormCtx.Current.Request.query = null;
                        }
                        if (StackFormCtx.Current.Request.formRequest.query is null)
                        {
                            StackFormCtx.Current.Request.formRequest.query = new AIS.Query();
                        }

                        Query = StackFormCtx.Current.Request.formRequest.query;

                        return base.OnExecute();
                    }
                    QryCmd QryCmd { get; set; }
                    public CondCmd(QryCmd qryCmd)
                    {
                        QryCmd = qryCmd;
                    }
                }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    if (StackFormCtx.Current.Request.formRequest.query is null)
                    {
                        StackFormCtx.Current.Request.formRequest.query = new AIS.Query();
                    }

                    Query = StackFormCtx.Current.Request.formRequest.query;

                    return base.OnExecute();
                }
                FormReqCmd FormReqCmd { get; set; }
                public QryCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            [Command("gi", Description = "Grid Insert")]
            public class GridInsCmd : GridActionCmd<AIS.GridInsert>
            {
                public override List<AIS.RowEvent> RowEvents(AIS.GridInsert action)
                {
                    if (action.gridRowInsertEvents == null)
                        action.gridRowInsertEvents = new List<AIS.RowEvent>();
                    return action.gridRowInsertEvents;
                }
                FormReqCmd FormReqCmd { get; set; }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    if (FormReqCmd.Request.formActions == null)
                        FormReqCmd.Request.formActions = new List<AIS.Action>();
                    FormActions = FormReqCmd.Request.formActions;

                    return base.OnExecute();
                }
                public GridInsCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            [Command("gu", Description = "Grid Update")]
            public class GridUpdCmd : GridActionCmd<AIS.GridUpdate>
            {
                public override List<AIS.RowEvent> RowEvents(AIS.GridUpdate action)
                {
                    if (action.gridRowUpdateEvents == null)
                        action.gridRowUpdateEvents = new List<AIS.RowEvent>();
                    return action.gridRowUpdateEvents;
                }
                FormReqCmd FormReqCmd { get; set; }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    FormActions = StackFormCtx.Current.Request.formRequest.formActions;

                    return base.OnExecute();
                }
                public GridUpdCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            [Command("fi", Description = "Form Input", ThrowOnUnexpectedArgument = false)]
            public class FormInpCmd : FormInputCmd
            {
                FormReqCmd FormReqCmd { get; set; }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    if (FormReqCmd.Request.formInputs == null)
                        FormReqCmd.Request.formInputs = new List<AIS.Input>();
                    FormInputs = FormReqCmd.Request.formInputs;

                    return base.OnExecute();
                }
                public FormInpCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            [Command("fa", Description = "Form Action", ThrowOnUnexpectedArgument = false)]
            public class FormActCmd : FormActionCmd
            {   
                FormReqCmd FormReqCmd { get; set; }
                protected override int OnExecute()
                {
                    if (FormReqCmd.OnExecute() == 0) return 0;
                    if (FormReqCmd.Request.formActions == null)
                       FormReqCmd.Request.formActions = new List<AIS.Action>();
                    FormActions = FormReqCmd.Request.formActions;

                    return base.OnExecute();
                }
                public FormActCmd(FormReqCmd formReqCmd)
                {
                    FormReqCmd = formReqCmd;
                }
            }
            StackFormCmd StackFormCmd { get; set; }
            protected override int OnExecute()
            {
                if (StackFormCmd.Id.HasValue && !StackFormCtx.Select(StackFormCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Stack Form Definition?", false))
                    {
                        PromptOptions();
                        StackFormCtx.Current = new StackFormCtx(StackFormCmd.Id.Parameter);
                        StackFormCtx.List.Add(StackFormCtx.Current);
                        Context = StackFormCtx.Current;
                    }
                    else return 0;
                }
                if (StackFormCtx.Current.Request.actionRequest != null)
                {
                    if (Prompt.GetYesNo("Switch to Stack Action?", false))
                    {
                        StackFormCtx.Current.Request.actionRequest = null;
                        StackFormCtx.Current.Request.formRequest = new AIS.FormRequest();
                    }
                    else
                        return 0;
                }
                if (StackFormCmd.NullCtx) return 0;
                if (StackFormCtx.Current.Request.formRequest is null)
                {
                    StackFormCtx.Current.Request.formRequest = new AIS.FormRequest()
                    {
                        formServiceAction = "R"
                    };
                }
                Request = StackFormCtx.Current.Request.formRequest;

                return base.OnExecute();
            }
            public FormReqCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command("sa", Description = "Stack Action")]
        [Subcommand(typeof(ActCmd))]
        public class StackActCmd : BaseCmd
        {
            [Command("fa", Description = "Stack Form Action", ThrowOnUnexpectedArgument = false)]
            class ActCmd : FormActionCmd
            {
                StackActCmd StackActCmd { get; set; }
                protected override int OnExecute()
                {
                    if (StackActCmd.OnExecute() == 0) return 0;
                    if (StackFormCtx.Current.Request.actionRequest.formActions == null)
                        StackFormCtx.Current.Request.actionRequest.formActions = new List<AIS.Action>();
                    FormActions = StackFormCtx.Current.Request.actionRequest.formActions;

                    return base.OnExecute();
                }
                public ActCmd(StackActCmd stackActCmd)
                {
                    StackActCmd = stackActCmd;
                }
            }
            [Option("-rc|--returnControlIds", CommandOptionType.SingleValue, Description = "Return Control IDs")]
            protected (bool HasValue, string Parameter) ReturnControlIDs { get; set; }
            [Option("-fo|--formOID", CommandOptionType.SingleValue, Description = "Open Form Id")]
            [PromptOption]
            public (bool HasValue, string Parameter) FormOID { get; set; }
            [Option("-sw|--stopOnWarning", CommandOptionType.SingleValue, Description = "Stop on Warning")]
            [AllowedValues(new string[] { "true", "false" })]
            protected (bool HasValue, string Parameter) StopOnWarning { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            protected virtual int OnExecute()
            {
                if (StackFormCmd.Id.HasValue && !StackFormCtx.Select(StackFormCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Stack Form Definition?", false))
                    {
                        PromptOptions();
                        StackFormCtx.Current = new StackFormCtx(StackFormCmd.Id.Parameter);
                        StackFormCtx.Current.Request.actionRequest = new AIS.ActionRequest();
                        StackFormCtx.List.Add(StackFormCtx.Current);
                        Context = StackFormCtx.Current;
                    }
                    else return 0;
                }
                if (StackFormCtx.Current.Request.formRequest != null)
                {
                    if (Prompt.GetYesNo("Switch to Form Actin?", false))
                    {
                        StackFormCtx.Current.Request.formRequest = null;
                        StackFormCtx.Current.Request.actionRequest = new AIS.ActionRequest();
                    }
                    else
                        return 0;
                }
                if (StackFormCmd.NullCtx) return 0;
                var rq = StackFormCtx.Current.Request.actionRequest;
                rq.returnControlIDs = ReturnControlIDs.HasValue ? ReturnControlIDs.Parameter : rq.returnControlIDs;
                rq.formOID = FormOID.HasValue ? FormOID.Parameter.ToUpper() : rq.formOID;
                rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;

                return 1;
            }
            public StackActCmd(AIS.ActionRequest rq)
            {
                ReturnControlIDs = (false, rq.returnControlIDs);
                FormOID = (false, rq.formOID);
                StopOnWarning = (false, rq.stopOnWarning);
            }
            public StackActCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command("o", Description = "Open Form")]
        class OpenCmd : BaseCmd
        {
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;
                StackFormCtx.Current.Request.action = "open";
                StackFormCtx.Current.Submit();

                return 1;
            }
            public OpenCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        protected bool SetStackParameters(int? index)
        {
            var ctx = StackFormCtx.Current;
            try
            {
                var res = index.HasValue ? StackFormCtx.Responses[index.Value] : StackFormCtx.Responses.Last();
                ctx.Request.stackId = res.Result.GetProperty("stackId").GetInt16();
                ctx.Request.stateId = res.Result.GetProperty("stateId").GetInt16();
                ctx.Request.rid = res.Result.GetProperty("rid").ToString();
            }
            catch (Exception e)
            {
                Error("Response Error!\n{0}", e.Message);
                return false;
            }
            return true;
        }
        [Command("e", Description = "Execute Stack Action")]
        class ExecuteCmd : BaseCmd
        {
            [Option("-ri|--responseIndex", CommandOptionType.SingleValue, Description = "Stack Parameters Response Index")]
            protected int? ResponseIndex { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;
                if (!StackFormCmd.SetStackParameters(ResponseIndex)) return 0;
                StackFormCtx.Current.Request.action = "execute";
                StackFormCtx.Current.Submit();

                return 1;
            }
            public ExecuteCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command("c", Description = "Close Form")]
        class CloseCmd : BaseCmd
        {
            [Option("-ri|--responseIndex", CommandOptionType.SingleValue, Description = "Stack Parameters Response Index")]
            protected int? ResponseIndex { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;
                if (!StackFormCmd.SetStackParameters(ResponseIndex)) return 0;

                StackFormCtx.Current.Request.action = "close";
                StackFormCtx.Current.Submit();

                return 1;
            }
            public CloseCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command("r", Description = "Response")]
        [Subcommand(typeof(IterCmd))]
        class ResCmd : ResponseCmd<AIS.StackFormRequest>
        {
            [Command("it", Description = "Iterate")]
            class IterCmd : JArrayCmd
            {
                protected override int OnExecute()
                {
                    ResCmd.Iter = true;
                    if (ResCmd.OnExecute() == 0 || ResCmd.NullJToken) return 0;
                    JToken = ResCmd.JToken.Value;
                    return base.OnExecute();
                }
                ResCmd ResCmd { get; set; }
                public IterCmd(ResCmd resCmd)
                {
                    ResCmd = resCmd;
                }
            }
            public ResCmd()
            {
                Responses = StackFormCtx.Responses;
            }
        }
        protected bool NullCtx
        {
            get
            {
                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return true;
                }
                return false;
            }
        }
        int OnExecute()
        {
            if (List) foreach (var c in StackFormCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !StackFormCtx.Select(Id.Parameter))
            {
                Error("Stack Form Context {0} not found!", Id.Parameter);
                return 0;
            }
            if (NullCtx) return 0;
            Context = StackFormCtx.Current;
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(StackFormCmd),
                Execute = CommandLineApplication.Execute<StackFormCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(StackFormCmd)));
        }
    }
}
