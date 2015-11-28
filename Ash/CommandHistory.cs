using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ash
{
    partial class Shell
    {
        class CommandHistory
        {
            private string[] _list;
            private int _size;
            private int _cap;
            private int _rear;
            private RangeBoundNumber<int> _range = new RangeBoundNumber<int>(0, 1, 1);
            private RangeBoundNumber<int> _iterator = new RangeBoundNumber<int>(0, 1, 1, true);
            public CommandHistory(int cap)
            {
                _size = _rear = 0;
                _cap = cap;
                _list = new string[cap];
            }

            public string Previous()
            {
                return (_range-- != 0) ? _list[--_iterator] : string.Empty;
                /*
                string ret = (_range-- != 0) ? _list[--_iterator] : string.Empty;
                Console.WriteLine("_rear:{0} _size:{1} _iterator:{2} _range:{3}", _rear, _size, _iterator, _range);
                return ret;
                */
            }
            public string Next()
            {
                return (_range++ != _size) ? _list[++_iterator] : string.Empty;
                /*
                string ret = (_range++ != _size) ? _list[++_iterator] : string.Empty;
                Console.WriteLine("_rear:{0} _size:{1} _iterator:{2} _range:{3}", _rear, _size, _iterator, _range);
                return ret;
                */
            }

            public string this[int idx]
            {
                get
                {
                    return _list[idx];
                }
            }

            public void Enlist(string saveme)
            {
                if (_rear > 0 && _list[_rear - 1] == saveme) return;

                _list[_rear] = saveme;
                _range.SetUBound((_size < _cap) ? ++_size : _cap);
                _range.Assign(_size);
                _iterator.SetUBound(_size);
                _iterator.Assign(_rear = ++_rear % _cap);
            }
            public string[] List() { return _list; }
            public int Size() { return _size; }
        }
    }
}
