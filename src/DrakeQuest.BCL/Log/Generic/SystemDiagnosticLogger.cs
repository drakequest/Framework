
namespace DrakeQuest.Log.Generic
{
	/// <summary>
	/// Generic System.Diagnostic.Trace Logger
	/// </summary>
	/// <typeparam name="T">Type of the Service</typeparam>
	internal class SystemDiagnosticTraceLogger<T> : SystemDiagnosticTraceLogger, ILogger<T>
	{
		/// <summary>
		/// Log factory required 
		/// </summary>
		public LogFactory<T> Factory { get; set; }
		
		/// <summary>
		/// Create a new logger based on the name
		/// </summary>
		/// <param name="loggerName"></param>
		public SystemDiagnosticTraceLogger(string loggerName) 
			: base(LevelEnum.Info, typeof(T), loggerName) { }

		/// <summary>
		/// Create a new logger based on the type
		/// </summary>
		public SystemDiagnosticTraceLogger()
			: base(LevelEnum.Info, typeof(T)) { }

	}
}
