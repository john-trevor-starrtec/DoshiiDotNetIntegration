namespace SampleDotNetPOS
{
    partial class SampleTableForm
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxMaxCovers = new System.Windows.Forms.TextBox();
            this.cbxIsActive = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxIsCommunal = new System.Windows.Forms.CheckBox();
            this.cbxCanMerge = new System.Windows.Forms.CheckBox();
            this.cbxIsSmoking = new System.Windows.Forms.CheckBox();
            this.cbxIsOutdoor = new System.Windows.Forms.CheckBox();
            this.lblUri = new System.Windows.Forms.LinkLabel();
            this.lblUriCaption = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(378, 226);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(480, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // tbxName
            // 
            this.tbxName.Location = new System.Drawing.Point(38, 30);
            this.tbxName.Name = "tbxName";
            this.tbxName.Size = new System.Drawing.Size(100, 20);
            this.tbxName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max Covers";
            // 
            // tbxMaxCovers
            // 
            this.tbxMaxCovers.Location = new System.Drawing.Point(41, 95);
            this.tbxMaxCovers.Name = "tbxMaxCovers";
            this.tbxMaxCovers.Size = new System.Drawing.Size(100, 20);
            this.tbxMaxCovers.TabIndex = 1;
            // 
            // cbxIsActive
            // 
            this.cbxIsActive.AutoSize = true;
            this.cbxIsActive.Location = new System.Drawing.Point(41, 137);
            this.cbxIsActive.Name = "cbxIsActive";
            this.cbxIsActive.Size = new System.Drawing.Size(67, 17);
            this.cbxIsActive.TabIndex = 2;
            this.cbxIsActive.Text = "Is Active";
            this.cbxIsActive.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxIsOutdoor);
            this.groupBox1.Controls.Add(this.cbxIsSmoking);
            this.groupBox1.Controls.Add(this.cbxCanMerge);
            this.groupBox1.Controls.Add(this.cbxIsCommunal);
            this.groupBox1.Location = new System.Drawing.Point(220, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 146);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Criteria";
            // 
            // cbxIsCommunal
            // 
            this.cbxIsCommunal.AutoSize = true;
            this.cbxIsCommunal.Location = new System.Drawing.Point(7, 20);
            this.cbxIsCommunal.Name = "cbxIsCommunal";
            this.cbxIsCommunal.Size = new System.Drawing.Size(86, 17);
            this.cbxIsCommunal.TabIndex = 0;
            this.cbxIsCommunal.Text = "Is Communal";
            this.cbxIsCommunal.UseVisualStyleBackColor = true;
            // 
            // cbxCanMerge
            // 
            this.cbxCanMerge.AutoSize = true;
            this.cbxCanMerge.Location = new System.Drawing.Point(7, 44);
            this.cbxCanMerge.Name = "cbxCanMerge";
            this.cbxCanMerge.Size = new System.Drawing.Size(78, 17);
            this.cbxCanMerge.TabIndex = 1;
            this.cbxCanMerge.Text = "Can Merge";
            this.cbxCanMerge.UseVisualStyleBackColor = true;
            // 
            // cbxIsSmoking
            // 
            this.cbxIsSmoking.AutoSize = true;
            this.cbxIsSmoking.Location = new System.Drawing.Point(7, 65);
            this.cbxIsSmoking.Name = "cbxIsSmoking";
            this.cbxIsSmoking.Size = new System.Drawing.Size(78, 17);
            this.cbxIsSmoking.TabIndex = 2;
            this.cbxIsSmoking.Text = "Is Smoking";
            this.cbxIsSmoking.UseVisualStyleBackColor = true;
            // 
            // cbxIsOutdoor
            // 
            this.cbxIsOutdoor.AutoSize = true;
            this.cbxIsOutdoor.Location = new System.Drawing.Point(7, 89);
            this.cbxIsOutdoor.Name = "cbxIsOutdoor";
            this.cbxIsOutdoor.Size = new System.Drawing.Size(75, 17);
            this.cbxIsOutdoor.TabIndex = 3;
            this.cbxIsOutdoor.Text = "Is Outdoor";
            this.cbxIsOutdoor.UseVisualStyleBackColor = true;
            // 
            // lblUri
            // 
            this.lblUri.AutoSize = true;
            this.lblUri.Location = new System.Drawing.Point(138, 204);
            this.lblUri.Name = "lblUri";
            this.lblUri.Size = new System.Drawing.Size(199, 13);
            this.lblUri.TabIndex = 55;
            this.lblUri.TabStop = true;
            this.lblUri.Text = "https://alphasandbox.doshii.co/orders/1";
            // 
            // lblUriCaption
            // 
            this.lblUriCaption.AutoSize = true;
            this.lblUriCaption.Location = new System.Drawing.Point(41, 205);
            this.lblUriCaption.Name = "lblUriCaption";
            this.lblUriCaption.Size = new System.Drawing.Size(29, 13);
            this.lblUriCaption.TabIndex = 54;
            this.lblUriCaption.Text = "URI:";
            // 
            // SampleTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 261);
            this.Controls.Add(this.lblUri);
            this.Controls.Add(this.lblUriCaption);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbxIsActive);
            this.Controls.Add(this.tbxMaxCovers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "SampleTableForm";
            this.Text = "Add/Edit Table";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxMaxCovers;
        private System.Windows.Forms.CheckBox cbxIsActive;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbxIsOutdoor;
        private System.Windows.Forms.CheckBox cbxIsSmoking;
        private System.Windows.Forms.CheckBox cbxCanMerge;
        private System.Windows.Forms.CheckBox cbxIsCommunal;
        private System.Windows.Forms.LinkLabel lblUri;
        private System.Windows.Forms.Label lblUriCaption;
    }
}