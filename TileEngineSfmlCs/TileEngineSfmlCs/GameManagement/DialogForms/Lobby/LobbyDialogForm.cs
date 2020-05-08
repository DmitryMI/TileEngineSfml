using System;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Logging;

namespace TileEngineSfmlCs.GameManagement.DialogForms.Lobby
{
    public class LobbyDialogForm : IDialogForm
    {
        private string _firstName;
        private string _lastName;

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
                        _firstName = input;
                        break;
                    case LobbyInputKeys.InputLastName:
                        _lastName = input;
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

        public string FirstName => _firstName;

        public string LastName => _lastName;

        public LobbyDialogForm()
        {
            DialogFormManager.Instance.AssignFormIndex(this);
        }
    }
}
