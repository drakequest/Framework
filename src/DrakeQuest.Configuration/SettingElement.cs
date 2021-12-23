namespace DrakeQuest.Configuration
{
    using System.ComponentModel;

    /// <summary>
    /// Represents a setting in the environment configuration
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SettingElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [Category("Properties")]
        public string Key { get; set; }

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
            return Key;
        }

        #endregion Methods
    }
}
