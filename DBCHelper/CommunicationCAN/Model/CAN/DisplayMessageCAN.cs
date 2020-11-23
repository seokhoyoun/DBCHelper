using DBCHelper;
using System.Diagnostics;

namespace CommunicationCAN.Model.CAN
{
    public class DisplayMessageCAN : MessageCAN
    {
        public bool IsSelected
        {
            get;
            set;
        }

        public DisplayMessageCAN(MessageCAN message)
        {
            if(message == null)
            {
                Debug.Assert(false);
            }

            base.ID = message.ID;
            base.MessageName = message.MessageName;
            base.Transmitter = message.Transmitter;
            base.Comment = message.Comment;
            base.DLC = message.DLC;

            foreach (var item in message.SignalList)
            {
                base.SignalList.Add(item);
            }

            foreach (var item in message.AttributeList)
            {
                base.AttributeList.Add(item);
            }
        }
    }
}
