using System.Data;
using System.Reflection;


namespace DrakeQuest.Data.Mapper
{
    public class MapperInfo : IMapperInfo
	{
		public PropertyInfo PropertyInfo { get; private set; }

		public virtual object GetValue(IDataReader reader, int index)
		{
			return reader.GetValue(index);
		}

		public MapperInfo(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;
		}
	}
}
