using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using AppStatistics.Common.Reporting.Exceptions;

namespace AppStatistics.Common.Core.Reporting.Exceptions {
	public class ExceptionLogMiddleware {
		internal static ExceptionLogOptions options;
		private RequestDelegate _next;
		private ILogger _logger;

		public ExceptionLogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) {
			_next = next;
			_logger = loggerFactory.CreateLogger<ExceptionLogMiddleware>();
		}

		public async Task Invoke(HttpContext context) {
			try {
				var handlerFeature = context.Features.Get<IExceptionHandlerFeature>();
				if (handlerFeature != null && handlerFeature.Error != null) {
					var task = Task.Run(async () => {
						await ExceptionLog.LogException(handlerFeature.Error, getMetaData(context));
					});
				}

				try {
					await _next(context);
				} catch (Exception exc) {
					await ExceptionLog.LogException(exc, getMetaData(context));
				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		private Dictionary<string, string> getMetaData(HttpContext ctx) {
			Dictionary<string, string> ret = new Dictionary<string, string>();
			if (ctx.User != null) {
				if (ctx.User.Identity != null) {
					ret.Add("User Identity Name", ctx.User.Identity.Name ?? "Null");
					ret.Add("User Identity Auth Type", ctx.User.Identity.AuthenticationType ?? "Null");
					ret.Add("User Identity Authenticated", ctx.User.Identity.IsAuthenticated.ToString());
				}

				StringBuilder sb = new StringBuilder();
				foreach (var claim in ctx.User.Claims)
					sb.Append(claim + ", ");

				var claimStr = sb.ToString().Trim();
				if(claimStr.Length > 2)
					claimStr = claimStr.Substring(0, claimStr.Length - 1);

				ret.Add("User Claims", string.IsNullOrEmpty(claimStr) ? "(blank)" : claimStr);
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

				var queryString = ctx.Request.QueryString.ToString();
				ret.Add("Request Query", string.IsNullOrEmpty(queryString) ? "(blank)" : queryString);
			}

			return ret;
		}
	}
}