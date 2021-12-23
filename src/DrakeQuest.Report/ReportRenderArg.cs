using System.Collections.Generic;

namespace DrakeQuest.Report
{
	public class ReportRenderArg
	{
		public int TimeoutInMinute { get; set; }

		public string ItemPath{ get; set; }

		public string MainDataSet { get; set; }

		public RenderFormat RenderFormat { get; set; }

		public ReportOutput Output { get; set; }

		public ReportOutput EmptyOutput { get; set; }

		public List<ReportParameter> Parameters { get; set; }
	}
}
