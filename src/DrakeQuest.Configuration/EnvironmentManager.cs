namespace DrakeQuest.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Provides methods to access the environment configuration.
    /// </summary>
    public static class EnvironmentManager
    {
        #region Attributes

        private static EnvironmentElementCollection environments;
        private static ApplicationElementCollection applications;
        private static EnvironmentsSection section;
        private static string currentEnvironment;

        #endregion Attributes

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="EnvironmentManager"/> class.
        /// </summary>
        static EnvironmentManager()
        {
            var configurationSection = (EnvironmentsSection)ConfigurationManager.GetSection("environments");
            if (configurationSection == null)
            {
                environments = new EnvironmentElementCollection();
            }
            else
            {
                section = configurationSection;
                environments = configurationSection.Environments;
                applications = configurationSection.Applications;
                currentEnvironment = configurationSection.Current;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the environments.
        /// </summary>
        /// <value>The environments.</value>
        public static EnvironmentElementCollection Environments
        {
            get { return environments; }
        }

        /// <summary>
        /// Gets the applications.
        /// </summary>
        /// <value>The applications.</value>
        public static ApplicationElementCollection Applications
        {
            get { return applications; }
        }

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        /// <value>The application settings.</value>
        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        /// <value>The connection strings.</value>
        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }

        /// <summary>
        /// Gets the master environments.
        /// </summary>
        /// <value>The master environments.</value>
        public static IEnumerable<EnvironmentElement> MasterEnvironments
        {
            get { return environments.Where(e => e.IsMasterServer); }
        }

        /// <summary>
        /// Gets the read only environments.
        /// </summary>
        /// <value>The read only environments.</value>
        public static IEnumerable<EnvironmentElement> ReadOnlyEnvironments
        {
            get { return environments.Where(e => !e.IsMasterServer); }
        }

        /// <summary>
        /// Gets the section that was used to load the configuration.
        /// </summary>
        /// <value>The section.</value>
        public static EnvironmentsSection Section
        {
            get { return section; }
        }

        /// <summary>
        /// Gets the current environment for the application.
        /// </summary>
        /// <value>The section.</value>
        public static EnvironmentElement CurrentEnvironment
        {
            get { return Environments.Where(e => e.Name == currentEnvironment).SingleOrDefault(); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Read the environment application configuration and write it to the application.config file.
        /// </summary>
        public static void ApplyEnvironmentConfiguration()
        {
            var applicationName = string.Empty;
            var isWebApplication = false;
            if (Assembly.GetEntryAssembly() == null)
            {
                //The GetCallingAssembly does not report the correct assembly name if this is used within a private method
                //as it will return the name of this assembly instead
                var fullName = Assembly.GetCallingAssembly().FullName.Split(new string[] { "," }, StringSplitOptions.None);
                if (fullName.Length != 4)
                    throw new InvalidOperationException(string.Concat("Unable to parse the calling assembly name: ", Assembly.GetCallingAssembly().FullName));

                isWebApplication = true;
                applicationName = fullName[0];
            }
            else
                applicationName = Assembly.GetEntryAssembly().GetName().Name;

            if (string.IsNullOrEmpty(applicationName))
                throw new InvalidOperationException("Unable to determine the name of the application.");

            //Set the current environment from the config (if not a web app)
            if(!isWebApplication)
                SetCurrentEnvironment();

            if (!EnvironmentManager.Environments.Select(e => e.Name).Contains(currentEnvironment, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("The configuration section for environments contains an invalid value for the current attribute");

            // Build list of replacements
            var applicationSettings = BuildNewConfigSettings(applicationName, isWebApplication);

            //IF we have something to replace, go an apply it
            if (applicationSettings.Count > 0)
            {
                ApplyConfiguration(applicationSettings, isWebApplication);
            }
        }

        /// <summary>
        /// Method to return an environment setting value
        /// </summary>
        /// <param name="settingName">The setting name which to get the value for</param>
        /// <returns>The value corresponding to the settig name</returns>
        public static string GetEnvironmentSetting(string settingName)
        {
            var environmentSetting = CurrentEnvironment.Settings.Where(item => item.Key.Equals(settingName,StringComparison.OrdinalIgnoreCase))
                                                          .Select(item => item.Value)
                                                          .SingleOrDefault();
            if(string.IsNullOrEmpty(environmentSetting))
                throw new InvalidOperationException(string.Concat("The environment settings does not contain an entry for: ", settingName));
            else
                return environmentSetting; 
        }

        /// <summary>
        /// Method to return a named connection string.
        /// </summary>
        /// <param name="connectionName">The name of the connection</param>
        /// <returns>The connection string</returns>
        public static string GetConnectionString(string connectionName)
        {
            var connectionString = 
                EnvironmentManager.CurrentEnvironment.ConnectionStrings.Where(item => item.Name.Equals(connectionName, StringComparison.OrdinalIgnoreCase))
                                                                       .Select(item => item.ConnectionString)
                                                                       .SingleOrDefault();
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(string.Concat("The connection string could not be found for: ", connectionName));
            else
                return connectionString;
        }

        #endregion Methods

        #region Private Methods

        /// <summary>
        /// Method for setting the current environment name
        /// </summary>
        private static void SetCurrentEnvironment()
        {
            var filePath = string.Concat(Process.GetCurrentProcess().MainModule.FileName, ".config");

            // Load XML configuration file
            var document = new XmlDocument();
            document.Load(filePath);

            // Get target environment
            var environmentsNode = document.SelectSingleNode("//environments/@current");
            if (environmentsNode == null)
                throw new InvalidOperationException("The configuration section for environments was not found");
            else
                currentEnvironment = environmentsNode.Value.ToUpperInvariant();
        }

		private static Dictionary<string, string> BuildNewConfigSettings(this ApplicationElement applicationElement, Dictionary<string, string> applicationSettings,bool isWebApplication)
		{
			if (applicationElement != null && applicationElement.Settings != null)
			{
				foreach (var setting in applicationElement.Settings)
				{
					if (isWebApplication)
						applicationSettings[setting.Name] = setting.Value;
					else if (string.IsNullOrEmpty(setting.Path))
						applicationSettings[string.Format("/configuration/appSettings/add[@key='{0}']/@value", setting.Name)] = setting.Value;
					else
						applicationSettings[setting.Path] = setting.Value;
				}
			}
			return applicationSettings;
		}

		/// <summary>
		/// Method to build a dictionary containing the new application configuration
		/// </summary>
		/// <param name="applicationName">The name of the application to build the configuration for</param>
		/// <param name="isWebApplication">Boolean flag to indicate is config is for a web application</param>
		/// <returns>Dictionary containing the new configuration</returns>
		private static Dictionary<string, string> BuildNewConfigSettings(string applicationName, bool isWebApplication)
        {
            var applicationSettings = new Dictionary<string, string>();
			//The order here is important as we want the environment specific settings to override the application specific
			//configuration.

			//Get the application specific configuration
			applicationSettings = Applications[applicationName].BuildNewConfigSettings(applicationSettings, isWebApplication);
			applicationSettings =  Environments[currentEnvironment].Applications[applicationName].BuildNewConfigSettings(applicationSettings, isWebApplication);
            return applicationSettings;
        }

        /// <summary>
        /// Method to apply the configuration to the application
        /// </summary>
        /// <param name="updatedConfiguration">The dictionary containing the new configuration values</param>
        /// <param name="isWebApplication">Boolean flag to indicate whether or not this is a web application</param>
        private static void ApplyConfiguration(Dictionary<string, string> updatedConfiguration, bool isWebApplication)
        {
            if (isWebApplication)
            {
                // Apply replacements. For web applications, we are only concerned with app.settings
                foreach (var setting in updatedConfiguration)
                {
                    if (ConfigurationManager.AppSettings[setting.Key] != null)
                        ConfigurationManager.AppSettings[setting.Key] = setting.Value;
                    else
                        throw new InvalidOperationException("The setting was not found: " + setting.Key);
                }
            }
            else
            {
                var filePath = string.Concat(Process.GetCurrentProcess().MainModule.FileName, ".config");

                // Load XML configuration file
                var document = new XmlDocument();
                document.Load(filePath);

                // Apply replacements
                foreach (var setting in updatedConfiguration)
                {
                    var node = document.SelectSingleNode(setting.Key);
                    if (node != null)
                        node.Value = setting.Value;
                    else
                        throw new InvalidOperationException("The setting was not found: " + setting.Key);
                }

                // Save XML file
                document.Save(filePath);

                // Reload default configuration sections
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");

                // Reload custom configuration sections
                var sections = document.SelectNodes("/configuration/configSections/section/@name");
                foreach (XmlNode section in sections)
                {
                    ConfigurationManager.RefreshSection(section.Value);
                }

                var properties = Assembly.GetEntryAssembly().GetTypes()
                                                            .Where(t => t.BaseType == typeof(ApplicationSettingsBase))
                                                            .Select(t => t.GetProperty("Default", BindingFlags.Public |
                                                                                                  BindingFlags.Static |
                                                                                                  BindingFlags.FlattenHierarchy))
                                                            .Where(p => p != null)
                                                            .ToList();

                foreach (var property in properties)
                {
                    var defaultProperty = property.GetValue(null, null) as ApplicationSettingsBase;
                    if (defaultProperty != null)
                    {
                        defaultProperty.Reload();
                    }
                }
            }
        }

        #endregion
    }
}