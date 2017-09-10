using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Core.Models;
using AppStatistics.Common.Models.Reporting;

namespace AppStatistics.Core.Controllers {
	public class ApplicationsController : Controller {
		public IActionResult Index() {
			List<ApplicationViewModel> model = new List<ApplicationViewModel>();
			var applications = Config.store.GetAllApplications();
			foreach (var app in applications) {
				model.Add(new ApplicationViewModel() {
					source = app,
					latestExceptions = Config.store.GetExceptionsByApplication(
						app.guid, DateTime.Now.AddDays(-7), DateTime.Now).OrderBy(e => e.timeStamp).Reverse().Take(5).ToList()
				});
			}

			return View(model);
		}

		public IActionResult Details(string appid) {
			var app = Config.store.GetApplication(appid);
			if (app == null)
				return RedirectToAction("Index");

			var model = new ApplicationViewModel();
			model.source = app;
			model.latestExceptions = Config.store.GetExceptionsByApplication(
						app.guid, DateTime.Now.AddDays(-7), DateTime.Now).OrderBy(e => e.timeStamp).Reverse().Take(25).ToList();

			if (model.source.description == null)
				model.source.description = "";

			return View(model);
		}

		public IActionResult UpdateApplication(string appid, string appname, string appdesc, string appguid, string analyticsendpoint) {
			var app = Config.store.GetApplication(appid);
			app.analyticsEndpoint = analyticsendpoint;
			app.applicationName = appname;
			app.description = appdesc;
			app.guid = appguid;
			Config.store.UpdateApplication(appid, app);
			return RedirectToAction("Details", new { appid = appguid });
		}

		[HttpPost]
		public IActionResult CreateApplication(string name, string guid, string desc) {
			var newApp = new ApplicationDataModel();
			newApp.applicationName = name;
			newApp.description = desc;
			newApp.creationDate = DateTime.Now;
			if (guid != null && guid.Trim() != string.Empty)
				newApp.guid = guid;

			Config.store.AddApplication(newApp);
			return RedirectToAction("Index");
		}
	}
}