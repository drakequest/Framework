using DrakeQuest.Log.Generic;

namespace DrakeQuest.Log.Log4Net.Generic
{
    internal class Log4NetLogger<T> : Log4NetLogger, ILogger<T>
	{
		public Log4NetLogger() : base(LevelEnum.Info, typeof(T), null) { }

		public Log4NetLogger(string loggerName) : base(LevelEnum.Info, typeof(T), loggerName) { }

		public Log4NetLogger(LevelEnum level) : base(level, typeof(T), null) { }

		public Log4NetLogger(LevelEnum level, string loggerName) : base(level, typeof(T), loggerName) { }

		public LogFactory<T> Factory { get; set; }
	}

}
