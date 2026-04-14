namespace JpgResizer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            btnSelectFile = new Button();
            txtFilePath = new TextBox();
            picPreview = new PictureBox();
            lblOriginalSize = new Label();
            lblTargetWidth = new Label();
            nudTargetWidth = new NumericUpDown();
            btnUpload = new Button();
            statusStrip = new StatusStrip();
            btnHistory = new Button();
            statusLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTargetWidth).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectFile
            // 
            btnSelectFile.BackColor = Color.MediumPurple;
            btnSelectFile.Location = new Point(14, 24);
            btnSelectFile.Margin = new Padding(3, 4, 3, 4);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(117, 27);
            btnSelectFile.TabIndex = 0;
            btnSelectFile.Text = "Выбрать файл...";
            btnSelectFile.UseVisualStyleBackColor = false;
            // 
            // txtFilePath
            // 
            txtFilePath.BackColor = Color.Lavender;
            txtFilePath.Location = new Point(137, 24);
            txtFilePath.Margin = new Padding(3, 4, 3, 4);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(343, 27);
            txtFilePath.TabIndex = 1;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.Lavender;
            picPreview.BorderStyle = BorderStyle.FixedSingle;
            picPreview.Location = new Point(14, 67);
            picPreview.Margin = new Padding(3, 4, 3, 4);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(228, 266);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 2;
            picPreview.TabStop = false;
            // 
            // lblOriginalSize
            // 
            lblOriginalSize.AutoSize = true;
            lblOriginalSize.Location = new Point(251, 67);
            lblOriginalSize.Name = "lblOriginalSize";
            lblOriginalSize.Size = new Size(78, 20);
            lblOriginalSize.TabIndex = 3;
            lblOriginalSize.Text = "Размеры: ";
            // 
            // lblTargetWidth
            // 
            lblTargetWidth.AutoSize = true;
            lblTargetWidth.Location = new Point(14, 347);
            lblTargetWidth.Name = "lblTargetWidth";
            lblTargetWidth.Size = new Size(220, 20);
            lblTargetWidth.TabIndex = 4;
            lblTargetWidth.Text = "Целевая ширина (в пикселях):";
            // 
            // nudTargetWidth
            // 
            nudTargetWidth.BackColor = Color.Lavender;
            nudTargetWidth.ForeColor = SystemColors.MenuText;
            nudTargetWidth.Location = new Point(238, 345);
            nudTargetWidth.Margin = new Padding(3, 4, 3, 4);
            nudTargetWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudTargetWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudTargetWidth.Name = "nudTargetWidth";
            nudTargetWidth.Size = new Size(91, 27);
            nudTargetWidth.TabIndex = 5;
            nudTargetWidth.Value = new decimal(new int[] { 800, 0, 0, 0 });
            // 
            // btnUpload
            // 
            btnUpload.BackColor = Color.MediumPurple;
            btnUpload.Location = new Point(335, 344);
            btnUpload.Margin = new Padding(3, 4, 3, 4);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(145, 27);
            btnUpload.TabIndex = 6;
            btnUpload.Text = "Загрузить";
            btnUpload.UseVisualStyleBackColor = false;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 438);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(568, 26);
            statusStrip.TabIndex = 7;
            statusStrip.Text = "statusStrip";
            // 
            // btnHistory
            // 
            btnHistory.BackColor = Color.Pink;
            btnHistory.Location = new Point(14, 380);
            btnHistory.Name = "btnHistory";
            btnHistory.Size = new Size(100, 30);
            btnHistory.TabIndex = 8;
            btnHistory.Text = "История";
            btnHistory.UseVisualStyleBackColor = false;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(57, 20);
            statusLabel.Text = "Готово";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.MenuBar;
            ClientSize = new Size(568, 464);
            Controls.Add(btnHistory);
            Controls.Add(statusStrip);
            Controls.Add(btnUpload);
            Controls.Add(nudTargetWidth);
            Controls.Add(lblTargetWidth);
            Controls.Add(lblOriginalSize);
            Controls.Add(picPreview);
            Controls.Add(txtFilePath);
            Controls.Add(btnSelectFile);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "JPEG Resizer Client";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTargetWidth).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Label lblOriginalSize;
        private System.Windows.Forms.Label lblTargetWidth;
        private System.Windows.Forms.NumericUpDown nudTargetWidth;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button btnHistory;
        private ToolStripStatusLabel statusLabel;
    }
}