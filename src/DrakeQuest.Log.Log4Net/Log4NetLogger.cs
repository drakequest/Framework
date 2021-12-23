using System;
using System.Linq;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;
using DrakeQuest.Configuration;
using Autofac;
using DrakeQuest.DependencyInjection;

namespace DrakeQuest.Log.Log4Net
{
	public abstract class Log4NetLogger : ILogger
    {
		#region Properties

		protected Type DeclaringType { get; private set; }

		protected Level Level { get; private set; }

		private log4net.Core.ILogger Logger { get; set; }

		#endregion

		#region Constructor

		protected Log4NetLogger(LevelEnum level, Type type, string loggerName)
		{
			DeclaringType = type;
			Level = GetLevel(level);

			loggerName = this.GetLoggerName(type, loggerName);

			Logger = string.IsNullOrWhiteSpace(loggerName)
				? LogManager.GetLogger(type).Logger
				: LogManager.GetLogger(type.Assembly, loggerName).Logger;
		}

		#endregion

		#region Methods

		protected void Log(Type declaringType, Level level, object message, Exception exception)
		{
			if(Logger.IsEnabledFor(level))
				Logger.Log(declaringType, level,message, exception);
		}

		public void SetDefault(LevelEnum level)
		{
			Level = GetLevel(level);
		}

		public void Log(Type declaringType, LevelEnum level, object message, Exception exception)
		{
			Level lvl = GetLevel(level);
			if (Logger.IsEnabledFor(lvl))
				Logger.Log(declaringType, lvl, message, exception);
		}

		public virtual void Log(LevelEnum level, object message)
		{
			Log(DeclaringType, level, message, null);
		}

		public virtual void Log(LevelEnum level, Exception exception, object message)
		{
			Log(DeclaringType,  level, message, exception);
		}

		public virtual void LogFormat(LevelEnum level, string format, params object[] args)
		{
			Log(DeclaringType, level, string.Format(format, args), null);
		}

		public virtual void LogFormat(LevelEnum level, Exception exception, string format, params object[] args)
		{
			Log(DeclaringType, level, string.Format(format, args), exception);
		}


		public virtual void Log(object message)
		{
			Log(DeclaringType, Level, message, null);
		}

		public virtual void Log(Exception exception, object message)
		{
			Log(DeclaringType, Level, message, exception);
		}

		public virtual void LogFormat(string format, params object[] args)
		{
			Log(DeclaringType, Level, string.Format(format, args), null);
		}

		public virtual void LogFormat(Exception exception, string format, params object[] args)
		{
			Log(DeclaringType, Level, string.Format(format, args), exception);
		}

		public static void ConfigureLog4Net()
		{
			IConfigurationProvider configProvider = Bootstrapper.GetContainer().Resolve<IConfigurationProvider>();

			string logName = configProvider.GetAppSettings("log4net:FileName");
			string applicationName = configProvider.GetAppSettings("log4net:ApplicationName");
			string log4NetPath = configProvider.GetAppSettings("log4net:ConfigFileName");

			if (logName == null)
			{
				logName = Assembly.GetEntryAssembly().GetName().Name;
			}

			GlobalContext.Properties["appName"] = applicationName;
			GlobalContext.Properties["fileName"] = logName;


			if (log4NetPath == null || !File.Exists(log4NetPath))
			{
				XmlConfigurator.Configure();
			}
			else
			{
				FileInfo fi = new FileInfo(log4NetPath);
				XmlConfigurator.Configure(fi);
			}
		}

		public virtual void Configure()
		{
			ConfigureLog4Net();
		}

		public Level GetLevel(LevelEnum level)
		{
			return (Level)typeof(Level).GetField(level.ToString(), BindingFlags.Public | BindingFlags.Static).GetValue(null);
		}

		#endregion
	}
}
