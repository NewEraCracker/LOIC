using System;
using System.Windows.Forms;

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
					try
					{
					    ircport = cmdLine[count + 2];
					}
					catch (Exception)
					{
                        //default
					    ircport = "6667";
					} 
					try
					{
					    ircchannel = cmdLine[count + 3];
					}
					catch (Exception)
					{
                        //default
					    ircchannel = "#loic";
					} 
				}
				if (s.ToLower() == "/hidden")
				{
				    hide = true;
				}
				count++;
			}

            // Lets try this! 
            Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, ircserver, ircport, ircchannel));
		}
	}
}