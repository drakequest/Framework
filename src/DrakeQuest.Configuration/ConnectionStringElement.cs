namespace DrakeQuest.Configuration
{
    using System.ComponentModel;

    /// <summary>
    /// Represents a connection string
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ConnectionStringElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Category("Properties")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [Category("Properties")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        [Category("Properties")]
        public string Provider { get; set; } 

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
