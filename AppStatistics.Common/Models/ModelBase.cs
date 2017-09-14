using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Common.Models {
	public class ModelBase {
		/// <summary>
		/// Internal stored guid
		/// </summary>
		private string _guid;
		
		/// <summary>
		/// Lazy load a new Guid if one isn't set
		/// </summary>
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

		/// <summary>
		/// Virtual method used to create a raw object for use with JSON Serialization.
		/// </summary>
		/// <returns></returns>
		public virtual dynamic toRaw() {
			return new { GUID = _guid };
		}

		/// <summary>
		/// Virtual method used to parse a raw object into the instance, for use with JSON.
		/// </summary>
		/// <param name="data"></param>
		public virtual void fromRaw(dynamic data) {
			_guid = data.GUID;
		}

		/// <summary>
		/// Virtual method used when creating new Guids, for custom identification.
		/// </summary>
		/// <returns></returns>
		protected virtual string newGuid() {
			return Guid.NewGuid().ToString();
		}
	}
}