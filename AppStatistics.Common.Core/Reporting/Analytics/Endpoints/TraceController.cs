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
		public object GetActivityByRange([FromQuery]string startTime, [FromQuery]string endTime) {
			TraceReportDataModel model = new TraceReportDataModel();
			DateTime startDateTime, endDateTime;
			if (DateTime.TryParse(startTime, out startDateTime) && DateTime.TryParse(endTime, out endDateTime))
				model = TraceLog.GetReport(startDateTime, endDateTime);

			return model.toRaw();
		}
	}
}