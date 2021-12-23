using Autofac;
using DrakeQuest.Report.SSRS.ReportingServices;

namespace DrakeQuest.Report.SSRS
{
	public class Module : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);
			
			builder.RegisterType<ReportEngineService>()
				.Keyed<IReportEngine>(ReportEngineKey.Service);

			/*builder.RegisterType<WebReportingTask>()
				.Keyed<global::NAnt.Core.Task>("webReport");*/
		}
	}
}
