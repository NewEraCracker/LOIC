using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOIC
{
	public partial class frmPartyVan : Form
	{
		public frmPartyVan()
		{
			InitializeComponent();

			txtEULA.Clear();
			txtEULA.ReadOnly = true;
			txtEULA.Rtf = global::LOIC.Properties.Resources.EULA;
		}
	}
}