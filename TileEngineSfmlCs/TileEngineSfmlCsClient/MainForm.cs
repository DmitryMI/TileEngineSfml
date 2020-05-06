using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;
using TileEngineSfmlCs.GameManagement.ClientSide.TileObjects;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCsClient.Rendering;
using Icon = TileEngineSfmlCs.Types.Icon;

namespace TileEngineSfmlCsClient
{
    public partial class MainForm : Form, ISpiritRenderer
    {
        private SfmlRenderControl _sfmlRenderControl;
        private HtmlPanel _chatPanel;
        public MainForm()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;

            CreateRenderControl();
            CreateChat();
            AdjustUi();
        }

        private void CreateChat()
        {
            _chatPanel = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            
            _chatPanel.Location = new Point(0, 0);
            _chatPanel.Size = new Size(UiControlPanel.Size.Width, UiControlPanel.Size.Height / 2);
            _chatPanel.Dock = DockStyle.Bottom;
            _chatPanel.BorderStyle = BorderStyle.Fixed3D;
            
            _chatPanel.BaseStylesheet =
                @"<style>
                    h1 {
                        font-family: 'Times New Roman', Times, serif;  /* Гарнитура текста */ 
                        font-size: 100%;                               /* Размер шрифта в процентах */ 
                    } 
                    h2 {
                        font-family: 'Times New Roman', Times, serif;  /* Гарнитура текста */ 
                        font-size: 100%;                               /* Размер шрифта в процентах */ 
                    } 
                    p {
                        font-family: Verdana, Arial, Helvetica, sans-serif; 
                        font-size: 11pt;                               /* Размер шрифта в пунктах */ 
                    }
                    body {
                        background-color: lightgrey;
                        color: blue;
                    }
                </style>";
            _chatPanel.Text =
                @"
                    <body style='background-color = #333'>
                        <h1>Welcome to TileEngine!</h1>
                        <h2>Enjoy your stay!</h2>
                    </body>
                ";

            UiControlPanel.Controls.Add(_chatPanel);
        }

        private void CreateRenderControl()
        {
            _sfmlRenderControl = new SfmlRenderControl();
            _sfmlRenderControl.Location = new Point(0, 0);
             _sfmlRenderControl.Size = new Size(Size.Width / 2, Size.Height);
            //_sfmlRenderControl.Location = new Point(50, 50);
            //_sfmlRenderControl.Size = new Size(800, 600);
            _sfmlRenderControl.BackColor = Color.Black;
            Controls.Add(_sfmlRenderControl);

            _sfmlRenderControl.Initialize();
        }

        private void AdjustUi()
        {
            UiControlPanel.Location = new Point(Size.Width / 2, 0);
            UiControlPanel.Size = new Size(Size.Width / 2, Size.Height);
        }

        public void PreRendering()
        {
            _sfmlRenderControl.PreRendering();
        }

        public void Render(Vector2 iconPosition, Icon icon, TileObjectSpirit spirit)
        {
            _sfmlRenderControl.Render(iconPosition, icon, spirit);
        }

        public void SetViewCenter(Vector2 viewCenter)
        {
            _sfmlRenderControl.SetViewCenter(viewCenter);
        }

        public void PostRendering()
        {
            _sfmlRenderControl.PostRendering();
        }
    }
}
