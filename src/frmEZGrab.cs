using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOIC
{
	public partial class frmEZGrab : Form
	{
		public string hivemind, overlord;

		public frmEZGrab(string HiveMind, string OverLord)
		{
			InitializeComponent();
			this.hivemind = HiveMind;
			txtHivemind.Text = HiveMind;
			this.overlord = OverLord;
			txtOverlord.Text = OverLord;
		}
		private void frmEZGrab_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.Dispose();
		}
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (txtDate.Text != "")
			{
				txtOverlord.Text = overlord + "&@time=" + txtDate.Text + "@";
			}
		}
		private void btnShorten_Click(object sender, EventArgs e)
		{
			string turl = "";
			if (rbbitly.Checked)
			{
				turl = "http://bit.ly/?u=";
			}
			else if (rbisgd.Checked)
			{
				turl = "http://is.gd/create.php?longurl=";
			}
			turl += Uri.EscapeDataString(txtOverlord.Text);
			System.Diagnostics.Process.Start(turl);
		}
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}