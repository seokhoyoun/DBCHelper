﻿using CommunicationCAN.DataAccess;
using CommunicationCAN.Model;
using CommunicationCAN.Properties;
using DBCHelper;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace CommunicationCAN.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Public Properties

        #region Commands

        private RelayCommand _loadFileCommand;

        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                    _loadFileCommand = new RelayCommand(param => LoadFile());

                return _loadFileCommand;
            }
        }

        public ObservableCollection<CommandViewModel> LeftSideMenuCommands
        {
            get;
            private set;
        } = new ObservableCollection<CommandViewModel>();

        public ReadOnlyCollection<CommandViewModel> RightSideMenuCommands
        {
            get
            {
                if (mRightSideMenuCommands == null)
                {
                    List<CommandViewModel> cmds = this.CreateRightSideMenuCommands();
                    mRightSideMenuCommands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return mRightSideMenuCommands;
            }
        }

        #endregion Commands

        #endregion Public Properties

        #region Fields

        private DBCParser mDbcParser;

        //private ObservableCollection<CommandViewModel> mSideMenuCommands;
        private ReadOnlyCollection<CommandViewModel> mRightSideMenuCommands;

        private ObservableCollection<WorkspaceViewModel> mWorkSpaces;
        private ObservableCollection<WorkspaceViewModel> mFooterWorkspaces;

        private DetailCANMessageViewModel mDetailCANMessageViewModel;

        private ReadOnlyCollection<CommandViewModel> _commands;
        private readonly CustomerRepository _customerRepository;

        #endregion Fields

        #region Constructor

        public MainWindowViewModel(string customerDataFile)
        {
            base.DisplayName = Strings.MainWindowViewModel_DisplayName;

            mDbcParser = new DBCParser();
            //mDbcParser.LoadFile();

            _customerRepository = new CustomerRepository(customerDataFile);
        }

        #endregion Constructor

        #region Workspaces

        public ObservableCollection<WorkspaceViewModel> FooterWorkspaces
        {
            get
            {
                if (mFooterWorkspaces == null)
                {
                    mFooterWorkspaces = new ObservableCollection<WorkspaceViewModel>();
                    mFooterWorkspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return mFooterWorkspaces;
            }
        }

        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (mWorkSpaces == null)
                {
                    mWorkSpaces = new ObservableCollection<WorkspaceViewModel>();
                    mWorkSpaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return mWorkSpaces;
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();

            Workspaces.Remove(workspace);
            FooterWorkspaces.Remove(workspace);
        }

        #endregion Workspaces

        #region Public Methods

        public void LoadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "dbc Worksheets|*.dbc";

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;

                mDbcParser.ClearCollections();

                mDbcParser.LoadFile(filePath);

                LeftSideMenuCommands.Clear();

                CreateLeftSideMenuCommands();
            }
        }

        public void DisplaySelectedMessage(MessageCAN message)
        {
            if(mDetailCANMessageViewModel == null)
            {
                mDetailCANMessageViewModel = new DetailCANMessageViewModel(message);
            }

            ShowFooterWorkspaceView(mDetailCANMessageViewModel);
        }

        #endregion Public Methods

        #region Private Helpers

        private void CreateLeftSideMenuCommands()
        {
            var nodes = mDbcParser.NetworkNodeDictionary;
            var messages = mDbcParser.MessageDictionary;

            //List<CommandViewModel> sideMenuList = new List<CommandViewModel>();

            List<CommandViewModel> nodeSubItems = new List<CommandViewModel>();
            List<CommandViewModel> messageSubItems = new List<CommandViewModel>();

            foreach (var item in nodes)
            {
                nodeSubItems.Add(new CommandViewModel(
                    displayName: item.Key,
                    command: new RelayCommand(param => ShowWorkspaceView(new SignalListViewModel(item.Value.SignalList, item.Key))),
                    subItems: null,
                    icon: PackIconKind.Signal));
            }

            CommandViewModel node = new CommandViewModel(
                displayName: "Nodes",
                command: new RelayCommand(param => ShowWorkspaceView(new NodeListViewModel(nodes, "Nodes"))),
                subItems: nodeSubItems,
                icon: PackIconKind.Package);

            foreach (var item in messages)
            {
                messageSubItems.Add(new CommandViewModel(
                    displayName: item.Value.MessageName,
                    command: new RelayCommand(param => ShowWorkspaceView(new SignalListViewModel(item.Value.SignalList, item.Value.MessageName))),
                    subItems: null,
                    icon: PackIconKind.Mail));
            }

            CommandViewModel message = new CommandViewModel(
                displayName: "Messages",
                command: new RelayCommand(param => ShowWorkspaceView(new MessageListViewModel(messages, "Messages", this))),
                subItems: messageSubItems,
                icon: PackIconKind.MailboxOpen);

            LeftSideMenuCommands.Add(node);
            LeftSideMenuCommands.Add(message);
        }

        private List<CommandViewModel> CreateRightSideMenuCommands()
        {
            List<CommandViewModel> menuList = new List<CommandViewModel>();

            // TODO:: JSON format load maybe?

            CommandViewModel settingCmd = new CommandViewModel(
                displayName: "Setting",
                command: new RelayCommand(param => ShowSettingView()),
                subItems: null,
                icon: PackIconKind.Menu
                );

            menuList.Add(settingCmd);

            CommandViewModel rawCanViewCmd = new CommandViewModel(
                displayName: "CAN Raw View",
                command: new RelayCommand(param => ShowFooterWorkspaceView(new CANCommunicationViewModel())),
                subItems: null,
                icon: PackIconKind.Walk
                );

            menuList.Add(rawCanViewCmd);

            CommandViewModel signalCanViewCmd = new CommandViewModel(
                displayName: "CAN Signal View",
                command: new RelayCommand(param => ShowFooterWorkspaceView(new CANCommunicationViewModel())),
                subItems: null,
                icon: PackIconKind.Signal
                );

            menuList.Add(signalCanViewCmd);

            return menuList;
        }

        private void ShowWorkspaceView(WorkspaceViewModel workspaceViewModel)
        {
            this.Workspaces.Add(workspaceViewModel);

            SetActiveWorkspace(workspaceViewModel);
        }

        private void ShowFooterWorkspaceView(WorkspaceViewModel workspaceViewModel)
        {
            WorkspaceViewModel workspace =
                this.FooterWorkspaces.FirstOrDefault(viewModel => viewModel is WorkspaceViewModel)
                as WorkspaceViewModel;

            if (workspace == null)
            {
                //workspace = new AllCustomersViewModel(_customerRepository);
                workspace = workspaceViewModel;
                this.FooterWorkspaces.Add(workspace);
            }

            SetActiveFooterWorkspace(workspaceViewModel);
        }

        private void ShowSettingView()
        {
            SettingViewModel workspace =
                this.FooterWorkspaces.FirstOrDefault(viewModel => viewModel is SettingViewModel)
                as SettingViewModel;

            if (workspace == null)
            {
                workspace = new SettingViewModel();
                this.FooterWorkspaces.Add(workspace);
            }

            SetActiveFooterWorkspace(workspace);
        }

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        private void SetActiveFooterWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.FooterWorkspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.FooterWorkspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        #endregion Private Helpers

        #region DEMO

        private void CreateNewCustomer()
        {
            Customer newCustomer = Customer.CreateNewCustomer();
            CustomerViewModel workspace = new CustomerViewModel(newCustomer, _customerRepository);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void ShowAllCustomers()
        {
            AllCustomersViewModel workspace =
                this.Workspaces.FirstOrDefault(viewModel => viewModel is AllCustomersViewModel)
                as AllCustomersViewModel;

            if (workspace == null)
            {
                workspace = new AllCustomersViewModel(_customerRepository);
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);
        }

        #endregion DEMO
    }
}