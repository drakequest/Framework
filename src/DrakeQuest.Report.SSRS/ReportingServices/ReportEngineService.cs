using System;
using System.IO;
using System.Linq;
using SsrsParameterValue = DrakeQuest.Report.SSRS.ReportingServices.ParameterValue;
using DrakeQuestReportParameter = DrakeQuest.Report.ReportParameter;
using DrakeQuest.Log;
using System.Web.Services.Protocols;
using DrakeQuest.Log.Generic;
using DrakeQuest.IO;

namespace DrakeQuest.Report.SSRS.ReportingServices
{
	internal class ReportEngineService : ReportEngine, Report.IReportEngine
	{

		#region Constructors

		public ReportEngineService(ILogger<ReportEngine> logger, IDirectoryService directory, IFileService fileService, IEurekaAccess eurekaAccess)
			: base(logger, directory, fileService, eurekaAccess) { }

		#endregion


		#region methods

		public void Render(ReportRenderArg args, string serverUrl)
		{
			var svcParameters = args.Parameters
				.Select(p => ToSsrsParameter(p))
				.ToArray();

			using (var rs = GetService(serverUrl, args.TimeoutInMinute))
			{
				
				Logger.Log(LevelEnum.Info, $"Loading report <{args.ItemPath}>");
				var execInfo = rs.LoadReport(args.ItemPath, null);
				Logger.Log(LevelEnum.Info, $"Setting report parameters");
				rs.SetExecutionParameters(svcParameters, "en-us");
				Logger.Log(LevelEnum.Info, $"SessionId: {rs.ExecutionHeaderValue.ExecutionID}");
				GetExtensions(args.RenderFormat).ForEach(a => Render(rs, a, args));
			}
		}

		#endregion

		#region Private Methods

		private SsrsParameterValue ToSsrsParameter(DrakeQuestReportParameter parameter)
		{

			if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name))
				return null;

			if (parameter.Name.EndsWith(":IsNull"))
			{
				return new SsrsParameterValue { Name = parameter.Name.Replace(":IsNull", ""), Value = null };
			}
			else
			{
				return new SsrsParameterValue { Name = parameter.Name, Value = parameter.Value?.ToString() };
			}
		}

		private void Render(ReportExecutionService rs, string extension, ReportRenderArg args)
		{
			byte[] result = null;
			string ext = null;
			string executionID = null;
			var checkIsEmpty = args.EmptyOutput != null && !string.IsNullOrWhiteSpace(args.MainDataSet);

			try
			{
				Logger.Log(LevelEnum.Info, $"Rendering report {args.ItemPath} for {extension} format...");
				result = rs.Render(GetFormat(extension), null, out ext, out string encoding, out string mimeType, out Warning[] warnings, out string[] streamId);
				var execInfo = rs.GetExecutionInfo();
				executionID = execInfo.ExecutionID;
				Logger.Log(LevelEnum.Info, $"ExecutionID: {execInfo.ExecutionID}");
				Logger.Log(LevelEnum.Debug, $"DataSourcePrompts: ");
				rs.GetExecutionInfo2().DataSourcePrompts.Select(e => e.Prompt).ToList().ForEach(p => Logger.Log(LevelEnum.Debug, p));
				Logger.Log(LevelEnum.Debug, $"Stream Ids: ");
				Logger.Log(LevelEnum.Debug, streamId);
				Logger.Log(LevelEnum.Debug, $"Warning Messages: ");
				warnings?.ToList().ForEach(w => Logger.Log(LevelEnum.Debug, w.Message));
				
			}
			catch (SoapException ex)
			{
				var message = $"Failed to generate report : {args.ItemPath} see error below :{Environment.NewLine}:{PrintXML(ex.Detail.OuterXml)}";
				Logger.Log(LevelEnum.Error, message);
				throw new RenderingException(message, ex);
			}

			ReportOutput output = (checkIsEmpty && IsEmpty(rs, executionID, args.MainDataSet)) ? args.EmptyOutput : args.Output;

			string path = $"{Path.Combine(output.DirectoryName, output.FileName)}.{extension}";

			Logger.Log(LevelEnum.Info, $"Saving the report {args.ItemPath} into {path}...");
			DirectoryService.CreateDirectory(output.DirectoryName);

			try
			{
				using (FileStream stream = FileService.Create(path, result.Length))
				{
					stream.Position = 0;
					stream.Write(result, 0, result.Length);
				}
			}
			catch (Exception ex)
			{
				var message = $"Failed to save the report : {args.ItemPath} into {path} : {ex.Message}";
				Logger.Log(LevelEnum.Error, message);
				throw new RenderingException(message, ex);
			}
		}

		private ReportExecutionService GetService(string serverUrl, int timeoutInMinutes = 10)
		{
			ReportExecutionService reportExecutionService = new ReportExecutionService
			{
				Url = $"{serverUrl}/ReportExecution2005.asmx?wsdl",
				Credentials = System.Net.CredentialCache.DefaultCredentials,
				Timeout = 1000 * 60 * timeoutInMinutes
			};
			ReportExecutionService rs = reportExecutionService;
			return rs;
		}

		private bool IsEmpty(ReportExecutionService rs, string id, string mainDataSet)
		{
			rs.GetExecutionInfo2().DataSourcePrompts.Select(e => e.Prompt).ToList().ForEach(p => Console.WriteLine(p));

			var count = DataAccess.GetRowCount(id, mainDataSet);
			return count == 0;
		}

		#endregion Private methods


	}
}
