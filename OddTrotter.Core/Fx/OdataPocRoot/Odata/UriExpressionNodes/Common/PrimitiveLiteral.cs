///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using System;
    
    public abstract class PrimitiveLiteral
    {
        private PrimitiveLiteral()
        {
        }

        public sealed class NullValue : PrimitiveLiteral
        {
            public NullValue()
            {
                //// TODO singletons for all of these literal derived types?
            }
        }

        public sealed class BooleanValueNode : PrimitiveLiteral
        {
            public BooleanValueNode(BooleanValue booleanValue)
            {
                BooleanValue = booleanValue;
            }

            public BooleanValue BooleanValue { get; }
        }

        public sealed class GuidValue : PrimitiveLiteral
        {
            public GuidValue(Guid guid)
            {
                //// TODO does it make sense to actually follow the ABNF for things like guidvalue, datevalue, etc?
                Guid = guid;
            }

            public Guid Guid { get; }
        }

        public sealed class DateValue : PrimitiveLiteral
        {
            public DateValue(DateOnly date)
            {
                Date = date;
            }

            public DateOnly Date { get; }
        }

        public sealed class DateTimeOffsetValue : PrimitiveLiteral
        {
            public DateTimeOffsetValue(DateTimeOffset dateTimeOffset)
            {
                DateTimeOffset = dateTimeOffset;
            }

            public DateTimeOffset DateTimeOffset { get; }
        }

        public sealed class TimeOfDayValue : PrimitiveLiteral
        {
            public TimeOfDayValue(TimeOnly timeOfDay)
            {
                TimeOfDay = timeOfDay;
            }

            public TimeOnly TimeOfDay { get; }
        }

        public sealed class DecimalValue : PrimitiveLiteral
        {
            public DecimalValue(decimal @decimal)
            {
                //// TODO for all of these numeric type, does the c# type "mean the same thing" as the EDM type?
                Decimal = @decimal;
            }

            public decimal Decimal { get; }
        }

        public sealed class DoubleValue : PrimitiveLiteral
        {
            public DoubleValue(double @double)
            {
                Double = @double;
            }

            public double Double { get; }
        }

        public sealed class SingleValue : PrimitiveLiteral
        {
            public SingleValue(float single)
            {
                Single = single;
            }

            public float Single { get; }
        }

        public sealed class SbyteValue : PrimitiveLiteral
        {
            public SbyteValue(sbyte @sbyte)
            {
                Sbyte = @sbyte;
            }

            public sbyte Sbyte { get; }
        }

        public sealed class ByteValue : PrimitiveLiteral
        {
            public ByteValue(byte @byte)
            {
                Byte = @byte;
            }

            public byte Byte { get; }
        }

        public sealed class Int16Value : PrimitiveLiteral
        {
            public Int16Value(short int16)
            {
                Int16 = int16;
            }

            public short Int16 { get; }
        }

        public sealed class Int32Value : PrimitiveLiteral
        {
            public Int32Value(int int32)
            {
                Int32 = int32;
            }

            public int Int32 { get; }
        }

        public sealed class Int64Value : PrimitiveLiteral
        {
            public Int64Value(long int64)
            {
                Int64 = int64;
            }

            public long Int64 { get; }
        }

        public sealed class StringValue : PrimitiveLiteral
        {
            public StringValue(string @string)
            {
                String = @string;
            }

            public string String { get; }
        }

        public sealed class DurationValue : PrimitiveLiteral
        {
            public DurationValue(TimeSpan duration)
            {
                Duration = duration;
            }

            public TimeSpan Duration { get; }
        }

        public abstract class EnumValue : PrimitiveLiteral
        {
            private EnumValue()
            {
                throw new Exception("TODO");
            }
        }

        public sealed class BinaryValue : PrimitiveLiteral
        {
            public BinaryValue(byte[] binary)
            {
                Binary = binary;
            }

            public byte[] Binary { get; }
        }

        ///// TODO do geograph and geometry nodes here 
    }
}
