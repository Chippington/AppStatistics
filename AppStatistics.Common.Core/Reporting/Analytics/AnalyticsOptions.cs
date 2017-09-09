using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Core.Reporting.Analytics
{
	public class AnalyticsOptions {
		public string contentFolder;
	}

	public class AnalyticsOptionsBuilder {
		public AnalyticsOptions options;

		public void UseFilePath(string contentFolder) {
			options.contentFolder = contentFolder;
		}
	}
}
