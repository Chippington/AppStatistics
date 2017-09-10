using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Core.Models {
	public class ExceptionViewModel {
		public ApplicationDataModel application;
		public ExceptionDataModel source;
		public string sessionActionURI;
	}
}