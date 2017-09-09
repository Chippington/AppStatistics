using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Core.Reporting.Exceptions;
using AppStatistics.Common.Reporting.Analytics;

namespace AppStatistics.Common.Core.Reporting.Analytics.Endpoints {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class TrafficController : Controller {
		[HttpGet]
		public object GetActivityByDate([FromQuery]int segments, [FromQuery]string dateTime) {
			TrafficReportDataModel model = new TrafficReportDataModel();
			DateTime date;
			if(DateTime.TryParse(dateTime, out date))
				return TrafficLog.GetReport(segments, date);

			return model.toRaw();
		}

		[HttpGet]
		public object GetActivityByRange([FromQuery]int segments, [FromQuery]string startTime, [FromQuery]string endTime) {
			TrafficReportDataModel model = new TrafficReportDataModel();
			DateTime startDateTime, endDateTime;
			if (DateTime.TryParse(startTime, out startDateTime) && DateTime.TryParse(endTime, out endDateTime))
				return TrafficLog.GetReport(segments, startDateTime, endDateTime);

			return model.toRaw();
		}

		[HttpGet]
		public object GetActivityRelative([FromQuery]int segments, [FromQuery]int minutes) {
			TrafficReportDataModel model = new TrafficReportDataModel();
			DateTime startDateTime = DateTime.Now.AddMinutes(-1 * minutes);
			DateTime endDateTime = DateTime.Now;

			return TrafficLog.GetReport(segments, startDateTime, endDateTime);
		}
	}
}
