using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.Drawing;

namespace DXApplication1
{
    public partial class RibbonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public RibbonForm()
        {
            InitializeComponent();

            this.ribbonControl1.ApplicationCaption = "HELLO";
            this.ribbonControl1.ApplicationDocumentCaption = "WORLD";

            DefaultBarAndDockingController defaultBarAndDockingController1 = new DefaultBarAndDockingController();
            defaultBarAndDockingController1.Controller.AppearancesRibbon.FormCaption.ForeColor = Color.Red;
            defaultBarAndDockingController1.Controller.AppearancesRibbon.FormCaptionForeColor2 = Color.Blue;

            WindowsFormsSettings.FormThickBorder = true;
            WindowsFormsSettings.ThickBorderWidth = 10;

            barStaticItem1.Caption = "HELLO";
        }
    }
}
