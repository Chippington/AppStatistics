using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Core.Reporting.Exceptions {
	public class ExceptionLogOptions {
		public Action<ExceptionDataModel> customHandlerAction;
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

		public void UseCustomErrorHandlerAction(Action<ExceptionDataModel> func) {
			options.customHandlerAction = func;
		}
	}
}
