using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class OutCmd : BaseCmd
    {
        public static readonly string Extension = ".json";
        [Option("-of|--outFile", CommandOptionType.SingleValue, Description = "Write Result to File")]
        protected (bool HasValue, string Parameter) OutFile { get; set; }
        public void Output(ValueTuple<bool, string> outFile, string result)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + Extension)) sw.Write(result);
            else Console.Write(result);
        }
        public void Output(string result)
        {
            Output(OutFile, result);
        }
        public void OutputLine(ValueTuple<bool, string> outFile, string result = null)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + Extension)) sw.WriteLine(result);
            else Console.WriteLine(result);
        }
        public void OutputLine(string result = null)
        {
            OutputLine(OutFile, result);
        }
        public static string DefaultOutFile { get; set; } = null;
        protected virtual int OnExecute()
        {
            if (!OutFile.HasValue)
            {
                OutFile = (DefaultOutFile != null, DefaultOutFile);
            }
            return 1;
        }
    }
}
