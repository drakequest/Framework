using System.ComponentModel;
using System.Security;

namespace DrakeQuest.Configuration
{
    /// <summary>
    /// Represents a setting in the environment configuration
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ApplicationSettingElement
    {
        #region Attributes

        private string path = string.Empty;

        #endregion Attributes

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Category("Properties")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [Category("Properties")]
        public string Path
        {
            get
            {
                return path.Replace("&apos;", "'")
                           .Replace("&quot;", "\"")
                           .Replace("&gt;", ">")
                           .Replace("&lt;", "<")
                           .Replace("&amp;", "&");
            }
            set
            {
                path = SecurityElement.Escape(value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Category("Properties")]
        public string Value { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}
