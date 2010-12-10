using System;
using System.Text;

namespace LOIC
{
    public static class Functions
    {
        private static readonly Random random = new Random();

        public static string RandomString ()
        {
            StringBuilder builder = new StringBuilder ();
                        
            for (int i = 0; i < 6; i++)
            {
                int charIndex = (int)Math.Floor(26*random.NextDouble()) + 65;
                char ch = (char)(charIndex);
                builder.Append (ch);
            }
			
            return builder.ToString ();
        }
    }
}