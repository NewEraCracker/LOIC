namespace LOIC
{
	#region using directives
	using System;
	using LOIC.Flooders;
	#endregion
	
	public class FloodManager
	{
		#region Fields
		private HiveManager hiveManager;
		
		private XXPFlooder[] xxpFlooders;
		private HTTPFlooder[] httpFlooders;
		#endregion
		
		#region Constructor
		public FloodManager ()
		{
		}
		
		/// <summary>
		/// Instance destructor, for the ones not familiar with the syntax.
		/// </summary>
		~FloodManager ()
		{
			if (HiveEnabled && hiveManager.State == HiveState.Connected)
			{
				hiveManager.Disconnect ();
			}
		}
		#endregion
		
		#region Properties
		private bool HiveEnabled {
			get { return hiveManager != null; }
		}
		#endregion
		
		#region Methods
		public void Initialize ()
		{
			
		}
		
		public void InitializeHive (string hostname, int port, string channel)
		{
			hiveManager = new HiveManager (hostname, port, channel);
			
		}
		
		public void Start ()
		{
			
		}
		
		public void Stop ()
		{
			
		}
		#endregion
	}
}

