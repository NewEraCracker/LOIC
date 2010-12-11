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
