namespace SampleDotNetPOS
{
    partial class SampleArriveForm
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
            this.tbxPosRef = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxTableNames = new System.Windows.Forms.TextBox();
            this.tbxCovers = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonArrive = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tbxId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "POS Reference";
            // 
            // tbxPosRef
            // 
            this.tbxPosRef.Location = new System.Drawing.Point(26, 56);
            this.tbxPosRef.Name = "tbxPosRef";
            this.tbxPosRef.Size = new System.Drawing.Size(100, 20);
            this.tbxPosRef.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Table Names";
            // 
            // tbxTableNames
            // 
            this.tbxTableNames.Location = new System.Drawing.Point(26, 178);
            this.tbxTableNames.Name = "tbxTableNames";
            this.tbxTableNames.ReadOnly = true;
            this.tbxTableNames.Size = new System.Drawing.Size(217, 20);
            this.tbxTableNames.TabIndex = 3;
            // 
            // tbxCovers
            // 
            this.tbxCovers.Location = new System.Drawing.Point(26, 228);
            this.tbxCovers.Name = "tbxCovers";
            this.tbxCovers.ReadOnly = true;
            this.tbxCovers.Size = new System.Drawing.Size(47, 20);
            this.tbxCovers.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Covers";
            // 
            // buttonArrive
            // 
            this.buttonArrive.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonArrive.Location = new System.Drawing.Point(32, 269);
            this.buttonArrive.Name = "buttonArrive";
            this.buttonArrive.Size = new System.Drawing.Size(75, 23);
            this.buttonArrive.TabIndex = 6;
            this.buttonArrive.Text = "Arrive";
            this.buttonArrive.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(131, 269);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tbxId
            // 
            this.tbxId.Location = new System.Drawing.Point(26, 123);
            this.tbxId.Name = "tbxId";
            this.tbxId.ReadOnly = true;
            this.tbxId.Size = new System.Drawing.Size(47, 20);
            this.tbxId.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Id";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(131, 228);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 10;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // SampleArriveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 323);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.tbxId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonArrive);
            this.Controls.Add(this.tbxCovers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxTableNames);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxPosRef);
            this.Controls.Add(this.label1);
            this.Name = "SampleArriveForm";
            this.Text = "Arrive Booking";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxPosRef;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxTableNames;
        private System.Windows.Forms.TextBox tbxCovers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonArrive;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox tbxId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonRefresh;
    }
}