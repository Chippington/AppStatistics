using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatistics.Common.Models.Reporting.Exceptions;
using AppStatistics.Common.Models.Reporting.Events;

namespace AppStatistics.Core.Controllers.API {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class EventsController : Controller {
		// GET: api/Exceptions
		[HttpGet]
		public IEnumerable<string> Get() {
			return null;
		}

		// GET: api/Exceptions/5
		[HttpGet(Name = "GetEvent")]
		public string Get([FromQuery]string appid, [FromQuery]string exceptionid) {
			return "";
		}

		// POST: api/Exceptions
		[HttpPost]
		public void Post([FromBody]dynamic data) {
			try {
				EventDataModel ev = new EventDataModel();
				ev.fromRaw(data);

				Config.store.AddEventData(ev.applicationID, ev);
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
		public void Delete([FromQuery]string eventid) {
			
		}
	}
}