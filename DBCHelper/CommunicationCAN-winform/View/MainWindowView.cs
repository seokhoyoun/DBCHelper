using DBCHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        private readonly string IMAGE_PATH =  $"..\\..\\..\\Images";

        private const string NETWORKS_NODE_NAME = "Nodes";
        private const string ATTRIBUTES_NAME = "Attributes";
        private const string VALUE_TABLES_NAME = "ValueTables";

        private const int NETWORKS_IMAGE_INDEX = 0;
        private const int ATTRIBUTES_IMAGE_INDEX = 1;
        private const int VALUE_TABLES_IMAGE_INDEX = 2;



        #endregion

        #region Constructor

        public MainWindowView()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            initializeCAN();

            InitializeTreeView();

            base.OnLoad(e);
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

       

        public void InitializeTreeView()
        {
            ImageList imageList = new ImageList();
            imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\barcode.png"));
            imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\check.png"));
            imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\done.png"));

            treeView1.ImageList = imageList;

            TreeNode nodeNetwork = new TreeNode(NETWORKS_NODE_NAME, NETWORKS_IMAGE_INDEX, NETWORKS_IMAGE_INDEX);

            foreach (var node in parser.NetworkNodeDictionary)
            {
                nodeNetwork.Nodes.Add(node.Key, node.Value.NodeName, 0, 0);
            }

            TreeNode attributes = new TreeNode(ATTRIBUTES_NAME, ATTRIBUTES_IMAGE_INDEX, ATTRIBUTES_IMAGE_INDEX);

            foreach(var attribute in parser.AttributeDictionary)
            {
                attributes.Nodes.Add(attribute.Key, attribute.Value.AttributeName, 1, 1);
            }

            TreeNode valueTables = new TreeNode(VALUE_TABLES_NAME, VALUE_TABLES_IMAGE_INDEX, VALUE_TABLES_IMAGE_INDEX);

            foreach (var valueTable in parser.ValueTableDictionary)
            {
                valueTables.Nodes.Add(valueTable.Key, valueTable.Value.ValueTableName, 2, 2);
            }

            treeView1.Nodes.Add(nodeNetwork);
            treeView1.Nodes.Add(attributes);
            treeView1.Nodes.Add(valueTables);

        }


        #endregion

        #region Private Method

        #region Event

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            string itemString = e.Item.Text;

            uint selectedMessageID = (uint)e.Item.Tag;

            DataTable signalTable = new DataTable();

            signalTable.Columns.Add("Signal Name");
            signalTable.Columns.Add("ID(hex)");
            signalTable.Columns.Add("Start Bit");
            signalTable.Columns.Add("Length");
            signalTable.Columns.Add("Byte Order");


            foreach (var signal in parser.MessageDictionary[selectedMessageID].SignalList)
            {
                DataRow row = signalTable.NewRow();

                row[0] = signal.SignalName;
                row[1] = $"0x{signal.ID:X}";
                row[2] = signal.StartBit;
                row[3] = signal.Length;
                row[4] = signal.ByteOrder;

                signalTable.Rows.Add(row);
            }

            dataGridView1.DataSource = signalTable;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //e.Node.Parent
            if (e.Node.Parent == null)
            {
                // Parent Node View
                switch(e.Node.Text)
                {
                    case NETWORKS_NODE_NAME:
                        displayNodesToWorkspace();
                        break;

                    case ATTRIBUTES_NAME:
                        displayAttributesToWorkspace();
                        break;

                    case VALUE_TABLES_NAME:
                        displayValueTablesToWorkspace();
                        break;

                    default: 
                        Debug.Assert(false); 
                        break;
                }

            }
            else
            {
                // Child Node View
            }
        }

        #endregion

        #region Helper

        private void displayNodesToWorkspace()
        {
            DataTable signalTable = new DataTable();

            signalTable.Columns.Add("Node Name");


            DataColumn column = new DataColumn();
            column.DataType = typeof(IList<SignalCAN>);
            column.ColumnName = "Signal List";
            signalTable.Columns.Add(column);


            foreach (var node in parser.NetworkNodeDictionary)
            {
                DataRow row = signalTable.NewRow();

                row[0] = node.Value.NodeName;
                row[1] = node.Value.SignalList;

                signalTable.Rows.Add(row);
            }

            dataGridView1.Columns[1].Width = 100;

            dataGridView1.DataSource = signalTable;
        }

        private void displayAttributesToWorkspace()
        {

        }

        private void displayValueTablesToWorkspace()
        {

        }

        private void displayCANMessageToWorkspace()
        {
            
        }

        #endregion

        #endregion

    }
}
