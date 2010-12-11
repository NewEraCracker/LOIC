using System;
using System.Windows.Forms;

namespace LOIC
{
	public partial class frmWtf : Form
	{
		#region Constructors
		public frmWtf()
		{
			InitializeComponent();
		}
		#endregion

		#region Event handlers
		private void frmWtf_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void frmWtf_KeyDown(object sender, KeyEventArgs e)
		{
			Close();
		}
		#endregion
	}
}
