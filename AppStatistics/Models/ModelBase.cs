using AppStatistics.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Models
{
    public class ModelBase : ISerializable
    {
		private string _guid;
		public string guid {
			get {
				if (_guid == null || _guid == string.Empty)
					_guid = Guid.NewGuid().ToString();

				return _guid;
			}

			set { }
		}

		public virtual object getRaw() {
			return new { GUID = _guid };
		}

		public virtual void readFrom(IDataBuffer buffer) {
			_guid = buffer.readString();
		}

		public virtual void writeTo(IDataBuffer buffer) {
			buffer.write((string)_guid);
		}
	}
}
