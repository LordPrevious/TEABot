namespace TEABot.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.tsmiTeaBot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTBAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTBGitHub = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTBExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIrcConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIrcDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIrcReconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestartWebSocket = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDataDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiReload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRecompile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReloadRtf = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.tsslIrcStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSeparator1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslWebSocketStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSeparator2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMain.SuspendLayout();
            this.ssStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTeaBot,
            this.tsmiConnection,
            this.tsmiConfig});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.msMain.Size = new System.Drawing.Size(684, 25);
            this.msMain.TabIndex = 0;
            // 
            // tsmiTeaBot
            // 
            this.tsmiTeaBot.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTBAbout,
            this.tsmiTBGitHub,
            this.tsSeparator1,
            this.tsmiTBExit});
            this.tsmiTeaBot.Name = "tsmiTeaBot";
            this.tsmiTeaBot.Size = new System.Drawing.Size(57, 19);
            this.tsmiTeaBot.Text = "TEABot";
            // 
            // tsmiTBAbout
            // 
            this.tsmiTBAbout.Name = "tsmiTBAbout";
            this.tsmiTBAbout.Size = new System.Drawing.Size(112, 22);
            this.tsmiTBAbout.Text = "About";
            this.tsmiTBAbout.Click += new System.EventHandler(this.TsmiTBAbout_Click);
            // 
            // tsmiTBGitHub
            // 
            this.tsmiTBGitHub.Name = "tsmiTBGitHub";
            this.tsmiTBGitHub.Size = new System.Drawing.Size(112, 22);
            this.tsmiTBGitHub.Text = "GitHub";
            this.tsmiTBGitHub.Click += new System.EventHandler(this.TsmiTBGitHub_Click);
            // 
            // tsSeparator1
            // 
            this.tsSeparator1.Name = "tsSeparator1";
            this.tsSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // tsmiTBExit
            // 
            this.tsmiTBExit.Name = "tsmiTBExit";
            this.tsmiTBExit.Size = new System.Drawing.Size(112, 22);
            this.tsmiTBExit.Text = "Exit";
            this.tsmiTBExit.Click += new System.EventHandler(this.TsmiTBExit_Click);
            // 
            // tsmiConnection
            // 
            this.tsmiConnection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiIrcConnect,
            this.tsmiIrcDisconnect,
            this.tsmiIrcReconnect,
            this.toolStripSeparator2,
            this.tsmiRestartWebSocket});
            this.tsmiConnection.Name = "tsmiConnection";
            this.tsmiConnection.Size = new System.Drawing.Size(81, 19);
            this.tsmiConnection.Text = "Connection";
            // 
            // tsmiIrcConnect
            // 
            this.tsmiIrcConnect.Name = "tsmiIrcConnect";
            this.tsmiIrcConnect.Size = new System.Drawing.Size(207, 22);
            this.tsmiIrcConnect.Text = "Connect IRC";
            this.tsmiIrcConnect.Click += new System.EventHandler(this.TsmiIrcConnect_Click);
            // 
            // tsmiIrcDisconnect
            // 
            this.tsmiIrcDisconnect.Name = "tsmiIrcDisconnect";
            this.tsmiIrcDisconnect.Size = new System.Drawing.Size(207, 22);
            this.tsmiIrcDisconnect.Text = "Disconnect IRC";
            this.tsmiIrcDisconnect.Click += new System.EventHandler(this.TsmiIrcDisconnect_Click);
            // 
            // tsmiIrcReconnect
            // 
            this.tsmiIrcReconnect.Enabled = false;
            this.tsmiIrcReconnect.Name = "tsmiIrcReconnect";
            this.tsmiIrcReconnect.Size = new System.Drawing.Size(207, 22);
            this.tsmiIrcReconnect.Text = "Reconnect IRC";
            this.tsmiIrcReconnect.Click += new System.EventHandler(this.tsmiIrcReconnect_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // tsmiRestartWebSocket
            // 
            this.tsmiRestartWebSocket.Enabled = false;
            this.tsmiRestartWebSocket.Name = "tsmiRestartWebSocket";
            this.tsmiRestartWebSocket.Size = new System.Drawing.Size(207, 22);
            this.tsmiRestartWebSocket.Text = "Restart WebSocket Server";
            this.tsmiRestartWebSocket.Click += new System.EventHandler(this.tsmiRestartWebSocket_Click);
            // 
            // tsmiConfig
            // 
            this.tsmiConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDataDirectory,
            this.toolStripSeparator1,
            this.tsmiReload,
            this.tsmiRecompile,
            this.tsmiReloadRtf});
            this.tsmiConfig.Name = "tsmiConfig";
            this.tsmiConfig.Size = new System.Drawing.Size(55, 19);
            this.tsmiConfig.Text = "Config";
            // 
            // tsmiDataDirectory
            // 
            this.tsmiDataDirectory.Name = "tsmiDataDirectory";
            this.tsmiDataDirectory.Size = new System.Drawing.Size(186, 22);
            this.tsmiDataDirectory.Text = "Data directory";
            this.tsmiDataDirectory.Click += new System.EventHandler(this.DataDirectoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // tsmiReload
            // 
            this.tsmiReload.Name = "tsmiReload";
            this.tsmiReload.Size = new System.Drawing.Size(186, 22);
            this.tsmiReload.Text = "Reload";
            this.tsmiReload.Click += new System.EventHandler(this.TsmiReload_Click);
            // 
            // tsmiRecompile
            // 
            this.tsmiRecompile.Name = "tsmiRecompile";
            this.tsmiRecompile.Size = new System.Drawing.Size(186, 22);
            this.tsmiRecompile.Text = "Recompile";
            this.tsmiRecompile.Click += new System.EventHandler(this.TsmiRecompile_Click);
            // 
            // tsmiReloadRtf
            // 
            this.tsmiReloadRtf.Name = "tsmiReloadRtf";
            this.tsmiReloadRtf.Size = new System.Drawing.Size(186, 22);
            this.tsmiReloadRtf.Text = "Reload RTF templates";
            this.tsmiReloadRtf.Click += new System.EventHandler(this.TsmiReloadRtf_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.DetectUrls = false;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 25);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(684, 414);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            // 
            // ssStatus
            // 
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslIrcStatus,
            this.tsslSeparator1,
            this.tsslWebSocketStatus,
            this.tsslSeparator2});
            this.ssStatus.Location = new System.Drawing.Point(0, 439);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(684, 22);
            this.ssStatus.TabIndex = 1;
            // 
            // tsslIrcStatus
            // 
            this.tsslIrcStatus.Image = global::TEABot.Icons.ic_connection_disabled;
            this.tsslIrcStatus.Name = "tsslIrcStatus";
            this.tsslIrcStatus.Size = new System.Drawing.Size(16, 17);
            // 
            // tsslSeparator1
            // 
            this.tsslSeparator1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslSeparator1.Name = "tsslSeparator1";
            this.tsslSeparator1.Size = new System.Drawing.Size(4, 17);
            // 
            // tsslWebSocketStatus
            // 
            this.tsslWebSocketStatus.Image = global::TEABot.Icons.ic_connection_disabled;
            this.tsslWebSocketStatus.Name = "tsslWebSocketStatus";
            this.tsslWebSocketStatus.Size = new System.Drawing.Size(16, 17);
            // 
            // tsslSeparator2
            // 
            this.tsslSeparator2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslSeparator2.Name = "tsslSeparator2";
            this.tsslSeparator2.Size = new System.Drawing.Size(4, 17);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.ssStatus);
            this.Controls.Add(this.msMain);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MainForm";
            this.Text = "TEABot ɣ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ToolStripMenuItem tsmiConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiDataDirectory;
        private System.Windows.Forms.ToolStripMenuItem tsmiReload;
        private System.Windows.Forms.ToolStripMenuItem tsmiRecompile;
        private System.Windows.Forms.ToolStripMenuItem tsmiReloadRtf;
        private System.Windows.Forms.ToolStripMenuItem tsmiConnection;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrcConnect;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrcDisconnect;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslIrcStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel tsslWebSocketStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiTeaBot;
        private System.Windows.Forms.ToolStripMenuItem tsmiTBAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiTBGitHub;
        private System.Windows.Forms.ToolStripSeparator tsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTBExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrcReconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestartWebSocket;
    }
}

