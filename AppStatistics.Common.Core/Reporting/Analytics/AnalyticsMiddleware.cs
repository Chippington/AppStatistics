using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting.Exceptions;

namespace AppStatistics.Common.Core.Reporting.Analytics {
	class AnalyticsMiddleware {
		internal static AnalyticsOptions options;
		private RequestDelegate _next;
		private ILogger _logger;

		public AnalyticsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) {
			_next = next;
			_logger = loggerFactory.CreateLogger<AnalyticsMiddleware>();
		}

		public async Task Invoke(HttpContext context) {
			try {
				var path = context.Request.Path.ToString();
				var method = context.Request.Method;
				var sessionid = context.Session.Id;
				var ipaddress = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
				var port = context.Connection.RemotePort.ToString();

				//TraceLog.Trace(path, method, sessionid, $"{ipaddress}:{port}");
				var query = context.Request.QueryString;
				var queryMap = new Dictionary<string, string>();

				TraceLog.Trace(new Common.Models.Reporting.Analytics.TraceDataModel() {
					sessionid = sessionid,
					method = method,
					path = path,
					ipaddress = $"{ipaddress}:{port}",
					query = queryMap,
				});
			} catch (Exception ex) {
				throw ex;
			}

			await _next(context);
		}

		private Dictionary<string, string> getMetaData(HttpContext ctx) {
			Dictionary<string, string> ret = new Dictionary<string, string>();
			if (ctx.User != null) {
				if (ctx.User.Identity != null) {
					ret.Add("User Identity Name", ctx.User.Identity.Name ?? "Null");
					ret.Add("User Identity Auth Type", ctx.User.Identity.AuthenticationType.ToString());
					ret.Add("User Identity Authenticated", ctx.User.Identity.IsAuthenticated.ToString());
				}

				StringBuilder sb = new StringBuilder();
				foreach (var claim in ctx.User.Claims)
					sb.Append(claim + ", ");

				var claimStr = sb.ToString().Trim();
				claimStr = claimStr.Substring(0, claimStr.Length - 1);

				ret.Add("User Claims", claimStr);
			}

			if (ctx.Session != null) {
				ret.Add("Session ID", ctx.Session.Id);
			}

			if (ctx.Request != null) {
				StringBuilder sb = new StringBuilder();
				foreach (var h in ctx.Request.Headers.Keys)
					sb.AppendLine($"{h}: {ctx.Request.Headers[h]}");

				ret.Add("Request Method", ctx.Request.Method);
				ret.Add("Request Headers", sb.ToString());
				ret.Add("Request Query", ctx.Request.QueryString.ToString());
			}

			return ret;
		}
	}
}