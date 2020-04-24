using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Scripting.Utils;
using TileEngineSfmlCs.ResourceManagement.ResourceTypes;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TypeManagement
{
    public class TypeManager : IComparer<EntityType>
    {
        #region Singleton
        
        private static TypeManager _instance;

        public static TypeManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private TreeNode<EntityType> _treeRoot;

        private List<EntityType> _registeredEntityTypes = new List<EntityType>();

        private Type[] _tileObjectDerivatives;

        public TypeManager()
        {
            _treeRoot = GetAssemblyTileObjectTree();
        }

        public TreeNode<EntityType> TreeRoot
        {
            get
            {
                return _treeRoot;
            }
        }

        public Type[] TileObjectDerivatives => _tileObjectDerivatives;

        public EntityType GetEntityType(string typeName)
        {
            foreach (var type in _registeredEntityTypes)
            {
                if (type.FullName == typeName)
                {
                    return type;
                }
            }

            return null;
        }

        public TreeNode<EntityType> GetAssemblyTileObjectTree()
        {
            var types = from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                        from assemblyType in domainAssembly.GetTypes()
                        where assemblyType.CustomAttributes.All(t => t.AttributeType != typeof(TypeManagerIgnoreAttribute))
                        where assemblyType.IsSubclassOf(typeof(TileObject))
                        select assemblyType;
            _tileObjectDerivatives = types.ToArray();

            TreeNode<EntityType> typeTree = ProcessAssemblyType(typeof(TileObject));

            return typeTree;
        }

        private TreeNode<EntityType> ProcessAssemblyType(Type currentType)
        {
            AssemblyEntityType assemblyEntity = new AssemblyEntityType(currentType);
            TreeNode<EntityType> node = new TreeNode<EntityType>(assemblyEntity);
            _registeredEntityTypes.Add(node.Data);

            var derivatives = from assemblyType in _tileObjectDerivatives
                where assemblyType.BaseType == currentType && assemblyType != currentType
                select assemblyType;

            foreach (var derivative in derivatives)
            {
                var childNode = ProcessAssemblyType(derivative);
                node.Add(childNode);
            }
            
            node.Sort(this);
            return node;
        }

        public int Compare(EntityType x, EntityType y)
        {
            if (x == null || y == null)
                return 0;

            string xString = x.Name;
            string yString = y.Name;

            return String.CompareOrdinal(xString, yString);
        }

        public void RegisterCustomTypes(IEnumerable<EntityType> types)
        {
            var customCategory = _treeRoot.FirstOrDefault(t => t.Data.Name == "UserTypes");
            if (customCategory == null)
            {
                 DirectoryEntityType userTypes = new DirectoryEntityType("UserTypes");
                customCategory = new TreeNode<EntityType>(userTypes);
                _treeRoot.Add(customCategory);
            }

            foreach (var type in types)
            {
                customCategory.Add(new TreeNode<EntityType>(type));
                _registeredEntityTypes.Add(type);
            }
        }
    }
}
