using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DrakeQuest.Configuration;

namespace DrakeQuest.Log
{
	public static class LogExtension
	{

		public static string GetLoggerName(this ILogger logger, Type type, string loggerName = null)
		{
			if (!string.IsNullOrWhiteSpace(loggerName))
				return loggerName;

			if (type == null)
				throw new ArgumentNullException(nameof(type));

			LoggerNameAttribute attribute = type.GetCustomAttributes<LoggerNameAttribute>(false).FirstOrDefault();
			if (attribute != null)
			{
				return attribute.Name;
			}

			attribute = type.GetCustomAttributes<LoggerNameAttribute>(true).FirstOrDefault();
			if (attribute != null)
			{
				return attribute.Name;
			}
			////
			/// TODO: remove support for this code
			///
			var field = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
					.Where(f => f.Name == "LoggerName" && f.FieldType == typeof(string))
					.FirstOrDefault();
			if (field != null)
			{
				return (string)field.GetRawConstantValue();
			}


			return type.ToString();


		}
	}
}
