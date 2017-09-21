/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;

namespace LOIC
{
	public enum ReqState
	{
		Ready,
		Connecting,
		Requesting,
		Downloading,
		Completed,
		Failed
	};
}