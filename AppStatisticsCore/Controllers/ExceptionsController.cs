using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppStatisticsCore.Controllers
{
    public class ExceptionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}