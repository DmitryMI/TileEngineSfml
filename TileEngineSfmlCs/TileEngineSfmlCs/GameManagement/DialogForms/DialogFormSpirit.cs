using System;

namespace TileEngineSfmlCs.GameManagement.DialogForms
{
    /// <summary>
    /// Represents DialogForm on client side
    /// </summary>
    public abstract class DialogFormSpirit
    {
        public abstract event Action OnKillEvent;

        public int InstanceId { get; }

        public object Wrapper { get; set; }

        public virtual DialogFormType GetDialogFormType()
        {
            return DialogFormManager.Instance.GetByFullName(GetType().FullName);
        }

        public DialogFormSpirit(int instanceId)
        {
            InstanceId = instanceId;
        }

        public string FormTypeName { get; }
        
        public abstract void OnServerDataUpdate(string key, string input);

        public abstract void UserDataUpdate(string key, string input);

        public abstract void Kill();
    }
}
