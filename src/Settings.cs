/* LOIC - Low Orbit Ion Cannon
 * Released to the public domain
 * Enjoy getting v&, kids.
 */

using System;
using System.Collections.Specialized;
using System.Configuration;

namespace LOIC
{
	public class Settings
	{
		private static readonly string EulaSetting = "AcceptEULA";
		private static readonly string KonamiSetting = "KonaniCode";

		public static bool HasAcceptedEula()
		{
			return (false == String.IsNullOrEmpty(ReadSetting(EulaSetting)));
		}

		public static bool SaveAcceptedEula()
		{
			return UpdateSetting(EulaSetting, "1");
		}

		public static bool HasKonamiCode()
		{
			return (false == String.IsNullOrEmpty(ReadSetting(KonamiSetting)));
		}

		public static bool SaveKonamiCode()
		{
			return UpdateSetting(KonamiSetting, "1");
		}

		public static string ReadSetting(string key, bool emptyUndefined = true)
		{
			try
			{
				NameValueCollection appSettings = ConfigurationManager.AppSettings;
				string result = appSettings[key] ?? (emptyUndefined ? "" : "Not Found");
				return result;
			}
			catch (ConfigurationErrorsException)
			{
				return null;
			}
		}

		public static bool UpdateSetting(string key, string value)
		{
			try
			{
				Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;
				if (settings[key] == null)
				{
					settings.Add(key, value);
				}
				else
				{
					settings[key].Value = value;
				}
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

				return true;
			}
			catch (ConfigurationErrorsException)
			{
				return false;
			}
		}
	}
}