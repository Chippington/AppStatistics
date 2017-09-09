using System;

namespace AppStatisticsTest {
	class Program {
		static void Main(string[] args) {
			System.Threading.Thread.Sleep(5000);
			//AppStatisticsCommon.Reporting.Exceptions.Log.Configure(new AppStatisticsCommon.Reporting.Exceptions.LogOptions() {
			//	baseURI = "http://localhost/",
			//	application = new AppStatisticsCommon.Models.Reporting.ApplicationDataModel() {
			//		guid = "0f0ee0cd-15c8-4f7b-b0b7-bb8794b3735a",
			//	},
			//});

			try {
				f5();
			} catch (Exception exc) {
				AppStatisticsCommon.Core.Reporting.Exceptions.ExceptionLog.LogException(exc, new System.Collections.Generic.Dictionary<string, string>() {
					{ "Test key 1", "Test value 1"},
					{ "Test key 2", "Test value 2"},
					{ "Test key 3", "Test value 3"},
				}).Wait();
			}

			Console.WriteLine("Hello World!");
			Console.ReadLine();
		}

		public static void f1() {
			throw new Exception("Test Exception with metadata", new Exception("Inner Exception 1", new Exception("Inner Exception 2")));
		}
		public static void f2() => f1();
		public static void f3() => f2();
		public static void f4() => f3();
		public static void f5() => f4();
	}
}