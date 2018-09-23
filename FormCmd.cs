using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("fm", Description = "Form Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("fi", typeof(FormInpCmd))]
    [Subcommand("fa", typeof(FormActCmd))]
    [Subcommand("gi", typeof(GridInsCmd))]
    [Subcommand("gu", typeof(GridUpdCmd))]
    [Subcommand("s", typeof(SubCmd))]
    [Subcommand("ex", typeof(ExpCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class FormCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        bool List { get; }
        [Command(Description = "Export Request")]
        class ExpCmd : OutCmd
        {
            [Option("-a|--all", CommandOptionType.NoValue, Description = "Export All")]
            bool All { get; }
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (FormCmd.OnExecute() == 1)
                {
                    if (!All && FormCtx.Current != null) Export(FormCtx.Current.Request);
                    if (All) foreach (var ctx in FormCtx.List) Export(ctx.Request);
                }
                return 1;
            }
            public ExpCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Save Definitions")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                FormCtx.Save(FileName.Parameter + ".fctx");
                return 1;
            }
        }
        [Command(Description = "Load Definitions")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                FormCtx.Load(FileName.Parameter + ".fctx");
                Context = FormCtx.Current;
                return 1;
            }
        }
        [Command(Description = "Response")]
        class ResCmd : ResponseCmd<AIS.FormRequest>
        {
            public ResCmd()
            {
                Responses = FormCtx.Responses;
            }
        }
        [Command(Description = "Grid Insert")]
        public class GridInsCmd : GridActionCmd<AIS.GridInsert>
        {
            public override List<AIS.RowEvent> RowEvents(AIS.GridInsert action)
            {
                return action.gridRowInsertEvents;
            }
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public GridInsCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Grid Update")]
        public class GridUpdCmd : GridActionCmd<AIS.GridUpdate>
        {
            public override List<AIS.RowEvent> RowEvents(AIS.GridUpdate action)
            {
                return action.gridRowUpdateEvents;
            }
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public GridUpdCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Form Input", ThrowOnUnexpectedArgument = false)]
        public class FormInpCmd : FormInputCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                FormInputs = FormCtx.Current.Request.formInputs;

                return base.OnExecute();
            }
            public FormInpCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Form Action", ThrowOnUnexpectedArgument = false)]
        public class FormActCmd : FormActionCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.OnExecute() == 0) return 0;
                FormActions = FormCtx.Current.Request.formActions;

                return base.OnExecute();
            }
            public FormActCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Form Definition")]
        public class DefCmd : FormRequestCmd
        {
            FormCmd FormCmd { get; set; }
            protected override int OnExecute()
            {
                if (FormCmd.Id.HasValue && !FormCtx.Select(FormCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Form Definition?", true))
                    {
                        PromptOptions();
                        FormCtx.Current = new FormCtx(FormCmd.Id.Parameter);
                        FormCtx.List.Add(FormCtx.Current);
                        Context = FormCtx.Current;
                    }
                    else return 0;
                }
                if (FormCtx.Current is null)
                {
                    Error("No Form Context!");
                    return 0;
                }

                Request = FormCtx.Current.Request;

                return base.OnExecute();
            }
            public DefCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Submit Request")]
        class SubCmd : BaseCmd
        {
            FormCmd FormCmd { get; set; }
            int OnExecute()
            {
                if (FormCmd.OnExecute() == 1)
                {
                    if (FormCtx.Current is null)
                    {
                        Error("No Form Context!");
                        return 1;
                    }
                    FormCtx.Current.Submit();

                };
                return 1;
            }
            public SubCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
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