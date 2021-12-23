using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;

using DrakeQuest.Configuration.Properties;

namespace DrakeQuest.Configuration
{
    /// <summary>
    /// Configuration element with a value enclosed in a CDATA section
    /// </summary>
    public class CDataConfigurationElement : ConfigurationElement
    {
        private readonly string cDataConfigurationPropertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CDataConfigurationElement"/> class.
        /// </summary>
        public CDataConfigurationElement()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            int cDataConfigurationPropertyCount = 0;
            int configurationElementPropertyCount = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                ConfigurationPropertyAttribute[] configurationPropertyAttributes = getAttributes<ConfigurationPropertyAttribute>(property);
                CDataConfigurationPropertyAttribute[] cDataConfigurationPropertyAttribute = getAttributes<CDataConfigurationPropertyAttribute>(property);

                bool hasConfigurationPropertyAttribute = configurationPropertyAttributes.Length != 0;
                bool hasCDataConfigurationPropertyAttribute = cDataConfigurationPropertyAttribute.Length != 0;

                if (hasConfigurationPropertyAttribute &&
                    property.PropertyType.IsSubclassOf(typeof(ConfigurationElement)))
                {
                    configurationElementPropertyCount++;
                }

                if (hasCDataConfigurationPropertyAttribute)
                {
                    cDataConfigurationPropertyCount++;
                    throwIf(cDataConfigurationPropertyCount > 1,
                            Resources.ERROR_TOO_MANY_CDATA_CONFIGURATION_ELEMENTS);

                    throwIf(!hasConfigurationPropertyAttribute,
                            Resources.ERROR_MISSING_CONFIGURATION_PROPERTY_ATTRIBUTE,
                            property.Name);

                    throwIf(!property.PropertyType.Equals(typeof(string)),
                            Resources.ERROR_CDATA_CONFIGURATION_PROPERTY_MUST_BE_STRING,
                            property.Name);

                    cDataConfigurationPropertyName = configurationPropertyAttributes[0].Name;
                }
            }

            throwIf(configurationElementPropertyCount > 0 &&
                    cDataConfigurationPropertyCount > 0,
                    Resources.ERROR_CLASS_CONTAINS_CONFIGURATION_PROPERTY,
                    GetType().FullName);
        }

        private T[] getAttributes<T>(PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttributes(typeof(T), true).Cast<T>().ToArray();
        }

        /// <summary>
        /// Throws an exception if the specified condition is true.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> throw an exception.</param>
        /// <param name="formatString">The format string.</param>
        /// <param name="values">The values.</param>
        private void throwIf(bool condition, string formatString, params object[] values)
        {
            if (condition)
            {
                if (values.Length > 0)
                {
                    formatString = string.Format(formatString, values);
                }

                Trace.WriteLine(formatString);
                throw new ConfigurationErrorsException(formatString);
            }
        }

        /// <summary>
        /// Writes the contents of this configuration element to the configuration file when implemented in a derived class.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> that writes to the configuration file.</param>
        /// <param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        /// <returns>
        /// true if any data was actually serialized; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
        /// The current attribute is locked at a higher configuration level.
        /// </exception>
        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            bool returnValue;
            if (string.IsNullOrEmpty(cDataConfigurationPropertyName))
            {
                returnValue = base.SerializeElement(writer, serializeCollectionKey);
            }
            else
            {
                foreach (ConfigurationProperty configurationProperty in Properties)
                {
                    string name = configurationProperty.Name;
                    TypeConverter converter = configurationProperty.Converter;
                    string propertyValue = converter.ConvertToString(base[name]);

                    if (writer != null)
                    {
                        if (name == cDataConfigurationPropertyName)
                        {
                            writer.WriteCData(propertyValue);
                        }
                        else
                        {
                            writer.WriteAttributeString("name", propertyValue);
                        }
                    }
                }

                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> that reads from the configuration file.</param>
        /// <param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
        /// The element to read is locked.
        /// - or -
        /// An attribute of the current node is not recognized.
        /// - or -
        /// The lock status of the current node cannot be determined.
        /// </exception>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            if (string.IsNullOrEmpty(cDataConfigurationPropertyName))
            {
                base.DeserializeElement(reader, serializeCollectionKey);
            }
            else
            {
                foreach (ConfigurationProperty configurationProperty in Properties)
                {
                    string name = configurationProperty.Name;
                    if (name == cDataConfigurationPropertyName)
                    {
                        string contentString = reader.ReadString();
                        base[name] = contentString.Trim();
                    }
                    else
                    {
                        string attributeValue = reader.GetAttribute(name);
                        base[name] = attributeValue;
                    }
                }

                reader.ReadEndElement();
            }
        }
    }
}
