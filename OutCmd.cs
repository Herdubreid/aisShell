using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
namespace Celin
{
    public abstract class OutCmd : BaseCmd
    {
        [Option("-of|--outFile", CommandOptionType.SingleValue, Description = "Write Result to File")]
        protected (bool HasValue, string Parameter) OutFile { get; set; }
        public void Output(ValueTuple<bool, string> outFile, string result)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + ".txt")) sw.Write(result);
            else Console.Write(result);
        }
        public void Output(string result)
        {
            Output(OutFile, result);
        }
        public void OutputLine(ValueTuple<bool, string> outFile, string result = null)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + ".txt")) sw.WriteLine(result);
            else Console.WriteLine(result);
        }
        public void OutputLine(string result = null)
        {
            OutputLine(OutFile, result);
        }
        public void Export(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            OutputLine(json);
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
