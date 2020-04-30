using System;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.Logging;

namespace TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby
{
    public class LobbyDialogForm : IDialogForm
    {
        public Player InteractingPlayer { get; set; }

        public int DialogInstanceId { get; set; }

        public Action<LobbyDialogForm> PlayerJoinCallback { get; set; }

        public void OnUserClose()
        {
            
        }

        public void OnUserInput(string key, string input)
        {
            LogManager.RuntimeLogger.Log($"[LobbyDialogForm] LobbyForm - Message from player {InteractingPlayer.Username}({InteractingPlayer.ConnectionId}): [{key}, {input}]");

            LobbyInputKeys keyValue;
            if (Enum.TryParse(key, out keyValue))
            {
                switch (keyValue)
                {
                    case LobbyInputKeys.InputFirstName:
                        // TODO Setup character name
                        break;
                    case LobbyInputKeys.InputLastName:
                        // TODO Setup character name
                        break;
                    case LobbyInputKeys.Join:
                        // TODO Validate user data
                        PlayerJoinCallback?.Invoke(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public DialogFormType SpiritType =>
            DialogFormManager.Instance.GetByFullName(typeof(LobbyDialogFormSpirit).FullName);

        public LobbyDialogForm()
        {
            DialogFormManager.Instance.AssignFormIndex(this);
        }
    }
}
