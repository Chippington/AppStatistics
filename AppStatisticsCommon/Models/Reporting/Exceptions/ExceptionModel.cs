﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppStatisticsCommon.Models.Reporting.Exceptions {
	public class ExceptionModel : ModelBase {
		public string message;
		public string stackTrace;
		public int hresult;

		public List<ExceptionModel> innerExceptions;

		public ExceptionModel() {
			innerExceptions = new List<ExceptionModel>();
		}

		public ExceptionModel(Exception src, ApplicationModel app) {
			innerExceptions = new List<ExceptionModel>();
			message = src.Message;
			stackTrace = src.StackTrace;
			hresult = src.HResult;

			List<ExceptionModel> inner = new List<ExceptionModel>();
			Exception current = src.InnerException;
			while (current != null) {
				inner.Add(new ExceptionModel(current, app));
				current = current.InnerException;
			}

			innerExceptions = inner;
			application = app;
		}

		public ApplicationModel application;
		public DateTime timeStamp;

		public override object toRaw() {
			string timestampData = Newtonsoft.Json.JsonConvert.SerializeObject(timeStamp);
			return new {
				Message = message,
				StackTrace = stackTrace,
				HResult = hresult,
				InnerExceptions = innerExceptions,
				Application = application.toRaw(),
			};
		}

		public override void fromRaw(dynamic data) {
			message = data.Message;
			stackTrace = data.StackTrace;
			hresult = data.HResult;
			innerExceptions = ((JArray)data.InnerExceptions).ToObject<List<ExceptionModel>>();

			application = new ApplicationModel("");
			application.fromRaw(data.Application);
		}
	}
}