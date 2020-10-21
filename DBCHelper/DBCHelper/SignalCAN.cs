using System;
using System.Collections.Generic;
using System.Text;

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
        StringType
    }

    public enum EAccessType
    {
        Unrestricted,
        Read,
        Write,
        ReadWrite
    }

    public class SignalCAN
    {
        public string NetworkNode { get; set; }
        public string SignalName { get; set; }
        
        public string MessageName { get; set; }
        public uint ID { get; set; }

        public int StartBit { get; set; }

        public int Length { get; set; }

        public EByteOrder ByteOrder { get; set; }

        public EValueType ValueType { get; set; }

        public double Maximum { get; set;}

        public double Minimum { get; set;}

        public double InitialValue { get; set; }

        public string Unit { get; set; }

        public string Comment { get; set; }



    }
}
