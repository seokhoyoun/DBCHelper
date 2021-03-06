﻿using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    public class SideMenuItemViewModel : ViewModelBase
    {
        public ICommand Command { get; private set; }

        public SideMenuItemViewModel(string displayName, ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            base.DisplayName = displayName;
            Command = command;
        }

        protected override void OnDispose()
        {
            base.DisplayName = null;
            this.Command = null;
        }
    }
}
