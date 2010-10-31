/*
 * $Id: Channel.cs 207 2006-01-11 10:08:56Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcClient/Channel.cs $
 * $Rev: 207 $
 * $Author: meebey $
 * $Date: 2006-01-11 11:08:56 +0100 (Wed, 11 Jan 2006) $
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
using System.Collections;
using System.Collections.Specialized;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    /// 
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class Channel
    {
        private string           _Name;
        private string           _Key       = String.Empty;
        private Hashtable        _Users     = Hashtable.Synchronized(new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer()));
        private Hashtable        _Ops       = Hashtable.Synchronized(new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer()));
        private Hashtable        _Voices    = Hashtable.Synchronized(new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer()));
        private StringCollection _Bans      = new StringCollection();
        private string           _Topic     = String.Empty;
        private int              _UserLimit;
        private string           _Mode      = String.Empty;
        private DateTime         _ActiveSyncStart;
        private DateTime         _ActiveSyncStop;
        private TimeSpan         _ActiveSyncTime;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"> </param>
        internal Channel(string name)
        {
            _Name = name;
            _ActiveSyncStart = DateTime.Now;
        }

#if LOG4NET
        ~Channel()
        {
            Logger.ChannelSyncing.Debug("Channel ("+Name+") destroyed");
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public string Name {
            get {
                return _Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public string Key {
            get {
                return _Key;
            }
            set {
                _Key = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public Hashtable Users {
            get {
                return (Hashtable)_Users.Clone();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeUsers {
            get {
                return _Users;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public Hashtable Ops {
            get {
                return (Hashtable)_Ops.Clone();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeOps {
            get {
                return _Ops;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public Hashtable Voices {
            get {
                return (Hashtable)_Voices.Clone();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeVoices {
            get {
                return _Voices;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public StringCollection Bans {
            get {
                return _Bans;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public string Topic {
            get {
                return _Topic;
            }
            set {
                _Topic = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public int UserLimit {
            get {
                return _UserLimit;
            }
            set {
                _UserLimit = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public string Mode {
            get {
                return _Mode;
            }
            set {
                _Mode = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public DateTime ActiveSyncStart {
            get {
                return _ActiveSyncStart;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public DateTime ActiveSyncStop {
            get {
                return _ActiveSyncStop;
            }
            set {
                _ActiveSyncStop = value;
                _ActiveSyncTime = _ActiveSyncStop.Subtract(_ActiveSyncStart);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value> </value>
        public TimeSpan ActiveSyncTime {
            get {
                return _ActiveSyncTime;
            }
        }
    }
}
