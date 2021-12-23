using System.Data;
using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	public class MapperInfoDouble : MapperInfo
	{
		public override object GetValue(IDataReader reader, int index)
		{
			return reader.GetDouble(index);
		}

		public MapperInfoDouble(PropertyInfo propertyInfo) : base(propertyInfo) { }
	}
}
