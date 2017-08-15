using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Data {
	public interface ISerializable {
		void writeTo(IDataBuffer buffer);
		void readFrom(IDataBuffer buffer);
	}
}