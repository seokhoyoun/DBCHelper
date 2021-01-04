using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunicationCAN_winform
{
    public partial class MainWindowView : Form
    {
        #region Public Properties

        public List<string> NodeStringList
        {
            get;
            private set;
        } = new List<string>();

        #endregion

        #region Constructor

        public MainWindowView()
        {
            InitializeComponent();

        }

        #endregion

        #region Public Methods

        public void InitializeNodeListView()
        {
            NodeStringList.Add("A1");
            NodeStringList.Add("A2");
            NodeStringList.Add("A3");
            NodeStringList.Add("A4");
            NodeStringList.Add("A5");

        }

        #endregion




    }
}
