using TileEngineSfmlCs.GameManagement.ServerSide;

namespace TileEngineSfmlCs.GameManagement.DialogForms
{
    public interface IDialogForm
    {
        Player InteractingPlayer { get; set; }
        /// <summary>
        /// For network manager usage only
        /// </summary>
        int DialogInstanceId { get; set; }
        void OnUserClose();
        void OnUserInput(string key, string input);

        DialogFormType SpiritType { get; }
    }
}
