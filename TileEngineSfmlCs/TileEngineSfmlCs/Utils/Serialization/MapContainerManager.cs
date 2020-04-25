using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public class MapContainerManager
    {
        #region Singleton

        private static MapContainerManager _instance;

        public static MapContainerManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        public IMapContainer GetMapContainer(string fileSystemPath)
        {
            FileInfo fileInfo = new FileInfo(fileSystemPath);
            if (fileInfo.Extension == ".temap")
            {
                return new ZipMapContainer(fileSystemPath);
            }

            throw new NotImplementedException();
        }
    }
}
