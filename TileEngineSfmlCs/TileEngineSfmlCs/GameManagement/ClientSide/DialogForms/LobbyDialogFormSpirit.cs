using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class LobbyDialogFormSpirit : DialogFormSpirit
    {
        public LobbyDialogFormSpirit(int instanceId) : base(instanceId)
        {
            Debug.WriteLine("LobbyDialog Spawned!");
        }

        public override event Action OnKillEvent;

        public override void OnDataUpdate(string key, string input)
        {
            
        }

        public override void Kill()
        {
            Debug.WriteLine("LobbyDialog killed!");
            OnKillEvent?.Invoke();
        }
    }
}
