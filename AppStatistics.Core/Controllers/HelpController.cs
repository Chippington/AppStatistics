﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppStatistics.Core.Controllers {
	public class HelpController : Controller {
		public IActionResult Index() {
			return View();
		}

		public IActionResult WebForms() {
			return View();
		}
	}
}
