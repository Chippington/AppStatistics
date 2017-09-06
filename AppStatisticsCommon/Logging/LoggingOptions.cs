using AppStatisticsCommon.Models.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Logging {
	public enum TraceType {
		NONE, DIRECT, INDIRECT
	}

	public class LogOptions {
		public string applicationID;
		public TimeSpan traceBatchCutoff;
		public TraceType traceType;
		public int traceBatchCount;
		public string baseURI;
		public string handlerPath;
	}

	public class LogOptionsBuilder {
		public LogOptions options;
		public LogOptionsBuilder() {
			options = new LogOptions();
			options.traceType = TraceType.NONE;
		}

		public void useDirectTracing(string appid, string apiBaseURI) {
			options.applicationID = appid;
			options.baseURI = apiBaseURI;
			options.traceType = TraceType.DIRECT;
		}

		public void useDirectTracing(string appid, string apiBaseURI, int batchCount) {
			useDirectTracing(appid, apiBaseURI);
			options.traceBatchCount = batchCount;
		}

		public void useDirectTracing(string appid, string apiBaseURI, int batchCount, TimeSpan cutoff) {
			useDirectTracing(appid, apiBaseURI, batchCount);
			options.traceBatchCutoff = cutoff;
		}

		public void useIndirectTracing(string appid, string apiBaseURI) {
			options.applicationID = appid;
			options.baseURI = apiBaseURI;
			options.traceType = TraceType.INDIRECT;
		}

		public void useCustomErrorHandlingPath(string path) {
			options.handlerPath = path;
		}
	}
}
