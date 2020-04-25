using System;
using System.IO;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public interface IMapContainer : IDisposable
    {
        void Save();
        bool CanWrite { get; }
        TreeNode<IFileSystemEntry> MapTree { get; }
        IFileSystemEntry GetEntry(string path);
        IFileSystemEntry CreateEntry(string path);
        void DeleteEntry(string path);
        void UpdateTree();
        TreeNode<IFileSystemEntry> GetTreeNode(string directoryPath);

        /// <summary>
        /// Resets object to initial state
        /// </summary>
        void Reload();
    }
}