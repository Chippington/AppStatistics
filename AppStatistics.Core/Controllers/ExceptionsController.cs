using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Core.Models;

namespace AppStatistics.Core.Controllers {
	public class ExceptionsController : Controller {
		public IActionResult Details(string appid, string excid) {
			var model = new ExceptionViewModel();
			try {
				var app = Config.store.GetApplication(appid);
				var exc = Config.store.GetException(excid);

				string sessionID = "";
				foreach (var key in exc.metadata.Keys)
					if (key.ToLower() == "session id")
						sessionID = exc.metadata[key];

				model.application = app;
				model.source = exc;
				if (sessionID != "") {
					var sessionReport = Config.store.GetSessionReport(appid, sessionID);
					if (sessionReport != null && sessionReport.traceMap != null && sessionReport.traceMap.ContainsKey(sessionID))
						model.sessionActionURI = Url.Action("Details", "Sessions", new { appid = appid, sessionid = sessionID });
				}
			} catch (Exception excc) {
				Config.store.AddException("root", new Common.Models.Reporting.Exceptions.ExceptionDataModel(excc, "root"));
			}

			return View(model);
		}
	}
}