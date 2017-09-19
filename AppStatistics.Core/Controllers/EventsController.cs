using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatistics.Common.Models.Reporting.Events;
using AppStatistics.Core.Models;

namespace AppStatistics.Core.Controllers {
	public class EventsController : Controller {
		public IActionResult Index() {
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Details(string appid) {
			EventLogViewModel model = new EventLogViewModel();
			if(appid != null) {
				model.application = Config.store.GetApplication(appid);

				var list = Config.store.GetEventsByApplication(appid);
				if (list != null)
					model.events = list.ToList();
			}

			return View(model);
		}
	}
}