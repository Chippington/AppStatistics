using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AppStatistics.Common.Models.Reporting {
	public class ApplicationDataModel : ModelBase {
		public string applicationName;
		public string description;
		public string analyticsEndpoint;
		public DateTime creationDate;

		public ApplicationDataModel() {
		}

		public override object toRaw() {
			return new {
				Name = applicationName,
				Description = description,
				CreationDate = creationDate,
				AnalyticsEndpoint = analyticsEndpoint,
				GUID = guid,
			};
		}

		public override void fromRaw(dynamic data) {
			applicationName = data.Name;
			description = data.Description;
			creationDate = data.CreationDate;
			analyticsEndpoint = data.AnalyticsEndpoint;
			guid = data.GUID;
		}
	}
}