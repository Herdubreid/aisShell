using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
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
        public void Display(ValueTuple<bool, string> outFile, bool full)
        {
            foreach (var e in GetType().GetProperties())
            {
                var optionAttribute = (OptionAttribute)Attribute.GetCustomAttribute(e, typeof(OptionAttribute));
                var argumentAttribute = (ArgumentAttribute)Attribute.GetCustomAttribute(e, typeof(ArgumentAttribute));
                var description = optionAttribute is null ? argumentAttribute?.Description : optionAttribute.Description;
                if (description != null)
                {
                    if (full) Output(outFile, String.Format("  {0, -20}->", description));
                    var parameter = e.PropertyType == typeof(ValueTuple<bool, string>) ? ((ValueTuple<bool, string>)e.GetValue(this)).Item2
                        : e.PropertyType == typeof(ValueTuple<bool, int>) ? ((ValueTuple<bool, int>)e.GetValue(this)).Item2.ToString() : e.GetValue(this).ToString();
                    Output(outFile, parameter);
                    if (full) OutputLine(outFile);
                    else if (parameter != null) Output(outFile, " ");
                }
            }
            if (!full) OutputLine(outFile);
        }
        public void Display(bool full)
        {
            Display(OutFile, full);
        }
        protected static string DefaultOutFile { get; set; } = null;
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
