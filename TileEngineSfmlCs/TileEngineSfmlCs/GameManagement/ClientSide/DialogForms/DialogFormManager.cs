using System;
using System.Collections.Generic;
using System.Linq;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class DialogFormManager : IComparer<DialogFormType>
    {
        #region Singleton

        private static DialogFormManager _instance;

        public static DialogFormManager Instance
        {
            get => _instance;
            set => _instance = value;
        }
        #endregion

        private TreeNode<DialogFormType> _typeTree;
        private List<DialogFormType> _registeredDialogFormTypes = new List<DialogFormType>();

        public TreeNode<DialogFormType> DialogFormTypes => _typeTree;

        public DialogFormManager()
        {
            LoadBuiltInForms();
        }

        private void LoadBuiltInForms()
        {
            _typeTree = GetAssemblyTileObjectTree();
        }

        public TreeNode<DialogFormType> GetAssemblyTileObjectTree()
        {
            var types = from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(DialogFormSpirit))
                select assemblyType;

            TreeNode<DialogFormType> typeTree = ProcessAssemblyType(typeof(TileObject), types.ToArray());

            return typeTree;
        }

        private TreeNode<DialogFormType> ProcessAssemblyType(Type currentType, Type[] assemblyDerivatives)
        {
            AssemblyDialogFormType assemblyEntity = new AssemblyDialogFormType(currentType);
            TreeNode<DialogFormType> node = new TreeNode<DialogFormType>(assemblyEntity);
            _registeredDialogFormTypes.Add(node.Data);

            var derivatives = from assemblyType in assemblyDerivatives
                              where assemblyType.BaseType == currentType && assemblyType != currentType
                select assemblyType;

            foreach (var derivative in derivatives)
            {
                var childNode = ProcessAssemblyType(derivative, assemblyDerivatives);
                node.Add(childNode);
            }

            node.Sort(this);
            return node;
        }


        public DialogFormType GetByFullName(string fullName)
        {
            return _registeredDialogFormTypes.FirstOrDefault(t => t.Name.Equals(fullName));
        }

        public void LoadFromMap(IMapContainer mapContainer)
        {
            // TODO Load resources from map
        }

        public int Compare(DialogFormType x, DialogFormType y)
        {
            if (x == null || y == null)
                return 0;
            return String.CompareOrdinal(x.Name, y.Name);
        }
    }
}
