using AppStatistics.Common.Reporting.Analytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Analytics {
	public class TrafficReportDataModel : ModelBase {
		/// <summary>
		/// A dictionary mapping paths to their respective hit count.
		/// </summary>
		public Dictionary<string, int> pageHits { get; set; }

		/// <summary>
		/// A dictionary mapping sessions to their respective hit count.
		/// </summary>
		public Dictionary<string, int> sessionHits { get; set; }

		/// <summary>
		/// A dictionary mapping times of the day to their respective hit count.
		/// </summary>
		public Dictionary<string, int> activity { get; set; }

		/// <summary>
		/// Application ID that this report represents.
		/// </summary>
		public string applicationID { get; set; }

		/// <summary>
		/// Instantiates with default values.
		/// </summary>
		public TrafficReportDataModel() {
			applicationID = "";
			activity = new Dictionary<string, int>();
			pageHits = new Dictionary<string, int>();
			sessionHits = new Dictionary<string, int>();
		}

		/// <summary>
		/// Builds a traffic report using the given TraceSet.
		/// </summary>
		/// <param name="segments">Number of "time segments" to create.</param>
		/// <param name="traceLog">The trace set to use in creating the report.</param>
		public TrafficReportDataModel(int segments, TraceSet<TraceDataModel> traceLog) {
			applicationID = "";
			activity = new Dictionary<string, int>();
			pageHits = new Dictionary<string, int>();
			sessionHits = new Dictionary<string, int>();

			float sectionLength = (float)(traceLog.endTime - traceLog.startTime).TotalSeconds / segments;
			foreach(var trace in traceLog) {
				recordEntry(trace, sectionLength);
			}
		}

		/// <summary>
		/// Converts this instance to a raw object for use in JSON serialization.
		/// </summary>
		/// <returns></returns>
		public override dynamic toRaw() {
			return new {
				PageHits = Newtonsoft.Json.JsonConvert.SerializeObject(pageHits),
				SessionHits = Newtonsoft.Json.JsonConvert.SerializeObject(sessionHits),
				Activity = Newtonsoft.Json.JsonConvert.SerializeObject(activity),
				ApplicationID = applicationID,
			};
		}

		/// <summary>
		/// Parses an anonymous object's data into this instance.
		/// </summary>
		/// <param name="data"></param>
		public override void fromRaw(dynamic data) {
			pageHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			sessionHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			activity = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			applicationID = (string)data.ApplicationID;
		}

		/// <summary>
		/// Records an entry into the report.
		/// </summary>
		/// <param name="trace"></param>
		/// <param name="sectionLength"></param>
		private void recordEntry(TraceDataModel trace, float sectionLength) {
			string dateStr = trace.timestamp.ToString();
			string session = trace.sessionid;
			string page = trace.path;

			DateTime date;
			if (DateTime.TryParse(dateStr, out date)) {
				TimeSpan diff = (new DateTime(date.Year, date.Month, date.Day) - date);
				int section = (int)(diff.TotalSeconds / sectionLength);

				DateTime sectionDate = new DateTime(date.Year, date.Month, date.Day).AddSeconds(section * sectionLength);
				string sectionDateStr = sectionDate.ToString("hhmmss");
				if (activity.ContainsKey(sectionDateStr) == false)
					activity[sectionDateStr] = 0;
				activity[sectionDateStr] += 1;
			}

			if (pageHits.ContainsKey(page) == false)
				pageHits[page] = 0;
			pageHits[page] += 1;

			if (sessionHits.ContainsKey(session) == false)
				sessionHits[session] = 0;
			sessionHits[session] += 1;
		}
	}
}
