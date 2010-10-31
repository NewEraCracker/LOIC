/*
 * $Id: AssemblyInfo.cs 214 2007-04-06 14:12:17Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.4.0/src/AssemblyInfo.cs $
 * $Rev: 214 $
 * $Author: meebey $
 * $Date: 2007-04-06 16:12:17 +0200 (Fri, 06 Apr 2007) $
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Information about this assembly is defined by the following
// attributes.
//
// change them to the information which is associated with the assembly
// you compile.

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

[assembly: AssemblyTitle("SmartIrc4net")]
[assembly: AssemblyDescription("IRC library for the .NET Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("qNETp")]
[assembly: AssemblyProduct("SmartIrc4net")]
[assembly: AssemblyCopyright("2003-2007 (C) Mirco Bauer <meebey@meebey.net>")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all values by your own or you can build default build and revision
// numbers with the '*' character (the default):

[assembly: AssemblyVersion("0.4.0.*")]

// The following attributes specify the key for the sign of your assembly. See the
// .NET Framework documentation for more information about signing.
// This is not required, if you don't want signing let these attributes like they're.
#if DELAY_SIGN
[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile("../SmartIrc4net-pub.snk")]
#else
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
#endif
