using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCommon.Models.Reporting.Exceptions {
	public class ExceptionModel : ModelBase {
		public class ExceptionInfo : Exception {
			private string _stackTrace;
			private string _source;
			private int _hresult;

			public ExceptionInfo(Exception src) : base(src.Message, src.InnerException) {
				_stackTrace = src.StackTrace;
				_source = src.Source;
				_hresult = src.HResult;
			}

			public ExceptionInfo(string msg, string stck, string src, int hresult, Exception inner) : base(msg, inner) {
				_stackTrace = stck;
				_source = src;
				_hresult = hresult;
			}

			public override string StackTrace => _stackTrace;
			public override string Source => _source;
		}

		public ExceptionInfo exception;
		public DateTime timeStamp;

		public override object toRaw() {
			//I am way too lazy to do this properly
			string exceptionData = Newtonsoft.Json.JsonConvert.SerializeObject(exception);
			string timestampData = Newtonsoft.Json.JsonConvert.SerializeObject(timeStamp);
			return new {
				ExceptionJson = exceptionData,
				TimeStampJson = timestampData,
			};
		}

		public override void fromRaw(dynamic data) {
			exception = Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionInfo>(data.ExceptionJson);
			timeStamp = Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionInfo>(data.TimeStampJson);
		}
	}
}
