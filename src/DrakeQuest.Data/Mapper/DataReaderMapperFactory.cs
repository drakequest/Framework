using DrakeQuest.Data.Properties;
using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
using DrakeQuest.DependencyInjection;

namespace DrakeQuest.Data.Mapper
{
  internal class DataReaderMapperFactory : IDataReaderMapperFactory
	{
		#region Fields

		/// <summary>
		/// 
		/// </summary>
		public const string LoggerName = "[Data.ObjectMapper]";

		#endregion

		#region Properties

		private Dictionary<object, IDataReaderObjectMapper> Mappers { get; set; }

		public ILogger Logger { get; private set; }

		#endregion

		#region Constructor

		public DataReaderMapperFactory(ILogger<DataReaderMapperFactory> logger)
		{
			Logger = logger;
			Mappers = new Dictionary<object, IDataReaderObjectMapper>();
		}

		#endregion

		#region Methods

		public IDataReaderObjectMapper GetMapper<T>(object key, IDataReader reader)
		{
			if (Mappers.Keys.Contains(key))
				return (IDataReaderObjectMapper)Mappers[key];

			var mapper = Bootstrapper.GetContainer().Resolve<IDataReaderObjectMapper>(); ;
			mapper.Initialize<T>(reader);
			Mappers.Add(key, mapper);
			return mapper;
		}

		/// <summary>
		/// Populate an object of Type T, using the statment and the data from the reader
		/// </summary>
		/// <typeparam name="T">Type of the object to map	</typeparam>
		/// <param name="data">Object to map</param>
		/// <param name="sqlStatement">procedure,direct statment used to get information</param>
		/// <param name="reader">DataReader to process</param>
		public void Populate<T>(ref T data, string sqlStatement, IDataReader reader)
		{
			IDataReaderObjectMapper mapper = null;
			try
			{
				mapper = GetMapper<T>(sqlStatement, reader);
				mapper.FieldCount = reader.FieldCount;
			}
			catch (Exception ex)
			{
				Logger.LogFormat(LevelEnum.Warn, ex, Resources.ERR_MAPPING_GET, ex.Message);
				throw;
			}
			mapper.Populate(ref data, reader);
		}

		#endregion
	}
}
