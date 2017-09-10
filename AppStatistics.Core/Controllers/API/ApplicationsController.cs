using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AppStatistics.Common.Models.Reporting;

namespace AppStatistics.Core.Controllers.API {
	[Produces("application/json")]
	[EnableCors("AllowCors"), Route("api/[controller]")]
	public class ApplicationsController : Controller {
		// GET: api/Applications
		[HttpGet]
		public IEnumerable<object> Get() {
			return (from app in Config.store.GetAllApplications()
					select app.toRaw());
		}

		// GET: api/Applications/5
		[HttpGet("{id}", Name = "GetApplication")]
		public object Get(string id) {
			return Config.store.GetApplication(id).toRaw();
		}

		// POST: api/Applications
		[HttpPost]
		public void Post([FromBody]dynamic data) {
			string name = data.name;
			var m = new ApplicationDataModel();
			m.applicationName = name;
			Config.store.AddApplication(m);
		}

		// PUT: api/Applications/5
		[HttpPut("{id}")]
		public void Put(string id, [FromBody]string name) {
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Delete(string id) {
			Config.store.DeleteApplication(id);
		}
	}
}