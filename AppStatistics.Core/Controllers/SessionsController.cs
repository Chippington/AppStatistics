using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Common.Models.Reporting.Analytics;

namespace AppStatistics.Core.Controllers
{
    public class SessionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Details([FromQuery]string appid, [FromQuery]string sessionid) {
			TraceReportDataModel m = new TraceReportDataModel();
			if (string.IsNullOrEmpty(appid) == false && string.IsNullOrEmpty(sessionid) == false) {
				var sessionReport = Config.store.GetSessionReport(appid, sessionid);
				if (sessionReport != null && sessionReport.traceMap != null && sessionReport.traceMap.ContainsKey(sessionid)) {
					m = sessionReport;
				}
			}

			return View(m);
		}
    }
}