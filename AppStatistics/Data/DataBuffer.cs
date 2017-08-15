using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Data {
	public class DataBufferStream : IDataBuffer {
		private MemoryStream ms;
		private BinaryWriter writer;
		private BinaryReader reader;

		private long _length;
		private long length {
			get { return _length; }
			set { if (_length < value) _length = value; }
		}

		public DataBufferStream() {
			ms = new MemoryStream();

			writer = new BinaryWriter(ms);
			reader = new BinaryReader(ms);
			length = 0;
		}

		public DataBufferStream(byte[] message) {
			ms = new MemoryStream();
			for (int i = 0; i < message.Length; i++)
				ms.WriteByte(message[i]);

			ms.Seek(0, SeekOrigin.Begin);

			writer = new BinaryWriter(ms);
			reader = new BinaryReader(ms);
			length = message.Length;
		}

		/// <summary>
		/// Sets the offset to the given position.
		/// </summary>
		/// <param name="pos"></param>
		public void seek(int pos) {
			ms.Seek(pos, SeekOrigin.Begin);
		}

		#region Writing
		/// <summary>
		/// Writes a string to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(string source) {
			writer.Write(source);
			length = ms.Position;
		}

		/// <summary>
		/// Writes the contents of another databuffer to the buffer.
		/// </summary>
		/// <param name="v"></param>
		public void write(IDataBuffer v) {
			var size = v.getPos();
			v.seek(0);

			write((int)size);
			for (int i = 0; i < size; i++)
				write(v.readByte());

			length = ms.Position;
		}

		/// <summary>
		/// Writes a 16-bit integer (short) to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(Int16 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes an unsigned 16-bit integer (ushort) to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(UInt16 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes a 32-bit integer to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(Int32 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes an unsigned 32-bit integer to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(UInt32 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes a 64-bit integer to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(Int64 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes an unsigned 64-bit integer to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(UInt64 source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes a float to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(float source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes a double to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(double source) {
			writer.Write(source);

			length = ms.Position;
		}

		/// <summary>
		/// Writes a byte to the buffer.
		/// </summary>
		/// <param name="source"></param>
		public void write(byte source) {
			ms.WriteByte(source);

			length = ms.Position;
		}
		#endregion

		#region Reading
		/// <summary>
		/// Reads the contents of another databuffer from the buffer.
		/// </summary>
		/// <returns></returns>
		public IDataBuffer readBuffer() {
			int len = readInt32();
			byte[] ret = new byte[len];

			for (int i = 0; i < len; i++)
				ret[i] = readByte();

			return new DataBufferStream(ret);
		}

		/// <summary>
		/// Reads a byte from the buffer.
		/// </summary>
		/// <returns></returns>
		public byte readByte() {
			return reader.ReadByte();
		}

		/// <summary>
		/// Reads a short from the buffer.
		/// </summary>
		/// <returns></returns>
		public Int16 readInt16() {
			return reader.ReadInt16();
		}

		/// <summary>
		/// Reads an int from the buffer.
		/// </summary>
		/// <returns></returns>
		public Int32 readInt32() {
			return reader.ReadInt32();
		}

		/// <summary>
		/// Reads a long from the buffer.
		/// </summary>
		/// <returns></returns>
		public Int64 readInt64() {
			return reader.ReadInt64();
		}

		/// <summary>
		/// Reads a ushort from the buffer.
		/// </summary>
		/// <returns></returns>
		public UInt16 readUInt16() {
			return reader.ReadUInt16();
		}

		/// <summary>
		/// Reads a uint from the buffer.
		/// </summary>
		/// <returns></returns>
		public UInt32 readUInt32() {
			return reader.ReadUInt32();
		}

		/// <summary>
		/// Reads a ulong from the buffer.
		/// </summary>
		/// <returns></returns>
		public UInt64 readUInt64() {
			return reader.ReadUInt64();
		}

		/// <summary>
		/// Reads a float from the buffer.
		/// </summary>
		/// <returns></returns>
		public float readFloat() {
			return reader.ReadSingle();
		}

		/// <summary>
		/// Reads a double from the buffer.
		/// </summary>
		/// <returns></returns>
		public double readDouble() {
			return reader.ReadDouble();
		}

		/// <summary>
		/// Reads a string from the buffer.
		/// </summary>
		/// <returns></returns>
		public string readString() {
			return reader.ReadString();
		}
		#endregion

		#region Peeking
		/// <summary>
		/// Peeks the contents of another databuffer from the buffer.
		/// </summary>
		/// <returns></returns>
		public IDataBuffer peekDataBuffer() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Peeks the next byte of the buffer.
		/// </summary>
		/// <returns></returns>
		public byte peekByte() {
			var pos = ms.Position;
			var ret = reader.ReadByte();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next sbyte of the buffer.
		/// </summary>
		/// <returns></returns>
		public sbyte peekSByte() {
			var pos = ms.Position;
			var ret = reader.ReadSByte();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next short of the buffer.
		/// </summary>
		/// <returns></returns>
		public short peekInt16() {
			var pos = ms.Position;
			var ret = reader.ReadInt16();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next int of the buffer.
		/// </summary>
		/// <returns></returns>
		public int peekInt32() {
			var pos = ms.Position;
			var ret = reader.ReadInt32();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next long of the buffer.
		/// </summary>
		/// <returns></returns>
		public long peekInt64() {
			var pos = ms.Position;
			var ret = reader.ReadInt64();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next short of the buffer.
		/// </summary>
		/// <returns></returns>
		public ushort peekUInt16() {
			var pos = ms.Position;
			var ret = reader.ReadUInt16();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next int of the buffer.
		/// </summary>
		/// <returns></returns>
		public uint peekUInt32() {
			var pos = ms.Position;
			var ret = reader.ReadUInt32();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next long of the buffer.
		/// </summary>
		/// <returns></returns>
		public ulong peekUInt64() {
			var pos = ms.Position;
			var ret = reader.ReadUInt64();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next float of the buffer.
		/// </summary>
		/// <returns></returns>
		public float peekFloat() {
			var pos = ms.Position;
			var ret = reader.ReadSingle();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next double of the buffer.
		/// </summary>
		/// <returns></returns>
		public double peekDouble() {
			var pos = ms.Position;
			var ret = reader.ReadDouble();
			ms.Position = pos;

			return ret;
		}

		/// <summary>
		/// Peeks the next string of the buffer.
		/// </summary>
		/// <returns></returns>
		public string peekString() {
			var pos = ms.Position;
			var ret = reader.ReadString();
			ms.Position = pos;

			return ret;
		}
		#endregion

		/// <summary>
		/// Returns the bytes array.
		/// </summary>
		/// <returns></returns>
		public byte[] toBytes() {
			return ms.ToArray();
		}

		/// <summary>
		/// Converts the contents of this buffer to a string.
		/// </summary>
		/// <returns></returns>
		public string bytesToString() {
			string ret = "";
			var bytes = toBytes();
			for (int i = 0; i < bytes.Length; i++)
				ret += (char)bytes[i];

			return ret;
		}

		/// <summary>
		/// Reads the contents of a string into the buffer.
		/// </summary>
		/// <param name="str"></param>
		public void stringToBytes(string str) {
			for (int i = 0; i < str.Length; i++) {
				ms.WriteByte((byte)str[i]);
			}
		}

		/// <summary>
		/// Returns the length of the buffer.
		/// </summary>
		/// <returns></returns>
		public int getLength() {
			return (int)length;
		}

		/// <summary>
		/// Returns the current position of the buffer.
		/// </summary>
		/// <returns></returns>
		public int getPos() {
			return (int)ms.Position;
		}

		/// <summary>
		/// Writes the buffer's data to a file.
		/// </summary>
		/// <param name="path"></param>
		public void writeToFile(string path) {
			var file = File.OpenWrite(path);
			ms.WriteTo(file);
			file.Flush();
			file.Dispose();
		}

		/// <summary>
		/// Reads the contents of a file to the buffer.
		/// </summary>
		/// <param name="path"></param>
		public void readFromFile(string path) {
			var file = File.OpenRead(path);
			file.CopyTo(ms);
			file.Flush();
			file.Dispose();
		}

		public void clear() {
			ms = new MemoryStream();
			LinkedList<int> list = new LinkedList<int>();

			writer = new BinaryWriter(ms);
			reader = new BinaryReader(ms);
			length = 0;
		}
	}
}