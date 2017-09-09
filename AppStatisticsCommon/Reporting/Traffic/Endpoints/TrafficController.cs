using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Traffic;
using AppStatisticsCommon.Reporting.Exceptions;

namespace AppStatisticsCommon.Reporting.Traffic.Endpoints {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class TrafficController : Controller {
		[HttpGet]
		public object GetActivityByDate([FromQuery]int segments, [FromQuery]string dateTime) {
			DateTime date;
			TrafficReportDataModel model = new TrafficReportDataModel();
			if (DateTime.TryParse(dateTime, out date)) {

			}

			return model.toRaw();
		}

		[HttpGet]
		public object GetActivityByRange([FromQuery]int segments, [FromQuery]string startTime, [FromQuery]string endTime) {
			TrafficReportDataModel model = new TrafficReportDataModel();
			DateTime startDateTime, endDateTime;
			if (DateTime.TryParse(startTime, out startDateTime) && DateTime.TryParse(endTime, out endDateTime)) {

			}

			return model.toRaw();
		}

		[HttpGet]
		public object GetActivityRelative([FromQuery]int segments, [FromQuery]int minutes) {
			TrafficReportDataModel model = new TrafficReportDataModel();

			return model.toRaw();
		}
	}
}
