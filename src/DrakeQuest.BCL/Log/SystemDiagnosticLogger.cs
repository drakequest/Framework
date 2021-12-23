using System;
using System.Diagnostics;

namespace DrakeQuest.Log
{
	/// <summary>
	/// Base implementation of the logger using System.Diagnostics trace 
	/// </summary>
	internal class SystemDiagnosticTraceLogger : ILogger
    {
		#region Properties

		/// <summary>
		/// Default Log level of the log(when no message level used)
		/// </summary>
		protected LevelEnum Level { get; private set; }

		/// <summary>
		/// LoggerName when logging message
		/// </summary>
		public string LoggerName { get; }

		#endregion

		#region

		/// <summary>
		/// Change the default Log Level
		/// </summary>
		/// <param name="level"></param>
		public void SetDefault(LevelEnum level)
		{
			Level = level;
		}

		/// <summary>
		/// Construtor for a new instance of Logger
		/// </summary>
		/// <param name="level"><see cref="LevelEnum"/> Default level to use when logging</param>
		/// <param name="type"><c>Type</c> Type of the calling object that will log something.</param>
		/// <param name="loggerName">Logger name to use in the message calling of the calling object that will log something.</param>
		protected SystemDiagnosticTraceLogger(LevelEnum level, Type type, string loggerName = null)
		{
			Level = level;
			LoggerName = this.GetLoggerName(type, loggerName);
		}

		#endregion

		#region methods

		/// <summary>
		/// Format the message to be written in the Trace <c>LoggerName</c>::<c>message</c>
		/// </summary>
		/// <param name="message">meesage to be written in the log trace</param>
		/// <returns>message formatted</returns>
		private string FormatMessage(object message){
			return $"{LoggerName}::{message}";
		}

		/// <summary>
		/// Format the message to be written in the Trace <c>LoggerName</c>::<c>string.Format(message, args)</c>
		/// </summary>
		/// <param name="format">Message with format placehoder</param>
		/// <param name = "args" > An object array that contains zero or more objects to format. </param>
		/// <returns>message formatted</returns>
		private string FormatMessage(string format, object[] args)
		{
			return  $"{LoggerName}::{ string.Format(format, args)}";
		}

		/// <summary>
		/// Write in trace <c>message</c> with the default level
		/// </summary>
		/// <param name="message">message to be logged</param>
		public void Log(object message)
		{
			Trace.Write(FormatMessage(message), Level.ToString());
		}

		/// <summary>
		/// Write in trace an exception occured <c>exception</c> with the <c>message</c> with the default level
		/// </summary>
		/// <param name="exception">Exception to be written</param>
		/// <param name="message">message to be logged</param>
		public void Log(Exception exception, object message)
		{
			Trace.Write(FormatMessage(message), Level.ToString());
			Trace.Write(exception, Level.ToString());
		}

		/// <summary>
		/// Write in trace a message that will be generated from the <c>format</c> and <c>args</c> with the default level
		/// </summary>
		/// <param name="format">string format of the message to be logged</param>
		/// <param name = "args" > An object array that contains zero or more objects to format. </param>
		public void LogFormat(string format, params object[] args)
		{
			Trace.Write(FormatMessage(format, args), Level.ToString());
		}

		/// <summary>
		/// Write in trace an exception occured <c>exception</c> with a message that will be generated from the <c>format</c> and <c>args</c> with the default level
		/// </summary>
		/// <param name="exception">Exception to be written</param>
		/// <param name="format">string format of the message to be logged</param>
		/// <param name = "args" > An object array that contains zero or more objects to format. </param>
		public void LogFormat(Exception exception, string format, params object[] args)
		{
			Trace.Write(FormatMessage(format, args), Level.ToString());
			Trace.Write(exception, Level.ToString());
		}

		/// <summary>
		/// Write in trace <c>message</c> with the <c>level</c> as level
		/// </summary>
		/// <param name="level">Level log to use instead of default</param>
		/// <param name="message">message to be logged</param>
		public void Log(LevelEnum level, object message)
		{
			Trace.Write(FormatMessage(message), level.ToString());
		}

		/// <summary>
		/// Write in trace an exception occured <c>exception</c> with the message in parameter with the <c>level</c> as level
		/// </summary>
		/// <param name="level">Level log to use instead of default</param>
		/// <param name="exception">Exception to be written</param>
		/// <param name="message">message to be logged</param>
		public void Log(LevelEnum level, Exception exception, object message)
		{
			Trace.Write(FormatMessage(message), level.ToString());
			Trace.Write(exception, Level.ToString());
		}

		/// <summary>
		/// Write in trace a message that will be generated from the <c>format</c> and <c>args</c> with the <c>level</c> as level
		/// </summary>
		/// <param name="level">Level log to use instead of default</param>
		/// <param name="format">message to be logged</param>
		/// <param name = "args" > An object array that contains zero or more objects to format. </param>
		public void LogFormat(LevelEnum level, string format, params object[] args)
		{
			Trace.Write(FormatMessage(format, args), Level.ToString());
		}

		/// <summary>
		/// Write in trace an exception occured <c>exception</c> that will be generated from the <c>format</c> and <c>args</c> with the <c>level</c> as level
		/// </summary>
		/// <param name="level">Level log to use instead of default</param>
		/// <param name="format">message to be logged</param>
		/// <param name = "args" > An object array that contains zero or more objects to format. </param>
		public void LogFormat(LevelEnum level, Exception exception, string format, params object[] args)
		{
			Trace.Write(FormatMessage(format, args), Level.ToString());
			Trace.Write(exception, Level.ToString());
		}


		/// <summary>
		/// Some Logger will need to be configured
		/// </summary>
		public void Configure() { }

		#endregion
	}
}
