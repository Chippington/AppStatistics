using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;
using AppStatistics.Common.Models.Reporting.Analytics;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using AppStatistics.Common.Models.Reporting.Events;

namespace AppStatistics.Core.Data {
	public class JsonDataStore : IDataStore {
		public class ApplicationCollection : IEnumerable<ApplicationDataModel> {
			public Dictionary<string, ApplicationDataModel> source;
			private string fileName;

			public ApplicationCollection(string fileName) {
				this.fileName = fileName;
				source = new Dictionary<string, ApplicationDataModel>();

				if (File.Exists(fileName)) {
					var data = File.ReadAllText(fileName);
					source = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ApplicationDataModel>>(data);
				}
			}

			public IEnumerator<ApplicationDataModel> GetEnumerator() {
				return source.Values.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return source.Values.GetEnumerator();
			}

			public void set(string id, ApplicationDataModel data) {
				var app = get(id);
				if (app != null) {
					app.fromRaw(data.toRaw());
					if (app.guid != id) {
						delete(id);
						set(app.guid, data);
						return;
					}
				}

				source[id] = data;
				saveSource();
			}

			public ApplicationDataModel get(string id) {
				if (source.ContainsKey(id))
					return source[id];

				return null;
			}

			public void delete(string id) {
				if (source.ContainsKey(id))
					source.Remove(id);
			}

			private void saveSource() {
				var data = Newtonsoft.Json.JsonConvert.SerializeObject(source);
				if (File.Exists(fileName))
					File.Delete(fileName);
				File.WriteAllText(fileName, data);
			}
		}

		public class ExceptionCollection {
			private string contentFolderPath;
			private ApplicationCollection applications;

			public ExceptionCollection(ApplicationCollection applications) {
				this.contentFolderPath = JsonDataStore.contentFolderPath;
				this.applications = applications;
			}

			public void add(string appid, string excid, ExceptionDataModel exc) {
				string folder = JsonDataStore.GetExceptionFolder(appid);
				if (Directory.Exists(folder) == false)
					Directory.CreateDirectory(folder);

				string file = JsonDataStore.GetExceptionFile(appid, exc.timeStamp);
				var list = getExceptionsFromFile(file);

				list.Add(exc);
				addKey(excid, file);
				saveExceptionsToFile(file, list);
			}


			public ExceptionDataModel get(string excid) {
				string file = getExceptionFileByID(excid);
				if (file == null)
					return null;

				var list = getExceptionsFromFile(file);
				foreach (var exc in list)
					if (exc.guid == excid)
						return exc;

				return null;
			}

			public List<ExceptionDataModel> get(string appid, DateTime startTime, DateTime endTime) {
				List<ExceptionDataModel> ret = new List<ExceptionDataModel>();
				List<string> files = new List<string>();
				var temp = startTime;
				while (temp <= endTime) {
					files.Add(GetExceptionFile(appid, temp));
					temp = temp.AddDays(1);
				}

				foreach (var file in files) {
					var list = getExceptionsFromFile(file);
					foreach (var exc in list)
						ret.Add(exc);
				}

				ret = (from exc in ret
					   where exc.timeStamp > startTime && exc.timeStamp <= endTime
					   select exc).ToList();

				return ret;
			}

			public void delete(string excid) {
				string file = getExceptionFileByID(excid);
				if (file == null) return;

				removeKey(excid);
				if (File.Exists(file) == false) return;

				var list = getExceptionsFromFile(file);
				for (int i = 0; i < list.Count; i++) {
					if (list[i].guid == excid) {
						list.RemoveAt(i);
						return;
					}
				}
				saveExceptionsToFile(file, list);
			}

			private void addKey(string excid, string file) {
				file = file.Replace(JsonDataStore.contentFolderPath, "");
				string path = JsonDataStore.exceptionsKeyFilePath;
				File.AppendAllText(path, $"{excid}|{file}" + Environment.NewLine);
			}

			private void removeKey(string excid) {
				string path = JsonDataStore.exceptionsKeyFilePath;
				string tmp = path + ".tmp";
				StreamWriter writer;
				StreamReader reader;
				using (reader = new StreamReader(File.OpenRead(path))) {
					using (writer = new StreamWriter(File.OpenWrite(tmp))) {
						while (reader.Peek() >= 0) {
							string line = reader.ReadLine().Trim();
							if (line.Length > excid.Length)
								if (line.Substring(excid.Length) == excid)
									continue;

							writer.WriteLine(line);
						}
					}
				}
			}

