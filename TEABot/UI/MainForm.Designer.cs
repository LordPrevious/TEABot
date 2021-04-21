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
            this.tsmiIrc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIrcConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIrcDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDataDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRecompile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReloadRtf = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.scChannelsLog = new System.Windows.Forms.SplitContainer();
            this.tsChannelSelection = new System.Windows.Forms.ToolStrip();
            this.tsbtnGlobal = new System.Windows.Forms.ToolStripButton();
            this.msMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scChannelsLog)).BeginInit();
            this.scChannelsLog.Panel1.SuspendLayout();
            this.scChannelsLog.Panel2.SuspendLayout();
            this.scChannelsLog.SuspendLayout();
            this.tsChannelSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiIrc,
            this.tsmiConfig});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.msMain.Size = new System.Drawing.Size(684, 25);
            this.msMain.TabIndex = 0;
            this.msMain.Text = "menuStrip1";
            // 
            // tsmiIrc
            // 
            this.tsmiIrc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiIrcConnect,
            this.tsmiIrcDisconnect});
            this.tsmiIrc.Name = "tsmiIrc";
            this.tsmiIrc.Size = new System.Drawing.Size(37, 19);
            this.tsmiIrc.Text = "IRC";
            // 
            // tsmiIrcConnect
            // 
            this.tsmiIrcConnect.Name = "tsmiIrcConnect";
            this.tsmiIrcConnect.Size = new System.Drawing.Size(133, 22);
            this.tsmiIrcConnect.Text = "Connect";
            this.tsmiIrcConnect.Click += new System.EventHandler(this.tsmiIrcConnect_Click);
            // 
            // tsmiIrcDisconnect
            // 
            this.tsmiIrcDisconnect.Name = "tsmiIrcDisconnect";
            this.tsmiIrcDisconnect.Size = new System.Drawing.Size(133, 22);
            this.tsmiIrcDisconnect.Text = "Disconnect";
            this.tsmiIrcDisconnect.Click += new System.EventHandler(this.tsmiIrcDisconnect_Click);
            // 
            // tsmiConfig
            // 
            this.tsmiConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDataDirectory,
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
            // tsmiReload
            // 
            this.tsmiReload.Name = "tsmiReload";
            this.tsmiReload.Size = new System.Drawing.Size(186, 22);
            this.tsmiReload.Text = "Reload";
            this.tsmiReload.Click += new System.EventHandler(this.tsmiReload_Click);
            // 
            // tsmiRecompile
            // 
            this.tsmiRecompile.Name = "tsmiRecompile";
            this.tsmiRecompile.Size = new System.Drawing.Size(186, 22);
            this.tsmiRecompile.Text = "Recompile";
            this.tsmiRecompile.Click += new System.EventHandler(this.tsmiRecompile_Click);
            // 
            // tsmiReloadRtf
            // 
            this.tsmiReloadRtf.Name = "tsmiReloadRtf";
            this.tsmiReloadRtf.Size = new System.Drawing.Size(186, 22);
            this.tsmiReloadRtf.Text = "Reload RTF templates";
            this.tsmiReloadRtf.Click += new System.EventHandler(this.tsmiReloadRtf_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.DetectUrls = false;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 0);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(480, 407);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            // 
            // tbInput
            // 
            this.tbInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbInput.Location = new System.Drawing.Point(0, 407);
            this.tbInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(480, 29);
            this.tbInput.TabIndex = 1;
            this.tbInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbInput_KeyPress);
            // 
            // scChannelsLog
            // 
            this.scChannelsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scChannelsLog.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scChannelsLog.Location = new System.Drawing.Point(0, 25);
            this.scChannelsLog.Name = "scChannelsLog";
            // 
            // scChannelsLog.Panel1
            // 
            this.scChannelsLog.Panel1.Controls.Add(this.tsChannelSelection);
            this.scChannelsLog.Panel1MinSize = 100;
            // 
            // scChannelsLog.Panel2
            // 
            this.scChannelsLog.Panel2.Controls.Add(this.rtbLog);
            this.scChannelsLog.Panel2.Controls.Add(this.tbInput);
            this.scChannelsLog.Panel2MinSize = 100;
            this.scChannelsLog.Size = new System.Drawing.Size(684, 436);
            this.scChannelsLog.SplitterDistance = 200;
            this.scChannelsLog.TabIndex = 0;
            // 
            // tsChannelSelection
            // 
            this.tsChannelSelection.AutoSize = false;
            this.tsChannelSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsChannelSelection.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.tsChannelSelection.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsChannelSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnGlobal});
            this.tsChannelSelection.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.tsChannelSelection.Location = new System.Drawing.Point(0, 0);
            this.tsChannelSelection.Name = "tsChannelSelection";
            this.tsChannelSelection.Padding = new System.Windows.Forms.Padding(1);
            this.tsChannelSelection.Size = new System.Drawing.Size(200, 436);
            this.tsChannelSelection.TabIndex = 3;
            this.tsChannelSelection.Text = "toolStrip1";
            // 
            // tsbtnGlobal
            // 
            this.tsbtnGlobal.Checked = true;
            this.tsbtnGlobal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbtnGlobal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnGlobal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnGlobal.Name = "tsbtnGlobal";
            this.tsbtnGlobal.Size = new System.Drawing.Size(197, 25);
            this.tsbtnGlobal.Text = "Global";
            this.tsbtnGlobal.Click += new System.EventHandler(this.channelSelectioButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.scChannelsLog);
            this.Controls.Add(this.msMain);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.scChannelsLog.Panel1.ResumeLayout(false);
            this.scChannelsLog.Panel2.ResumeLayout(false);
            this.scChannelsLog.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scChannelsLog)).EndInit();
            this.scChannelsLog.ResumeLayout(false);
            this.tsChannelSelection.ResumeLayout(false);
            this.tsChannelSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.ToolStripMenuItem tsmiConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiDataDirectory;
        private System.Windows.Forms.ToolStripMenuItem tsmiReload;
        private System.Windows.Forms.ToolStripMenuItem tsmiRecompile;
        private System.Windows.Forms.SplitContainer scChannelsLog;
        private System.Windows.Forms.ToolStrip tsChannelSelection;
        private System.Windows.Forms.ToolStripButton tsbtnGlobal;
        private System.Windows.Forms.ToolStripMenuItem tsmiReloadRtf;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrc;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrcConnect;
        private System.Windows.Forms.ToolStripMenuItem tsmiIrcDisconnect;
    }
}

