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
		/// <summary>
		/// Logs an exception asynchronously to the API.
		/// If logging to API fails, uses fallback (folder \\Fallback\\ is used)
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <returns></returns>
		public static async Task<bool> LogExceptionAsync(Exception exception) {
			return await LogExceptionAsync(exception, new Dictionary<string, string>());
		}

		/// <summary>
		/// Logs an exception asynchronously to the API.
		/// If logging to API fails, uses fallback (folder \\Fallback\\ is used)
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata included with exception</param>
		/// <returns></returns>
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

		/// <summary>
		/// Logs an exception to the API.
		/// If logging to API fails, uses fallback (folder \\Fallback\\ is used)
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <returns></returns>
		public static bool LogException(Exception exception) {
			return LogException(exception, new Dictionary<string, string>());
		}

		/// <summary>
		/// Logs an exception to the API.
		/// If logging to API fails, uses fallback (folder \\Fallback\\ is used)
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata included with exception</param>
		/// <returns></returns>
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

		/// <summary>
		/// Creates a data model from the given exception/metadata
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata to include with exception</param>
		/// <returns></returns>
		private static ExceptionDataModel createModel(Exception exception, Dictionary<string, string> metadata) {
			var appid = ReportingConfig.applicationID;
			var exc = new ExceptionDataModel(exception);
			exc.timeStamp = DateTime.Now;
			exc.metadata = metadata;
			return exc;
		}

		/// <summary>
		/// Logs an exception to the API.
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata to include with exception</param>
		/// <returns>True if succeeded</returns>
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

		/// <summary>
		/// Logs an exception to the API asynchronously.
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata to include with exception</param>
		/// <returns>True if succeeded</returns>
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

		/// <summary>
		/// Logs the exception to the fallback folder.
		/// </summary>
		/// <param name="exception">Exception source</param>
		/// <param name="metadata">Metadata to include with exception</param>
		private static void logFallback(Exception exception, Dictionary<string, string> metadata) {
			var exc = createModel(exception, metadata);
			string fallbackFolder = ReportingConfig.contentFolderPath + "\\Fallback";

			int ct = 0;
			while (File.Exists(fallbackFolder + $"\\exc{ct}.dat"))
				ct++;

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
			File.WriteAllText(fallbackFolder + $"\\exc{ct}.dat", data);
		}
	}
}