			private string getExceptionFileByID(string excid) {
				string path = JsonDataStore.exceptionsKeyFilePath;
				if (File.Exists(path) == false)
					return null;

				StreamReader reader;
				using (reader = new StreamReader(File.OpenRead(path))) {
					while (reader.Peek() >= 0) {
						string line = reader.ReadLine();
						if (line.Length < excid.Length)
							continue;

						if (line.Trim().Substring(0, excid.Length) == excid) {
							return JsonDataStore.contentFolderPath + line.Trim().Split('|')[1];
						}
					}
				}

				return null;
			}

			private List<ExceptionDataModel> getExceptionsFromFile(string file) {
				if (File.Exists(file) == false)
					return new List<ExceptionDataModel>();

				var data = File.ReadAllText(file);
				return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExceptionDataModel>>(data);
			}

			private void saveExceptionsToFile(string file, List<ExceptionDataModel> list) {
				if (File.Exists(file))
					File.Delete(file);

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(list);
				File.WriteAllText(file, data);
			}
		}

		private ExceptionCollection exceptions;
		private ApplicationCollection applications;
		private Exception last;

		internal static string contentFolderPath;
		internal static string tempFolderPath {
			get { return contentFolderPath + "\\Temp"; }
		}
		internal static string applicationListFilePath {
			get { return contentFolderPath + "\\applications.dat"; }
		}
		internal static string applicationsFolderPath {
			get { return contentFolderPath + "\\Applications"; }
		}
		internal static string exceptionsKeyFilePath {
			get { return contentFolderPath + "\\exceptions.dat"; }
		}

		public JsonDataStore(string contentFolderPath) {
			JsonDataStore.contentFolderPath = contentFolderPath;

			if (Directory.Exists(JsonDataStore.contentFolderPath) == false)
				Directory.CreateDirectory(JsonDataStore.contentFolderPath);
			if (Directory.Exists(JsonDataStore.applicationsFolderPath) == false)
				Directory.CreateDirectory(JsonDataStore.applicationsFolderPath);
			if (Directory.Exists(JsonDataStore.applicationsFolderPath) == false)
				Directory.CreateDirectory(JsonDataStore.applicationsFolderPath);


			applications = new ApplicationCollection(applicationListFilePath);
			exceptions = new ExceptionCollection(applications);
		}

