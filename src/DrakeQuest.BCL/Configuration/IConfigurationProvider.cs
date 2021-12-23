namespace DrakeQuest.Configuration
{
	/// <summary>
	/// Get Configuration settings 
	/// </summary>
    public interface IConfigurationProvider
	{
		/// <summary>
		/// Get the value name for a specific configuration
		/// </summary>
		/// <param name="key">Key name of the application settings</param>
		/// <returns>value</returns>
		string GetAppSettings(string key);
	}
}
