using AppStatistics.Common.Models.Reporting.Events;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppStatistics.Common.Reporting.Events {
	public static class EventLog {
		public static void LogEvent(string message) {
			EventLog.LogEvent(message, "", new Dictionary<string, string>());
		}

		public static void LogEvent(string message, string category, Dictionary<string, string> metadata) {
			EventDataModel ev = new EventDataModel(message, metadata);
			ev.category = category;
			logToApi(ev);
		}

		public async static void LogEventAsync(string message) {
			await Task.Run(() => {
				LogEvent(message);
			});
		}
		public async static void LogEventAsync(string message, string category, Dictionary<string, string> metadata) {
			await Task.Run(() => {
				LogEvent(message, category, metadata);
			});
		}

		private static string logToApi(EventDataModel ev) {
			try {
				using (var httpClient = new HttpClient()) {
					httpClient.BaseAddress = new Uri(ReportingConfig.baseURI);
					httpClient.DefaultRequestHeaders.Accept.Clear();
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					ev.applicationID = ReportingConfig.applicationID;
					var data = Newtonsoft.Json.JsonConvert.SerializeObject(ev.toRaw());
					StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

					HttpResponseMessage response = httpClient.PostAsync("api/Events", stringContent).Result;
					if (response.StatusCode == HttpStatusCode.OK)
						return ev.guid;
				}
			} catch(Exception exc) {
				try {
					ExceptionLog.LogException(exc);
				} catch {
					//Ignore
				}
			}

			return null;
		}
	}
}
