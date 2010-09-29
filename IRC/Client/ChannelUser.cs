/*
 * $Id: ChannelUser.cs 198 2005-06-08 16:50:11Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcClient/ChannelUser.cs $
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
    /// This class manages the information of a user within a channel.
    /// </summary>
    /// <remarks>
    /// only used with channel sync
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class ChannelUser
    {
        private string    _Channel;
        private IrcUser   _IrcUser;
        private bool      _IsOp;
        private bool      _IsVoice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"> </param>
        /// <param name="ircuser"> </param>
        internal ChannelUser(string channel, IrcUser ircuser)
        {
            _Channel = channel;
            _IrcUser = ircuser;
        }

#if LOG4NET
        ~ChannelUser()
        {
            Logger.ChannelSyncing.Debug("ChannelUser ("+Channel+":"+IrcUser.Nick+") destroyed");
        }
#endif

        /// <summary>
        /// Gets the channel name
        /// </summary>
        public string Channel {
            get {
                return _Channel;
            }
        }

        /// <summary>
        /// Gets the server operator status of the user
        /// </summary>
        public bool IsIrcOp {
            get {
                return _IrcUser.IsIrcOp;
            }
        }

        /// <summary>
        /// Gets or sets the op flag of the user (+o)
        /// </summary>
        /// <remarks>
        /// only used with channel sync
        /// </remarks>
        public bool IsOp {
            get {
                return _IsOp;
            }
            set {
                _IsOp = value;
            }
        }

        /// <summary>
        /// Gets or sets the voice flag of the user (+v)
        /// </summary>
        /// <remarks>
        /// only used with channel sync
        /// </remarks>
        public bool IsVoice {
            get {
                return _IsVoice;
            }
            set {
                _IsVoice = value;
            }
        }

        /// <summary>
        /// Gets the away status of the user
        /// </summary>
        public bool IsAway {
            get {
                return _IrcUser.IsAway;
            }
        }

        /// <summary>
        /// Gets the underlaying IrcUser object
        /// </summary>
        public IrcUser IrcUser {
            get {
                return _IrcUser;
            }
        }

        /// <summary>
        /// Gets the nickname of the user
        /// </summary>
        public string Nick {
            get {
                return _IrcUser.Nick;
            }
        }

        /// <summary>
        /// Gets the identity (username) of the user, which is used by some IRC networks for authentication.
        /// </summary>
        public string Ident {
            get {
                return _IrcUser.Ident;
            }
        }

        /// <summary>
        /// Gets the hostname of the user,
        /// </summary>
        public string Host {
            get {
                return _IrcUser.Host;
            }
        }

        /// <summary>
        /// Gets the supposed real name of the user.
        /// </summary>
        public string Realname {
            get {
                return _IrcUser.Realname;
            }
        }

        /// <summary>
        /// Gets the server the user is connected to.
        /// </summary>
        /// <value> </value>
        public string Server {
            get {
                return _IrcUser.Server;
            }
        }

        /// <summary>
        /// Gets or sets the count of hops between you and the user's server
        /// </summary>
        public int HopCount {
            get {
                return _IrcUser.HopCount;
            }
        }

        /// <summary>
        /// Gets the list of channels the user has joined
        /// </summary>
        public string[] JoinedChannels {
            get {
                return _IrcUser.JoinedChannels;
            }
        }
    }
}
