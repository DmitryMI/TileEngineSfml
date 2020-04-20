﻿using System;
using System.Collections.Generic;
using System.Linq;
using TileEngineSfmlMapEditor.MapEditing;

namespace TileEngineSfmlCs.TileEngine.TypeManagement
{
    public class TypeManager : IComparer<Type>
    {
        #region Singleton
        
        private static TypeManager _instance;

        public static TypeManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private TreeNode<Type> _treeRoot;
        private Type[] _tileObjectDerivatives;

        public TreeNode<Type> TreeRoot
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

        public TreeNode<Type> GetTileObjectTree()
        {
            var types = from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                        from assemblyType in domainAssembly.GetTypes()
                        where assemblyType.CustomAttributes.All(t => t.AttributeType != typeof(TypeManagerIgnoreAttribute))
                        where assemblyType.IsSubclassOf(typeof(TileObject))
                        select assemblyType;
            _tileObjectDerivatives = types.ToArray();

            TreeNode<Type> typeTree = new TreeNode<Type>(typeof(TileObject));
            ProcessType(typeof(TileObject), typeTree);
            return typeTree;
        }

        private void ProcessType(Type currentType, TreeNode<Type> parentNode)
        {
            TreeNode<Type> node = new TreeNode<Type>(currentType);
            parentNode.Add(node);

            var derivatives = from assemblyType in _tileObjectDerivatives
                where assemblyType.BaseType == currentType && assemblyType != currentType
                select assemblyType;

            foreach (var derivative in derivatives)
            {
                ProcessType(derivative, node);
            }
            
            node.Sort(this);
        }

        public int Compare(Type x, Type y)
        {
            if (x == null || y == null)
                return 0;

            string xString = x.Name;
            string yString = y.Name;

            return String.CompareOrdinal(xString, yString);
        }
    }
}
