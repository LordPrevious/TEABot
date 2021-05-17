using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        /// Whether to automatically connect to the IRC server after startup
        /// </summary>
        private bool mAutoConnect = true;

        /// <summary>
        /// Do not use data directory from settings, but from command line
        /// </summary>
        private bool mOverrideDataDirectory = false;

        /// <summary>
        /// Data directory from command line
        /// </summary>
        private string mCmdDataDirectory = String.Empty;

        /// <summary>
        /// Chat bot core
        /// </summary>
        private readonly TBChatBot mChatBot = new();

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
        /// Get the configuration data directory path
        /// </summary>
        /// <returns>Data directory path</returns>
        private string GetDataDirectory()
        {
            return mOverrideDataDirectory ? mCmdDataDirectory : Properties.Settings.Default.DataDirectory;
        }

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
                var dataDir = GetDataDirectory();
                mChatBot.ReloadConfig(dataDir);
                mChatBot.ReloadScripts(dataDir);
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
                string templateText = File.ReadAllText(Path.Combine(GetDataDirectory(), a_filename));
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

        /// <summary>
        /// Handle connection status dependend display and access
        /// </summary>
        /// <param name="a_connectionStatus">The bot connection status</param>
        private void UpdateConnectionStatusDisplay(TBConnectionStatus a_connectionStatus)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { UpdateConnectionStatusDisplay(a_connectionStatus); }));
            }

            if (a_connectionStatus.IrcClientRunning)
            {
                if (a_connectionStatus.IrcClientConnected)
                {
                    tsslIrcStatus.Image = Icons.ic_connected;
                    tsmiIrcReconnect.Enabled = true;
                }
                else
                {
                    tsslIrcStatus.Image = Icons.ic_connecting;
                    tsmiIrcReconnect.Enabled = false;
                }
                tsmiIrcConnect.Enabled = false;
            }
            else
            {
                tsslIrcStatus.Image = Icons.ic_disconnected;
                tsmiIrcConnect.Enabled = true;
                tsmiIrcReconnect.Enabled = false;
            }
            if (a_connectionStatus.WebSocketServerRunning)
            {
                tsslWebSocketStatus.Image = (a_connectionStatus.WebSocketClientCount > 0)
                    ? Icons.ic_connected
                    : Icons.ic_connecting;
                tsslWebSocketStatus.Text = a_connectionStatus.WebSocketClientCount.ToString();
                tsmiRestartWebSocket.Enabled = true;
            }
            else
            {
                tsslWebSocketStatus.Image = Icons.ic_connection_disabled;
                tsslWebSocketStatus.Text = String.Empty;
                tsmiRestartWebSocket.Enabled = false;
            }
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

        private void MChatBot_OnConnectionStatusChanged(TBChatBot a_sender, TBConnectionStatus a_connectionStatus)
        {
            UpdateConnectionStatusDisplay(a_connectionStatus);
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void DataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDataDirectoryConfigurationForm();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!mOverrideDataDirectory && !Directory.Exists(Properties.Settings.Default.DataDirectory))
            {
                OpenDataDirectoryConfigurationForm();
            }
        }

        private void TsmiReload_Click(object sender, EventArgs e)
        {
            mChatBot.ReloadConfig(GetDataDirectory());
        }

        private void TsmiRecompile_Click(object sender, EventArgs e)
        {
            mChatBot.ReloadScripts(GetDataDirectory());
        }

        private void TsmiReloadRtf_Click(object sender, EventArgs e)
        {
            ReloadRTFTemplates();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // handle command line arguments
            var cmdArgs = Environment.GetCommandLineArgs();
            // disable autoconnect
            if (cmdArgs.Any(a => a.Equals("/noConnect", StringComparison.InvariantCultureIgnoreCase)))
            {
                mAutoConnect = false;
            }
            // override data directory
            if ((cmdArgs.Length > 1)
                && Directory.Exists(cmdArgs.Last()))
            {
                mCmdDataDirectory = cmdArgs.Last();
                mOverrideDataDirectory = true;
                // block opening data directory configuration form
                tsmiDataDirectory.Enabled = false;
            }

            // set initial connection status
            UpdateConnectionStatusDisplay(mChatBot.ConnectionStatus);

            // attach chatbot events
            mChatBot.OnChatMessage += MChatBot_OnChatMessage;
            mChatBot.OnInfo += MChatBot_OnInfo;
            mChatBot.OnNotice += MChatBot_OnNotice;
            mChatBot.OnWarning += MChatBot_OnWarning;
            mChatBot.OnError += MChatBot_OnError;
            mChatBot.OnConnectionStatusChanged += MChatBot_OnConnectionStatusChanged;
            // load config
            var dataDir = GetDataDirectory();
            if (Directory.Exists(dataDir))
            {
                // load and print motd
                try
                {
                    string motd = File.ReadAllText(Path.Combine(dataDir, "motd.rtf"));
                    rtbLog.Rtf = motd;
                }
                catch
                {
                    // don't care, no motd given
                }
                // load config and scripts
                ReloadRTFTemplates();
                mChatBot.ReloadConfig(dataDir);
                mChatBot.ReloadScripts(dataDir);
                // autoconnect
                if (mAutoConnect)
                {
                    mChatBot.Connect();
                }
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

        private void TsmiIrcConnect_Click(object sender, EventArgs e)
        {
            mChatBot.Connect();
        }

        private void TsmiIrcDisconnect_Click(object sender, EventArgs e)
        {
            mChatBot.Disconnect();
        }

        private void TsmiTBGitHub_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Properties.Resources.GitHubUrl,
                UseShellExecute = true
            });
        }

        private void TsmiTBExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TsmiTBAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog(this);
        }

        private void tsmiIrcReconnect_Click(object sender, EventArgs e)
        {
            mChatBot.ReconnectIrc();
        }

        private void tsmiRestartWebSocket_Click(object sender, EventArgs e)
        {
            mChatBot.RestartWebSocketServer();
        }
    }
}
