using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Ash
{
    class EntryPoint
    {

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void Main(string[] args)
        {


            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UEEHandler);

            RangeBoundNumber<double> rbn = new RangeBoundNumber<double>(10.0, 20.0, .1, true);
            rbn.Show();
            rbn += 9.9;
            rbn.Show();
            rbn += .2;
            rbn.Show();
            return; 


            Shell shell = new Shell(args);
            shell.IncarnateShell();
            Console.WriteLine("Bye");
        }

        private static void UEEHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine(string.Format("Unhandled Exception Event caught : " + e.Message + "," + e.ToString() + "\n" + e.StackTrace.ToString()));
            Console.WriteLine(string.Format("Runtime terminating: {0}, {1}", args.IsTerminating, args.ToString()));
        }
    }

      partial class Shell
    {
        private static string _prompt = "wsh>";
        private Dictionary<string, Action<ConsoleKeyInfo>> KeyEventActionMap;
        private Dictionary<string, Action> CommandActionMap;
        //private bool flag_debug;
        private string _input;
        private CommandHistory _history;

        private void Print(string input)
        {
            if (input != null /*&& input != string.Empty*/)
            {
                int cursorCol = (input.Length > _input.Length) ? Console.CursorLeft + (input.Length - _input.Length) : Console.CursorLeft - (_input.Length - input.Length);
                int oldLength = _input.Length;
                int extraRows = oldLength / 80;

                _input = input;
                Console.CursorLeft = _prompt.Length;
                Console.CursorTop = Console.CursorTop - extraRows;
                string pad = (oldLength > input.Length) ? new string(' ', oldLength - input.Length) : string.Empty;
                Console.Write(input + pad);
                //Console.CursorLeft = cursorCol;
                Console.CursorLeft = (input).Length + _prompt.Length;
            }
        }

        public Shell(string[] args)
        {
            _input = string.Empty;
            _history = new CommandHistory(10);
            KeyEventActionMap = new Dictionary<string, Action<ConsoleKeyInfo>>();
            CommandActionMap = new Dictionary<string, Action>();
            KeyEventActionMap.Add("default", OnEventDefault);
            KeyEventActionMap.Add("RightArrow", OnEventRightArrow);
            KeyEventActionMap.Add("LeftArrow", OnEventLeftArrow);
            KeyEventActionMap.Add("UpArrow", OnEventUpArrow);
            KeyEventActionMap.Add("DownArrow", OnEventDownArrow);
            KeyEventActionMap.Add("Enter", OnEventEnter);
            KeyEventActionMap.Add("Backspace", OnEventBackspace);

            CommandActionMap.Add("history", OnCommandHistory);
            CommandActionMap.Add("default", OnCommandDefault);
            CommandActionMap.Add("clear", OnCommandClear);
            CommandActionMap.Add("exit", OnCommandExit);
            CommandActionMap.Add("tata", OnCommandExit);
            CommandActionMap.Add("bye", OnCommandExit);
            CommandActionMap.Add("quit", OnCommandExit);
            
            // set flags
        }

        private void HandleInput(string input)
        {
            if (CommandActionMap.ContainsKey(input))
                CommandActionMap[input]();
            else if (input.StartsWith("!"))
            {
                try
                {
                    HandleInput(_history[Int32.Parse(input.Substring(1)) - 1]);
                    return;
                }
                catch(Exception e)
                {
                    switch (e.GetType().ToString())
                    {
                        case "System.FormatException":
                        case "System.OverflowException":
                        case "System.ArgumentNullException":
                        case "System.IndexOutOfRangeException":
                            Console.WriteLine("Event not found");
                            break;
                        default:
                            Console.WriteLine("Exception:" + e.Message);
                            break;
                    }

                }
            }
            else
                CommandActionMap["default"]();
            _history.Enlist(input);

        }
        public void IncarnateShell()
        {
            ConsoleKeyInfo keyInfo;
            /*
            bool isCtrl = false;
            bool isAlt = false;
            bool isShift = false;
            */
            string input = string.Empty;

            Console.Write(_prompt);
            do
            {
                keyInfo = Console.ReadKey(true);
                /*
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control) isCtrl = true;
                if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt) isAlt = true;
                if ((keyInfo.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift) isShift = true;
                */
                if (KeyEventActionMap.ContainsKey(keyInfo.Key.ToString()))
                    KeyEventActionMap[keyInfo.Key.ToString()](keyInfo);
                else
                    KeyEventActionMap["default"](keyInfo);

            } while (!(((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control) && keyInfo.Key.ToString().ToLower() == "c"));
        }
    }

}
