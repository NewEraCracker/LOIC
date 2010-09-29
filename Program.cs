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
            string server = "";
            int count = 0;
            foreach (string s in cmdLine)
            {
                if (s.ToLower() == "/hivemind")
                {
                    hive = true;
                    server = cmdLine[count + 1];
                }
                if (s.ToLower() == "/hidden")
                {
                    hide = true;
                }
                count++;
            }
			//Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, server));
		}
	}
}
