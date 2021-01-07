using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommunicationCAN_winform.View.Unit
{
    public class DataGridViewWatermarkColumn : DataGridViewTextBoxColumn

    {
        public DataGridViewWatermarkColumn()
        {
            CellTemplate = new DataGridViewWatermarkCell();
        }
        public String WatermarkText
        {
            get
            {
                if (((DataGridViewWatermarkCell)CellTemplate) == null)
                {
                    throw new InvalidOperationException("cell template required");
                }
                return ((DataGridViewWatermarkCell)CellTemplate).WatermarkText;
            }
            set

            {

                if (this.WatermarkText != value)
                {
                    ((DataGridViewWatermarkCell)CellTemplate).WatermarkText = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows =
                            this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow =
                                dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewWatermarkCell cell =
                                dataGridViewRow.Cells[this.Index]
                                as DataGridViewWatermarkCell;
                            if (cell != null)
                            {
                                cell.WatermarkText = value;
                            }
                        }
                    }
                }
            }
        }
    }
    public class DataGridViewWatermarkCell : DataGridViewTextBoxCell

    {
        private String watermarkTextValue;
        public String WatermarkText
        {
            get { return watermarkTextValue; }
            set { watermarkTextValue = value; }
        }
        public override object Clone()
        {
            DataGridViewWatermarkCell cell = (DataGridViewWatermarkCell)base.Clone();
            cell.WatermarkText = this.WatermarkText;
            return cell;
        }
        protected override void Paint(Graphics graphics, Rectangle clipBounds,
            Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if ((OwningColumn.Site == null || !OwningColumn.Site.DesignMode) &&
                (RowIndex < 0 || !IsInEditMode) && !String.IsNullOrEmpty(WatermarkText) &&
                (GetValue(rowIndex) == null || GetValue(rowIndex) == DBNull.Value))
            {
                cellStyle.Font = new Font(cellStyle.Font, FontStyle.Italic);
                cellStyle.ForeColor = Color.Gray;
                formattedValue = WatermarkText;
            }
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                cellState, value, formattedValue, errorText,
                cellStyle, advancedBorderStyle, paintParts);
        }
    }
}
