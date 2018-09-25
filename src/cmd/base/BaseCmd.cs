using System;
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

    public class BaseCmd
    {
        public static List<Cmd> Commands { get; } = new List<Cmd>();
        public static IBaseCtx Context { get; set; }
        public static string PromptTx
        {
            get
            {
                var ctxFmt = " {0}:{1}";
                return String.Format("[{0}:{1}]{2} $ ",
                                     ServerCtx.Current?.Id,
                                     ServerCtx.Current?.Server.AuthResponse?.username,
                                     Context is null ? String.Empty : String.Format(ctxFmt, Context.Cmd, Context.Id));
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
        public void PromptOptions()
        {
            foreach (var e in GetType().GetProperties())
            {
                var promptOption = (PromptOptionAttribute)Attribute.GetCustomAttribute(e, typeof(PromptOptionAttribute));
                if (promptOption != null)
                {
                    var allowedValues = (AllowedValuesAttribute)Attribute.GetCustomAttribute(e, typeof(AllowedValuesAttribute));
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