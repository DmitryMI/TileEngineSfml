using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;

namespace TileEngineSfmlCsClient.DialogFormWrappers
{
    public partial class LobbyDialogWrapper : Form
    {
        private LobbyDialogFormSpirit _dialogFormSpirit;
        public LobbyDialogWrapper(LobbyDialogFormSpirit spirit)
        {
            InitializeComponent();
            _dialogFormSpirit = spirit;
            spirit.OnKillEvent += OnKill;
            spirit.Wrapper = this;
        }

        void OnKill()
        {
            Close();
        }

        private void JoinGameButton_Click(object sender, EventArgs e)
        {
            _dialogFormSpirit.UserDataUpdate(LobbyInputKeys.InputFirstName, NameBox.Text);
            _dialogFormSpirit.UserDataUpdate(LobbyInputKeys.InputLastName, LastNameBox.Text);
            _dialogFormSpirit.UserDataUpdate(LobbyInputKeys.Join, "0");
        }

        private void RandomNameButton_Click(object sender, EventArgs e)
        {
            NameBox.Text = "John";
        }

        private void RandomLastNameButton_Click(object sender, EventArgs e)
        {
            LastNameBox.Text = "Doe";
        }
    }
}
