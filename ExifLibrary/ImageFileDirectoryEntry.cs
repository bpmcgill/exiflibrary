using System;
using System.Collections.Generic;
using System.Text;

namespace ExifLibrary
{
    /// <summary>
    /// Represents an entry in the image file directory.
    /// </summary>
    public struct ImageFileDirectoryEntry
    {
        /// <summary>
        /// Count of Type.
        /// </summary>
        public uint Count;

        /// <summary>
        /// Field data.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// The tag that identifies the field.
        /// </summary>
        public ushort Tag;

        /// <summary>
        /// Field type identifier.
        /// </summary>
        public ushort Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileDirectoryEntry"/> struct.
        /// </summary>
        /// <param name="tag">The tag that identifies the field.</param>
        /// <param name="type">Field type identifier.</param>
        /// <param name="count">Count of Type.</param>
        /// <param name="data">Field data.</param>
        public ImageFileDirectoryEntry(ushort tag, ushort type, uint count, byte[] data)
        {
            Tag = tag;
            Type = type;
            Count = count;
            Data = data;
        }

        /// <summary>
        /// Gets the base byte length for the given type.
        /// </summary>
        /// <param name="type">Type identifier.</param>
        private static uint GetBaseLength(ushort type)
        {
            return type switch
            {
                1 or 6 or 2 or 7 => 1, // BYTE, SBYTE, ASCII, UNDEFINED
                3 or 8 => 2, // SHORT, SSHORT
                4 or 9 or 11 => 4, // LONG, SLONG, FLOAT
                5 or 10 or 12 => 8, // RATIONAL, SRATIONAL, DOUBLE
                _ => throw new ArgumentException("Unknown type identifier.", nameof(type)),
            };
        }

        /// <summary>
        /// Returns a <see cref="ImageFileDirectoryEntry"/> initialized from the given byte data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset into <paramref name="data"/>.</param>
        /// <param name="byteOrder">The byte order of <paramref name="data"/>.</param>
        /// <returns>A <see cref="ImageFileDirectoryEntry"/> initialized from the given byte data.</returns>
        public static ImageFileDirectoryEntry FromBytes(byte[] data, uint offset, BitConverterEx.ByteOrder byteOrder)
        {
            // Tag ID
            ushort tag = BitConverterEx.ToUInt16(data, offset, byteOrder, BitConverterEx.SystemByteOrder);

            // Tag Type
            ushort type = BitConverterEx.ToUInt16(data, offset + 2, byteOrder, BitConverterEx.SystemByteOrder);

            // Count of Type
            uint count = BitConverterEx.ToUInt32(data, offset + 4, byteOrder, BitConverterEx.SystemByteOrder);

            // Field value or offset to field data
            byte[] value = new byte[4];

            Array.Copy(data, (int)offset + 8, value, 0, 4);

            // Calculate the bytes we need to read
            uint baselength = GetBaseLength(type);
            uint totallength = count * baselength;

            // If field value does not fit in 4 bytes
            // the value field is an offset to the actual
            // field value
            if (totallength > 4)
            {
                uint dataoffset = BitConverterEx.ToUInt32(value, 0, byteOrder, BitConverterEx.SystemByteOrder);

                value = new byte[totallength];

                Array.Copy(data, (int)dataoffset, value, 0, (int)totallength);
            }

            // Reverse array order if byte orders are different
            if (byteOrder != BitConverterEx.SystemByteOrder)
            {
                for (int i = 0; i < count; i++)
                {
                    byte[] val = new byte[baselength];

                    Array.Copy(value, i * (int)baselength, val, 0, (int)baselength);
                    Array.Reverse(val);
                    Array.Copy(val, 0, value, i * (int)baselength, (int)baselength);
                }
            }

            return new ImageFileDirectoryEntry(tag, type, count, value);
        }
    }
}