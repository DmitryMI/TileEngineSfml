using System;
using System.Collections.Generic;
using System.Linq;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.GameManagement
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

        private List<ClientSide.DialogForms.DialogFormSpirit> _activeDialogSpirits = new List<DialogFormSpirit>();
        private List<IDialogForm> _activeDialogForms = new List<IDialogForm>();

        public TreeNode<DialogFormType> DialogFormTypes => _typeTree;

        public event Action<DialogFormSpirit> OnDialogSpiritSpawned;
        public event Action<DialogFormSpirit> OnDialogSpiritKilled; 

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

            TreeNode<DialogFormType> typeTree = ProcessAssemblyType(typeof(DialogFormSpirit), types.ToArray());

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
            return _registeredDialogFormTypes.FirstOrDefault(t => t.SpiritName.Equals(fullName));
        }

        public DialogFormType GetTypeByIndex(int typeIndex)
        {
            return _registeredDialogFormTypes[typeIndex];
        }

        public int GetTypeIndex(string name)
        {
            return _registeredDialogFormTypes.FindIndex(t => t.SpiritName.Equals(name));
        }

        public int GetTypeIndex(DialogFormType type)
        {
            var index = _registeredDialogFormTypes.IndexOf(type);
            if (index == -1)
            {
                throw new ArgumentException("Unregistered type!");
            }

            return index;
        }

        public void LoadFromMap(IMapContainer mapContainer)
        {
            // TODO Load resources from map
        }

        public int Compare(DialogFormType x, DialogFormType y)
        {
            if (x == null || y == null)
                return 0;
            return String.CompareOrdinal(x.SpiritName, y.SpiritName);
        }

        public DialogFormSpirit CreateDialogFormSpirit(DialogFormType type, int instanceId)
        {
            int existingIndex = _activeDialogSpirits.FindIndex(f => f.InstanceId == instanceId);
            if (existingIndex != -1)
            {
                return _activeDialogSpirits[existingIndex];
            }

            var spirit = type.ActivateSpirit(instanceId);
            _activeDialogSpirits.Add(spirit);
            OnDialogSpiritSpawned?.Invoke(spirit);
            return spirit;
        }

        public void AssignFormIndex(IDialogForm dialogForm)
        {
            int index = 0;
            while (index < _activeDialogForms.Count)
            {
                if(_activeDialogForms[index] == null)
                    break;
                index++;
            }

            if (index == _activeDialogForms.Count)
            {
                _activeDialogForms.Add(dialogForm);
            }
            else
            {
                _activeDialogForms[index] = dialogForm;
            }

            dialogForm.DialogInstanceId = index;
        }

        public void UnregisterFormIndex(IDialogForm dialogForm)
        {
            _activeDialogForms[dialogForm.DialogInstanceId] = null;
            dialogForm.DialogInstanceId = -1;
        }

        public void KillDialogFormSpirit(int instanceId)
        {
            int index = _activeDialogSpirits.FindIndex(f => f.InstanceId == instanceId);
            if (index != -1)
            {
                var spirit = _activeDialogSpirits[index];
                spirit.Kill();
                OnDialogSpiritKilled?.Invoke(spirit);
                _activeDialogSpirits.RemoveAt(index);
            }
        }

        public DialogFormSpirit GetFormSpirit(int instanceId)
        {
            return _activeDialogSpirits.FirstOrDefault(f => f.InstanceId == instanceId);
        }
    }
}
