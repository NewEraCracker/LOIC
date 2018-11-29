/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LOIC
{
	public class Konami
	{
		private static short KonamiState = 0;
		private static Keys[] KonamiKeys = {Keys.Up, Keys.Up, Keys.Down, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right, Keys.B, Keys.A, Keys.Enter};

		public static bool Check(Form frm)
		{
			bool on = Settings.HasKonamiCode();
			if(!on) {
				frm.KeyUp += new KeyEventHandler(HandleKonamiKeyPress);
			}
			return on;
		}


		public static void HandleKonamiKeyPress(object sender, KeyEventArgs e)
		{
			if (KonamiKeys[KonamiState] == e.KeyCode) {
				KonamiState++;
				if (KonamiState >= KonamiKeys.Length) {
					KonamiState = 0;
					e.Handled = true;
					Settings.SaveKonamiCode();
					MessageBox.Show("Close and re-open the application to apply the changes.", "Konami Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			} else if (KonamiState != 0) {
				KonamiState = 0;
			}
		}
	}
}