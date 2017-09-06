﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppStatisticsCommon.Models.Reporting.Exceptions {
	public class ExceptionDataModel : ModelBase {
		public string message;
		public string stackTrace;
		public int hresult;

		public List<ExceptionDataModel> innerExceptions;

		public ExceptionDataModel() {
			innerExceptions = new List<ExceptionDataModel>();
		}

		public ExceptionDataModel(Exception src, string appid) {
			innerExceptions = new List<ExceptionDataModel>();
			message = src.Message;
			stackTrace = src.StackTrace;
			hresult = src.HResult;

			List<ExceptionDataModel> inner = new List<ExceptionDataModel>();
			Exception current = src.InnerException;
			while (current != null) {
				inner.Add(new ExceptionDataModel(current, appid));
				current = current.InnerException;
			}

			innerExceptions = inner;
			applicationID = appid;
		}

		public Dictionary<string, string> metadata;
		public string applicationID;
		public DateTime timeStamp;

		public override object toRaw() {
			return new {
				MetaData = Newtonsoft.Json.JsonConvert.SerializeObject(metadata),
				TimeStamp = timeStamp,
				Message = message,
				StackTrace = stackTrace,
				HResult = hresult,
				InnerExceptions = innerExceptions,
				ApplicationID = applicationID,
			};
		}

		public override void fromRaw(dynamic data) {
			metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((string)data.MetaData);
			timeStamp = data.TimeStamp;
			message = data.Message;
			stackTrace = data.StackTrace;
			hresult = data.HResult;
			innerExceptions = ((JArray)data.InnerExceptions).ToObject<List<ExceptionDataModel>>();
			applicationID = data.applicationID;
		}
	}
}