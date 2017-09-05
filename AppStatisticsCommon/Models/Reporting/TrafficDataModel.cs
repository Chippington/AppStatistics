using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Models.Reporting {
	public class TrafficDataModel : ModelBase {
		public Dictionary<string, int> pageHits { get; set; }
		public Dictionary<string, int> sessionHits { get; set; }
		public Dictionary<string, int> activity { get; set; }
		public ApplicationModel application { get; set; }

		public TrafficDataModel() { }
		internal TrafficDataModel(int segments, List<string> trafficData) {
			float sectionLength = (60f * 60f * 24f) / segments;
			activity = new Dictionary<string, int>();
			foreach(var str in trafficData) {
				string[] split = str.Split('|');
				string dateStr = split[0];
				string session = split[1];
				string page = split[2];

				DateTime date;
				if(DateTime.TryParse(dateStr, out date)) {
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

		public override dynamic toRaw() {
			return new {
				PageHits = Newtonsoft.Json.JsonConvert.SerializeObject(pageHits),
				SessionHits = Newtonsoft.Json.JsonConvert.SerializeObject(sessionHits),
				Activity = Newtonsoft.Json.JsonConvert.SerializeObject(activity),
				Application = application.toRaw(),
			};
		}

		public override void fromRaw(dynamic data) {
			pageHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			sessionHits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			activity = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>((string)data.PageHits);
			application = new ApplicationModel();
			application.fromRaw(data.Application);
		}
	}
}
