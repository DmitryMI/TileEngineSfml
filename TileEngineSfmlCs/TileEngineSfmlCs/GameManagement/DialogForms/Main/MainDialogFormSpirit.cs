using System;
using System.Text;

namespace TileEngineSfmlCs.GameManagement.DialogForms.Main
{
    public class MainDialogFormSpirit : DialogFormSpirit
    {
        StringBuilder _htmlData = new StringBuilder();

        public MainDialogFormSpirit(int instanceId) : base(instanceId)
        {
        }

        public override event Action OnKillEvent;

        public event Action<string> OnChatAppend; 

        public override void OnServerDataUpdate(string key, string input)
        {
            MainDialogFormKeys keyEnum;
            if (Enum.TryParse(key, out keyEnum))
            {
                switch (keyEnum)
                {
                    case MainDialogFormKeys.ChatAppend:
                        _htmlData.Append(input);
                        OnChatAppend?.Invoke(input);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void UserDataUpdate(string key, string input)
        {
            
        }

        public override void Kill()
        {
            OnKillEvent?.Invoke();
        }

        public void AppendHtml(string html)
        {
            _htmlData.Append(html);
        }

        public string GetHtmlString() => _htmlData.ToString();
    }
}
