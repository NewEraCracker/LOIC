using System;
using System.Text;

namespace LOIC
{
    public static class Functions
    {
        private static readonly Random random = new Random();

        public static string RandomString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                int charIndex = Convert.ToInt32(Math.Floor(26*random.NextDouble())) + 65;
                char ch = Convert.ToChar(charIndex);
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}