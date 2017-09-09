using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Reporting.Analytics
{
	public class AnalyticsOptions {
		public string contentFolderPath;
	}

	public class TrafficOptionsBuilder {
		public AnalyticsOptions options;

		public void UseFilePath(string contentFolderPath) {
			options.contentFolderPath = contentFolderPath;
		}
	}
}
