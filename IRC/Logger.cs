/*
 * $Id: Logger.cs 209 2006-12-22 19:11:35Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/Logger.cs $
 * $Rev: 209 $
 * $Author: meebey $
 * $Date: 2006-12-22 20:11:35 +0100 (Fri, 22 Dec 2006) $
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

using System.IO;
using System.Collections;

namespace Meebey.SmartIrc4net
{
#if LOG4NET
    /// <summary>
    ///
    /// </summary>
    public enum LogCategory
    {
        Main,
        Connection,
        Socket,
        Queue,
        IrcMessages,
        MessageTypes,
        MessageParser,
        ActionHandler,
        TimeHandler,
        MessageHandler,
        ChannelSyncing,
        UserSyncing,
        Modules,
        Dcc
    }

    /// <summary>
    ///
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    internal class Logger
    {
        private static SortedList _LoggerList = new SortedList();
        private static bool       _Init;
        
        private Logger()
        {
        }
        
        public static void Init()
        {
            if (_Init) {
                return;
            }
            
            _Init = true;
            
            /*
            FileInfo fi = new FileInfo("SmartIrc4net_log.config");
            if (fi.Exists) {
                    log4net.Config.DOMConfigurator.ConfigureAndWatch(fi);
            } else {
                log4net.Config.BasicConfigurator.Configure();
            }
            */
            
            _LoggerList[LogCategory.Main]           = log4net.LogManager.GetLogger("MAIN");
            _LoggerList[LogCategory.Socket]         = log4net.LogManager.GetLogger("SOCKET");
            _LoggerList[LogCategory.Queue]          = log4net.LogManager.GetLogger("QUEUE");
            _LoggerList[LogCategory.Connection]     = log4net.LogManager.GetLogger("CONNECTION");
            _LoggerList[LogCategory.IrcMessages]    = log4net.LogManager.GetLogger("IRCMESSAGE");
            _LoggerList[LogCategory.MessageParser]  = log4net.LogManager.GetLogger("MESSAGEPARSER");
            _LoggerList[LogCategory.MessageTypes]   = log4net.LogManager.GetLogger("MESSAGETYPES");
            _LoggerList[LogCategory.ActionHandler]  = log4net.LogManager.GetLogger("ACTIONHANDLER");
            _LoggerList[LogCategory.TimeHandler]    = log4net.LogManager.GetLogger("TIMEHANDLER");
            _LoggerList[LogCategory.MessageHandler] = log4net.LogManager.GetLogger("MESSAGEHANDLER");
            _LoggerList[LogCategory.ChannelSyncing] = log4net.LogManager.GetLogger("CHANNELSYNCING");
            _LoggerList[LogCategory.UserSyncing]    = log4net.LogManager.GetLogger("USERSYNCING");
            _LoggerList[LogCategory.Modules]        = log4net.LogManager.GetLogger("MODULES");
            _LoggerList[LogCategory.Dcc]            = log4net.LogManager.GetLogger("DCC");
        }

        public static log4net.ILog Main
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Main];
            }
        }

        public static log4net.ILog Socket
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Socket];
            }
        }

        public static log4net.ILog Queue
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Queue];
            }
        }

        public static log4net.ILog Connection
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Connection];
            }
        }

        public static log4net.ILog IrcMessages
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.IrcMessages];
            }
        }

        public static log4net.ILog MessageParser
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageParser];
            }
        }

        public static log4net.ILog MessageTypes
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageTypes];
            }
        }

        public static log4net.ILog ActionHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.ActionHandler];
            }
        }

        public static log4net.ILog TimeHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.TimeHandler];
            }
        }

        public static log4net.ILog MessageHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageHandler];
            }
        }

        public static log4net.ILog ChannelSyncing
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.ChannelSyncing];
            }
        }

        public static log4net.ILog UserSyncing
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.UserSyncing];
            }
        }

        public static log4net.ILog Modules
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Modules];
            }
        }

        public static log4net.ILog Dcc
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Dcc];
            }
        }
    }
#endif
}