		public bool AddApplication(ApplicationDataModel appData) {
			try {
				applications.set(appData.guid, appData);
				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public bool AddApplication(string applicationID, ApplicationDataModel appData) {
			try {
				applications.set(applicationID, appData);
				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public bool AddException(string applicationID, ExceptionDataModel exceptionData) {
			try {
				exceptionData.applicationID = applicationID;
				exceptions.add(applicationID, exceptionData.guid, exceptionData);
				return true;
			} catch (Exception exc) {
				last = exc;
			}

			return false;
		}

		public bool AddTraceData(string sessionID, TraceDataModel traceData) {
			try {

				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public bool DeleteApplication(string applicationID) {
			try {
				applications.delete(applicationID);
				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public bool DeleteException(string exceptionID) {
			try {
				exceptions.delete(exceptionID);
				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public bool DeleteTraceData(string traceID) {
			try {

				return true;
			} catch (Exception exc) {
				last = exc;
				AddException("root", new ExceptionDataModel(exc));
			}

			return false;
		}

		public IEnumerable<ApplicationDataModel> GetAllApplications() {
			return new List<ApplicationDataModel>(applications.source.Values.ToList());
		}

		public ApplicationDataModel GetApplication(string applicationID) {
			return applications.get(applicationID);
		}

		public ExceptionDataModel GetException(string exceptionID) {
			return exceptions.get(exceptionID);
		}

		public IEnumerable<ExceptionDataModel> GetExceptionsByApplication(string applicationID, DateTime startTime, DateTime endTime) {
			return exceptions.get(applicationID, startTime, endTime);
		}

		public TraceDataModel GetTraceData(string traceID) {
			throw new NotImplementedException();
		}

		public IEnumerable<TraceDataModel> GetTraceDataBySession(string sessionID) {
			throw new NotImplementedException();
		}

		public bool UpdateApplication(string applicationID, ApplicationDataModel appData) {
			try {
				applications.set(applicationID, appData);
				if (applicationID != appData.guid) {
					string oldFolder = GetApplicationFolder(applicationID);
					string newFolder = GetApplicationFolder(appData.guid);

					if (newFolder == oldFolder)
						return true;

					if (Directory.Exists(newFolder))
						Directory.Delete(newFolder);

					Directory.Move(oldFolder, newFolder);
				}

				return true;
			} catch (Exception exc) {
				last = exc;
			}

			return false;
		}

		internal static string GetApplicationFolder(string appid) {
			return $"{applicationsFolderPath}\\{appid}";
		}

		internal static string GetSessionFolder(string appid) {
			return GetApplicationFolder(appid) + "\\Sessions";
		}

		internal static string GetSessionFile(string appid, string sessionid) {
			return GetSessionFolder(appid) + $"\\session-{sessionid}.dat";
		}

		internal static string GetExceptionFolder(string appid) {
			var appPath = GetApplicationFolder(appid);
			return $"{appPath}\\Exceptions";
		}

		internal static string GetExceptionFile(string appid, DateTime date) {
			date = date.Date;
			var folderPath = GetExceptionFolder(appid);
			return $"{folderPath}\\excset-{date.ToString("yyyyMMdd")}.dat";
		}

		internal static string GetEventFile(string appid) {
			var appPath = GetApplicationFolder(appid);
			return $"{appPath}\\eventlog.dat";
		}

		public Exception GetLastException() {
			return last;
		}

		public TraceReportDataModel GetTraceReport(string appid, DateTime startDate, DateTime endDate) {
			var app = GetApplication(appid);
			if (app == null)
				return null;

			if (string.IsNullOrEmpty(app.analyticsEndpoint))
				return null;

			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(app.analyticsEndpoint);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(app.analyticsEndpoint + $"/GetActivity");
				req.Method = "POST";
				req.ContentType = "application/json";

				using (var sw = new StreamWriter(req.GetRequestStream())) {
					string json = Newtonsoft.Json.JsonConvert.SerializeObject(new { startDate, endDate });
					sw.Write(json);
					sw.Flush();
				}

				var response = req.GetResponse();
				var dataStream = response.GetResponseStream();
				var reader = new StreamReader(dataStream);

				StringBuilder sb = new StringBuilder();
				while(reader.Peek() >= 0) {
					sb.AppendLine(reader.ReadLine());
				}

				var data = sb.ToString();
				if(data.Trim().Length > 0)
					return Newtonsoft.Json.JsonConvert.DeserializeObject<TraceReportDataModel>(data);
			}

			return null;
		}

		public TraceReportDataModel GetSessionReport(string appid, string sessionid) {
			var app = GetApplication(appid);
			if (app == null)
				return null;

			if (string.IsNullOrEmpty(app.analyticsEndpoint))
				return null;

			string fname = GetSessionFile(appid, sessionid);
			if (File.Exists(fname)) {
				TraceReportDataModel ret = new TraceReportDataModel();
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(fname));
				ret.fromRaw(data);
				return ret;
			}

			string address = app.analyticsEndpoint + $"?op=getsession&sessionID={sessionid}";

			try {
				using (WebClient client = new WebClient()) {
					client.UseDefaultCredentials = true;
					client.Credentials = CredentialCache.DefaultNetworkCredentials;
					var result = client.DownloadString(address);
					if (result.Trim().Length == 0)
						return null;

					dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

					TraceReportDataModel ret = new TraceReportDataModel();
					ret.fromRaw(data);

					if (Directory.Exists(GetSessionFolder(appid)) == false)
						Directory.CreateDirectory(GetSessionFolder(appid));

					if (File.Exists(fname))
						File.Delete(fname);

					File.WriteAllText(fname, result);
					return ret;
				}
			} catch (Exception exc) {
				Config.store.AddException("root", new ExceptionDataModel(exc) {
					metadata = new Dictionary<string, string>() {
						{ "Application ID", appid },
						{ "Session ID", sessionid },
						{ "URI Address", address },
						{ "Cache File Name", fname },
					}
				});
			}

			return null;
		}

		public IEnumerable<EventDataModel> GetEventsByApplication(string applicationID) {
			return getEvents(applicationID);
		}

		public EventDataModel GetEvent(string applicationID, string eventID) {
			var list = getEvents(applicationID);
			foreach (var item in list)
				if (item.guid == eventID)
					return item;

			return null;
		}

		public bool AddEventData(string appid, EventDataModel eventData) {
			var list = getEvents(appid);
			if (list == null)
				list = new List<EventDataModel>();

			list.Add(eventData);
			setEvents(appid, list);
			return true;
		}

		public bool DeleteEventData(string appid, string eventID) {
			var list = getEvents(appid);
			list = (from ev in list
					where ev.guid != eventID
					select ev).ToList();

			setEvents(appid, list);
			return true;
		}

		private List<EventDataModel> getEvents(string applicationID) {
			string file = GetEventFile(applicationID);
			if (File.Exists(file) == false)
				return null;

			var data = File.ReadAllText(file);
			var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EventDataModel>>(data);
			return list;
		}

		private void setEvents(string applicationID, List<EventDataModel> list) {
			string file = GetEventFile(applicationID);
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(list);
			if (File.Exists(file))
				File.Delete(file);

			File.WriteAllText(file, data);
		}
	}
}