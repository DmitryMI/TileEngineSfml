using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.TileEngine.Logging;

namespace TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby
{
    public class LobbyDialogForm : IDialogForm
    {
        public Player InteractingPlayer { get; set; }

        public int DialogInstanceId { get; set; }
        public void OnUserClose()
        {
            
        }

        public void OnUserInput(string key, string input)
        {
            LogManager.RuntimeLogger.Log($"[LobbyDialogForm] LobbyForm - Message from player {InteractingPlayer.Username}({InteractingPlayer.ConnectionId}): [{key}, {input}]");
        }

        public DialogFormType SpiritType =>
            DialogFormManager.Instance.GetByFullName(typeof(LobbyDialogFormSpirit).FullName);

        public LobbyDialogForm()
        {
            DialogFormManager.Instance.AssignFormIndex(this);
        }
    }
}
