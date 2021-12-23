using System;

namespace DrakeQuest.Data
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public class OrderAttribute : Attribute
	{
		public int Order { get; }

		public OrderAttribute(int value)
		{
			Order = value;
		}
	}
}
