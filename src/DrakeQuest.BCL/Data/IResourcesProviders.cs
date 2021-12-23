namespace DrakeQuest.Configuration
{
    /// <summary>
    /// Resource providers for getting resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResourcesProviders<T>
	{
        /// <summary>
        /// get an object value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		object GetObject(string key);

        /// <summary>
        /// Get an resources as string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		string GetString(string key);
	}
}
