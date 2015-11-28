using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ash
{
    partial class Shell
    {
        private void OnCommandHistory()
        {
            string[] history = _history.List();
            for (int i = 0; i < _history.Size(); i++)
            {
                Console.WriteLine("{0}\t{1}", i+1,  history[i]);
            }
        }

        private void OnCommandDefault()
        {
            Console.WriteLine("Command not found");
        }

        private void OnCommandExit()
        {
            Environment.Exit(0);
        }

        private void OnCommandClear()
        {
            Console.Clear();
        }
    }
}
