/*
 * $Id: Rfc2812.cs 198 2005-06-08 16:50:11Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/IrcCommands/Rfc2812.cs $
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

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    ///
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class Rfc2812
    {
        // nickname   =  ( letter / special ) *8( letter / digit / special / "-" )
        // letter     =  %x41-5A / %x61-7A       ; A-Z / a-z
        // digit      =  %x30-39                 ; 0-9
        // special    =  %x5B-60 / %x7B-7D
        //                  ; "[", "]", "\", "`", "_", "^", "{", "|", "}"
        private static Regex _NicknameRegex = new Regex(@"^[A-Za-z\[\]\\`_^{|}][A-Za-z0-9\[\]\\`_\-^{|}]+$", RegexOptions.Compiled);
        
        private Rfc2812()
        {
        }
        
        /// <summary>
        /// Checks if the passed nickname is valid according to the RFC
        ///
        /// Use with caution, many IRC servers are not conform with this!
        /// </summary>
        public static bool IsValidNickname(string nickname)
        {
            if ((nickname != null) &&
                (nickname.Length > 0) &&
                (_NicknameRegex.Match(nickname).Success)) {
                return true;
            }
            
            return false;
        }
        
        public static string Pass(string password)
        {
            return "PASS "+password;
        }
        
        public static string Nick(string nickname)
        {
            return "NICK "+nickname;
        }
        
        public static string User(string username, int usermode, string realname)
        {
            return "USER "+username+" "+usermode.ToString()+" * :"+realname;
        }

        public static string Oper(string name, string password)
        {
            return "OPER "+name+" "+password;
        }
        
        public static string Privmsg(string destination, string message)
        {
            return "PRIVMSG "+destination+" :"+message;
        }

        public static string Notice(string destination, string message)
        {
            return "NOTICE "+destination+" :"+message;
        }

        public static string Join(string channel)
        {
            return "JOIN "+channel;
        }
        
        public static string Join(string[] channels)
        {
            string channellist = String.Join(",", channels);
            return "JOIN "+channellist;
        }
        
        public static string Join(string channel, string key)
        {
            return "JOIN "+channel+" "+key;
        }

        public static string Join(string[] channels, string[] keys)
        {
            string channellist = String.Join(",", channels);
            string keylist = String.Join(",", keys);
            return "JOIN "+channellist+" "+keylist;
        }
        
        public static string Part(string channel)
        {
            return "PART "+channel;
        }

        public static string Part(string[] channels)
        {
            string channellist = String.Join(",", channels);
            return "PART "+channellist;
        }
        
        public static string Part(string channel, string partmessage)
        {
            return "PART "+channel+" :"+partmessage;
        }

        public static string Part(string[] channels, string partmessage)
        {
            string channellist = String.Join(",", channels);
            return "PART "+channellist+" :"+partmessage;
        }

        public static string Kick(string channel, string nickname)
        {
            return "KICK "+channel+" "+nickname;
        }

        public static string Kick(string channel, string nickname, string comment)
        {
            return "KICK "+channel+" "+nickname+" :"+comment;
        }
        
        public static string Kick(string[] channels, string nickname)
        {
            string channellist = String.Join(",", channels);
            return "KICK "+channellist+" "+nickname;
        }

        public static string Kick(string[] channels, string nickname, string comment)
        {
            string channellist = String.Join(",", channels);
            return "KICK "+channellist+" "+nickname+" :"+comment;
        }

        public static string Kick(string channel, string[] nicknames)
        {
            string nicknamelist = String.Join(",", nicknames);
            return "KICK "+channel+" "+nicknamelist;
        }

        public static string Kick(string channel, string[] nicknames, string comment)
        {
            string nicknamelist = String.Join(",", nicknames);
            return "KICK "+channel+" "+nicknamelist+" :"+comment;
        }

        public static string Kick(string[] channels, string[] nicknames)
        {
            string channellist = String.Join(",", channels);
            string nicknamelist = String.Join(",", nicknames);
            return "KICK "+channellist+" "+nicknamelist;
        }

        public static string Kick(string[] channels, string[] nicknames, string comment)
        {
            string channellist = String.Join(",", channels);
            string nicknamelist = String.Join(",", nicknames);
            return "KICK "+channellist+" "+nicknamelist+" :"+comment;
        }
        
        public static string Motd()
        {
            return "MOTD";
        }

        public static string Motd(string target)
        {
            return "MOTD "+target;
        }

        public static string Luser()
        {
            return "LUSER";
        }

        public static string Luser(string mask)
        {
            return "LUSER "+mask;
        }

        public static string Luser(string mask, string target)
        {
            return "LUSER "+mask+" "+target;
        }
        
        public static string Version()
        {
            return "VERSION";
        }

        public static string Version(string target)
        {
            return "VERSION "+target;
        }

        public static string Stats()
        {
            return "STATS";
        }

        public static string Stats(string query)
        {
            return "STATS "+query;
        }

        public static string Stats(string query, string target)
        {
            return "STATS "+query+" "+target;
        }

        public static string Links()
        {
            return "LINKS";
        }
        
        public static string Links(string servermask)
        {
            return "LINKS "+servermask;
        }
        
        public static string Links(string remoteserver, string servermask)
        {
            return "LINKS "+remoteserver+" "+servermask;
        }
        
        public static string Time()
        {
            return "TIME";
        }
        
        public static string Time(string target)
        {
            return "TIME "+target;
        }
        
        public static string Connect(string targetserver, string port)
        {
            return "CONNECT "+targetserver+" "+port;
        }
        
        public static string Connect(string targetserver, string port, string remoteserver)
        {
            return "CONNECT "+targetserver+" "+port+" "+remoteserver;
        }
        
        public static string Trace()
        {
            return "TRACE";
        }
        
        public static string Trace(string target)
        {
            return "TRACE "+target;
        }
        
        public static string Admin()
        {
            return "ADMIN";
        }
        
        public static string Admin(string target)
        {
            return "ADMIN "+target;
        }
        
        public static string Info()
        {
            return "INFO";
        }
        
        public static string Info(string target)
        {
            return "INFO "+target;
        }
        
        public static string Servlist()
        {
            return "SERVLIST";
        }
        
        public static string Servlist(string mask)
        {
            return "SERVLIST "+mask;
        }
        
        public static string Servlist(string mask, string type)
        {
            return "SERVLIST "+mask+" "+type;
        }
        
        public static string Squery(string servicename, string servicetext)
        {
            return "SQUERY "+servicename+" :"+servicetext;
        }
        
        public static string List()
        {
            return "LIST";
        }

        public static string List(string channel)
        {
            return "LIST "+channel;
        }

        public static string List(string[] channels)
        {
            string channellist = String.Join(",", channels);
            return "LIST "+channellist;
        }
        
        public static string List(string channel, string target)
        {
            return "LIST "+channel+" "+target;
        }

        public static string List(string[] channels, string target)
        {
            string channellist = String.Join(",", channels);
            return "LIST "+channellist+" "+target;
        }
        
        public static string Names()
        {
            return "NAMES";
        }

        public static string Names(string channel)
        {
            return "NAMES "+channel;
        }

        public static string Names(string[] channels)
        {
            string channellist = String.Join(",", channels);
            return "NAMES "+channellist;
        }
        
        public static string Names(string channel, string target)
        {
            return "NAMES "+channel+" "+target;
        }
        
        public static string Names(string[] channels, string target)
        {
            string channellist = String.Join(",", channels);
            return "NAMES "+channellist+" "+target;
        }
        
        public static string Topic(string channel)
        {
            return "TOPIC "+channel;
        }

        public static string Topic(string channel, string newtopic)
        {
            return "TOPIC "+channel+" :"+newtopic;
        }

        public static string Mode(string target)
        {
            return "MODE "+target;
        }

        public static string Mode(string target, string newmode)
        {
            return "MODE "+target+" "+newmode;
        }

        public static string Service(string nickname, string distribution, string info)
        {
            return "SERVICE "+nickname+" * "+distribution+" * * :"+info;
        }
        
        public static string Invite(string nickname, string channel)
        {
            return "INVITE "+nickname+" "+channel;
        }

        public static string Who()
        {
            return "WHO";
        }
        
        public static string Who(string mask)
        {
            return "WHO "+mask;
        }
        
        public static string Who(string mask, bool ircop)
        {
            if (ircop) {
                return "WHO "+mask+" o";
            } else {
                return "WHO "+mask;
            }
        }
        
        public static string Whois(string mask)
        {
            return "WHOIS "+mask;
        }
        
        public static string Whois(string[] masks)
        {
            string masklist = String.Join(",", masks);
            return "WHOIS "+masklist;
        }
        
        public static string Whois(string target, string mask)
        {
            return "WHOIS "+target+" "+mask;
        }
        
        public static string Whois(string target, string[] masks)
        {
            string masklist = String.Join(",", masks);
            return "WHOIS "+target+" "+masklist;
        }
        
        public static string Whowas(string nickname)
        {
            return "WHOWAS "+nickname;
        }
        
        public static string Whowas(string[] nicknames)
        {
            string nicknamelist = String.Join(",", nicknames);
            return "WHOWAS "+nicknamelist;
        }

        public static string Whowas(string nickname, string count)
        {
            return "WHOWAS "+nickname+" "+count+" ";
        }
        
        public static string Whowas(string[] nicknames, string count)
        {
            string nicknamelist = String.Join(",", nicknames);
            return "WHOWAS "+nicknamelist+" "+count+" ";
        }
        
        public static string Whowas(string nickname, string count, string target)
        {
            return "WHOWAS "+nickname+" "+count+" "+target;
        }
        
        public static string Whowas(string[] nicknames, string count, string target)
        {
            string nicknamelist = String.Join(",", nicknames);
            return "WHOWAS "+nicknamelist+" "+count+" "+target;
        }
        
        public static string Kill(string nickname, string comment)
        {
            return "KILL "+nickname+" :"+comment;
        }
        
        public static string Ping(string server)
        {
            return "PING "+server;
        }
        
        public static string Ping(string server, string server2)
        {
            return "PING "+server+" "+server2;
        }
        
        public static string Pong(string server)
        {
            return "PONG "+server;
        }
        
        public static string Pong(string server, string server2)
        {
            return "PONG "+server+" "+server2;
        }

        public static string Error(string errormessage)
        {
            return "ERROR :"+errormessage;
        }
        
        public static string Away()
        {
            return "AWAY";
        }
        
        public static string Away(string awaytext)
        {
            return "AWAY :"+awaytext;
        }
        
        public static string Rehash()
        {
            return "REHASH";
        }
        
        public static string Die()
        {
            return "DIE";
        }
        
        public static string Restart()
        {
            return "RESTART";
        }
        
        public static string Summon(string user)
        {
            return "SUMMON "+user;
        }
        
        public static string Summon(string user, string target)
        {
            return "SUMMON "+user+" "+target;
        }
        
        public static string Summon(string user, string target, string channel)
        {
            return "SUMMON "+user+" "+target+" "+channel;
        }
        
        public static string Users()
        {
            return "USERS";
        }
        
        public static string Users(string target)
        {
            return "USERS "+target;
        }
        
        public static string Wallops(string wallopstext)
        {
            return "WALLOPS :"+wallopstext;
        }
        
        public static string Userhost(string nickname)
        {
            return "USERHOST "+nickname;
        }
        
        public static string Userhost(string[] nicknames)
        {
            string nicknamelist = String.Join(" ", nicknames);
            return "USERHOST "+nicknamelist;
        }
        
        public static string Ison(string nickname)
        {
            return "ISON "+nickname;
        }
        
        public static string Ison(string[] nicknames)
        {
            string nicknamelist = String.Join(" ", nicknames);
            return "ISON "+nicknamelist;
        }
        
        public static string Quit()
        {
            return "QUIT";
        }
        
        public static string Quit(string quitmessage)
        {
            return "QUIT :"+quitmessage;
        }
        
        public static string Squit(string server, string comment)
        {
            return "SQUIT "+server+" :"+comment;
        }
    }
}
