namespace PE_Grader
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
            this.label1 = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowseButton = new System.Windows.Forms.Button();
            this.unarchiveButton = new System.Windows.Forms.Button();
            this.compileButton = new System.Windows.Forms.Button();
            this.clearStatusButton = new System.Windows.Forms.Button();
            this.clearAllButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Grading Directory: ";
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(207, 19);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(624, 30);
            this.directoryTextBox.TabIndex = 1;
            // 
            // folderBrowseButton
            // 
            this.folderBrowseButton.Location = new System.Drawing.Point(873, 15);
            this.folderBrowseButton.Name = "folderBrowseButton";
            this.folderBrowseButton.Size = new System.Drawing.Size(87, 38);
            this.folderBrowseButton.TabIndex = 2;
            this.folderBrowseButton.Text = "Browse";
            this.folderBrowseButton.UseVisualStyleBackColor = true;
            this.folderBrowseButton.Click += new System.EventHandler(this.folderBrowseButton_Click);
            // 
            // unarchiveButton
            // 
            this.unarchiveButton.Location = new System.Drawing.Point(157, 91);
            this.unarchiveButton.Name = "unarchiveButton";
            this.unarchiveButton.Size = new System.Drawing.Size(142, 51);
            this.unarchiveButton.TabIndex = 3;
            this.unarchiveButton.Text = "Unarchive";
            this.unarchiveButton.UseVisualStyleBackColor = true;
            this.unarchiveButton.Click += new System.EventHandler(this.unarchiveButton_Click);
            // 
            // compileButton
            // 
            this.compileButton.Location = new System.Drawing.Point(341, 91);
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new System.Drawing.Size(147, 51);
            this.compileButton.TabIndex = 4;
            this.compileButton.Text = "Compile";
            this.compileButton.UseVisualStyleBackColor = true;
            this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
            // 
            // clearStatusButton
            // 
            this.clearStatusButton.Location = new System.Drawing.Point(528, 91);
            this.clearStatusButton.Name = "clearStatusButton";
            this.clearStatusButton.Size = new System.Drawing.Size(147, 51);
            this.clearStatusButton.TabIndex = 5;
            this.clearStatusButton.Text = "Clear Status";
            this.clearStatusButton.UseVisualStyleBackColor = true;
            this.clearStatusButton.Click += new System.EventHandler(this.clearStatusButton_Click);
            // 
            // clearAllButton
            // 
            this.clearAllButton.Location = new System.Drawing.Point(715, 91);
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.Size = new System.Drawing.Size(147, 51);
            this.clearAllButton.TabIndex = 6;
            this.clearAllButton.Text = "Clear All";
            this.clearAllButton.UseVisualStyleBackColor = true;
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Status:";
            // 
            // statusTextBox
            // 
            this.statusTextBox.Location = new System.Drawing.Point(107, 181);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.statusTextBox.Size = new System.Drawing.Size(1158, 333);
            this.statusTextBox.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 553);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clearAllButton);
            this.Controls.Add(this.clearStatusButton);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.unarchiveButton);
            this.Controls.Add(this.folderBrowseButton);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "PE Grader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button folderBrowseButton;
        private System.Windows.Forms.Button unarchiveButton;
        private System.Windows.Forms.Button compileButton;
        private System.Windows.Forms.Button clearStatusButton;
        private System.Windows.Forms.Button clearAllButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox statusTextBox;
    }
}

