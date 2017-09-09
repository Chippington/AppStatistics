using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;
using System.Threading.Tasks;
using System.IO;
using System.Linq;


namespace AppStatistics.Common.Reporting.Exceptions {
	public static class ExceptionLog {
		public static async Task<HttpStatusCode> LogException(Exception exception) {
			return await LogException(exception, new Dictionary<string, string>());
		}

		public static async Task<HttpStatusCode> LogException(Exception exception, Dictionary<string, string> metadata) {
			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(ReportingConfig.baseURI);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var exc = new ExceptionDataModel(exception, ReportingConfig.applicationID);
				exc.timeStamp = DateTime.Now;
				exc.metadata = metadata;

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
				StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync("reporting/api/Exceptions", stringContent);
				return response.StatusCode;
			}
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