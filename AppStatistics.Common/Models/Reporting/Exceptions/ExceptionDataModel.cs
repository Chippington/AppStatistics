using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppStatistics.Common.Models.Reporting.Exceptions {
	public class ExceptionDataModel : ModelBase {
		/// <summary>
		/// Stack trace of the exception.
		/// </summary>
		public string stackTrace;

		/// <summary>
		/// Message of the exception.
		/// </summary>
		public string message;

		/// <summary>
		/// Severity (set by user)
		/// </summary>
		public int severity;

		/// <summary>
		/// HResult of the exception.
		/// </summary>
		public int hresult;

		/// <summary>
		/// Exception metadata table. This information is displayed on the UI end.
		/// </summary>
		public Dictionary<string, string> metadata;

		/// <summary>
		/// Application ID of the application this exception occurred in.
		/// </summary>
		public string applicationID;

		/// <summary>
		/// Timestamp of the exception.
		/// </summary>
		public DateTime timeStamp;

		/// <summary>
		/// Inner exceptions list
		/// </summary>
		public List<ExceptionDataModel> innerExceptions;

		/// <summary>
		/// Instantiates with default values.
		/// </summary>
		public ExceptionDataModel() {
			innerExceptions = new List<ExceptionDataModel>();
			metadata = new Dictionary<string, string>();
			timeStamp = DateTime.Now;
		}

		/// <summary>
		/// Creates a model from the given source.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="appid"></param>
		public ExceptionDataModel(Exception src) {
			metadata = new Dictionary<string, string>();
			innerExceptions = new List<ExceptionDataModel>();
			message = src.Message;
			stackTrace = src.StackTrace;
			hresult = src.HResult;
			timeStamp = DateTime.Now;

			List<ExceptionDataModel> inner = new List<ExceptionDataModel>();
			Exception current = src.InnerException;
			while (current != null) {
				inner.Add(new ExceptionDataModel(current));
				current = current.InnerException;
			}

			innerExceptions = inner;
		}

		/// <summary>
		/// Converts this instance into a raw object.
		/// </summary>
		/// <returns></returns>
		public override object toRaw() {
			return new {
				Message = message,
				HResult = hresult,
				Severity = severity,
				TimeStamp = timeStamp,
				StackTrace = stackTrace,
				ApplicationID = applicationID,
				InnerExceptions = innerExceptions,
				MetaData = Newtonsoft.Json.JsonConvert.SerializeObject(metadata),
			};
		}

		/// <summary>
		/// Parses a raw object into this instance.
		/// </summary>
		/// <param name="data"></param>
		public override void fromRaw(dynamic data) {
			message = data.Message;
			hresult = data.HResult;
			severity = data.Severity;
			timeStamp = data.TimeStamp;
			stackTrace = data.StackTrace;
			applicationID = data.ApplicationID;
			innerExceptions = ((JArray)data.InnerExceptions).ToObject<List<ExceptionDataModel>>();
			metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((string)data.MetaData);

			if (metadata == null)
				metadata = new Dictionary<string, string>();

			if (innerExceptions == null)
				innerExceptions = new List<ExceptionDataModel>();
		}
	}
}