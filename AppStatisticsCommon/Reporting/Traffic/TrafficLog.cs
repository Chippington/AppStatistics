using AppStatisticsCommon.Models.Reporting.Traffic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

namespace AppStatisticsCommon.Reporting.Traffic {
	internal class TraceLog<T> : IEnumerable<T> where T : TraceDataModel {
		internal class Enumerator : IEnumerator<TraceDataModel> {
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

				string tempFileName = getTempFileName();
				string tempFilePath = getTempFolder() + $"";

				stream = File.OpenRead(filePaths[fileIndex]);
				reader = new StreamReader(stream);
				MoveNext();
			}

			public TraceDataModel Current => current;

			object IEnumerator.Current => current;

			public void Dispose() {
				if(stream != null)
					stream.Dispose();

				if(reader != null)
					reader.Dispose();

				if(string.IsNullOrEmpty(filePaths[fileIndex]) == false) {
					if (File.Exists(filePaths[fileIndex]))
						File.Delete(filePaths[fileIndex]);
				}

				stream = null;
				reader = null;
			}

			public bool MoveNext() {
				current = null;
				if (string.IsNullOrEmpty(contentFolder) || 
					string.IsNullOrEmpty(filePaths[fileIndex]))
					return false;

				if (reader.Peek() < 0)
					return false;

				var data = reader.ReadLine();
				if (string.IsNullOrEmpty(data)) {
					if (fileIndex + 1 >= filePaths.Count)
						return false;

					Dispose();
					fileIndex++;

					stream = File.OpenRead(filePaths[fileIndex]);
					reader = new StreamReader(stream);
					return MoveNext();
				}

				current = new TraceDataModel();
				current.fromRaw(data);

				DateTime timeStamp;
				if(string.IsNullOrEmpty(current.timestamp) == false && 
					DateTime.TryParse(current.timestamp, out timeStamp)) {

					if (timeStamp <= startTime)
						return MoveNext();

					if (timeStamp >= endTime)
						return MoveNext();
				}

				return true;
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

		public TraceLog(string contentFolder, List<string> dataFiles, DateTime dateTime) {
			this.startTime = dateTime.Date;
			this.endTime = dateTime.Date.AddDays(1);
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		public TraceLog(string contentFolder, List<string> dataFiles, DateTime startTime, DateTime endTime) {
			this.startTime = startTime;
			this.endTime = endTime;
			this.filePaths = dataFiles;
			this.contentFolder = contentFolder;
		}

		public IEnumerator<T> GetEnumerator() {
			return (IEnumerator<T>) new Enumerator(contentFolder, filePaths, startTime, endTime);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(contentFolder, filePaths, startTime, endTime);
		}
	}


	public static class TrafficLog {
		public static TrafficOptions options;
		public static void Trace(string path, string method, string sessionID, string ipaddress) {
			string datetime = DateTime.Now.ToString();

			TraceDataModel m = new TraceDataModel();
			m.path = path;
			m.method = method;
			m.sessionid = sessionID;
			m.ipaddress = ipaddress;
			m.timestamp = datetime;

			string contentPath = options.contentFolderPath;
			string filePath = contentPath + getTraceLogFileName(DateTime.Now);

			if (Directory.Exists(contentPath) == false)
				Directory.CreateDirectory(contentPath);

			string data = (string)m.toRaw();
			File.AppendAllText(filePath, data + Environment.NewLine);
		}

		internal static TraceLog<TraceDataModel> GetTraceLog(DateTime date) {
			date = date.Date;
			string contentPath = options.contentFolderPath;
			string filePath = contentPath + getTraceLogFileName(date);

			if (File.Exists(filePath) == false)
				return new TraceLog<TraceDataModel>(null, null, DateTime.Now);

			return new TraceLog<TraceDataModel>(contentPath, new List<string>() {
				filePath
			}, DateTime.Now);
		}

		internal static TraceLog<TraceDataModel> GetTraceLog(DateTime startTime, DateTime endTime) {
			List<string> files = new List<string>();
			string contentPath = options.contentFolderPath;

			DateTime temp = startTime;
			while(temp < endTime) {
				string filePath = contentPath + getTraceLogFileName(temp);

				if(File.Exists(filePath))
					files.Add(filePath);
			}

			if (files.Count == 0)
				return new TraceLog<TraceDataModel>(null, null, DateTime.Now);

			return new TraceLog<TraceDataModel>(contentPath, files, DateTime.Now);
		}

		private static string getTraceLogFileName(DateTime date) {
			return $"\\{date.ToString("yyyyMMdd")}.log";
		}
	}
}
