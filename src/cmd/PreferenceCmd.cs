using McMaster.Extensions.CommandLineUtils;
using System;

namespace Celin
{
    [Command("pr", Description = "Preference Context")]
    [Subcommand(typeof(ExpCmd))]
    [Subcommand(typeof(SaveCmd))]
    [Subcommand(typeof(LoadCmd))]
    [Subcommand(typeof(ResCmd))]
    [Subcommand(typeof(DefCmd))]
    [Subcommand(typeof(GetCmd))]
    [Subcommand(typeof(PutCmd))]
    public class PreferenceCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        protected (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        protected bool List { get; }
        [Command("exp", Description = "Export Request")]
        class ExpCmd : JObjectCmd
        {
            protected PreferenceCmd Cmd { get; set; }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (Cmd.OnExecute() == 0) return 0;
                Object = PreferenceCtx.Current.Request;
                if (!Iter) Dump();

                return 1;
            }
            public ExpCmd(PreferenceCmd cmd)
            {
                Cmd = cmd;
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
                PreferenceCtx.Save(FileName.Parameter + ".pctx");
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
                PreferenceCtx.Load(FileName.Parameter + ".pctx");
                Context = PreferenceCtx.Current;
                return 1;
            }
        }
        [Command("r", Description = "Response")]
        class ResCmd : ResponseCmd<AIS.PreferenceRequest>
        {
            public ResCmd()
            {
                Responses = PreferenceCtx.Responses;
            }
        }
        [Command("d", Description = "Preference Definition")]
        class DefCmd : BaseCmd
        {
            PreferenceCmd Cmd { get; }
            [Option("-fn|--formName", CommandOptionType.SingleValue, Description = "Form Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FormName { get; set; }
            [Option("-il|--idList", CommandOptionType.SingleValue, Description = "ID List Field")]
            protected (bool HasValue, string Parameter) IdList { get; set; }
            [Option("-on|--objectName", CommandOptionType.SingleValue, Description = "Object Name")]
            protected (bool HasValue, string Parameter) ObjectName { get; set; }
            [Option("-seq|--sequence", CommandOptionType.SingleValue, Description = "Sequence")]
            protected (bool HasValue, int Parameter) Sequence { get; set; }
            [Option("-pd|--preferenceData", CommandOptionType.SingleValue, Description = "Preference Data")]
            protected (bool HasValue, string Parameter) PreferenceData { get; set; }
            protected virtual int OnExecute()
            {
                if (Cmd.Id.HasValue && !PreferenceCtx.Select(Cmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Preference Definition?", false))
                    {
                        PromptOptions();
                        PreferenceCtx.Current = new PreferenceCtx(Cmd.Id.Parameter);
                        PreferenceCtx.List.Add(PreferenceCtx.Current);
                        Context = PreferenceCtx.Current;
                    }
                    else return 0;
                }
                if (Cmd.NullCtx) return 0;

                var rq = PreferenceCtx.Current.Request;
                rq.formName = FormName.HasValue ? FormName.Parameter.ToUpper() : rq.formName;
                rq.idList = IdList.HasValue ? IdList.Parameter : rq.idList;
                rq.objectName = ObjectName.HasValue ? ObjectName.Parameter : rq.objectName;
                rq.sequence = Sequence.HasValue ? Sequence.Parameter : rq.sequence;
                rq.preferenceData = PreferenceData.HasValue ? PreferenceData.Parameter : rq.preferenceData;

                return 1;
            }
            public DefCmd(PreferenceCmd cmd)
            {
                Cmd = cmd;
            }
        }
        [Command("g", Description = "Get Preference")]
        class GetCmd
        {
            PreferenceCmd Cmd { get; }
            int OnExecute()
            {
                if (Cmd.OnExecute() == 0) return 0;
                PreferenceCtx.Current.Request.action = AIS.PreferenceRequest.GET;
                PreferenceCtx.Current.Submit();
                return 1;
            }
            public GetCmd(PreferenceCmd cmd)
            {
                Cmd = cmd;
            }
        }
        [Command("p", Description = "Put Preference")]
        class PutCmd
        {
            PreferenceCmd Cmd { get; }
            int OnExecute()
            {
                if (Cmd.OnExecute() == 0) return 0;
                PreferenceCtx.Current.Request.action = AIS.PreferenceRequest.PUT;
                PreferenceCtx.Current.Submit();
                return 1;
            }
            public PutCmd(PreferenceCmd cmd)
            {
                Cmd = cmd;
            }
        }
        protected bool NullCtx
        {
            get
            {
                if (PreferenceCtx.Current is null)
                {
                    Error("No Preference Context!");
                    return true;
                }
                return false;
            }
        }
        protected int OnExecute()
        {
            if (List) foreach (var c in PreferenceCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !PreferenceCtx.Select(Id.Parameter))
            {
                Error("Preference Context {0} not found!", Id.Parameter);
                return 0;
            }
            if (NullCtx) return 0;
            Context = PreferenceCtx.Current;

            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd
            {
                Type = typeof(PreferenceCmd),
                Execute = CommandLineApplication.Execute<PreferenceCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(PreferenceCmd)));
        }
    }
}
