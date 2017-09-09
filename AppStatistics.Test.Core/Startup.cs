using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace AppStatistics.Test.Core {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddReportingServices();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			//app.UseExceptionHandler("/Home/Error");
			app.UseReportingServices((b) => {
				b.UseAPI("http://localhost/reporting/", "testwebapp");
				b.UseContentFolderPath(Directory.GetCurrentDirectory() + "\\Content\\Analytics");
			});

			app.UseExceptionReporting((b) => {
				b.UseCustomErrorHandlingPath("/Home/Error");
			});

			app.UseAnalyticsReporting((b) => {
			});

			//app.UseDeveloperExceptionPage();
			//app.UseBrowserLink();
			app.UseStaticFiles();

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}