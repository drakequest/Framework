using System.Data;
using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	public class MapperInfoLong : MapperInfo
	{
		public override object GetValue(IDataReader reader, int index)
		{
			return reader.GetInt64(index);
		}

		public MapperInfoLong(PropertyInfo propertyInfo)
			: base(propertyInfo){ }
	}
}
