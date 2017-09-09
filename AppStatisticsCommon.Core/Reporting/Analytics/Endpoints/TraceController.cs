using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppStatisticsCommon.Core.Reporting.Analytics.Endpoints {
	public class TraceController : Controller {
		// GET: api/Applications/5
		[HttpGet(Name = "GetTraceData")]
		public object Get([FromQuery]int segments, [FromQuery]string dateTime) {
			return null;	
		}

		public object GetActivity([FromQuery]int segments, [FromQuery]string dateTime) {
			return null;
		}
	}
}