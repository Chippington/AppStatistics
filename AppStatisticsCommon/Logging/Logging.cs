using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Linq;


namespace AppStatisticsCommon.Logging {
	public static class Log {
		internal static LogOptions options;

		public static void StartupConfigureServices(IServiceCollection services) {
			services.AddCors(options => options.AddPolicy("AllowCors", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
		}

		public static void StartupConfigureApplication(IApplicationBuilder app, IHostingEnvironment env) {
			app.UseCors("AllowCors");
		}

		public static async Task<HttpStatusCode> LogException(Exception exception) {
			return await LogException(exception, new Dictionary<string, string>());
		}

		public static async Task<HttpStatusCode> LogException(Exception exception, Dictionary<string, string> metadata) {
			using (var httpClient = new HttpClient()) {
				httpClient.BaseAddress = new Uri(options.baseURI);
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var exc = new ExceptionDataModel(exception, options.applicationID);
				exc.timeStamp = DateTime.Now;
				exc.metadata = metadata;

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(exc.toRaw());
				StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync("reporting/api/Exceptions", stringContent);
				return response.StatusCode;
			}
		}

		public static async Task LogTrafficHit(string sessionID, string page) {
			try {
				string dir = getContentPath();
				string fname = getTrafficFileName(DateTime.Now);

				sessionID = sessionID.Replace("|", "");
				page = page.Replace("|", "");

				File.AppendAllText(dir + fname, $"{DateTime.Now.ToString("hh:mm:ss")}|{sessionID}|{page}");
			} catch (Exception exc) {
				if (options.applicationID != null)
					await LogException(exc, new Dictionary<string, string>() {
						{ "Session ID", sessionID },
						{ "Page", page },
					});
			}
		}

		private static void saveOptions() {
			string dir = getContentPath();
			string fname = getConfigFileName();
			try {
				if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
				if (File.Exists(fname)) File.Delete(fname);

				var data = Newtonsoft.Json.JsonConvert.SerializeObject(options);
				File.WriteAllText(dir + fname, data);
			} catch (Exception exc) {
				if (options.applicationID != null)
					LogException(exc, new Dictionary<string, string>() {
						{ "File name", dir + fname },
					}).Wait();
			}
		}

		private static void loadOptions() {
			string dir = getContentPath();
			string fname = getConfigFileName();
			try {
				if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
				if (File.Exists(fname) == false) return;

				var data = File.ReadAllText(dir + fname);
				//options = Newtonsoft.Json.JsonConvert.DeserializeObject<LogOptions>(data);
			} catch (Exception exc) {
				if (options.applicationID != null)
					LogException(exc, new Dictionary<string, string>() {
						{ "File name", dir + fname },
					}).Wait();
			}
		}

		internal static List<string> getTrafficData(DateTime date) {
			string dir = getContentPath();
			string fname = getTrafficFileName(DateTime.Now);

			if(File.Exists(fname)) {
				return File.ReadAllLines(fname).ToList();
			}

			return new List<string>();
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