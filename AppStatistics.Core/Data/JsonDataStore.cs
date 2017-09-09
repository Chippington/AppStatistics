using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;

namespace AppStatistics.Core.Data {
	public class JsonDataStore : IDataStore {
		private List<ApplicationDataModel> applications;
		private static string applicationsDataFile = "applications.dat";


		public JsonDataStore() {
			if (Directory.Exists(rootPath()) == false)
				Directory.CreateDirectory(rootPath());
			loadApplications();
		}

		public void addApplication(ApplicationDataModel app) {
			applications.Add(app);
			saveApplications();
		}

		public void removeApplication(ApplicationDataModel app) {
			ApplicationDataModel match = getApplication(app.guid);
			if (match == null)
				return;

			applications.Remove(match);
			saveApplications();
		}

		public void addException(ExceptionDataModel exception) {
			try {
				if (exception.timeStamp == null)
					exception.timeStamp = DateTime.Now;

				var app = getApplication(exception.applicationID);

				string dir = getApplicationDataPath(app);
				string fname = getExceptionSetFileName(DateTime.Now);
				string fpath = Path.Combine(dir, fname);

				var excSet = loadExceptionSetFile(fpath);
				if (excSet == null)
					excSet = new List<ExceptionDataModel>();

				excSet.Add(exception);

				if (Directory.Exists(dir) == false)
					Directory.CreateDirectory(dir);

				saveExceptionSetFile(fpath, excSet);

				fname = getExceptionSetFileName();
				fpath = Path.Combine(dir, fname);

				excSet = loadExceptionSetFile(fpath);
				if (excSet == null)
					excSet = new List<ExceptionDataModel>();

				excSet.Add(exception);

				if (Directory.Exists(dir) == false)
					Directory.CreateDirectory(dir);

				saveExceptionSetFile(fpath, excSet);
			} catch (Exception exc) {
				var id = exception.applicationID == null ? "" : exception.applicationID;
				Config.store.addException(new ExceptionDataModel(exc, "root") {
					metadata = new Dictionary<string, string>() {
						{ "Application ID: ", id }
					},
					timeStamp = DateTime.Now
				});

				throw exc;
			}
		}

		public ApplicationDataModel getApplication(string key) {
			foreach (var app in applications)
				if (app.guid == key)
					return app;

			return null;
		}

		public IEnumerable<ApplicationDataModel> getApplications() {
			return new List<ApplicationDataModel>(applications);
		}

		public ExceptionDataModel getException(ApplicationDataModel app, string key) {
			var path = Path.Combine(getApplicationDataPath(app), getExceptionSetFileName());
			var set = loadExceptionSet(path);

			if (set == null)
				return null;

			foreach (var ex in set)
				if (ex.guid == key)
					return ex;

			return null;
		}

		public IEnumerable<ExceptionDataModel> getExceptions(ApplicationDataModel app) {
			var path = Path.Combine(getApplicationDataPath(app), getExceptionSetFileName());

			var excSet = loadExceptionSet(path);
			return excSet == null ? new List<ExceptionDataModel>() : excSet;
		}

		public NotificationDataModel getNotification(string key) {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationDataModel> getNotifications() {
			throw new NotImplementedException();
		}

		public IEnumerable<NotificationDataModel> getNotifications(ApplicationDataModel app) {
			throw new NotImplementedException();
		}

		public ReportDataModel getReport(ApplicationDataModel app, DateTime startDate, DateTime endDate) {
			if (startDate > endDate)
				return null;

			ReportDataModel report = new ReportDataModel();
			report.startTime = startDate;
			report.endTime = endDate;
			report.application = app;

			DateTime temp = new DateTime(startDate.Year, startDate.Month, startDate.Day);
			endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

			while (temp <= endDate) {
				var path = Path.Combine(getApplicationDataPath(app), getExceptionSetFileName(temp));
				var set = loadExceptionSet(path);
				foreach (var exc in set)
					report.exceptions.Add(exc);

				temp.AddDays(1);
			}

			return report;
		}

		public IEnumerable<ReportDataModel> getReports(DateTime startDate, DateTime endDate) {
			List<ReportDataModel> reports = new List<ReportDataModel>();
			foreach (var app in applications)
				reports.Add(getReport(app, startDate, endDate));

			return reports;
		}

		private List<ExceptionDataModel> loadExceptionSetFile(string path) {
			if (File.Exists(path) == false)
				return null;

			return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExceptionDataModel>>(
				File.ReadAllText(path));
		}

		private void saveExceptionSetFile(string path, List<ExceptionDataModel> exc) {
			if (File.Exists(path))
				File.Delete(path);

			File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(exc));
		}

		private void saveApplications() {
			File.Delete(rootPath() + applicationsDataFile);

			var startTick = Environment.TickCount;
			bool exitFlag = false;
			Exception last = null;
			while (Environment.TickCount - startTick < 10000 && exitFlag == false) {
				try {
					File.WriteAllText(rootPath() + applicationsDataFile, Newtonsoft.Json.JsonConvert.SerializeObject(applications));
					exitFlag = true;
				} catch (Exception exc) {
					last = exc;
				}
			}

			if (exitFlag == false)
				throw last;
		}

		private void loadApplications() {
			if (File.Exists(rootPath() + applicationsDataFile)) {
				applications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApplicationDataModel>>(
					File.ReadAllText(rootPath() + applicationsDataFile));
			} else {
				applications = new List<ApplicationDataModel>();
			}
		}

		public string rootPath() {
			return Directory.GetCurrentDirectory() + "\\Content\\";
		}

		public string getApplicationDataPath(ApplicationDataModel app) {
			if (app == null)
				throw new ArgumentNullException("app");

			if (app.guid == null)
				throw new ArgumentNullException("app.guid");

			return rootPath() + $"applications\\{app.guid}\\";
		}

		public string getExceptionSetFileName(DateTime date) {
			return $"excset-{date.ToString("yyyyMMdd")}.dat";
		}

		public string getExceptionSetFileName() {
			return $"excset-all.dat";
		}

		public List<ExceptionDataModel> loadExceptionSet(string path) {
			if (File.Exists(path) == false)
				return null;

			return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExceptionDataModel>>(
				File.ReadAllText(path));
		}

		public void saveExceptionSet(string path, List<ExceptionDataModel> set) {
			if (File.Exists(path))
				File.Delete(path);

			File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(set));
		}

		public void updateApplication(ApplicationDataModel app) {
			if (applications.Contains(app) == false)
				throw new Exception("This should not have happened.");

			saveApplications();
		}
	}
}