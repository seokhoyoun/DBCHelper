using DBCHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommunicationCAN.ViewModel
{
    public class MessageListViewModel : WorkspaceViewModel
    {
        private ObservableCollection<MessageCAN> _messageCollection = new ObservableCollection<MessageCAN>();

        public ObservableCollection<MessageCAN> MessageCollection
        {
            get;
            private set;
        }

        public MessageListViewModel(Dictionary<uint, MessageCAN> messages, string displayName)
        {
            base.DisplayName = displayName;

            foreach (var message in messages)
            {
                _messageCollection.Add(message.Value);
            }

            MessageCollection = _messageCollection;
        }
    }
}
