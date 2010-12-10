namespace LOIC.Flooders
{
	#region using directives
	using System;
	#endregion
	
	/// <summary>
	/// Target protocols supportable by each flooder class
	/// </summary>
	public enum TargetProtocol
	{
		Undefined = 0x0,
		TCP = 0x1,
		UDP = 0x2,
		HTTP = 0x3
	}
}

