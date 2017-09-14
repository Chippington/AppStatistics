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
				if (exc != null)
					ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));

				//Transfer to http redirect if needed
				if (isHttpException && redirectHttpExceptions)
					sv.Transfer(redirectHttpExceptionPath);

				//Transfer to redirect if needed
				if (redirectExceptions)
					sv.Transfer(redirectExceptionPath);

				//Clear error if no redirect, as we have handled it.
				sv.ClearError();
			} catch (Exception exc2) {
				try {
					//Attempt to log exception if something goes wrong
					ExceptionLog.LogException(exc, new Dictionary<string, string>());
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
	}
}
