using System;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("out", Description = "Set Output File")]
    public class OutFileCmd : BaseCmd
    {
        [Argument(0, Description = "Default Output File")]
        (bool HasValue, string Parameter) OutFile { get; }
        [Option("-c|--clear", CommandOptionType.NoValue, Description = "Clear Default Output File")]
        bool Clear { get; set; }
        protected int OnExecute()
        {
            OutCmd.DefaultOutFile = Clear ? null : OutFile.HasValue ? OutFile.Parameter : null;
            Success(OutCmd.DefaultOutFile is null ? "No Default Output File" : string.Format("Output Default {0}{1}", OutCmd.DefaultOutFile, OutCmd.Extension));

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
