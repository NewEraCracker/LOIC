namespace LOIC
{
	#region using directives
	using System;
	using System.Text;
	using System.Windows.Forms;
	#endregion

	static class Program
	{
		[STAThread]
		static void Main (string[] cmdLine)
		{
			bool hive = false;
			bool hide = false;
			
			// IRC
			string ircServerUri = string.Empty;
			string ircPort = string.Empty;
			string ircChannel = string.Empty;
			
			// Lets try this!
			int count = 0;
			foreach (string s in cmdLine) {
				if (!string.IsNullOrEmpty (s) && s[0] == '/') {
					string s2 = s.ToLower ();
					if (s2 == "/hivemind") {
						hive = true;
						ircServerUri = cmdLine[count + 1];
						//if no server entered let it crash
						try {
							ircPort = cmdLine[count + 2];
						} catch (Exception) {
							ircPort = "6667";
						}
						//default
						try {
							ircChannel = cmdLine[count + 3];
						} catch (Exception) {
							ircChannel = "#loic";
						}
						//default
					} else if (s2 == "/hidden") {
						hide = true;
					}
				}
				
				count++;
			}
			
			Application.SetCompatibleTextRenderingDefault (false);
			Application.Run (new frmMain (hive, hide, ircServerUri, ircPort, ircChannel));
		}
	}

	public static class Functions
	{
		public static string RandomString ()
		{
			StringBuilder builder = new StringBuilder ();
			Random random = new Random ();
			char ch;
			for (int i = 0; i < 6; i++) {
				ch = (char)((int)(Math.Floor (26 * random.NextDouble () + 65)));
				builder.Append (ch);
			}
			
			return builder.ToString ();
		}
	}
}
