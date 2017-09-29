using AppStatistics.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Core {
	public static class Config {
		//public static IDataStore store = new JsonDataStore(Directory.GetCurrentDirectory() + "\\Content\\Data");
		public static IDataStore store = new SqliteDataStore();
	}
}