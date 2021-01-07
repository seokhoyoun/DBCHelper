using System;
using System.Collections.Generic;
using System.Text;

namespace DBCHelper
{
    
    public enum EAccessType
    {
        Unrestricted = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3
    }



    public class EnvironmentVariableCAN
    {
        public uint ID { get; set; }

        public string Name { get; set; }

        public string Maximum { get; set; }

        public string Minimum { get; set; }

        public string Unit { get; set; }

        public string InitialValue { get; set; }

        public EAccessType AccessType { get; set; }

        public EVariableType VariableType { get; set; }

        public string AccessNode { get; set; }
    }
}
