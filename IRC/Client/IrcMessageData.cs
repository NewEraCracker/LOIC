/*
 * $Id: IrcMessageData.cs 198 2005-06-08 16:50:11Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcClient/IrcMessageData.cs $
 * $Rev: 198 $
 * $Author: meebey $
 * $Date: 2005-06-08 18:50:11 +0200 (Wed, 08 Jun 2005) $
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

namespace Meebey.SmartIrc4net
{
    /// <summary>
    /// This class contains an IRC message in a parsed form
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class IrcMessageData
    {
        private IrcClient   _Irc;
        private string      _From;
        private string      _Nick;
        private string      _Ident;
        private string      _Host;
        private string      _Channel;
        private string      _Message;
        private string[]    _MessageArray;
        private string      _RawMessage;
        private string[]    _RawMessageArray;
        private ReceiveType _Type;
        private ReplyCode   _ReplyCode;
        
        /// <summary>
        /// Gets the IrcClient object the message originated from
        /// </summary>
        public IrcClient Irc {
            get {
                return _Irc;
            }
        }
        
        /// <summary>
        /// Gets the combined nickname, identity and hostname of the user that sent the message
        /// </summary>
        /// <example>
        /// nick!ident@host
        /// </example>
        public string From {
            get {
                return _From;
            }
        }
        
        /// <summary>
        /// Gets the nickname of the user that sent the message
        /// </summary>
        public string Nick {
            get {
                return _Nick;
            }
        }

        /// <summary>
        /// Gets the identity (username) of the user that sent the message
        /// </summary>
        public string Ident {
            get {
                return _Ident;
            }
        }

        /// <summary>
        /// Gets the hostname of the user that sent the message
        /// </summary>
        public string Host {
            get {
                return _Host;
            }
        }

        /// <summary>
        /// Gets the channel the message originated from
        /// </summary>
        public string Channel {
            get {
                return _Channel;
            }
        }
        
        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message {
            get {
                return _Message;
            }
        }
        
        /// <summary>
        /// Gets the message as an array of strings (splitted by space)
        /// </summary>
        public string[] MessageArray {
            get {
                return _MessageArray;
            }
        }
        
        /// <summary>
        /// Gets the raw message sent by the server
        /// </summary>
        public string RawMessage {
            get {
                return _RawMessage;
            }
        }
        
        /// <summary>
        /// Gets the raw message sent by the server as array of strings (splitted by space)
        /// </summary>
        public string[] RawMessageArray {
            get {
                return _RawMessageArray;
            }
        }

        /// <summary>
        /// Gets the message type
        /// </summary>
        public ReceiveType Type {
            get {
                return _Type;
            }
        }

        /// <summary>
        /// Gets the message reply code
        /// </summary>
        public ReplyCode ReplyCode {
            get {
                return _ReplyCode;
            }
        }

        /// <summary>
        /// Constructor to create an instace of IrcMessageData
        /// </summary>
        /// <param name="ircclient">IrcClient the message originated from</param>
        /// <param name="from">combined nickname, identity and host of the user that sent the message (nick!ident@host)</param>
        /// <param name="nick">nickname of the user that sent the message</param>
        /// <param name="ident">identity (username) of the userthat sent the message</param>
        /// <param name="host">hostname of the user that sent the message</param>
        /// <param name="channel">channel the message originated from</param>
        /// <param name="message">message</param>
        /// <param name="rawmessage">raw message sent by the server</param>
        /// <param name="type">message type</param>
        /// <param name="replycode">message reply code</param>
        public IrcMessageData(IrcClient ircclient, string from, string nick, string ident, string host, string channel, string message, string rawmessage, ReceiveType type, ReplyCode replycode)
        {
            _Irc = ircclient;
            _RawMessage = rawmessage;
            _RawMessageArray = rawmessage.Split(new char[] {' '});
            _Type = type;
            _ReplyCode = replycode;
            _From = from;
            _Nick = nick;
            _Ident = ident;
            _Host = host;
            _Channel = channel;
            if (message != null) {
                // message is optional
                _Message = message;
                _MessageArray = message.Split(new char[] {' '});
            }
        }
    }
}
