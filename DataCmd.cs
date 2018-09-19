using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("dt", Description = "Data Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("l", typeof(ListCmd))]
    [Subcommand("s", typeof(SubCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class DataCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        public (bool HasValue, string Parameter) Id { get; }
        [Command(Description = "Save Definition")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                DataCtx.Save(FileName.Parameter + ".dctx");
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
                DataCtx.Load(FileName.Parameter + ".dctx");
                Context = DataCtx.Current;
                return 1;
            }
        }
        [Command(Description = "List Parameters")]
        class ListCmd : OutCmd
        {
            [Option("-a|--all", CommandOptionType.NoValue, Description = "List All")]
            bool All { get; }
            [Option("-l|--long", CommandOptionType.NoValue, Description = "Long Format")]
            bool Long { get; }
            DataCmd DataCmd { get; set; }
            void Show(DataCtx dataCtx)
            {
                var cmd = new DefCmd(dataCtx.Request);
                OutputLine(OutFile, String.Format("Data Context {0}", dataCtx.Id));
                cmd.Display(OutFile, Long);
            }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (DataCmd.OnExecute() == 1)
                {
                    if (!All && DataCtx.Current != null) Show(DataCtx.Current);
                    if (All) foreach (var ctx in DataCtx.List) Show(ctx);
                }
                return 1;
            }
            public ListCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Define")]
        class DefCmd : RequestCmd<AIS.DatabrowserRequest>
        {
            const string TABLE = "TARGET_TABLE";
            const string VIEW = "TARGET_VIEW";
            [Argument(0, Description = "Table or View Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) TargetName { get; set; }
            [Option("-v|--view", Description = "View Target Type")]
            public bool ViewTarget { get; }
            DataCmd DataCmd { get; set; }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (DataCmd.Id.HasValue && !DataCtx.Select(DataCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Data Definition?", true))
                    {
                        PromptOptions();
                        DataCtx.Current = new DataCtx(DataCmd.Id.Parameter);
                        DataCtx.List.Add(DataCtx.Current);
                        Context = DataCtx.Current;
                    }
                }
                if (DataCtx.Current is null)
                {
                    Error("No Data Context!");
                    return 1;
                }
                Request = DataCtx.Current.Request;
                var rq = DataCtx.Current.Request;
                rq.targetName = TargetName.HasValue ? TargetName.Parameter.ToUpper() : rq.targetName;
                rq.targetType = ViewTarget ? VIEW : TABLE;
                return 1;
            }
            public DefCmd(AIS.DatabrowserRequest rq) : base(rq)
            {
                TargetName = (false, rq.targetName);
                ViewTarget = rq.targetType.Equals(VIEW);
            }
            public DefCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Submit Request")]
        class SubCmd : BaseCmd
        {
            DataCmd DataCmd { get; set; }
            int OnExecute()
            {
                if (DataCmd.OnExecute() == 1)
                {
                    if (DataCtx.Current is null)
                    {
                        Error("No Data Context!");
                        return 1;
                    }
                    DataCtx.Current.Submit();
                }
                return 1;
            }
            public SubCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Response")]
        class ResCmd : ResponseCmd<AIS.DatabrowserRequest>
        {
            public ResCmd()
            {
                Responses = DataCtx.Responses;
            }
        }
        int OnExecute()
        {
            if (Id.HasValue && !DataCtx.Select(Id.Parameter))
            {
                Error("Data Context '{0}' not found!", Id.Parameter);
                return 0;
            }
            Context = DataCtx.Current;
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(DataCmd),
                Execute = CommandLineApplication.Execute<DataCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(DataCmd)));
        }
    }
}
