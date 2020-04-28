namespace TileEngineSfmlCs.Utils.RandomGenerators
{
    public static class RandomUtils
    {
        private static XorShift64 _xorShift64 = new XorShift64();
        public static ulong GetRandomUInt64()
        {
            return _xorShift64.Next();
        }
    }
}
