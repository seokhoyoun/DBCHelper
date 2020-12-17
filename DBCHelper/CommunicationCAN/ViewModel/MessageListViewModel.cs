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

        private MessageCAN _selectedMessageCAN;
        public MessageCAN SelectedMessage
        {
            get { return _selectedMessageCAN; }
            set
            {
                _selectedMessageCAN = value;
                mMainWindowviewModel.DisplaySelectedMessage(value);
            }
        }

        private MainWindowViewModel mMainWindowviewModel;

        public MessageListViewModel(Dictionary<uint, MessageCAN> messages, string displayName, MainWindowViewModel mainWindowViewModel)
        {
            base.DisplayName = displayName;
            mMainWindowviewModel = mainWindowViewModel;

            foreach (var message in messages)
            {
                MessageCollection.Add(message.Value);
            }
        }


      
    }
}
