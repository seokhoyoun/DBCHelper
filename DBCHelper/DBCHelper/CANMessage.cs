using System.Collections.Generic;

namespace DBCHelper
{
    public class CANMessage
    {
        public uint ID
        {
            get;
            set;
        }

        public string MessageName
        {
            get;
            set;
        }

        public byte DLC
        {
            get;
            set;
        }

        public string Transmitter
        {
            get;
            set;
        }

        public List<CANAttribute> AttributeList 
        { 
            get;
            set;
        } = new List<CANAttribute>();

        public List<CANSignal> SignalLIst
        {
            get;
            set;
        } = new List<CANSignal>();


        public byte[] Data
        {
            get;
            set;
        }

        public string Comment 
        {
            get;
            set;
        }
        

    }
}
