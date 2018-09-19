using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("fm", Description = "Form Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("s", typeof(SubCmd))]
    [Subcommand("l", typeof(ListCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class FormCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        public (bool HasValue, string Parameter) Id { get; }
        [Command(Description = "List Parameters")]
        class ListCmd : OutCmd
        {
            [Option("-a|--all", CommandOptionType.NoValue, Description = "List All")]
            bool All { get; }
            [Option("-l|--long", CommandOptionType.NoValue, Description = "Long Format")]
            bool Long { get; }
            FormCmd FormCmd { get; set; }
            void Show(FormCtx formCtx)
            {
                var cmd = new DefCmd(formCtx.Request);
                OutputLine(OutFile, String.Format("Form Context {0}", formCtx.Id));
                cmd.Display(OutFile, Long);
                if (Long)
                {
                    OutputLine("  formActions:");
                    foreach (var fa in formCtx.Request.formActions)
                    {
                        var ctx = new FormActionCmd(fa as AIS.FormAction);
                        Output("    ");
                        ctx.Display(OutFile, false);
                    }
                }
            }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (FormCmd.OnExecute() == 1)
                {
                    if (!All && FormCtx.Current != null) Show(FormCtx.Current);
                    if (All) foreach (var ctx in FormCtx.List) Show(ctx);
                }
                return 1;
            }
            public ListCmd(FormCmd formCmd)
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
        [Command(Description = "Define Form Request")]
        [Subcommand("fa", typeof(FormActCmd))]
        public class DefCmd : FormRequestCmd
        {
            [Command(Description = "Form Action", ThrowOnUnexpectedArgument = false)]
            public class FormActCmd : FormActionCmd
            {
                DefCmd DefCmd { get; set; }
                protected override int OnExecute()
                {
                    if (DefCmd.OnExecute() == 0) return 0;
                    FormActions = FormCtx.Current.Request.formActions;

                    return base.OnExecute();
                }
                public FormActCmd(DefCmd defCmd)
                {
                    DefCmd = defCmd;
                }
            }
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
            public DefCmd(AIS.FormRequest rq) : base(rq)
            {}
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
        int OnExecute()
        {
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