using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    public class SideMenuViewModel : ViewModelBase
    {
        public ICommand Command { get; private set; }

        public IList<SideMenuItemViewModel> SubItems { get; private set; } 

        public SideMenuViewModel(string displayName, ICommand command, IList<SideMenuItemViewModel> viewModels)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
            SubItems = viewModels;

        }

    }
}
