using System.Data;

namespace DrakeQuest.Data.Mapper
{
	public interface IDataReaderMapperFactory
	{
		IDataReaderObjectMapper GetMapper<T>(object key, IDataReader reader);
	}
}