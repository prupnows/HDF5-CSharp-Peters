namespace HDF5CSharp.Winforms.Tests
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCreateData = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbPopup = new System.Windows.Forms.CheckBox();
            this.ceCompare = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCreateData
            // 
            this.btnCreateData.Location = new System.Drawing.Point(12, 12);
            this.btnCreateData.Name = "btnCreateData";
            this.btnCreateData.Size = new System.Drawing.Size(229, 33);
            this.btnCreateData.TabIndex = 0;
            this.btnCreateData.Text = "Create Data";
            this.btnCreateData.UseVisualStyleBackColor = true;
            this.btnCreateData.Click += new System.EventHandler(this.btnCreateData_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(12, 51);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(229, 33);
            this.btnCompare.TabIndex = 1;
            this.btnCompare.Text = "Read And Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbPopup
            // 
            this.cbPopup.AutoSize = true;
            this.cbPopup.Location = new System.Drawing.Point(282, 12);
            this.cbPopup.Name = "cbPopup";
            this.cbPopup.Size = new System.Drawing.Size(75, 24);
            this.cbPopup.TabIndex = 2;
            this.cbPopup.Text = "popup";
            this.cbPopup.UseVisualStyleBackColor = true;
            // 
            // ceCompare
            // 
            this.ceCompare.AutoSize = true;
            this.ceCompare.Location = new System.Drawing.Point(282, 42);
            this.ceCompare.Name = "ceCompare";
            this.ceCompare.Size = new System.Drawing.Size(142, 24);
            this.ceCompare.TabIndex = 3;
            this.ceCompare.Text = "Compare Results";
            this.ceCompare.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 450);
            this.Controls.Add(this.ceCompare);
            this.Controls.Add(this.cbPopup);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnCreateData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateData;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbPopup;
        private System.Windows.Forms.CheckBox ceCompare;
    }
}
