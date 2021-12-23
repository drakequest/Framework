namespace DrakeQuest.Configuration
{
    using System;

    /// <summary>
    /// Represents a configuration property enclosed in a CDATA section
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CDataConfigurationPropertyAttribute : Attribute
    {
    }
}
