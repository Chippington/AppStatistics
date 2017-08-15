using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Models;
using System.IO;

namespace AppStatistics.Data
{
	public class FileDataStore : IDataStore {
		private List<ApplicationModel> _applications;
		private List<ApplicationModel> applications {
			get {
				load();
				return new List<ApplicationModel>(_applications);
			}
		}
		private List<NotificationModel> notifications;

		public void addApplication(ApplicationModel app) {
			applications.Add(app);
			app.save(app.guid + ".dat");
		}

		public ApplicationModel getApplication(string key) {
			return (from app in applications
					where app.guid == key
					select app).FirstOrDefault();
		}

		public IEnumerable<ApplicationModel> getApplications() {
			return new List<ApplicationModel>(applications);
		}

		public void removeApplication(ApplicationModel app) {
			app = getApplication(app.guid);
			applications.Remove(app);
		}

		public ExceptionModel getException(string key) {
			return (from app in applications
					where app.exceptionList.Exists((exc) => exc.guid == key)
					select (from exc in app.exceptionList
							where exc.guid == key
							select exc).FirstOrDefault()).FirstOrDefault();
		}

		public IEnumerable<ExceptionModel> getExceptions() {
			List<ExceptionModel> ret = new List<ExceptionModel>();
			foreach (var app in applications)
				foreach (var exc in app.exceptionList)
					ret.Add(exc);

			return ret;
		}

		public void addException(ApplicationModel app, ExceptionModel exception) {
			app = getApplication(app.guid);
			app.exceptionList.Add(exception);
			app.save(app.guid + ".dat");
		}

		public NotificationModel getNotification(string key) {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationModel> getNotifications() {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationModel> getNotifications(ApplicationModel app) {
			app = getApplication(app.guid);
			throw new NotImplementedException();
		}

		public ReportModel getReport(ApplicationModel app, DateTime startDate, DateTime endDate) {
			app = getApplication(app.guid);
			var excList = (from exc in app.exceptionList
						   where exc.timeStamp >= startDate && exc.timeStamp <= endDate
						   select exc).OrderBy((exc) => exc.timeStamp).ToList();

			return new ReportModel() {
				startTime = startDate,
				endTime = endDate,
				exceptions = excList,
			};
		}

		public IEnumerable<ReportModel> getReports(DateTime startDate, DateTime endDate) {
			List<ReportModel> ret = new List<ReportModel>();
			foreach (var app in applications)
				ret.Add(getReport(app, startDate, endDate));

			return ret;
		}

		private void load() {
			var path = Directory.GetCurrentDirectory();
			string[] files = Directory.GetFiles(path, "*.dat");
			_applications = new List<ApplicationModel>();

			foreach(var str in files) {
				ApplicationModel m = new ApplicationModel("");
				m.load(str);

				_applications.Add(m);
			}
		}
	}
}
