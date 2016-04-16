using System;
using System.Collections.Generic;
using System.Text;

namespace LOIC
{
	public class Functions
	{
		[ThreadStatic]
		private static readonly Random rnd = new Random(Guid.NewGuid().GetHashCode());
		private static readonly String[] ntv = { "6.0", "6.1", "6.2", "6.3", "10.0" };

		public static string RandomString()
		{
			StringBuilder builder = new StringBuilder();

			char ch;
			for (int i = 0; i < 6; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
				builder.Append(ch);
			}
			return builder.ToString();
		}

		public static string RandomUserAgent()
		{
			if (rnd.NextDouble() >= 0.5)
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