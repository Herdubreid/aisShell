using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("sfm", Description = "Stack Form Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("o", typeof(OpenCmd))]
    [Subcommand("l", typeof(ListCmd))]
    public class StackFormCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        public (bool HasValue, string Parameter) Id { get; }
        [Command(Description = "List Parameters")]
        class ListCmd : BaseCmd
        {
            [Option("-a|--all", CommandOptionType.NoValue, Description = "List All")]
            bool All { get; }
            [Option("-l|--long", CommandOptionType.NoValue, Description = "Long Format")]
            bool Long { get; }
            StackFormCmd StackFormCmd { get; set; }
            void Show(StackFormCtx stackFormCtx)
            {
                var cmd = new DefCmd(stackFormCtx);
                OutputLine(OutFile, String.Format("Stack Form Context {0}", stackFormCtx.Id));
                cmd.Display(OutFile, Long);
                if (Long)
                {
                    OutputLine(OutFile, String.Format("Form Context {0}", stackFormCtx.FormCtx.Id));
                    var fcmd = new FormCmd.DefCmd(stackFormCtx.FormCtx);
                    fcmd.Display(OutFile, Long);
                }
            }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 1)
                {
                    if (!All && FormCtx.Current != null) Show(StackFormCtx.Current);
                    if (All) foreach (var ctx in StackFormCtx.List) Show(ctx);
                }
                return 1;
            }
            public ListCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }

        }
        [Command(Description = "Define")]
        public class DefCmd : BaseCmd
        {
            [Command(Description = "Stack Action")]
            public class StackActCmd : BaseCmd
            {
                [Option("-ri|--returnIds", CommandOptionType.SingleValue, Description = "Return Control IDs")]
                public (bool HasValue, string Parameter) ReturnControlIDs { get; set; }
                [Option("-fo|--formOID", CommandOptionType.SingleValue, Description = "Open Form Id")]
                public (bool HasValue, string Parameter) FormOID { get; set; }
                [Option("-sw|--stopOnWarning", CommandOptionType.SingleValue, Description = "Stop on Warning")]
                [AllowedValues(new string[] { "true", "false" })]
                public (bool HasValue, string Parameter) StopOnWarning { get; set; }
                DefCmd DefCmd { get; set; }
                int OnExecute()
                {
                    if (DefCmd.OnExecute() == 0) return 0;

                    var rq = StackFormCtx.Current.Request.actionRequest;
                    rq.returnControlIDs = ReturnControlIDs.HasValue ? ReturnControlIDs.Parameter : rq.returnControlIDs;
                    rq.formOID = FormOID.HasValue ? FormOID.Parameter : rq.formOID;
                    rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;

                    return 1;
                }
                public StackActCmd(DefCmd defCmd)
                {
                    DefCmd = defCmd;
                }
            }
            //public (bool HasValue, string Parameter) List<FormAction> formActions { get; set; };
            [Option("-fc|--formContext", CommandOptionType.SingleValue, Description = "Form Context")]
            public (bool HasValue, string Parameter) FormContext { get; set; }
            [Option("-sk|--stackId", CommandOptionType.SingleValue, Description = "Stack Id")]
            public (bool HasValue, int Parameter) StackId { get; set; }
            [Option("-st|--stateId", CommandOptionType.SingleValue, Description = "State Id")]
            public (bool HasValue, int Parameter) StateId { get; set; }
            [Option("-ri|--rid", CommandOptionType.SingleValue, Description = "Rid")]
            public (bool HasValue, string Parameter) Rid { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.Id.HasValue && !StackFormCtx.Select(StackFormCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Stack Form Definition?", true))
                    {
                        PromptOptions();
                        StackFormCtx.Current = new StackFormCtx(StackFormCmd.Id.Parameter);
                        StackFormCtx.List.Add(StackFormCtx.Current);
                    }
                }
                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return 1;
                }
                if (FormContext.HasValue && !FormCtx.Select(FormContext.Parameter))
                {
                    Error("Form Request {0} not found!", FormContext.Parameter);
                    return 1;
                }
                if (FormCtx.Current is null)
                {
                    Error("No Form Context!");
                    return 1;
                }
                var rq = StackFormCtx.Current.Request;
                if (StackFormCtx.Current.FormCtx is null || FormContext.HasValue)
                {
                    StackFormCtx.Current.FormCtx = FormCtx.Current;
                    rq.formRequest = FormCtx.Current.Request;
                }

                return 1;
            }
            public DefCmd(StackFormCtx stackFormCtx)
            {
                FormContext = (false, stackFormCtx.FormCtx.Id);
                var rq = stackFormCtx.Request;
                StackId = (false, rq.stackId);
                StateId = (false, rq.stateId);
                Rid = (false, rq.rid);
            }
            public DefCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command(Description = "Open Form")]
        class OpenCmd : BaseCmd
        {
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 1)
                {
                    if (StackFormCtx.Current is null)
                    {
                        Error("No Stack Form Context!");
                        return 1;
                    }
                    StackFormCtx.Current.Request.action = "open";
                    StackFormCtx.Current.Submit();
                }
                return 1;
            }
        }
        int OnExecute()
        {
            if (Id.HasValue && !StackFormCtx.Select(Id.Parameter))
            {
                Error("Stack Form Context {0} not found!", Id.Parameter);
                return 0;
            }
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
