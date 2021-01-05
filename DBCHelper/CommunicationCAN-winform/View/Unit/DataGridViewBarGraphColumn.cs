using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommunicationCAN_winform.View.Unit
{
    public class DataGridViewBarGraphColumn : DataGridViewTextBoxCell
    {
        #region Public Properties

        public long MaxValue
        {
            get;
            set;
        }

        #endregion

        #region Field

        private bool needRecalc = true;

        #endregion

        #region Constructor

        public DataGridViewBarGraphColumn()
        {

        }
        #endregion

        #region Public Methods

        public void CalcMaxValue()
        {

        }


        #endregion

        #region Overrides

        protected override void Paint( 
            Graphics graphics, 
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value, object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle
            advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText,cellStyle, advancedBorderStyle, paintParts);
        }

        #endregion

    }
}
