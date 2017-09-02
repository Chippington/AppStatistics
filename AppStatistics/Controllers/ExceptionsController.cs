using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppStatistics.Controllers {
	[Produces("application/json")]
	[Route("api/Exceptions")]
	public class ExceptionsController : Controller {
		// GET: api/Exceptions
		[HttpGet]
		public IEnumerable<string> Get() {
			return new string[] { "value1", "value2" };
		}

		// GET: api/Exceptions/5
		[HttpGet("{id}", Name = "GetException")]
		public string Get(int id) {
			return "value";
		}

		// POST: api/Exceptions
		[HttpPost]
		public void Post([FromBody]string value) {
		}

		// PUT: api/Exceptions/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value) {
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Delete(int id) {
		}
	}
}