using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        public ICommand Command { get; private set; }

        public ObservableCollection<string> SubItems { get; private set; } = new ObservableCollection<string>();

        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
        }

    }
}