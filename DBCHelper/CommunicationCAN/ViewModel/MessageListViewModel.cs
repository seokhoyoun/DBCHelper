using DBCHelper;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommunicationCAN.ViewModel
{
    public class MessageListViewModel : WorkspaceViewModel
    {

        public ObservableCollection<MessageCAN> MessageCollection
        {
            get;
            private set;
        } = new ObservableCollection<MessageCAN>();


        public MessageListViewModel(Dictionary<uint, MessageCAN> messages, string displayName)
        {
            base.DisplayName = displayName;

            foreach (var message in messages)
            {
                MessageCollection.Add(message.Value);
            }
        }


      
    }
}
