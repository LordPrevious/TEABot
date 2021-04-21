using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEABot.UI
{
    public partial class DataDirectoryConfigForm : Form
    {
        public DataDirectoryConfigForm()
        {
            InitializeComponent();
        }

        private void BBrowse_Click(object sender, EventArgs e)
        {
            fbdBrowseDataDirectory.SelectedPath = tbDataDirectoryPath.Text;
            var res = fbdBrowseDataDirectory.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                tbDataDirectoryPath.Text = fbdBrowseDataDirectory.SelectedPath;
            }
        }

        private void BOk_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DataDirectory = tbDataDirectoryPath.Text;
            Properties.Settings.Default.Save();
            DialogResult = DialogResult.OK;
        }

        private void DataDirectoryConfigForm_Load(object sender, EventArgs e)
        {
            tbDataDirectoryPath.Text = Properties.Settings.Default.DataDirectory;
        }
    }
}
