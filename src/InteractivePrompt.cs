using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace Cintio
{
    public static class InteractivePrompt
    {
        public static string Prompt { get; set; } = "$";
        static int startingCursorLeft;
        static int startingCursorTop;
        static ConsoleKeyInfo key, lastKey;

        static bool Comment(string msg)
        {
            return Regex.Match(msg, @"^//").Success;
        }
        static void Output(string msg)
        {
            if (Comment(msg)) Console.Write(msg, Color.DarkGreen);
            else
            {
                Console.Write(msg);
            }
        }
        private static bool InputIsOnNewLine(List<char> input, int inputPosition)
        {
            return (inputPosition + Prompt.Length > Console.BufferWidth - 1);
        }
        private static int GetCurrentLineForInput(List<char> input, int inputPosition)
        {
            int currentLine = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == '\n')
                    currentLine += 1;
                if (i == inputPosition)
                    break;
            }
            return currentLine;
        }
        /// <summary>
        /// Gets the cursor position relative to the current line it is on
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputPosition"></param>
        /// <returns></returns>
        private static Tuple<int, int> GetCursorRelativePosition(List<char> input, int inputPosition)
        {
            int currentPos = 0;
            int currentLine = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == '\n')
                {
                    currentLine += 1;
                    currentPos = 0;
                }
                if (i == inputPosition)
                {
                    if (currentLine == 0)
                    {
                        currentPos += Prompt.Length;
                    }
                    break;
                }
                currentPos++;
            }
            return Tuple.Create(currentPos, currentLine);
        }
        private static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        private static void ClearLine(List<char> input, int inputPosition)
        {
            int cursorLeft = InputIsOnNewLine(input, inputPosition) ? 0 : Prompt.Length;
            Console.SetCursorPosition(Prompt.Length, startingCursorTop);
            Console.Write(new string(' ', input.Count + Prompt.Length));
        }

        /// <summary>
        /// A hacktastic way to scroll the buffer - WriteLine
        /// </summary>
        /// <param name="lines"></param>
        private static void ScrollBuffer(int lines = 0)
        {
            for (int i = 0; i <= lines; i++)
                Console.WriteLine("");
            Console.SetCursorPosition(0, Console.CursorTop - lines);
            startingCursorTop = Console.CursorTop - lines;
        }

        /// <summary>
        /// RewriteLine will rewrite every character in the input List, and given the inputPosition
        /// will determine whether or not to continue to the next line
        /// </summary>
        /// <param name="input">The input buffer</param>
        /// <param name="inputPosition">Current character position in the List</param>
        private static void RewriteLine(List<char> input, int inputPosition)
        {
            int cursorTop = 0;

            try
            {
                Console.SetCursorPosition(startingCursorLeft, startingCursorTop);
                var coords = GetCursorRelativePosition(input, inputPosition);
                cursorTop = startingCursorTop;
                int cursorLeft = 0;

                if (GetCurrentLineForInput(input, inputPosition) == 0)
                {
                    cursorTop += (inputPosition + Prompt.Length) / Console.BufferWidth;
                    cursorLeft = inputPosition + Prompt.Length;
                }
                else
                {
                    cursorTop += coords.Item2;
                    cursorLeft = coords.Item1 - 1;
                }

                // if the new vertical cursor position is going to exceed the buffer height (i.e., we are
                // at the bottom of console) then we need to scroll the buffer however much we are about to exceed by
                if (cursorTop >= Console.BufferHeight)
                {
                    ScrollBuffer(cursorTop - Console.BufferHeight + 1);
                    RewriteLine(input, inputPosition);
                    return;
                }

                Output(String.Concat(input));
                Console.SetCursorPosition(mod(cursorLeft, Console.BufferWidth), cursorTop);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static IEnumerable<string> GetMatch(List<string> s, string input)
        {
            s.Add(input);
            int direction = (key.Modifiers == ConsoleModifiers.Shift) ? -1 : 1;
            for (int i = -1; i < s.Count;)
            {
                direction = (key.Modifiers == ConsoleModifiers.Shift) ? -1 : 1;
                i = mod((i + direction), s.Count);

                if (Regex.IsMatch(s[i], ".*(?:" + input + ").*", RegexOptions.IgnoreCase))
                {
                    yield return s[i];
                }
            }
        }

        static Tuple<int, int> HandleMoveLeft(List<char> input, int inputPosition)
        {
            //var coords = GetCursorRelativePosition(input, inputPosition);
            //int cursorLeftPosition = coords.Item1;
            int cursorLeftPosition = inputPosition + Prompt.Length;
            int cursorTopPosition = Console.CursorTop;

            if (GetCurrentLineForInput(input, inputPosition) == 0)
                cursorLeftPosition = cursorLeftPosition % Console.BufferWidth;

            if (Console.CursorLeft == 0)
                cursorTopPosition = Console.CursorTop - 1;

            return Tuple.Create(cursorLeftPosition, cursorTopPosition);
        }

        static Tuple<int, int> HandleMoveRight(List<char> input, int inputPosition)
        {
            //var coords = GetCursorRelativePosition(input, inputPosition);
            //int cursorLeftPosition = coords.Item1;
            int cursorLeftPosition = inputPosition + Prompt.Length;
            int cursorTopPosition = Console.CursorTop;
            if (Console.CursorLeft + 1 >= Console.BufferWidth)
            {
                cursorLeftPosition = 0;
                cursorTopPosition = Console.CursorTop + 1;
            }
            return Tuple.Create(cursorLeftPosition % Console.BufferWidth, cursorTopPosition);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source) { action(item); }
        }

        /// <summary>
        /// Run will start an interactive prompt
        /// </summary>
        /// <param name="lambda">This func is provided for the user to handle the input.  Input is provided in both string and List&lt;char&gt;. A return response is provided as a string.</param>
        /// <param name="prompt">The prompt for the interactive shell</param>
        /// <param name="startupMsg">Startup msg to display to user</param>
        public static void Run(Func<string, List<char>, List<string>, bool> lambda, string startupMsg)
        {
            Console.WriteLine(startupMsg);
            List<List<char>> inputHistory = new List<List<char>>();
            bool resume = true;

            while (resume)
            {
                List<char> input = new List<char>();
                startingCursorLeft = Prompt.Length;
                startingCursorTop = Console.CursorTop;
                int inputPosition = 0;
                int inputHistoryPosition = inputHistory.Count;

                key = lastKey = new ConsoleKeyInfo();
                Console.Write(Prompt);
                do
                {
                    key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (inputPosition > 0)
                            {
                                var pos = HandleMoveLeft(input, --inputPosition);
                                Console.SetCursorPosition(pos.Item1, pos.Item2);
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (inputPosition < input.Count)
                            {
                                var pos = HandleMoveRight(input, ++inputPosition);
                                Console.SetCursorPosition(pos.Item1, pos.Item2);
                            }
                            break;
                        case ConsoleKey.Home:
                            inputPosition = 0;
                            Console.SetCursorPosition(Prompt.Length, startingCursorTop);
                            break;

                        case ConsoleKey.End:
                            inputPosition = input.Count;
                            var cursorLeft = 0;
                            int cursorTop = startingCursorTop;
                            if ((inputPosition + Prompt.Length) / Console.BufferWidth > 0)
                            {
                                cursorTop += (inputPosition + Prompt.Length) / Console.BufferWidth;
                                cursorLeft = (inputPosition + Prompt.Length) % Console.BufferWidth;
                            }
                            Console.SetCursorPosition(cursorLeft, cursorTop);
                            break;


                        case ConsoleKey.Delete:
                            if (inputPosition < input.Count)
                            {
                                input.RemoveAt(inputPosition);
                                //ClearLine(input, inputPosition);
                                RewriteLine(input, inputPosition);
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (inputHistoryPosition > 0)
                            {
                                inputHistoryPosition -= 1;
                                ClearLine(input, inputPosition);

                                // ToList() so we make a copy and don't use the reference in the list
                                input = inputHistory[inputHistoryPosition].ToList();
                                RewriteLine(input, input.Count);
                                inputPosition = input.Count;
                            }
                            break;


                        case ConsoleKey.DownArrow:
                            if (inputHistoryPosition < inputHistory.Count - 1)
                            {
                                inputHistoryPosition += 1;
                                ClearLine(input, inputPosition);

                                // ToList() so we make a copy and don't use the reference in the list
                                input = inputHistory[inputHistoryPosition].ToList();
                                RewriteLine(input, input.Count);
                                inputPosition = input.Count;
                            }
                            else
                            {
                                inputHistoryPosition = inputHistory.Count;
                                ClearLine(input, inputPosition);
                                Console.SetCursorPosition(Prompt.Length, Console.CursorTop);
                                input = new List<char>();
                                inputPosition = 0;
                            }
                            break;
                        case ConsoleKey.Backspace:
                            if (inputPosition > 0)
                            {
                                inputPosition--;
                                input.RemoveAt(inputPosition);
                                ClearLine(input, inputPosition);
                                RewriteLine(input, inputPosition);
                            }
                            break;

                        case ConsoleKey.Enter:
                            break;

                        default:
                            input.Insert(inputPosition++, key.KeyChar);
                            RewriteLine(input, inputPosition);
                            break;
                    }

                    lastKey = key;
                } while (key.Key != ConsoleKey.Enter);

                var cmd = string.Concat(input).Trim();
                if (Comment(cmd)) cmd = "";
                else if (!inputHistory.Contains(input))
                    inputHistory.Add(input);

                Console.WriteLine();

                resume = lambda(cmd, input, new List<string>(cmd.Split(' ')));
            }
        }
    }

}