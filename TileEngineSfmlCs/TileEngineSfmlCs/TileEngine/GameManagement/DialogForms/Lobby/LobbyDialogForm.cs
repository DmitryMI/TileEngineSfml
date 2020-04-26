using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.GameManagement.Networking;
using TileEngineSfmlCs.TileEngine.Logging;

namespace TileEngineSfmlCs.TileEngine.GameManagement.DialogForms.Lobby
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
