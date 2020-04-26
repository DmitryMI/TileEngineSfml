using TileEngineSfmlCs.TileEngine.Logging;

namespace TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby
{
    public class LobbyDialogForm : IDialogForm
    {
        public Player InteractingPlayer { get; set; }
        public void CloseForm()
        {
            NetworkManager.Instance.UpdateDialogForm(this, "close", "0");
        }

        public int DialogInstanceId { get; set; }
        public void OnUserClose()
        {
            
        }

        public void OnUserInput(string key, string input)
        {
            LogManager.RuntimeLogger.Log($"[LobbyDialogForm] Lobby#{InteractingPlayer} - Message from player {InteractingPlayer.Username}: [{key}, {input}]");
        }
    }
}
