﻿using System;

namespace ExifLibrary
{
    /// <summary>
    /// An endian-aware converter for converting between base data types
    /// and an array of bytes.
    /// </summary>
    public class BitConverterEx
    {
        private ByteOrder mFrom, mTo;

        /// <summary>
        /// Intializes a new instance of the <see cref="BitConverterEx"/> class.
        /// </summary>
        /// <param name="from">The byte order to convert from.</param>
        /// <param name="to">The byte order to convert to.</param>
        public BitConverterEx(ByteOrder from, ByteOrder to)
        {
            mFrom = from;
            mTo = to;
        }

        /// <summary>
        /// Represents the byte order.
        /// </summary>
        public enum ByteOrder
        {
            /// <summary>
            /// The least significant value is stored first.
            /// </summary>
            LittleEndian = 1,

            /// <summary>
            /// The most significant value is stored first.
            /// </summary>
            BigEndian = 2,
        }

        /// <summary>
        /// Returns a bit converter that converts between big-endian and system byte-order.
        /// </summary>
        public static BitConverterEx BigEndian
        {
            get => new BitConverterEx(ByteOrder.BigEndian, BitConverterEx.SystemByteOrder);
        }

        /// <summary>
        /// Returns a bit converter that converts between little-endian and system byte-order.
        /// </summary>
        public static BitConverterEx LittleEndian
        {
            get => new BitConverterEx(ByteOrder.LittleEndian, BitConverterEx.SystemByteOrder);
        }

        /// <summary>
        /// Indicates the byte order in which data is stored in this platform.
        /// </summary>
        public static ByteOrder SystemByteOrder
        {
            get => (BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian);
        }

        /// <summary>
        /// Returns a bit converter that does not do any byte-order conversion.
        /// </summary>
        public static BitConverterEx SystemEndian
        {
            get => new BitConverterEx(BitConverterEx.SystemByteOrder, BitConverterEx.SystemByteOrder);
        }

        /// <summary>
        /// Reverse the array of bytes as needed.
        /// </summary>
        private static byte[] CheckData(byte[] value, long startIndex, long length, ByteOrder from, ByteOrder to)
        {
            byte[] data = new byte[length];

            Array.Copy(value, (int)startIndex, data, 0, (int)length);

            if (from != to)
            {
                Array.Reverse(data);
            }

            return data;
        }

        /// <summary>
        /// Reverse the array of bytes as needed.
        /// </summary>
        private static byte[] CheckData(byte[] value, ByteOrder from, ByteOrder to) => CheckData(value, 0, value.Length, from, to);

        /// <summary>
        /// Converts the given 16-bit unsigned integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(ushort value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given 32-bit unsigned integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(uint value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given 64-bit unsigned integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(ulong value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);
            
            return data;
        }

        /// <summary>
        /// Converts the given 16-bit signed integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(short value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given 32-bit signed integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(int value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given 64-bit signed integer to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(long value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given single precision floating-point number to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(float value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given double precision floating-point number to an array of bytes.
        /// </summary>
        public static byte[] GetBytes(double value, ByteOrder from, ByteOrder to)
        {
            byte[] data = BitConverter.GetBytes(value);

            data = CheckData(data, from, to);

            return data;
        }

        /// <summary>
        /// Converts the given array of bytes to a Unicode character.
        /// </summary>
        public static char ToChar(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 2, from, to);

            return BitConverter.ToChar(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a double precision floating number.
        /// </summary>
        public static double ToDouble(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 8, from, to);

            return BitConverter.ToDouble(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 16-bit signed integer.
        /// </summary>
        public static short ToInt16(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 2, from, to);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 32-bit signed integer.
        /// </summary>
        public static int ToInt32(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 4, from, to);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 64-bit signed integer.
        /// </summary>
        public static long ToInt64(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 8, from, to);

            return BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a single precision floating number.
        /// </summary>
        public static float ToSingle(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 4, from, to);

            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 16-bit unsigned integer.
        /// </summary>
        public static ushort ToUInt16(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 2, from, to);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 32-bit unsigned integer.
        /// </summary>
        public static uint ToUInt32(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 4, from, to);

            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// Converts the given array of bytes to a 64-bit unsigned integer.
        /// </summary>
        public static ulong ToUInt64(byte[] value, long startIndex, ByteOrder from, ByteOrder to)
        {
            byte[] data = CheckData(value, startIndex, 8, from, to);

            return BitConverter.ToUInt64(data, 0);
        }

        /// <summary>
        /// Converts the given 16-bit unsigned integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(ushort value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given 32-bit unsigned integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(uint value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given 64-bit unsigned integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(ulong value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given 16-bit signed integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(short value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given 32-bit signed integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(int value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given 64-bit signed integer to an array of bytes.
        /// </summary>
        public byte[] GetBytes(long value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given single precision floating-point number to an array of bytes.
        /// </summary>
        public byte[] GetBytes(float value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given double precision floating-point number to an array of bytes.
        /// </summary>
        public byte[] GetBytes(double value) => BitConverterEx.GetBytes(value, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 16-bit unsigned integer.
        /// </summary>
        public char ToChar(byte[] value, long startIndex) => BitConverterEx.ToChar(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a double precision floating number.
        /// </summary>
        public double ToDouble(byte[] value, long startIndex) => BitConverterEx.ToDouble(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 16-bit signed integer.
        /// </summary>
        public short ToInt16(byte[] value, long startIndex) => BitConverterEx.ToInt16(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 32-bit signed integer.
        /// </summary>
        public int ToInt32(byte[] value, long startIndex) => BitConverterEx.ToInt32(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 64-bit signed integer.
        /// </summary>
        public long ToInt64(byte[] value, long startIndex) => BitConverterEx.ToInt64(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a single precision floating number.
        /// </summary>
        public float ToSingle(byte[] value, long startIndex) => BitConverterEx.ToSingle(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 16-bit unsigned integer.
        /// </summary>
        public ushort ToUInt16(byte[] value, long startIndex) => BitConverterEx.ToUInt16(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 32-bit unsigned integer.
        /// </summary>
        public uint ToUInt32(byte[] value, long startIndex) => BitConverterEx.ToUInt32(value, startIndex, mFrom, mTo);

        /// <summary>
        /// Converts the given array of bytes to a 64-bit unsigned integer.
        /// </summary>
        public ulong ToUInt64(byte[] value, long startIndex) => BitConverterEx.ToUInt64(value, startIndex, mFrom, mTo);
    }
}