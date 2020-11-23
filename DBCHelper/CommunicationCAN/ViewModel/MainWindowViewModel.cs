using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using CommunicationCAN.DataAccess;
using CommunicationCAN.Model;
using CommunicationCAN.Properties;
using DBCHelper;
using MaterialDesignThemes.Wpf;

namespace CommunicationCAN.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Fields
        private DBCParser mDbcParser;


        private ReadOnlyCollection<CommandViewModel> mSideMenuCommands;
        private ReadOnlyCollection<CommandViewModel> mFooterMenuCommands;

        private ObservableCollection<WorkspaceViewModel> mWorkSpaces;
        private ObservableCollection<WorkspaceViewModel> mFooterWorkspaces;

        ReadOnlyCollection<CommandViewModel> _commands;
        readonly CustomerRepository _customerRepository;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel(string customerDataFile)
        {
            base.DisplayName = Strings.MainWindowViewModel_DisplayName;

            mDbcParser = new DBCParser();
            mDbcParser.LoadFile();

            _customerRepository = new CustomerRepository(customerDataFile);
        }

        #endregion // Constructor

        #region Commands

        public ReadOnlyCollection<CommandViewModel> SideMenuCommands
        {
            get
            {
                if (mSideMenuCommands == null)
                {
                    List<CommandViewModel> cmds = this.CreateSideMenuCommands();
                    mSideMenuCommands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return mSideMenuCommands;
            }
        }

        public ReadOnlyCollection<CommandViewModel> FooterMenuCommands
        {
            get
            {
                if (mFooterMenuCommands == null)
                {
                    List<CommandViewModel> cmds = this.CreateFooterMenuCommands();
                    mFooterMenuCommands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return mFooterMenuCommands;
            }
        }

        private List<CommandViewModel> CreateSideMenuCommands()
        {
            var nodes = mDbcParser.NetworkNodeDictionary;
            var messages = mDbcParser.MessageDictionary;

            List<CommandViewModel> sideMenuList = new List<CommandViewModel>();

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
                command: new RelayCommand(param => ShowWorkspaceView(new MessageListViewModel(messages, "Messages"))),
                subItems: messageSubItems,
                icon: PackIconKind.MailboxOpen);

            sideMenuList.Add(node);
            sideMenuList.Add(message);

            return sideMenuList;
        }

        private List<CommandViewModel> CreateFooterMenuCommands()
        {
            List<CommandViewModel> footerMenuList = new List<CommandViewModel>();

            CommandViewModel settingCmd = new CommandViewModel(
                displayName: "Setting",
                command: new RelayCommand(param => ShowFooterWorkspaceView(new CANCommunicationViewModel())),
                subItems: null,
                icon: PackIconKind.SettingsApplications
                );

            footerMenuList.Add(settingCmd);
            

            return footerMenuList;
        }

        #endregion // Commands

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

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();

            Workspaces.Remove(workspace);
            FooterWorkspaces.Remove(workspace);
        }

        #endregion // Workspaces

        #region Public Methods

   
        #endregion

        #region Private Helpers

        private void ShowWorkspaceView(WorkspaceViewModel workspaceViewModel)
        {
            this.Workspaces.Add(workspaceViewModel);

            SetActiveWorkspace(workspaceViewModel);
        }

        private void ShowFooterWorkspaceView(WorkspaceViewModel workspaceViewModel)
        {
            this.FooterWorkspaces.Add(workspaceViewModel);

            SetActiveFooterWorkspace(workspaceViewModel);
        }



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

        #endregion // Private Helpers
    }
}