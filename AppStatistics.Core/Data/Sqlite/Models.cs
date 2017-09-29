using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppStatistics.Common.Models.Reporting.Exceptions;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Events;
using AppStatistics.Common.Models.Reporting.Analytics;
using System.Text;

namespace AppStatistics.Core.Data.Sqlite
{
	public static class DbUtils {
		private static string keySep = "•";
		private static string pairSep = "│";

		public static string flatten(Dictionary<string, string> map) {
			StringBuilder sb = new StringBuilder();
			foreach (var x in map) {
				sb.Append(x.Key);
				sb.Append(keySep);
				sb.Append(x.Value);
				sb.Append(pairSep);
			}

			return sb.ToString(0, sb.Length - 1);
		}

		public static Dictionary<string, string> expand(string map) {
			Dictionary<string, string> ret = new Dictionary<string, string>();
			string[] pairs = map.Split(pairSep);
			foreach(var pair in pairs) {
				string[] split = pair.Split(keySep);
				ret.Add(split[0], split[1]);
			}

			return ret;
		}
	}

	public class ReportingContext : DbContext {
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlite("Data Source=reporting.db");
		}

		public DbSet<SqlEvent> Events { get; set; }
		public DbSet<SqlException> Exceptions { get; set; }
		public DbSet<SqlApplication> Applications { get; set; }
	}

	public class SqlException {
		public string id { get; set; }
		public bool active { get; set; }
		public string stackTrace { get; set; }
		public string message { get; set; }
		public int severity { get; set; }
		public int hresult { get; set; }
		public string metadata { get; set; }
		public string applicationID { get; set; }
		public DateTime timeStamp { get; set; }
		public List<SqlException> innerExceptions { get; set; }

		public static ExceptionDataModel to(SqlException src) {
			ExceptionDataModel ret = new ExceptionDataModel();
			ret.guid = src.id;
			ret.message = src.message;
			ret.hresult = src.hresult;
			ret.severity = src.severity;
			ret.timeStamp = src.timeStamp;
			ret.stackTrace = src.stackTrace;
			ret.applicationID = src.applicationID;
			ret.metadata = DbUtils.expand(src.metadata);
			ret.innerExceptions = new List<ExceptionDataModel>();
			foreach (var inner in src.innerExceptions)
				ret.innerExceptions.Add(to(inner));

			return ret;
		}

		public static SqlException from(ExceptionDataModel src) {
			SqlException ret = new SqlException();
			ret.id = src.guid;
			ret.hresult = src.hresult;
			ret.message = src.message;
			ret.severity = src.severity;
			ret.timeStamp = src.timeStamp;
			ret.stackTrace = src.stackTrace;
			ret.applicationID = src.applicationID;
			ret.metadata = DbUtils.flatten(src.metadata);
			ret.innerExceptions = new List<SqlException>();
			foreach (var inner in src.innerExceptions)
				ret.innerExceptions.Add(from(inner));

			ret.active = true;
			return ret;
		}
	}

	public class SqlApplication {
		public string id { get; set; }
		public bool active { get; set; }
		public string analyticsEndpoint { get; set; }
		public string applicationName { get; set; }
		public string description { get; set; }
		public DateTime creationDate { get; set; }

		public static ApplicationDataModel to(SqlApplication src) {
			ApplicationDataModel ret = new ApplicationDataModel();
			ret.analyticsEndpoint = src.analyticsEndpoint;
			ret.applicationName = src.applicationName;
			ret.description = src.description;
			ret.creationDate = src.creationDate;
			ret.guid = src.id;
			return ret;
		}

		public static SqlApplication from(ApplicationDataModel src) {
			SqlApplication ret = new SqlApplication();
			ret.analyticsEndpoint = src.analyticsEndpoint;
			ret.applicationName = src.applicationName;
			ret.description = src.description;
			ret.creationDate = src.creationDate;
			ret.id = src.guid;
			ret.active = true;
			return ret;
		}
	}

	public class SqlEvent {
		public string id { get; set; }
		public bool active { get; set; }
		public string metadata { get; set; }
		public DateTime timestamp { get; set; }
		public string applicationID { get; set; }
		public string category { get; set; }
		public string message { get; set; }

		public static EventDataModel to(SqlEvent src) {
			EventDataModel ret = new EventDataModel();
			ret.metadata = DbUtils.expand(src.metadata);
			ret.applicationID = src.applicationID;
			ret.timestamp = src.timestamp;
			ret.category = src.category;
			ret.message = src.message;
			ret.guid = src.id;
			return ret;
		}

		public static SqlEvent from(EventDataModel src) {
			SqlEvent ret = new SqlEvent();
			ret.metadata = DbUtils.flatten(src.metadata);
			ret.applicationID = src.applicationID;
			ret.timestamp = src.timestamp;
			ret.category = src.category;
			ret.message = src.message;
			ret.id = src.guid;
			ret.active = true;
			return ret;
		}
	}

	public class SqlTrace {
		public string id { get; set; }
		public string path { get; set; }
		public string query { get; set; }
		public string method { get; set; }
		public string ipaddress { get; set; }
		public string sessionid { get; set; }
		public DateTime timestamp { get; set; }

		public static TraceDataModel to(SqlTrace src) {
			TraceDataModel ret = new TraceDataModel();
			ret.guid = src.id;
			ret.path = src.path;
			ret.query = src.query;
			ret.method = src.method;
			ret.ipaddress = src.ipaddress;
			ret.sessionid = src.sessionid;
			ret.timestamp = src.timestamp;
			return ret;
		}

		public static SqlTrace from(TraceDataModel src) {
			SqlTrace ret = new SqlTrace();
			ret.id = src.guid;
			ret.path = src.path;
			ret.query = src.query;
			ret.method = src.method;
			ret.ipaddress = src.ipaddress;
			ret.sessionid = src.sessionid;
			ret.timestamp = src.timestamp;
			return ret;
		}
	}

	public class SqlTraceReport {
		public string id { get; set; }
		public Dictionary<string, List<SqlTrace>> traceMap { get; set; }
		public DateTime startTime { get; set; }
		public DateTime endTime { get; set; }

		public static TraceReportDataModel from(SqlTraceReport src) {
			TraceReportDataModel ret = new TraceReportDataModel();
			ret.guid = src.id;
			ret.endTime = src.endTime;
			ret.startTime = src.startTime;
			ret.traceMap = new Dictionary<string, List<TraceDataModel>>();
			foreach (var key in src.traceMap.Keys) {
				var traces = src.traceMap[key];
				List<TraceDataModel> list = new List<TraceDataModel>();
				foreach (var trace in traces)
					list.Add(SqlTrace.to(trace));

				ret.traceMap.Add(key, list);
			}

			return ret;
		}

		public static SqlTraceReport from(TraceReportDataModel src) {
			SqlTraceReport ret = new SqlTraceReport();
			ret.id = src.guid;
			ret.endTime = src.endTime;
			ret.startTime = src.startTime;
			ret.traceMap = new Dictionary<string, List<SqlTrace>>();
			foreach(var key in src.traceMap.Keys) {
				var traces = src.traceMap[key];
				List<SqlTrace> list = new List<SqlTrace>();
				foreach (var trace in traces)
					list.Add(SqlTrace.from(trace));

				ret.traceMap.Add(key, list);
			}

			return ret;
		}
	}

	public class SqlTrafficReport {
		public string id { get; set; }
		public Dictionary<string, int> pageHits { get; set; }
		public Dictionary<string, int> sessionHits { get; set; }
		public Dictionary<string, int> activity { get; set; }
		public string applicationID { get; set; }
	}
}
