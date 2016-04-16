using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace LOIC
{
	public class Settings
	{
		public static bool HasAcceptedEula()
		{
			return (false == String.IsNullOrEmpty(ReadSetting("EULA")));
		}

		public static bool SaveAcceptedEula()
		{
			return UpdateSetting("EULA", "1");
		}

		public static string ReadSetting(string key, bool emptyUndefined = true)
		{
			try
			{
				var appSettings = ConfigurationManager.AppSettings;
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
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;
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