using System;

namespace LOIC {

	/// <summary>
	/// Protocol.
	/// </summary>
	public enum Protocol
	{
		/// <summary>
		/// No (invalid) protocol.
		/// </summary>
		None	= 0,

		/// <summary>
		/// Transmission Control Protocol.
		/// </summary>
		TCP		= 1,

		/// <summary>
		/// User Datagram Protocol.
		/// </summary>
		UDP		= 2,

		/// <summary>
		/// HyperText Transfer Protocol.
		/// </summary>
		HTTP	= 3,
	}
}

