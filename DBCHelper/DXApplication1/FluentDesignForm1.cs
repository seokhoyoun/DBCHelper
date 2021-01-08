using DBCHelper;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class FluentDesignForm1 : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
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

        public FluentDesignForm1()
        {
            InitializeComponent();

            InitializeCAN();

            InitializeTreeView();
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
            this.accordionControl1.BeginUpdate();

            AccordionControlElement acRootGroupHome = new AccordionControlElement();
            AccordionControlElement acItemActivity = new AccordionControlElement();
            AccordionControlElement acItemNews = new AccordionControlElement();
            AccordionControlElement acRootItemSettings = new AccordionControlElement();

            //acControl.ElementClick += new ElementClickEventHandler(this.accordionControl1_ElementClick);
            AccordionControlElement[] rangeArr = new AccordionControlElement[parser.NetworkNodeDictionary.Count];
            int rangeArrIndex = 0;
            foreach (var node in parser.NetworkNodeDictionary)
            {
                AccordionControlElement tempNodeElement = new AccordionControlElement();
                tempNodeElement.Name = node.Value.NodeName;
                tempNodeElement.Style = ElementStyle.Item;
                tempNodeElement.Tag = node.Value;
                tempNodeElement.Text = node.Value.NodeName;

                accordionControl1.ElementClick += AccordionControl1_ElementClick;

                rangeArr[rangeArrIndex++] = tempNodeElement;
            }

            accordionControlElement1.Elements.AddRange(rangeArr);

            accordionControl1.EndUpdate();

            //TreeNode nodeNetwork = new TreeNode(NETWORKS_NODE_NAME);
            //foreach (var node in parser.NetworkNodeDictionary)
            //{
            //    nodeNetwork.Nodes.Add(node.Value.NodeName);
            //}

            //TreeNode attributes = new TreeNode(ATTRIBUTES_NAME);

            //foreach (var attribute in parser.AttributeDictionary)
            //{
            //    attributes.Nodes.Add(attribute.Key, attribute.Value.AttributeName, 1, 1);
            //}

            //TreeNode valueTables = new TreeNode(VALUE_TABLES_NAME);

            //foreach (var valueTable in parser.ValueTableDictionary)
            //{
            //    valueTables.Nodes.Add(valueTable.Key, valueTable.Value.ValueTableName, 2, 2);
            //}




        }

        private void AccordionControl1_ElementClick(object sender, ElementClickEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
