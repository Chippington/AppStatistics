using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Reporting;
using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AnalyticsEndpoint : System.Web.UI.Page {
	/// <summary>
	/// Sorts to the appropriate function based on the operation in the query
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e) {
		var op = Request.QueryString["op"];
		if(op.ToLower() == "getsession") {
			Response.Write(GetSession(Request.QueryString["sessionID"]));
		}

		if (op.ToLower() == "getactivity") {
			Response.Write(GetActivity(
				Request.QueryString["startDate"],
				Request.QueryString["endDate"]));
		}
	}

	/// <summary>
	/// Returns a trace report containing the given session's trace data.
	/// </summary>
	/// <param name="sessionID"></param>
	/// <returns></returns>
	protected string GetSession(string sessionID) {
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
			var raw = model.toRaw();
			return Newtonsoft.Json.JsonConvert.SerializeObject(raw);
		} catch (Exception exc) {
			if (string.IsNullOrEmpty(ReportingConfig.baseURI) == false)
				ExceptionLog.LogException(exc);

			return "";
		}
	}

	/// <summary>
	/// Returns a trace report containing trace data for all sessions in the given timeframe.
	/// </summary>
	/// <param name="startDate"></param>
	/// <param name="endDate"></param>
	/// <returns></returns>
	protected string GetActivity(string startDate, string endDate) {
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
				ExceptionLog.LogException(exc);

			return "";
		}
	}
}
