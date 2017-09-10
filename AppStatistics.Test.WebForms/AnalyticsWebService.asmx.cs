using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Reporting.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace AppStatistics.Test.WebForms {
	/// <summary>
	/// Summary description for AnalyticsWebService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class AnalyticsWebService : System.Web.Services.WebService {

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public object GetActivity(string startDate, string endDate) {
			var model = new TraceReportDataModel();
			DateTime startDateTime, endDateTime;
			if(DateTime.TryParse(startDate, out startDateTime) && DateTime.TryParse(endDate, out endDateTime)) {
				model = TraceLog.GetReport(startDateTime, endDateTime);
				model.startTime = startDateTime;
				model.endTime = endDateTime;
			}

			return model.toRaw();
		}
	}
}
