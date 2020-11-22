using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

        public IList<CommandViewModel> SubItems { get; private set; }

        public PackIconKind Icon { get; private set; }

        public CommandViewModel(string displayName, ICommand command, IList<CommandViewModel> subItems, PackIconKind icon)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            Command = command;
            SubItems = subItems;
            Icon = icon;
        }

    }
}