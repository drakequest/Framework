using System;

namespace DrakeQuest.Data
{
    /// <summary>
    /// Describe a 
    /// </summary>
	public class PropertyDescriptionAttribute : Attribute
	{
		#region Properties

		public string Name { get; }

		public int? Order { get; }

		#endregion

		#region Constructor

		public PropertyDescriptionAttribute(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			Name = name;
		}

		public PropertyDescriptionAttribute(string name, int order)
			: this(name)
		{
			Order = order;
		}

		#endregion
	}
}
