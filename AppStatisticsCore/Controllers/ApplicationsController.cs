using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatisticsCore.Models;
using AppStatisticsCommon.Models.Reporting;

namespace AppStatisticsCore.Controllers {
	public class ApplicationsController : Controller {
		public IActionResult Index() {
			List<ApplicationViewModel> model = new List<ApplicationViewModel>();
			var applications = Config.store.getApplications();
			foreach (var app in applications) {
				model.Add(new ApplicationViewModel() {
					source = app,
					latestExceptions = Config.store.getExceptions(app).OrderBy(e => e.timeStamp).Take(5).ToList()
				});
			}

			return View(model);
		}

		public IActionResult Details(string appid) {
			var app = Config.store.getApplication(appid);
			var model = new ApplicationViewModel();
			model.source = app;
			model.latestExceptions = Config.store.getExceptions(app).OrderBy(e => e.timeStamp).Take(25).ToList();
			
			return View(model);
		}

		public IActionResult UpdateApplication(string appid, string appname, string appdesc) {
			var app = Config.store.getApplication(appid);
			app.applicationName = appname;
			app.description = appdesc;
			Config.store.updateApplication(app);

			return RedirectToAction("Details", new { appid });
		}

		[HttpPost]
		public IActionResult CreateApplication(string name, string guid, string desc) {
			var newApp = new ApplicationModel(name);
			newApp.description = desc;
			newApp.creationDate = DateTime.Now;
			if (guid != null && guid.Trim() != string.Empty)
				newApp.guid = guid;

			Config.store.addApplication(newApp);
			return RedirectToAction("Index");
		}
	}
}