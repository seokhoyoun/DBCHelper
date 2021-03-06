﻿using CommunicationCAN.Properties;
using DBCHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CommunicationCAN.ViewModel
{
    public class SignalListViewModel : WorkspaceViewModel
    {
        private ObservableCollection<SignalCAN> _signalCollection = new ObservableCollection<SignalCAN>();
        public ObservableCollection<SignalCAN> SignalCollection
        {
            get;
            private set;
        }

        public SignalListViewModel(IList<SignalCAN> signals, string displayName)
        {
            base.DisplayName = displayName;

            foreach (var signal in signals)
            {
                _signalCollection.Add(signal);
            }

            SignalCollection = _signalCollection;
        }

        protected override void OnDispose()
        {
            SignalCollection.Clear();
            SignalCollection = null;

        }
    }
}
