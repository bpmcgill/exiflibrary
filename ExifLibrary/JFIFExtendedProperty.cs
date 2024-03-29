using System;
using System.Collections.Generic;
using System.Text;

namespace ExifLibrary
{
    /// <summary>
    /// Represents a JFIF thumbnail. (EXIF Specification: BYTE)
    /// </summary>
    public class JFIFThumbnailProperty : ExifProperty
    {
        protected JFIFThumbnail mValue;

        public JFIFThumbnailProperty(ExifTag tag, JFIFThumbnail value) : base(tag)
        {
            mValue = value;
        }

        protected override object _Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (JFIFThumbnail)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get
            {
                switch (mValue.Format)
                {
                    case JFIFThumbnail.ImageFormat.BMP24Bit:
                        return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.BYTE, (uint)mValue.PixelData.Length, mValue.PixelData);

                    case JFIFThumbnail.ImageFormat.BMPPalette:
                        {
                            byte[] data = new byte[mValue.Palette.Length + mValue.PixelData.Length];
                            Array.Copy(mValue.Palette, data, mValue.Palette.Length);
                            Array.Copy(mValue.PixelData, 0, data, mValue.Palette.Length, mValue.PixelData.Length);
                            return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.BYTE, (uint)data.Length, data);
                        }

                    case JFIFThumbnail.ImageFormat.JPEG:
                        return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.BYTE, (uint)mValue.PixelData.Length, mValue.PixelData);

                    default:
                        throw new InvalidOperationException("Unknown thumbnail type.");
                }
            }
        }

        public new JFIFThumbnail Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public override string ToString() => mValue.Format.ToString();
    }

    /// <summary>
    /// Represents the JFIF version as a 16 bit unsigned integer. (EXIF Specification: SHORT)
    /// </summary>
    public class JFIFVersion : ExifUShort
    {
        public JFIFVersion(ExifTag tag, ushort value) : base(tag, value)
        { }

        public JFIFVersion(ExifTag tag, byte major, byte minor) : base(tag, (ushort)(major * 256 + minor))
        { }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        public byte Major
        {
            get => (byte)(mValue >> 8);
        }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        public byte Minor
        {
            get => (byte)(mValue - (mValue >> 8) * 256);
        }

        public override string ToString() => $"{Major}.{Minor:00}";
    }
}