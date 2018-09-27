using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    [Command("clear", Description = "Clear Screen")]
    public class ClearCmd : BaseCmd
    {
        int OnExecute()
        {
            Console.Clear();
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(ClearCmd),
                Execute = CommandLineApplication.Execute<ClearCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Single<Cmd>(c => c.Type == typeof(ClearCmd)));
        }
    }
    [Command("quit", Description = "Quit Shell")]
    public class QuitCmd : BaseCmd
    {
        int OnExecute()
        {
            if (Prompt.GetYesNo("Quit Shell?", false))
            {
                return -1;
            }
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(QuitCmd),
                Execute = CommandLineApplication.Execute<QuitCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Single<Cmd>(c => c.Type == typeof(QuitCmd)));
        }
    }
    [Command("help", Description = "Show Available Commands")]
    public class HelpCmd : BaseCmd
    {
        void DisplayCmd(BaseCmd cmd, ref int spaces)
        {
        }
        int OnExecute()
        {
            Console.WriteLine("Usage: [command] [options]\n");
            Console.WriteLine("Commands:");
            Commands.ForEach(c =>
            {
                var attribute = (CommandAttribute)Attribute.GetCustomAttribute(c.Type, typeof(CommandAttribute));
                Console.WriteLine("  {0, -20} {1}", attribute.Name, attribute.Description);

            });
            Console.WriteLine("\nUse [command] [-?|-h|--help] to get command help\n");
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(HelpCmd),
                Execute = CommandLineApplication.Execute<HelpCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Single<Cmd>(c => c.Type == typeof(HelpCmd)));
        }
    }
}
