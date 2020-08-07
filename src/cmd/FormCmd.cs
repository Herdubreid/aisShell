using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("fm", Description = "Form Context")]
    [Subcommand(typeof(DefCmd))]
    [Subcommand(typeof(FormInpCmd))]
    [Subcommand(typeof(FormActCmd))]
    [Subcommand(typeof(GridInsCmd))]
    [Subcommand(typeof(GridUpdCmd))]
    [Subcommand(typeof(QryCmd))]
    [Subcommand(typeof(CpqCmd))]
    [Subcommand(typeof(SubCmd))]
    [Subcommand(typeof(ExpCmd))]
    [Subcommand(typeof(ResCmd))]
    [Subcommand(typeof(SaveCmd))]
    [Subcommand(typeof(LoadCmd))]
    public class FormCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        protected (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        protected bool List { get; }
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
                    JToken = ExpCmd.JToken;
                    return base.OnExecute();
                }
                ExpCmd ExpCmd { get; set; }
                public IterCmd(ExpCmd expCmd)
                {
                    ExpCmd = expCmd;
                }
            }
            protected FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (FormCmd.OnExecute() == 0) return 0;
                Object = FormCtx.Current.Request;
                if (!Iter) Dump();

                return 1;
            }
            public ExpCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command("save", Description = "Save Definitions")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; set; }
            int OnExecute()
            {
                PromptOptions();
                FormCtx.Save(FileName.Parameter + ".fctx");
                return 1;
            }
        }
        [Command("load", Description = "Load Definitions")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; set; }
            int OnExecute()
            {
                PromptOptions();
                FormCtx.Load(FileName.Parameter + ".fctx");
                Context = FormCtx.Current;
                return 1;
            }
        }
        [Command("r", Description = "Response")]
        [Subcommand(typeof(IterCmd))]
        class ResCmd : ResponseCmd<AIS.FormRequest>
        {
            [Command("it", Description = "Iterate")]
            class IterCmd : JArrayCmd
            {
                protected override int OnExecute()
                {
                    ResCmd.Iter = true;
                    if (ResCmd.OnExecute() == 0 || ResCmd.NullJToken) return 0;
                    JToken = ResCmd.JToken;
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
                Responses = FormCtx.Responses;
            }
        }
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
                Request = FormCtx.Current.Request;

                return base.OnExecute();
            }
            FormCmd FormReqCmd { get; set; }
            public CpqCmd(FormCmd formReqCmd)
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
                    Query = FormCtx.Current.Request.query;

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
                if (FormCmd.OnExecute() == 0) return 0;
                var qry = FormCtx.Current.Request.query;
                if (qry != null && qry.complexQuery != null)
                {
                    if (!Prompt.GetYesNo("Do you want to hange to normal Query", false)) return 0;
                    FormCtx.Current.Request.query = null;
                }
                if (FormCtx.Current.Request.query is null)
                {
                    FormCtx.Current.Request.query = new AIS.Query();
                }

                Query = FormCtx.Current.Request.query;

                return base.OnExecute();
            }
            FormCmd FormCmd { get; set; }
            public QryCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
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
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                if (FormCtx.Current.Request.formActions == null)
                    FormCtx.Current.Request.formActions = new List<AIS.Action>();
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public GridInsCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
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
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                if (FormCtx.Current.Request.formActions == null)
                    FormCtx.Current.Request.formActions = new List<AIS.Action>();
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public GridUpdCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command("fi", Description = "Form Input", ThrowOnUnexpectedArgument = false)]
        public class FormInpCmd : FormInputCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                if (FormCtx.Current.Request.formInputs == null)
                    FormCtx.Current.Request.formInputs = new List<AIS.Input>();
                FormInputs = FormCtx.Current.Request.formInputs;

                return base.OnExecute();
            }
            public FormInpCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command("fa", Description = "Form Action", ThrowOnUnexpectedArgument = false)]
        public class FormActCmd : FormActionCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                if (FormCtx.Current.Request.formActions == null)
                    FormCtx.Current.Request.formActions = new List<AIS.Action>();
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public FormActCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command("d", Description = "Form Definition")]
        public class DefCmd : FormRequestCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.Id.HasValue && !FormCtx.Select(FormCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Form Definition?", false))
                    {
                        PromptOptions();
                        FormCtx.Current = new FormCtx(FormCmd.Id.Parameter);
                        FormCtx.List.Add(FormCtx.Current);
                        Context = FormCtx.Current;
                    }
                    else return 0;
                }
                if (FormCmd.NullCtx) return 0;
                Request = FormCtx.Current.Request;

                return base.OnExecute();
            }
            public DefCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command("s", Description = "Submit Request")]
        class SubCmd : BaseCmd
        {
            FormCmd FormCmd { get; set; }
            int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                FormCtx.Current.Submit();
                return 1;
            }
            public SubCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        protected bool NullCtx
        {
            get
            {
                if (FormCtx.Current is null)
                {
                    Error("No Form Context!");
                    return true;
                }
                return false;
            }
        }
        protected int OnExecute()
        {
            if (List) foreach (var c in FormCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !FormCtx.Select(Id.Parameter))
            {
                Error("Form Context {0} not found!", Id.Parameter);
                return 0;
            }
            if (NullCtx) return 0;
            Context = FormCtx.Current;

            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(FormCmd),
                Execute = CommandLineApplication.Execute<FormCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(FormCmd)));
        }
    }
}