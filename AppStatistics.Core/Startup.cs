using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.IISIntegration;
using AppStatistics.Common.Reporting.Exceptions;
using AppStatistics.Common.Models.Reporting;

namespace AppStatistics.Core {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddCors(options => options.AddPolicy("AllowCors", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
			services.AddReportingServices();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			app.UseReportingServices((b) => {
				b.UseAPI("/", "root");
				b.UseContentFolderPath(Directory.GetCurrentDirectory() + "\\Content\\Analytics");
			});

			app.UseExceptionReporting((b) => {
				b.UseCustomErrorHandlingPath("/Home/Error");
				b.UseCustomErrorHandlerAction((exc) => {
					exc.applicationID = "root";
					Config.store.AddException("root", exc);
				});
			});

			//app.UseAnalyticsReporting((b) => {
			//});

			//if (env.IsDevelopment()) {
			//	app.UseDeveloperExceptionPage();
			//	app.UseBrowserLink();
			//} else {
			//	app.UseExceptionReporting((b) => {
			//		b.UseCustomErrorHandlingPath("/Home/Error");
			//	});
			//}

			app.UseStaticFiles();
			app.UseCors("AllowCors");
			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			if (Config.store.GetApplication("root") == null)
				Config.store.AddApplication(new ApplicationDataModel() {
					applicationName = "Application Statistics",
					guid = "root",
				});

			//if (Config.store.GetApplication("testapp") == null)
			//	Config.store.AddApplication(new ApplicationDataModel() {
			//		applicationName = "Test Application",
			//		guid = "testapp",
			//	});

			//if (Config.store.GetApplication("testwebapp") == null)
			//	Config.store.AddApplication(new ApplicationDataModel() {
			//		applicationName = "Test ASP.NET Core 2.0 Application",
			//		guid = "testwebapp",
			//	});

			//if (Config.store.GetApplication("testwebapp2") == null)
			//	Config.store.AddApplication(new ApplicationDataModel() {
			//		applicationName = "Test ASP.NET WebForms Application",
			//		guid = "testwebapp2",
			//	});
		}
	}
}