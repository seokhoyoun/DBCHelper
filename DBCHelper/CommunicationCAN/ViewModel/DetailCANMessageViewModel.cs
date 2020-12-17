using DBCHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationCAN.ViewModel
{
    public class DetailCANMessageViewModel : WorkspaceViewModel
    {
        public DetailCANMessageViewModel(MessageCAN message)
        {
            base.DisplayName = "Detail Message";
        }
    }
}
