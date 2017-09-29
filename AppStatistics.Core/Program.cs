using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AppStatistics.Core.Data.Sqlite;

namespace AppStatistics.Core {
	public class Program {
		public static void Main(string[] args) {
			BuildWebHost2(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();

		public static IWebHost BuildWebHost2(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
			.UseKestrel()
			.UseContentRoot(Directory.GetCurrentDirectory())
			.UseIISIntegration()
			.UseStartup<Startup>()
			.Build();
	}
}