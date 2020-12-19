using DBCHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationCAN.ViewModel
{
    public class DetailCANMessageViewModel : WorkspaceViewModel
    {
        public MessageCAN CurrentMessage { get; set; }

        public DetailCANMessageViewModel(MessageCAN message)
        {
            base.DisplayName = "Detail Message";

            CurrentMessage = message;
        }
    }
}
