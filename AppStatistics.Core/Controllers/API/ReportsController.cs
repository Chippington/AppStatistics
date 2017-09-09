using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace AppStatistics.Core.Controllers.API {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class ReportsController : Controller {
		// GET: api/Reports
		[HttpGet]
		public IEnumerable<string> Get() {
			return new string[] { "value1", "value2" };
		}

		// GET: api/Reports/5
		[HttpGet("{id}", Name = "GetReport")]
		public string Get(int id) {
			return "value";
		}

		// POST: api/Reports
		[HttpPost]
		public void Post([FromBody]dynamic data) {
			string appID = data.applicationID;
		}

		// PUT: api/Reports/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value) {
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Delete(int id) {
		}
	}
}