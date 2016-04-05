namespace LOIC
{
	partial class frmWtf
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// frmWtf
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::LOIC.Properties.Resources.WTF;
			this.ClientSize = new System.Drawing.Size(416, 300);
			this.ControlBox = false;
			this.Name = "frmWtf";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Click += new System.EventHandler(this.frmWtf_Click);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmWtf_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}