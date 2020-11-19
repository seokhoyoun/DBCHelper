using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    public class SideMenuViewModel : ViewModelBase
    {
        public SideMenuViewModel(string displayName, ICommand command, ObservableCollection<SideMenuItemViewModel> viewModels, PackIconKind icon)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
            SubItems = viewModels;
            Icon = icon;
        }

        public PackIconKind Icon { get; private set; }

        public ICommand Command { get; private set; }

        public IList<SideMenuItemViewModel> SubItems { get; private set; } 
    }
}
