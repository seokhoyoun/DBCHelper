using DBCHelper;
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



        #endregion

        #region Field

        private DBCParser parser = new DBCParser();

        

        #endregion

        #region Constructor

        public MainWindowView()
        {
            InitializeComponent();

            initializeCAN();

            InitializeMainListView();
        }

        #endregion

        #region Public Methods

        public void initializeCAN()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "dbc Worksheets|*.dbc";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;

                parser.ClearCollections();

                parser.LoadFile(filePath);

            }
        }

        public void InitializeMainListView()
        {
            foreach(var item in parser.MessageDictionary)
            {
                listView1.Items.Add(item.Value.MessageName);
            }
        }


        #endregion


        #region Event



        #endregion

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            string itemString = e.Item.Text;

            panel1.BackColor = Color.White;
            
        }
    }
}
