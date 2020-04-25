using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Utils.Serialization.ZipContainer;

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
            if (File.Exists(fileSystemPath))
            {
                FileInfo fileInfo = new FileInfo(fileSystemPath);
                if (fileInfo.Extension == ".temap")
                {
                    return new ZipMapContainer(fileSystemPath);
                }
                throw new NotImplementedException("Only .temap format is supported");
            }
            else if (Directory.Exists(fileSystemPath))
            {

            }

            throw new NotImplementedException();
        }
    }
}
