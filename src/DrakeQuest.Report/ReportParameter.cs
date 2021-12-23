namespace DrakeQuest.Report
{
	public class ReportParameter
	{
		public string Name { get; set; }

		public object Value { get; set; }

		public override string ToString()
		{
			return $"{{name:{Name}, value:{Value}}}";
		}
	}
}
