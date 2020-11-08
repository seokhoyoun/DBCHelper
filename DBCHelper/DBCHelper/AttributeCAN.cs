using System;
using System.Collections.Generic;
using System.Text;

namespace DBCHelper
{
    public enum EAttributeType
    {
        Network,
        Node,
        Message,
        Signal
    }

    public enum EAttributeValueType
    {
        EnumType,
        StringType,
        IntegerType,
        HexType,
        FloatType
    }

    public class AttributeCAN
    {
        public string AttributeName
        {
            get;
            set;
        }

        public EAttributeType AttributeType
        {
            get;
            set;
        }

        public EAttributeValueType AttributeValueType
        {
            get;
            set;
        }

        public string Minimum
        {
            get;
            set;
        }

        public string Maximum
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }

        public string Default
        {
            get;
            set;
        } = string.Empty;

        public IList<string> EnumValueList
        {
            get;
            private set;
        } = new List<string>();

    }
}
