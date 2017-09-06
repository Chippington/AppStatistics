using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppStatisticsCommon.Logging;

namespace Microsoft.AspNetCore.Builder {
	public static class ExceptionLoggingExtensions {
		public static IApplicationBuilder UseExceptionLogging(this IApplicationBuilder app, Action<LogOptionsBuilder> builder) {
			var optionsBuilder = new LogOptionsBuilder();
			builder.Invoke(optionsBuilder);

			Log.options = optionsBuilder.options;
			app.UseExceptionHandler(new ExceptionHandlerOptions() {
				ExceptionHandlingPath = optionsBuilder.options.handlerPath,
			});

			return app.UseMiddleware<LoggingMiddleware>(optionsBuilder);
		}
	}
}