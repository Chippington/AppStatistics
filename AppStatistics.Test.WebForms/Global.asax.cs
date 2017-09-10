using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AppStatistics.Test.WebForms {
	public class Global : HttpApplication {
		void Application_Start(object sender, EventArgs e) {
			AppStatistics.Common.Reporting.ReportingConfig.applicationID = "testwebapp2";
			AppStatistics.Common.Reporting.ReportingConfig.contentFolderPath = HttpRuntime.AppDomainAppPath + "Content";
			AppStatistics.Common.Reporting.ReportingConfig.baseURI = "http://localhost:14286/";
			// Code that runs on application startup
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		void Application_Error(object sender, EventArgs e) {
			// Code that runs when an unhandled error occurs

			// Get the exception object.
			Exception exc = Server.GetLastError();

			// Handle HTTP errors
			if (exc.GetType() == typeof(HttpException)) {
				// The Complete Error Handling Example generates
				// some errors using URLs with "NoCatch" in them;
				// ignore these here to simulate what would happen
				// if a global.asax handler were not implemented.
				if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
					return;

				//Redirect HTTP errors to HttpError page
				Server.Transfer("HttpErrorPage.aspx");
			}

			// For other kinds of errors give the user some information
			// but stay on the default page
			Response.Write("<h2>Global Page Error</h2>\n");
			Response.Write(
				"<p>" + exc.Message + "</p>\n");
			Response.Write("Return to the <a href='Default.aspx'>" +
				"Default Page</a>\n");

			// Log the exception and notify system operators
			var httpExc = exc as System.Web.HttpUnhandledException;
			if (exc != null)
				exc = exc.InnerException;


			if(exc != null)
				ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));

			// Clear the error from the server
			Server.ClearError();
		}

		void Application_AcquireRequestState(dynamic sender, EventArgs e) {
			if (HttpContext.Current != null && HttpContext.Current.Session == null)
				return;

			try {
				if (Session["analyticsid"] == null)
					Session["analyticsid"] = Guid.NewGuid().ToString();

				var sessionID = (string)Session["analyticsid"];// HttpContext.Current.Session.SessionID.ToString();
				System.Web.HttpRequest req = sender.Request;
				TraceLog.Trace(req.Path.ToString(), req.HttpMethod, sessionID, req.UserHostAddress);
			} catch (Exception exc) {
				ExceptionLog.LogException(exc, getMetaData(HttpContext.Current));
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