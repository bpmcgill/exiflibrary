﻿using System;
using System.Text;

namespace ExifLibrary
{
    /// <summary>
    /// Represents the location and area of the subject (EXIF Specification: 3xSHORT)
    /// The coordinate values, width, and height are expressed in relation to the
    /// upper left as origin, prior to rotation processing as per the Rotation tag.
    /// </summary>
    public class ExifCircularSubjectArea : ExifPointSubjectArea
    {
        public ExifCircularSubjectArea(ExifTag tag, ushort[] value) : base(tag, value)
        { }

        public ExifCircularSubjectArea(ExifTag tag, ushort x, ushort y, ushort d) : base(tag, new ushort[] { x, y, d })
        { }

        public ushort Diameter
        {
            get => mValue[2];
            set
            {
                mValue[2] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat($"({mValue[0]:d}, {mValue[1]:d}) {mValue[2]:d}");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents an ASCII string formatted as Date. (EXIF Specification: ASCII)
    /// Used for date fields.
    /// </summary>
    public class ExifDate : ExifProperty
    {
        protected DateTime mValue;

        public ExifDate(ExifTag tag, DateTime value) : base(tag)
        {
            mValue = value;
        }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (DateTime)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get => new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.ASCII, 11, ExifBitConverter.GetBytes(mValue, false));
        }

        public new DateTime Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public static implicit operator DateTime(ExifDate obj) => obj.mValue;

        public override string ToString() => mValue.ToString("yyyy.MM.dd");
    }

    /// <summary>
    /// Represents an ASCII string formatted as DateTime. (EXIF Specification: ASCII)
    /// Used for date time fields.
    /// </summary>
    public class ExifDateTime : ExifProperty
    {
        protected DateTime mValue;

        public ExifDateTime(ExifTag tag, DateTime value) : base(tag)
        {
            mValue = value;
        }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (DateTime)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get => new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.ASCII, 20, ExifBitConverter.GetBytes(mValue, true));
        }

        public new DateTime Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public static implicit operator DateTime(ExifDateTime obj) => obj.mValue;

