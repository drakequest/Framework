using System;
using System.ComponentModel;
using System.Globalization;

namespace DrakeQuest.Configuration.Design
{
    /// <summary>
    /// Converter for the <see cref="ConnectionStringElementCollection"/> class.
    /// </summary>
    public sealed class ConnectionStringElementCollectionConverter : ExpandableObjectConverter
    {
        #region Overrides

        /// <summary>
        /// Converts the given object to another type.
        /// </summary>
        /// <param name="context">A formatter context.</param>
        /// <param name="culture">The culture into which value will be converted.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var collection = value as ConnectionStringElementCollection;

            if (destinationType == typeof(string) && collection != null)
            {
                return "Count: " + collection.Count;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Overrides
    }
}
