using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Analytics {
	public class TraceReportDataModel : ModelBase{
		public Dictionary<string, List<TraceDataModel>> traceMap;
		public DateTime startTime;
		public DateTime endTime;

		public TraceReportDataModel() {
			traceMap = new Dictionary<string, List<TraceDataModel>>();
		}

		public TraceReportDataModel(IEnumerable<TraceDataModel> source) {
			traceMap = new Dictionary<string, List<TraceDataModel>>();
			foreach (var trace in source) {
				if (traceMap.ContainsKey(trace.sessionid) == false)
					traceMap.Add(trace.sessionid, new List<TraceDataModel>());

				traceMap[trace.sessionid].Add(trace);
			}

			var keyList = traceMap.Keys.ToList();
			foreach (var key in keyList)
				traceMap[key] = traceMap[key].OrderBy((trace) => trace.timestamp).ToList();
		}

		public override dynamic toRaw() {
			return new {
				TraceMap = Newtonsoft.Json.JsonConvert.SerializeObject(traceMap),
			};
		}

		public override void fromRaw(dynamic data) {
			traceMap = Newtonsoft.Json.JsonConvert.DeserializeObject
				<Dictionary<string, List<TraceDataModel>>>((string)data.TraceMap);
		}
	}
}
