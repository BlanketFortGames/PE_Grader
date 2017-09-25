namespace PE_Grader
{
    partial class CodeWindow
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
            this.scintilla = new ScintillaNET.Scintilla();
            this.SuspendLayout();
            // 
            // scintilla
            // 
            this.scintilla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scintilla.Location = new System.Drawing.Point(12, 12);
            this.scintilla.Name = "scintilla";
            this.scintilla.Size = new System.Drawing.Size(1124, 1154);
            this.scintilla.TabIndex = 0;
            this.scintilla.Text = "scintilla1";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 1178);
            this.Controls.Add(this.scintilla);
            this.Name = "CodeWindow";
            this.Text = "Form3";
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla scintilla;
    }
}