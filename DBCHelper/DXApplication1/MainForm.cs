using DBCHelper;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        #region Field

        private DBCParser parser = new DBCParser();

        private readonly string IMAGE_PATH = $"..\\..\\..\\Images";

        private const string NETWORKS_NODE_NAME = "Nodes";
        private const string ATTRIBUTES_NAME = "Attributes";
        private const string VALUE_TABLES_NAME = "ValueTables";

        private const int NETWORKS_IMAGE_INDEX = 0;
        private const int ATTRIBUTES_IMAGE_INDEX = 1;
        private const int VALUE_TABLES_IMAGE_INDEX = 2;

        // Specifies whether the root nodes that represent the local drivers are created.
        private bool loadDrives = false;

        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            InitializeCAN();

            InitializeTreeView();


            base.OnLoad(e);

        }

        public void InitializeCAN()
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
            //ImageList imageList = new ImageList();
            //imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\barcode.png"));
            //imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\check.png"));
            //imageList.Images.Add(Bitmap.FromFile($"{IMAGE_PATH}\\done.png"));

            //treeView1.ImageList = imageList;

            //treeList1 = new TreeList();

            //((ISupportInitialize)treeList1).BeginInit();

            TreeNode nodeNetwork = new TreeNode(NETWORKS_NODE_NAME);
            foreach (var node in parser.NetworkNodeDictionary)
            {
                nodeNetwork.Nodes.Add(node.Value.NodeName);
            }

            TreeNode attributes = new TreeNode(ATTRIBUTES_NAME);

            foreach (var attribute in parser.AttributeDictionary)
            {
                attributes.Nodes.Add(attribute.Key, attribute.Value.AttributeName, 1, 1);
            }

            TreeNode valueTables = new TreeNode(VALUE_TABLES_NAME);

            foreach (var valueTable in parser.ValueTableDictionary)
            {
                valueTables.Nodes.Add(valueTable.Key, valueTable.Value.ValueTableName, 2, 2);
            }

            treeView1.Nodes.Add(nodeNetwork);
            treeView1.Nodes.Add(attributes);
            treeView1.Nodes.Add(valueTables);


        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //treeList1.DataSource = new object();
        }



        //Indicates whether the processed node corresponds to the file.
        bool IsFile(DirectoryInfo info)
        {
            return (info.Attributes & FileAttributes.Directory) == 0;
        }

        // Initializes cell values.
        private void treeList1_VirtualTreeGetCellValue(object sender,
        DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo e)
        {
            DirectoryInfo di = new DirectoryInfo((string)e.Node);
            if (e.Column == treeListColumn1)
                e.CellData = di.Name;
            if (e.Column == treeListColumn2)
            {
                if (!IsFile(di))
                    e.CellData = "Folder";
                else
                    e.CellData = "File";
            }
            if (e.Column == treeListColumn3)
            {
                if (IsFile(di))
                {
                    e.CellData = new FileInfo((string)e.Node).Length;
                }
                else e.CellData = null;
            }
        }

        // Creates and initializes child nodes.
        private void treeList1_VirtualTreeGetChildNodes(object sender,
        DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo e)
        {
            if (!loadDrives)
            { // create drives
                string[] root = Directory.GetLogicalDrives();
                e.Children = root;
                loadDrives = true;
            }
            else
            {
                try
                {
                    string path = (string)e.Node;
                    if (Directory.Exists(path))
                    {
                        string[] dirs = Directory.GetDirectories(path);
                        string[] files = Directory.GetFiles(path);
                        string[] arr = new string[dirs.Length + files.Length];
                        dirs.CopyTo(arr, 0);
                        files.CopyTo(arr, dirs.Length);
                        e.Children = arr;
                    }
                    else e.Children = new object[] { };
                }
                catch { e.Children = new object[] { }; }
            }
        }
    }
}