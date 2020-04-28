using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.Utils.RandomGenerators
{
    public class XorShift64
    {
        private UInt64 _state64;
        public XorShift64(UInt64 state)
        {
            if (state == 0)
            {
                throw new ArgumentException("state must be non-zero");
            }
            _state64 = state;
        }

        public XorShift64()
        {
            _state64 = (UInt64)DateTime.Now.ToBinary();
        }

        public ulong Next()
        {
            var  x = _state64;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            _state64 = x;
            return x;
        }
}
}
