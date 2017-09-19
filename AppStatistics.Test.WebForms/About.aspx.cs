using AppStatistics.Common.Reporting.Events;
using AppStatistics.Common.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppStatistics.Test.WebForms {
	public partial class About : Page {
		protected void Page_Load(object sender, EventArgs e) {
			//EventLog.LogEvent("Y'all went to the ABOUT PAGE");
			var tmp = ExceptionLog.GetLastErrorCode();
		}
	}
}