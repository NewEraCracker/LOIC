/*
 * $Id: Consts.cs 198 2005-06-08 16:50:11Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/Consts.cs $
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
    public enum Priority
    {
        Low,
        BelowMedium,
        Medium,
        AboveMedium,
        High,
        Critical
    }

    /// <summary>
    ///
    /// </summary>
    public enum SendType
    {
        Message,
        Action,
        Notice,
        CtcpReply,
        CtcpRequest
    }

    /// <summary>
    ///
    /// </summary>
    public enum ReceiveType
    {
        Info,
        Login,
        Motd,
        List,
        Join,
        Kick,
        Part,
        Invite,
        Quit,
        Who,
        WhoIs,
        WhoWas,
        Name,
        Topic,
        BanList,
        NickChange,
        TopicChange,
        UserMode,
        UserModeChange,
        ChannelMode,
        ChannelModeChange,
        ChannelMessage,
        ChannelAction,
        ChannelNotice,
        QueryMessage,
        QueryAction,
        QueryNotice,
        CtcpReply,
        CtcpRequest,
        Error,
        ErrorMessage,
        Unknown
    }

    /// <summary>
    ///
    /// </summary>
    public enum ReplyCode: int
    {
        Null =                           000,
        Welcome =                        001,
        YourHost =                       002,
        Created =                        003,
        MyInfo =                         004,
        Bounce =                         005,
        TraceLink =                      200,
        TraceConnecting =                201,
        TraceHandshake =                 202,
        TraceUnknown =                   203,
        TraceOperator =                  204,
        TraceUser =                      205,
        TraceServer =                    206,
        TraceService =                   207,
        TraceNewType =                   208,
        TraceClass =                     209,
        TraceReconnect =                 210,
        StatsLinkInfo =                  211,
        StatsCommands =                  212,
        EndOfStats =                     219,
        UserModeIs =                     221,
        ServiceList =                    234,
        ServiceListEnd =                 235,
        StatsUptime =                    242,
        StatsOLine =                     243,
        LuserClient =                    251,
        LuserOp =                        252,
        LuserUnknown =                   253,
        LuserChannels =                  254,
        LuserMe =                        255,
        AdminMe =                        256,
        AdminLocation1 =                 257,
        AdminLocation2 =                 258,
        AdminEmail =                     259,
        TraceLog =                       261,
        TraceEnd =                       262,
        TryAgain =                       263,
        Away =                           301,
        UserHost =                       302,
        IsOn =                           303,
        UnAway =                         305,
        NowAway =                        306,
        WhoIsUser =                      311,
        WhoIsServer =                    312,
        WhoIsOperator =                  313,
        WhoWasUser =                     314,
        EndOfWho =                       315,
        WhoIsIdle =                      317,
        EndOfWhoIs =                     318,
        WhoIsChannels =                  319,
        ListStart =                      321,
        List =                           322,
        ListEnd =                        323,
        ChannelModeIs =                  324,
        UniqueOpIs =                     325,
        NoTopic =                        331,
        Topic =                          332,
        Inviting =                       341,
        Summoning =                      342,
        InviteList =                     346,
        EndOfInviteList =                347,
        ExceptionList =                  348,
        EndOfExceptionList =             349,
        Version =                        351,
        WhoReply =                       352,
        NamesReply =                     353,
        Links =                          364,
        EndOfLinks =                     365,
        EndOfNames =                     366,
        BanList =                        367,
        EndOfBanList =                   368,
        EndOfWhoWas =                    369,
        Info =                           371,
        Motd =                           372,
        EndOfInfo =                      374,
        MotdStart =                      375,
        EndOfMotd =                      376,
        YouAreOper =                     381,
        Rehashing =                      382,
        YouAreService =                  383,
        Time =                           391,
        UsersStart =                     392,
        Users =                          393,
        EndOfUsers =                     394,
        NoUsers =                        395,
        ErrorNoSuchNickname =            401,
        ErrorNoSuchServer =              402,
        ErrorNoSuchChannel =             403,
        ErrorCannotSendToChannel =       404,
        ErrorTooManyChannels =           405,
        ErrorWasNoSuchNickname =         406,
        ErrorTooManyTargets =            407,
        ErrorNoSuchService =             408,
        ErrorNoOrigin =                  409,
        ErrorNoRecipient =               411,
        ErrorNoTextToSend =              412,
        ErrorNoTopLevel =                413,
        ErrorWildTopLevel =              414,
        ErrorBadMask =                   415,
        ErrorUnknownCommand =            421,
        ErrorNoMotd =                    422,
        ErrorNoAdminInfo =               423,
        ErrorFileError =                 424,
        ErrorNoNicknameGiven =           431,
        ErrorErroneusNickname =          432,
        ErrorNicknameInUse =             433,
        ErrorNicknameCollision =         436,
        ErrorUnavailableResource =       437,
        ErrorUserNotInChannel =          441,
        ErrorNotOnChannel =              442,
        ErrorUserOnChannel =             443,
        ErrorNoLogin =                   444,
        ErrorSummonDisabled =            445,
        ErrorUsersDisabled =             446,
        ErrorNotRegistered =             451,
        ErrorNeedMoreParams =            461,
        ErrorAlreadyRegistered =         462,
        ErrorNoPermissionForHost =       463,
        ErrorPasswordMismatch =          464,
        ErrorYouAreBannedCreep =         465,
        ErrorYouWillBeBanned =           466,
        ErrorKeySet =                    467,
        ErrorChannelIsFull =             471,
        ErrorUnknownMode =               472,
        ErrorInviteOnlyChannel =         473,
        ErrorBannedFromChannel =         474,
        ErrorBadChannelKey =             475,
        ErrorBadChannelMask =            476,
        ErrorNoChannelModes =            477,
        ErrorBanListFull =               478,
        ErrorNoPrivileges =              481,
        ErrorChannelOpPrivilegesNeeded = 482,
        ErrorCannotKillServer =          483,
        ErrorRestricted =                484,
        ErrorUniqueOpPrivilegesNeeded =  485,
        ErrorNoOperHost =                491,
        ErrorUserModeUnknownFlag =       501,
        ErrorUsersDoNotMatch =           502
    }
}
