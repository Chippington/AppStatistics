using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Analytics {
	public class TraceReportDataModel : ModelBase{
		/// <summary>
		/// A dictionary containing a map of session ID (string) to all traces of that session.
		/// </summary>
		public Dictionary<string, List<TraceDataModel>> traceMap;

		/// <summary>
		/// The start time of this trace report.
		/// </summary>
		public DateTime startTime;

		/// <summary>
		/// The end time of this trace report.
		/// </summary>
		public DateTime endTime;

		/// <summary>
		/// Instantiates with default values.
		/// </summary>
		public TraceReportDataModel() {
			traceMap = new Dictionary<string, List<TraceDataModel>>();
		}

		/// <summary>
		/// Creates a trace report from the given set of TraceDataModels.
		/// </summary>
		/// <param name="source"></param>
		public TraceReportDataModel(IEnumerable<TraceDataModel> source) {
			traceMap = new Dictionary<string, List<TraceDataModel>>();
			build(source);
		}

		/// <summary>
		/// Builds the trace report from the given source set.
		/// </summary>
		/// <param name="source"></param>
		private void build(IEnumerable<TraceDataModel> source) {
			//Sort each trace into seperate lists based on session ID
			foreach (var trace in source) {
				if (traceMap.ContainsKey(trace.sessionid) == false)
					traceMap.Add(trace.sessionid, new List<TraceDataModel>());

				traceMap[trace.sessionid].Add(trace);
			}

			//Sort each list by timestamp, ascending
			var keyList = traceMap.Keys.ToList();
			foreach (var key in keyList)
				traceMap[key] = traceMap[key].OrderBy((trace) => trace.timestamp).ToList();
		}

		/// <summary>
		/// Converts this report instance into a raw object for use with JSON conversion.
		/// </summary>
		/// <returns></returns>
		public override dynamic toRaw() {
			List<string> tmp = new List<string>();
			foreach (var session in traceMap.Values)
				foreach (var trace in session)
					tmp.Add((string)trace.toRaw());

			return new {
				TraceMap = tmp,
				StartTime = startTime,
				EndTime = endTime,
			};
		}

		/// <summary>
		/// Parses a raw object's data into this instance
		/// </summary>
		/// <param name="data"></param>
		public override void fromRaw(dynamic data) {
			var tmp = ((JArray)data.TraceMap).ToObject<List<string>>();
			List<TraceDataModel> traces = new List<TraceDataModel>();
			foreach(var str in tmp) {
				var m = new TraceDataModel();
				m.fromRaw(str);
				traces.Add(m);
			}

			startTime = (DateTime)data.StartTime;
			endTime = (DateTime)data.EndTime;
			build(traces);
		}
	}
}
