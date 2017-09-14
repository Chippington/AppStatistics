using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;
using System.Threading.Tasks;
using System.IO;
using System.Linq;


namespace AppStatistics.Common.Reporting.Exceptions {
	public static class ExceptionLog {
		public static async Task<bool> LogExceptionAsync(Exception exception) {
			return await LogExceptionAsync(exception, new Dictionary<string, string>());
		}

		public static async Task<bool> LogExceptionAsync(Exception exception, Dictionary<string, string> metadata) {
			if (exception == null)
				return false;

			if (metadata == null)
				metadata = new Dictionary<string, string>();

			try {
				var apiResult = await logToApiAsync(exception, metadata);
				if (!apiResult)
					logFallback(exception, metadata);

				return true;
			} catch (Exception exc) {
				try {
					logFallback(exc, new Dictionary<string, string>() {
						{ "Base Exception Message", exception.Message },
						{ "Base Exception Stack Trace", exception.StackTrace }
					});
				} catch {
					throw exc;
				}
			}

			return false;
		}

		public static bool LogException(Exception exception) {
			return LogException(exception, new Dictionary<string, string>());
		}

		public static bool LogException(Exception exception, Dictionary<string, string> metadata) {
			if (exception == null)
				return false;

			if (metadata == null)
				metadata = new Dictionary<string, string>();

			try {
				var apiResult = logToApi(exception, metadata);
				if (!apiResult)
					logFallback(exception, metadata);

				return true;
			} catch (Exception exc) {
				try {
					logFallback(exc, new Dictionary<string, string>() {
						{ "Base Exception Message", exception.Message },
						{ "Base Exception Stack Trace", exception.StackTrace }
					});
				} catch {
					throw exc;
				}
			}

			return false;
		}

		private static ExceptionDataModel createModel(Exception exception, Dictionary<string, string> metadata) {
			var appid = ReportingConfig.applicationID;
			var exc = new ExceptionDataModel(exception, appid);
			exc.timeStamp = DateTime.Now;
			exc.metadata = metadata;
			return exc;
		}

		private static bool logToApi(Exception exception, Dictionary<string, string> metadata) {
			var exc = createModel(exception, metadata);

			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(ReportingConfig.baseURI);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
				StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

				HttpResponseMessage response = httpClient.PostAsync("api/Exceptions", stringContent).Result;
				if (response.StatusCode == HttpStatusCode.OK)
					return true;
			}

			return false;
		}

		private static async Task<bool> logToApiAsync(Exception exception, Dictionary<string, string> metadata) {
			var exc = createModel(exception, metadata);

			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(ReportingConfig.baseURI);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
				StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync("api/Exceptions", stringContent);
				if (response.StatusCode == HttpStatusCode.OK)
					return true;
			}

			return false;
		}

		private static void logFallback(Exception exception, Dictionary<string, string> metadata) {
			var exc = createModel(exception, metadata);
			string fallbackFolder = ReportingConfig.contentFolderPath + "\\Fallback";

			int ct = 0;
			while (File.Exists(fallbackFolder + $"\\exc{ct}.dat"))
				ct++;

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
			File.WriteAllText(fallbackFolder + $"\\exc{ct}.dat", data);
		}

		private static string getContentPath() {
			string rootDir = Directory.GetCurrentDirectory();
			if (Directory.Exists(rootDir + "\\App_Data")) {
				return rootDir + "\\App_Data";
			}

			return rootDir + "\\Content";
		}

		private static string getConfigFileName() {
			return "\\reporting-config.dat";
		}

		private static string getTrafficFileName(DateTime date) {
			return $"\\{date.ToString("traffic-yyyyMMdd.dat")}";
		}
	}
}