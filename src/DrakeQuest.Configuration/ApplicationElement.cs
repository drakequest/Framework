using DrakeQuest.Configuration.Design;

using System.ComponentModel;


namespace DrakeQuest.Configuration
{
    /// <summary>
    /// Represents an environment element
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ApplicationElement
    {
        #region Attributes

        private ApplicationSettingElementCollection applicationSettings;

        #endregion Attributes

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Category("Properties")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Application settings")]
        [TypeConverter(typeof(ApplicationSettingElementCollectionConverter))]
        public ApplicationSettingElementCollection Settings
        {
            get { return applicationSettings; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationElement"/> class.
        /// </summary>
        public ApplicationElement()
        {
            this.applicationSettings = new ApplicationSettingElementCollection();
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
