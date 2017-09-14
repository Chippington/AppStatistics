using AppStatistics.Common.Models.Reporting.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AppStatistics.Common.Reporting.Events {
	public static class EventLog {
		public static void LogEvent(string message) {
			EventLog.LogEvent(message, new Dictionary<string, string>());
		}

		public static void LogEvent(string message, Dictionary<string, string> metadata) {
			EventDataModel ev = new EventDataModel(message, metadata);
			logToApi(ev);
		}
		
		private static string logToApi(EventDataModel ev) {
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

			return null;
		}
	}
}
