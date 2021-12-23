using System.Data;
using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	public class MapperInfoInteger : MapperInfo
	{
		public override object GetValue(IDataReader reader, int index)
		{
			return reader.GetInt32(index);
		}

		public MapperInfoInteger(PropertyInfo propertyInfo)
			: base(propertyInfo) { }
	}
}
