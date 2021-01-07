namespace DevExpressSample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spreadsheetFormulaBarPanel = new System.Windows.Forms.Panel();
            this.spreadsheetControl = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            this.splitterControl = new DevExpress.XtraEditors.SplitterControl();
            this.formulaBarNameBoxSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.spreadsheetNameBoxControl = new DevExpress.XtraSpreadsheet.SpreadsheetNameBoxControl();
            this.spreadsheetFormulaBarControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetFormulaBarControl();
            this.spreadsheetFormulaBarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl.Panel1)).BeginInit();
            this.formulaBarNameBoxSplitContainerControl.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl.Panel2)).BeginInit();
            this.formulaBarNameBoxSplitContainerControl.Panel2.SuspendLayout();
            this.formulaBarNameBoxSplitContainerControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spreadsheetNameBoxControl.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // spreadsheetFormulaBarPanel
            // 
            this.spreadsheetFormulaBarPanel.Controls.Add(this.spreadsheetControl);
            this.spreadsheetFormulaBarPanel.Controls.Add(this.splitterControl);
            this.spreadsheetFormulaBarPanel.Controls.Add(this.formulaBarNameBoxSplitContainerControl);
            this.spreadsheetFormulaBarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetFormulaBarPanel.Location = new System.Drawing.Point(0, 0);
            this.spreadsheetFormulaBarPanel.Name = "spreadsheetFormulaBarPanel";
            this.spreadsheetFormulaBarPanel.Size = new System.Drawing.Size(1092, 746);
            this.spreadsheetFormulaBarPanel.TabIndex = 3;
            // 
            // spreadsheetControl
            // 
            this.spreadsheetControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetControl.Location = new System.Drawing.Point(0, 33);
            this.spreadsheetControl.Name = "spreadsheetControl";
            this.spreadsheetControl.Size = new System.Drawing.Size(1092, 713);
            this.spreadsheetControl.TabIndex = 1;
            this.spreadsheetControl.Text = "spreadsheetControl1";
            // 
            // splitterControl
            // 
            this.splitterControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterControl.Location = new System.Drawing.Point(0, 28);
            this.splitterControl.MinSize = 20;
            this.splitterControl.Name = "splitterControl";
            this.splitterControl.Size = new System.Drawing.Size(1092, 5);
            this.splitterControl.TabIndex = 2;
            this.splitterControl.TabStop = false;
            // 
            // formulaBarNameBoxSplitContainerControl
            // 
            this.formulaBarNameBoxSplitContainerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.formulaBarNameBoxSplitContainerControl.Location = new System.Drawing.Point(0, 0);
            this.formulaBarNameBoxSplitContainerControl.MinimumSize = new System.Drawing.Size(0, 28);
            this.formulaBarNameBoxSplitContainerControl.Name = "formulaBarNameBoxSplitContainerControl";
            // 
            // formulaBarNameBoxSplitContainerControl.Panel1
            // 
            this.formulaBarNameBoxSplitContainerControl.Panel1.Controls.Add(this.spreadsheetNameBoxControl);
            // 
            // formulaBarNameBoxSplitContainerControl.Panel2
            // 
            this.formulaBarNameBoxSplitContainerControl.Panel2.Controls.Add(this.spreadsheetFormulaBarControl1);
            this.formulaBarNameBoxSplitContainerControl.Size = new System.Drawing.Size(1092, 28);
            this.formulaBarNameBoxSplitContainerControl.SplitterPosition = 145;
            this.formulaBarNameBoxSplitContainerControl.TabIndex = 3;
            // 
            // spreadsheetNameBoxControl
            // 
            this.spreadsheetNameBoxControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.spreadsheetNameBoxControl.EditValue = "A1";
            this.spreadsheetNameBoxControl.Location = new System.Drawing.Point(0, 0);
            this.spreadsheetNameBoxControl.Name = "spreadsheetNameBoxControl";
            this.spreadsheetNameBoxControl.Properties.AutoHeight = false;
            this.spreadsheetNameBoxControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.spreadsheetNameBoxControl.Size = new System.Drawing.Size(145, 28);
            this.spreadsheetNameBoxControl.SpreadsheetControl = this.spreadsheetControl;
            this.spreadsheetNameBoxControl.TabIndex = 0;
            // 
            // spreadsheetFormulaBarControl1
            // 
            this.spreadsheetFormulaBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetFormulaBarControl1.Location = new System.Drawing.Point(0, 0);
            this.spreadsheetFormulaBarControl1.MinimumSize = new System.Drawing.Size(0, 21);
            this.spreadsheetFormulaBarControl1.Name = "spreadsheetFormulaBarControl1";
            this.spreadsheetFormulaBarControl1.Size = new System.Drawing.Size(942, 28);
            this.spreadsheetFormulaBarControl1.SpreadsheetControl = this.spreadsheetControl;
            this.spreadsheetFormulaBarControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.False;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 746);
            this.Controls.Add(this.spreadsheetFormulaBarPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form1";
            this.Text = "Form1";
            this.spreadsheetFormulaBarPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl.Panel1)).EndInit();
            this.formulaBarNameBoxSplitContainerControl.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl.Panel2)).EndInit();
            this.formulaBarNameBoxSplitContainerControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.formulaBarNameBoxSplitContainerControl)).EndInit();
            this.formulaBarNameBoxSplitContainerControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spreadsheetNameBoxControl.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel spreadsheetFormulaBarPanel;
        private DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl;
        private DevExpress.XtraEditors.SplitterControl splitterControl;
        private DevExpress.XtraEditors.SplitContainerControl formulaBarNameBoxSplitContainerControl;
        private DevExpress.XtraSpreadsheet.SpreadsheetNameBoxControl spreadsheetNameBoxControl;
        private DevExpress.XtraSpreadsheet.SpreadsheetFormulaBarControl spreadsheetFormulaBarControl1;

    }
}
