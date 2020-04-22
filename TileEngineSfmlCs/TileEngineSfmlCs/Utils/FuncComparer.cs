using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.Utils
{
    public class FuncComparer<T> : IComparer<T>
    {
        private readonly Func<T,T, int> _func;
        public FuncComparer(Func<T, T, int> func)
        {
            _func = func;
        }
        public int Compare(T x, T y)
        {
            int value =  _func(x, y);
            return Math.Sign(value);
        }
    }
}
