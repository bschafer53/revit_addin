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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParameterSelector));
            this.filterBtn = new System.Windows.Forms.Button();
            this.checksParam = new System.Windows.Forms.CheckedListBox();
            this.filterBox = new System.Windows.Forms.GroupBox();
            this.andRadio = new System.Windows.Forms.RadioButton();
            this.orRadio = new System.Windows.Forms.RadioButton();
            this.filterBox.SuspendLayout();
            this.SuspendLayout();
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
            // checksParam
            // 
            this.checksParam.CheckOnClick = true;
            this.checksParam.FormattingEnabled = true;
            this.checksParam.Location = new System.Drawing.Point(12, 95);
            this.checksParam.Name = "checksParam";
            this.checksParam.Size = new System.Drawing.Size(334, 289);
            this.checksParam.TabIndex = 2;
            // 
            // filterBox
            // 
            this.filterBox.Controls.Add(this.orRadio);
            this.filterBox.Controls.Add(this.andRadio);
            this.filterBox.Location = new System.Drawing.Point(261, 12);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(76, 70);
            this.filterBox.TabIndex = 3;
            this.filterBox.TabStop = false;
            this.filterBox.Text = "Filter Type";
            // 
            // andRadio
            // 
            this.andRadio.AutoSize = true;
            this.andRadio.Checked = true;
            this.andRadio.Location = new System.Drawing.Point(7, 20);
            this.andRadio.Name = "andRadio";
            this.andRadio.Size = new System.Drawing.Size(48, 17);
            this.andRadio.TabIndex = 0;
            this.andRadio.TabStop = true;
            this.andRadio.Text = "AND";
            this.andRadio.UseVisualStyleBackColor = true;
            // 
            // orRadio
            // 
            this.orRadio.AutoSize = true;
            this.orRadio.Location = new System.Drawing.Point(7, 44);
            this.orRadio.Name = "orRadio";
            this.orRadio.Size = new System.Drawing.Size(41, 17);
            this.orRadio.TabIndex = 1;
            this.orRadio.Text = "OR";
            this.orRadio.UseVisualStyleBackColor = true;
            // 
            // ParameterSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 397);
            this.Controls.Add(this.filterBox);
            this.Controls.Add(this.checksParam);
            this.Controls.Add(this.filterBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ParameterSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ParameterSelector";
            this.filterBox.ResumeLayout(false);
            this.filterBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button filterBtn;
        private System.Windows.Forms.CheckedListBox checksParam;
        private System.Windows.Forms.GroupBox filterBox;
        private System.Windows.Forms.RadioButton orRadio;
        private System.Windows.Forms.RadioButton andRadio;
    }
}