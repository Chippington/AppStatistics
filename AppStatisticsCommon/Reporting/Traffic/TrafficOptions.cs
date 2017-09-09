using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Reporting.Traffic
{
	public class TrafficOptions {
		public string contentFolderPath;
	}

	public class TrafficOptionsBuilder {
		public TrafficOptions options;

		public void UseFilePath(string contentFolderPath) {
			options.contentFolderPath = contentFolderPath;
		}
	}
}
