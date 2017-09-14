using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
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


		public void Dispose() {
			//clean-up code here.
		}

		public void Init(HttpApplication context) {
			logAnalytics = ConfigurationManager.AppSettings["reportingLogAnalytics"] == "true";
			logExceptions = ConfigurationManager.AppSettings["reportingLogExceptions"] == "true";
			if (logAnalytics == false && logExceptions == false)
				return;

			var root = HttpRuntime.AppDomainAppPath.Substring(0, HttpRuntime.AppDomainAppPath.Length - 1);
			var contentFolderPath = root + ConfigurationManager.AppSettings["contentPath"].Replace("/", "\\");
			var applicationID = ConfigurationManager.AppSettings["applicationID"];
			var baseURI = ConfigurationManager.AppSettings["baseUrl"];

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

			//Redirect settings
			redirectExceptions = ConfigurationManager.AppSettings["reportingRedirectExceptions"] == "true";
			handleHttpExceptions = ConfigurationManager.AppSettings["reportingHandleHttpExceptions"] == "true";
			redirectHttpExceptions = ConfigurationManager.AppSettings["reportingRedirectHttpExceptions"] == "true";

			//Redirect paths
			redirectExceptionPath = ConfigurationManager.AppSettings["reportingRedirectExceptionsPath"];
			redirectHttpExceptionPath = ConfigurationManager.AppSettings["reportingRedirectHttpExceptionsPath"];

			context.Error += Context_Error;
			context.AcquireRequestState += Context_AcquireRequestState;
			var test = ConfigurationManager.AppSettings["customsetting1"];

			if (File.Exists(root + "\\AnalyticsEndpoint.aspx") == false)
				File.WriteAllText(root + "\\AnalyticsEndpoint.aspx",
					"<%@ Page Language=\"C#\" AutoEventWireup=\"true\" CodeBehind=\"AnalyticsEndpoint.aspx.cs\" Inherits=\"AnalyticsEndpoint\" %>");
		}

		private void Context_AcquireRequestState(dynamic sender, EventArgs e) {
			if (logAnalytics == false)
				return;

			if (HttpContext.Current != null && HttpContext.Current.Session == null)
				return;

			var Session = HttpContext.Current.Session;
			if (Session == null)
				return;

			try {
				if (Session["analyticsid"] == null)
					Session["analyticsid"] = Guid.NewGuid().ToString();

				var sessionID = (string)Session["analyticsid"];// HttpContext.Current.Session.SessionID.ToString();
				System.Web.HttpRequest req = sender.Request;
				var query = req.QueryString;
				var queryMap = new Dictionary<string, string>();
				foreach (var key in query.Keys)
					queryMap.Add((string)key, query[(string)key]);

				var agent = req.UserAgent;
				TraceLog.Trace(new Common.Models.Reporting.Analytics.TraceDataModel() {
					sessionid = sessionID,
					method = req.HttpMethod,
					path = req.Path.ToString(),
					ipaddress = req.UserHostAddress,
					query = queryMap,
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

		private void Context_Error(dynamic sender, EventArgs e) {
			if (logExceptions == false)
				return;

			Exception exc = null;

			try {
				var sv = (HttpServerUtility)sender.Server;
				exc = sv.GetLastError();

				if (exc == null)
					return;

				var httpExc = exc as System.Web.HttpUnhandledException;
				if (exc != null)
					exc = exc.InnerException;

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

				if (exc != null)
					ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));

				if (isHttpException && redirectHttpExceptions)
					sv.Transfer(redirectHttpExceptionPath);

				if (redirectExceptions)
					sv.Transfer(redirectExceptionPath);

				sv.ClearError();
			} catch (Exception exc2) {
				try {
					ExceptionLog.LogException(exc, new Dictionary<string, string>() {

					});
				} catch {
					throw exc2;
				}
			}
		}

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
	}
}
