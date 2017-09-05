using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatisticsCommon.Models.Reporting;

namespace AppStatisticsCommon.Logging.Controllers {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class TrafficController : Controller {
		// GET: api/Applications/5
		[HttpGet(Name = "GetTrafficData")]
		public object Get([FromQuery]int segments, [FromQuery]string dateTime) {
			DateTime date;
			TrafficDataModel model = new TrafficDataModel();
			if (DateTime.TryParse(dateTime, out date)) {
				var data = Log.getTrafficData(date);
				model = new TrafficDataModel(segments, data);
			}

			return model.toRaw();
		}

		public object GetActivity([FromQuery]int segments, [FromQuery]string dateTime) {
			DateTime date;
			TrafficDataModel model = new TrafficDataModel();
			if (DateTime.TryParse(dateTime, out date)) {
				var data = Log.getTrafficData(date);
				model = new TrafficDataModel(segments, data);
			}

			model.pageHits = null;
			model.sessionHits = null;
			return model.toRaw();
		}
	}
}
