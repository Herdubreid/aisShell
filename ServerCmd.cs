using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("sv", Description = "AIS Server Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("c", typeof(ConCmd))]
    [Subcommand("l", typeof(ListCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class ServerCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Server Context")]
        public (bool HasValue, string Parameter) Id { get; private set; }
        [Command(Description = "List Servers")]
        class ListCmd : BaseCmd
        {
            [Option("-a|--all", CommandOptionType.NoValue, Description = "List All")]
            bool All { get; }
            [Option("-l|--long", CommandOptionType.NoValue, Description = "Long Format")]
            bool Long { get; }
            ServerCmd ServerCmd { get; set; }
            void Show(ServerCtx serverCtx)
            {
                var cmd = new DefCmd(serverCtx);
                OutputLine(OutFile, String.Format("Server Context {0}", serverCtx.Id));
                cmd.Display(OutFile, Long);
            }
            int OnExecute()
            {
                ServerCmd.OnExecute();
                if (!All && ServerCtx.Current != null) Show(ServerCtx.Current);
                if (All) foreach (var ctx in ServerCtx.List) Show(ctx);
                return 1;
            }
            public ListCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
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
                ServerCtx.Load(FileName.Parameter + ".sctx");
                return 1;
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
                ServerCtx.Save(FileName.Parameter + ".sctx");
                return 1;
            }
        }
        [Command(Description = "Define")]
        public class DefCmd : BaseCmd
        {
            [Option("-b|--baseUrl", CommandOptionType.SingleValue, Description = "Base Url")]
            [PromptOption]
            public (bool HasValue, string Parameter) BaseUrl { get; private set; }
            ServerCmd ServerCmd { get; set; }
            int OnExecute()
            {
                if (ServerCmd.OnExecute() == 0 && ServerCmd.Id.HasValue)
                {
                    if (Prompt.GetYesNo("New Server Definition?", true))
                    {
                        PromptOptions();
                        ServerCtx.Current = new ServerCtx(ServerCmd.Id.Parameter, BaseUrl.Parameter);
                        ServerCtx.List.Add(ServerCtx.Current);
                    }
                }
                if (ServerCtx.Current is null)
                {
                    Error("No Server Context!");
                    return 1;
                }
                ServerCtx.Current.Server.BaseUrl = BaseUrl.Parameter;
                return 1;
            }
            public DefCmd(ServerCtx serverCtx)
            {
                var rq = serverCtx.Server;
                BaseUrl = (false, rq.BaseUrl);
            }
            public DefCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
            }
        }
        [Command(Description = "Connect")]
        public class ConCmd : BaseCmd
        {
            [Option("-d|--device", CommandOptionType.SingleValue, Description = "Device")]
            public (bool HasValue, string Parameter) Device { get; private set; }
            [Option("-rc|--requiredCapabilities", CommandOptionType.SingleValue, Description = "Required Capabilities")]
            public (bool HasValue, string Parameter) RequiredCapabilities { get; private set; }
            [Option("-u|--user", CommandOptionType.SingleValue, Description = "User name")]
            [PromptOption]
            public (bool HasValue, string Parameter) User { get; private set; }
            [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
            [PromptOption(false, PromptType.Password)]
            public (bool HasValue, string Parameter) Password { get; private set; }
            public delegate bool Authenticate();
            ServerCmd ServerCmd { get; set; }
            int OnExecute()
            {
                ServerCmd.OnExecute();
                if (ServerCtx.Current is null)
                {
                    Error("No Server Context!");
                }
                else
                {
                    PromptOptions();
                    var rq = ServerCtx.Current.Server.AuthRequest;
                    rq.deviceName = Device.HasValue ? Device.Parameter : rq.deviceName;
                    rq.requiredCapabilities = RequiredCapabilities.HasValue ?
                        RequiredCapabilities.Parameter :
                        rq.requiredCapabilities;
                    rq.username = User.Parameter;
                    rq.password = Password.Parameter;
                    Task<bool> t = new Task<bool>(ServerCtx.Current.Server.Authenticate);
                    t.Start();
                    while (!t.IsCompleted)
                    {
                        Thread.Sleep(500);
                        Console.Write('.');
                    }
                    if (t.Result)
                    {
                        Success("\nSignon success");
                    }
                    else
                    {
                        Error("\nSignon failed!");
                    }
                }
                return 1;
            }
            public ConCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
                var auth = ServerCtx.Current?.Server.AuthRequest;
                if (auth != null)
                {
                    Device = (false, auth.deviceName);
                    RequiredCapabilities = (false, auth.requiredCapabilities);
                    User = (false, auth.username);
                }
            }
        }
        public int OnExecute()
        {
            if (Id.HasValue)
            {
                return ServerCtx.Select(Id.Parameter) ? 1 : 0;
            }
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(ServerCmd),
                Execute = CommandLineApplication.Execute<ServerCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(ServerCmd)));
        }
    }
}
