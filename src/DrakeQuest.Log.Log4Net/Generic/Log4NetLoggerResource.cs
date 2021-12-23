using DrakeQuest.Configuration;
using System;

namespace DrakeQuest.Log.Log4Net.Generic
{
    public class Log4NetResourcesLogger<T, TResources> : Log4NetLogger
	{
		#region Properties

		private IResourcesProviders<TResources> ResourcesManager { get; set; }

		#endregion

		#region Constructor

		public Log4NetResourcesLogger(string loggerName) : base(LevelEnum.Info, typeof(T), loggerName) { }

		public Log4NetResourcesLogger() : base(LevelEnum.Info, typeof(T), null) { }

		public Log4NetResourcesLogger(LevelEnum level) : base(level, typeof(T), null) { }

		public Log4NetResourcesLogger(LevelEnum level, string loggerName) : base(level, typeof(T), loggerName) { }

		#endregion

		#region Methods

		public override void Log(Exception exception, object message)
		{
			message = ResourcesManager.GetString((string)message);
			Log(DeclaringType, Level, message, exception);
		}

		public override void Log(object message)
		{
			message = ResourcesManager.GetString((string)message);
			Log(DeclaringType, Level, message, null);
		}

		public override void LogFormat(Exception exception, string format, params object[] args)
		{
			format = ResourcesManager.GetString((string)format);
			Log(DeclaringType, Level, string.Format(format, args), exception);
		}

		public override void LogFormat(string format, params object[] args)
		{
			format = ResourcesManager.GetString((string)format);
			Log(DeclaringType, Level, string.Format(format, args), null);
		}

		#endregion 

	}

}
