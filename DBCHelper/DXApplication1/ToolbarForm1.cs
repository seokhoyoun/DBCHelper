using DevExpress.XtraBars;
using DevExpress.XtraBars.ToolbarForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class ToolbarForm1 : DevExpress.XtraEditors.XtraForm
    {
        public ToolbarForm1()
        {
            InitializeComponent();

            ToolbarForm myForm = new ToolbarForm();

            myForm.Size = new Size(800, 600);
            myForm.Text = "Toolbar Form";

            ToolbarFormManager tfcManger = new ToolbarFormManager() { Form = myForm };
            ToolbarFormControl tfcHeader = new ToolbarFormControl() { ToolbarForm = myForm };

            myForm.Controls.Add(tfcHeader);
            myForm.ToolbarFormControl = tfcHeader;

            BarButtonItem item1 = new BarButtonItem(tfcManger, "Button 1");
            BarButtonItem item2 = new BarButtonItem(tfcManger, "Button 2");
            BarButtonItem item3 = new BarButtonItem(tfcManger, "Button 3");
            BarButtonItem item4 = new BarButtonItem(tfcManger, "Button 4");

            item3.Alignment = item4.Alignment = BarItemLinkAlignment.Right;

            tfcHeader.TitleItemLinks.AddRange(new BarItem[] { item1, item2, item3, item4 });

            myForm.Show();

        }
    }
}