using AppStatisticsCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCore {
	public static class Config {
		public static IDataStore store = new JsonDataStore();
	}
}