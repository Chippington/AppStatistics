using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCore.Models {
	public class ApplicationViewModel {
		public ApplicationDataModel source;
		public TrafficDataModel traffic;
		public List<ExceptionDataModel> latestExceptions;

		public ApplicationViewModel() {
			latestExceptions = new List<ExceptionDataModel>();
		}

		public ApplicationViewModel(ApplicationDataModel source) {
			this.source = source;

			latestExceptions = new List<ExceptionDataModel>();
		}

		public ApplicationViewModel(ApplicationDataModel source, int excCount) {
			this.source = source;

			latestExceptions = Config.store.getExceptions(source).OrderBy(e => e.timeStamp).Reverse().Take(5).ToList();
		}
	}
}