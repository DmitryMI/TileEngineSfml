using TileEngineSfmlCs.GameManagement.ServerSide;

namespace TileEngineSfmlCs.GameManagement.DialogForms.Main
{
    public class MainDialogForm : IDialogForm
    {
        public Player InteractingPlayer { get; set; }
        public int DialogInstanceId { get; set; }
        public void OnUserClose()
        {
            
        }

        public void OnUserInput(string key, string input)
        {
            
        }

        public DialogFormType SpiritType { get; } =  DialogFormManager.Instance.GetByFullName(typeof(MainDialogFormSpirit).FullName);

        public void AppendChat(string htmlAppend)
        {
            NetworkManager.Instance.UpdateDialogForm(this, MainDialogFormKeys.ChatAppend.ToString(), htmlAppend);
        }
    }
}
