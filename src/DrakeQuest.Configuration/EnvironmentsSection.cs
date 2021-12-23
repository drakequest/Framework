namespace DrakeQuest.Configuration
{
	using System.ComponentModel;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Globalization;
	using System.IO;
	using System.Xml.Serialization;

	using DrakeQuest.Configuration.Design;

	/// <summary>
	/// Represents a configuration section containing the URLs and connection strings of several working environments.
	/// </summary>
	public sealed class EnvironmentsSection : ConfigurationSection
  {
    #region Attributes

    private EnvironmentElementCollection environments = new EnvironmentElementCollection();
    private ApplicationElementCollection applications = new ApplicationElementCollection();

    #endregion Attributes

    #region Properties

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    [ConfigurationProperty("connectionString", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired, DefaultValue = "NotSet"), StringValidator(MinLength = 1)]
    [Category("Configuration")]
    public string ConnectionString
    {
      get
      {
        return (string)base["connectionString"];
      }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          value = string.Empty;
        }
        base["connectionString"] = value;
      }
    }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    [ConfigurationProperty("current", DefaultValue = "DEV01"), StringValidator(MinLength = 3)]
    [Category("Configuration")]
    public string Current
    {
      get
      {
        return (string)base["current"];
      }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          value = string.Empty;
        }
        base["current"] = value;
      }
    }

    /// <summary>
    /// Gets or sets the environments.
    /// </summary>
    /// <value>The environments.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Environments")]
    [TypeConverter(typeof(EnvironmentElementCollectionConverter))]
    public EnvironmentElementCollection Environments
    {
      get
      {
        if (environments == null || environments.Count == 0)
        {
          var envs = LoadEnvironmentsConfiguration();
          if (envs != null)
          {
            environments = envs;
          }
        }

        return environments;
      }
    }

    /// <summary>
    /// Gets or sets the environments.
    /// </summary>
    /// <value>The environments.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Applications")]
    [TypeConverter(typeof(ApplicationElementCollectionConverter))]
    public ApplicationElementCollection Applications
    {
      get
      {
        if (applications == null || applications.Count == 0)
        {
          var apps = LoadApplicationsConfiguration();
          if (apps != null)
          {
            applications = apps;
          }
        }

        return applications;
      }
    }

    #endregion Properties

    #region Shadow unneeded properties.

    /// <summary>
    /// Gets or sets a value indicating whether the element is locked.
    /// </summary>
    /// <value></value>
    /// <returns>true if the element is locked; otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
    /// The element has already been locked at a higher configuration level.
    /// </exception>
    [Browsable(false)]
    public new bool LockItem { get { return base.LockItem; } set { base.LockItem = value; } }

    /// <summary>
    /// Gets an <see cref="T:System.Configuration.ElementInformation"/> object that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationElement"/> object.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// An <see cref="T:System.Configuration.ElementInformation"/> that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationElement"/>.
    /// </returns>
    [Browsable(false)]
    public new ElementInformation ElementInformation { get { return base.ElementInformation; } }

    /// <summary>
    /// Gets a <see cref="T:System.Configuration.SectionInformation"/> object that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationSection"/> object.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// A <see cref="T:System.Configuration.SectionInformation"/> that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationSection"/>.
    /// </returns>
    [Browsable(false)]
    public new SectionInformation SectionInformation { get { return base.SectionInformation; } }

    /// <summary>
    /// Gets the collection of locked attributes.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The <see cref="T:System.Configuration.ConfigurationLockCollection"/> of locked attributes (properties) for the element.
    /// </returns>
    [Browsable(false)]
    public new ConfigurationLockCollection LockAllAttributesExcept { get { return base.LockAllAttributesExcept; } }

    /// <summary>
    /// Gets the collection of locked elements.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The <see cref="T:System.Configuration.ConfigurationLockCollection"/> of locked elements.
    /// </returns>
    [Browsable(false)]
    public new ConfigurationLockCollection LockAllElementsExcept { get { return base.LockAllElementsExcept; } }

    /// <summary>
    /// Gets the collection of locked attributes
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The <see cref="T:System.Configuration.ConfigurationLockCollection"/> of locked attributes (properties) for the element.
    /// </returns>
    [Browsable(false)]
    public new ConfigurationLockCollection LockAttributes { get { return base.LockAttributes; } }

    /// <summary>
    /// Gets the collection of locked elements.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The <see cref="T:System.Configuration.ConfigurationLockCollection"/> of locked elements.
    /// </returns>
    [Browsable(false)]
    public new ConfigurationLockCollection LockElements { get { return base.LockElements; } }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>A shallow copy of this instance</returns>
    public EnvironmentsSection Copy()
    {
      var copy = new EnvironmentsSection();
      copy.ConnectionString = this.ConnectionString;
      copy.environments = this.environments;
      copy.applications = this.applications;

      return copy;
    }

    /// <summary>
    /// Saves the configuration to the target database.
    /// </summary>
    public void Save()
    {
      this.SaveConfigurationSection();
      LoadEnvironmentsConfiguration();
      LoadApplicationsConfiguration();
    }


    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static EnvironmentsSection GetConfiguration(string connectionString)
    {
      return new EnvironmentsSection { ConnectionString = connectionString };
    }

    /// <summary>
    /// Loads the configuration.
    /// </summary>
    /// <returns></returns>
    private EnvironmentElementCollection LoadEnvironmentsConfiguration()
    {
      return this.LoadApplicationsConfiguration<EnvironmentElementCollection>("Environments");
    }

    /// <summary>
    /// Loads the configuration.
    /// </summary>
    /// <returns></returns>
    private ApplicationElementCollection LoadApplicationsConfiguration()
    {
      return this.LoadApplicationsConfiguration<ApplicationElementCollection>("Applications");
    }

    #endregion Methods
  }
}
