using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class HtmlDialogFormSpirit : DialogFormSpirit
    {
        private string _html;

        public string Html => _html;

        public HtmlDialogFormSpirit(int instanceId) : base(instanceId)
        {
        }


        public override event Action OnKillEvent;

        public override void OnDataUpdate(string key, string input)
        {
            
        }

        public override void Kill()
        {
            OnKillEvent?.Invoke();
        }
    }
}
