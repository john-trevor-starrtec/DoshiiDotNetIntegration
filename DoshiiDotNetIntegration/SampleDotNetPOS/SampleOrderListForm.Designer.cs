namespace SampleDotNetPOS
{
	partial class SampleOrderListForm
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
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.lblIconLink = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.btnAddOrder = new System.Windows.Forms.ToolStripButton();
			this.btnEditOrder = new System.Windows.Forms.ToolStripButton();
			this.btnDeleteOrder = new System.Windows.Forms.ToolStripButton();
			this.gbxMain = new System.Windows.Forms.GroupBox();
			this.dgvOrders = new System.Windows.Forms.DataGridView();
			this.colOrderId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderDoshiiId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderInvoiceId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderUrl = new System.Windows.Forms.DataGridViewLinkColumn();
			this.colOrderItemCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderSurcountCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderSurcountValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tcOrderChildViews = new System.Windows.Forms.TabControl();
			this.tpOrderItems = new System.Windows.Forms.TabPage();
			this.tpOrderSurcounts = new System.Windows.Forms.TabPage();
			this.tpOrderPayments = new System.Windows.Forms.TabPage();
			this.dgvOrderItems = new System.Windows.Forms.DataGridView();
			this.colOrderItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemPosId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderItemImageUrl = new System.Windows.Forms.DataGridViewLinkColumn();
			this.colOrderItemStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvSurcounts = new System.Windows.Forms.DataGridView();
			this.colOrderSurcountsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colOrderSurcountsPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvOrderPayments = new System.Windows.Forms.DataGridView();
			this.colTransactionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionInvoice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionPaymentAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionAcceptLess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colTransactionPartnerInitiated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colTransactionPartner = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTransactionUrl = new System.Windows.Forms.DataGridViewLinkColumn();
			this.pnlBottom.SuspendLayout();
			this.statusbar.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.gbxMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).BeginInit();
			this.tcOrderChildViews.SuspendLayout();
			this.tpOrderItems.SuspendLayout();
			this.tpOrderSurcounts.SuspendLayout();
			this.tpOrderPayments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSurcounts)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvOrderPayments)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.tcOrderChildViews);
			this.pnlBottom.Controls.Add(this.statusbar);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 317);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(784, 245);
			this.pnlBottom.TabIndex = 1;
			// 
			// statusbar
			// 
			this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblIconLink});
			this.statusbar.Location = new System.Drawing.Point(0, 223);
			this.statusbar.Name = "statusbar";
			this.statusbar.Size = new System.Drawing.Size(784, 22);
			this.statusbar.TabIndex = 0;
			this.statusbar.Text = "statusStrip1";
			// 
			// lblIconLink
			// 
			this.lblIconLink.IsLink = true;
			this.lblIconLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this.lblIconLink.Name = "lblIconLink";
			this.lblIconLink.Size = new System.Drawing.Size(133, 17);
			this.lblIconLink.Text = "Icons by Lokas Software";
			this.lblIconLink.Click += new System.EventHandler(this.lblIconLink_Click);
			// 
			// toolbar
			// 
			this.toolbar.AutoSize = false;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddOrder,
            this.btnEditOrder,
            this.btnDeleteOrder});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(784, 43);
			this.toolbar.TabIndex = 2;
			this.toolbar.Text = "toolbar";
			// 
			// btnAddOrder
			// 
			this.btnAddOrder.AutoSize = false;
			this.btnAddOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnAddOrder.Image = global::SampleDotNetPOS.Properties.Resources.add;
			this.btnAddOrder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnAddOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnAddOrder.Name = "btnAddOrder";
			this.btnAddOrder.Size = new System.Drawing.Size(40, 40);
			this.btnAddOrder.Text = "Add Order";
			this.btnAddOrder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// btnEditOrder
			// 
			this.btnEditOrder.AutoSize = false;
			this.btnEditOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnEditOrder.Image = global::SampleDotNetPOS.Properties.Resources.edit;
			this.btnEditOrder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnEditOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnEditOrder.Name = "btnEditOrder";
			this.btnEditOrder.Size = new System.Drawing.Size(40, 40);
			this.btnEditOrder.Text = "Edit Order";
			// 
			// btnDeleteOrder
			// 
			this.btnDeleteOrder.AutoSize = false;
			this.btnDeleteOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnDeleteOrder.Image = global::SampleDotNetPOS.Properties.Resources.delete;
			this.btnDeleteOrder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnDeleteOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnDeleteOrder.Name = "btnDeleteOrder";
			this.btnDeleteOrder.Size = new System.Drawing.Size(40, 40);
			this.btnDeleteOrder.Text = "Remove Order";
			// 
			// gbxMain
			// 
			this.gbxMain.Controls.Add(this.dgvOrders);
			this.gbxMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbxMain.Location = new System.Drawing.Point(0, 43);
			this.gbxMain.Name = "gbxMain";
			this.gbxMain.Size = new System.Drawing.Size(784, 274);
			this.gbxMain.TabIndex = 0;
			this.gbxMain.TabStop = false;
			this.gbxMain.Text = "Orders";
			// 
			// dgvOrders
			// 
			this.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvOrders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOrderId,
            this.colOrderDoshiiId,
            this.colOrderStatus,
            this.colOrderInvoiceId,
            this.colOrderVersion,
            this.colOrderUrl,
            this.colOrderItemCount,
            this.colOrderItemValue,
            this.colOrderSurcountCount,
            this.colOrderSurcountValue});
			this.dgvOrders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvOrders.Location = new System.Drawing.Point(3, 16);
			this.dgvOrders.MultiSelect = false;
			this.dgvOrders.Name = "dgvOrders";
			this.dgvOrders.ReadOnly = true;
			this.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvOrders.Size = new System.Drawing.Size(778, 255);
			this.dgvOrders.TabIndex = 0;
			this.dgvOrders.SelectionChanged += new System.EventHandler(this.dgvOrders_SelectionChanged);
			// 
			// colOrderId
			// 
			this.colOrderId.Frozen = true;
			this.colOrderId.HeaderText = "Id";
			this.colOrderId.Name = "colOrderId";
			this.colOrderId.ReadOnly = true;
			// 
			// colOrderDoshiiId
			// 
			this.colOrderDoshiiId.HeaderText = "DoshiiId";
			this.colOrderDoshiiId.Name = "colOrderDoshiiId";
			this.colOrderDoshiiId.ReadOnly = true;
			// 
			// colOrderStatus
			// 
			this.colOrderStatus.HeaderText = "Status";
			this.colOrderStatus.Name = "colOrderStatus";
			this.colOrderStatus.ReadOnly = true;
			// 
			// colOrderInvoiceId
			// 
			this.colOrderInvoiceId.HeaderText = "InvoiceId";
			this.colOrderInvoiceId.Name = "colOrderInvoiceId";
			this.colOrderInvoiceId.ReadOnly = true;
			// 
			// colOrderVersion
			// 
			this.colOrderVersion.HeaderText = "Version";
			this.colOrderVersion.Name = "colOrderVersion";
			this.colOrderVersion.ReadOnly = true;
			// 
			// colOrderUrl
			// 
			this.colOrderUrl.HeaderText = "URL";
			this.colOrderUrl.Name = "colOrderUrl";
			this.colOrderUrl.ReadOnly = true;
			// 
			// colOrderItemCount
			// 
			this.colOrderItemCount.HeaderText = "# Items";
			this.colOrderItemCount.Name = "colOrderItemCount";
			this.colOrderItemCount.ReadOnly = true;
			// 
			// colOrderItemValue
			// 
			this.colOrderItemValue.HeaderText = "$ Items";
			this.colOrderItemValue.Name = "colOrderItemValue";
			this.colOrderItemValue.ReadOnly = true;
			// 
			// colOrderSurcountCount
			// 
			this.colOrderSurcountCount.HeaderText = "# Surcounts";
			this.colOrderSurcountCount.Name = "colOrderSurcountCount";
			this.colOrderSurcountCount.ReadOnly = true;
			// 
			// colOrderSurcountValue
			// 
			this.colOrderSurcountValue.HeaderText = "$ Surcounts";
			this.colOrderSurcountValue.Name = "colOrderSurcountValue";
			this.colOrderSurcountValue.ReadOnly = true;
			// 
			// tcOrderChildViews
			// 
			this.tcOrderChildViews.Controls.Add(this.tpOrderItems);
			this.tcOrderChildViews.Controls.Add(this.tpOrderSurcounts);
			this.tcOrderChildViews.Controls.Add(this.tpOrderPayments);
			this.tcOrderChildViews.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcOrderChildViews.Location = new System.Drawing.Point(0, 0);
			this.tcOrderChildViews.Name = "tcOrderChildViews";
			this.tcOrderChildViews.SelectedIndex = 0;
			this.tcOrderChildViews.Size = new System.Drawing.Size(784, 223);
			this.tcOrderChildViews.TabIndex = 1;
			// 
			// tpOrderItems
			// 
			this.tpOrderItems.Controls.Add(this.dgvOrderItems);
			this.tpOrderItems.Location = new System.Drawing.Point(4, 22);
			this.tpOrderItems.Name = "tpOrderItems";
			this.tpOrderItems.Padding = new System.Windows.Forms.Padding(3);
			this.tpOrderItems.Size = new System.Drawing.Size(776, 197);
			this.tpOrderItems.TabIndex = 0;
			this.tpOrderItems.Text = "Items";
			this.tpOrderItems.UseVisualStyleBackColor = true;
			// 
			// tpOrderSurcounts
			// 
			this.tpOrderSurcounts.Controls.Add(this.dgvSurcounts);
			this.tpOrderSurcounts.Location = new System.Drawing.Point(4, 22);
			this.tpOrderSurcounts.Name = "tpOrderSurcounts";
			this.tpOrderSurcounts.Padding = new System.Windows.Forms.Padding(3);
			this.tpOrderSurcounts.Size = new System.Drawing.Size(776, 197);
			this.tpOrderSurcounts.TabIndex = 1;
			this.tpOrderSurcounts.Text = "Surcounts";
			this.tpOrderSurcounts.UseVisualStyleBackColor = true;
			// 
			// tpOrderPayments
			// 
			this.tpOrderPayments.Controls.Add(this.dgvOrderPayments);
			this.tpOrderPayments.Location = new System.Drawing.Point(4, 22);
			this.tpOrderPayments.Name = "tpOrderPayments";
			this.tpOrderPayments.Padding = new System.Windows.Forms.Padding(3);
			this.tpOrderPayments.Size = new System.Drawing.Size(776, 197);
			this.tpOrderPayments.TabIndex = 2;
			this.tpOrderPayments.Text = "Payments";
			this.tpOrderPayments.UseVisualStyleBackColor = true;
			// 
			// dgvOrderItems
			// 
			this.dgvOrderItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvOrderItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOrderItemId,
            this.colOrderItemName,
            this.colOrderItemDescription,
            this.colOrderItemPrice,
            this.colOrderItemTags,
            this.colOrderItemVersion,
            this.colOrderItemPosId,
            this.colOrderItemImageUrl,
            this.colOrderItemStatus});
			this.dgvOrderItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvOrderItems.Location = new System.Drawing.Point(3, 3);
			this.dgvOrderItems.MultiSelect = false;
			this.dgvOrderItems.Name = "dgvOrderItems";
			this.dgvOrderItems.ReadOnly = true;
			this.dgvOrderItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvOrderItems.Size = new System.Drawing.Size(770, 191);
			this.dgvOrderItems.TabIndex = 0;
			// 
			// colOrderItemId
			// 
			this.colOrderItemId.Frozen = true;
			this.colOrderItemId.HeaderText = "Id";
			this.colOrderItemId.Name = "colOrderItemId";
			this.colOrderItemId.ReadOnly = true;
			// 
			// colOrderItemName
			// 
			this.colOrderItemName.HeaderText = "Name";
			this.colOrderItemName.Name = "colOrderItemName";
			this.colOrderItemName.ReadOnly = true;
			// 
			// colOrderItemDescription
			// 
			this.colOrderItemDescription.HeaderText = "Description";
			this.colOrderItemDescription.Name = "colOrderItemDescription";
			this.colOrderItemDescription.ReadOnly = true;
			// 
			// colOrderItemPrice
			// 
			this.colOrderItemPrice.HeaderText = "Price";
			this.colOrderItemPrice.Name = "colOrderItemPrice";
			this.colOrderItemPrice.ReadOnly = true;
			// 
			// colOrderItemTags
			// 
			this.colOrderItemTags.HeaderText = "Tags";
			this.colOrderItemTags.Name = "colOrderItemTags";
			this.colOrderItemTags.ReadOnly = true;
			// 
			// colOrderItemVersion
			// 
			this.colOrderItemVersion.HeaderText = "Version";
			this.colOrderItemVersion.Name = "colOrderItemVersion";
			this.colOrderItemVersion.ReadOnly = true;
			// 
			// colOrderItemPosId
			// 
			this.colOrderItemPosId.HeaderText = "POS Id";
			this.colOrderItemPosId.Name = "colOrderItemPosId";
			this.colOrderItemPosId.ReadOnly = true;
			// 
			// colOrderItemImageUrl
			// 
			this.colOrderItemImageUrl.HeaderText = "Image URL";
			this.colOrderItemImageUrl.Name = "colOrderItemImageUrl";
			this.colOrderItemImageUrl.ReadOnly = true;
			// 
			// colOrderItemStatus
			// 
			this.colOrderItemStatus.HeaderText = "Status";
			this.colOrderItemStatus.Name = "colOrderItemStatus";
			this.colOrderItemStatus.ReadOnly = true;
			// 
			// dgvSurcounts
			// 
			this.dgvSurcounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSurcounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOrderSurcountsName,
            this.colOrderSurcountsPrice});
			this.dgvSurcounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSurcounts.Location = new System.Drawing.Point(3, 3);
			this.dgvSurcounts.MultiSelect = false;
			this.dgvSurcounts.Name = "dgvSurcounts";
			this.dgvSurcounts.ReadOnly = true;
			this.dgvSurcounts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvSurcounts.Size = new System.Drawing.Size(770, 191);
			this.dgvSurcounts.TabIndex = 0;
			// 
			// colOrderSurcountsName
			// 
			this.colOrderSurcountsName.HeaderText = "Name";
			this.colOrderSurcountsName.Name = "colOrderSurcountsName";
			this.colOrderSurcountsName.ReadOnly = true;
			// 
			// colOrderSurcountsPrice
			// 
			this.colOrderSurcountsPrice.HeaderText = "Price";
			this.colOrderSurcountsPrice.Name = "colOrderSurcountsPrice";
			this.colOrderSurcountsPrice.ReadOnly = true;
			// 
			// dgvOrderPayments
			// 
			this.dgvOrderPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvOrderPayments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTransactionId,
            this.colTransactionReference,
            this.colTransactionInvoice,
            this.colTransactionPaymentAmount,
            this.colTransactionAcceptLess,
            this.colTransactionPartnerInitiated,
            this.colTransactionPartner,
            this.colTransactionStatus,
            this.colTransactionVersion,
            this.colTransactionUrl});
			this.dgvOrderPayments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvOrderPayments.Location = new System.Drawing.Point(3, 3);
			this.dgvOrderPayments.MultiSelect = false;
			this.dgvOrderPayments.Name = "dgvOrderPayments";
			this.dgvOrderPayments.ReadOnly = true;
			this.dgvOrderPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvOrderPayments.Size = new System.Drawing.Size(770, 191);
			this.dgvOrderPayments.TabIndex = 0;
			// 
			// colTransactionId
			// 
			this.colTransactionId.Frozen = true;
			this.colTransactionId.HeaderText = "Id";
			this.colTransactionId.Name = "colTransactionId";
			this.colTransactionId.ReadOnly = true;
			// 
			// colTransactionReference
			// 
			this.colTransactionReference.HeaderText = "Reference";
			this.colTransactionReference.Name = "colTransactionReference";
			this.colTransactionReference.ReadOnly = true;
			// 
			// colTransactionInvoice
			// 
			this.colTransactionInvoice.HeaderText = "Invoice";
			this.colTransactionInvoice.Name = "colTransactionInvoice";
			this.colTransactionInvoice.ReadOnly = true;
			// 
			// colTransactionPaymentAmount
			// 
			this.colTransactionPaymentAmount.HeaderText = "Payment Amount";
			this.colTransactionPaymentAmount.Name = "colTransactionPaymentAmount";
			this.colTransactionPaymentAmount.ReadOnly = true;
			// 
			// colTransactionAcceptLess
			// 
			this.colTransactionAcceptLess.HeaderText = "Accept Less?";
			this.colTransactionAcceptLess.Name = "colTransactionAcceptLess";
			this.colTransactionAcceptLess.ReadOnly = true;
			// 
			// colTransactionPartnerInitiated
			// 
			this.colTransactionPartnerInitiated.HeaderText = "Partner Initiated?";
			this.colTransactionPartnerInitiated.Name = "colTransactionPartnerInitiated";
			this.colTransactionPartnerInitiated.ReadOnly = true;
			// 
			// colTransactionPartner
			// 
			this.colTransactionPartner.HeaderText = "Partner";
			this.colTransactionPartner.Name = "colTransactionPartner";
			this.colTransactionPartner.ReadOnly = true;
			// 
			// colTransactionStatus
			// 
			this.colTransactionStatus.HeaderText = "Status";
			this.colTransactionStatus.Name = "colTransactionStatus";
			this.colTransactionStatus.ReadOnly = true;
			// 
			// colTransactionVersion
			// 
			this.colTransactionVersion.HeaderText = "Version";
			this.colTransactionVersion.Name = "colTransactionVersion";
			this.colTransactionVersion.ReadOnly = true;
			// 
			// colTransactionUrl
			// 
			this.colTransactionUrl.HeaderText = "URL";
			this.colTransactionUrl.Name = "colTransactionUrl";
			this.colTransactionUrl.ReadOnly = true;
			// 
			// SampleOrderListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.gbxMain);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.pnlBottom);
			this.Name = "SampleOrderListForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Current Orders";
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.statusbar.ResumeLayout(false);
			this.statusbar.PerformLayout();
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.gbxMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).EndInit();
			this.tcOrderChildViews.ResumeLayout(false);
			this.tpOrderItems.ResumeLayout(false);
			this.tpOrderSurcounts.ResumeLayout(false);
			this.tpOrderPayments.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSurcounts)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvOrderPayments)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.ToolStripButton btnAddOrder;
		private System.Windows.Forms.GroupBox gbxMain;
		private System.Windows.Forms.DataGridView dgvOrders;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.ToolStripStatusLabel lblIconLink;
		private System.Windows.Forms.ToolStripButton btnEditOrder;
		private System.Windows.Forms.ToolStripButton btnDeleteOrder;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderDoshiiId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderStatus;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderInvoiceId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderVersion;
		private System.Windows.Forms.DataGridViewLinkColumn colOrderUrl;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemCount;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemValue;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderSurcountCount;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderSurcountValue;
		private System.Windows.Forms.TabControl tcOrderChildViews;
		private System.Windows.Forms.TabPage tpOrderItems;
		private System.Windows.Forms.TabPage tpOrderSurcounts;
		private System.Windows.Forms.TabPage tpOrderPayments;
		private System.Windows.Forms.DataGridView dgvOrderItems;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemDescription;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemPrice;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemTags;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemVersion;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemPosId;
		private System.Windows.Forms.DataGridViewLinkColumn colOrderItemImageUrl;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemStatus;
		private System.Windows.Forms.DataGridView dgvSurcounts;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderSurcountsName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colOrderSurcountsPrice;
		private System.Windows.Forms.DataGridView dgvOrderPayments;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionReference;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionInvoice;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionPaymentAmount;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colTransactionAcceptLess;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colTransactionPartnerInitiated;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionPartner;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionStatus;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionVersion;
		private System.Windows.Forms.DataGridViewLinkColumn colTransactionUrl;
	}
}