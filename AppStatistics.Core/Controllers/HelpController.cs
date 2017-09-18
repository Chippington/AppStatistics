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
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingBaseUrl\" [span style='color:lightblue']value[/span]=\"[span id='_endpoint'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingApplicationID\" [span style='color:lightblue']value[/span]=\"[span id='_appid'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingBaseContentPath\" [span style='color:lightblue']value[/span]=\"[span id='_contentpath'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingAnalyticsEndpointPath\" [span style='color:lightblue']value[/span]=\"[span id='_endpointPath']/Analytics[/span]\"/>");

			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingLogAnalytics\" [span style='color:lightblue']value[/span]=\"true[span id='_logAnalytics'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingLogExceptions\" [span style='color:lightblue']value[/span]=\"true[span id='_logExceptions'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingRedirectExceptions\" [span style='color:lightblue']value[/span]=\"false[span id='_redirectExceptions'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingHandleHttpExceptions\" [span style='color:lightblue']value[/span]=\"false[span id='_handleHttpExceptions'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingRedirectHttpExceptions\" [span style='color:lightblue']value[/span]=\"[span id='_redirectHttpExceptions'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingRedirectExceptionsPath\" [span style='color:lightblue']value[/span]=\"[span id='_redirectExceptionsPath'][/span]\"/>");
			webconfig2.Add("  <[span style='color:deepskyblue']add[/span] [span style='color:lightblue']key[/span]=\"reportingRedirectHttpExceptionsPath\" [span style='color:lightblue']value[/span]=\"[span id='_redirectHttpExceptionsPath'][/span]\"/>");
			webconfig2.Add("<[span style='color:deepskyblue']/appSettings[/span]>");

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
