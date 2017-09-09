using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Core.Reporting {
	public class ReportingOptions {
		public string baseURI;
		public string applicationID;
		public string contentFolderPath;
	}

	public class ReportingOptionsBuilder {
		internal ReportingOptions options;

		public ReportingOptionsBuilder() {
			options = new ReportingOptions();
		}

		public void UseContentFolderPath(string path) {
			options.contentFolderPath = path;
		}

		public void UseAPI(string baseURI, string applicationID) {
			options.baseURI = baseURI;
			options.applicationID = applicationID;
		}
	}
}
