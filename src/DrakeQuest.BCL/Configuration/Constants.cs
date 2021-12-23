namespace DrakeQuest.Configuration
{
	/// <summary>
	/// Constants class for the configuration
	/// </summary>
    public static class Constants
	{
		/// <summary>
		/// Give the path for the folder configuration
		/// </summary>
		public const string ConfigFolderKey = "folder:configs";

		/// <summary>
		/// Default config folder use in case of not defined
		/// </summary>
		public const string DefaultConfigFolderName = "Config";

		/// <summary>
		/// Folder use for the data
		/// </summary>
		public const string DataFolderKey = "folder:datas";

		/// <summary>
		/// Folder to save the logs
		/// </summary>
		public const string LogFolderKey = "folder:Logs";


        #region Data Config Key

		/// <summary>
		/// In the case of using bulk, key config for the batch size
		/// </summary>
        public const string ConfigBulkCopyBatchSize = "bulk:BatchSize";

		/// <summary>
		/// In the case of using bulk, key config for the timeout
		/// </summary>
		public const string ConfigBulkCopyTimeout = "bulk:Timeout";

        #endregion

        #region autofac defaults Config

		/// <summary>
		/// Autofac Key for the autofact
		/// </summary>
        public const string ModuleFolderKey = "folder:Modules";

		/// <summary>
		/// autofac default config file name
		/// </summary>
        public const string AutofacConfigExtension = ".autofac.config";

		/// <summary>
		/// autofac default config section name
		/// </summary>
		public const string AutofacSettingSectionName = "autofac";

        #endregion

    }
}
