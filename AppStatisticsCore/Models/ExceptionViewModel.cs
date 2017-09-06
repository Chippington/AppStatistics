using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCore.Models {
	public class ExceptionViewModel {
		public ApplicationDataModel application;
		public ExceptionDataModel source;
	}
}