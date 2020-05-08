using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;
using TileEngineSfmlCs.GameManagement.ClientSide.TileObjects;
using TileEngineSfmlCs.GameManagement.DialogForms.Main;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCsClient.Rendering;
using Icon = TileEngineSfmlCs.Types.Icon;

namespace TileEngineSfmlCsClient
{
    public partial class MainForm : Form, ISpiritRenderer, ILogger
    {
        private SfmlRenderControl _sfmlRenderControl;
        private HtmlPanel _chatPanel;
        private MainDialogFormSpirit _spirit;

        private StringBuilder _htmlHeadData = new StringBuilder();
        private StringBuilder _htmlBodyData = new StringBuilder(); 

        public MainDialogFormSpirit DialogSpirit
        {
            get => _spirit;
            set
            {
                _spirit = value;
                _spirit.OnChatAppend += OnChatAppend;
                _spirit.OnKillEvent += Close;       // TODO Not the best solution
            }
        }

        public MainForm()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;

            CreateRenderControl();
            CreateChat();
            AdjustUi();
        }

        private void OnChatAppend(string appendHtml)
        {
            _htmlBodyData.Append(appendHtml);
            _chatPanel.Text = GetHtmlDocument(_htmlHeadData.ToString(), _htmlBodyData.ToString());
        }

        private string CreateChatStyles()
        {
            string style =
                @"<style>
                    h1{
                        font-family: Geneva, Arial, Helvetica, sans-serif;
                        padding: 0 5px 0 5px;
                    }
                    .welcome{
                        color: blue;
                    }                   

                    p{
                        text-align: justify;
                        padding: 0 5px 0 5px;
                        margin: 0px;
                    }                    
                    .log_message {                       
                        font-family: Consolas,monaco,monospace; 
                        color: black;                  
                        font-size: 10px;                      
                    }  
                    .log_error {
                        font-family: Consolas,monaco,monospace; 
                        color: red;
                        font-size: 10px;                       
                    }  
                </style>";
            return style;
        }

        private void CreateChat()
        {
            _chatPanel = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            
            _chatPanel.Location = new Point(0, 0);
            _chatPanel.Size = new Size(UiControlPanel.Size.Width, UiControlPanel.Size.Height / 2);
            _chatPanel.Dock = DockStyle.Bottom;
            _chatPanel.BorderStyle = BorderStyle.Fixed3D;

            _htmlHeadData.Append(CreateChatStyles());
            _htmlBodyData.Append($@"<h1><span class=""welcome"">Welcome to TileEngine!</span></h1>");

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

        private string GetHtmlDocument(string headHtml, string bodyHtml)
        {
            string result = $"<html><head>{headHtml}</head><body>{bodyHtml}</body></html>";
            return result;
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

        public void Log(string message)
        {
            string htmlMessage =
                $@"<p><span class=""log_message"">{message}</span></p>";

            _htmlBodyData.Append(htmlMessage);
            _chatPanel.Text = GetHtmlDocument(_htmlHeadData.ToString(), _htmlBodyData.ToString());
        }

        public void LogError(string message)
        {
            string htmlMessage =
                $@"<p><span class=""log_error"">{message}</span></p>";

            _htmlBodyData.Append(htmlMessage);
            _chatPanel.Text = GetHtmlDocument(_htmlHeadData.ToString(), _htmlBodyData.ToString());

            Debug.WriteLine(_chatPanel.Text);
        }
    }
}
