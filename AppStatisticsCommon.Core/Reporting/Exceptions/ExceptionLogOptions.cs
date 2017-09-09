using AppStatisticsCommon.Models.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatisticsCommon.Core.Reporting.Exceptions {
	public class ExceptionLogOptions {
		public string handlerPath;
	}

	public class LogOptionsBuilder {
		public ExceptionLogOptions options;
		public LogOptionsBuilder() {
			options = new ExceptionLogOptions();
		}

		public void UseCustomErrorHandlingPath(string path) {
			options.handlerPath = path;
		}
	}
}
