using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AppStatistics.Common.Models.Reporting {
	public class ApplicationDataModel : ModelBase {
		/// <summary>
		/// Analytics endpoint of the application.
		/// </summary>
		public string analyticsEndpoint;

		/// <summary>
		/// Name of this application.
		/// </summary>
		public string applicationName;

		/// <summary>
		/// Description of this application.
		/// </summary>
		public string description;

		//Creation date of this application.
		public DateTime creationDate;

		/// <summary>
		/// Instantiates with default values.
		/// </summary>
		public ApplicationDataModel() {
			analyticsEndpoint = applicationName = description = "";
			creationDate = DateTime.Now;
		}

		/// <summary>
		/// Converts this instance into a raw object.
		/// </summary>
		/// <returns></returns>
		public override object toRaw() {
			return new {
				Name = applicationName,
				Description = description,
				CreationDate = creationDate,
				AnalyticsEndpoint = analyticsEndpoint,
				GUID = guid,
			};
		}

		/// <summary>
		/// Parses a raw object into this instance.
		/// </summary>
		/// <param name="data"></param>
		public override void fromRaw(dynamic data) {
			applicationName = data.Name;
			description = data.Description;
			creationDate = data.CreationDate;
			analyticsEndpoint = data.AnalyticsEndpoint;
			guid = data.GUID;
		}
	}
}