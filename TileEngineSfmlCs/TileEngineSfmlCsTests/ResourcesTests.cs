using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileEngineSfmlCs.TileEngine.ResourceManagement;
using TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes;

namespace TileEngineSfmlCsTests
{
    [TestClass()]
    public class ResourcesTests
    {
        [TestMethod()]
        public void GetEntryTest()
        {
            GameResources resources = new GameResources("C:\\Users\\Dmitry\\Downloads\\COCOMO_1");

            ResourceEntry entry = resources.GetEntry("bin\\Debug\\COCOMO_1.exe");

            if (entry != null && entry.Name.Equals("COCOMO_1.exe"))
            {
                
            }
            else
            {
                Assert.Fail("File not found!");
            }
        }
    }
}