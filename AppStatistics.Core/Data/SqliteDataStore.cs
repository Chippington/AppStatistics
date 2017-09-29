using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Models.Reporting.Events;
using AppStatistics.Common.Models.Reporting.Exceptions;
using AppStatistics.Core.Data.Sqlite;

namespace AppStatistics.Core.Data
{
	public class SqliteDataStore : IDataStore {
		public bool AddApplication(ApplicationDataModel appData) {
			using (ReportingContext db = new ReportingContext()) {
				var app = SqlApplication.from(appData);
				app.active = true;

				if (db.Applications.Count(a => a.id == app.id) > 0) {
					DeleteApplication(app.id);
				}

				db.Applications.Add(app);
				db.SaveChanges();
			}

			return true;
		}

		public bool AddApplication(string applicationID, ApplicationDataModel appData) {
			using (ReportingContext db = new ReportingContext()) {
				var app = SqlApplication.from(appData);
				app.active = true;
				app.id = applicationID;

				if (db.Applications.Where(a => a.id == app.id).ToList().Count > 0) {
					DeleteApplication(app.id);
				}

				db.Applications.Add(app);
				db.SaveChanges();
			}

			return true;
		}

		public bool AddEventData(string appid, EventDataModel eventData) {
			using (ReportingContext db = new ReportingContext()) {
				var ev = SqlEvent.from(eventData);
				ev.active = true;
				db.Events.Add(ev);
				db.SaveChanges();
			}

			return true;
		}

		public bool AddException(string applicationID, ExceptionDataModel exceptionData) {
			using (ReportingContext db = new ReportingContext()) {
				var exc = SqlException.from(exceptionData);
				exc.active = true;
				db.Exceptions.Add(exc);
				db.SaveChanges();
			}

			return true;
		}

		public bool AddTraceData(string sessionID, TraceDataModel traceData) {
			throw new NotImplementedException();
		}

		public bool DeleteApplication(string applicationID) {
			using (ReportingContext db = new ReportingContext()) {
				var app = db.Applications.Where(a => a.id == applicationID).FirstOrDefault();
				if (app != null) {
					db.Applications.Remove(app);
					db.SaveChanges();
				}
			}

			return true;
		}

		public bool DeleteEventData(string appid, string eventID) {
			throw new NotImplementedException();
		}

		public bool DeleteException(string exceptionID) {
			throw new NotImplementedException();
		}

		public bool DeleteTraceData(string traceID) {
			throw new NotImplementedException();
		}

		public IEnumerable<ApplicationDataModel> GetAllApplications() {
			List<ApplicationDataModel> ret = new List<ApplicationDataModel>();
			using (ReportingContext db = new ReportingContext()) {
				ret = db.Applications.Where((a) => a.active)
					.Select((a) => SqlApplication.to(a))
					.ToList();

				db.SaveChanges();
			}

			return ret;
		}

		public ApplicationDataModel GetApplication(string applicationID) {
			ApplicationDataModel ret = null;
			using (ReportingContext db = new ReportingContext()) {
				var tmp = db.Applications.Where((a) => a.id == applicationID && a.active).FirstOrDefault();

				if(tmp != null)
					ret = SqlApplication.to(tmp);
			}

			return ret;
		}

		public EventDataModel GetEvent(string appid, string eventID) {
			EventDataModel ret = null;
			using (ReportingContext db = new ReportingContext()) {
				var tmp = db.Events.Where((a) => a.id == eventID).FirstOrDefault();

				if (tmp != null)
					ret = SqlEvent.to(tmp);
			}

			return ret;
		}

		public IEnumerable<EventDataModel> GetEventsByApplication(string applicationID) {
			List<EventDataModel> ret = new List<EventDataModel>();
			using (ReportingContext db = new ReportingContext()) {
				ret = db.Events.Where((a) => a.applicationID == applicationID && a.active)
					.Select((a) => SqlEvent.to(a))
					.ToList();
			}

			return ret;
		}

		public ExceptionDataModel GetException(string exceptionID) {
			ExceptionDataModel ret = null;
			using (ReportingContext db = new ReportingContext()) {
				var tmp = db.Exceptions.Where((a) => a.id == exceptionID).FirstOrDefault();

				if (tmp != null)
					ret = SqlException.to(tmp);
			}

			return ret;
		}

		public IEnumerable<ExceptionDataModel> GetExceptionsByApplication(string applicationID, DateTime startTime, DateTime endTime) {
			List<ExceptionDataModel> ret = new List<ExceptionDataModel>();
			using (ReportingContext db = new ReportingContext()) {
				ret = db.Exceptions.Where((a) => a.applicationID == applicationID && a.active)
					.Select((a) => SqlException.to(a))
					.ToList();
			}

			return ret;
		}

		public Exception GetLastException() {
			throw new NotImplementedException();
		}

		public TraceReportDataModel GetSessionReport(string appid, string sessionid) {
			return null;
		}

		public TraceDataModel GetTraceData(string traceID) {
			throw new NotImplementedException();
		}

		public IEnumerable<TraceDataModel> GetTraceDataBySession(string sessionID) {
			throw new NotImplementedException();
		}

		public TraceReportDataModel GetTraceReport(string appid, DateTime startDate, DateTime endDate) {
			throw new NotImplementedException();
		}

		public bool UpdateApplication(string applicationID, ApplicationDataModel appData) {
			using (ReportingContext db = new ReportingContext()) {
				if(applicationID == appData.guid) {
					var app = SqlApplication.from(appData);
					app.active = true;

					db.Applications.Update(app);
				} else {
					db.Applications.Remove(db.Applications.First((a) => a.id == applicationID));
					db.Applications.Add(SqlApplication.from(appData));
				}

				db.SaveChanges();
			}

			return true;
		}
	}
}
