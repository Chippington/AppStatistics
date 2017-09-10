using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Core.Models;

namespace AppStatistics.Core.Controllers {
	public class ExceptionsController : Controller {
		public IActionResult Details(string appid, string excid) {
			var app = Config.store.GetApplication(appid);
			var exc = Config.store.GetException(excid);

			string sessionID = "";
			foreach (var key in exc.metadata.Keys)
				if (key.ToLower() == "session id")
					sessionID = exc.metadata[key];

			var model = new ExceptionViewModel();
			model.application = app;
			model.source = exc;
			if(sessionID != "") {
				var sessionReport = Config.store.GetSessionReport(appid, sessionID);
				if(sessionReport.traceMap.ContainsKey(sessionID))
					model.sessionActionURI = Url.Action("Details", "Session", new { appid = appid, sessionid = sessionID });
			}

			return View(model);
		}
	}
}