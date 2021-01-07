using System.Windows.Forms;

namespace CommunicationCAN_winform.View.Unit
{
    public class DataGridViewRolloverCellColumn : DataGridViewColumn
    {
        public DataGridViewRolloverCellColumn()
        {
            this.CellTemplate = new DataGridViewRolloverCell();
        }
    }
}
