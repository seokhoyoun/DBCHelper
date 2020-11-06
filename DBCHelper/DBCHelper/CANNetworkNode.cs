using System;
using System.Collections.Generic;
using System.Text;

namespace DBCHelper
{
    public class CANNetworkNode
    {
        #region Public Properties

        public string NodeName { get; set; }

        public List<CANSignal> SignalList { get; set; } = new List<CANSignal>();

        public List<CANAttribute> AttributeList { get; set; } = new List<CANAttribute>();

        #endregion
    }
}
