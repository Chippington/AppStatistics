using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Reporting;
using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for AnalyticsWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
[System.Web.Script.Services.ScriptService]
public class AnalyticsWebService : System.Web.Services.WebService {

	[WebMethod]
	[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
	public object GetActivity(string startDate, string endDate) {
		try {
			var model = new TraceReportDataModel();
			DateTime startDateTime, endDateTime;
			if (DateTime.TryParse(startDate, out startDateTime) && DateTime.TryParse(endDate, out endDateTime)) {
				model = TraceLog.GetReport(startDateTime, endDateTime);
				model.startTime = startDateTime;
				model.endTime = endDateTime;
			}

			return model.toRaw();
		} catch (Exception exc) {
			if (string.IsNullOrEmpty(ReportingConfig.baseURI) == false)
				ExceptionLog.LogException(exc).Wait();

			throw exc;
		}
	}

	[WebMethod]
	[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
	public object GetSession(string sessionID) {
		try {
			var model = new TraceReportDataModel();
			DateTime startDateTime, endDateTime;
			startDateTime = DateTime.Now.AddDays(-7);
			endDateTime = DateTime.Now;

			var set = TraceLog.GetTraceLog(startDateTime, endDateTime);
			model.startTime = startDateTime;
			model.endTime = endDateTime;
			model.traceMap = new Dictionary<string, List<TraceDataModel>>();
			model.traceMap.Add(sessionID, new List<TraceDataModel>());
			foreach (var trace in set)
				if (trace.sessionid == sessionID)
					model.traceMap[sessionID].Add(trace);

			model.traceMap[sessionID].OrderBy((t) => t.timestamp).ToList();
			return model.toRaw();
		} catch (Exception exc) {
			if(string.IsNullOrEmpty(ReportingConfig.baseURI) == false)
				ExceptionLog.LogException(exc).Wait();

			throw exc;
		}
	}
}
