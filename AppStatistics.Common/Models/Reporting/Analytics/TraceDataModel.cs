using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Analytics {
	public class TraceDataModel : ModelBase {
		/// <summary>
		/// Path the user visited
		/// </summary>
		public string path;

		/// <summary>
		/// The URI query string, if any
		/// </summary>
		public string query;

		/// <summary>
		/// GET, POST, PUT, etc
		/// </summary>
		public string method;

		/// <summary>
		/// IP Address (and port) of the user
		/// </summary>
		public string ipaddress;

		/// <summary>
		/// Session ID of the user
		/// </summary>
		public string sessionid;

		/// <summary>
		/// Timestamp of the trace
		/// </summary>
		public DateTime timestamp;

		/// <summary>
		/// Instantiates with default values.
		/// </summary>
		public TraceDataModel() {
			timestamp = DateTime.Now;
			path = method = ipaddress = sessionid = query = "";
		}

		/// <summary>
		/// Converts this trace object into a string.
		/// </summary>
		/// <returns>A string representing this trace</returns>
		public override dynamic toRaw() {
			return $"{path}|{query}|{method}|{ipaddress}|{sessionid}|{timestamp.ToString()}";
		}

		/// <summary>
		/// Parses a string input
		/// </summary>
		/// <param name="data">A string from the 'toRaw' method</param>
		public override void fromRaw(dynamic data) {
			var str = (string)data;
			string[] split = str.Split('|');
			path = split[0];
			query = split[1];
			method = split[2];
			ipaddress = split[3];
			sessionid = split[4];
			timestamp = DateTime.Parse(split[5]);
		}
	}
}
