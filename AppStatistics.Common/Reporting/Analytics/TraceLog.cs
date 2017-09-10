using AppStatistics.Common.Models.Reporting.Analytics;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace AppStatistics.Common.Reporting.Analytics {
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

				for(int i = 0; i < filePaths.Count; i++) {
					string tempFileName = getTempFileName();
					string tempFilePath = getTempFolder() + "\\" + tempFileName;

					File.Copy(filePaths[i], tempFilePath);
					filePaths[i] = tempFilePath;
				}

				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
				MoveNext();
			}

			public TraceDataModel Current => current;

			object IEnumerator.Current => current;

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

			public bool MoveNext() {
				current = null;
				if (fileIndex >= filePaths.Count)
					return false;

				if (string.IsNullOrEmpty(contentFolder) ||
					string.IsNullOrEmpty(filePaths[fileIndex]))
					return false;

				if (reader.Peek() < 0)
					MoveNextFile();

				if (stream == null || reader == null)
					return false;

				var data = reader.ReadLine();
				if (string.IsNullOrEmpty(data)) {
					MoveNextFile();
					return MoveNext();
				}

				current = new TraceDataModel();
				current.fromRaw(data);

				DateTime timeStamp = current.timestamp;

				if (timeStamp <= startTime)
					return MoveNext();

				if (timeStamp >= endTime)
					return MoveNext();

				return true;
			}

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

			public void Reset() {
				Dispose();
				if (contentFolder == null)
					return;

				if (filePaths == null || filePaths.Count == 0)
					return;

				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
			}

			private string getTempFolder() {
				return contentFolder + "\\Temp";
			}

			private string getTempFileName() {
				int i = 1;
				while (File.Exists($"_temp{i}.dat")) i++;
				return $"_temp{i}.dat";
			}
		}

		public string contentFolder;
		public List<string> filePaths;

		public DateTime startTime;
		public DateTime endTime;

		public TraceSet(string contentFolder, List<string> dataFiles, DateTime dateTime) {
			this.startTime = dateTime.Date;
			this.endTime = dateTime.Date.AddDays(1);
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		public TraceSet(string contentFolder, List<string> dataFiles, DateTime startTime, DateTime endTime) {
			this.startTime = startTime;
			this.endTime = endTime;
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		public IEnumerator<T> GetEnumerator() {
			return (IEnumerator<T>)new Enumerator(contentFolder, filePaths, startTime, endTime);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(contentFolder, filePaths, startTime, endTime);
		}
	}

	public static class TraceLog {
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

		public static TraceReportDataModel GetReport(DateTime startTime, DateTime endTime) {
			return new TraceReportDataModel(GetTraceLog(startTime, endTime)) {
				startTime = startTime,
				endTime = endTime,
			};
		}

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

		private static string getTraceLogFileName(DateTime date) {
			return $"\\{date.ToString("yyyyMMdd")}.log";
		}
	}
}