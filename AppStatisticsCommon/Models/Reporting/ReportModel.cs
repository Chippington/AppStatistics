using AppStatisticsCommon.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCommon.Models.Reporting {
	public class ReportModel : ModelBase {
		public ApplicationModel application;
		public DateTime startTime;
		public DateTime endTime;
		public List<ExceptionModel> exceptions;

		public override dynamic toRaw() {
			return new {
				Application = application.toRaw(),
				StartTime = startTime,
				EndTime = endTime,
				Exceptions = (from ex in exceptions
							  select ex.toRaw()).ToArray()
			};
		}

		public override void fromRaw(dynamic data) {
			application = new ApplicationModel();
			application.fromRaw(data.Application);

			startTime = data.StartTime;
			endTime = data.EndTime;

			exceptions = new List<ExceptionModel>();
			dynamic[] rawExceptionList = data.Exceptions;
			foreach (var d in rawExceptionList) {
				ExceptionModel exception = new ExceptionModel();
				exception.fromRaw(d);
				exceptions.Add(exception);
			}
		}
	}
}