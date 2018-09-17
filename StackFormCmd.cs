using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("sfm", Description = "Stack Form Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("o", typeof(OpenCmd))]
    [Subcommand("e", typeof(ExecuteCmd))]
    [Subcommand("c", typeof(CloseCmd))]
    [Subcommand("l", typeof(ListCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class StackFormCmd : BaseCmd
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
            StackFormCmd StackFormCmd { get; set; }
            void Show(StackFormCtx stackFormCtx)
            {
                var cmd = new DefCmd(stackFormCtx.Request);
                OutputLine(OutFile, String.Format("Stack Form Context {0}", stackFormCtx.Id));
                cmd.Display(OutFile, Long);
                if (Long)
                {
                    OutputLine("Stack Actions:");
                    var saCmd = new DefCmd.StackActCmd(stackFormCtx.Request.actionRequest);
                    saCmd.Display(OutFile, true);
                    OutputLine("Form Actions:");
                    foreach (var fa in stackFormCtx.Request.actionRequest.formActions)
                    {
                        var faCmd = new FormActionCmd(fa as AIS.FormAction);
                        Output("  ");
                        faCmd.Display(OutFile, false);
                    }
                    if (stackFormCtx.Request.formRequest != null)
                    {
                        OutputLine(OutFile, "Form Request:");
                        var fcmd = new DefCmd.FormReqCmd(stackFormCtx.Request.formRequest);
                        fcmd.Display(OutFile, Long);
                        OutputLine("  formActions:");
                        foreach (var fa in stackFormCtx.Request.formRequest.formActions)
                        {
                            var faCmd = new FormActionCmd(fa as AIS.FormAction);
                            Output("    ");
                            faCmd.Display(OutFile, false);
                        }
                    }
                }
            }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 1)
                {
                    if (!All && StackFormCtx.Current != null) Show(StackFormCtx.Current);
                    if (All) foreach (var ctx in StackFormCtx.List) Show(ctx);
                }
                return 1;
            }
            public ListCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }

        }
        [Command(Description = "Save Definition")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                StackFormCtx.Save(FileName.Parameter + ".sfctx");
                return 1;
            }
        }
        [Command(Description = "Load Definition")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                StackFormCtx.Load(FileName.Parameter + ".sfctx");
                return 1;
            }
        }
        [Command(Description = "Define")]
        [Subcommand("fr", typeof(FormReqCmd))]
        [Subcommand("sa", typeof(StackActCmd))]
        public class DefCmd : OutCmd
        {
            [Command(Description = "Form Request")]
            [Subcommand("fa", typeof(FormActCmd))]
            public class FormReqCmd : FormRequestCmd
            {
                [Command(Description = "Form Action")]
                public class FormActCmd : FormActionCmd
                {
                    FormReqCmd FormReqCmd { get; set; }
                    protected override int OnExecute()
                    {
                        if (FormReqCmd.OnExecute() == 0) return 0;
                        FormActions = FormReqCmd.Request.formActions;

                        return base.OnExecute();
                    }
                    public FormActCmd(FormReqCmd formReqCmd)
                    {
                        FormReqCmd = formReqCmd;
                    }
                }
                DefCmd DefCmd { get; set; }
                protected override int OnExecute()
                {
                    if (DefCmd.OnExecute() == 0) return 0;

                    if (StackFormCtx.Current.Request.formRequest is null)
                    {
                        StackFormCtx.Current.Request.formRequest = new AIS.FormRequest();
                    }
                    Request = StackFormCtx.Current.Request.formRequest;

                    return base.OnExecute();
                }
                public FormReqCmd(AIS.FormRequest rq) : base(rq)
                { }
                public FormReqCmd(DefCmd defCmd)
                {
                    DefCmd = defCmd;
                }
            }
            [Command(Description = "Stack Action")]
            [Subcommand("fa", typeof(ActCmd))]
            public class StackActCmd : OutCmd
            {
                [Command(Description = "Stack Form Action")]
                class ActCmd : FormActionCmd
                {
                    StackActCmd StackActCmd { get; set; }
                    protected override int OnExecute()
                    {
                        if (StackActCmd.OnExecute() == 0) return 0;
                        FormActions = StackFormCtx.Current.Request.actionRequest.formActions;

                        return base.OnExecute();
                    }
                    public ActCmd(StackActCmd stackActCmd)
                    {
                        StackActCmd = stackActCmd;
                    }
                }
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

                    var cmd = new StackActCmd(rq);
                    cmd.Display(OutFile, false);

                    return 1;
                }
                public StackActCmd(AIS.ActionRequest rq)
                {
                    ReturnControlIDs = (false, rq.returnControlIDs);
                    FormOID = (false, rq.formOID);
                    StopOnWarning = (false, rq.stopOnWarning);
                }
                public StackActCmd(DefCmd defCmd)
                {
                    DefCmd = defCmd;
                }
            }
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
                    else return 0;
                }
                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return 1;
                }

                var rq = StackFormCtx.Current.Request;
                rq.stackId = StackId.HasValue ? StackId.Parameter : rq.stackId;
                rq.stateId = StateId.HasValue ? StateId.Parameter : rq.stateId;
                rq.rid = Rid.HasValue ? Rid.Parameter : rq.rid;

                var cmd = new DefCmd(rq);
                cmd.Display(OutFile, false);

                return 1;
            }
            public DefCmd(AIS.StackFormRequest rq)
            {
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
                if (StackFormCmd.OnExecute() == 0) return 0;

                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return 0;
                }

                StackFormCtx.Current.Request.action = "open";
                StackFormCtx.Current.Submit();

                return 1;
            }
            public OpenCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command(Description = "Execute Form Action")]
        class ExecuteCmd : BaseCmd
        {
            [Option("-ri|--responseIndex", CommandOptionType.SingleValue, Description = "Stack Parameters Response Index")]
            protected (bool HasValue, int Paramter) ResponseIndex { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;

                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return 0;
                }

                var ctx = StackFormCtx.Current;

                if (ResponseIndex.HasValue)
                {
                    try
                    {
                        var res = StackFormCtx.Responses[ResponseIndex.Paramter];
                        ctx.Request.stackId = res.Result["stackId"].ToObject<Int16>();
                        ctx.Request.stateId = res.Result["stateId"].ToObject<Int16>();
                        ctx.Request.rid = res.Result["rid"].ToString();
                    }
                    catch (Exception e)
                    {
                        Error("Response Error!\n{0}", e.Message);
                    }
                }

                ctx.Request.action = "execute";
                ctx.Submit();

                return 1;
            }
            public ExecuteCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command(Description = "Close Form")]
        class CloseCmd : BaseCmd
        {
            [Option("-ri|--responseIndex", CommandOptionType.SingleValue, Description = "Stack Parameters Response Index")]
            protected (bool HasValue, int Paramter) ResponseIndex { get; set; }
            StackFormCmd StackFormCmd { get; set; }
            int OnExecute()
            {
                if (StackFormCmd.OnExecute() == 0) return 0;

                if (StackFormCtx.Current is null)
                {
                    Error("No Stack Form Context!");
                    return 0;
                }

                var ctx = StackFormCtx.Current;

                if (ResponseIndex.HasValue)
                {
                    try
                    {
                        var res = StackFormCtx.Responses[ResponseIndex.Paramter];
                        ctx.Request.stackId = res.Result["stackId"].ToObject<Int16>();
                        ctx.Request.stateId = res.Result["stateId"].ToObject<Int16>();
                        ctx.Request.rid = res.Result["rid"].ToString();
                    }
                    catch (Exception e)
                    {
                        Error("Response Error!\n{0}", e.Message);
                    }
                }

                ctx.Request.action = "close";
                ctx.Submit();

                return 1;
            }
            public CloseCmd(StackFormCmd stackFormCmd)
            {
                StackFormCmd = stackFormCmd;
            }
        }
        [Command(Description = "Response")]
        [Subcommand("l", typeof(ResCmd.ListCmd))]
        class ResCmd : ResponseCmd<AIS.StackFormRequest>
        {
            [Command(Description = "List Responses")]
            public class ListCmd : OutCmd
            {
                [Argument(0, Description = "Result Key")]
                string Key { get; set; }
                ResCmd ResCmd { get; set; }
                int OnExecute()
                {
                    var ndx = 0;
                    foreach (var rs in ResCmd.Responses)
                    {
                        OutputLine(String.Format("{0, 3} {1}", ndx++, rs[Key]));
                    }

                    return 1;
                }
                public ListCmd(ResCmd resCmd)
                {
                    ResCmd = resCmd;
                }
            }
            public ResCmd()
            {
                Responses = StackFormCtx.Responses;
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
