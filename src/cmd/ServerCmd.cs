﻿using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("sv", Description = "Server Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("c", typeof(ConCmd))]
    [Subcommand("lo", typeof(LogoutCmd))]
    [Subcommand("exp", typeof(ExpCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class ServerCmd : BaseCmd
    {
        [Option("-c|--context", CommandOptionType.SingleValue, Description = "Server Context")]
        protected (bool HasValue, string Parameter) Id { get; private set; }
        [Option("-l|--listContexts", CommandOptionType.NoValue, Description = "List Contexts")]
        bool List { get; }
        [Command(Description = "Export Servers")]
        class ExpCmd : JObjectCmd
        {
            ServerCmd ServerCmd { get; set; }
            protected override int OnExecute()
            {
                if (ServerCmd.OnExecute() == 0) return 0;
                Object = ServerCtx.Current.Server;
                Dump();

                return base.OnExecute();
            }
            public ExpCmd(ServerCmd serverCmd)
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
            [Option("-d|--device", CommandOptionType.SingleValue, Description = "Device")]
            protected (bool HasValue, string Parameter) Device { get; private set; }
            [Option("-rc|--requiredCapabilities", CommandOptionType.SingleValue, Description = "Required Capabilities")]
            protected (bool HasValue, string Parameter) RequiredCapabilities { get; private set; }
            ServerCmd ServerCmd { get; set; }
            protected int OnExecute()
            {
                if (ServerCmd.OnExecute() == 0 && ServerCmd.Id.HasValue)
                {
                    if (Prompt.GetYesNo("New Server Definition?", false))
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
                if (BaseUrl.HasValue) ServerCtx.Current.Server.BaseUrl = BaseUrl.Parameter;
                var rq = ServerCtx.Current.Server.AuthRequest;
                if (Device.HasValue) rq.deviceName = Device.Parameter;
                if (RequiredCapabilities.HasValue) rq.requiredCapabilities = RequiredCapabilities.Parameter;

                return 1;
            }
            public DefCmd(ServerCtx serverCtx)
            {
                var rq = serverCtx.Server;
                BaseUrl = (false, rq.BaseUrl);
                var auth = ServerCtx.Current?.Server.AuthRequest;
                if (auth != null)
                {
                    Device = (false, auth.deviceName);
                    RequiredCapabilities = (false, auth.requiredCapabilities);
                }
            }
            public DefCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
            }
        }
        [Command(Description = "Connect")]
        public class ConCmd : BaseCmd
        {
            [Option("-u|--user", CommandOptionType.SingleValue, Description = "User name")]
            [PromptOption]
            public (bool HasValue, string Parameter) User { get; private set; }
            [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
            [PromptOption(false, PromptType.Password)]
            public (bool HasValue, string Parameter) Password { get; set; }
            int OnExecute()
            {
                if (ServerCmd.OnExecute() == 0) return 0;
                PromptOptions();
                var rq = ServerCtx.Current.Server.AuthRequest;
                rq.username = User.Parameter;
                rq.password = Password.Parameter;
                Task<bool> t = new Task<bool>(ServerCtx.Current.Server.Authenticate);
                rq.password = "";
                t.Start();
                while (!t.IsCompleted)
                {
                    Thread.Sleep(500);
                    Console.Write('.');
                }
                if (t.Result)
                {
                    Success("\nSignon success!");
                }
                else
                {
                    Error("\nSignon failed!");
                }
                return 1;
            }
            ServerCmd ServerCmd { get; set; }
            public ConCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
                var auth = ServerCtx.Current?.Server.AuthRequest;
                if (auth != null)
                {
                    User = (false, auth.username);
                }
            }
        }
        [Command(Description = "Logout")]
        public class LogoutCmd : BaseCmd
        {
            int OnExecute()
            {
                if (ServerCmd.OnExecute() == 0) return 0;
                if (!Prompt.GetYesNo("Do you want to log out?", false)) return 0;
                var rq = new AIS.LogoutRequest();
                Task<bool> t = new Task<bool>(ServerCtx.Current.Server.Logout);
                t.Start();
                while (!t.IsCompleted)
                {
                    Thread.Sleep(500);
                    Console.Write('.');
                }
                if (t.Result)
                {
                    Success("\nLogged out successfully!");
                }
                else
                {
                    Error("\nLogout failed!");
                }

                return 1;
            }
            ServerCmd ServerCmd { get; set; }
            public LogoutCmd(ServerCmd serverCmd)
            {
                ServerCmd = serverCmd;
            }
        }
        public int OnExecute()
        {
            if (List) foreach (var c in ServerCtx.List) Console.WriteLine(c.Id);
            if (Id.HasValue && !ServerCtx.Select(Id.Parameter))
            {
                Error("Server Context '{0}' not found!", Id.Parameter);
                return 0;
            }
            if (ServerCtx.Current is null)
            {
                Error("No Server Context!");
                return 0;
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
