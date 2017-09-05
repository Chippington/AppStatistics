using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AppStatisticsCommon.Models.Reporting {
	public class ApplicationDataModel : ModelBase {
		public string applicationName;
		public string description;
		public DateTime creationDate;

		public ApplicationDataModel() {
		}

		public override object toRaw() {
			return new {
				Name = applicationName,
				Description = description,
				CreationDate = creationDate,
				GUID = guid,
			};
		}

		public override void fromRaw(dynamic data) {
			applicationName = data.Name;
			description = data.Description;
			creationDate = data.CreationDate;
			guid = data.GUID;
		}
	}
}