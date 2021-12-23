using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	public interface IMapperPropertyInfo
	{
		PropertyInfo PropertyInfo { get; }

		string Name { get; }

		int? Order { get; }

		object MetaInformation { get; }
	}
}
