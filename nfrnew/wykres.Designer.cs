namespace nfrnew
{
    partial class wykres
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
            this.bSave = new System.Windows.Forms.Button();
            this.chbLockY = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(12, 12);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 45);
            this.bSave.TabIndex = 37;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // chbLockY
            // 
            this.chbLockY.AutoSize = true;
            this.chbLockY.Location = new System.Drawing.Point(12, 63);
            this.chbLockY.Name = "chbLockY";
            this.chbLockY.Size = new System.Drawing.Size(60, 17);
            this.chbLockY.TabIndex = 38;
            this.chbLockY.Text = "Lock Y";
            this.chbLockY.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(89, 63);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(93, 17);
            this.checkBox1.TabIndex = 39;
            this.checkBox1.Text = "Clear previous";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // wykres
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 643);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.chbLockY);
            this.Controls.Add(this.bSave);
            this.Name = "wykres";
            this.Text = "wykres";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.wykres_FormClosing);
            this.Load += new System.EventHandler(this.wykres_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.CheckBox chbLockY;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}