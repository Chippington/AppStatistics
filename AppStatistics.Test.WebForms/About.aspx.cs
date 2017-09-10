using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppStatistics.Test.WebForms {
	public partial class About : Page {
		protected void Page_Load(object sender, EventArgs e) {
			throw new Exception("Test trace exception");
		}
	}
}