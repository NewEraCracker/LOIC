/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Windows.Forms;

namespace LOIC
{
	public partial class frmWtf : Form
	{
		public frmWtf()
		{
			InitializeComponent();
		}
		private void frmWtf_Click(object sender, EventArgs e)
		{
			Close();
		}
		private void frmWtf_KeyDown(object sender, KeyEventArgs e)
		{
			Close();
		}
		private void frmWtf_FormClosed(object sender, FormClosedEventArgs e)
		{
			Dispose();
		}
	}
}