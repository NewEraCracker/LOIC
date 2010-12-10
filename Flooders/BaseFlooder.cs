namespace LOIC.Flooders
{
	#region using directives
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Net;
	using System.Threading;
	#endregion

	public abstract class BaseFlooder
	{
		#region Fields
		private BackgroundWorker worker = new BackgroundWorker ();
		#endregion

		#region Constructor
		public BaseFlooder (string ip, int port, int delay, TargetProtocol protocol)
		{
			// Protocol validation
			if (!SupportedProtocols.Contains (Protocol)) {
				throw new NotSupportedException ("The specified protocol is not supported by this flooder");
			}
			
			// Initialization
			Delay = delay;
			IsFlooding = false;
			Protocol = protocol;
			RemoteHost = new IPEndPoint (IPAddress.Parse (ip), port);
			
			worker.DoWork += new DoWorkEventHandler (Run);
		}
		#endregion

		#region Properties
		public bool IsFlooding { get; protected set; }

		protected int Delay { get; private set; }

		protected IPEndPoint RemoteHost { get; private set; }

		protected TargetProtocol Protocol { get; private set; }

		protected abstract IList<TargetProtocol> SupportedProtocols { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Starts the flooding activity.
		/// </summary>
		/// <remarks>
		/// It will invoke OnStart()
		/// </remarks>
		public void Start ()
		{
			IsFlooding = true;
			OnStart ();
			worker.RunWorkerAsync ();
		}

		/// <summary>
		/// Stops the flooding activity.
		/// </summary>
		/// <remarks>
		/// It will call the OnStop()
		/// </remarks>
		public void Stop ()
		{
			IsFlooding = false;
		}

		protected abstract void OnStart ();

		protected abstract void OnRun ();

		private void Run (object sender, DoWorkEventArgs e)
		{
			int effectiveDelay = Math.Max (1, Delay);
			
			while (IsFlooding) {
				OnRun ();
				Thread.Sleep (effectiveDelay);
			}
		}
		#endregion
	}
}