        public override string ToString() => mValue.ToString("yyyy.MM.dd HH:mm:ss");
    }

    /// <summary>
    /// Represents an ASCII string. (EXIF Specification: UNDEFINED)
    /// Used for the UserComment field.
    /// </summary>
    public class ExifEncodedString : ExifProperty
    {
        private Encoding mEncoding;

        protected string mValue;

        public ExifEncodedString(ExifTag tag, string value, Encoding encoding) : base(tag)
        {
            mValue = value;
            mEncoding = encoding;
        }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (string)value;
            }
        }

        public Encoding Encoding
        {
            get => mEncoding;
            set
            {
                mEncoding = value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get
            {
                string enc = "";
                switch (mEncoding)
                {
                    case null:
                        enc = "\0\0\0\0\0\0\0\0";
                        break;
                    default:
                        if (mEncoding.EncodingName == "US-ASCII")
                        {
                            enc = "ASCII\0\0\0";
                        }
                        else if (mEncoding.EncodingName == "Japanese (JIS 0208-1990 and 0212-1990)")
                        {
                            enc = "JIS\0\0\0\0\0";
                        }
                        else if (mEncoding.EncodingName == "Unicode")
                        {
                            enc = "Unicode\0";
                        }
                        else
                        {
                            enc = "\0\0\0\0\0\0\0\0";
                        }

                        break;
                }

                byte[] benc = Encoding.ASCII.GetBytes(enc);
                byte[] bstr = (mEncoding == null ? Encoding.ASCII.GetBytes(mValue) : mEncoding.GetBytes(mValue));
                byte[] data = new byte[benc.Length + bstr.Length];

                Array.Copy(benc, 0, data, 0, benc.Length);
                Array.Copy(bstr, 0, data, benc.Length, bstr.Length);

                return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.UNDEFINED, (uint)data.Length, data);
            }
        }

        public new string Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public static implicit operator string(ExifEncodedString obj) => obj.mValue;

        public override string ToString() => mValue;
    }

    /// <summary>
    /// Represents an enumerated value.
    /// </summary>
    public class ExifEnumProperty<T> : ExifProperty where T : Enum
    {
        protected bool mIsBitField;

        protected T mValue;

        public ExifEnumProperty(ExifTag tag, T value, bool isbitfield) : base(tag)
        {
            mValue = value;
            mIsBitField = isbitfield;
        }

        public ExifEnumProperty(ExifTag tag, T value) : this(tag, value, false)
        { }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (T)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get
            {
                ushort tagid = ExifTagFactory.GetTagID(mTag);

                Type type = typeof(T);
                Type basetype = Enum.GetUnderlyingType(type);

                if (type == typeof(FileSource) || type == typeof(SceneType))
                {
                    // UNDEFINED
                    return new ExifInterOperability(tagid, InterOpType.UNDEFINED, 1, new byte[] { (byte)((object)mValue) });
                }
                else if (type == typeof(GPSLatitudeRef) || type == typeof(GPSLongitudeRef) ||
                    type == typeof(GPSStatus) || type == typeof(GPSMeasureMode) ||
                    type == typeof(GPSSpeedRef) || type == typeof(GPSDirectionRef) ||
                    type == typeof(GPSDistanceRef))
                {
                    // ASCII
                    return new ExifInterOperability(tagid, InterOpType.ASCII, 2, new byte[] { (byte)((object)mValue), 0 });
                }
                else if (basetype == typeof(byte))
                {
                    // BYTE
                    return new ExifInterOperability(tagid, InterOpType.BYTE, 1, new byte[] { (byte)((object)mValue) });
                }
                else if (basetype == typeof(ushort))
                {
                    // SHORT
                    return new ExifInterOperability(tagid, InterOpType.SHORT, 1, ExifBitConverter.GetBytes((ushort)((object)mValue), BitConverterEx.SystemByteOrder, BitConverterEx.SystemByteOrder));
                }
                else
                    throw new UnknownEnumTypeException();
            }
        }

        public bool IsBitField
        {
            get => mIsBitField;
        }

        public new T Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public static implicit operator T(ExifEnumProperty<T> obj) => obj.mValue;

        public override string ToString() => mValue.ToString();
    }

    /// <summary>
    /// Represents the location and area of the subject (EXIF Specification: 2xSHORT)
    /// The coordinate values, width, and height are expressed in relation to the
    /// upper left as origin, prior to rotation processing as per the Rotation tag.
    /// </summary>
    public class ExifPointSubjectArea : ExifUShortArray
    {
        public ExifPointSubjectArea(ExifTag tag, ushort[] value) : base(tag, value)
        { }

        public ExifPointSubjectArea(ExifTag tag, ushort x, ushort y) : base(tag, new ushort[] { x, y })
        { }

        protected new ushort[] Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public ushort X
        {
            get => mValue[0];
            set
            {
                mValue[0] = value;
            }
        }

        public ushort Y
        {
            get => mValue[1];
            set
            {
                mValue[1] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"({mValue[0]:d}, {mValue[1]:d})");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents the location and area of the subject (EXIF Specification: 4xSHORT)
    /// The coordinate values, width, and height are expressed in relation to the
    /// upper left as origin, prior to rotation processing as per the Rotation tag.
    /// </summary>
    public class ExifRectangularSubjectArea : ExifPointSubjectArea
    {
        public ExifRectangularSubjectArea(ExifTag tag, ushort[] value) : base(tag, value)
        { }

        public ExifRectangularSubjectArea(ExifTag tag, ushort x, ushort y, ushort w, ushort h) : base(tag, new ushort[] { x, y, w, h })
        { }

        public ushort Height
        {
            get => mValue[3];
            set
            {
                mValue[3] = value;
            }
        }

        public ushort Width
        {
            get => mValue[2];
            set
            {
                mValue[2] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat($"({mValue[0]:d}, {mValue[1]:d}) ({mValue[2]:d} x {mValue[3]:d})");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents the exif version as a 4 byte ASCII string. (EXIF Specification: UNDEFINED)
    /// Used for the ExifVersion, FlashpixVersion and InteroperabilityVersion fields.
    /// </summary>
    public class ExifVersion : ExifProperty
    {
        protected string mValue;

        public ExifVersion(ExifTag tag, string value) : base(tag)
        {
            switch (value.Length)
            {
                case > 4:
                    mValue = value.Substring(0, 4);
                    break;
                case < 4:
                    mValue = value + new string(' ', 4 - value.Length);
                    break;
                default:
                    mValue = value;
                    break;
            }
        }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (string)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get
            {
                if (mTag == ExifTag.ExifVersion || mTag == ExifTag.FlashpixVersion || mTag == ExifTag.InteroperabilityVersion)
                {
                    return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.UNDEFINED, 4, Encoding.ASCII.GetBytes(mValue));
                }
                else
                {
                    byte[] data = new byte[4];

                    for (int i = 0; i < 4; i++)
                    {
                        data[i] = byte.Parse(mValue[0].ToString());
                    }

                    return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.UNDEFINED, 4, data);
                }
            }
        }

        public new string Value
        {
            get => mValue; set
            {
                mValue = value.Substring(0, 4);
            }
        }

        public override string ToString() => mValue;
    }

    /// <summary>
    /// Represents GPS latitudes and longitudes (EXIF Specification: 3xRATIONAL)
    /// </summary>
    public class GPSLatitudeLongitude : ExifURationalArray
    {
        public GPSLatitudeLongitude(ExifTag tag, MathEx.UFraction32[] value) : base(tag, value)
        { }

        public GPSLatitudeLongitude(ExifTag tag, float d, float m, float s) : base(tag, new MathEx.UFraction32[] { new MathEx.UFraction32(d), new MathEx.UFraction32(m), new MathEx.UFraction32(s) })
        { }

        protected new MathEx.UFraction32[] Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public MathEx.UFraction32 Degrees
        {
            get => mValue[0];
            set
            {
                mValue[0] = value;
            }
        }

        public MathEx.UFraction32 Minutes
        {
            get => mValue[1];
            set
            {
                mValue[1] = value;
            }
        }

        public MathEx.UFraction32 Seconds
        {
            get => mValue[2];
            set
            {
                mValue[2] = value;
            }
        }

        public static explicit operator float(GPSLatitudeLongitude obj) => obj.ToFloat();

        public float ToFloat() => (float)Degrees + ((float)Minutes) / 60.0f + ((float)Seconds) / 3600.0f;

        public override string ToString() => $"{(float)Degrees:F2}°{(float)Minutes:F2}'{(float)Seconds:F2}\"";
    }

    /// <summary>
    /// Represents a GPS time stamp as UTC (EXIF Specification: 3xRATIONAL)
    /// </summary>
    public class GPSTimeStamp : ExifURationalArray
    {
        public GPSTimeStamp(ExifTag tag, MathEx.UFraction32[] value) : base(tag, value)
        { }

        public GPSTimeStamp(ExifTag tag, float h, float m, float s) : base(tag, new MathEx.UFraction32[] { new MathEx.UFraction32(h), new MathEx.UFraction32(m), new MathEx.UFraction32(s) })
        { }

        protected new MathEx.UFraction32[] Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public MathEx.UFraction32 Hour
        {
            get => mValue[0]; set
            {
                mValue[0] = value;
            }
        }

        public MathEx.UFraction32 Minute
        {
            get => mValue[1];
            set
            {
                mValue[1] = value;
            }
        }

        public MathEx.UFraction32 Second
        {
            get => mValue[2];
            set
            {
                mValue[2] = value;
            }
        }

        public override string ToString() => $"{(float)Hour:F2}:{(float)Minute:F2}:{(float)Second:F2}";
    }

    /// <summary>
    /// Represents lens specification (EXIF Specification: 4xRATIONAL)
    /// </summary>
    public class LensSpecification : ExifURationalArray
    {
        public LensSpecification(ExifTag tag, MathEx.UFraction32[] value) : base(tag, value)
        { }

        public LensSpecification(ExifTag tag, float minFocal, float maxFocal, float minFocalF, float maxFocalF)
            : base(tag, new MathEx.UFraction32[] { new MathEx.UFraction32(minFocal), new MathEx.UFraction32(maxFocal),
                new MathEx.UFraction32(minFocalF), new MathEx.UFraction32(maxFocalF) })
        { }

        protected new MathEx.UFraction32[] Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public MathEx.UFraction32 MaxFocalLength
        {
            get => mValue[1];
            set
            {
                mValue[1] = value;
            }
        }

        public MathEx.UFraction32 MaxFocalLengthFNumber
        {
            get => mValue[3];
            set
            {
                mValue[3] = value;
            }
        }

        public MathEx.UFraction32 MinFocalLength
        {
            get => mValue[0];
            set
            {
                mValue[0] = value;
            }
        }

        public MathEx.UFraction32 MinFocalLengthFNumber
        {
            get => mValue[2];
            set
            {
                mValue[2] = value;
            }
        }

        public override string ToString() => $"{MinFocalLength} F{MinFocalLengthFNumber}, {MaxFocalLength} F{MaxFocalLengthFNumber}";
    }

    /// <summary>
    /// Represents a version as a 4 byte byte array. (Specification: int8u[4])
    /// Used for the GPSVersionID field.
    /// </summary>
    public class VersionID : ExifByteArray
    {
        public VersionID(ExifTag tag, byte[] value) : base(tag, value)
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var b in Value)
            {
                sb.Append(b).Append('.');
            }

            return sb.ToString().TrimEnd('.');
        }
    }

    /// <summary>
    /// Represents an ASCII string. (EXIF Specification: BYTE)
    /// Used by Windows XP.
    /// </summary>
    public class WindowsByteString : ExifProperty
    {
        protected string mValue;

        public WindowsByteString(ExifTag tag, string value) : base(tag)
        {
            mValue = value;
        }

        protected override object _Value
        {
            get => Value;
            set
            {
                Value = (string)value;
            }
        }

        public override ExifInterOperability Interoperability
        {
            get
            {
                byte[] data = Encoding.Unicode.GetBytes(mValue);
                return new ExifInterOperability(ExifTagFactory.GetTagID(mTag), InterOpType.BYTE, (uint)data.Length, data);
            }
        }

        public new string Value
        {
            get => mValue;
            set
            {
                mValue = value;
            }
        }

        public static implicit operator string(WindowsByteString obj) => obj.mValue;

        public override string ToString() => mValue;
    }
}