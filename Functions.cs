using System;
using System.Text;

namespace LOIC
{
    public static class Functions
    {
        public static string RandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}