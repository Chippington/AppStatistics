using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Models;

namespace AppStatistics.Data {
	public class TestDataStore : IDataStore {

		private List<ApplicationModel> _applications;
		private List<ApplicationModel> applications {
			get {
				if (_applications == null) {
					List<ExceptionModel> excList1 = new List<ExceptionModel>();
					List<ExceptionModel> excList2 = new List<ExceptionModel>();
					for (int i = 0; i < 20; i++) {
						excList1.Add(new ExceptionModel() {
							exception = new ExceptionModel.ExceptionInfo(new Exception("Test Exception 1")),
							timeStamp = DateTime.Now.AddDays(-1 * i),
						});
					}

					for (int i = 0; i < 20; i++) {
						excList2.Add(new ExceptionModel() {
							exception = new ExceptionModel.ExceptionInfo(new Exception("Test Exception 2")),
							timeStamp = DateTime.Now.AddDays(-1 * i),
						});
					}

					_applications = new List<ApplicationModel>() {
						new ApplicationModel("Test Application") {
							exceptionList = excList1,
						},
						new ApplicationModel("Test Application 2") {
							exceptionList = excList2,
						},
					};
				}

				return _applications;
			}
			set { }
		}

		public void addApplication(ApplicationModel app) {
			applications.Add(app);
		}

		public void addException(ApplicationModel app, ExceptionModel exception) {
			app.exceptionList.Add(exception);
		}

		public ApplicationModel getApplication(string key) {
			return (from app in getApplications() where app.guid == key select app).FirstOrDefault();
		}

		public IEnumerable<ApplicationModel> getApplications() {
			return applications;
		}

		public ExceptionModel getException(string key) {
			return (from exc in getExceptions() where exc.guid == key select exc).FirstOrDefault();
		}

		public IEnumerable<ExceptionModel> getExceptions() {
			List<ExceptionModel> ret = new List<ExceptionModel>();
			foreach (var app in applications)
				foreach (var exc in app.exceptionList)
					ret.Add(exc);

			return ret;
		}

		public NotificationModel getNotification(string key) {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationModel> getNotifications() {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationModel> getNotifications(ApplicationModel app) {
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

		public void removeApplication(ApplicationModel app) {
			throw new NotImplementedException();
		}
	}
}