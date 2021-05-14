using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TEABot.Bot;
using TEABot.TEAScript;

namespace TEABot.UI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Chat bot core
        /// </summary>
        private readonly TBChatBot mChatBot = new();

        /// <summary>
        /// The currently selected, active channel from mChannels/mGlobal
        /// </summary>
        private TBChannel mActiveChannel = null;

        /// <summary>
        /// The tool strip button associated with the currently active channel
        /// </summary>
        private ToolStripButton mActiveChannelButton = null;

        /// <summary>
        /// RTF template for outgoing messages
        /// </summary>
        private RtfTemplate mRTFOutgoing = new();
        /// <summary>
        /// RTF template for incoming messages
        /// </summary>
        private RtfTemplate mRTFIncoming = new();
        /// <summary>
        /// RTF template for messages entered via the input box
        /// </summary>
        private RtfTemplate mRTFLocal = new();
        /// <summary>
        /// RTF template for info messages
        /// </summary>
        private RtfTemplate mRTFInfo = new();
        /// <summary>
        /// RTF template for warning messages
        /// </summary>
        private RtfTemplate mRTFWarning = new();
        /// <summary>
        /// RTF template for error messages
        /// </summary>
        private RtfTemplate mRTFError = new();

        /// <summary>
        /// Open the form for data directory configuration
        /// </summary>
        private void OpenDataDirectoryConfigurationForm()
        {
            var ddcf = new DataDirectoryConfigForm();
            var res = ddcf.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                ReloadRTFTemplates();
                mChatBot.ReloadConfig(Properties.Settings.Default.DataDirectory);
                mChatBot.ReloadScripts(Properties.Settings.Default.DataDirectory);
                UpdateChannelList();
            }
        }

        /// <summary>
        /// Reload the logging RTF templates
        /// </summary>
        private void ReloadRTFTemplates()
        {
            LoadRtfTemplate(out mRTFOutgoing, "outgoing.rtf");
            LoadRtfTemplate(out mRTFIncoming, "incoming.rtf");
            LoadRtfTemplate(out mRTFLocal, "local.rtf");
            LoadRtfTemplate(out mRTFInfo, "info.rtf");
            LoadRtfTemplate(out mRTFWarning, "warning.rtf");
            LoadRtfTemplate(out mRTFError, "error.rtf");
        }

        /// <summary>
        /// Load a specific RTF template
        /// </summary>
        /// <param name="a_template">The template to load into</param>
        /// <param name="a_filename">The file to load from</param>
        private void LoadRtfTemplate(out RtfTemplate ao_template, string a_filename)
        {
            try
            {
                string templateText = File.ReadAllText(Path.Combine(Properties.Settings.Default.DataDirectory, a_filename));
                ao_template = new RtfTemplate(templateText);
            }
            catch (Exception e)
            {
                LogMessage(mRTFError, String.Format("Failed to load RTF template from \"{0}\": {1}",
                    a_filename,
                    e.Message));
                ao_template = new RtfTemplate();
            }
        }

        /// <summary>
        /// Update the channel selection list to show channels in mChannels
        /// </summary>
        private void UpdateChannelList()
        {
            // prepare map from channel name to button
            var channelButtons = new Dictionary<string, ToolStripButton>();

            // get current buttons
            foreach (var button in tsChannelSelection.Items.OfType<ToolStripButton>())
            {
                // keep only buttons which still have a channel
                if (mChatBot.Channels.ContainsKey(button.Text))
                {
                    channelButtons[button.Text] = button;
                }
                else if (button == mActiveChannelButton)
                {
                    // the active channel no longer exists
                    mActiveChannel = mChatBot.Global;
                    mActiveChannelButton = tsbtnGlobal;
                    tsbtnGlobal.Checked = true;
                }
            }

            // remove them all from the tool strip
            tsChannelSelection.Items.Clear();

            // add back the global channel button
            tsChannelSelection.Items.Add(tsbtnGlobal);

            // create buttons for missing channels
            var activeChannelName = mActiveChannel.Name.ToLowerInvariant();
            foreach (var channel in mChatBot.Channels.Keys)
            {
                if (!channelButtons.ContainsKey(channel))
                {
                    channelButtons[channel] = new ToolStripButton(channel, null, ChannelSelectioButton_Click)
                    {
                        DisplayStyle = ToolStripItemDisplayStyle.Text,
                        Checked = channel.Equals(activeChannelName)
                    };
                }
            }

            // add buttons back to toolstrip
            foreach (var button in channelButtons.Values)
            {
                tsChannelSelection.Items.Add(button);
            }
        }

        /// <summary>
        /// Log a message using the given RTF template and the global channel context
        /// </summary>
        /// <param name="a_template">The RTF template to use</param>
        /// <param name="a_message">The message to log</param>
        private void LogMessage(RtfTemplate a_template, string a_message)
        {
            LogMessage(a_template, a_message, mChatBot.Global);
        }

        /// <summary>
        /// Log a message using the given RTF template and the given channel's own identity as the message sender
        /// </summary>
        /// <param name="a_template">The RTF template to use</param>
        /// <param name="a_message">The message to log</param>
        /// <param name="a_channel">The channel for which this message is to be logged</param>
        private void LogMessage(RtfTemplate a_template, string a_message, TBChannel a_channel)
        {
            LogMessage(a_template, a_message, mChatBot.Global, a_channel.Configuration.Self);
        }

        /// <summary>
        /// Log a message using the given RTF template
        /// </summary>
        /// <param name="a_template">The RTF template to use</param>
        /// <param name="a_message">The message to log</param>
        /// <param name="a_channel">The channel for which this message is to be logged</param>
        /// <param name="a_sender">The sender os the message</param>
        private void LogMessage(RtfTemplate a_template, string a_message, TBChannel a_channel, string a_sender)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => LogMessage(a_template, a_message, a_channel, a_sender)));
                return;
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            try
            {
                rtbLog.SelectedRtf = a_template.Apply(a_message,
                    a_sender,
                    a_channel?.Configuration?.TimestampFormat ?? "HH:mm:ss")
                    + Environment.NewLine;
            }
            catch
            {
                // fallback for invalid RTF format: print message as-is
                rtbLog.SelectedText = a_message + Environment.NewLine;
            }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void MChatBot_OnChatMessage(TBChannel a_channel, TBMessageDirection a_direction, string a_sender, string a_message)
        {
            RtfTemplate template = a_direction switch
            {
                TBMessageDirection.SENT => mRTFOutgoing,
                TBMessageDirection.MANUAL => mRTFLocal,
                _ => mRTFIncoming,
            };
            LogMessage(template, a_message, a_channel, a_sender);
        }

        private void MChatBot_OnInfo(TBChannel a_channel, string a_message)
        {
            if (a_channel.Configuration.InfoLog)
            {
                LogMessage(mRTFInfo, a_message, a_channel, String.Empty);
            }
        }

        private void MChatBot_OnNotice(TBChannel a_channel, string a_message)
        {
            // Same as info but without filtering
            LogMessage(mRTFInfo, a_message, a_channel, String.Empty);
        }

        private void MChatBot_OnWarning(TBChannel a_channel, string a_message)
        {
            LogMessage(mRTFWarning, a_message, a_channel, String.Empty);
        }

        private void MChatBot_OnError(TBChannel a_channel, string a_message)
        {
            LogMessage(mRTFError, a_message, a_channel, String.Empty);
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void TbInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                var input = tbInput.Text;
                tbInput.Text = String.Empty;
                e.Handled = true;

                // send via chat bot
                mChatBot.SendMessage(mActiveChannel, input);
            }
        }

        private void DataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDataDirectoryConfigurationForm();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!Directory.Exists(Properties.Settings.Default.DataDirectory))
            {
                OpenDataDirectoryConfigurationForm();
            }
        }

        private void TsmiReload_Click(object sender, EventArgs e)
        {
            mChatBot.ReloadConfig(Properties.Settings.Default.DataDirectory);
            UpdateChannelList();
        }

        private void TsmiRecompile_Click(object sender, EventArgs e)
        {
            mChatBot.ReloadScripts(Properties.Settings.Default.DataDirectory);
            UpdateChannelList();
        }

        private void TsmiReloadRtf_Click(object sender, EventArgs e)
        {
            ReloadRTFTemplates();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // attach chatbot events
            mChatBot.OnChatMessage += MChatBot_OnChatMessage;
            mChatBot.OnInfo += MChatBot_OnInfo;
            mChatBot.OnNotice += MChatBot_OnNotice;
            mChatBot.OnWarning += MChatBot_OnWarning;
            mChatBot.OnError += MChatBot_OnError;
            // activate global channel
            mActiveChannel = mChatBot.Global;
            mActiveChannelButton = tsbtnGlobal;
            if (Directory.Exists(Properties.Settings.Default.DataDirectory))
            {
                // load and print motd
                try
                {
                    string motd = File.ReadAllText(Path.Combine(Properties.Settings.Default.DataDirectory, "motd.rtf"));
                    rtbLog.Rtf = motd;
                }
                catch
                {
                    // don't care, no motd given
                }
                // load config and scripts
                ReloadRTFTemplates();
                mChatBot.ReloadConfig(Properties.Settings.Default.DataDirectory);
                mChatBot.ReloadScripts(Properties.Settings.Default.DataDirectory);
                UpdateChannelList();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // cancel tasks and disconnect bot
            mChatBot.Disconnect();
            // detach chatbot events
            mChatBot.OnChatMessage -= MChatBot_OnChatMessage;
            mChatBot.OnInfo -= MChatBot_OnInfo;
            mChatBot.OnNotice -= MChatBot_OnNotice;
            mChatBot.OnWarning -= MChatBot_OnWarning;
            mChatBot.OnError -= MChatBot_OnError;
        }

        private void ChannelSelectioButton_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripButton tsbSender) return;
            if (tsbSender == mActiveChannelButton) return;
            mActiveChannelButton.Checked = false;
            if (mChatBot.Channels.TryGetValue(tsbSender.Text, out mActiveChannel))
            {
                mActiveChannelButton = tsbSender;
            }
            else
            {
                mActiveChannel = mChatBot.Global;
                mActiveChannelButton = tsbtnGlobal;
            }
            mActiveChannelButton.Checked = true;
        }

        private void TsmiIrcConnect_Click(object sender, EventArgs e)
        {
            mChatBot.Connect();
        }

        private void TsmiIrcDisconnect_Click(object sender, EventArgs e)
        {
            mChatBot.Disconnect();
        }
    }
}
