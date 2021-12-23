namespace DrakeQuest.Configuration
{
    using System.ComponentModel;

    /// <summary>
    /// Represents an URL
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class UrlElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Category("Properties")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [Category("Properties")]
        public string Url { get; set; }

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
