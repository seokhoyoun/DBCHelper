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

        public IList<SideMenuViewModel> SubItems { get; private set; } 

        public SideMenuViewModel(string displayName, ICommand command, IList<SideMenuViewModel> viewModels)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
            SubItems = viewModels;

        }

        protected override void OnDispose()
        {
            foreach (SideMenuViewModel sideMenuItem in this.SubItems)
            {
                sideMenuItem.Dispose();
            }

            SubItems.Clear();

            SubItems = null;
            this.Command = null;
        }

    }
}
