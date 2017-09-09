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
			if (Config.store.getApplication("root") == null)
				Config.store.addApplication(new ApplicationDataModel() {
					applicationName = "Application Statistics",
					guid = "root",
				});

			if (Config.store.getApplication("testapp") == null)
				Config.store.addApplication(new ApplicationDataModel() {
					applicationName = "Application Statistics",
					guid = "testapp",
				});

			if (Config.store.getApplication("testwebapp") == null)
				Config.store.addApplication(new ApplicationDataModel() {
					applicationName = "Application Statistics",
					guid = "testwebapp",
				});
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddCors(options => options.AddPolicy("AllowCors", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
			services.AddMvc();
			services.AddReportingServices();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			} else {
				app.UseExceptionReporting((b) => {
					b.UseCustomErrorHandlingPath("/Home/Error");
				});
			}

			app.UseStaticFiles();
			app.UseCors("AllowCors");
			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}