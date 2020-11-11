using System;
using System.Collections.Generic;

namespace DBCHelper
{
    public class ValueTableCAN
    {
        public string ValueTableName
        {
            get;
            set;
        }

        public Dictionary<int, string> ValueTableDictionary
        {
            get;
            private set;
        } = new Dictionary<int, string>();

    }
}
