using System.ComponentModel;
using System.Linq;

using DrakeQuest.Configuration.Design;

namespace DrakeQuest.Configuration
{
    /// <summary>
    /// Represents an environment element
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class EnvironmentElement
    {
        #region Attributes

        private ConnectionStringElementCollection connectionStrings;
        private UrlElementCollection urls;
        private SettingElementCollection settings;
        private ApplicationElementCollection applications;

        #endregion Attributes

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Category("Properties")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        /// <value>The connection strings.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Databases")]
        [TypeConverter(typeof(ConnectionStringElementCollectionConverter))]
        public ConnectionStringElementCollection ConnectionStrings
        {
            get { return connectionStrings; }
        }

        /// <summary>
        /// Gets the urls.
        /// </summary>
        /// <value>The urls.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Resources")]
        [TypeConverter(typeof(UrlElementCollectionConverter))]
        public UrlElementCollection Urls
        {
            get { return urls; }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Additional parameters")]
        [TypeConverter(typeof(SettingElementCollectionConverter))]
        public SettingElementCollection Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Applications")]
        [TypeConverter(typeof(ApplicationElementCollectionConverter))]
        public ApplicationElementCollection Applications
        {
            get { return applications; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is master server.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is master server; otherwise, <c>false</c>.
        /// </value>
        public bool IsMasterServer
        {
            get { return Settings.All(s => s.Key.Equals("MasterServer", System.StringComparison.OrdinalIgnoreCase) == false); }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentElement"/> class.
        /// </summary>
        public EnvironmentElement()
        {
            this.connectionStrings = new ConnectionStringElementCollection();
            this.settings = new SettingElementCollection();
            this.urls = new UrlElementCollection();
            this.applications = new ApplicationElementCollection();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Name: " + Name;
        }

        #endregion Methods
    }
}
