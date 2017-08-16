using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Data;

namespace AppStatistics.Models {
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
		public override void writeTo(IDataBuffer buffer) {
			base.writeTo(buffer);
			buffer.write((string)timeStamp.ToString());

			Queue<ExceptionInfo> writeQueue = new Queue<ExceptionInfo>();
			Exception exc = exception;
			while(exc != null) {
				writeQueue.Enqueue(new ExceptionInfo(exc));
				exc = exc.InnerException;
			}

			writeQueue.Reverse();
			while(writeQueue.Count > 0) {
				exc = writeQueue.Dequeue();
				buffer.write((string)exc.Message);
				buffer.write((string)exc.StackTrace);
				buffer.write((string)exc.Source);
				buffer.write((int)exc.HResult);
			}
		}

		public override void readFrom(IDataBuffer buffer) {
			base.readFrom(buffer);
			timeStamp = DateTime.Parse(buffer.readString());

			int ct = buffer.readInt32();

			exception = null;
			for(int i = 0; i < ct; i++) {
				string message = buffer.readString();
				string stacktrace = buffer.readString();
				string source = buffer.readString();
				int hresult = buffer.readInt32();
				exception = new ExceptionInfo(message, stacktrace, source, hresult, exception);
			}
		}

		public override object getRaw() {
			Func<ExceptionInfo, object> inst = (info) => {
				return new {
					Message = info.Message,
					StackTrace = info.StackTrace,
					Source = info.Source,
				};
			};

			List<ExceptionInfo> exceptions = new List<ExceptionInfo>();
			var exc = exception.InnerException == null ? null : new ExceptionInfo(exception.InnerException);
			while(exc != null) {
				exceptions.Add(exc);
				exc = exc.InnerException == null ? null : new ExceptionInfo(exc.InnerException);
			}

			return new {
				Message = exception.Message,
				StackTrace = exception.StackTrace,
				HResult = exception.HResult,
				TimeStamp = timeStamp.ToString(),
				Exceptions = exceptions.ToArray(),
			};
		}
	}
}
