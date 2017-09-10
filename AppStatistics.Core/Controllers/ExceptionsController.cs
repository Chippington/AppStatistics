﻿using System;
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
			return View(new ExceptionViewModel() {
				application = app,
				source = exc,
			});
		}
	}
}