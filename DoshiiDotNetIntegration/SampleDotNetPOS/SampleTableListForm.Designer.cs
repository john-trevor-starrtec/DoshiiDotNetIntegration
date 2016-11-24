namespace SampleDotNetPOS
{
    partial class SampleTableListForm
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
            this.btnAddTable = new System.Windows.Forms.ToolStripButton();
            this.btnEditTable = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteTable = new System.Windows.Forms.ToolStripButton();
            this.dgvBookings = new System.Windows.Forms.DataGridView();
            this.statusbar = new System.Windows.Forms.StatusStrip();
            this.lblIconLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.TableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxCovers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsCommunal = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CanMerge = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsSmoking = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsOutdoor = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).BeginInit();
            this.statusbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbar
            // 
            this.toolbar.AutoSize = false;
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddTable,
            this.btnEditTable,
            this.btnDeleteTable});
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(734, 43);
            this.toolbar.TabIndex = 4;
            this.toolbar.Text = "toolbar";
            // 
            // btnAddTable
            // 
            this.btnAddTable.AutoSize = false;
            this.btnAddTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddTable.Image = global::SampleDotNetPOS.Properties.Resources.add;
            this.btnAddTable.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddTable.Name = "btnAddTable";
            this.btnAddTable.Size = new System.Drawing.Size(40, 40);
            this.btnAddTable.Text = "Add Table";
            this.btnAddTable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAddTable.Click += new System.EventHandler(this.btnAddOrder_Click);
            // 
            // btnEditTable
            // 
            this.btnEditTable.AutoSize = false;
            this.btnEditTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditTable.Image = global::SampleDotNetPOS.Properties.Resources.edit;
            this.btnEditTable.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEditTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditTable.Name = "btnEditTable";
            this.btnEditTable.Size = new System.Drawing.Size(40, 40);
            this.btnEditTable.Text = "Edit Table";
            this.btnEditTable.Click += new System.EventHandler(this.btnEditTable_Click);
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.AutoSize = false;
            this.btnDeleteTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteTable.Image = global::SampleDotNetPOS.Properties.Resources.delete;
            this.btnDeleteTable.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnDeleteTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(40, 40);
            this.btnDeleteTable.Text = "Remove Table";
            // 
            // dgvBookings
            // 
            this.dgvBookings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBookings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TableName,
            this.MaxCovers,
            this.IsActive,
            this.IsCommunal,
            this.CanMerge,
            this.IsSmoking,
            this.IsOutdoor});
            this.dgvBookings.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvBookings.Location = new System.Drawing.Point(0, 43);
            this.dgvBookings.Name = "dgvBookings";
            this.dgvBookings.Size = new System.Drawing.Size(734, 248);
            this.dgvBookings.TabIndex = 6;
            this.dgvBookings.SelectionChanged += new System.EventHandler(this.dgvBookings_SelectionChanged);
            // 
            // statusbar
            // 
            this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblIconLink});
            this.statusbar.Location = new System.Drawing.Point(0, 528);
            this.statusbar.Name = "statusbar";
            this.statusbar.Size = new System.Drawing.Size(734, 22);
            this.statusbar.TabIndex = 7;
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
            // TableName
            // 
            this.TableName.HeaderText = "Name";
            this.TableName.Name = "TableName";
            this.TableName.ReadOnly = true;
            // 
            // MaxCovers
            // 
            this.MaxCovers.HeaderText = "Max Covers";
            this.MaxCovers.Name = "MaxCovers";
            this.MaxCovers.ReadOnly = true;
            // 
            // IsActive
            // 
            this.IsActive.HeaderText = "Is Active";
            this.IsActive.Name = "IsActive";
            this.IsActive.ReadOnly = true;
            // 
            // IsCommunal
            // 
            this.IsCommunal.HeaderText = "Is Communal";
            this.IsCommunal.Name = "IsCommunal";
            this.IsCommunal.ReadOnly = true;
            // 
            // CanMerge
            // 
            this.CanMerge.HeaderText = "Can Merge";
            this.CanMerge.Name = "CanMerge";
            this.CanMerge.ReadOnly = true;
            // 
            // IsSmoking
            // 
            this.IsSmoking.HeaderText = "Is Smoking";
            this.IsSmoking.Name = "IsSmoking";
            this.IsSmoking.ReadOnly = true;
            // 
            // IsOutdoor
            // 
            this.IsOutdoor.HeaderText = "Is Outdoor";
            this.IsOutdoor.Name = "IsOutdoor";
            this.IsOutdoor.ReadOnly = true;
            // 
            // SampleTableListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 550);
            this.Controls.Add(this.statusbar);
            this.Controls.Add(this.dgvBookings);
            this.Controls.Add(this.toolbar);
            this.Name = "SampleTableListForm";
            this.Text = "Current Tables";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).EndInit();
            this.statusbar.ResumeLayout(false);
            this.statusbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripButton btnAddTable;
        private System.Windows.Forms.ToolStripButton btnEditTable;
        private System.Windows.Forms.ToolStripButton btnDeleteTable;
        private System.Windows.Forms.DataGridView dgvBookings;
        private System.Windows.Forms.StatusStrip statusbar;
        private System.Windows.Forms.ToolStripStatusLabel lblIconLink;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxCovers;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsActive;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsCommunal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CanMerge;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsSmoking;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsOutdoor;
    }
}