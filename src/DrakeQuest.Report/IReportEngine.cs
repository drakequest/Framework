using System.Collections.Generic;

namespace DrakeQuest.Report
{
	public interface IReportEngine
	{
		List<ReportParameter> Execute(string name, string query);

		void Render(ReportRenderArg args, string serverUrl);
	}
}
