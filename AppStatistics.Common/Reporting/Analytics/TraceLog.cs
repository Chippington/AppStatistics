using AppStatistics.Common.Models.Reporting.Analytics;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace AppStatistics.Common.Reporting.Analytics {
	/// <summary>
	/// Enumerable set containing trace data for a given span of time.
	/// </summary>
	/// <typeparam name="T">Enforced type TraceDataModel for clarity</typeparam>
	public class TraceSet<T> : IEnumerable<T> where T : TraceDataModel {
		public class Enumerator : IEnumerator<TraceDataModel> {
			internal TraceDataModel current;
			internal StreamReader reader;
			internal DateTime startTime;
			internal DateTime endTime;
			internal Stream stream;

			internal List<string> filePaths;
			internal string contentFolder;
			internal int fileIndex;

			/// <summary>
			/// (Internal) Creates an enumerator from the given list of file paths, and start/end time.
			/// Creates temporary files for reading, in case it is written to on another thread.
			/// </summary>
			/// <param name="contentFolder"></param>
			/// <param name="filePaths"></param>
			/// <param name="startTime"></param>
			/// <param name="endTime"></param>
			internal Enumerator(string contentFolder, List<string> filePaths, DateTime startTime, DateTime endTime) {
				this.startTime = startTime;
				this.endTime = endTime;

				fileIndex = 0;
				this.contentFolder = contentFolder;
				if (contentFolder == null)
					return;

				this.filePaths = filePaths;
				if (filePaths == null || filePaths.Count == 0)
					return;

				if (Directory.Exists(getTempFolder()) == false)
					Directory.CreateDirectory(getTempFolder());

				//Create a temp copy for each file to read through.
				for(int i = 0; i < filePaths.Count; i++) {
					string tempFileName = getTempFileName();
					string tempFilePath = getTempFolder() + "\\" + tempFileName;

					File.Copy(filePaths[i], tempFilePath);
					filePaths[i] = tempFilePath;
				}

				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
				
				//Move to the first instance
				MoveNext();
			}

			/// <summary>
			/// IEnumerator implementation
			/// </summary>
			public TraceDataModel Current => current;

			/// <summary>
			/// IEnumerator implementation
			/// </summary>
			object IEnumerator.Current => current;

			/// <summary>
			/// IEnumerator implementation
			/// </summary>
			public void Dispose() {
				if (stream != null)
					stream.Dispose();

				if (reader != null)
					reader.Dispose();

				if (fileIndex >= filePaths.Count)
					return;

				if (filePaths != null && string.IsNullOrEmpty(filePaths[fileIndex]) == false) {
					if (File.Exists(filePaths[fileIndex]))
						File.Delete(filePaths[fileIndex]);
				}

				stream = null;
				reader = null;
			}

			/// <summary>
			/// Moves to the next Trace element.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext() {
				current = null;

				//If we are on the last file, end
				if (fileIndex >= filePaths.Count)
					return false;

				//If the content folder or the file doesn't exist, end
				if (string.IsNullOrEmpty(contentFolder) ||
					string.IsNullOrEmpty(filePaths[fileIndex]))
					return false;

				//If we reached the end of the file, move to the next
				if (reader.Peek() < 0)
					MoveNextFile();

				//If the stream or reader don't exist, end
				if (stream == null || reader == null)
					return false;

				//Read data, move to next file if empty.
				var data = reader.ReadLine();
				if (string.IsNullOrEmpty(data)) {
					MoveNextFile();
					return MoveNext();
				}


				//Parse the data and return it if it's valid.
				current = new TraceDataModel();
				current.fromRaw(data);

				//If outside the datetime range, continue through the rest just in case
				var timeStamp = current.timestamp;
				if (timeStamp <= startTime)
					return MoveNext();

				if (timeStamp >= endTime)
					return MoveNext();

				//Return success
				return true;
			}

			/// <summary>
			/// Moves to the next file in the series.
			/// </summary>
			private void MoveNextFile() {
				Dispose();
				fileIndex++;

				if (fileIndex >= filePaths.Count)
					return;

				if (File.Exists(filePaths[fileIndex]) == false)
					return;

				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
			}

			/// <summary>
			/// Resets the enumerator.
			/// </summary>
			public void Reset() {
				Dispose();
				if (contentFolder == null)
					return;

				if (filePaths == null || filePaths.Count == 0)
					return;

				fileIndex = 0;
				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
			}

			/// <summary>
			/// Returns the temp folder
			/// </summary>
			/// <returns></returns>
			private string getTempFolder() {
				return contentFolder + "\\Temp";
			}

			/// <summary>
			/// Returns a random file name for use with temp data.
			/// </summary>
			/// <returns></returns>
			private string getTempFileName() {
				var i = Guid.NewGuid().ToString();
				return $"_temp{i}.dat";
			}
		}

		public string contentFolder;
		public List<string> filePaths;

		public DateTime startTime;
		public DateTime endTime;

		/// <summary>
		/// Creates a trace set based on the given content folder, files and date.
		/// </summary>
		/// <param name="contentFolder"></param>
		/// <param name="dataFiles"></param>
		/// <param name="dateTime"></param>
		public TraceSet(string contentFolder, List<string> dataFiles, DateTime dateTime) {
			this.startTime = dateTime.Date;
			this.endTime = dateTime.Date.AddDays(1);
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		/// <summary>
		/// Creates a trace set based on the given content folder, files and date range.
		/// </summary>
		/// <param name="contentFolder"></param>
		/// <param name="dataFiles"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public TraceSet(string contentFolder, List<string> dataFiles, DateTime startTime, DateTime endTime) {
			this.startTime = startTime;
			this.endTime = endTime;
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		/// <summary>
		/// Returns an enumerator for the trace set.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator() {
			return (IEnumerator<T>)new Enumerator(contentFolder, filePaths, startTime, endTime);
		}
		/// <summary>
		/// Returns an enumerator for the trace set.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(contentFolder, filePaths, startTime, endTime);
		}
	}

	public static class TraceLog {
		/// <summary>
		/// Logs a trace event using the given source model.
		/// </summary>
		/// <param name="traceData"></param>
		public static void Trace(TraceDataModel traceData) {
			var datetime = DateTime.Now;
			traceData.timestamp = datetime;

			string contentPath = ReportingConfig.contentFolderPath;
			string filePath = contentPath + getTraceLogFileName(DateTime.Now);

			if (Directory.Exists(contentPath) == false)
				Directory.CreateDirectory(contentPath);

			string data = (string)traceData.toRaw();

			Task.Run(() => {
				bool success = false;
				while (success == false) {
					try {
						File.AppendAllText(filePath, data + Environment.NewLine);
						success = true;
					} catch {
						Task.Delay(100);
					}
				}
			});
		}

		/// <summary>
		/// Returns a trace report from the given date time range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public static TraceReportDataModel GetReport(DateTime startTime, DateTime endTime) {
			return new TraceReportDataModel(GetTraceLog(startTime, endTime)) {
				startTime = startTime,
				endTime = endTime,
			};
		}

		/// <summary>
		/// Returns a trace set for the given date.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static TraceSet<TraceDataModel> GetTraceLog(DateTime date) {
			date = date.Date;
			string contentPath = ReportingConfig.contentFolderPath;
			string filePath = contentPath + getTraceLogFileName(date);

			if (File.Exists(filePath) == false)
				return new TraceSet<TraceDataModel>(null, null, DateTime.Now);

			return new TraceSet<TraceDataModel>(contentPath, new List<string>() {
				filePath
			}, DateTime.Now);
		}

		/// <summary>
		/// Returns a trace set for the given date range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public static TraceSet<TraceDataModel> GetTraceLog(DateTime startTime, DateTime endTime) {
			List<string> files = new List<string>();
			string contentPath = ReportingConfig.contentFolderPath;

			DateTime temp = startTime;
			while (temp.Date <= endTime.Date) {
				string filePath = contentPath + getTraceLogFileName(temp);

				if (File.Exists(filePath))
					files.Add(filePath);

				temp = temp.AddDays(1);
			}

			if (files.Count == 0)
				return new TraceSet<TraceDataModel>(contentPath, new List<string>(), DateTime.Now);

			return new TraceSet<TraceDataModel>(contentPath, files, startTime, endTime);
		}

		/// <summary>
		/// Returns the generic log file name based on the given date.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		private static string getTraceLogFileName(DateTime date) {
			return $"\\{date.ToString("yyyyMMdd")}.log";
		}
	}
}