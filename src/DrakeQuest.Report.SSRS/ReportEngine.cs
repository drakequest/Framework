using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using DrakeQuest.IO;
using DrakeQuest.Log;
using DrakeQuestReportParameter = DrakeQuest.Report.ReportParameter;

namespace DrakeQuest.Report.SSRS
{
	internal abstract class ReportEngine
	{

		#region properties

		public IDirectoryService DirectoryService { get; }

		public IFileService FileService { get; }

		public ILogger Logger { get; }

		public IEurekaAccess DataAccess { get; }

		#endregion

		#region Constructors

		protected ReportEngine(ILogger logger, IDirectoryService directory, IFileService fileService, IEurekaAccess eurekaAccess )
		{
			Logger = logger;
			DirectoryService = directory;
			FileService = fileService;
			DataAccess = eurekaAccess;
		}

		#endregion

		#region Methods

		public List<DrakeQuestReportParameter> Execute(string name, string query)
		{
			if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(name))
				return null;

			List<DrakeQuestReportParameter> result = new List<DrakeQuestReportParameter>();

			var values = (List<string>)DataAccess.GetParameterValues(query) ?? new List<string>();
			if (values.Count == 0)
			{
				result.Add(new DrakeQuestReportParameter { Name = $"{name}:IsNull" });
			}
			else
			{
				values.ForEach(val => result.Add(new DrakeQuestReportParameter { Name = name, Value = val }));
			}
			return result;
		}

		protected string GetFormat(string extension)
		{
			switch (extension)
			{
				case "pdf": return "pdf";
				case "xls": return "excel";
				case "csv": return "csv";
				case "xlsx": return "excelopenxml";
				case "doc": return "word";
				case "docx": return "wordopenxml";
				case "xml": return "xml";
				default:
					throw new NotSupportedException($"The format is not supported");
			}
		}

		protected List<string> GetExtensions(RenderFormat formats)
		{
			List<string> extensions = new List<string>();
			extensions = Add(extensions, formats, RenderFormat.csv);
			extensions = Add(extensions, formats, RenderFormat.doc);
			extensions = Add(extensions, formats, RenderFormat.docx);
			extensions = Add(extensions, formats, RenderFormat.pdf);
			extensions = Add(extensions, formats, RenderFormat.xls);
			extensions = Add(extensions, formats, RenderFormat.xlsx);
			extensions = Add(extensions, formats, RenderFormat.xml);
			return extensions;
		}

		private List<string> Add(List<string> extensions, RenderFormat formats, RenderFormat renderFormat){
			if ((formats & renderFormat) == renderFormat)
				extensions.Add(renderFormat.ToString().ToLower());
			return extensions;
		}

		protected string PrintXML(string xml)
		{
			string result = "";
			using (MemoryStream mStream = new MemoryStream())
			using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode))
			{
				XmlDocument document = new XmlDocument();

				try
				{
					// Load the XmlDocument with the XML.
					document.LoadXml(xml);

					writer.Formatting = Formatting.Indented;

					// Write the XML into a formatting XmlTextWriter
					document.WriteContentTo(writer);
					writer.Flush();
					mStream.Flush();

					// Have to rewind the MemoryStream in order to read
					// its contents.
					mStream.Position = 0;

					// Read MemoryStream contents into a StreamReader.
					StreamReader sReader = new StreamReader(mStream);

					// Extract the text from the StreamReader.
					string formattedXml = sReader.ReadToEnd();

					result = formattedXml;
				}
				catch (XmlException)
				{
					// Handle the exception
				}
			}
			return result;
		}

		#endregion Methods
	}
}
