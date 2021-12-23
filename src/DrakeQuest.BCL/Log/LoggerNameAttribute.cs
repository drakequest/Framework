namespace DrakeQuest.Log
{
	/// <summary>
	/// This attribute <c>LoggerNameAttribute</c> applies to class, struct, interface attribute that will set the logger name for the logger
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class LoggerNameAttribute : System.Attribute
	{
		/// <summary>
		/// Name that will use the logger
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Constructor to set the logger name with <c>name</c>
		/// </summary>
		/// <param name="name"></param>
		public LoggerNameAttribute(string name)
		{
			Name = name;
		}
	}
}
