using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

namespace AppStatistics.Common.WebForms {
	public class AnalyticsModule : IHttpModule {
		/// <summary>
		/// You will need to configure this module in the Web.config file of your
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: https://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpModule Members

		public void Dispose() {
			//clean-up code here.
		}

		public void Init(HttpApplication context) {
			// Below is an example of how you can handle LogRequest event and provide 
			// custom logging implementation for it
			AppStatistics.Common.Reporting.ReportingConfig.contentFolderPath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["contentPath"];
			AppStatistics.Common.Reporting.ReportingConfig.applicationID = ConfigurationManager.AppSettings["applicationID"];
			AppStatistics.Common.Reporting.ReportingConfig.baseURI = ConfigurationManager.AppSettings["baseUrl"];

			context.Error += Context_Error;
			context.AcquireRequestState += Context_AcquireRequestState;
			var test = ConfigurationManager.AppSettings["customsetting1"];
		}

		private void Context_AcquireRequestState(dynamic sender, EventArgs e) {
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
				ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));
			}
		}

		private void Context_Error(dynamic sender, EventArgs e) {
			var sv = (HttpServerUtility)sender.Server;
			var exc = sv.GetLastError();

			if (exc == null)
				return;

			if (exc.GetType() == typeof(HttpException)) {
				// The Complete Error Handling Example generates
				// some errors using URLs with "NoCatch" in them;
				// ignore these here to simulate what would happen
				// if a global.asax handler were not implemented.
				if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
					return;

				//Redirect HTTP errors to HttpError page
				sv.Transfer("HttpErrorPage.aspx");
			}

			// Log the exception and notify system operators
			var httpExc = exc as System.Web.HttpUnhandledException;
			if (exc != null)
				exc = exc.InnerException;


			if (exc != null)
				ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));

			// Clear the error from the server
			sv.ClearError();
		}

		#endregion

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
