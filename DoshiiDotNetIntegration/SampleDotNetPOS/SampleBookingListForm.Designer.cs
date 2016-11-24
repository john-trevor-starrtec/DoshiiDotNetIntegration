namespace SampleDotNetPOS
{
    partial class SampleBookingListForm
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
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.btnArrive = new System.Windows.Forms.ToolStripButton();
            this.statusbar = new System.Windows.Forms.StatusStrip();
            this.lblIconLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.dgvBookings = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TableNames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Covers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConsumerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CheckinId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolbar.SuspendLayout();
            this.statusbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbar
            // 
            this.toolbar.AutoSize = false;
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnArrive});
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(739, 43);
            this.toolbar.TabIndex = 3;
            this.toolbar.Text = "toolbar";
            // 
            // btnArrive
            // 
            this.btnArrive.AutoSize = false;
            this.btnArrive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnArrive.Image = global::SampleDotNetPOS.Properties.Resources.add;
            this.btnArrive.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnArrive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnArrive.Name = "btnArrive";
            this.btnArrive.Size = new System.Drawing.Size(40, 40);
            this.btnArrive.Text = "Arrive Booking";
            this.btnArrive.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnArrive.Click += new System.EventHandler(this.btnArrive_Click);
            // 
            // statusbar
            // 
            this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblIconLink});
            this.statusbar.Location = new System.Drawing.Point(0, 599);
            this.statusbar.Name = "statusbar";
            this.statusbar.Size = new System.Drawing.Size(739, 22);
            this.statusbar.TabIndex = 4;
            this.statusbar.Text = "statusStrip1";
            // 
            // lblIconLink
            // 
            this.lblIconLink.IsLink = true;
            this.lblIconLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblIconLink.Name = "lblIconLink";
            this.lblIconLink.Size = new System.Drawing.Size(133, 17);
            this.lblIconLink.Text = "Icons by Lokas Software";
            // 
            // dgvBookings
            // 
            this.dgvBookings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBookings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.TableNames,
            this.Date,
            this.Covers,
            this.ConsumerName,
            this.Phone,
            this.CheckinId});
            this.dgvBookings.Location = new System.Drawing.Point(0, 46);
            this.dgvBookings.Name = "dgvBookings";
            this.dgvBookings.Size = new System.Drawing.Size(739, 350);
            this.dgvBookings.TabIndex = 5;
            this.dgvBookings.SelectionChanged += new System.EventHandler(this.dgvBookings_SelectionChanged);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // TableNames
            // 
            this.TableNames.HeaderText = "Table Names";
            this.TableNames.Name = "TableNames";
            this.TableNames.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Covers
            // 
            this.Covers.HeaderText = "# Covers";
            this.Covers.Name = "Covers";
            this.Covers.ReadOnly = true;
            // 
            // Name
            // 
            this.ConsumerName.HeaderText = "Name";
            this.ConsumerName.Name = "Name";
            this.ConsumerName.ReadOnly = true;
            // 
            // Phone
            // 
            this.Phone.HeaderText = "Phone";
            this.Phone.Name = "Phone";
            this.Phone.ReadOnly = true;
            // 
            // CheckinId
            // 
            this.CheckinId.HeaderText = "Checkin Id";
            this.CheckinId.Name = "CheckinId";
            this.CheckinId.ReadOnly = true;
            // 
            // SampleBookingListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 621);
            this.Controls.Add(this.dgvBookings);
            this.Controls.Add(this.statusbar);
            this.Controls.Add(this.toolbar);
            this.Name = "SampleBookingListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Current Bookings";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.statusbar.ResumeLayout(false);
            this.statusbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripButton btnArrive;
        private System.Windows.Forms.StatusStrip statusbar;
        private System.Windows.Forms.ToolStripStatusLabel lblIconLink;
        private System.Windows.Forms.DataGridView dgvBookings;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConsumerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableNames;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Covers;
        private System.Windows.Forms.DataGridViewTextBoxColumn Phone;
        private System.Windows.Forms.DataGridViewTextBoxColumn CheckinId;
    }
}