using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppStatisticsCommon.Core.Reporting.Exceptions;
using AppStatisticsCommon.Core.Reporting.Analytics;
using AppStatisticsCommon.Core.Reporting;
using AppStatisticsCommon.Reporting.Exceptions;
using AppStatisticsCommon.Reporting.Analytics;
using AppStatisticsCommon.Reporting;

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