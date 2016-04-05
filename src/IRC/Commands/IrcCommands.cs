/*
 * $Id: IrcCommands.cs 198 2005-06-08 16:50:11Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcCommands/IrcCommands.cs $
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
    ///
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class IrcCommands: IrcConnection
    {
#if LOG4NET
        public IrcCommands()
        {
            Logger.Main.Debug("IrcCommands created");
        }
#endif
        
#if LOG4NET
        ~IrcCommands()
        {
            Logger.Main.Debug("IrcCommands destroyed");
        }
#endif
    
        // API commands
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public void SendMessage(SendType type, string destination, string message, Priority priority)
        {
            switch(type) {
                case SendType.Message:
                    RfcPrivmsg(destination, message, priority);
                break;
                case SendType.Action:
                    RfcPrivmsg(destination, "\x1"+"ACTION "+message+"\x1", priority);
                break;
                case SendType.Notice:
                    RfcNotice(destination, message, priority);
                break;
                case SendType.CtcpRequest:
                    RfcPrivmsg(destination, "\x1"+message+"\x1", priority);
                break;
                case SendType.CtcpReply:
                    RfcNotice(destination, "\x1"+message+"\x1", priority);
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        public void SendMessage(SendType type, string destination, string message)
        {
            SendMessage(type, destination, message, Priority.Medium);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public void SendReply(IrcMessageData data, string message, Priority priority)
        {
            switch (data.Type) {
                case ReceiveType.ChannelMessage:
                    SendMessage(SendType.Message, data.Channel, message, priority);
                break;
                case ReceiveType.QueryMessage:
                    SendMessage(SendType.Message, data.Nick, message, priority);
                break;
                case ReceiveType.QueryNotice:
                    SendMessage(SendType.Notice, data.Nick, message, priority);
                break;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public void SendReply(IrcMessageData data, string message)
        {
            SendReply(data, message, Priority.Medium);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void Op(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "+o "+nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void Op(string channel, string nickname)
        {
            WriteLine(Rfc2812.Mode(channel, "+o "+nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void Deop(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "-o "+nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void Deop(string channel, string nickname)
        {
            WriteLine(Rfc2812.Mode(channel, "-o "+nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void Voice(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "+v "+nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void Voice(string channel, string nickname)
        {
            WriteLine(Rfc2812.Mode(channel, "+v "+nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void Devoice(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "-v "+nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void Devoice(string channel, string nickname)
        {
            WriteLine(Rfc2812.Mode(channel, "-v "+nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void Ban(string channel, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "+b"), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void Ban(string channel)
        {
            WriteLine(Rfc2812.Mode(channel, "+b"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="hostmask"></param>
        /// <param name="priority"></param>
        public void Ban(string channel, string hostmask, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "+b "+hostmask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="hostmask"></param>
        public void Ban(string channel, string hostmask)
        {
            WriteLine(Rfc2812.Mode(channel, "+b "+hostmask));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="hostmask"></param>
        /// <param name="priority"></param>
        public void Unban(string channel, string hostmask, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "-b "+hostmask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="hostmask"></param>
        public void Unban(string channel, string hostmask)
        {
            WriteLine(Rfc2812.Mode(channel, "-b "+hostmask));
        }
        
        // non-RFC commands
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void Halfop(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Mode(channel, "+h "+nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void Dehalfop(string channel, string nickname)
        {
            WriteLine(Rfc2812.Mode(channel, "-h "+nickname));
        }

        // RFC commands
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="priority"></param>
        public void RfcPass(string password, Priority priority)
        {
            WriteLine(Rfc2812.Pass(password), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        public void RfcPass(string password)
        {
            WriteLine(Rfc2812.Pass(password));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="usermode"></param>
        /// <param name="realname"></param>
        /// <param name="priority"></param>
        public void RfcUser(string username, int usermode, string realname, Priority priority)
        {
            WriteLine(Rfc2812.User(username, usermode, realname), priority);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="usermode"></param>
        /// <param name="realname"></param>
        public void RfcUser(string username, int usermode, string realname)
        {
            WriteLine(Rfc2812.User(username, usermode, realname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="priority"></param>
        public void RfcOper(string name, string password, Priority priority)
        {
            WriteLine(Rfc2812.Oper(name, password), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public void RfcOper(string name, string password)
        {
            WriteLine(Rfc2812.Oper(name, password));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public void RfcPrivmsg(string destination, string message, Priority priority)
        {
            WriteLine(Rfc2812.Privmsg(destination, message), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        public void RfcPrivmsg(string destination, string message)
        {
            WriteLine(Rfc2812.Privmsg(destination, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public void RfcNotice(string destination, string message, Priority priority)
        {
            WriteLine(Rfc2812.Notice(destination, message), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        public void RfcNotice(string destination, string message)
        {
            WriteLine(Rfc2812.Notice(destination, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcJoin(string channel, Priority priority)
        {
            WriteLine(Rfc2812.Join(channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void RfcJoin(string channel)
        {
            WriteLine(Rfc2812.Join(channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="priority"></param>
        public void RfcJoin(string[] channels, Priority priority)
        {
            WriteLine(Rfc2812.Join(channels), priority);
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        public void RfcJoin(string[] channels)
        {
            WriteLine(Rfc2812.Join(channels));
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        /// <param name="priority"></param>
        public void RfcJoin(string channel, string key, Priority priority)
        {
            WriteLine(Rfc2812.Join(channel, key), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        public void RfcJoin(string channel, string key)
        {
            WriteLine(Rfc2812.Join(channel, key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="keys"></param>
        /// <param name="priority"></param>
        public void RfcJoin(string[] channels, string[] keys, Priority priority)
        {
            WriteLine(Rfc2812.Join(channels, keys), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="keys"></param>
        public void RfcJoin(string[] channels, string[] keys)
        {
            WriteLine(Rfc2812.Join(channels, keys));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcPart(string channel, Priority priority)
        {
            WriteLine(Rfc2812.Part(channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void RfcPart(string channel)
        {
            WriteLine(Rfc2812.Part(channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="priority"></param>
        public void RfcPart(string[] channels, Priority priority)
        {
            WriteLine(Rfc2812.Part(channels), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        public void RfcPart(string[] channels)
        {
            WriteLine(Rfc2812.Part(channels));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="partmessage"></param>
        /// <param name="priority"></param>
        public void RfcPart(string channel, string partmessage, Priority priority)
        {
            WriteLine(Rfc2812.Part(channel, partmessage), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="partmessage"></param>
        public void RfcPart(string channel, string partmessage)
        {
            WriteLine(Rfc2812.Part(channel, partmessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="partmessage"></param>
        /// <param name="priority"></param>
        public void RfcPart(string[] channels, string partmessage, Priority priority)
        {
            WriteLine(Rfc2812.Part(channels, partmessage), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="partmessage"></param>
        public void RfcPart(string[] channels, string partmessage)
        {
            WriteLine(Rfc2812.Part(channels, partmessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void RfcKick(string channel, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channel, nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        public void RfcKick(string channel, string nickname)
        {
            WriteLine(Rfc2812.Kick(channel, nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void RfcKick(string[] channels, string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channels, nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nickname"></param>
        public void RfcKick(string[] channels, string nickname)
        {
            WriteLine(Rfc2812.Kick(channels, nickname));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nicknames"></param>
        /// <param name="priority"></param>
        public void RfcKick(string channel, string[] nicknames, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channel, nicknames), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nicknames"></param>
        public void RfcKick(string channel, string[] nicknames)
        {
            WriteLine(Rfc2812.Kick(channel, nicknames));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nicknames"></param>
        /// <param name="priority"></param>
        public void RfcKick(string[] channels, string[] nicknames, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channels, nicknames), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nicknames"></param>
        public void RfcKick(string[] channels, string[] nicknames)
        {
            WriteLine(Rfc2812.Kick(channels, nicknames));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcKick(string channel, string nickname, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channel, nickname, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        public void RfcKick(string channel, string nickname, string comment)
        {
            WriteLine(Rfc2812.Kick(channel, nickname, comment));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcKick(string[] channels, string nickname, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channels, nickname, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        public void RfcKick(string[] channels, string nickname, string comment)
        {
            WriteLine(Rfc2812.Kick(channels, nickname, comment));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nicknames"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcKick(string channel, string[] nicknames, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channel, nicknames, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nicknames"></param>
        /// <param name="comment"></param>
        public void RfcKick(string channel, string[] nicknames, string comment)
        {
            WriteLine(Rfc2812.Kick(channel, nicknames, comment));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nicknames"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcKick(string[] channels, string[] nicknames, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Kick(channels, nicknames, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="nicknames"></param>
        /// <param name="comment"></param>
        public void RfcKick(string[] channels, string[] nicknames, string comment)
        {
            WriteLine(Rfc2812.Kick(channels, nicknames, comment));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcMotd(Priority priority)
        {
            WriteLine(Rfc2812.Motd(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcMotd()
        {
            WriteLine(Rfc2812.Motd());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcMotd(string target, Priority priority)
        {
            WriteLine(Rfc2812.Motd(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcMotd(string target)
        {
            WriteLine(Rfc2812.Motd(target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcLuser(Priority priority)
        {
            WriteLine(Rfc2812.Luser(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcLuser()
        {
            WriteLine(Rfc2812.Luser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="priority"></param>
        public void RfcLuser(string mask, Priority priority)
        {
            WriteLine(Rfc2812.Luser(mask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        public void RfcLuser(string mask)
        {
            WriteLine(Rfc2812.Luser(mask));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcLuser(string mask, string target, Priority priority)
        {
            WriteLine(Rfc2812.Luser(mask, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="target"></param>
        public void RfcLuser(string mask, string target)
        {
            WriteLine(Rfc2812.Luser(mask, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcVersion(Priority priority)
        {
            WriteLine(Rfc2812.Version(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcVersion()
        {
            WriteLine(Rfc2812.Version());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcVersion(string target, Priority priority)
        {
            WriteLine(Rfc2812.Version(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcVersion(string target)
        {
            WriteLine(Rfc2812.Version(target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcStats(Priority priority)
        {
            WriteLine(Rfc2812.Stats(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcStats()
        {
            WriteLine(Rfc2812.Stats());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="priority"></param>
        public void RfcStats(string query, Priority priority)
        {
            WriteLine(Rfc2812.Stats(query), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        public void RfcStats(string query)
        {
            WriteLine(Rfc2812.Stats(query));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcStats(string query, string target, Priority priority)
        {
            WriteLine(Rfc2812.Stats(query, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="target"></param>
        public void RfcStats(string query, string target)
        {
            WriteLine(Rfc2812.Stats(query, target));
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RfcLinks()
        {
            WriteLine(Rfc2812.Links());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servermask"></param>
        /// <param name="priority"></param>
        public void RfcLinks(string servermask, Priority priority)
        {
            WriteLine(Rfc2812.Links(servermask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servermask"></param>
        public void RfcLinks(string servermask)
        {
            WriteLine(Rfc2812.Links(servermask));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteserver"></param>
        /// <param name="servermask"></param>
        /// <param name="priority"></param>
        public void RfcLinks(string remoteserver, string servermask, Priority priority)
        {
            WriteLine(Rfc2812.Links(remoteserver, servermask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteserver"></param>
        /// <param name="servermask"></param>
        public void RfcLinks(string remoteserver, string servermask)
        {
            WriteLine(Rfc2812.Links(remoteserver, servermask));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcTime(Priority priority)
        {
            WriteLine(Rfc2812.Time(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcTime()
        {
            WriteLine(Rfc2812.Time());
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcTime(string target, Priority priority)
        {
            WriteLine(Rfc2812.Time(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcTime(string target)
        {
            WriteLine(Rfc2812.Time(target));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetserver"></param>
        /// <param name="port"></param>
        /// <param name="priority"></param>
        public void RfcConnect(string targetserver, string port, Priority priority)
        {
            WriteLine(Rfc2812.Connect(targetserver, port), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetserver"></param>
        /// <param name="port"></param>
        public void RfcConnect(string targetserver, string port)
        {
            WriteLine(Rfc2812.Connect(targetserver, port));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetserver"></param>
        /// <param name="port"></param>
        /// <param name="remoteserver"></param>
        /// <param name="priority"></param>
        public void RfcConnect(string targetserver, string port, string remoteserver, Priority priority)
        {
            WriteLine(Rfc2812.Connect(targetserver, port, remoteserver), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetserver"></param>
        /// <param name="port"></param>
        /// <param name="remoteserver"></param>
        public void RfcConnect(string targetserver, string port, string remoteserver)
        {
            WriteLine(Rfc2812.Connect(targetserver, port, remoteserver));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcTrace(Priority priority)
        {
            WriteLine(Rfc2812.Trace(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcTrace()
        {
            WriteLine(Rfc2812.Trace());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcTrace(string target, Priority priority)
        {
            WriteLine(Rfc2812.Trace(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcTrace(string target)
        {
            WriteLine(Rfc2812.Trace(target));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcAdmin(Priority priority)
        {
            WriteLine(Rfc2812.Admin(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcAdmin()
        {
            WriteLine(Rfc2812.Admin());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcAdmin(string target, Priority priority)
        {
            WriteLine(Rfc2812.Admin(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcAdmin(string target)
        {
            WriteLine(Rfc2812.Admin(target));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcInfo(Priority priority)
        {
            WriteLine(Rfc2812.Info(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcInfo()
        {
            WriteLine(Rfc2812.Info());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcInfo(string target, Priority priority)
        {
            WriteLine(Rfc2812.Info(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcInfo(string target)
        {
            WriteLine(Rfc2812.Info(target));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcServlist(Priority priority)
        {
            WriteLine(Rfc2812.Servlist(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcServlist()
        {
            WriteLine(Rfc2812.Servlist());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="priority"></param>
        public void RfcServlist(string mask, Priority priority)
        {
            WriteLine(Rfc2812.Servlist(mask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        public void RfcServlist(string mask)
        {
            WriteLine(Rfc2812.Servlist(mask));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="type"></param>
        /// <param name="priority"></param>
        public void RfcServlist(string mask, string type, Priority priority)
        {
            WriteLine(Rfc2812.Servlist(mask, type), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="type"></param>
        public void RfcServlist(string mask, string type)
        {
            WriteLine(Rfc2812.Servlist(mask, type));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="servicename"></param>
        /// <param name="servicetext"></param>
        /// <param name="priority"></param>
        public void RfcSquery(string servicename, string servicetext, Priority priority)
        {
            WriteLine(Rfc2812.Squery(servicename, servicetext), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servicename"></param>
        /// <param name="servicetext"></param>
        public void RfcSquery(string servicename, string servicetext)
        {
            WriteLine(Rfc2812.Squery(servicename, servicetext));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcList(string channel, Priority priority)
        {
            WriteLine(Rfc2812.List(channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void RfcList(string channel)
        {
            WriteLine(Rfc2812.List(channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="priority"></param>
        public void RfcList(string[] channels, Priority priority)
        {
            WriteLine(Rfc2812.List(channels), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        public void RfcList(string[] channels)
        {
            WriteLine(Rfc2812.List(channels));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcList(string channel, string target, Priority priority)
        {
            WriteLine(Rfc2812.List(channel, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="target"></param>
        public void RfcList(string channel, string target)
        {
            WriteLine(Rfc2812.List(channel, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcList(string[] channels, string target, Priority priority)
        {
            WriteLine(Rfc2812.List(channels, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="target"></param>
        public void RfcList(string[] channels, string target)
        {
            WriteLine(Rfc2812.List(channels, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcNames(string channel, Priority priority)
        {
            WriteLine(Rfc2812.Names(channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void RfcNames(string channel)
        {
            WriteLine(Rfc2812.Names(channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="priority"></param>
        public void RfcNames(string[] channels, Priority priority)
        {
            WriteLine(Rfc2812.Names(channels), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        public void RfcNames(string[] channels)
        {
            WriteLine(Rfc2812.Names(channels));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcNames(string channel, string target, Priority priority)
        {
            WriteLine(Rfc2812.Names(channel, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="target"></param>
        public void RfcNames(string channel, string target)
        {
            WriteLine(Rfc2812.Names(channel, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcNames(string[] channels, string target, Priority priority)
        {
            WriteLine(Rfc2812.Names(channels, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="target"></param>
        public void RfcNames(string[] channels, string target)
        {
            WriteLine(Rfc2812.Names(channels, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcTopic(string channel, Priority priority)
        {
            WriteLine(Rfc2812.Topic(channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void RfcTopic(string channel)
        {
            WriteLine(Rfc2812.Topic(channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="newtopic"></param>
        /// <param name="priority"></param>
        public void RfcTopic(string channel, string newtopic, Priority priority)
        {
            WriteLine(Rfc2812.Topic(channel, newtopic), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="newtopic"></param>
        public void RfcTopic(string channel, string newtopic)
        {
            WriteLine(Rfc2812.Topic(channel, newtopic));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcMode(string target, Priority priority)
        {
            WriteLine(Rfc2812.Mode(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcMode(string target)
        {
            WriteLine(Rfc2812.Mode(target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newmode"></param>
        /// <param name="priority"></param>
        public void RfcMode(string target, string newmode, Priority priority)
        {
            WriteLine(Rfc2812.Mode(target, newmode), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newmode"></param>
        public void RfcMode(string target, string newmode)
        {
            WriteLine(Rfc2812.Mode(target, newmode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="distribution"></param>
        /// <param name="info"></param>
        /// <param name="priority"></param>
        public void RfcService(string nickname, string distribution, string info, Priority priority)
        {
            WriteLine(Rfc2812.Service(nickname, distribution, info), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="distribution"></param>
        /// <param name="info"></param>
        public void RfcService(string nickname, string distribution, string info)
        {
            WriteLine(Rfc2812.Service(nickname, distribution, info));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcInvite(string nickname, string channel, Priority priority)
        {
            WriteLine(Rfc2812.Invite(nickname, channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="channel"></param>
        public void RfcInvite(string nickname, string channel)
        {
            WriteLine(Rfc2812.Invite(nickname, channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newnickname"></param>
        /// <param name="priority"></param>
        public void RfcNick(string newnickname, Priority priority)
        {
            WriteLine(Rfc2812.Nick(newnickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newnickname"></param>
        public void RfcNick(string newnickname)
        {
            WriteLine(Rfc2812.Nick(newnickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcWho(Priority priority)
        {
            WriteLine(Rfc2812.Who(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcWho()
        {
            WriteLine(Rfc2812.Who());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="priority"></param>
        public void RfcWho(string mask, Priority priority)
        {
            WriteLine(Rfc2812.Who(mask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        public void RfcWho(string mask)
        {
            WriteLine(Rfc2812.Who(mask));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="ircop"></param>
        /// <param name="priority"></param>
        public void RfcWho(string mask, bool ircop, Priority priority)
        {
            WriteLine(Rfc2812.Who(mask, ircop), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="ircop"></param>
        public void RfcWho(string mask, bool ircop)
        {
            WriteLine(Rfc2812.Who(mask, ircop));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="priority"></param>
        public void RfcWhois(string mask, Priority priority)
        {
            WriteLine(Rfc2812.Whois(mask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        public void RfcWhois(string mask)
        {
            WriteLine(Rfc2812.Whois(mask));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masks"></param>
        /// <param name="priority"></param>
        public void RfcWhois(string[] masks, Priority priority)
        {
            WriteLine(Rfc2812.Whois(masks), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masks"></param>
        public void RfcWhois(string[] masks)
        {
            WriteLine(Rfc2812.Whois(masks));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mask"></param>
        /// <param name="priority"></param>
        public void RfcWhois(string target, string mask, Priority priority)
        {
            WriteLine(Rfc2812.Whois(target, mask), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mask"></param>
        public void RfcWhois(string target, string mask)
        {
            WriteLine(Rfc2812.Whois(target, mask));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="masks"></param>
        /// <param name="priority"></param>
        public void RfcWhois(string target, string[] masks, Priority priority)
        {
            WriteLine(Rfc2812.Whois(target ,masks), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="masks"></param>
        public void RfcWhois(string target, string[] masks)
        {
            WriteLine(Rfc2812.Whois(target, masks));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        public void RfcWhowas(string nickname)
        {
            WriteLine(Rfc2812.Whowas(nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string[] nicknames, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nicknames), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        public void RfcWhowas(string[] nicknames)
        {
            WriteLine(Rfc2812.Whowas(nicknames));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="count"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string nickname, string count, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nickname, count), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="count"></param>
        public void RfcWhowas(string nickname, string count)
        {
            WriteLine(Rfc2812.Whowas(nickname, count));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="count"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string[] nicknames, string count, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nicknames, count), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="count"></param>
        public void RfcWhowas(string[] nicknames, string count)
        {
            WriteLine(Rfc2812.Whowas(nicknames, count));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="count"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string nickname, string count, string target, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nickname, count, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="count"></param>
        /// <param name="target"></param>
        public void RfcWhowas(string nickname, string count, string target)
        {
            WriteLine(Rfc2812.Whowas(nickname, count, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="count"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcWhowas(string[] nicknames, string count, string target, Priority priority)
        {
            WriteLine(Rfc2812.Whowas(nicknames, count, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="count"></param>
        /// <param name="target"></param>
        public void RfcWhowas(string[] nicknames, string count, string target)
        {
            WriteLine(Rfc2812.Whowas(nicknames, count, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcKill(string nickname, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Kill(nickname, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="comment"></param>
        public void RfcKill(string nickname, string comment)
        {
            WriteLine(Rfc2812.Kill(nickname, comment));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="priority"></param>
        public void RfcPing(string server, Priority priority)
        {
            WriteLine(Rfc2812.Ping(server), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        public void RfcPing(string server)
        {
            WriteLine(Rfc2812.Ping(server));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="server2"></param>
        /// <param name="priority"></param>
        public void RfcPing(string server, string server2, Priority priority)
        {
            WriteLine(Rfc2812.Ping(server, server2), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="server2"></param>
        public void RfcPing(string server, string server2)
        {
            WriteLine(Rfc2812.Ping(server, server2));
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="priority"></param>
        public void RfcPong(string server, Priority priority)
        {
            WriteLine(Rfc2812.Pong(server), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        public void RfcPong(string server)
        {
            WriteLine(Rfc2812.Pong(server));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="server2"></param>
        /// <param name="priority"></param>
        public void RfcPong(string server, string server2, Priority priority)
        {
            WriteLine(Rfc2812.Pong(server, server2), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="server2"></param>
        public void RfcPong(string server, string server2)
        {
            WriteLine(Rfc2812.Pong(server, server2));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcAway(Priority priority)
        {
            WriteLine(Rfc2812.Away(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcAway()
        {
            WriteLine(Rfc2812.Away());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="awaytext"></param>
        /// <param name="priority"></param>
        public void RfcAway(string awaytext, Priority priority)
        {
            WriteLine(Rfc2812.Away(awaytext), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="awaytext"></param>
        public void RfcAway(string awaytext)
        {
            WriteLine(Rfc2812.Away(awaytext));
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RfcRehash()
        {
            WriteLine(Rfc2812.Rehash());
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RfcDie()
        {
            WriteLine(Rfc2812.Die());
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RfcRestart()
        {
            WriteLine(Rfc2812.Restart());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="priority"></param>
        public void RfcSummon(string user, Priority priority)
        {
            WriteLine(Rfc2812.Summon(user), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void RfcSummon(string user)
        {
            WriteLine(Rfc2812.Summon(user));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcSummon(string user, string target, Priority priority)
        {
            WriteLine(Rfc2812.Summon(user, target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        public void RfcSummon(string user, string target)
        {
            WriteLine(Rfc2812.Summon(user, target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="channel"></param>
        /// <param name="priority"></param>
        public void RfcSummon(string user, string target, string channel, Priority priority)
        {
            WriteLine(Rfc2812.Summon(user, target, channel), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="channel"></param>
        public void RfcSummon(string user, string target, string channel)
        {
            WriteLine(Rfc2812.Summon(user, target, channel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcUsers(Priority priority)
        {
            WriteLine(Rfc2812.Users(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcUsers()
        {
            WriteLine(Rfc2812.Users());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="priority"></param>
        public void RfcUsers(string target, Priority priority)
        {
            WriteLine(Rfc2812.Users(target), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void RfcUsers(string target)
        {
            WriteLine(Rfc2812.Users(target));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallopstext"></param>
        /// <param name="priority"></param>
        public void RfcWallops(string wallopstext, Priority priority)
        {
            WriteLine(Rfc2812.Wallops(wallopstext), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallopstext"></param>
        public void RfcWallops(string wallopstext)
        {
            WriteLine(Rfc2812.Wallops(wallopstext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void RfcUserhost(string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Userhost(nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        public void RfcUserhost(string nickname)
        {
            WriteLine(Rfc2812.Userhost(nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="priority"></param>
        public void RfcUserhost(string[] nicknames, Priority priority)
        {
            WriteLine(Rfc2812.Userhost(nicknames), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        public void RfcUserhost(string[] nicknames)
        {
            WriteLine(Rfc2812.Userhost(nicknames));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="priority"></param>
        public void RfcIson(string nickname, Priority priority)
        {
            WriteLine(Rfc2812.Ison(nickname), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        public void RfcIson(string nickname)
        {
            WriteLine(Rfc2812.Ison(nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        /// <param name="priority"></param>
        public void RfcIson(string[] nicknames, Priority priority)
        {
            WriteLine(Rfc2812.Ison(nicknames), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nicknames"></param>
        public void RfcIson(string[] nicknames)
        {
            WriteLine(Rfc2812.Ison(nicknames));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void RfcQuit(Priority priority)
        {
            WriteLine(Rfc2812.Quit(), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RfcQuit()
        {
            WriteLine(Rfc2812.Quit());
        }

        public void RfcQuit(string quitmessage, Priority priority)
        {
            WriteLine(Rfc2812.Quit(quitmessage), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quitmessage"></param>
        public void RfcQuit(string quitmessage)
        {
            WriteLine(Rfc2812.Quit(quitmessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="comment"></param>
        /// <param name="priority"></param>
        public void RfcSquit(string server, string comment, Priority priority)
        {
            WriteLine(Rfc2812.Squit(server, comment), priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="comment"></param>
        public void RfcSquit(string server, string comment)
        {
            WriteLine(Rfc2812.Squit(server, comment));
        }
    }
}
