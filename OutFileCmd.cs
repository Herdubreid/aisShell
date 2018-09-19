using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("out", Description = "Set Output File")]
    public class OutFileCmd : OutCmd
    {
        [Option("-c|--clear", CommandOptionType.NoValue, Description = "Clear Default Output File")]
        bool Clear { get; set; }
        protected override int OnExecute()
        {
            DefaultOutFile = Clear ? null : OutFile.HasValue ? OutFile.Parameter : null;
            Success(DefaultOutFile is null ? "No Default Output File" : String.Format("Output Default {0}.txt", DefaultOutFile));

            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(OutFileCmd),
                Execute = CommandLineApplication.Execute<OutFileCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Find(c => c.Type == typeof(OutFileCmd)));
        }
    }
}
