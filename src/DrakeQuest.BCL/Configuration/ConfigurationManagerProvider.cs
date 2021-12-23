using System.Configuration;

namespace DrakeQuest.Configuration
{

    /// <summary>
    /// Default Configuration proviver using ConfigurationManager as sources
    /// </summary>
    internal class ConfigurationManagerProvider : IConfigurationProvider
	{
		/// <summary>
		/// Wrapper call of ConfigurationManager.AppSettings
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetAppSettings(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}
	}
}
