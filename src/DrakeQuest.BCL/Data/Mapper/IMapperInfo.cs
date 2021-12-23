using System.Data;
using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	public interface IMapperInfo
	{
		PropertyInfo PropertyInfo { get; }

		object GetValue(IDataReader reader, int index);
	}
}
