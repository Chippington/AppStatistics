using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AppStatisticsCommon.Models.Reporting {
	public class ApplicationModel : ModelBase {
		public string applicationName;

		public ApplicationModel(string name) {
			applicationName = name;
		}

		public override object toRaw() {
			return new {
				Name = applicationName,
				GUID = guid,
			};
		}

		public override void fromRaw(dynamic data) {
			applicationName = data.Name;
			guid = data.GUID;
		}
	}
}