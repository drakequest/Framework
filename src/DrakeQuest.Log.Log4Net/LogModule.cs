using Autofac;
using DrakeQuest.Log.Generic;

namespace DrakeQuest.Log.Log4Net
{
	public class LogModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);
			builder.RegisterGeneric(typeof(Generic.Log4NetLogger<>))
				.AsSelf()
				.As(typeof(ILogger<>));
		}
	}
}
