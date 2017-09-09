using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Models.Reporting.Traffic {
	public class TraceDataModel : ModelBase {
		public string path;
		public string method;
		public string ipaddress;
		public string sessionid;
		public string timestamp;

		public TraceDataModel() {
			path = method = ipaddress = sessionid = timestamp = "";
		}

		public override dynamic toRaw() {
			return $"{path}|{method}|{ipaddress}|{sessionid}|{timestamp}";
		}

		public override void fromRaw(dynamic data) {
			var str = (string)data;
			string[] split = str.Split('|');
			path = split[0];
			method = split[1];
			ipaddress = split[2];
			sessionid = split[3];
			timestamp = split[4];
		}
	}
}
