using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppStatistics.Common.Core.Reporting.Exceptions;
using AppStatistics.Common.Core.Reporting.Analytics;
using AppStatistics.Common.Core.Reporting;
using AppStatistics.Common.Reporting.Exceptions;
using AppStatistics.Common.Reporting.Analytics;
using AppStatistics.Common.Reporting;

namespace Microsoft.AspNetCore.Builder {
	public static class ExceptionLoggingExtensions {
		public static IApplicationBuilder UseExceptionReporting(this IApplicationBuilder app, Action<LogOptionsBuilder> builder) {
			var optionsBuilder = new LogOptionsBuilder();
			builder.Invoke(optionsBuilder);

			ExceptionLogMiddleware.options = optionsBuilder.options;

			app.UseExceptionHandler(new ExceptionHandlerOptions() {
				ExceptionHandlingPath = optionsBuilder.options.handlerPath,
			});

			return app.UseMiddleware<ExceptionLogMiddleware>(optionsBuilder);
		}

		public static IApplicationBuilder UseTrafficReporting(this IApplicationBuilder app, Action<TrafficOptionsBuilder> builder) {
			var optionsBuilder = new TrafficOptionsBuilder();
			builder.Invoke(optionsBuilder);

			AnalyticsMiddleware.options = optionsBuilder.options;
			return app.UseMiddleware<AnalyticsMiddleware>(optionsBuilder);
		}

		public static IApplicationBuilder UseReportingServices(this IApplicationBuilder app, Action<ReportingOptionsBuilder> builder) {
			var optionsBuilder = new ReportingOptionsBuilder();
			builder.Invoke(optionsBuilder);

			ReportingConfig.baseURI = optionsBuilder.options.baseURI;
			ReportingConfig.applicationID = optionsBuilder.options.applicationID;
			ReportingConfig.contentFolderPath = optionsBuilder.options.contentFolderPath;
			return app;
		}
	}
}