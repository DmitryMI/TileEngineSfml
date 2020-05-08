using System;
using System.Diagnostics;
using TileEngineSfmlCs.GameManagement.ClientSide;

namespace TileEngineSfmlCs.GameManagement.DialogForms.Lobby
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
