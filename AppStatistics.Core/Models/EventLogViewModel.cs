using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Core.Models {
	public class EventLogViewModel {
		public List<EventDataModel> events;
		public ApplicationDataModel application;

		public EventLogViewModel() {
			events = new List<EventDataModel>();
		}
	}
}
