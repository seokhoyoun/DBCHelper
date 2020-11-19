using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        ReadOnlyCollection<SideMenuViewModel> mSideMenuCommands;
        ReadOnlyCollection<CommandViewModel> _commands;
        readonly CustomerRepository _customerRepository;
        ObservableCollection<WorkspaceViewModel> _workspaces;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel(string customerDataFile)
        {
            base.DisplayName = Strings.MainWindowViewModel_DisplayName;

            mDbcParser = new DBCParser();
            mDbcParser.LoadFile();

            //var nodes = mDbcParser.NetworkNodeDictionary;
            //var messages = mDbcParser.MessageDictionary;


            //List<string> nodeList = new List<string>();
            //foreach(var keyStr in nodes.Keys)
            //{
            //    nodeList.Add(keyStr);
            //}

            _customerRepository = new CustomerRepository(customerDataFile);
        }

        #endregion // Constructor

        #region Commands

        public ReadOnlyCollection<SideMenuViewModel> SideMenuCommands
        {
            get
            {
                if (mSideMenuCommands == null)
                {
                    List<SideMenuViewModel> cmds = this.CreateSideMenuCommands();
                    mSideMenuCommands = new ReadOnlyCollection<SideMenuViewModel>(cmds);
                }
                return mSideMenuCommands;
            }
        }

        List<SideMenuViewModel> CreateSideMenuCommands()
        {
            var nodes = mDbcParser.NetworkNodeDictionary;
            var sideMenuList = new List<SideMenuViewModel>();

          
                foreach (var node in nodes)
                {
                    List<SignalCAN> signalList = (List<SignalCAN>)node.Value.SignalList;

                    List<SideMenuItemViewModel> subItems = new List<SideMenuItemViewModel>();

                    for (int i = 0; i < signalList.Count; i++)
                    {
                        subItems.Add(new SideMenuItemViewModel(
                            signalList[i].SignalName,
                            new RelayCommand(param => CreateNewCustomer())
                            ));
                    }

                    sideMenuList.Add(new SideMenuViewModel(
                        node.Key,
                        //new RelayCommand(param => ShowSignalView(signalList)),
                        new RelayCommand(param => ShowAllCustomers()),
                        subItems
                        ));

                    Thread.Sleep(10);
                }

            

            return sideMenuList;
            //return new List<SideMenuViewModel>
            //{
            //    new SideMenuViewModel(
            //        Strings.MainWindowViewModel_Command_ViewAllCustomers,
            //        new RelayCommand(param => this.ShowAllCustomers())),

            //    new SideMenuViewModel(
            //        Strings.MainWindowViewModel_Command_CreateNewCustomer,
            //        new RelayCommand(param => this.CreateNewCustomer()))
            //};
        }

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewAllCustomers,
                    new RelayCommand(param => this.ShowAllCustomers())),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_CreateNewCustomer,
                    new RelayCommand(param => this.CreateNewCustomer()))
            };
        }

        #endregion // Commands

        #region Workspaces

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
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
            this.Workspaces.Remove(workspace);
        }

        #endregion // Workspaces

        #region Private Helpers

        private void ShowSignalView(List<SignalCAN> signalList)
        {
            SignalListViewModel signalListViewModel = new SignalListViewModel();
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
                this.Workspaces.FirstOrDefault(vm => vm is AllCustomersViewModel)
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

        #endregion // Private Helpers
    }
}