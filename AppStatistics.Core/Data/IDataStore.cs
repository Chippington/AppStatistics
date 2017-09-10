using AppStatistics.Common.Models.Reporting;
using AppStatistics.Common.Models.Reporting.Analytics;
using AppStatistics.Common.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Core.Data {
	public interface IDataStore {

		IEnumerable<ApplicationDataModel> GetAllApplications();
		ApplicationDataModel GetApplication(string applicationID);
		bool AddApplication(ApplicationDataModel appData);
		bool AddApplication(string applicationID, ApplicationDataModel appData);
		bool UpdateApplication(string applicationID, ApplicationDataModel appData);
		bool DeleteApplication(string applicationID);

		IEnumerable<ExceptionDataModel> GetExceptionsByApplication(string applicationID, DateTime startTime, DateTime endTime);
		ExceptionDataModel GetException(string exceptionID);
		bool AddException(string applicationID, ExceptionDataModel exceptionData);
		bool DeleteException(string exceptionID);

		IEnumerable<TraceDataModel> GetTraceDataBySession(string sessionID);
		TraceDataModel GetTraceData(string traceID);
		bool AddTraceData(string sessionID, TraceDataModel traceData);
		bool DeleteTraceData(string traceID);

		Exception GetLastException();

		///// <summary>
		///// Returns a collection of all application models.
		///// </summary>
		///// <returns></returns>
		//IEnumerable<ApplicationDataModel> getApplications();

		///// <summary>
		///// Return the application model associated with the given key.
		///// </summary>
		///// <param name="key"></param>
		///// <returns></returns>
		//ApplicationDataModel getApplication(string key);

		///// <summary>
		///// Adds an application to the data store.
		///// </summary>
		///// <param name="app"></param>
		//void addApplication(ApplicationDataModel app);

		///// <summary>
		///// Update the currently stored application model with this one
		///// </summary>
		///// <param name="app"></param>
		//void updateApplication(ApplicationDataModel app);


		///// <summary>
		///// Removes an application from the data store.
		///// </summary>
		///// <param name="app"></param>
		//void removeApplication(ApplicationDataModel app);

		///// <summary>
		///// Returns a collection of all exceptions.
		///// </summary>
		///// <returns></returns>
		//IEnumerable<ExceptionDataModel> getExceptions(ApplicationDataModel app);

		///// <summary>
		///// Adds an exception to the data store.
		///// </summary>
		///// <param name="app"></param>
		///// <param name="exception"></param>
		//void addException(ExceptionDataModel exception);

		///// <summary>
		///// Returns the exception associated with the given key.
		///// </summary>
		///// <param name="key"></param>
		///// <returns></returns>
		//ExceptionDataModel getException(ApplicationDataModel app, string key);

		///// <summary>
		///// Returns a collection of reports for all applications using the given timeframe.
		///// </summary>
		///// <param name="startDate"></param>
		///// <param name="endDate"></param>
		///// <returns></returns>
		//IEnumerable<ReportDataModel> getReports(DateTime startDate, DateTime endDate);

		///// <summary>
		///// Returns a report for a specific application using the given timeframe.
		///// </summary>
		///// <param name="app"></param>
		///// <param name="startDate"></param>
		///// <param name="endDate"></param>
		///// <returns></returns>
		//ReportDataModel getReport(ApplicationDataModel app, DateTime startDate, DateTime endDate);

		///// <summary>
		///// Returns a collection of all notification models.
		///// </summary>
		///// <returns></returns>
		//IEnumerable<NotificationDataModel> getNotifications();

		///// <summary>
		///// Returns a list of notification models associated with a specific application.
		///// </summary>
		///// <param name="app"></param>
		///// <returns></returns>
		//IEnumerable<NotificationDataModel> getNotifications(ApplicationDataModel app);

		///// <summary>
		///// Returns a notification model associated with the given key.
		///// </summary>
		///// <param name="key"></param>
		///// <returns></returns>
		//NotificationDataModel getNotification(string key);
	}
}