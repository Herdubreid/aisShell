using McMaster.Extensions.CommandLineUtils;
using System;

namespace Celin
{
    [Command("wl", Description = "Wachlist Context")]
    [Subcommand(typeof(ExpCmd))]
    [Subcommand(typeof(SaveCmd))]
    [Subcommand(typeof(LoadCmd))]
    [Subcommand(typeof(ResCmd))]
    [Subcommand(typeof(DefCmd))]
    [Subcommand(typeof(SubCmd))]
    public class WatchListCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Context Id")]
        protected (bool HasValue, string Parameter) Id { get; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        protected bool List { get; }
        [Command("exp", Description = "Export Request")]
        class ExpCmd : JObjectCmd
        {
            protected WatchListCmd Cmd { get; set; }
            protected override int OnExecute()
            {
                base.OnExecute();
                if (Cmd.OnExecute() == 0) return 0;
                Object = WatchListCtx.Current.Request;
                if (!Iter) Dump();

                return 1;
            }
            public ExpCmd(WatchListCmd cmd)
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
                PreferenceCtx.Save(FileName.Parameter + ".wctx");
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
        class ResCmd : ResponseCmd<AIS.WatchListRequest>
        {
            public ResCmd()
            {
                Responses = WatchListCtx.Responses;
            }
        }
        [Command("d", Description = "Watch List Definition")]
        class DefCmd : BaseCmd
        {
            WatchListCmd Cmd { get; }
            [Option("-fu|--forceUpdate", CommandOptionType.SingleValue, Description = "Force Update")]
            [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
            public (bool HasValue, string Parameter) ForceUpdate { get; set; }
            [Option("-sd|--setDirty", CommandOptionType.SingleValue, Description = "Set Dirty Only")]
            [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
            public (bool HasValue, string Parameter) SetDirtyOnly { get; set; }
            [Option("-wi|--watchListId", CommandOptionType.SingleValue, Description = "Watch List Id")]
            public (bool HasValue, string Parameter) WatchlistId { get; set; }
            [Option("-wo|--watchListObjectName", CommandOptionType.SingleValue, Description = "Watch List Object Name")]
            public (bool HasValue, string Parameter) WatchlistObjectName { get; set; }
            protected virtual int OnExecute()
            {
                if (Cmd.Id.HasValue && !WatchListCtx.Select(Cmd.Id.Parameter))
                {
                    if (Prompt.GetYesNo("New Watch List Definition?", false))
                    {
                        PromptOptions();
                        WatchListCtx.Current = new WatchListCtx(Cmd.Id.Parameter);
                        WatchListCtx.List.Add(WatchListCtx.Current);
                        Context = WatchListCtx.Current;
                    }
                    else return 0;
                }
                if (Cmd.NullCtx) return 0;

                var rq = WatchListCtx.Current.Request;
                rq.forceUpdate = ForceUpdate.HasValue ? ForceUpdate.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase) : rq.forceUpdate;
                rq.setDirtyOnly = SetDirtyOnly.HasValue ? SetDirtyOnly.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase) : rq.setDirtyOnly;
                rq.watchlistId = WatchlistId.HasValue ? WatchlistId.Parameter : rq.watchlistId;
                rq.watchlistObjectName = WatchlistObjectName.HasValue ? WatchlistObjectName.Parameter : rq.watchlistObjectName;

                return 1;
            }
            public DefCmd(WatchListCmd cmd)
            {
                Cmd = cmd;
            }
        }
        [Command("s", Description = "Submit Request")]
        class SubCmd : BaseCmd
        {
            WatchListCmd Cmd { get; }
            int OnExecute()
            {
                if (Cmd.OnExecute() == 0) return 0;
                WatchListCtx.Current.Submit();
                return 1;
            }
            public SubCmd(WatchListCmd cmd)
            {
                Cmd = cmd;
            }
        }
        protected bool NullCtx
        {
            get
            {
                if (WatchListCtx.Current is null)
                {
                    Error("No Watchlist Context!");
                    return true;
                }
                return false;
            }
        }
        protected int OnExecute()
        {
            if (List) foreach (var c in WatchListCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !WatchListCtx.Select(Id.Parameter))
            {
                Error("Watch List Context {0} not found!", Id.Parameter);
                return 0;
            }
            if (NullCtx) return 0;
            Context = WatchListCtx.Current;

            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd
            {
                Type = typeof(WatchListCmd),
                Execute = CommandLineApplication.Execute<WatchListCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(WatchListCmd)));
        }
    }
}
