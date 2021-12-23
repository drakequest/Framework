namespace DrakeQuest.Log.Generic
{

    public delegate ILogger<T> LogFactory<out T>(string loggerName);

	public interface ILogger<out T> : ILogger
	{
		LogFactory<T> Factory { get; }
	}
}
