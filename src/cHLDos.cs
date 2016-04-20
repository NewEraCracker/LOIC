/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace LOIC
{
	/// <summary>
	/// Abstract class cHLDos contributed by B²
	/// </summary>
	public abstract class cHLDos : IFlooder
	{
		public ReqState State = ReqState.Ready;

		/// <summary>
		/// Shows if all possible sockets are build.
		/// TRUE as long as the maximum amount of sockets is NOT reached.
		/// </summary>
		public bool IsDelayed { get; set; }

		/// <summary>
		/// Set or get the current working state.
		/// </summary>
		public bool IsFlooding { get; set; }

		/// <summary>
		/// Amount of send requests.
		/// </summary>
		public int Requested { get; set; }

		/// <summary>
		/// Amount of received responses / packets.
		/// </summary>
		public int Downloaded { get; set; }

		/// <summary>
		/// Amount of failed packets / requests.
		/// </summary>
		public int Failed { get; set; }

		/// <summary>
		/// The time in milliseconds between the creation of new sockets
		/// </summary>
		public int Delay { get; set; }

		/// <summary>
		/// The timeout in seconds between requests for the same connection.
		/// </summary>
		public int Timeout { get; set; }

		public virtual void Start()
		{ }

		public virtual void Stop()
		{
			IsFlooding = false;
			IsDelayed = true;
		}

		/// <summary>
		/// override this if you want to test the settings before spreading the word to the hivemind!
		/// should make a single connection and check for the expected outcome!
		/// </summary>
		public virtual bool Test()
		{
			return true;
		}
	}
}