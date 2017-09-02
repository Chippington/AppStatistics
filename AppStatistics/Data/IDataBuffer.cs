using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStatistics.Data {
	public interface IDataBuffer {
		void write(byte v);
		void write(Int16 v);
		void write(Int32 v);
		void write(Int64 v);
		void write(UInt16 v);
		void write(UInt32 v);
		void write(UInt64 v);
		void write(float v);
		void write(double v);
		void write(string v);
		void write(IDataBuffer v);

		byte readByte();
		Int16 readInt16();
		Int32 readInt32();
		Int64 readInt64();
		UInt16 readUInt16();
		UInt32 readUInt32();
		UInt64 readUInt64();
		float readFloat();
		double readDouble();
		string readString();
		IDataBuffer readBuffer();

		byte peekByte();
		Int16 peekInt16();
		Int32 peekInt32();
		Int64 peekInt64();
		UInt16 peekUInt16();
		UInt32 peekUInt32();
		UInt64 peekUInt64();
		float peekFloat();
		double peekDouble();
		string peekString();
		IDataBuffer peekDataBuffer();

		int getPos();
		int getLength();
		void seek(int pos);
		byte[] toBytes();
		string bytesToString();
		void stringToBytes(string str);

		void clear();
	}
}