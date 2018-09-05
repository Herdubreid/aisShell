using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    [Command("sv", Description = "AIS Server Context")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("c", typeof(ConCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class ServerCmd : BaseCmd
    {
        static string LastFileName { get; set; }
        [Command(Description = "Load Servers")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                var fname = FileName.Parameter + ".sctx";
                try
                {
                    using (StreamReader sr = File.OpenText(fname))
                    {
                        var data = (JArray)JsonConvert.DeserializeObject(sr.ReadToEnd());
                        if (data.Count > 0)
                        {
                            ServerCtx.List = data.ToObject<List<ServerCtx>>();
                            ServerCtx.Current = ServerCtx.List.First();
                            LastFileName = FileName.Parameter;
                        }
                        else Warning("Not data to load!");
                    }
                }
                catch (Exception e)
                {
                    Error(e.Message);
                }
                return 1;
            }
            public LoadCmd()
            {
                FileName = (false, LastFileName);
            }
        }
        [Command(Description = "Save Servers")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                var fname = FileName.Parameter + ".sctx";
                if (!File.Exists(fname) || Prompt.GetYesNo("Do you want to overwrite existing file?", false))
                {
                    try
                    {
                        using (StreamWriter sw = File.CreateText(fname))
                        {
                            sw.Write(JsonConvert.SerializeObject(ServerCtx.List));
                        }
                        LastFileName = FileName.Parameter;
                    }
                    catch (Exception e)
                    {
                        Error(e.Message);
                    }
                }
                return 1;
            }
            public SaveCmd()
            {
                FileName = (false, LastFileName);
            }
        }
        [Command(Description = "Definition")]
        public class DefCmd : BaseCmd
        {
            [Argument(0, Description = "Server Id")]
            [PromptOption]
            public (bool HasValue, string Parameter) Id { get; private set; }
            [Option("-b|--baseUrl", CommandOptionType.SingleValue, Description = "Base Url")]
            [PromptOption]
            public (bool HasValue, string Parameter) BaseUrl { get; private set; }
            int OnExecute()
            {
                var ctx = Id.HasValue ? ServerCtx.Select(Id.Parameter) : ServerCtx.Current;
                if (ctx is null)
                {
                    PromptOptions();
                    ServerCtx.Current = new ServerCtx(Id.Parameter, BaseUrl.Parameter);
                    ServerCtx.List.Add(ServerCtx.Current);
                }
                else
                {
                    ServerCtx.Current.Server.BaseUrl = BaseUrl.Parameter;
                }
                return 1;
            }
            public DefCmd(ServerCtx serverCtx)
            {
                if (serverCtx != null)
                {
                    Id = (false, serverCtx.Id);
                    var rq = serverCtx.Server;
                    BaseUrl = (false, rq.BaseUrl);
                }
            }
            public DefCmd()
            { }
        }
        [Command(Description = "Connect")]
        public class ConCmd : BaseCmd
        {
            [Argument(0, Description = "Server Id")]
            public (bool HasValue, string Parameter) Id { get; }
            [Option("-d|--device", CommandOptionType.SingleValue, Description = "Device")]
            [PromptOption]
            public (bool HasValue, string Parameter) Device { get; private set; }
            [Option("-u|--user", CommandOptionType.SingleValue, Description = "User name")]
            [PromptOption]
            public (bool HasValue, string Parameter) User { get; private set; }
            [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
            [PromptOption(false, PromptType.Password)]
            public (bool HasValue, string Parameter) Password { get; private set; }
            public delegate bool Authenticate();
            private int OnExecute()
            {
                var ctx = Id.HasValue ? ServerCtx.Select(Id.Parameter) : ServerCtx.Current;
                if (ctx is null)
                {
                    Warning("{0} Server Context Invalid!", Id.HasValue ? Id.Parameter : "[Current]");
                }
                else
                {
                    PromptOptions();
                    var rq = ServerCtx.Current.Server.AuthRequest;
                    rq.deviceName = Device.Parameter;
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
                        Warning("\nSignon failed!");
                    }
                }
                return 1;
            }
            public ConCmd()
            {
                var auth = ServerCtx.Current?.Server.AuthRequest;
                if (auth != null)
                {
                    Device = (false, auth.deviceName);
                    User = (false, auth.username);
                }
            }
        }
        private int OnExecute()
        {
            foreach (var ctx in ServerCtx.List)
            {
                var cmd = new DefCmd(ctx);
                cmd.Display(OutFile, false);
                OutputLine(OutFile);
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
            Commands.Remove(Commands.Single<Cmd>(c => c.Type == typeof(ServerCmd)));
        }
    }
}
