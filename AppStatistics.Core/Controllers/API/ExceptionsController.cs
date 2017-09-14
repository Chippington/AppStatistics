using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatistics.Common.Models.Reporting.Exceptions;

namespace AppStatistics.Core.Controllers.API {
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
			try {
				ExceptionDataModel exception = new ExceptionDataModel();
				exception.fromRaw(data);

				Config.store.AddException(exception.applicationID, exception);
			} catch(Exception exc) {
				Config.store.AddException("root", new ExceptionDataModel(exc) {
					timeStamp = DateTime.Now,
					metadata = new Dictionary<string, string>() {
						{ "Raw JSON", Newtonsoft.Json.JsonConvert.SerializeObject(data) }
					}
				});
			}
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete]
		public void Delete([FromQuery]string exceptionid) {
			try {
				Config.store.DeleteException(exceptionid);
			} catch (Exception exc) {
				Config.store.AddException("root", new ExceptionDataModel(exc) {
					timeStamp = DateTime.Now,
					metadata = new Dictionary<string, string>() {
						{ "Exception ID", exceptionid == null ? "Null" : exceptionid }
					}
				});
			}
		}
	}
}