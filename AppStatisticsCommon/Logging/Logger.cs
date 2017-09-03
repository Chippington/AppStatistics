using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System.Threading.Tasks;

namespace AppStatisticsCommon.Logging {
	public static class Logger {
		private static ApplicationModel application;
		private static string baseURI;
		
		public static void Configure(string baseURI, ApplicationModel application) {
			Logger.application = application;
			Logger.baseURI = baseURI;
		}

		public static async Task<HttpStatusCode> LogException(Exception exception) {
			return await LogException(exception, new Dictionary<string, string>());
		}

		public static async Task<HttpStatusCode> LogException(Exception exception, Dictionary<string, string> metadata) {
			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(baseURI);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var exc = new ExceptionModel(exception, application);
				exc.timeStamp = DateTime.Now;
				exc.metadata = metadata;

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
				StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync("api/Exceptions/", stringContent);
				return response.StatusCode;
			}
		}
	}
}
