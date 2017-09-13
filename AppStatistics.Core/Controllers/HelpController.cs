using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AppStatistics.Core.Controllers {
	public class HelpController : Controller {
		public IActionResult Index() {
			return View();
		}

		public IActionResult WebForms() {
			List<string> webconfig1 = new List<string>();
			webconfig1.Add("<[span style='color:deepskyblue']system.webServer[/span]>");
			webconfig1.Add("  <[span style='color:deepskyblue']modules[/span]>");
			webconfig1.Add("      <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']name[/span]=\"AnalyticsModule\" [span style='color:lightblue']type[/span]=\"AppStatistics.Common.WebForms.AnalyticsModule\"/>");
			webconfig1.Add("  <[span style='color:deepskyblue']/modules[/span]>");
			webconfig1.Add("<[span style='color:deepskyblue']/system.webServer[/span]>");

			string output = "";
			for (int i = 0; i < webconfig1.Count; i++) {
				var str = webconfig1[i];
				str = str.Replace("<", "&lt;")
					.Replace(">", "&gt;")
					.Replace("\r\n", "")
					.Replace("[", "<")
					.Replace("]", ">")
					.Replace("  ", "&nbsp;&nbsp;");

				output += str + "<br />";
			}

			ViewData["webconfig1"] = output;

			List<string> webconfig2 = new List<string>();
			webconfig2.Add("<[span style='color:deepskyblue']appSettings[/span]>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"baseUrl\" [span style='color:lightblue']value[/span]=\"[span id='_endpoint'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"applicationID\" [span style='color:lightblue']value[/span]=\"[span id='_appid'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"contentPath\" [span style='color:lightblue']value[/span]=\"[span id='_contentpath'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"endpointPath\" [span style='color:lightblue']value[/span]=\"[span id='_endpointPath'][/span]\"/>");
			webconfig2.Add("<[span style='color:deepskyblue']/appSettings[/span]>");
			webconfig2.Add("[br /]");
			webconfig2.Add("<[span style='color:deepskyblue']system.web[/span]>");
			webconfig2.Add("  <[span style='color:deepskyblue']webServers[/span]>");
			webconfig2.Add("    <[span style='color:deepskyblue']protocols[/span]>");
			webconfig2.Add("      <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']name[/span]=\"HttpGet\"/>");
			webconfig2.Add("    <[span style='color:deepskyblue']/protocols[/span]>");
			webconfig2.Add("  <[span style='color:deepskyblue']/webServers[/span]>");
			webconfig2.Add("<[span style='color:deepskyblue']/system.web[/span]>");

			output = "";
			for (int i = 0; i < webconfig2.Count; i++) {
				var str = webconfig2[i];
				str = str.Replace("<", "&lt;")
					.Replace(">", "&gt;")
					.Replace("\r\n", "")
					.Replace("[", "<")
					.Replace("]", ">")
					.Replace("  ", "&nbsp;&nbsp;");

				output += str + "<br />";
			}

			ViewData["webconfig2"] = output;

			return View();
		}
	}
}
