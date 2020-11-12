using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace DBCHelper
{
    public enum EByteOrder
    {
        Intel,
        Motorola
    }

    public enum EValueType
    {
        SignedType,
        UnsignedType
    }

    public enum EVariableType
    {
        IntegerType,
        FloatType,
        DoubleType,
        StringType
    }

    public class SignalCAN
    {
        public string SignalName { get; set; }

        public string MessageName { get; set; }

        public uint ID { get; set; }

        public uint StartBit { get; set; }

        public uint Length { get; set; }

        public EByteOrder ByteOrder { get; set; }

        public EValueType ValueType { get; set; }

        public double Maximum { get; set; }

        public double Minimum { get; set; }

        public double Offset { get; set; }

        public double Factor { get; set; }

        public double InitialValue { get; set; }

        public string Unit { get; set; }

        public string ReceiverName { get; set; }

        public string Transmitter { get; set; }

        public string Comment { get; set; }

        public ValueTableCAN ValueTable { get; set; }  

        public IList<AttributeCAN> AttributeList { get; private set; } = new List<AttributeCAN>();

    }
}
