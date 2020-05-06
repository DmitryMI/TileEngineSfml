using System;

namespace TileEngineSfmlCs.Utils.RandomGenerators
{
    public static class RandomUtils
    {
        private static XorShift64 _xorShift64 = new XorShift64();
        private static Random _random = new Random();
        public static ulong GetRandomUInt64()
        {
            return _xorShift64.Next();
        }

        public static int GetRandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
