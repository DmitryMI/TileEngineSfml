using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class LobbyDialogFormSpirit : DialogFormSpirit
    {
        public LobbyDialogFormSpirit(int instanceId) : base(instanceId)
        {
            Debug.WriteLine("LobbyDialog Spawned!");
        }

        public override event Action OnKillEvent;

        public override void OnServerDataUpdate(string key, string input)
        {
            
        }

        public override void UserDataUpdate(string key, string input)
        {
            ClientNetworkManager.Instance.SendDialogFormInput(this, key, input);
        }

        public void UserDataUpdate(LobbyInputKeys key, string input)
        {
            string keyString = key.ToString();
            UserDataUpdate(keyString, input);
        }

        public override void Kill()
        {
            Debug.WriteLine("LobbyDialog killed!");
            OnKillEvent?.Invoke();
        }
    }
}
