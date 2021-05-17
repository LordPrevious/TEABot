
namespace TEABot.UI
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.tlpWhat = new System.Windows.Forms.TableLayoutPanel();
            this.lblByVal = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersionCap = new System.Windows.Forms.Label();
            this.lblVersionVal = new System.Windows.Forms.Label();
            this.lblByCap = new System.Windows.Forms.Label();
            this.tlpLinks = new System.Windows.Forms.TableLayoutPanel();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblSourceCap = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.llblGitHub = new System.Windows.Forms.LinkLabel();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pImageMargin = new System.Windows.Forms.Panel();
            this.tlpWhat.SuspendLayout();
            this.tlpLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.pImageMargin.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpWhat
            // 
            this.tlpWhat.ColumnCount = 1;
            this.tlpWhat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWhat.Controls.Add(this.lblByVal, 0, 4);
            this.tlpWhat.Controls.Add(this.lblTitle, 0, 0);
            this.tlpWhat.Controls.Add(this.lblVersionCap, 0, 1);
            this.tlpWhat.Controls.Add(this.lblVersionVal, 0, 2);
            this.tlpWhat.Controls.Add(this.lblByCap, 0, 3);
            this.tlpWhat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpWhat.Location = new System.Drawing.Point(100, 0);
            this.tlpWhat.Name = "tlpWhat";
            this.tlpWhat.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.tlpWhat.RowCount = 5;
            this.tlpWhat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWhat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhat.Size = new System.Drawing.Size(184, 146);
            this.tlpWhat.TabIndex = 0;
            // 
            // lblByVal
            // 
            this.lblByVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblByVal.AutoSize = true;
            this.lblByVal.Location = new System.Drawing.Point(28, 116);
            this.lblByVal.Name = "lblByVal";
            this.lblByVal.Size = new System.Drawing.Size(153, 20);
            this.lblByVal.TabIndex = 4;
            this.lblByVal.Text = "marc markus mafalda björn";
            this.lblByVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(178, 56);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TEABot ɣ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVersionCap
            // 
            this.lblVersionCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersionCap.AutoSize = true;
            this.lblVersionCap.Location = new System.Drawing.Point(3, 56);
            this.lblVersionCap.Name = "lblVersionCap";
            this.lblVersionCap.Size = new System.Drawing.Size(45, 20);
            this.lblVersionCap.TabIndex = 1;
            this.lblVersionCap.Text = "Version";
            this.lblVersionCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVersionVal
            // 
            this.lblVersionVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersionVal.AutoSize = true;
            this.lblVersionVal.Location = new System.Drawing.Point(141, 76);
            this.lblVersionVal.Name = "lblVersionVal";
            this.lblVersionVal.Size = new System.Drawing.Size(40, 20);
            this.lblVersionVal.TabIndex = 2;
            this.lblVersionVal.Text = "0.0.0.0";
            this.lblVersionVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblByCap
            // 
            this.lblByCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblByCap.AutoSize = true;
            this.lblByCap.Location = new System.Drawing.Point(3, 96);
            this.lblByCap.Name = "lblByCap";
            this.lblByCap.Size = new System.Drawing.Size(20, 20);
            this.lblByCap.TabIndex = 3;
            this.lblByCap.Text = "by";
            this.lblByCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpLinks
            // 
            this.tlpLinks.AutoSize = true;
            this.tlpLinks.ColumnCount = 1;
            this.tlpLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLinks.Controls.Add(this.lblCopyright, 0, 0);
            this.tlpLinks.Controls.Add(this.lblSourceCap, 0, 1);
            this.tlpLinks.Controls.Add(this.btnOk, 0, 3);
            this.tlpLinks.Controls.Add(this.llblGitHub, 0, 2);
            this.tlpLinks.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlpLinks.Location = new System.Drawing.Point(0, 146);
            this.tlpLinks.Name = "tlpLinks";
            this.tlpLinks.RowCount = 4;
            this.tlpLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpLinks.Size = new System.Drawing.Size(284, 115);
            this.tlpLinks.TabIndex = 1;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(3, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(193, 25);
            this.lblCopyright.TabIndex = 0;
            this.lblCopyright.Text = "Copyright © 2021 Christopher Dillo";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSourceCap
            // 
            this.lblSourceCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSourceCap.AutoSize = true;
            this.lblSourceCap.Location = new System.Drawing.Point(3, 25);
            this.lblSourceCap.Name = "lblSourceCap";
            this.lblSourceCap.Size = new System.Drawing.Size(201, 25);
            this.lblSourceCap.TabIndex = 1;
            this.lblSourceCap.Text = "Source Code and License available at";
            this.lblSourceCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(206, 89);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // llblGitHub
            // 
            this.llblGitHub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.llblGitHub.AutoSize = true;
            this.llblGitHub.Location = new System.Drawing.Point(236, 50);
            this.llblGitHub.Name = "llblGitHub";
            this.llblGitHub.Size = new System.Drawing.Size(45, 25);
            this.llblGitHub.TabIndex = 4;
            this.llblGitHub.TabStop = true;
            this.llblGitHub.Text = "GitHub";
            this.llblGitHub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.llblGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlblGitHub_LinkClicked);
            // 
            // pbIcon
            // 
            this.pbIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbIcon.Image")));
            this.pbIcon.Location = new System.Drawing.Point(10, 10);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(80, 126);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIcon.TabIndex = 2;
            this.pbIcon.TabStop = false;
            // 
            // pImageMargin
            // 
            this.pImageMargin.Controls.Add(this.pbIcon);
            this.pImageMargin.Dock = System.Windows.Forms.DockStyle.Left;
            this.pImageMargin.Location = new System.Drawing.Point(0, 0);
            this.pImageMargin.Name = "pImageMargin";
            this.pImageMargin.Padding = new System.Windows.Forms.Padding(10);
            this.pImageMargin.Size = new System.Drawing.Size(100, 146);
            this.pImageMargin.TabIndex = 3;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tlpWhat);
            this.Controls.Add(this.pImageMargin);
            this.Controls.Add(this.tlpLinks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.tlpWhat.ResumeLayout(false);
            this.tlpWhat.PerformLayout();
            this.tlpLinks.ResumeLayout(false);
            this.tlpLinks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.pImageMargin.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpWhat;
        private System.Windows.Forms.TableLayoutPanel tlpLinks;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Panel pImageMargin;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblByVal;
        private System.Windows.Forms.Label lblVersionCap;
        private System.Windows.Forms.Label lblVersionVal;
        private System.Windows.Forms.Label lblByCap;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblSourceCap;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.LinkLabel llblGitHub;
    }
}