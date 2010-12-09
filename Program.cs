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
			IrcData ircData = null;

			int count = 0;
			foreach (string s in cmdLine)
			{
				/* IRC */
				if (s.ToLower() == "/hivemind")
				{
				    hive = true;
                    ircData = ReadIrcData(cmdLine, count);
				}
			    if (s.ToLower() == "/hidden")
				{
				    hide = true;
				}
				count++;
			}

            // Lets try this! 
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(hive, hide, ircData));
		}

        private static IrcData ReadIrcData(string[] cmdLine, int baseIndex)
	    {
            IrcData result = new IrcData();
            result.Server = cmdLine[baseIndex + 1]; //if no server entered let it crash

	        try
	        {
                result.Port = cmdLine[baseIndex + 2];
	        }
	        catch (Exception)
	        {
	            //default
                result.Port = "6667";
	        }
 
	        try
	        {
                result.Channel = cmdLine[baseIndex + 3];
	        }
	        catch (Exception)
	        {
	            //default
                result.Channel = "#loic";
	        }

            return result;
	    }
	}
}