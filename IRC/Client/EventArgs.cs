/*
 * $Id: EventArgs.cs 203 2005-06-10 01:42:42Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcClient/EventArgs.cs $
 * $Rev: 203 $
 * $Author: meebey $
 * $Date: 2005-06-10 03:42:42 +0200 (Fri, 10 Jun 2005) $
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
using System.Collections.Specialized;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    ///
    /// </summary>
    public class ActionEventArgs : CtcpEventArgs
    {
        private string _ActionMessage;
        
        public string ActionMessage {
            get {
                return _ActionMessage;
            }
        }
         
        internal ActionEventArgs(IrcMessageData data, string actionmsg) : base(data, "ACTION", actionmsg)
        {
            _ActionMessage = actionmsg;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class CtcpEventArgs : IrcEventArgs
    {
        private string _CtcpCommand;
        private string _CtcpParameter;
        
        public string CtcpCommand {
            get {
                return _CtcpCommand;
            }
        }
         
        public string CtcpParameter {
            get {
                return _CtcpParameter;
            }
        }
         
        internal CtcpEventArgs(IrcMessageData data, string ctcpcmd, string ctcpparam) : base(data)
        {
            _CtcpCommand = ctcpcmd;
            _CtcpParameter = ctcpparam;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class ErrorEventArgs : IrcEventArgs
    {
        private string _ErrorMessage;
        
        public string ErrorMessage {
            get {
                return _ErrorMessage;
            }
        }
         
        internal ErrorEventArgs(IrcMessageData data, string errormsg) : base(data)
        {
            _ErrorMessage = errormsg;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class MotdEventArgs : IrcEventArgs
    {
        private string _MotdMessage;
        
        public string MotdMessage {
            get {
                return _MotdMessage;
            }
        }
         
        internal MotdEventArgs(IrcMessageData data, string motdmsg) : base(data)
        {
            _MotdMessage = motdmsg;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class PingEventArgs : IrcEventArgs
    {
        private string _PingData;
        
        public string PingData {
            get {
                return _PingData;
            }
        }
         
        internal PingEventArgs(IrcMessageData data, string pingdata) : base(data)
        {
            _PingData = pingdata;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class PongEventArgs : IrcEventArgs
    {
        private TimeSpan _Lag;
        
        public TimeSpan Lag {
            get {
                return _Lag;
            }
        }

        internal PongEventArgs(IrcMessageData data, TimeSpan lag) : base(data)
        {
            _Lag = lag;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class KickEventArgs : IrcEventArgs
    {
        private string _Channel;
        private string _Who;
        private string _Whom;
        private string _KickReason;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }
         
        public string Whom {
            get {
                return _Whom;
            }
        }
         
        public string KickReason {
            get {
                return _KickReason;
            }
        }
         
        internal KickEventArgs(IrcMessageData data, string channel, string who, string whom, string kickreason) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
            _KickReason = kickreason;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class JoinEventArgs : IrcEventArgs
    {
        private string _Channel;
        private string _Who;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }
         
        internal JoinEventArgs(IrcMessageData data, string channel, string who) : base(data)
        {
            _Channel = channel;
            _Who = who;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class NamesEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string[] _UserList;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string[] UserList {
            get {
                return _UserList;
            }
        }
         
        internal NamesEventArgs(IrcMessageData data, string channel, string[] userlist) : base(data)
        {
            _Channel = channel;
            _UserList = userlist;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class InviteEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }
         
        internal InviteEventArgs(IrcMessageData data, string channel, string who) : base(data)
        {
            _Channel = channel;
            _Who = who;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class PartEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _PartMessage;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }
         
        public string PartMessage {
            get {
                return _PartMessage;
            }
        }
        
        internal PartEventArgs(IrcMessageData data, string channel, string who, string partmessage) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _PartMessage = partmessage;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public class WhoEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Nick;
        private string   _Ident;
        private string   _Host;
        private string   _Realname;
        private bool     _IsAway;
        private bool     _IsOp;
        private bool     _IsVoice;
        private bool     _IsIrcOp;
        private string   _Server;
        private int      _HopCount;

        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Nick {
            get {
                return _Nick;
            }
        }
         
        public string Ident {
            get {
                return _Ident;
            }
        }
        
        public string Host {
            get {
                return _Host;
            }
        }
        
        public string Realname {
            get {
                return _Realname;
            }
        }
        
        public bool IsAway {
            get {
                return _IsAway;
            }
        }
        
        public bool IsOp {
            get {
                return _IsOp;
            }
        }
        
        public bool IsVoice {
            get {
                return _IsVoice;
            }
        }
        
        public bool IsIrcOp {
            get {
                return _IsIrcOp;
            }
        }
        
        public string Server {
            get {
                return _Server;
            }
        }

        public int HopCount {
            get {
                return _HopCount;
            }
        }

        internal WhoEventArgs(IrcMessageData data, string channel, string nick, string ident, string host, string realname, bool away, bool op, bool voice, bool ircop, string server, int hopcount) : base(data)
        {
            _Channel = channel;
            _Nick = nick;
            _Ident = ident;
            _Host = host;
            _Realname = realname;
            _IsAway = away;
            _IsOp = op;
            _IsVoice = voice;
            _IsIrcOp = ircop;
            _Server = server;
            _HopCount = hopcount;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class QuitEventArgs : IrcEventArgs
    {
        private string   _Who;
        private string   _QuitMessage;
        
        public string Who {
            get {
                return _Who;
            }
        }

        public string QuitMessage {
            get {
                return _QuitMessage;
            }
        }
        
        internal QuitEventArgs(IrcMessageData data, string who, string quitmessage) : base(data)
        {
            _Who = who;
            _QuitMessage = quitmessage;
        }
    }


    /// <summary>
    ///
    /// </summary>
    public class AwayEventArgs : IrcEventArgs
    {
        private string   _Who;
        private string   _AwayMessage;
        
        public string Who {
            get {
                return _Who;
            }
        }

        public string AwayMessage{
            get {
                return _AwayMessage;
            }
        }
        
        internal AwayEventArgs(IrcMessageData data, string who, string awaymessage) : base(data)
        {
            _Who = who;
            _AwayMessage = awaymessage;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public class NickChangeEventArgs : IrcEventArgs
    {
        private string   _OldNickname;
        private string   _NewNickname;
        
        public string OldNickname {
            get {
                return _OldNickname;
            }
        }

        public string NewNickname {
            get {
                return _NewNickname;
            }
        }
        
        internal NickChangeEventArgs(IrcMessageData data, string oldnick, string newnick) : base(data)
        {
            _OldNickname = oldnick;
            _NewNickname = newnick;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class TopicEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Topic;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Topic {
            get {
                return _Topic;
            }
        }
        
        internal TopicEventArgs(IrcMessageData data, string channel, string topic) : base(data)
        {
            _Channel = channel;
            _Topic = topic;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class TopicChangeEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _NewTopic;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string NewTopic {
            get {
                return _NewTopic;
            }
        }
        
        internal TopicChangeEventArgs(IrcMessageData data, string channel, string who, string newtopic) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _NewTopic = newtopic;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class BanEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Hostmask;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Hostmask {
            get {
                return _Hostmask;
            }
        }
        
        internal BanEventArgs(IrcMessageData data, string channel, string who, string hostmask) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Hostmask = hostmask;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class UnbanEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Hostmask;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Hostmask {
            get {
                return _Hostmask;
            }
        }
        
        internal UnbanEventArgs(IrcMessageData data, string channel, string who, string hostmask) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Hostmask = hostmask;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class OpEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal OpEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class DeopEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal DeopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class HalfopEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal HalfopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class DehalfopEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal DehalfopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }
    
    /// <summary>
    ///
    /// </summary>
    public class VoiceEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal VoiceEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class DevoiceEventArgs : IrcEventArgs
    {
        private string   _Channel;
        private string   _Who;
        private string   _Whom;
        
        public string Channel {
            get {
                return _Channel;
            }
        }

        public string Who {
            get {
                return _Who;
            }
        }

        public string Whom {
            get {
                return _Whom;
            }
        }
        
        internal DevoiceEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            _Channel = channel;
            _Who = who;
            _Whom = whom;
        }
    }

}
