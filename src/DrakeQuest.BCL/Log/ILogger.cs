using System;

namespace DrakeQuest.Log
{
    public interface ILogger
	{

		// Summary:
		//     Log a message object with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   message:
		//     The message object to log.
		//
		// Remarks:
		//      This method first checks if this logger is DEBUG enabled by comparing the
		//     level of this logger with the log4net.Core.Level.Debug level. If this logger
		//     is DEBUG enabled, then it converts the message object (passed as parameter)
		//     to a string by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer.
		//     It then proceeds to call all the registered appenders in this logger and
		//     also higher in the hierarchy depending on the value of the additivity flag.
		//     WARNING Note that passing an System.Exception to this method will print the
		//     name of the System.Exception but no stack trace. To print a stack trace use
		//     the Debug(object,Exception) form instead.
		void Log(object message);

		//
		// Summary:
		//     Log a message object with the log4net.Core.Level.Debug level including the
		//     stack trace of the System.Exception passed as a parameter.
		//
		// Parameters:
		//   message:
		//     The message object to log.
		//
		//   exception:
		//     The exception to log, including its stack trace.
		//
		// Remarks:
		//      See the Debug(object) form for more detailed information.
		void Log(Exception exception, object message);

		//
		// Summary:
		//     Logs a formatted message string with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   format:
		//     A String containing zero or more format items
		//
		//   args:
		//     An Object array containing zero or more objects to format
		//
		// Remarks:
		//      The message is formatted using the String.Format method. See String.Format(string,
		//     object[]) for details of the syntax of the format string and the behavior
		//     of the formatting.
		//     This method does not take an System.Exception object to include in the log
		//     event. To pass an System.Exception use one of the Debug(object,Exception)
		//     methods instead.
		void LogFormat(string format, params object[] args);

		//
		// Summary:
		//     Logs a formatted message string with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   format:
		//     A String containing zero or more format items
		//
		//   args:
		//     An Object array containing zero or more objects to format
		//
		// Remarks:
		//      The message is formatted using the String.Format method. See String.Format(string,
		//     object[]) for details of the syntax of the format string and the behavior
		//     of the formatting.
		//     This method does not take an System.Exception object to include in the log
		//     event. To pass an System.Exception use one of the Debug(object,Exception)
		//     methods instead.
		void LogFormat(Exception exception, string format, params object[] args);

		// Summary:
		//     Log a message object with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   message:
		//     The message object to log.
		//
		// Remarks:
		//      This method first checks if this logger is DEBUG enabled by comparing the
		//     level of this logger with the log4net.Core.Level.Debug level. If this logger
		//     is DEBUG enabled, then it converts the message object (passed as parameter)
		//     to a string by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer.
		//     It then proceeds to call all the registered appenders in this logger and
		//     also higher in the hierarchy depending on the value of the additivity flag.
		//     WARNING Note that passing an System.Exception to this method will print the
		//     name of the System.Exception but no stack trace. To print a stack trace use
		//     the Debug(object,Exception) form instead.
		void Log(LevelEnum level, object message);

		//
		// Summary:
		//     Log a message object with the log4net.Core.Level.Debug level including the
		//     stack trace of the System.Exception passed as a parameter.
		//
		// Parameters:
		//   message:
		//     The message object to log.
		//
		//   exception:
		//     The exception to log, including its stack trace.
		//
		// Remarks:
		//      See the Debug(object) form for more detailed information.
		void Log(LevelEnum level, Exception exception, object message);

		//
		// Summary:
		//     Logs a formatted message string with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   format:
		//     A String containing zero or more format items
		//
		//   args:
		//     An Object array containing zero or more objects to format
		//
		// Remarks:
		//      The message is formatted using the String.Format method. See String.Format(string,
		//     object[]) for details of the syntax of the format string and the behavior
		//     of the formatting.
		//     This method does not take an System.Exception object to include in the log
		//     event. To pass an System.Exception use one of the Debug(object,Exception)
		//     methods instead.
		void LogFormat(LevelEnum level, string format, params object[] args);

		//
		// Summary:
		//     Logs a formatted message string with the log4net.Core.Level.Debug level.
		//
		// Parameters:
		//   format:
		//     A String containing zero or more format items
		//
		//   args:
		//     An Object array containing zero or more objects to format
		//
		// Remarks:
		//      The message is formatted using the String.Format method. See String.Format(string,
		//     object[]) for details of the syntax of the format string and the behavior
		//     of the formatting.
		//     This method does not take an System.Exception object to include in the log
		//     event. To pass an System.Exception use one of the Debug(object,Exception)
		//     methods instead.
		void LogFormat(LevelEnum level, Exception exception, string format, params object[] args);

		void Configure();

		void SetDefault(LevelEnum level);
	}
}
