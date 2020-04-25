using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.Scripting.PythonObjects;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.Scripting
{
    public class ScriptingManager
    {
        #region Singleton

        private static ScriptingManager _instance;
        public static ScriptingManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        public List<TreeNode<IFileSystemEntry>> _scriptFiles = new List<TreeNode<IFileSystemEntry>>();
        public List<PythonEntityType> _pythonEntityTypes = new List<PythonEntityType>();

        public ScriptingManager()
        {

        }

        public void LoadTypesFromMap(IMapContainer map, string directory)
        {
            _scriptFiles.Clear();
            var customTypesDirectory = map.GetTreeNode(directory);
            if (customTypesDirectory == null)
            {
                return;
            }

            TreeNode<IFileSystemEntry>.TraverseTree(customTypesDirectory, LoadTypeFromFile, false);

            foreach (var fileNode in _scriptFiles)
            {
                IFileSystemEntry file = fileNode.Data;
                PythonObjects.PythonObject obj = new PythonObject(file.OpenStream(), null);
                string path = TreeNode<IFileSystemEntry>.GetPath(fileNode, f => f?.Name);
                PythonEntityType entityType = new PythonEntityType(obj, path);
                _pythonEntityTypes.Add(entityType);
            }

            TypeManagement.TypeManager.Instance.RegisterCustomTypes(_pythonEntityTypes);
        }

        private void LoadTypeFromFile(TreeNode<IFileSystemEntry> node)
        {
            _scriptFiles.Add(node);
        }

        public void SaveTypesToMap(IMapContainer map, string directory)
        {
            foreach (var type in _pythonEntityTypes)
            {
                Stream stream = map.CreateEntry(type.FilePath);
                byte[] sourceCode = Encoding.UTF8.GetBytes(type.SourceCode);
                stream.Write(sourceCode, 0, sourceCode.Length);
            }
        }
    }
}
