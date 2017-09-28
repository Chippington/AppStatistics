using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Reporting;
using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace AppStatistics.Common.WebForms {
	public class AnalyticsModule : IHttpModule {
		public static bool logAnalytics;
		public static bool logExceptions;
		public static bool redirectExceptions;
		public static bool redirectHttpExceptions;
		public static bool handleHttpExceptions;
		public static string redirectExceptionPath;
		public static string redirectHttpExceptionPath;
		public static string analyticsEndpointPath;


		public void Dispose() {
			//clean-up code here.
		}

		/// <summary>
		/// Initializes the module
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context) {
			logAnalytics = ConfigurationManager.AppSettings["reportingLogAnalytics"] == "true";
			logExceptions = ConfigurationManager.AppSettings["reportingLogExceptions"] == "true";
			if (logAnalytics == false && logExceptions == false)
				return;

			var root = HttpRuntime.AppDomainAppPath.Substring(0, HttpRuntime.AppDomainAppPath.Length - 1);
			var contentFolderPath = root + ConfigurationManager.AppSettings["reportingBaseContentPath"];
			var endpoint = ConfigurationManager.AppSettings["reportingAnalyticsEndpointPath"];
			var applicationID = ConfigurationManager.AppSettings["reportingApplicationID"];
			var baseURI = ConfigurationManager.AppSettings["reportingBaseUrl"];
			if (contentFolderPath != null)
				contentFolderPath = contentFolderPath.Replace("/", "\\");

			//Scrub directory path so that it does not end in a '/' or '\' (for internal use)
			while (contentFolderPath.Length > 0 && contentFolderPath[contentFolderPath.Length - 1] == '\\')
				contentFolderPath = contentFolderPath.Substring(0, contentFolderPath.Length - 1);

			//Ensure reporting api path ends in forward slash
			//example: http://hostname.com/reporting/
			if (baseURI[baseURI.Length - 1] != '/')
				baseURI = baseURI + '/';

			Reporting.ReportingConfig.contentFolderPath = contentFolderPath;
			Reporting.ReportingConfig.applicationID = applicationID;
			Reporting.ReportingConfig.baseURI = baseURI;

			//Custom analytics endpoint
			if(endpoint.Length > 0) {
				if (endpoint[0] != '/')
					endpoint = "/" + endpoint;

				if (endpoint[endpoint.Length - 1] == '/')
					endpoint = endpoint.Substring(0, endpoint.Length - 1);
			}

			analyticsEndpointPath = endpoint;

			//Redirect settings
			redirectExceptions = ConfigurationManager.AppSettings["reportingRedirectExceptions"] == "true";
			handleHttpExceptions = ConfigurationManager.AppSettings["reportingHandleHttpExceptions"] == "true";
			redirectHttpExceptions = ConfigurationManager.AppSettings["reportingRedirectHttpExceptions"] == "true";

			//Redirect paths
			redirectExceptionPath = ConfigurationManager.AppSettings["reportingRedirectExceptionsPath"];
			redirectHttpExceptionPath = ConfigurationManager.AppSettings["reportingRedirectHttpExceptionsPath"];

			context.Error += Context_Error;
			context.AcquireRequestState += Context_AcquireRequestState;
			context.PreRequestHandlerExecute += Context_PreRequestHandlerExecute;
		}

		private void Context_PreRequestHandlerExecute(dynamic sender, EventArgs e) {
			if (logAnalytics == false)
				return;

			var context = sender.Context as HttpContext;
			if (context == null || context.Request == null)
				return;

			var path = context.Request.Path;
			if (path[path.Length - 1] == '/')
				path = path.Substring(0, path.Length - 1);

			if(path == analyticsEndpointPath) {
				handleSessionDataRequest(context);
			}
		}

		private void handleSessionDataRequest(HttpContext context) {
			var op = context.Request.QueryString["op"];
			if (op.ToLower() == "getsession") {
				context.Response.Write(GetSession(context.Request.QueryString["sessionID"]));
			}

			if (op.ToLower() == "getactivity") {
				context.Response.Write(GetActivity(
					context.Request.QueryString["startDate"],
					context.Request.QueryString["endDate"]));
			}

			context.Response.End();
		}

		/// <summary>
		/// Application lifetime event used to trace the user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Context_AcquireRequestState(dynamic sender, EventArgs e) {
			if (logAnalytics == false)
				return;

			if (HttpContext.Current != null && HttpContext.Current.Session == null)
				return;

			var Session = HttpContext.Current.Session;
			if (Session == null)
				return;

			try {
				//Create a new session ID if none exists for this session
				if (Session["analyticsid"] == null)
					Session["analyticsid"] = Guid.NewGuid().ToString();

				var sessionID = (string)Session["analyticsid"];

				HttpRequest req = sender.Request;
				var agent = req.UserAgent;

				//Convert request queries to string
				var query = req.QueryString;
				var queryStr = "?";
				foreach (var key in query.Keys) {
					queryStr += $"{key}={query[(string)key]}&";
				}

				//Trim off the last & in query string
				if (query.Count == 0) {
					queryStr = "";
				} else {
					queryStr = queryStr.Substring(0, queryStr.Length - 1);
				}

				//Trace this event
				TraceLog.Trace(new Common.Models.Reporting.Analytics.TraceDataModel() {
					sessionid = sessionID,
					method = req.HttpMethod,
					path = req.Path.ToString(),
					ipaddress = req.UserHostAddress,
					query = queryStr,
				});
			} catch (Exception exc) {
				try {
					ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));
				} catch {
					#if DEBUG
					throw exc;
					#endif
				}
			}
		}

		/// <summary>
		/// Application lifetime event called when an unhandled exception occurs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Context_Error(dynamic sender, EventArgs e) {
			if (logExceptions == false)
				return;

			Exception exc = null;

			try {
				//Retrieve the exception from the "Server"
				var sv = (HttpServerUtility)sender.Server;
				exc = sv.GetLastError();

				if (exc == null)
					return;

				//We want the inner exception of unhandled exceptions
				var httpExc = exc as System.Web.HttpUnhandledException;
				if (exc != null)
					exc = exc.InnerException;

				//Handle http exceptions if we don't want to log them
				bool isHttpException = exc.GetType() == typeof(HttpException);
				if (isHttpException) {
					if (handleHttpExceptions == false) {
						if (redirectHttpExceptions) {
							sv.Transfer(redirectHttpExceptionPath);
						} else {
							sv.ClearError();
							return;
						}
					}
				}

				//Log exception
				if (exc != null) {
					if(redirectExceptions == false && redirectHttpExceptions == false) {
						//Fire and forget if we don't need to handle the error on the clientside
						Task.Run(async () => await ExceptionLog.LogExceptionAsync(exc, getMetaData(HttpContext.Current)));
					} else {
						ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));
					}
				}

				//Transfer to http redirect if needed
				if (isHttpException && redirectHttpExceptions && handleHttpExceptions)
					sv.Transfer(redirectHttpExceptionPath);

				//Transfer to redirect if needed
				if (redirectExceptions)
					sv.Transfer(redirectExceptionPath);

				//Clear error if no redirect, as we have handled it.
				sv.ClearError();
			} catch (Exception exc2) {
				try {
					//Attempt to log exception if something goes wrong
					ExceptionLog.LogException(exc2, new Dictionary<string, string>());
				} catch {
					throw exc2;
				}
			}
		}

		/// <summary>
		/// Creates a metadata dictionary using the given HttpContext instance.
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		private Dictionary<string, string> getMetaData(HttpContext ctx) {
			if (ctx == null)
				return new Dictionary<string, string>();

			Dictionary<string, string> ret = new Dictionary<string, string>();
			if (ctx.User != null) {
				if (ctx.User.Identity != null) {
					ret.Add("User Identity Name", ctx.User.Identity.Name ?? "Null");
					ret.Add("User Identity Auth Type", ctx.User.Identity.AuthenticationType ?? "Null");
					ret.Add("User Identity Authenticated", ctx.User.Identity.IsAuthenticated.ToString());
				}
			}

			if (ctx.Session != null && ctx.Session["analyticsid"] != null) {
				ret.Add("Session ID", (string)ctx.Session["analyticsid"]);
			}

			if (ctx.Request != null) {
				StringBuilder sb = new StringBuilder();
				foreach (var h in ctx.Request.Headers.Keys)
					sb.AppendLine($"{h}: {ctx.Request.Headers[(string)h]}");

				ret.Add("Request Method", ctx.Request.HttpMethod);
				ret.Add("Request Headers", sb.ToString());

				var queryString = ctx.Request.QueryString.ToString();
				ret.Add("Request Query", string.IsNullOrEmpty(queryString) ? "(blank)" : queryString);
			}

			return ret;
		}

		/// <summary>
		/// Returns a trace report containing the given session's trace data.
		/// </summary>
		/// <param name="sessionID"></param>
		/// <returns></returns>
		protected string GetSession(string sessionID) {
			try {
				var model = new TraceReportDataModel();
				DateTime startDateTime, endDateTime;
				startDateTime = DateTime.Now.AddDays(-7);
				endDateTime = DateTime.Now;

				var set = TraceLog.GetTraceLog(startDateTime, endDateTime);
				model.startTime = startDateTime;
				model.endTime = endDateTime;
				model.traceMap = new Dictionary<string, List<TraceDataModel>>();
				model.traceMap.Add(sessionID, new List<TraceDataModel>());
				foreach (var trace in set)
					if (trace.sessionid == sessionID)
						model.traceMap[sessionID].Add(trace);

				model.traceMap[sessionID].OrderBy((t) => t.timestamp).ToList();
				var raw = model.toRaw();
				return Newtonsoft.Json.JsonConvert.SerializeObject(raw);
			} catch (Exception exc) {
				if (string.IsNullOrEmpty(ReportingConfig.baseURI) == false)
					ExceptionLog.LogException(exc);

				return "";
			}
		}

		/// <summary>
		/// Returns a trace report containing trace data for all sessions in the given timeframe.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		protected string GetActivity(string startDate, string endDate) {
			try {
				var model = new TraceReportDataModel();
				DateTime startDateTime, endDateTime;
				if (DateTime.TryParse(startDate, out startDateTime) && DateTime.TryParse(endDate, out endDateTime)) {
					model = TraceLog.GetReport(startDateTime, endDateTime);
					model.startTime = startDateTime;
					model.endTime = endDateTime;
				}

				return model.toRaw();
			} catch (Exception exc) {
				if (string.IsNullOrEmpty(ReportingConfig.baseURI) == false)
					ExceptionLog.LogException(exc);

				return "";
			}
		}
	}
}
