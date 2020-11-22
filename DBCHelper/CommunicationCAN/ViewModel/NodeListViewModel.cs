using DBCHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommunicationCAN.ViewModel
{
    public class NodeListViewModel : WorkspaceViewModel
    {
        private ObservableCollection<NetworkNodeCAN> _nodeCollection = new ObservableCollection<NetworkNodeCAN>();
        public ObservableCollection<NetworkNodeCAN> NodeCollection
        {
            get;
            private set;
        }

        public NodeListViewModel(Dictionary<string, NetworkNodeCAN> nodes, string displayName)
        {
            base.DisplayName = displayName;

            foreach (var node in nodes)
            {
                _nodeCollection.Add(node.Value);
            }

            NodeCollection = _nodeCollection;

        }
    }
}
