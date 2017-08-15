using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStatistics.Data;
using System.IO;

namespace AppStatistics.Models {
	public class ApplicationModel : ModelBase {
		public string applicationName;
		public List<ExceptionModel> exceptionList;

		public ApplicationModel(string name) {
			applicationName = name;
			exceptionList = new List<ExceptionModel>();
		}

		public override void writeTo(IDataBuffer buffer) {
			base.writeTo(buffer);
			buffer.write((string)applicationName);

			buffer.write((int)exceptionList.Count);
			for(int i = 0; i < exceptionList.Count; i++) {
				exceptionList[i].writeTo(buffer);
			}
		}

		public override void readFrom(IDataBuffer buffer) {
			base.readFrom(buffer);
			applicationName = buffer.readString();

			exceptionList = new List<ExceptionModel>();
			var ct = buffer.readInt32();
			for(int i = 0; i < ct; i++) {
				ExceptionModel m = new ExceptionModel();
				m.readFrom(buffer);
				exceptionList.Add(m);
			}
		}

		public void save(string fname) {
			DataBufferStream buffer = new DataBufferStream();
			writeTo(buffer);

			buffer.writeToFile(Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fname)));
		}

		public void load(string fname) {
			exceptionList = new List<ExceptionModel>();

			DataBufferStream buffer = new DataBufferStream();
			buffer.readFromFile(Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fname)));

			buffer.seek(0);
			readFrom(buffer);
		}

		public void dispose() {
			exceptionList.Clear();
			exceptionList = null;
		}

		public override object getRaw() {
			return new {
				Name = applicationName,
				GUID = guid,
				Exceptions = (from exc in exceptionList
							  select exc.getRaw()).ToArray(),
			};
		}
	}
}