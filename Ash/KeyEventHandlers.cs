using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ash
{
    partial class Shell
    {
        private void OnEventRightArrow(ConsoleKeyInfo cki) { throw new NotImplementedException(); }
        private void OnEventUpArrow(ConsoleKeyInfo cki) { Print(_history.Previous()); }
        private void OnEventDownArrow(ConsoleKeyInfo cki) { Print(_history.Next()); }
        private void OnEventLeftArrow(ConsoleKeyInfo cki) { throw new NotImplementedException(); }

        private void OnEventDefault(ConsoleKeyInfo cki)
        {
            Console.Write(cki.KeyChar);
            _input += cki.KeyChar;
        }

        private void OnEventEnter(ConsoleKeyInfo cki)
        {
            Console.WriteLine("");
            if (_input != string.Empty) HandleInput(_input);
            _input = string.Empty;
            Console.Write("ash>");
        }

        private void OnEventBackspace(ConsoleKeyInfo cki)
        {
            if (_input.Length >= 1) Print(_input.Substring(0, _input.Length - 1));
        }
    }
}
