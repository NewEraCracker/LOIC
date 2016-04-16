using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOIC
{
    public partial class FrmPartyVan : Form
    {
        public FrmPartyVan()
        {
            InitializeComponent();

            txtEULA.Clear();
            txtEULA.ReadOnly = true;
            txtEULA.Rtf = global::LOIC.Properties.Resources.EULA;
        }
    }
}
