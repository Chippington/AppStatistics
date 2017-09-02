using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Data;

namespace AppStatistics.Models
{
    public class ReportModel : ModelBase
    {
		public DateTime startTime;
		public DateTime endTime;

		public List<ExceptionModel> exceptions;

		public override void writeTo(IDataBuffer buffer) {
			base.writeTo(buffer);
			buffer.write((string)startTime.ToString());
			buffer.write((string)endTime.ToString());

			buffer.write((int)exceptions.Count);
			for(int i = 0; i < exceptions.Count; i++) {
				exceptions[i].writeTo(buffer);
			}
		}

		public override void readFrom(IDataBuffer buffer) {
			base.readFrom(buffer);
			startTime = DateTime.Parse(buffer.readString());
			endTime = DateTime.Parse(buffer.readString());

			exceptions = new List<ExceptionModel>();
			var ct = buffer.readInt32();
			for(int i = 0; i < ct; i++) {
				ExceptionModel m = new ExceptionModel();
				m.readFrom(buffer);
				exceptions.Add(m);
			}
		}
	}
}
