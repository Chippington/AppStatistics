using AppStatistics.Common.Models.Reporting.Analytics;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace AppStatistics.Common.Reporting.Analytics {
	public static class TrafficLog {
		public static TrafficReportDataModel GetReport(int segments, DateTime dateTime) {
			var set = TraceLog.GetTraceLog(dateTime);
			var ret = new TrafficReportDataModel(segments, set);
			return ret;
		}

		public static TrafficReportDataModel GetReport(int segments, DateTime startTime, DateTime endTime) {
			var set = TraceLog.GetTraceLog(startTime, endTime);
			var ret = new TrafficReportDataModel(segments, set);
			return ret;
		}
	}
}
