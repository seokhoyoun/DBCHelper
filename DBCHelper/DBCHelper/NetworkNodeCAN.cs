using System;
using System.Collections.Generic;
using System.Text;

namespace DBCHelper
{
    public class NetworkNodeCAN
    {
        #region Public Properties

        public string NodeName { get; set; }

        public IList<SignalCAN> SignalList { get; private set; } = new List<SignalCAN>();

        public IList<AttributeCAN> AttributeList { get; private set; } = new List<AttributeCAN>();

        #endregion
    }
}
