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
	public static class ReportingExtensions {
		public static IApplicationBuilder UseExceptionReporting(this IApplicationBuilder app, Action<LogOptionsBuilder> builder) {
			var optionsBuilder = new LogOptionsBuilder();
			builder.Invoke(optionsBuilder);

			ExceptionLogMiddleware.options = optionsBuilder.options;
			return app.UseMiddleware<ExceptionLogMiddleware>();
			

			//return app.UseExceptionHandler(new ExceptionHandlerOptions() {
			//	ExceptionHandlingPath = optionsBuilder.options.handlerPath,
			//});
		}

		public static IApplicationBuilder UseAnalyticsReporting(this IApplicationBuilder app, Action<AnalyticsOptionsBuilder> builder) {
			var optionsBuilder = new AnalyticsOptionsBuilder();
			builder.Invoke(optionsBuilder);

			AnalyticsMiddleware.options = optionsBuilder.options;
			return app.UseMiddleware<AnalyticsMiddleware>();
		}

		public static IApplicationBuilder UseReportingServices(this IApplicationBuilder app, Action<ReportingOptionsBuilder> builder) {
			var optionsBuilder = new ReportingOptionsBuilder();
			builder.Invoke(optionsBuilder);

			ReportingConfig.baseURI = optionsBuilder.options.baseURI;
			ReportingConfig.applicationID = optionsBuilder.options.applicationID;
			ReportingConfig.contentFolderPath = optionsBuilder.options.contentFolderPath;
			app.UseSession();
			return app;
		}
	}
}

namespace Microsoft.Extensions.DependencyInjection {
	public static class ReportingExtensions {

		public static IServiceCollection AddReportingServices(this IServiceCollection services) {
			services.AddDistributedMemoryCache();
			services.AddSession(options => {
				options.IdleTimeout = TimeSpan.FromSeconds(10);
				options.Cookie.HttpOnly = true;
			});
			return services;
		}
	}
}