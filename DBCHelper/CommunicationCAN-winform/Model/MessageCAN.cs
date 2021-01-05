using System.Collections.Generic;

namespace DBCHelper
{
    public class MessageCAN
    {
        #region Public Properties

        
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

        public IList<AttributeCAN> AttributeList
        {
            get;
            private set;
        } = new List<AttributeCAN>();

        public IList<SignalCAN> SignalList
        {
            get;
            private set;
        } = new List<SignalCAN>();


        public string Comment 
        {
            get;
            set;
        }

        #endregion

        #region Field

        private byte[] mMessageData;
   
        #endregion

        #region public Method

      

        public byte[] GetData()
        {
            return mMessageData;
        }

        public void SetData(byte[] value)
        {
            mMessageData = value;
        }

        #endregion

    }
}
