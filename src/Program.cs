﻿using System;
using System.Linq;
using System.Collections.Generic;
using Cintio;
namespace Celin
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ServerCmd.AddCmd();
            FormCmd.AddCmd();
            StackFormCmd.AddCmd();
            DataCmd.AddCmd();
            OutFileCmd.AddCmd();
            HelpCmd.AddCmd();
            ClearCmd.AddCmd();
            QuitCmd.AddCmd();
            var startupMsg = "Celin's AIS Command Shell";
            List<string> completionList = new List<string>();
            InteractivePrompt.Prompt = BaseCmd.PromptTx;
            InteractivePrompt.Run(
                ((strCmd, listCmd, completion) =>
                {
                    var resume = true;
                    if (strCmd.Length > 0)
                    {
                        var command = BaseCmd.Find(completion[0]);
                        if (command != null)
                        {
                            resume = -1 != command.Execute(completion.Skip(1).ToArray());
                        }
                        else
                        {
                            command = BaseCmd.Context is null ? null : BaseCmd.Find(BaseCmd.Context.Cmd);
                            if (command != null)
                            {
                                command.Execute(completion.ToArray());
                            }
                            else
                            {
                                Console.WriteLine("{0}: command not found! Type 'help' for available commands.", completion[0]);
                            }
                        }
                    }
                    InteractivePrompt.Prompt = BaseCmd.PromptTx;
                    return resume;
                }), startupMsg, completionList);
        }
    }
}