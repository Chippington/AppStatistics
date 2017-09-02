using AppStatisticsCommon.Models.Reporting;
using AppStatisticsCommon.Models.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatisticsCore.Data {
	public interface IDataStore {
		/// <summary>
		/// Returns a collection of all application models.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ApplicationModel> getApplications();

		/// <summary>
		/// Return the application model associated with the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		ApplicationModel getApplication(string key);

		/// <summary>
		/// Adds an application to the data store.
		/// </summary>
		/// <param name="app"></param>
		void addApplication(ApplicationModel app);

		/// <summary>
		/// Removes an application from the data store.
		/// </summary>
		/// <param name="app"></param>
		void removeApplication(ApplicationModel app);

		/// <summary>
		/// Returns a collection of all exceptions.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ExceptionModel> getExceptions(ApplicationModel app);

		/// <summary>
		/// Adds an exception to the data store.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="exception"></param>
		void addException(ApplicationModel app, ExceptionModel exception);

		/// <summary>
		/// Returns the exception associated with the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		ExceptionModel getException(ApplicationModel app, string key);

		/// <summary>
		/// Returns a collection of reports for all applications using the given timeframe.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		IEnumerable<ReportModel> getReports(DateTime startDate, DateTime endDate);
		
		/// <summary>
		/// Returns a report for a specific application using the given timeframe.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		ReportModel getReport(ApplicationModel app, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Returns a collection of all notification models.
		/// </summary>
		/// <returns></returns>
		IEnumerable<NotificationModel> getNotifications();

		/// <summary>
		/// Returns a list of notification models associated with a specific application.
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		IEnumerable<NotificationModel> getNotifications(ApplicationModel app);

		/// <summary>
		/// Returns a notification model associated with the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		NotificationModel getNotification(string key);
	}
}