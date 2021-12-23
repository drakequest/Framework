using System.ComponentModel;

namespace DrakeQuest.Log
{
	/// <summary>
	/// Defines the set of levels recognised by the NAnt logging system.
	/// </summary>
	[TypeConverter(typeof(LevelConverter))]
	public enum LevelEnum : int
	{
		/// <summary>
		/// To pay attention
		/// </summary>
		Alert = 4500,

		/// <summary>
		/// Application error
		/// </summary>
		Fatal = 6000,

		/// <summary>
		/// Designates fine-grained informational events that are most useful 
		/// </summary>
		Debug = 1000,

		/// <summary>
		/// Designates events that offer a more detailed.
		/// </summary>
		Verbose = 2000,

		/// <summary>
		/// Designates informational events that are useful for getting a 
		/// high-level view.
		/// </summary>
		Info = 3000,

		/// <summary>
		/// Designates potentionally harmful events.
		/// </summary>
		Warning = 4000,
		Warn = 4000,

		/// <summary>
		/// Designates error events.
		/// </summary>
		Error = 5000,

		/// <summary>
		/// Can be used to suppress all messages.
		/// </summary>
		/// <remarks>
		/// No events should be logged with this <see cref="Level" />.
		/// </remarks>
		None = 9999

	}
}
