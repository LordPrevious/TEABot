using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace TEABot.UI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            llblGitHub.Text = Properties.Resources.GitHubUrl;
            lblVersionVal.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void LlblGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Properties.Resources.GitHubUrl,
                UseShellExecute = true
            });
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
