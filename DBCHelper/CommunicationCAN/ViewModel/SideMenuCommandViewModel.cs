using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    public class SidMenuCommandViewModel : ViewModelBase
    {
        public SidMenuCommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
        }

        public ICommand Command { get; private set; }

        public ObservableCollection<string> SubItems { get; private set; } = new ObservableCollection<string>();
    }
}
