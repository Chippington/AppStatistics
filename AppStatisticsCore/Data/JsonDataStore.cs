using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;

namespace AppStatisticsCore.Data {
	public class JsonDataStore : IDataStore {
		private List<ApplicationModel> applications;
		private static string applicationsDataFile = "applications.dat";


		public JsonDataStore() {
			if (Directory.Exists(rootPath()) == false)
				Directory.CreateDirectory(rootPath());
			loadApplications();
		}

		public void addApplication(ApplicationModel app) {
			applications.Add(app);
			saveApplications();
		}

		public void removeApplication(ApplicationModel app) {
			ApplicationModel match = getApplication(app.guid);
			if (match == null)
				return;

			applications.Remove(match);
			saveApplications();
		}

		public void addException(ExceptionModel exception) {
			var app = getApplication(exception.application.guid);
			exception.application = app;

			string dir = getApplicationDataPath(app);
			string fname = getExceptionSetFileName(DateTime.Now);
			string fpath = Path.Combine(dir, fname);

			var excSet = loadExceptionSetFile(fpath);
			if (excSet == null)
				excSet = new List<ExceptionModel>();

			excSet.Add(exception);

			if (Directory.Exists(dir) == false)
				Directory.CreateDirectory(dir);

			saveExceptionSetFile(fpath, excSet);

			fname = getExceptionSetFileName();
			fpath = Path.Combine(dir, fname);

			excSet = loadExceptionSetFile(fpath);
			if (excSet == null)
				excSet = new List<ExceptionModel>();

			excSet.Add(exception);

			if (Directory.Exists(dir) == false)
				Directory.CreateDirectory(dir);

			saveExceptionSetFile(fpath, excSet);
		}

		public ApplicationModel getApplication(string key) {
			foreach (var app in applications)
				if (app.guid == key)
					return app;

			return null;
		}

		public IEnumerable<ApplicationModel> getApplications() {
			return new List<ApplicationModel>(applications);
		}

		public ExceptionModel getException(ApplicationModel app, string key) {
			var path = Path.Combine(getApplicationDataPath(app), getExceptionSetFileName());
			var set = loadExceptionSet(path);

			if (set == null)
				return null;

			foreach (var ex in set)
				if (ex.guid == key)
					return ex;

			return null;
		}

		public IEnumerable<ExceptionModel> getExceptions(ApplicationModel app) {
			var path = Path.Combine(getApplicationDataPath(app), getExceptionSetFileName());

			var excSet = loadExceptionSet(path);
			return excSet == null ? new List<ExceptionModel>() : excSet;
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
			if (startDate > endDate)
				return null;

			ReportModel report = new ReportModel();
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

		public IEnumerable<ReportModel> getReports(DateTime startDate, DateTime endDate) {
			List<ReportModel> reports = new List<ReportModel>();
			foreach (var app in applications)
				reports.Add(getReport(app, startDate, endDate));

			return reports;
		}

		private List<ExceptionModel> loadExceptionSetFile(string path) {
			if (File.Exists(path) == false)
				return null;

			return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExceptionModel>>(
				File.ReadAllText(path));
		}

		private void saveExceptionSetFile(string path, List<ExceptionModel> exc) {
			if (File.Exists(path))
				File.Delete(path);

			File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(exc));
		}

		private void saveApplications() {
			File.Delete(rootPath() + applicationsDataFile);
			File.WriteAllText(rootPath() + applicationsDataFile, Newtonsoft.Json.JsonConvert.SerializeObject(applications));
		}

		private void loadApplications() {
			if (File.Exists(rootPath() + applicationsDataFile)) {
				applications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApplicationModel>>(
					File.ReadAllText(rootPath() + applicationsDataFile));
			} else {
				applications = new List<ApplicationModel>();
			}
		}

		public string rootPath() {
			return Directory.GetCurrentDirectory() + "\\Content\\";
		}

		public string getApplicationDataPath(ApplicationModel app) {
			return rootPath() + $"applications\\{app.guid}\\";
		}

		public string getExceptionSetFileName(DateTime date) {
			return $"excset-{date.ToString("yyyyMMdd")}.dat";
		}

		public string getExceptionSetFileName() {
			return $"excset-all.dat";
		}

		public List<ExceptionModel> loadExceptionSet(string path) {
			if (File.Exists(path) == false)
				return null;

			return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExceptionModel>>(
				File.ReadAllText(path));
		}

		public void saveExceptionSet(string path, List<ExceptionModel> set) {
			if (File.Exists(path))
				File.Delete(path);

			File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(set));
		}
	}
}