namespace TEABot.UI
{
    partial class DataDirectoryConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataDirectoryConfigForm));
            this.tbDataDirectoryPath = new System.Windows.Forms.TextBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.bBrowse = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.fbdBrowseDataDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // tbDataDirectoryPath
            // 
            this.tbDataDirectoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataDirectoryPath.Location = new System.Drawing.Point(12, 83);
            this.tbDataDirectoryPath.Name = "tbDataDirectoryPath";
            this.tbDataDirectoryPath.Size = new System.Drawing.Size(279, 29);
            this.tbDataDirectoryPath.TabIndex = 1;
            // 
            // lDescription
            // 
            this.lDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lDescription.Location = new System.Drawing.Point(12, 9);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(360, 71);
            this.lDescription.TabIndex = 0;
            this.lDescription.Text = "Please select the data directory where TEABot configuration and script files are " +
    "located.";
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(297, 83);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(75, 30);
            this.bBrowse.TabIndex = 2;
            this.bBrowse.Text = "Browse";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.BBrowse_Click);
            // 
            // bOk
            // 
            this.bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOk.Location = new System.Drawing.Point(297, 119);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 30);
            this.bOk.TabIndex = 3;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.BOk_Click);
            // 
            // DataDirectoryConfigForm
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.tbDataDirectoryPath);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataDirectoryConfigForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TEABot Data Directory";
            this.Load += new System.EventHandler(this.DataDirectoryConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDataDirectoryPath;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.FolderBrowserDialog fbdBrowseDataDirectory;
    }
}