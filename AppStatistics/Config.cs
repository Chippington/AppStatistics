using AppStatistics.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics
{
    public static class Config
    {
		public static IDataStore store = new TestDataStore();
    }
}
