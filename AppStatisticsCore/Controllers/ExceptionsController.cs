using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppStatisticsCore.Models;

namespace AppStatisticsCore.Controllers {
	public class ExceptionsController : Controller {
		public IActionResult Details(string appid, string excid) {
			var app = Config.store.getApplication(appid);
			var exc = Config.store.getException(app, excid);
			return View(new ExceptionViewModel() {
				application = app,
				source = exc,
			});
		}
	}
}