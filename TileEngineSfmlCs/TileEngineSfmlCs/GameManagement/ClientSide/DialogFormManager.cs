using System.Collections.Generic;
using System.Linq;

namespace TileEngineSfmlCs.GameManagement.ClientSide
{
    public class DialogFormManager
    {
        #region Singleton

        private static DialogFormManager _instance;

        public static DialogFormManager Instance
        {
            get => _instance;
            set => _instance = value;
        }
        #endregion

        private List<DialogFormType> _registeredDialogFormTypes = new List<DialogFormType>();

        public DialogFormType GetByFullName(string fullName)
        {
            return _registeredDialogFormTypes.FirstOrDefault(t => t.Name.Equals(fullName));
        }
        
    }
}
