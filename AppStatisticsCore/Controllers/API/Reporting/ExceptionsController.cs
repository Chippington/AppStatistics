using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatisticsCommon.Models.Reporting.Exceptions;

namespace AppStatisticsCore.Controllers.API.Reporting {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class ExceptionsController : Controller {
		// GET: api/Exceptions
		[HttpGet]
		public IEnumerable<string> Get() {
			return null;
		}

		// GET: api/Exceptions/5
		[HttpGet(Name = "GetException")]
		public string Get([FromQuery]string appid, [FromQuery]string exceptionid) {
			return "";
		}

		// POST: api/Exceptions
		[HttpPost]
		public void Post([FromBody]dynamic data) {
			ExceptionModel exception = new ExceptionModel();
			exception.fromRaw(data);

			Config.store.addException(exception);
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete]
		public void Delete([FromQuery]string exceptionid) {
		}
	}
}