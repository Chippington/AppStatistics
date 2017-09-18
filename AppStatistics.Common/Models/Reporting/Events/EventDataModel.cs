using System;
using System.Collections.Generic;
using System.Text;

namespace AppStatistics.Common.Models.Reporting.Events {
	public class EventDataModel : ModelBase {
		public Dictionary<string, string> metadata;
		public DateTime timestamp;
		public string applicationID;
		public string category;
		public string message;

		public EventDataModel() {
			this.metadata = new Dictionary<string, string>();
			this.message = "";
			this.category = "";
			this.timestamp = DateTime.Now;
		}

		public EventDataModel(string message) {
			this.metadata = new Dictionary<string, string>();
			this.message = message;
			this.timestamp = DateTime.Now;
			if (this.message == null)
				this.message = "";
		}

		public EventDataModel(string message, Dictionary<string, string> metadata) {
			this.metadata = metadata;
			this.message = message;
			this.timestamp = DateTime.Now;

			if (this.message == null)
				this.message = "";

			if (this.metadata == null)
				this.metadata = new Dictionary<string, string>();
		}

		public override dynamic toRaw() {
			return new {
				ApplicationID = applicationID,
				Message = message,
				TimeStamp = timestamp,
				MetaData = Newtonsoft.Json.JsonConvert.SerializeObject(metadata),
				Category = category,
			};
		}

		public override void fromRaw(dynamic data) {
			message = data.Message;
			timestamp = data.TimeStamp;
			applicationID = data.ApplicationID;
			metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((string)data.MetaData);
			category = data.Category;
		}
	}
}
