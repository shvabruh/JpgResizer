namespace JpgResizer
{
    partial class HistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Button btnRefresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            this.dgvHistory = new DataGridView();
            this.picPreview = new PictureBox();
            this.btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.ForeColor = Color.Black;
            this.dgvHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvHistory.BackgroundColor = Color.LavenderBlush;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.LightPink;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = Color.LightPink;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            this.dgvHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHistory.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = Color.LavenderBlush;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.Desktop;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            this.dgvHistory.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvHistory.Dock = DockStyle.Top;
            this.dgvHistory.EnableHeadersVisualStyles = false;
            this.dgvHistory.Location = new Point(0, 0);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new Size(884, 300);
            this.dgvHistory.TabIndex = 0;
            // 
            // picPreview
            // 
            this.picPreview.BackColor = SystemColors.HighlightText;
            this.picPreview.BorderStyle = BorderStyle.FixedSingle;
            this.picPreview.Dock = DockStyle.Fill;
            this.picPreview.Location = new Point(0, 300);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new Size(884, 262);
            this.picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 1;
            this.picPreview.TabStop = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = Color.LightPink;
            this.btnRefresh.Dock = DockStyle.Bottom;
            this.btnRefresh.Location = new Point(0, 562);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(884, 35);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(884, 597);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.btnRefresh);
            this.Name = "HistoryForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "История загрузок";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);
        }
    }
}