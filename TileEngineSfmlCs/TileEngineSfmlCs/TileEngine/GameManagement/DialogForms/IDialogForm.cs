using System;
using TileEngineSfmlCs.TileEngine.GameManagement.Networking;

namespace TileEngineSfmlCs.TileEngine.GameManagement.DialogForms
{
    public interface IDialogForm
    {
        Player InteractingPlayer { get; set; }
        void CloseForm();
        /// <summary>
        /// For network manager usage only
        /// </summary>
        int DialogInstanceId { get; set; }
        void OnUserClose();
        void OnUserInput(string key, string input);
    }
}
