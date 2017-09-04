using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCore.Models {
	public class ApplicationViewModel {
		public ApplicationModel source;
		public List<ExceptionModel> latestExceptions;
	}
}