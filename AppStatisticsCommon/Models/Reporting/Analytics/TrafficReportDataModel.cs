using AppStatisticsCommon.Reporting.Analytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Models.Reporting.Analytics {
	public class TrafficReportDataModel : ModelBase {
		public Dictionary<string, int> pageHits { get; set; }
		public Dictionary<string, int> sessionHits { get; set; }
		public Dictionary<string, int> activity { get; set; }
		public string applicationID { get; set; }

		public TrafficReportDataModel() { }

		public TrafficReportDataModel(int segments, TraceSet<TraceDataModel> traceLog) {
			float sectionLength = (float)(traceLog.endTime - traceLog.startTime).TotalSeconds / segments;
			foreach(var trace in traceLog) {
				recordEntry(trace, sectionLength);
			}
		}

		public override dynamic toRaw() {
			return new {
				PageHits = Newtonsoft.Json.JsonConvert.SerializeObject(pageHits),
				SessionHits = Newtonsoft.Json.JsonConvert.SerializeObject(sessionHits),
				Activity = Newtonsoft.Json.JsonConvert.SerializeObject(activity),
				ApplicationID = applicationID,
			};
		}

		public override void fromRaw(dynamic data) {
			pageHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			sessionHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			activity = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			applicationID = (string)data.ApplicationID;
		}

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
