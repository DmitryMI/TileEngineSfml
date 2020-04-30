namespace TileEngineSfmlCsClient.DialogFormWrappers
{
    partial class LobbyDialogWrapper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.RandomNameButton = new System.Windows.Forms.Button();
            this.RandomLastNameButton = new System.Windows.Forms.Button();
            this.LastNameBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.JoinGameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Character first name:";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(12, 25);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(192, 20);
            this.NameBox.TabIndex = 1;
            // 
            // RandomNameButton
            // 
            this.RandomNameButton.Location = new System.Drawing.Point(210, 25);
            this.RandomNameButton.Name = "RandomNameButton";
            this.RandomNameButton.Size = new System.Drawing.Size(75, 20);
            this.RandomNameButton.TabIndex = 2;
            this.RandomNameButton.Text = "Random";
            this.RandomNameButton.UseVisualStyleBackColor = true;
            this.RandomNameButton.Click += new System.EventHandler(this.RandomNameButton_Click);
            // 
            // RandomLastNameButton
            // 
            this.RandomLastNameButton.Location = new System.Drawing.Point(210, 67);
            this.RandomLastNameButton.Name = "RandomLastNameButton";
            this.RandomLastNameButton.Size = new System.Drawing.Size(75, 20);
            this.RandomLastNameButton.TabIndex = 5;
            this.RandomLastNameButton.Text = "Random";
            this.RandomLastNameButton.UseVisualStyleBackColor = true;
            this.RandomLastNameButton.Click += new System.EventHandler(this.RandomLastNameButton_Click);
            // 
            // LastNameBox
            // 
            this.LastNameBox.Location = new System.Drawing.Point(12, 67);
            this.LastNameBox.Name = "LastNameBox";
            this.LastNameBox.Size = new System.Drawing.Size(192, 20);
            this.LastNameBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Character last name:";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(640, 415);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 6;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // JoinGameButton
            // 
            this.JoinGameButton.Location = new System.Drawing.Point(721, 415);
            this.JoinGameButton.Name = "JoinGameButton";
            this.JoinGameButton.Size = new System.Drawing.Size(75, 23);
            this.JoinGameButton.TabIndex = 7;
            this.JoinGameButton.Text = "Join!";
            this.JoinGameButton.UseVisualStyleBackColor = true;
            this.JoinGameButton.Click += new System.EventHandler(this.JoinGameButton_Click);
            // 
            // LobbyDialogWrapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.JoinGameButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.RandomLastNameButton);
            this.Controls.Add(this.LastNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RandomNameButton);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.label1);
            this.Name = "LobbyDialogWrapper";
            this.Text = "LobbyDialogWrapper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Button RandomNameButton;
        private System.Windows.Forms.Button RandomLastNameButton;
        private System.Windows.Forms.TextBox LastNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button JoinGameButton;
    }
}