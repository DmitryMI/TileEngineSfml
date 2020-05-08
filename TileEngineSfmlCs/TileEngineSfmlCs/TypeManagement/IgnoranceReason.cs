namespace TileEngineSfmlCs.TypeManagement
{
    public enum IgnoranceReason
    {
        TestObject = 1,
        WorkInProgress = 1 << 1,
        Obsolete = 1 << 2,
        NotReallyAnObject = 1 << 3,
        Other = 1 << 4
    }
}
