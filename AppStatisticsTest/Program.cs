using System;

namespace AppStatisticsTest
{
    class Program
    {
        static void Main(string[] args)
        {
			System.Threading.Thread.Sleep(5000);
			AppStatisticsCommon.Logging.Logger.Configure("http://localhost:8964", new AppStatisticsCommon.Models.Reporting.ApplicationModel("Test Application") {
				guid = "0cedac24-b3e4-420f-8e25-8ba52703018c",
			});

			try {
				throw new Exception("Test Exception", new Exception("Inner Exception 1", new Exception("Inner Exception 2")));
			} catch (Exception exc) {
				AppStatisticsCommon.Logging.Logger.LogException(exc).Wait();
			}

            Console.WriteLine("Hello World!");
			Console.ReadLine();
		}
    }
}
