using System;

namespace DrakeQuest.Report
{
	[Flags]
	public enum RenderFormat : int
	{
		pdf  = 01,
		xls  = 02,
		xlsx = 04,
		csv  = 08,
		doc  = 16,
		docx = 32,
		xml  = 64
	}
}