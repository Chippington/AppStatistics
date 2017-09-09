using AppStatistics.Common.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Common.Models.Reporting {
	public class ReportDataModel : ModelBase {
		public ApplicationDataModel application;
		public DateTime startTime;
		public DateTime endTime;
		public List<ExceptionDataModel> exceptions;

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
			application = new ApplicationDataModel();
			application.fromRaw(data.Application);

			startTime = data.StartTime;
			endTime = data.EndTime;

			exceptions = new List<ExceptionDataModel>();
			dynamic[] rawExceptionList = data.Exceptions;
			foreach (var d in rawExceptionList) {
				ExceptionDataModel exception = new ExceptionDataModel();
				exception.fromRaw(d);
				exceptions.Add(exception);
			}
		}
	}
}