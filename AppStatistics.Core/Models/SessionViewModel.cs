using AppStatistics.Common.Models.Reporting.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Core.Models {
	public class SessionViewModel {
		public List<TraceDataModel> traceList;

		public SessionViewModel() {
			traceList = new List<TraceDataModel>();
		}
	}
}
