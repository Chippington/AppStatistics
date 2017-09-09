﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatisticsCore.Models;
using AppStatisticsCommon.Models.Reporting.Analytics;

namespace AppStatisticsCore.Controllers {
	public class HomeController : Controller {
		public IActionResult Index() {
			List<ApplicationViewModel> model = new List<ApplicationViewModel>();
			var applications = Config.store.getApplications();
			foreach (var app in applications) {
				model.Add(new ApplicationViewModel() {
					source = app,
					latestExceptions = Config.store.getExceptions(app).OrderBy(e => e.timeStamp).Reverse().Take(5).ToList(),
					traffic = new TrafficReportDataModel() {
						activity = new Dictionary<string, int>() {
							{ "000", 1 },
							{ "001", 2 },
							{ "002", 3 },
							{ "003", 4 },
							{ "004", 4 },
							{ "005", 4 },
							{ "006", 3 },
							{ "007", 2 },
							{ "008", 1 },
						},
					}
				});
			}

			return View(model);
		}

		public IActionResult About() {
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact() {
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}