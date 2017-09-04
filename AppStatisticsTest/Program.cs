using System;

namespace AppStatisticsTest {
	class Program {
		static void Main(string[] args) {
			System.Threading.Thread.Sleep(5000);
			AppStatisticsCommon.Logging.Logger.Configure("http://localhost:8964", new AppStatisticsCommon.Models.Reporting.ApplicationModel("Test Application") {
				guid = "testapp",
			});

			try {
				f5();
			} catch (Exception exc) {
				AppStatisticsCommon.Logging.Logger.LogException(exc).Wait();
			}

			Console.WriteLine("Hello World!");
			Console.ReadLine();
		}

		public static void f1() {
			throw new Exception("Test Exception", new Exception("Inner Exception 1", new Exception("Inner Exception 2")));
		}
		public static void f2() => f1();
		public static void f3() => f2();
		public static void f4() => f3();
		public static void f5() => f4();
	}
}