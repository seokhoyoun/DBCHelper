using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
