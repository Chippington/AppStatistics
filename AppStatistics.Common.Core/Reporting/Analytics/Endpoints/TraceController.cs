using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Reporting.Analytics;
using Microsoft.AspNetCore.Cors;

namespace AppStatistics.Common.Core.Reporting.Analytics.Endpoints {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class TraceController : Controller {
		[HttpGet]
		public object GetActivity([FromQuery]string startTime, [FromQuery]string endTime) {
			TraceReportDataModel model = new TraceReportDataModel();
			DateTime startDateTime, endDateTime;
			if (DateTime.TryParse(startTime, out startDateTime) && DateTime.TryParse(endTime, out endDateTime))
				model = TraceLog.GetReport(startDateTime, endDateTime);

			return model.toRaw();
		}

		[HttpGet]
		public object GetSession([FromQuery]string sessionID) {
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
				if(trace.sessionid == sessionID)
					model.traceMap[sessionID].Add(trace);

			model.traceMap[sessionID].OrderBy((t) => t.timestamp).ToList();
			return model.toRaw();
		}
	}
}