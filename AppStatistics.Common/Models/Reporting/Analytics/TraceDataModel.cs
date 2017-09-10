using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Analytics {
	public class TraceDataModel : ModelBase {
		public string path;
		public string method;
		public string ipaddress;
		public string sessionid;
		public DateTime timestamp;

		public TraceDataModel() {
			timestamp = DateTime.Now;
			path = method = ipaddress = sessionid = "";
		}

		public override dynamic toRaw() {
			return $"{path}|{method}|{ipaddress}|{sessionid}|{timestamp.ToString()}";
		}

		public override void fromRaw(dynamic data) {
			var str = (string)data;
			string[] split = str.Split('|');
			path = split[0];
			method = split[1];
			ipaddress = split[2];
			sessionid = split[3];
			timestamp = DateTime.Parse(split[4]);
		}
	}
}
