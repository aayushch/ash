using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ash
{ 
    // Known Issues: 
    // 1. Circular ranges with quanta > 1 do not recoil properly : Fixed Nov 6th 2015
    // 2. No overloads for +, -, *, /, % yet. : Fixed Nov 7th 2015
    // 3. Implement increment/decrement thresholding.
    public class RangeBoundNumber<T>
    {
        private T _data;
        private T _lbound;
        private T _ubound;
        private T _step;
        private T _quanta;
        private T _quantainv; // experimental
        private T _precision; // experimantal
        private ulong _rsize;
        private bool _circular;
        private static HashSet<Type> AllowedTypes = new HashSet<Type>
        {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double)
        };

        public void Show()
        {
            Console.WriteLine("Range: {0} - {1} _data:{2} _step:{3} _quanta:{4} _quantainv:{5} _precision:{6} _rsize:{7} _circular:{8}", 
                                 _lbound, _ubound, _data, _step, _quanta, _quantainv, _precision, _rsize, _circular.ToString());
        }

        private RangeBoundNumber(RangeBoundNumber<T> that)
        {
            _data = that._data;
            _lbound = that._lbound;
            _ubound = that._ubound;
            _step = that._step;
            _circular = that._circular;
            _quanta = that._quanta;
            _quantainv = that._quantainv;
            _precision = that._precision;
            _rsize = that._rsize;
        }

        public RangeBoundNumber(T lbound, T ubound, T step, bool circular = false)
        {
            if (!AllowedTypes.Contains((typeof(T)))) throw new ArgumentException("Type not supported.");
            if ((dynamic)lbound > (dynamic)ubound) throw new ArgumentException("Range is invalid.");
            if ((dynamic)step < 0) throw new NotImplementedException();

            _lbound = lbound;
            _ubound = ubound;
            _step = step;
            _circular = circular;
            _data = _lbound;
            AdjustQuanta();
        }

        private void AdjustQuanta()
        {
            // Calculate the total valid values that can reside in the range.
            // We use it to modulus the addend and the subtrahend to optimize + and - operations.
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            {
                string[] lbsplit = _lbound.ToString().Split('.');
                string[] ubsplit = _ubound.ToString().Split('.');
                string[] qtsplit = _step.ToString().Split('.');

                long lBoundPrec = (lbsplit.Length > 1) ? lbsplit[1].Length : 0;
                long uBoundPrec = (ubsplit.Length > 1) ? ubsplit[1].Length : 0;
                long quantaPrec = (qtsplit.Length > 1) ? qtsplit[1].Length : 0;
                _precision = (dynamic) ((lBoundPrec > uBoundPrec) ? (lBoundPrec > quantaPrec) ? lBoundPrec : quantaPrec : (uBoundPrec > quantaPrec) ? uBoundPrec : quantaPrec);
                _quanta = (dynamic) 1 / Math.Pow(10, (dynamic)_precision);
                _quantainv = Math.Pow(10, (dynamic)_precision);
            }
            else
            {
                _quanta = (dynamic)1;
                _quantainv = (dynamic)1;
                _precision = (dynamic)0;
            }

            // Ranges are inclusive, so add an extra 1.
            _rsize = (ulong)(((dynamic)_ubound - _lbound) / _quanta) + 1;
        }

        public static implicit operator sbyte   (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator byte    (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator char    (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator short   (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator ushort  (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator int     (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator uint    (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator long    (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator ulong   (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator float   (RangeBoundNumber<T> box) { return (dynamic)box._data; }
        public static implicit operator double  (RangeBoundNumber<T> box) { return (dynamic)box._data; }

        /*
        // Note that we do not overload the implicit conversion operations from supported types to RangeBoundNumber<T>.
        // The reason for this is, that during conversion we only have the data but not the ranges and the quanta which 
        // are required parameter for our class. C# does not allow us to overload the = operator and hence we cannot copy
        // those values from the assignee. Hence, an Assign() method is exposed to perform assignment operation.
        
            public static implicit operator RangeBoundNumber<T> (type) { return new RangeBoundNumber<typeof (type)> (0,0,0); }
        */

        public static bool operator ==(RangeBoundNumber<T> val, T data) { return ((dynamic)val._data == data); }
        public static bool operator !=(RangeBoundNumber<T> val, T data) { return ((dynamic)val._data != data); }
        public static bool operator <=(RangeBoundNumber<T> val, T data) { return ((dynamic)val._data <= data); }
        public static bool operator >=(RangeBoundNumber<T> val, T data) { return ((dynamic)val._data >= data); }
        public static bool operator < (RangeBoundNumber<T> val, T data) { return ((dynamic)val._data < data); }
        public static bool operator > (RangeBoundNumber<T> val, T data) { return ((dynamic)val._data > data); }
        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return new string(_data.ToString().ToCharArray()); }

        public void SetLBound(T lbound)
        {
            _lbound = lbound;
            AdjustQuanta();
        }

        public void SetUBound(T ubound)
        {
            _ubound = ubound;
            AdjustQuanta();
        }

        public void SetStep(T step)
        {
            _step = step;
            AdjustQuanta();
        }

        public static RangeBoundNumber<T> operator ++(RangeBoundNumber<T> val)
        {
            return val + val._step;
            /*
            RangeBoundNumber<T> inc = new RangeBoundNumber<T>(val);
            inc._data = ((dynamic)inc._lbound == inc._ubound || (dynamic)inc._data < inc._ubound) ? (dynamic)inc._data + inc._quanta : (inc._circular) ? inc._lbound : inc._ubound;
            return inc;
            */
        }

        public static RangeBoundNumber<T> operator --(RangeBoundNumber<T> val)
        {
            return val - val._step;
            /*
            RangeBoundNumber<T> dec = new RangeBoundNumber<T>(val);
            dec._data = ((dynamic)dec._lbound == dec._ubound || (dynamic)dec._data > dec._lbound) ? (dynamic)dec._data - dec._step : (dec._circular) ? dec._ubound : dec._lbound;
            return dec;
            */
        }

        public static RangeBoundNumber<T> operator +(RangeBoundNumber<T> val, T data)
        {
            RangeBoundNumber<T> inc = new RangeBoundNumber<T>(val);
            T addend = (((dynamic)data / inc._quanta) % inc._rsize) * inc._quanta;
            inc._data = (((dynamic)inc._ubound - inc._data) < addend) ? (!inc._circular) ? inc._ubound : inc._lbound + (addend - ((dynamic)inc._ubound - inc._data)) - 1 : (dynamic)inc._data + addend;
            return inc;

            /*
            if (((dynamic)inc._ubound - inc._data) < addend)
            {
                if (!inc._circular)
                    inc._data = inc._ubound;
                else
                {
                    inc._data = inc._lbound + (addend - ((dynamic)inc._ubound - inc._data)) - 1;
                }
            }
            else
            {
                inc._data = (dynamic)inc._data + addend;
            }
            */


            /*
            inc._data = (dynamic)inc._data + addend;
            inc._data = ((dynamic)inc._data > inc._ubound) ? (inc._circular) ? ((dynamic)inc._lbound - 1) + ((dynamic)inc._data - inc._ubound) : inc._ubound : inc._data;
            */
        }

        public static RangeBoundNumber<T> operator -(RangeBoundNumber<T> val, T data)
        {
            RangeBoundNumber<T> dec = new RangeBoundNumber<T>(val);
            T subtrahend = (((dynamic)data / dec._quanta) % dec._rsize) * dec._quanta;
            dec._data = (((dynamic)dec._data - dec._lbound) < subtrahend) ? (!dec._circular) ? dec._lbound : dec._ubound - (subtrahend - ((dynamic)dec._data - dec._lbound)) + 1 : (dynamic)dec._data - subtrahend;
            return dec;
        }

        public void Assign(T data)
        {
            if ((dynamic)data < _lbound ||  _ubound < (dynamic)data) throw new ArgumentException("Out of range. " + data.ToString());
            _data = (dynamic)data;

            /*
            if (_lbound < (dynamic)data && (dynamic)data < _ubound)
            {
                _data = (dynamic)data;
                return;
            }
            
            throw new ArgumentException ("Out of range. " + data.ToString());
            */
        }
    }
}
