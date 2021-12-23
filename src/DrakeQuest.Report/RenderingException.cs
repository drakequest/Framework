using System;
using System.Runtime.Serialization;

namespace DrakeQuest.Report
{
	public sealed class RenderingException : Exception
	{
		public RenderingException(string message, Exception exception) : base(message, exception) { }

		public RenderingException(Exception exception) : base("An error occured while rendering", exception) { }

		public RenderingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
