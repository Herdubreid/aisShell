using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Console = Colorful.Console;
namespace Celin
{
    public delegate int CmdDel(string[] cmd);
    public class Cmd
    {
        public Type Type { get; set; }
        public CmdDel Execute { get; set; }
    }

    public enum PromptType
    {
        String,
        Bool,
        Int,
        Password
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PromptOptionAttribute : Attribute
    {
        public bool AllowBlank { get; set; }
        public PromptType PromptType { get; set; }
        public PromptOptionAttribute(bool allowBlank = false, PromptType promptType = PromptType.String)
        {
            AllowBlank = allowBlank;
            PromptType = promptType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SuppressDisplayAttribute : Attribute { }

    public class BaseCmd
    {
        [Option("-of|--outFile", CommandOptionType.SingleValue, Description = "Write Result to File")]
        [SuppressDisplay]
        public (bool HasValue, string Parameter) OutFile { get; set; }
        public void Output(ValueTuple<bool, string> outFile, string result)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + ".txt")) sw.Write(result);
            else Console.Write(result);
        }
        public void Output(string result)
        {
            if (OutFile.Item1) using (StreamWriter sw = File.AppendText(OutFile.Item2 + ".txt")) sw.Write(result);
            else Console.Write(result);
        }
        public void OutputLine(ValueTuple<bool, string> outFile, string result = null)
        {
            if (outFile.Item1) using (StreamWriter sw = File.AppendText(outFile.Item2 + ".txt")) sw.WriteLine(result);
            else Console.WriteLine(result);
        }
        public void OutputLine(string result = null)
        {
            if (OutFile.Item1) using (StreamWriter sw = File.AppendText(OutFile.Item2 + ".txt")) sw.WriteLine(result);
            else Console.WriteLine(result);
        }
        public static List<Cmd> Commands { get; } = new List<Cmd>();
        public static string PromptTx
        {
            get
            {
                return String.Format("[{0}][{1}] $ ",
                                     ServerCtx.Current != null ?
                                     ServerCtx.Current.Server.AuthResponse != null ?
                                     ServerCtx.Current.Id + ":" + ServerCtx.Current.Server.AuthResponse.username :
                                     ServerCtx.Current.Id : "",
                                     FormCtx.Current != null ?
                                     FormCtx.Current.Id : "");
            }
        }
        public static void Success(string fmt, params object[] args)
        {
            Console.WriteLine(String.Format(fmt, args), Color.Green);
        }
        public static void Warning(string fmt, params object[] args)
        {
            Console.WriteLine(String.Format(fmt, args), Color.Yellow);
        }
        public static void Error(string fmt, params object[] args)
        {
            Console.WriteLine(String.Format(fmt, args), Color.Red);
        }
        public static Cmd Find(string cmd)
        {
            return Commands.Find(c =>
            {
                var attribute = (CommandAttribute)Attribute.GetCustomAttribute(c.Type, typeof(CommandAttribute));
                return String.Equals(cmd, attribute.Name, StringComparison.OrdinalIgnoreCase);
            });
        }
        public void Display(ValueTuple<bool, string> outFile, bool full)
        {
            foreach (var e in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(e, typeof(SuppressDisplayAttribute)) is null)
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
            }
        }
        public void PromptOptions()
        {
            foreach (var e in GetType().GetProperties())
            {
                var promptOption = (PromptOptionAttribute)Attribute.GetCustomAttribute(e, typeof(PromptOptionAttribute));
                if (promptOption != null)
                {
                    var optionAttribute = (OptionAttribute)Attribute.GetCustomAttribute(e, typeof(OptionAttribute));
                    var argumentAttribute = (ArgumentAttribute)Attribute.GetCustomAttribute(e, typeof(ArgumentAttribute));
                    var description = optionAttribute is null ? argumentAttribute?.Description : optionAttribute.Description;
                    switch (promptOption.PromptType)
                    {
                        case PromptType.String:
                            var stringValue = ((bool, string))e.GetValue(this);
                            if (!stringValue.Item1)
                            {
                                do
                                {
                                    stringValue.Item2 = Prompt.GetString(description + ':', stringValue.Item2);
                                    stringValue.Item1 = stringValue.Item2 != null;
                                }
                                while (!promptOption.AllowBlank && !stringValue.Item1);
                                e.SetValue(this, stringValue);
                            }
                            break;
                        case PromptType.Password:
                            var passwordValue = ((bool, string))e.GetValue(this);
                            if (!passwordValue.Item1)
                            {
                                do
                                {
                                    passwordValue.Item2 = Prompt.GetPassword(description + ':');
                                    passwordValue.Item1 = passwordValue.Item2 != null;
                                }
                                while (!promptOption.AllowBlank && !passwordValue.Item1);
                                e.SetValue(this, passwordValue);
                            }
                            break;
                    }
                }
            }
        }
    }
}