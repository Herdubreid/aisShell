using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("dt", Description = "Data Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("qry", typeof(QryCmd))]
    [Subcommand("exp", typeof(ExpCmd))]
    [Subcommand("s", typeof(SubCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class DataCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        protected (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        protected bool List { get; }
        [Command(Description = "Save Definition")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; set; }
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
            public (bool HasValue, string Parameter) FileName { get; set; }
            int OnExecute()
            {
                PromptOptions();
                DataCtx.Load(FileName.Parameter + ".dctx");
                Context = DataCtx.Current;
                return 1;
            }
        }
        [Command(Description = "Export Request")]
        [Subcommand("it", typeof(IterCmd))]
        class ExpCmd : JObjectCmd
        {
            [Command(Description = "Iterate")]
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
            DataCmd DataCmd { get; set; }
            protected override int OnExecute()
            {
                if (DataCmd.OnExecute() == 0) return 0;
                Object = DataCtx.Current.Request;
                if (!Iter) Dump();

                return base.OnExecute();
            }
            public ExpCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Query")]
        [Subcommand("cn", typeof(CondCmd))]
        class QryCmd : QueryCmd
        {
            [Command(Description = "Condition", ThrowOnUnexpectedArgument = false)]
            class CondCmd : ConditionCmd
            {
                protected override int OnExecute()
                {
                    if (QryCmd.OnExecute() == 0) return 0;
                    Request = DataCtx.Current.Request;

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
                if (DataCmd.OnExecute() == 0) return 0;
                Request = DataCtx.Current.Request;

                return base.OnExecute();
            }
            DataCmd DataCmd { get; set; }
            public QryCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Define")]
        class DefCmd : RequestCmd<AIS.DatabrowserRequest>
        {

            [Option("-n|--name", CommandOptionType.SingleValue, Description = "Table or View Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) TargetName { get; set; }
            [Option("-t|--type", CommandOptionType.SingleValue, Description = "Target Type")]
            [AllowedValues(new string[] { "table", "view" }, IgnoreCase = true)]
            protected (bool HasValue, string Parameter) TargetType { get; }
            [Option("-st|--serviceType", CommandOptionType.SingleValue, Description = "Service Type")]
            [AllowedValues(new string[] { "BROWSE", "COUNT", "AGGREGATION" }, IgnoreCase = true)]
            protected (bool HasValue, string Parameter) ServiceType { get; }
            DataCmd DataCmd { get; set; }
            protected virtual int OnExecute()
            {
                if (DataCmd.Id.HasValue && !DataCtx.Select(DataCmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Data Definition?", false))
                    {
                        PromptOptions();
                        DataCtx.Current = new DataCtx(DataCmd.Id.Parameter);
                        DataCtx.List.Add(DataCtx.Current);
                        Context = DataCtx.Current;
                    }
                }
                if (DataCmd.NullCtx) return 0;
                Request = DataCtx.Current.Request;
                var rq = DataCtx.Current.Request;
                if (TargetName.HasValue) rq.targetName =  TargetName.Parameter.ToUpper();
                rq.dataServiceType = ServiceType.HasValue ? ServiceType.Parameter.ToUpper() : rq.dataServiceType ?? "BROWSE";
                rq.targetType = TargetType.HasValue ? TargetType.Parameter.ToLower() : rq.targetType ?? "table";
                return 1;
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
                if (DataCmd.OnExecute() == 0) return 0;
                DataCtx.Current.Submit();
                return 1;
            }
            public SubCmd(DataCmd dataCmd)
            {
                DataCmd = dataCmd;
            }
        }
        [Command(Description = "Response")]
        [Subcommand("it", typeof(IterCmd))]
        class ResCmd : ResponseCmd<AIS.DatabrowserRequest>
        {
            [Command(Description = "Iterate")]
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
                Responses = DataCtx.Responses;
            }
        }
        protected bool NullCtx
        {
            get
            {
                if (DataCtx.Current is null)
                {
                    Error("No Data Context!");
                    return true;
                }
                return false;
            }
        }
        int OnExecute()
        {
            if (List) foreach (var c in DataCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !DataCtx.Select(Id.Parameter))
            {
                Error("Data Context '{0}' not found!", Id.Parameter);
                return 0;
            }
            if (NullCtx) return 0;
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
