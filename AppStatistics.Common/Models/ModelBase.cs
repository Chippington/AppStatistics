using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Common.Models {
	public class ModelBase {
		private string _guid;
		public string guid {
			get {
				if (_guid == null || _guid == string.Empty)
					_guid = newGuid();

				return _guid;
			}

			set {
				_guid = value;
			}
		}

		public virtual dynamic toRaw() {
			return new { GUID = _guid };
		}

		public virtual void fromRaw(dynamic data) {
			_guid = data.GUID;
		}

		protected virtual string newGuid() {
			return Guid.NewGuid().ToString();
		}
	}
}