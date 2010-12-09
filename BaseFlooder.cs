namespace LOIC
{
	#region using directives
	using System;
	using System.ComponentModel;
	#endregion

	public abstract class BaseFlooder
	{
		#region Fields
		private BackgroundWorker worker = new BackgroundWorker ();
		#endregion

		#region Constructor
		public BaseFlooder ()
		{
			IsFlooding = false;
			worker.DoWork += new DoWorkEventHandler (Run);
		}
		#endregion

		#region Properties
		public bool IsFlooding { get; protected set; }
		#endregion

		#region Methods
		public void Start ()
		{
			IsFlooding = true;
			OnStart ();
			worker.RunWorkerAsync ();
		}

		public void Stop ()
		{
			IsFlooding = false;
		}

		protected abstract void OnStart ();

		protected abstract void OnRun ();

		private void Run (object sender, DoWorkEventArgs e)
		{
			while (IsFlooding) {
				OnRun ();
			}
		}
		#endregion
	}
}

