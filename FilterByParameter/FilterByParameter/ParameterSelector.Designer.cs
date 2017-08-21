namespace FilterByParameter
{
    partial class ParameterSelector
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
            this.cbParams = new System.Windows.Forms.ComboBox();
            this.filterBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbParams
            // 
            this.cbParams.FormattingEnabled = true;
            this.cbParams.Location = new System.Drawing.Point(12, 12);
            this.cbParams.Name = "cbParams";
            this.cbParams.Size = new System.Drawing.Size(334, 21);
            this.cbParams.TabIndex = 0;
            // 
            // filterBtn
            // 
            this.filterBtn.Location = new System.Drawing.Point(12, 50);
            this.filterBtn.Name = "filterBtn";
            this.filterBtn.Size = new System.Drawing.Size(75, 23);
            this.filterBtn.TabIndex = 1;
            this.filterBtn.Text = "Filter";
            this.filterBtn.UseVisualStyleBackColor = true;
            this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
            // 
            // ParameterSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 86);
            this.Controls.Add(this.filterBtn);
            this.Controls.Add(this.cbParams);
            this.Name = "ParameterSelector";
            this.Text = "ParameterSelector";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbParams;
        private System.Windows.Forms.Button filterBtn;
    }
}