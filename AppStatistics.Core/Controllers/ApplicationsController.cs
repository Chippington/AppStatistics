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
			var applications = Config.store.getApplications();
			foreach (var app in applications) {
				model.Add(new ApplicationViewModel() {
					source = app,
					latestExceptions = Config.store.getExceptions(app).OrderBy(e => e.timeStamp).Reverse().Take(5).ToList()
				});
			}

			return View(model);
		}

		public IActionResult Details(string appid) {
			var app = Config.store.getApplication(appid);
			if (app == null)
				return RedirectToAction("Index");

			var model = new ApplicationViewModel();
			model.source = app;
			model.latestExceptions = Config.store.getExceptions(app).OrderBy(e => e.timeStamp).Reverse().Take(25).ToList();

			if (model.source.description == null)
				model.source.description = "";

			return View(model);
		}

		public IActionResult UpdateApplication(string appid, string appname, string appdesc, string appguid) {
			var app = Config.store.getApplication(appid);
			app.applicationName = appname;
			app.description = appdesc;
			app.guid = appguid;
			Config.store.updateApplication(app);

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

			Config.store.addApplication(newApp);
			return RedirectToAction("Index");
		}
	}
}