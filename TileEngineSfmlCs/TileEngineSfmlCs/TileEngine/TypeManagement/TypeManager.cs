﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private Type[] _tileObjectDerivatives;

        public TreeNode<EntityType> TreeRoot
        {
            get
            {
                if (_treeRoot == null)
                {
                    _treeRoot = GetTileObjectTree();
                }

                return _treeRoot;
            }
        }

        public Type[] TileObjectDerivatives => _tileObjectDerivatives;

        public TreeNode<EntityType> GetTileObjectTree()
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
    }
}
