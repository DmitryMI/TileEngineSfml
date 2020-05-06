using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Utils.Serialization.FolderContainer;
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
            FileInfo fileInfo = new FileInfo(fileSystemPath);

            if (fileInfo.Extension == ".temap")
            {
                return new ZipMapContainer(fileSystemPath);
            }
            else if (fileInfo.Extension == ".scene")
            {
                return new FolderMapContainer(fileInfo.DirectoryName);
            }
            else
            {
                if (fileInfo.Exists)
                {
                    throw new NotImplementedException($"File format {fileInfo.Extension} is not supported");
                }
                else if (Directory.Exists(fileSystemPath))
                {
                    return new FolderMapContainer(fileSystemPath);
                }
            }

            throw new NotImplementedException("Unsupported format");
        }

    }
}
