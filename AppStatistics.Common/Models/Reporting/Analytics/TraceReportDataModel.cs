using Newtonsoft.Json.Linq;
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
			build(source);
		}

		private void build(IEnumerable<TraceDataModel> source) {
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
			List<string> tmp = new List<string>();
			foreach (var session in traceMap.Values)
				foreach (var trace in session)
					tmp.Add((string)trace.toRaw());

			return new {
				TraceMap = tmp,
			};
		}

		public override void fromRaw(dynamic data) {
			var tmp = ((JArray)data.TraceMap).ToObject<List<string>>();
			List<TraceDataModel> traces = new List<TraceDataModel>();
			foreach(var str in tmp) {
				var m = new TraceDataModel();
				m.fromRaw(str);
				traces.Add(m);
			}

			build(traces);
		}
	}
}
