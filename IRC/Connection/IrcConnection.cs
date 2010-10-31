/*
 * $Id: IrcConnection.cs 211 2007-02-01 22:08:32Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcConnection/IrcConnection.cs $
 * $Rev: 211 $
 * $Author: meebey $
 * $Date: 2007-02-01 23:08:32 +0100 (Thu, 01 Feb 2007) $
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2005 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 * 
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    /// 
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class IrcConnection
    {
        private string           _VersionNumber;
        private string           _VersionString;
        private string[]         _AddressList = {"localhost"};
        private int              _CurrentAddress;
        private int              _Port = 6667;
        private StreamReader     _Reader;
        private StreamWriter     _Writer;
        private ReadThread       _ReadThread;
        private WriteThread      _WriteThread;
        private IdleWorkerThread _IdleWorkerThread;
        private IrcTcpClient     _TcpClient;
        private Hashtable        _SendBuffer = Hashtable.Synchronized(new Hashtable());
        private int              _SendDelay = 200;
        private bool             _IsRegistered;
        private bool             _IsConnected;
        private bool             _IsConnectionError;
        private int              _ConnectTries;
        private bool             _AutoRetry;
        private int              _AutoRetryDelay = 30;
        private bool             _AutoReconnect;
        private Encoding         _Encoding = Encoding.Default;
        private int              _SocketReceiveTimeout  = 600;
        private int              _SocketSendTimeout = 600;
        private int              _IdleWorkerInterval = 60;
        private int              _PingInterval = 60;
        private int              _PingTimeout = 300;
        private DateTime         _LastPingSent;
        private DateTime         _LastPongReceived;
        private TimeSpan         _Lag;
        
        /// <event cref="OnReadLine">
        /// Raised when a \r\n terminated line is read from the socket
        /// </event>
        public event ReadLineEventHandler   OnReadLine;
        /// <event cref="OnWriteLine">
        /// Raised when a \r\n terminated line is written to the socket
        /// </event>
        public event WriteLineEventHandler  OnWriteLine;
        /// <event cref="OnConnect">
        /// Raised before the connect attempt
        /// </event>
        public event EventHandler           OnConnecting;
        /// <event cref="OnConnect">
        /// Raised on successful connect
        /// </event>
        public event EventHandler           OnConnected;
        /// <event cref="OnConnect">
        /// Raised before the connection is closed
        /// </event>
        public event EventHandler           OnDisconnecting;
        /// <event cref="OnConnect">
        /// Raised when the connection is closed
        /// </event>
        public event EventHandler           OnDisconnected;
        /// <event cref="OnConnectionError">
        /// Raised when the connection got into an error state
        /// </event>
        public event EventHandler           OnConnectionError;
        /// <event cref="AutoConnectErrorEventHandler">
        /// Raised when the connection got into an error state during auto connect loop
        /// </event>
        public event AutoConnectErrorEventHandler   OnAutoConnectError;
        
        /// <summary>
        /// When a connection error is detected this property will return true
        /// </summary>
        protected bool IsConnectionError {
            get {
                lock (this) {
                    return _IsConnectionError;
                }
            }
            set {
                lock (this) {
                    _IsConnectionError = value;
                }
            }
        }

        /// <summary>
        /// Gets the current address of the connection
        /// </summary>
        public string Address {
            get {
                return _AddressList[_CurrentAddress];
            }
        }

        /// <summary>
        /// Gets the address list of the connection
        /// </summary>
        public string[] AddressList {
            get {
                return _AddressList;
            }
        }

        /// <summary>
        /// Gets the used port of the connection
        /// </summary>
        public int Port {
            get {
                return _Port;
            }
        }

        /// <summary>
        /// By default nothing is done when the library looses the connection
        /// to the server.
        /// Default: false
        /// </summary>
        /// <value>
        /// true, if the library should reconnect on lost connections
        /// false, if the library should not take care of it
        /// </value>
        public bool AutoReconnect {
            get {
                return _AutoReconnect;
            }
            set {
#if LOG4NET
                if (value) {
                    Logger.Connection.Info("AutoReconnect enabled");
                } else {
                    Logger.Connection.Info("AutoReconnect disabled");
                }
#endif
                _AutoReconnect = value;
            }
        }

        /// <summary>
        /// If the library should retry to connect when the connection fails.
        /// Default: false
        /// </summary>
        /// <value>
        /// true, if the library should retry to connect
        /// false, if the library should not retry
        /// </value>
        public bool AutoRetry {
            get {
                return _AutoRetry;
            }
            set {
#if LOG4NET
                if (value) {
                    Logger.Connection.Info("AutoRetry enabled");
                } else {
                    Logger.Connection.Info("AutoRetry disabled");
                }
#endif
                _AutoRetry = value;
            }
        }

        /// <summary>
        /// Delay between retry attempts in Connect() in seconds.
        /// Default: 30
        /// </summary>
        public int AutoRetryDelay {
            get {
                return _AutoRetryDelay;
            }
            set {
                _AutoRetryDelay = value;
            }
        }

        /// <summary>
        /// To prevent flooding the IRC server, it's required to delay each
        /// message, given in milliseconds.
        /// Default: 200
        /// </summary>
        public int SendDelay {
            get {
                return _SendDelay;
            }
            set {
                _SendDelay = value;
            }
        }

        /// <summary>
        /// On successful registration on the IRC network, this is set to true.
        /// </summary>
        public bool IsRegistered {
            get {
                return _IsRegistered;
            }
        }

        /// <summary>
        /// On successful connect to the IRC server, this is set to true.
        /// </summary>
        public bool IsConnected {
            get {
                return _IsConnected;
            }
        }

        /// <summary>
        /// Gets the SmartIrc4net version number
        /// </summary>
        public string VersionNumber {
            get {
                return _VersionNumber;
            }
        }

        /// <summary>
        /// Gets the full SmartIrc4net version string
        /// </summary>
        public string VersionString {
            get {
                return _VersionString;
            }
        }

        /// <summary>
        /// Encoding which is used for reading and writing to the socket
        /// Default: encoding of the system
        /// </summary>
        public Encoding Encoding {
            get {
                return _Encoding;
            }
            set {
                _Encoding = value;
            }
        }

        /// <summary>
        /// Timeout in seconds for receiving data from the socket
        /// Default: 600
        /// </summary>
        public int SocketReceiveTimeout {
            get {
                return _SocketReceiveTimeout;
            }
            set {
                _SocketReceiveTimeout = value;
            }
        }
        
        /// <summary>
        /// Timeout in seconds for sending data to the socket
        /// Default: 600
        /// </summary>
        public int SocketSendTimeout {
            get {
                return _SocketSendTimeout;
            }
            set {
                _SocketSendTimeout = value;
            }
        }
        
        /// <summary>
        /// Interval in seconds to run the idle worker
        /// Default: 60
        /// </summary>
        public int IdleWorkerInterval {
            get {
                return _IdleWorkerInterval;
            }
            set {
                _IdleWorkerInterval = value;
            }
        }

        /// <summary>
        /// Interval in seconds to send a PING
        /// Default: 60
        /// </summary>
        public int PingInterval {
            get {
                return _PingInterval;
            }
            set {
                _PingInterval = value;
            }
        }
        
        /// <summary>
        /// Timeout in seconds for server response to a PING
        /// Default: 600
        /// </summary>
        public int PingTimeout {
            get {
                return _PingTimeout;
            }
            set {
                _PingTimeout = value;
            }
        }

        /// <summary>
        /// Latency between client and the server
        /// </summary>
        public TimeSpan Lag {
            get {
                return _Lag;
            }
        }

        /// <summary>
        /// Initializes the message queues, read and write thread
        /// </summary>
        public IrcConnection()
        {
#if LOG4NET
            Logger.Init();
            Logger.Main.Debug("IrcConnection created");
#endif
            _SendBuffer[Priority.High]        = Queue.Synchronized(new Queue());
            _SendBuffer[Priority.AboveMedium] = Queue.Synchronized(new Queue());
            _SendBuffer[Priority.Medium]      = Queue.Synchronized(new Queue());
            _SendBuffer[Priority.BelowMedium] = Queue.Synchronized(new Queue());
            _SendBuffer[Priority.Low]         = Queue.Synchronized(new Queue());

            // setup own callbacks
            OnReadLine        += new ReadLineEventHandler(_SimpleParser);
            OnConnectionError += new EventHandler(_OnConnectionError);

            _ReadThread  = new ReadThread(this);
            _WriteThread = new WriteThread(this);
            _IdleWorkerThread = new IdleWorkerThread(this);

            Assembly assm = Assembly.GetAssembly(this.GetType());
            AssemblyName assm_name = assm.GetName(false);

            AssemblyProductAttribute pr = (AssemblyProductAttribute)assm.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];

            _VersionNumber = assm_name.Version.ToString();
            _VersionString = pr.Product+" "+_VersionNumber;
        }
        
#if LOG4NET
        ~IrcConnection()
        {
            Logger.Main.Debug("IrcConnection destroyed");
        }
#endif
        
        /// <overloads>this method has 2 overloads</overloads>
        /// <summary>
        /// Connects to the specified server and port, when the connection fails
        /// the next server in the list will be used.
        /// </summary>
        /// <param name="addresslist">List of servers to connect to</param>
        /// <param name="port">Portnumber to connect to</param>
        /// <exception cref="CouldNotConnectException">The connection failed</exception>
        /// <exception cref="AlreadyConnectedException">If there is already an active connection</exception>
        public void Connect(string[] addresslist, int port)
        {
            if (_IsConnected) {
                throw new AlreadyConnectedException("Already connected to: "+Address+":"+Port);
            }

#if LOG4NET
            Logger.Connection.Info("connecting...");
#endif
            _ConnectTries++;
            _AddressList = (string[])addresslist.Clone();
            _Port = port;

            if (OnConnecting != null) {
                OnConnecting(this, EventArgs.Empty);
            }
            try {
                System.Net.IPAddress ip = System.Net.Dns.Resolve(Address).AddressList[0];
                _TcpClient = new IrcTcpClient();
                _TcpClient.NoDelay = true;
                _TcpClient.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
                // set timeout, after this the connection will be aborted
                _TcpClient.ReceiveTimeout = _SocketReceiveTimeout*1000;
                _TcpClient.SendTimeout = _SocketSendTimeout*1000;
                _TcpClient.Connect(ip, port);
                
                _Reader = new StreamReader(_TcpClient.GetStream(), _Encoding);
                _Writer = new StreamWriter(_TcpClient.GetStream(), _Encoding);
                
                if (_Encoding.GetPreamble().Length > 0) {
                    // HACK: we have an encoding that has some kind of preamble
                    // like UTF-8 has a BOM, this will confuse the IRCd!
                    // Thus we send a \r\n so the IRCd can safely ignore that
                    // garbage.
                    _Writer.WriteLine();
                }

                // Connection was succeful, reseting the connect counter
                _ConnectTries = 0;

                // updating the connection error state, so connecting is possible again
                IsConnectionError = false;
                _IsConnected = true;

                // lets power up our threads
                _ReadThread.Start();
                _WriteThread.Start();
                _IdleWorkerThread.Start();
                
#if LOG4NET
                Logger.Connection.Info("connected");
#endif
                if (OnConnected != null) {
                    OnConnected(this, EventArgs.Empty);
                }
            } catch (Exception e) {
                if (_Reader != null) {
                    try {
                        _Reader.Close();
                    } catch (ObjectDisposedException) {
                    }
                }
                if (_Writer != null) {
                    try {
                        _Writer.Close();
                    } catch (ObjectDisposedException) {
                    }
                }
                if (_TcpClient != null) {
                    _TcpClient.Close();
                }
                _IsConnected = false;
                IsConnectionError = true; 
                
#if LOG4NET
                Logger.Connection.Info("connection failed: "+e.Message);
#endif
                if (_AutoRetry &&
                    _ConnectTries <= 3) {
                    if (OnAutoConnectError != null) {
                        OnAutoConnectError(this, new AutoConnectErrorEventArgs(Address, Port, e));
                    }
#if LOG4NET
                    Logger.Connection.Debug("delaying new connect attempt for "+_AutoRetryDelay+" sec");
#endif
                    Thread.Sleep(_AutoRetryDelay * 1000);
                    _NextAddress();
                    Connect(_AddressList, _Port);
                } else {
                    OnDisconnected(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Connects to the specified server and port.
        /// </summary>
        /// <param name="address">Server address to connect to</param>
        /// <param name="port">Port number to connect to</param>
        public void Connect(string address, int port)
        {
            Connect(new string[] {address}, port);
        }

        /// <summary>
        /// Reconnects to the server
        /// </summary>
        /// <exception cref="NotConnectedException">
        /// If there was no active connection
        /// </exception>
        /// <exception cref="CouldNotConnectException">
        /// The connection failed
        /// </exception>
        /// <exception cref="AlreadyConnectedException">
        /// If there is already an active connection
        /// </exception>
        public void Reconnect()
        {
#if LOG4NET
            Logger.Connection.Info("reconnecting...");
#endif
            Disconnect();
            Connect(_AddressList, _Port);
        }
        
        /// <summary>
        /// Disconnects from the server
        /// </summary>
        /// <exception cref="NotConnectedException">
        /// If there was no active connection
        /// </exception>
        public void Disconnect()
        {
            if (!IsConnected) {
                throw new NotConnectedException("The connection could not be disconnected because there is no active connection");
            }
            
#if LOG4NET
            Logger.Connection.Info("disconnecting...");
#endif
            if (OnDisconnecting != null) {
                OnDisconnecting(this, EventArgs.Empty);
            }

            _ReadThread.Stop();
            _WriteThread.Stop();
            _TcpClient.Close();
            _IsConnected = false;
            _IsRegistered = false;
            
            if (OnDisconnected != null) {
                OnDisconnected(this, EventArgs.Empty);
            }

#if LOG4NET
            Logger.Connection.Info("disconnected");
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocking"></param>
        public void Listen(bool blocking)
        {
            if (blocking) {
                while (IsConnected) {
                    ReadLine(true);
                }
            } else {
                while (ReadLine(false).Length > 0) {
                    // loop as long as we receive messages
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Listen()
        {
            Listen(true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocking"></param>
        public void ListenOnce(bool blocking)
        {
            ReadLine(blocking);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ListenOnce()
        {
            ListenOnce(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocking"></param>
        /// <returns></returns>
        public string ReadLine(bool blocking)
        {
            string data = "";
            if (blocking) {
                // block till the queue has data, but bail out on connection error
                while (IsConnected &&
                       !IsConnectionError &&
                       _ReadThread.Queue.Count == 0) {
                    Thread.Sleep(10);
                }
            }

            if (IsConnected &&
                _ReadThread.Queue.Count > 0) {
                data = (string)(_ReadThread.Queue.Dequeue());
            }

            if (data != null && data.Length > 0) {
#if LOG4NET
                Logger.Queue.Debug("read: \""+data+"\"");
#endif
                if (OnReadLine != null) {
                    OnReadLine(this, new ReadLineEventArgs(data));
                }
            }

            if (IsConnectionError &&
                OnConnectionError != null) {
                OnConnectionError(this, EventArgs.Empty);
            }
            
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="priority"></param>
        public void WriteLine(string data, Priority priority)
        {
            if (priority == Priority.Critical) {
                if (!IsConnected) {
                    throw new NotConnectedException();
                }
                
                _WriteLine(data);
            } else {
                ((Queue)_SendBuffer[priority]).Enqueue(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void WriteLine(string data)
        {
            WriteLine(data, Priority.Medium);
        }

        private bool _WriteLine(string data)
        {
            if (IsConnected) {
                try {
                    _Writer.Write(data+"\r\n");
                    _Writer.Flush();
                } catch (IOException) {
#if LOG4NET
                    Logger.Socket.Warn("sending data failed, connection lost");
#endif
                    IsConnectionError = true;
                    return false;
                } catch (ObjectDisposedException) {
#if LOG4NET
                    Logger.Socket.Warn("sending data failed (stream error), connection lost");
#endif
                    IsConnectionError = true;
                    return false;
                }

#if LOG4NET
                Logger.Socket.Debug("sent: \""+data+"\"");
#endif
                if (OnWriteLine != null) {
                    OnWriteLine(this, new WriteLineEventArgs(data));
                }
                return true;
            }

            return false;
        }

        private void _NextAddress()
        {
            _CurrentAddress++;
            if (_CurrentAddress >= _AddressList.Length) {
                _CurrentAddress = 0;
            }
#if LOG4NET
            Logger.Connection.Info("set server to: "+Address);
#endif
        }

        private void _SimpleParser(object sender, ReadLineEventArgs args)
        {
            string   rawline = args.Line; 
            string[] rawlineex = rawline.Split(new char[] {' '});
            string   messagecode = "";

            if (rawline[0] == ':') {
                messagecode = rawlineex[1];
                
                ReplyCode replycode = ReplyCode.Null;
                try {
                    replycode = (ReplyCode)int.Parse(messagecode);
                } catch (FormatException) {
                }
                
                if (replycode != ReplyCode.Null) {
                    switch (replycode) {
                        case ReplyCode.Welcome:
                            _IsRegistered = true;
#if LOG4NET
                            Logger.Connection.Info("logged in");
#endif
                            break;
                    }
                } else {
                    switch (rawlineex[1]) {
                        case "PONG":
                            DateTime now = DateTime.Now;
                            _LastPongReceived = now;
                            _Lag = now - _LastPingSent;

#if LOG4NET
                            Logger.Connection.Debug("PONG received, took: "+_Lag.TotalMilliseconds+" ms");
#endif
                            break;
                    }
                }
            } else {
                messagecode = rawlineex[0];
                switch (messagecode) {
                    case "ERROR":
                        IsConnectionError = true;
                    break;
                }
            }
        }

        private void _OnConnectionError(object sender, EventArgs e)
        {
            try {
                if (AutoReconnect) {
                    // lets try to recover the connection
                    Reconnect();
                } else {
                    // make sure we clean up
                    Disconnect();
                }
            } catch (ConnectionException) {
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private class ReadThread
        {
            private IrcConnection  _Connection;
            private Thread         _Thread;
            private Queue          _Queue = Queue.Synchronized(new Queue());

            public Queue Queue {
                get {
                    return _Queue;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="connection"></param>
            public ReadThread(IrcConnection connection)
            {
                _Connection = connection;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Start()
            {
                _Thread = new Thread(new ThreadStart(_Worker));
                _Thread.Name = "ReadThread ("+_Connection.Address+":"+_Connection.Port+")";
                _Thread.IsBackground = true;
                _Thread.Start();
            }

            /// <summary>
            /// 
            /// </summary>
            public void Stop()
            {
                _Thread.Abort();
                try {
                    _Connection._Reader.Close();
                } catch (ObjectDisposedException) {
                }
            }

            private void _Worker()
            {
#if LOG4NET
                Logger.Socket.Debug("ReadThread started");
#endif
                try {
                    string data = "";
                    try {
                        while (_Connection.IsConnected &&
                               ((data = _Connection._Reader.ReadLine()) != null)) {
                            _Queue.Enqueue(data);
#if LOG4NET
                            Logger.Socket.Debug("received: \""+data+"\"");
#endif
                        }
                    } catch (IOException e) {
#if LOG4NET
                        Logger.Socket.Warn("IOException: "+e.Message);
#endif
                    } finally {
#if LOG4NET
                        Logger.Socket.Warn("connection lost");
#endif
                        _Connection.IsConnectionError = true;
                    }
                } catch (ThreadAbortException) {
                    Thread.ResetAbort();
#if LOG4NET
                    Logger.Socket.Debug("ReadThread aborted");
#endif
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class WriteThread
        {
            private IrcConnection  _Connection;
            private Thread         _Thread;
            private int            _HighCount;
            private int            _AboveMediumCount;
            private int            _MediumCount;
            private int            _BelowMediumCount;
            private int            _LowCount;
            private int            _AboveMediumSentCount;
            private int            _MediumSentCount;
            private int            _BelowMediumSentCount;
            private int            _AboveMediumThresholdCount = 4;
            private int            _MediumThresholdCount      = 2;
            private int            _BelowMediumThresholdCount = 1;
            private int            _BurstCount;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="connection"></param>
            public WriteThread(IrcConnection connection)
            {
                _Connection = connection;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Start()
            {
                _Thread = new Thread(new ThreadStart(_Worker));
                _Thread.Name = "WriteThread ("+_Connection.Address+":"+_Connection.Port+")";
                _Thread.IsBackground = true;
                _Thread.Start();
            }

            /// <summary>
            /// 
            /// </summary>
            public void Stop()
            {
                _Thread.Abort();
                try {
                    _Connection._Writer.Close();
                } catch (ObjectDisposedException) {
                }
            }

            private void _Worker()
            {
#if LOG4NET
                Logger.Socket.Debug("WriteThread started");
#endif
                try {
                    try {
                        while (_Connection.IsConnected) {
                            _CheckBuffer();
                            Thread.Sleep(_Connection._SendDelay);
                        }
                    } catch (IOException e) {
#if LOG4NET
                        Logger.Socket.Warn("IOException: "+e.Message);
#endif
                    } finally {
#if LOG4NET
                        Logger.Socket.Warn("connection lost");
#endif
                        _Connection.IsConnectionError = true;
                    }
                } catch (ThreadAbortException) {
                    Thread.ResetAbort();
#if LOG4NET
                    Logger.Socket.Debug("WriteThread aborted");
#endif
                }
            }

            #region WARNING: complex scheduler, don't even think about changing it!
            // WARNING: complex scheduler, don't even think about changing it!
            private void _CheckBuffer()
            {
                // only send data if we are succefully registered on the IRC network
                if (!_Connection._IsRegistered) {
                    return;
                }

                _HighCount        = ((Queue)_Connection._SendBuffer[Priority.High]).Count;
                _AboveMediumCount = ((Queue)_Connection._SendBuffer[Priority.AboveMedium]).Count;
                _MediumCount      = ((Queue)_Connection._SendBuffer[Priority.Medium]).Count;
                _BelowMediumCount = ((Queue)_Connection._SendBuffer[Priority.BelowMedium]).Count;
                _LowCount         = ((Queue)_Connection._SendBuffer[Priority.Low]).Count;

                if (_CheckHighBuffer() &&
                    _CheckAboveMediumBuffer() &&
                    _CheckMediumBuffer() &&
                    _CheckBelowMediumBuffer() &&
                    _CheckLowBuffer()) {
                    // everything is sent, resetting all counters
                    _AboveMediumSentCount = 0;
                    _MediumSentCount      = 0;
                    _BelowMediumSentCount = 0;
                    _BurstCount = 0;
                }

                if (_BurstCount < 3) {
                    _BurstCount++;
                    //_CheckBuffer();
                }
            }

            private bool _CheckHighBuffer()
            {
                if (_HighCount > 0) {
                    string data = (string)((Queue)_Connection._SendBuffer[Priority.High]).Dequeue();
                    if (_Connection._WriteLine(data) == false) {
#if LOG4NET
                        Logger.Queue.Warn("Sending data was not sucessful, data is requeued!");
#endif
                        ((Queue)_Connection._SendBuffer[Priority.High]).Enqueue(data);
                    }

                    if (_HighCount > 1) {
                        // there is more data to send
                        return false;
                    }
                }

                return true;
            }

            private bool _CheckAboveMediumBuffer()
            {
                if ((_AboveMediumCount > 0) &&
                    (_AboveMediumSentCount < _AboveMediumThresholdCount)) {
                    string data = (string)((Queue)_Connection._SendBuffer[Priority.AboveMedium]).Dequeue();
                    if (_Connection._WriteLine(data) == false) {
#if LOG4NET
                        Logger.Queue.Warn("Sending data was not sucessful, data is requeued!");
#endif
                        ((Queue)_Connection._SendBuffer[Priority.AboveMedium]).Enqueue(data);
                    }
                    _AboveMediumSentCount++;

                    if (_AboveMediumSentCount < _AboveMediumThresholdCount) {
                        return false;
                    }
                }

                return true;
            }

            private bool _CheckMediumBuffer()
            {
                if ((_MediumCount > 0) &&
                    (_MediumSentCount < _MediumThresholdCount)) {
                    string data = (string)((Queue)_Connection._SendBuffer[Priority.Medium]).Dequeue();
                    if (_Connection._WriteLine(data) == false) {
#if LOG4NET
                        Logger.Queue.Warn("Sending data was not sucessful, data is requeued!");
#endif
                        ((Queue)_Connection._SendBuffer[Priority.Medium]).Enqueue(data);
                    }
                    _MediumSentCount++;

                    if (_MediumSentCount < _MediumThresholdCount) {
                        return false;
                    }
                }

                return true;
            }

            private bool _CheckBelowMediumBuffer()
            {
                if ((_BelowMediumCount > 0) &&
                    (_BelowMediumSentCount < _BelowMediumThresholdCount)) {
                    string data = (string)((Queue)_Connection._SendBuffer[Priority.BelowMedium]).Dequeue();
                    if (_Connection._WriteLine(data) == false) {
#if LOG4NET
                        Logger.Queue.Warn("Sending data was not sucessful, data is requeued!");
#endif
                        ((Queue)_Connection._SendBuffer[Priority.BelowMedium]).Enqueue(data);
                    }
                    _BelowMediumSentCount++;

                    if (_BelowMediumSentCount < _BelowMediumThresholdCount) {
                        return false;
                    }
                }

                return true;
            }

            private bool _CheckLowBuffer()
            {
                if (_LowCount > 0) {
                    if ((_HighCount > 0) ||
                        (_AboveMediumCount > 0) ||
                        (_MediumCount > 0) ||
                        (_BelowMediumCount > 0)) {
                            return true;
                    }

                    string data = (string)((Queue)_Connection._SendBuffer[Priority.Low]).Dequeue();
                    if (_Connection._WriteLine(data) == false) {
#if LOG4NET
                        Logger.Queue.Warn("Sending data was not sucessful, data is requeued!");
#endif
                        ((Queue)_Connection._SendBuffer[Priority.Low]).Enqueue(data);
                    }

                    if (_LowCount > 1) {
                        return false;
                    }
                }

                return true;
            }
            // END OF WARNING, below this you can read/change again ;)
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        private class IdleWorkerThread
        {
            private IrcConnection   _Connection;
            private Thread          _Thread;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="connection"></param>
            public IdleWorkerThread(IrcConnection connection)
            {
                _Connection = connection;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Start()
            {
                DateTime now = DateTime.Now;
                _Connection._LastPingSent = now;
                _Connection._LastPongReceived = now;
                
                _Thread = new Thread(new ThreadStart(_Worker));
                _Thread.Name = "IdleWorkerThread ("+_Connection.Address+":"+_Connection.Port+")";
                _Thread.IsBackground = true;
                _Thread.Start();
            }

            /// <summary>
            /// 
            /// </summary>
            public void Stop()
            {
                _Thread.Abort();
            }

            private void _Worker()
            {
#if LOG4NET
                Logger.Socket.Debug("IdleWorkerThread started");
#endif
                try {
                   while (_Connection.IsConnected ) {
                       if (_Connection.IsRegistered) {
                           DateTime now = DateTime.Now;
                           int last_ping_sent = (int)(now - _Connection._LastPingSent).TotalSeconds;
                           int last_pong_rcvd = (int)(now - _Connection._LastPongReceived).TotalSeconds;
                           // determins if the resoponse time is ok
                           if (last_ping_sent < _Connection._PingTimeout) {
                               // determines if it need to send another ping yet
                               if (last_pong_rcvd > _Connection._PingInterval) {
                                   _Connection.WriteLine(Rfc2812.Ping(_Connection.Address), Priority.Critical);
                                   _Connection._LastPingSent = now;
                                   _Connection._LastPongReceived = now;
                               } // else connection is fine, just continue
                           } else {
#if LOG4NET
                               Logger.Socket.Warn("ping timeout, connection lost");
#endif
                               _Connection.IsConnectionError = true;
                               break;
                           }
                       }
                       Thread.Sleep(_Connection._IdleWorkerInterval);
                   }
                } catch (ThreadAbortException) {
                    Thread.ResetAbort();
#if LOG4NET
                    Logger.Socket.Debug("IdleWorkerThread aborted");
#endif
                }
            }
        }
    }
}
