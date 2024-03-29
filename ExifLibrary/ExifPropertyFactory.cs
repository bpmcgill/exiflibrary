using System;
using System.Text;

namespace ExifLibrary
{
    /// <summary>
    /// Creates exif properties from interoperability parameters.
    /// </summary>
    internal static class ExifPropertyFactory
    {
        /// <summary>
        /// Creates an ExifProperty from the given interoperability parameters.
        /// </summary>
        /// <param name="tag">The tag id of the exif property.</param>
        /// <param name="type">The type id of the exif property.</param>
        /// <param name="count">Byte or component count.</param>
        /// <param name="value">Field data as an array of bytes.</param>
        /// <param name="byteOrder">Byte order of value.</param>
        /// <param name="ifd">IFD section containing this propery.</param>
        /// <param name="encoding">The encoding to be used for text metadata when the source encoding is unknown.</param>
        /// <returns>an ExifProperty initialized from the interoperability parameters.</returns>
        public static ExifProperty Get(ushort tag, ushort type, uint count, byte[] value, BitConverterEx.ByteOrder byteOrder, IFD ifd, Encoding encoding)
        {
            BitConverterEx conv = new BitConverterEx(byteOrder, BitConverterEx.SystemByteOrder);

            // Find the exif tag corresponding to given tag id
            ExifTag etag = ExifTagFactory.GetExifTag(ifd, tag);

            switch (ifd)
            {
                case IFD.Zeroth:
                    switch (tag) // Compression
                    {
                        case 0x103:
                            return new ExifEnumProperty<Compression>(ExifTag.Compression, (Compression)conv.ToUInt16(value, 0));
                        case 0x106:
                            return new ExifEnumProperty<PhotometricInterpretation>(ExifTag.PhotometricInterpretation, (PhotometricInterpretation)conv.ToUInt16(value, 0));
                        case 0x112:
                            return new ExifEnumProperty<Orientation>(ExifTag.Orientation, (Orientation)conv.ToUInt16(value, 0));
                        case 0x11c:
                            return new ExifEnumProperty<PlanarConfiguration>(ExifTag.PlanarConfiguration, (PlanarConfiguration)conv.ToUInt16(value, 0));
                        case 0x213:
                            return new ExifEnumProperty<YCbCrPositioning>(ExifTag.YCbCrPositioning, (YCbCrPositioning)conv.ToUInt16(value, 0));
                        case 0x128:
                            return new ExifEnumProperty<ResolutionUnit>(ExifTag.ResolutionUnit, (ResolutionUnit)conv.ToUInt16(value, 0));
                        case 0x132:
                            return new ExifDateTime(ExifTag.DateTime, ExifBitConverter.ToDateTime(value));
                        case 0x9c9b:
                        case 0x9c9c:
                        case 0x9c9d:
                        case 0x9c9e:
                        case 0x9c9f:
                            return new WindowsByteString(etag, Encoding.Unicode.GetString(value).TrimEnd('\0'));
                    }
                    break;
                case IFD.EXIF:
                    {
                        switch (tag) // ExifVersion
                        {
                            case 0x9000:
                                return new ExifVersion(ExifTag.ExifVersion, ExifBitConverter.ToAscii(value, Encoding.ASCII));
                            case 0xa000:
                                return new ExifVersion(ExifTag.FlashpixVersion, ExifBitConverter.ToAscii(value, Encoding.ASCII));
                            case 0xa001:
                                return new ExifEnumProperty<ColorSpace>(ExifTag.ColorSpace, (ColorSpace)conv.ToUInt16(value, 0));
                            case 0x9286:
                                {
                                    // Default to ASCII
                                    Encoding enc = Encoding.ASCII;
                                    bool hasenc;

                                    switch (value.Length)
                                    {
                                        case < 8:
                                            hasenc = false;
                                            break;
                                        default:
                                            {
                                                hasenc = true;

                                                string encstr = enc.GetString(value, 0, 8);

                                                switch (string.Compare(encstr, "ASCII\0\0\0", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    case 0:
                                                        enc = Encoding.ASCII;
                                                        break;
                                                    default:
                                                        if (string.Compare(encstr, "JIS\0\0\0\0\0", StringComparison.OrdinalIgnoreCase) == 0)
                                                        {
                                                            enc = Encoding.GetEncoding("Japanese (JIS 0208-1990 and 0212-1990)");
                                                        }
                                                        else if (string.Compare(encstr, "Unicode\0", StringComparison.OrdinalIgnoreCase) == 0)
                                                        {
                                                            enc = Encoding.Unicode;
                                                        }
                                                        else
                                                        {
                                                            hasenc = false;
                                                        }

                                                        break;
                                                }

                                                break;
                                            }
                                    }

                                    string val = (hasenc ? enc.GetString(value, 8, value.Length - 8) : enc.GetString(value)).Trim('\0');

                                    return new ExifEncodedString(ExifTag.UserComment, val, enc);
                                }
                            case 0x9003:
                                return new ExifDateTime(ExifTag.DateTimeOriginal, ExifBitConverter.ToDateTime(value));
                            case 0x9004:
                                return new ExifDateTime(ExifTag.DateTimeDigitized, ExifBitConverter.ToDateTime(value));
                            case 0x8822:
                                return new ExifEnumProperty<ExposureProgram>(ExifTag.ExposureProgram, (ExposureProgram)conv.ToUInt16(value, 0));
                            case 0x9207:
                                return new ExifEnumProperty<MeteringMode>(ExifTag.MeteringMode, (MeteringMode)conv.ToUInt16(value, 0));
                            case 0x9208:
                                return new ExifEnumProperty<LightSource>(ExifTag.LightSource, (LightSource)conv.ToUInt16(value, 0));
                            case 0x9209:
                                return new ExifEnumProperty<Flash>(ExifTag.Flash, (Flash)conv.ToUInt16(value, 0), true);
                            case 0x9214:
                                if (count == 3)
                                {
                                    return new ExifCircularSubjectArea(ExifTag.SubjectArea, ExifBitConverter.ToUShortArray(value, (int)count, byteOrder));
                                }
                                else if (count == 4)
                                {
                                    return new ExifRectangularSubjectArea(ExifTag.SubjectArea, ExifBitConverter.ToUShortArray(value, (int)count, byteOrder));
                                }
                                else // count == 2
                                {
                                    return new ExifPointSubjectArea(ExifTag.SubjectArea, ExifBitConverter.ToUShortArray(value, (int)count, byteOrder));
                                }
                            case 0xa210:
                                return new ExifEnumProperty<ResolutionUnit>(ExifTag.FocalPlaneResolutionUnit, (ResolutionUnit)conv.ToUInt16(value, 0), true);
                            case 0xa214:
                                return new ExifPointSubjectArea(ExifTag.SubjectLocation, ExifBitConverter.ToUShortArray(value, (int)count, byteOrder));
                            case 0xa217:
                                return new ExifEnumProperty<SensingMethod>(ExifTag.SensingMethod, (SensingMethod)conv.ToUInt16(value, 0), true);
                            case 0xa300:
                                return new ExifEnumProperty<FileSource>(ExifTag.FileSource, (FileSource)conv.ToUInt16(value, 0), true);
                            case 0xa301:
                                return new ExifEnumProperty<SceneType>(ExifTag.SceneType, (SceneType)conv.ToUInt16(value, 0), true);
                            case 0xa401:
                                return new ExifEnumProperty<CustomRendered>(ExifTag.CustomRendered, (CustomRendered)conv.ToUInt16(value, 0), true);
                            case 0xa402:
                                return new ExifEnumProperty<ExposureMode>(ExifTag.ExposureMode, (ExposureMode)conv.ToUInt16(value, 0), true);
                            case 0xa403:
                                return new ExifEnumProperty<WhiteBalance>(ExifTag.WhiteBalance, (WhiteBalance)conv.ToUInt16(value, 0), true);
                            case 0xa406:
                                return new ExifEnumProperty<SceneCaptureType>(ExifTag.SceneCaptureType, (SceneCaptureType)conv.ToUInt16(value, 0), true);
                            case 0xa407:
                                return new ExifEnumProperty<GainControl>(ExifTag.GainControl, (GainControl)conv.ToUInt16(value, 0), true);
                            case 0xa408:
                                return new ExifEnumProperty<Contrast>(ExifTag.Contrast, (Contrast)conv.ToUInt16(value, 0), true);
                            case 0xa409:
                                return new ExifEnumProperty<Saturation>(ExifTag.Saturation, (Saturation)conv.ToUInt16(value, 0), true);
                            case 0xa40a:
                                return new ExifEnumProperty<Sharpness>(ExifTag.Sharpness, (Sharpness)conv.ToUInt16(value, 0), true);
                            case 0xa40c:
                                return new ExifEnumProperty<SubjectDistanceRange>(ExifTag.SubjectDistanceRange, (SubjectDistanceRange)conv.ToUInt16(value, 0), true);
                            case 0xa432:
                                return new LensSpecification(ExifTag.LensSpecification, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        }

                        break;
                    }

                case IFD.GPS:
                    switch (tag) // GPSVersionID
                    {
                        case 0:
                            return new ExifVersion(ExifTag.GPSVersionID, ExifBitConverter.ToString(value));
                        case 1:
                            return new ExifEnumProperty<GPSLatitudeRef>(ExifTag.GPSLatitudeRef, (GPSLatitudeRef)value[0]);
                        case 2:
                            return new GPSLatitudeLongitude(ExifTag.GPSLatitude, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        case 3:
                            return new ExifEnumProperty<GPSLongitudeRef>(ExifTag.GPSLongitudeRef, (GPSLongitudeRef)value[0]);
                        case 4:
                            return new GPSLatitudeLongitude(ExifTag.GPSLongitude, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        case 5:
                            return new ExifEnumProperty<GPSAltitudeRef>(ExifTag.GPSAltitudeRef, (GPSAltitudeRef)value[0]);
                        case 7:
                            return new GPSTimeStamp(ExifTag.GPSTimeStamp, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        case 9:
                            return new ExifEnumProperty<GPSStatus>(ExifTag.GPSStatus, (GPSStatus)value[0]);
                        case 10:
                            return new ExifEnumProperty<GPSMeasureMode>(ExifTag.GPSMeasureMode, (GPSMeasureMode)value[0]);
                        case 12:
                            return new ExifEnumProperty<GPSSpeedRef>(ExifTag.GPSSpeedRef, (GPSSpeedRef)value[0]);
                        case 14:
                            return new ExifEnumProperty<GPSDirectionRef>(ExifTag.GPSTrackRef, (GPSDirectionRef)value[0]);
                        case 16:
                            return new ExifEnumProperty<GPSDirectionRef>(ExifTag.GPSImgDirectionRef, (GPSDirectionRef)value[0]);
                        case 19:
                            return new ExifEnumProperty<GPSLatitudeRef>(ExifTag.GPSDestLatitudeRef, (GPSLatitudeRef)value[0]);
                        case 20:
                            return new GPSLatitudeLongitude(ExifTag.GPSDestLatitude, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        case 21:
                            return new ExifEnumProperty<GPSLongitudeRef>(ExifTag.GPSDestLongitudeRef, (GPSLongitudeRef)value[0]);
                        case 22:
                            return new GPSLatitudeLongitude(ExifTag.GPSDestLongitude, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder));
                        case 23:
                            return new ExifEnumProperty<GPSDirectionRef>(ExifTag.GPSDestBearingRef, (GPSDirectionRef)value[0]);
                        case 25:
                            return new ExifEnumProperty<GPSDistanceRef>(ExifTag.GPSDestDistanceRef, (GPSDistanceRef)value[0]);
                        case 29:
                            return new ExifDate(ExifTag.GPSDateStamp, ExifBitConverter.ToDateTime(value, false));
                        case 30:
                            return new ExifEnumProperty<GPSDifferential>(ExifTag.GPSDifferential, (GPSDifferential)conv.ToUInt16(value, 0));
                    }
                    break;
                case IFD.Interop:
                    switch (tag) // InteroperabilityIndex
                    {
                        case 1:
                            return new ExifAscii(ExifTag.InteroperabilityIndex, ExifBitConverter.ToAscii(value, Encoding.ASCII), Encoding.ASCII);
                        case 2:
                            return new ExifVersion(ExifTag.InteroperabilityVersion, ExifBitConverter.ToAscii(value, Encoding.ASCII));
                    }
                    break;
                case IFD.First:
                    switch (tag) // Compression
                    {
                        case 0x103:
                            return new ExifEnumProperty<Compression>(ExifTag.ThumbnailCompression, (Compression)conv.ToUInt16(value, 0));
                        case 0x106:
                            return new ExifEnumProperty<PhotometricInterpretation>(ExifTag.ThumbnailPhotometricInterpretation, (PhotometricInterpretation)conv.ToUInt16(value, 0));
                        case 0x112:
                            return new ExifEnumProperty<Orientation>(ExifTag.ThumbnailOrientation, (Orientation)conv.ToUInt16(value, 0));
                        case 0x11c:
                            return new ExifEnumProperty<PlanarConfiguration>(ExifTag.ThumbnailPlanarConfiguration, (PlanarConfiguration)conv.ToUInt16(value, 0));
                        case 0x213:
                            return new ExifEnumProperty<YCbCrPositioning>(ExifTag.ThumbnailYCbCrPositioning, (YCbCrPositioning)conv.ToUInt16(value, 0));
                        case 0x128:
                            return new ExifEnumProperty<ResolutionUnit>(ExifTag.ThumbnailResolutionUnit, (ResolutionUnit)conv.ToUInt16(value, 0));
                        case 0x132:
                            return new ExifDateTime(ExifTag.ThumbnailDateTime, ExifBitConverter.ToDateTime(value));
                    }
                    break;
            }

            switch (type) // 1 = BYTE An 8-bit unsigned integer.
            {
                case 1:
                    return count switch
                    {
                        1 => new ExifByte(etag, value[0]),
                        _ => new ExifByteArray(etag, value)
                    };
                case 2:
                    return new ExifAscii(etag, ExifBitConverter.ToAscii(value, encoding), encoding);
                case 3:
                    return count switch
                    {
                        1 => new ExifUShort(etag, conv.ToUInt16(value, 0)),
                        _ => new ExifUShortArray(etag, ExifBitConverter.ToUShortArray(value, (int)count, byteOrder))
                    };
                case 4:
                    return count switch
                    {
                        1 => new ExifUInt(etag, conv.ToUInt32(value, 0)),
                        _ => new ExifUIntArray(etag, ExifBitConverter.ToUIntArray(value, (int)count, byteOrder))
                    };
                case 5:
                    return count switch
                    {
                        1 => new ExifURational(etag, ExifBitConverter.ToURational(value, byteOrder)),
                        _ => new ExifURationalArray(etag, ExifBitConverter.ToURationalArray(value, (int)count, byteOrder))
                    };
                case 6:
                    {
                        switch (count)
                        {
                            case 1:
                                return new ExifSByte(etag, (sbyte)value[0]);
                            default:
                                {
                                    sbyte[] data = new sbyte[count];
                                    Buffer.BlockCopy(value, 0, data, 0, (int)count);
                                    return new ExifSByteArray(etag, data);
                                }
                        }
                    }
                case 7:
                    return new ExifUndefined(etag, value);
                case 8:
                    return count switch
                    {
                        1 => new ExifSShort(etag, conv.ToInt16(value, 0)),
                        _ => new ExifSShortArray(etag, ExifBitConverter.ToSShortArray(value, (int)count, byteOrder))
                    };
                case 9:
                    return count switch
                    {
                        1 => new ExifSInt(etag, conv.ToInt32(value, 0)),
                        _ => new ExifSIntArray(etag, ExifBitConverter.ToSIntArray(value, (int)count, byteOrder))
                    };
                case 10:
                    return count switch
                    {
                        1 => new ExifSRational(etag, ExifBitConverter.ToSRational(value, byteOrder)),
                        _ => new ExifSRationalArray(etag, ExifBitConverter.ToSRationalArray(value, (int)count, byteOrder))
                    };
                case 11:
                    return count switch
                    {
                        1 => new ExifFloat(etag, conv.ToSingle(value, 0)),
                        _ => new ExifFloatArray(etag, ExifBitConverter.ToSingleArray(value, (int)count, byteOrder))
                    };
                case 12:
                    return count switch
                    {
                        1 => new ExifDouble(etag, conv.ToDouble(value, 0)),
                        _ => new ExifDoubleArray(etag, ExifBitConverter.ToDoubleArray(value, (int)count, byteOrder))
                    };
                default:
                    throw new ArgumentException("Unknown property type.");
            }
        }
    }
}