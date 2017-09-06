﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.IISIntegration;
using AppStatisticsCommon.Logging;
using System.IO;

namespace AppStatisticsCore {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
			if (Config.store.getApplication("root") == null)
				Config.store.addApplication(new AppStatisticsCommon.Models.Reporting.ApplicationDataModel() {
					applicationName = "Application Statistics",
					guid = "root",
				});
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddCors(options => options.AddPolicy("AllowCors", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			} else {
				app.UseExceptionLogging((b) => {
					b.useCustomErrorHandlingPath("/Home/Error");
					b.useIndirectTracing("root", "./");
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