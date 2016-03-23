namespace SampleDotNetPOS
{
	partial class SampleOrderForm
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
			this.gbxGeneral = new System.Windows.Forms.GroupBox();
			this.lblUri = new System.Windows.Forms.LinkLabel();
			this.lblUriCaption = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblVersionCaption = new System.Windows.Forms.Label();
			this.nudNotPayingTotal = new System.Windows.Forms.NumericUpDown();
			this.lblNotPayingTotal = new System.Windows.Forms.Label();
			this.nudPayTotal = new System.Windows.Forms.NumericUpDown();
			this.lblPayTotal = new System.Windows.Forms.Label();
			this.nudSplitWays = new System.Windows.Forms.NumericUpDown();
			this.lblSplitWays = new System.Windows.Forms.Label();
			this.nudPaySplits = new System.Windows.Forms.NumericUpDown();
			this.lblPaySplits = new System.Windows.Forms.Label();
			this.nudTip = new System.Windows.Forms.NumericUpDown();
			this.lblTip = new System.Windows.Forms.Label();
			this.tbxLocationId = new System.Windows.Forms.TextBox();
			this.lblLocationId = new System.Windows.Forms.Label();
			this.tbxCheckinId = new System.Windows.Forms.TextBox();
			this.lblCheckinId = new System.Windows.Forms.Label();
			this.tbxTransactionId = new System.Windows.Forms.TextBox();
			this.lblTransactionId = new System.Windows.Forms.Label();
			this.tbxInvoiceId = new System.Windows.Forms.TextBox();
			this.lblInvoiceId = new System.Windows.Forms.Label();
			this.cbxStatus = new System.Windows.Forms.ComboBox();
			this.lblStatus = new System.Windows.Forms.Label();
			this.btnRetrieve = new System.Windows.Forms.Button();
			this.tbxOrderId = new System.Windows.Forms.TextBox();
			this.lblOrderId = new System.Windows.Forms.Label();
			this.gbxItems = new System.Windows.Forms.GroupBox();
			this.dgvItems = new System.Windows.Forms.DataGridView();
			this.gbxSurcounts = new System.Windows.Forms.GroupBox();
			this.dgvSurcounts = new System.Windows.Forms.DataGridView();
			this.colSurcountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSurcountPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gbxPayments = new System.Windows.Forms.GroupBox();
			this.dgvPayments = new System.Windows.Forms.DataGridView();
			this.colPaymentType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPaymentAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPaymentStatus = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemPosId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemImageUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colItemStatus = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.gbxGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNotPayingTotal)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPayTotal)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSplitWays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPaySplits)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTip)).BeginInit();
			this.gbxItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
			this.gbxSurcounts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSurcounts)).BeginInit();
			this.gbxPayments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
			this.SuspendLayout();
			// 
			// gbxGeneral
			// 
			this.gbxGeneral.Controls.Add(this.lblUri);
			this.gbxGeneral.Controls.Add(this.lblUriCaption);
			this.gbxGeneral.Controls.Add(this.lblVersion);
			this.gbxGeneral.Controls.Add(this.lblVersionCaption);
			this.gbxGeneral.Controls.Add(this.nudNotPayingTotal);
			this.gbxGeneral.Controls.Add(this.lblNotPayingTotal);
			this.gbxGeneral.Controls.Add(this.nudPayTotal);
			this.gbxGeneral.Controls.Add(this.lblPayTotal);
			this.gbxGeneral.Controls.Add(this.nudSplitWays);
			this.gbxGeneral.Controls.Add(this.lblSplitWays);
			this.gbxGeneral.Controls.Add(this.nudPaySplits);
			this.gbxGeneral.Controls.Add(this.lblPaySplits);
			this.gbxGeneral.Controls.Add(this.nudTip);
			this.gbxGeneral.Controls.Add(this.lblTip);
			this.gbxGeneral.Controls.Add(this.tbxLocationId);
			this.gbxGeneral.Controls.Add(this.lblLocationId);
			this.gbxGeneral.Controls.Add(this.tbxCheckinId);
			this.gbxGeneral.Controls.Add(this.lblCheckinId);
			this.gbxGeneral.Controls.Add(this.tbxTransactionId);
			this.gbxGeneral.Controls.Add(this.lblTransactionId);
			this.gbxGeneral.Controls.Add(this.tbxInvoiceId);
			this.gbxGeneral.Controls.Add(this.lblInvoiceId);
			this.gbxGeneral.Controls.Add(this.cbxStatus);
			this.gbxGeneral.Controls.Add(this.lblStatus);
			this.gbxGeneral.Controls.Add(this.btnRetrieve);
			this.gbxGeneral.Controls.Add(this.tbxOrderId);
			this.gbxGeneral.Controls.Add(this.lblOrderId);
			this.gbxGeneral.Dock = System.Windows.Forms.DockStyle.Left;
			this.gbxGeneral.Location = new System.Drawing.Point(0, 0);
			this.gbxGeneral.Name = "gbxGeneral";
			this.gbxGeneral.Size = new System.Drawing.Size(321, 392);
			this.gbxGeneral.TabIndex = 0;
			this.gbxGeneral.TabStop = false;
			this.gbxGeneral.Text = "General Settings";
			// 
			// lblUri
			// 
			this.lblUri.AutoSize = true;
			this.lblUri.Location = new System.Drawing.Point(106, 362);
			this.lblUri.Name = "lblUri";
			this.lblUri.Size = new System.Drawing.Size(199, 13);
			this.lblUri.TabIndex = 53;
			this.lblUri.TabStop = true;
			this.lblUri.Text = "https://alphasandbox.doshii.co/orders/1";
			// 
			// lblUriCaption
			// 
			this.lblUriCaption.AutoSize = true;
			this.lblUriCaption.Location = new System.Drawing.Point(9, 363);
			this.lblUriCaption.Name = "lblUriCaption";
			this.lblUriCaption.Size = new System.Drawing.Size(29, 13);
			this.lblUriCaption.TabIndex = 52;
			this.lblUriCaption.Text = "URI:";
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Location = new System.Drawing.Point(103, 334);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(73, 13);
			this.lblVersion.TabIndex = 51;
			this.lblVersion.Text = "kJkdhNW334";
			// 
			// lblVersionCaption
			// 
			this.lblVersionCaption.AutoSize = true;
			this.lblVersionCaption.Location = new System.Drawing.Point(6, 334);
			this.lblVersionCaption.Name = "lblVersionCaption";
			this.lblVersionCaption.Size = new System.Drawing.Size(45, 13);
			this.lblVersionCaption.TabIndex = 50;
			this.lblVersionCaption.Text = "Version:";
			// 
			// nudNotPayingTotal
			// 
			this.nudNotPayingTotal.DecimalPlaces = 2;
			this.nudNotPayingTotal.Location = new System.Drawing.Point(106, 300);
			this.nudNotPayingTotal.Name = "nudNotPayingTotal";
			this.nudNotPayingTotal.Size = new System.Drawing.Size(203, 20);
			this.nudNotPayingTotal.TabIndex = 49;
			// 
			// lblNotPayingTotal
			// 
			this.lblNotPayingTotal.AutoSize = true;
			this.lblNotPayingTotal.Location = new System.Drawing.Point(6, 300);
			this.lblNotPayingTotal.Name = "lblNotPayingTotal";
			this.lblNotPayingTotal.Size = new System.Drawing.Size(89, 13);
			this.lblNotPayingTotal.TabIndex = 48;
			this.lblNotPayingTotal.Text = "Not Paying Total:";
			// 
			// nudPayTotal
			// 
			this.nudPayTotal.DecimalPlaces = 2;
			this.nudPayTotal.Location = new System.Drawing.Point(106, 273);
			this.nudPayTotal.Name = "nudPayTotal";
			this.nudPayTotal.Size = new System.Drawing.Size(203, 20);
			this.nudPayTotal.TabIndex = 47;
			// 
			// lblPayTotal
			// 
			this.lblPayTotal.AutoSize = true;
			this.lblPayTotal.Location = new System.Drawing.Point(6, 275);
			this.lblPayTotal.Name = "lblPayTotal";
			this.lblPayTotal.Size = new System.Drawing.Size(55, 13);
			this.lblPayTotal.TabIndex = 46;
			this.lblPayTotal.Text = "Pay Total:";
			// 
			// nudSplitWays
			// 
			this.nudSplitWays.Location = new System.Drawing.Point(106, 246);
			this.nudSplitWays.Name = "nudSplitWays";
			this.nudSplitWays.Size = new System.Drawing.Size(203, 20);
			this.nudSplitWays.TabIndex = 45;
			// 
			// lblSplitWays
			// 
			this.lblSplitWays.AutoSize = true;
			this.lblSplitWays.Location = new System.Drawing.Point(6, 248);
			this.lblSplitWays.Name = "lblSplitWays";
			this.lblSplitWays.Size = new System.Drawing.Size(60, 13);
			this.lblSplitWays.TabIndex = 44;
			this.lblSplitWays.Text = "Split Ways:";
			// 
			// nudPaySplits
			// 
			this.nudPaySplits.Location = new System.Drawing.Point(106, 220);
			this.nudPaySplits.Name = "nudPaySplits";
			this.nudPaySplits.Size = new System.Drawing.Size(203, 20);
			this.nudPaySplits.TabIndex = 43;
			// 
			// lblPaySplits
			// 
			this.lblPaySplits.AutoSize = true;
			this.lblPaySplits.Location = new System.Drawing.Point(6, 220);
			this.lblPaySplits.Name = "lblPaySplits";
			this.lblPaySplits.Size = new System.Drawing.Size(56, 13);
			this.lblPaySplits.TabIndex = 42;
			this.lblPaySplits.Text = "Pay Splits:";
			// 
			// nudTip
			// 
			this.nudTip.DecimalPlaces = 2;
			this.nudTip.Location = new System.Drawing.Point(106, 193);
			this.nudTip.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.nudTip.Name = "nudTip";
			this.nudTip.Size = new System.Drawing.Size(203, 20);
			this.nudTip.TabIndex = 41;
			// 
			// lblTip
			// 
			this.lblTip.AutoSize = true;
			this.lblTip.Location = new System.Drawing.Point(6, 195);
			this.lblTip.Name = "lblTip";
			this.lblTip.Size = new System.Drawing.Size(25, 13);
			this.lblTip.TabIndex = 40;
			this.lblTip.Text = "Tip:";
			// 
			// tbxLocationId
			// 
			this.tbxLocationId.Location = new System.Drawing.Point(106, 166);
			this.tbxLocationId.Name = "tbxLocationId";
			this.tbxLocationId.Size = new System.Drawing.Size(203, 20);
			this.tbxLocationId.TabIndex = 39;
			// 
			// lblLocationId
			// 
			this.lblLocationId.AutoSize = true;
			this.lblLocationId.Location = new System.Drawing.Point(6, 169);
			this.lblLocationId.Name = "lblLocationId";
			this.lblLocationId.Size = new System.Drawing.Size(63, 13);
			this.lblLocationId.TabIndex = 38;
			this.lblLocationId.Text = "Location Id:";
			// 
			// tbxCheckinId
			// 
			this.tbxCheckinId.Location = new System.Drawing.Point(106, 140);
			this.tbxCheckinId.Name = "tbxCheckinId";
			this.tbxCheckinId.Size = new System.Drawing.Size(203, 20);
			this.tbxCheckinId.TabIndex = 37;
			// 
			// lblCheckinId
			// 
			this.lblCheckinId.AutoSize = true;
			this.lblCheckinId.Location = new System.Drawing.Point(6, 143);
			this.lblCheckinId.Name = "lblCheckinId";
			this.lblCheckinId.Size = new System.Drawing.Size(61, 13);
			this.lblCheckinId.TabIndex = 36;
			this.lblCheckinId.Text = "Checkin Id:";
			// 
			// tbxTransactionId
			// 
			this.tbxTransactionId.Location = new System.Drawing.Point(106, 114);
			this.tbxTransactionId.Name = "tbxTransactionId";
			this.tbxTransactionId.Size = new System.Drawing.Size(203, 20);
			this.tbxTransactionId.TabIndex = 35;
			// 
			// lblTransactionId
			// 
			this.lblTransactionId.AutoSize = true;
			this.lblTransactionId.Location = new System.Drawing.Point(6, 117);
			this.lblTransactionId.Name = "lblTransactionId";
			this.lblTransactionId.Size = new System.Drawing.Size(78, 13);
			this.lblTransactionId.TabIndex = 34;
			this.lblTransactionId.Text = "Transaction Id:";
			// 
			// tbxInvoiceId
			// 
			this.tbxInvoiceId.Location = new System.Drawing.Point(106, 88);
			this.tbxInvoiceId.Name = "tbxInvoiceId";
			this.tbxInvoiceId.Size = new System.Drawing.Size(203, 20);
			this.tbxInvoiceId.TabIndex = 33;
			// 
			// lblInvoiceId
			// 
			this.lblInvoiceId.AutoSize = true;
			this.lblInvoiceId.Location = new System.Drawing.Point(6, 91);
			this.lblInvoiceId.Name = "lblInvoiceId";
			this.lblInvoiceId.Size = new System.Drawing.Size(57, 13);
			this.lblInvoiceId.TabIndex = 32;
			this.lblInvoiceId.Text = "Invoice Id:";
			// 
			// cbxStatus
			// 
			this.cbxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxStatus.FormattingEnabled = true;
			this.cbxStatus.Items.AddRange(new object[] {
            "pending",
            "rejected",
            "accepted",
            "cancelled",
            "ready_to_pay",
            "waiting_for_payment",
            "paid"});
			this.cbxStatus.Location = new System.Drawing.Point(106, 57);
			this.cbxStatus.Name = "cbxStatus";
			this.cbxStatus.Size = new System.Drawing.Size(203, 21);
			this.cbxStatus.TabIndex = 31;
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(6, 60);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(40, 13);
			this.lblStatus.TabIndex = 30;
			this.lblStatus.Text = "Status:";
			// 
			// btnRetrieve
			// 
			this.btnRetrieve.Location = new System.Drawing.Point(251, 23);
			this.btnRetrieve.Name = "btnRetrieve";
			this.btnRetrieve.Size = new System.Drawing.Size(58, 23);
			this.btnRetrieve.TabIndex = 29;
			this.btnRetrieve.Text = "Retrieve";
			this.btnRetrieve.UseVisualStyleBackColor = true;
			// 
			// tbxOrderId
			// 
			this.tbxOrderId.Location = new System.Drawing.Point(106, 25);
			this.tbxOrderId.Name = "tbxOrderId";
			this.tbxOrderId.Size = new System.Drawing.Size(139, 20);
			this.tbxOrderId.TabIndex = 28;
			// 
			// lblOrderId
			// 
			this.lblOrderId.AutoSize = true;
			this.lblOrderId.Location = new System.Drawing.Point(6, 28);
			this.lblOrderId.Name = "lblOrderId";
			this.lblOrderId.Size = new System.Drawing.Size(19, 13);
			this.lblOrderId.TabIndex = 27;
			this.lblOrderId.Text = "Id:";
			// 
			// gbxItems
			// 
			this.gbxItems.Controls.Add(this.dgvItems);
			this.gbxItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbxItems.Location = new System.Drawing.Point(321, 0);
			this.gbxItems.Name = "gbxItems";
			this.gbxItems.Size = new System.Drawing.Size(463, 292);
			this.gbxItems.TabIndex = 1;
			this.gbxItems.TabStop = false;
			this.gbxItems.Text = "Items";
			// 
			// dgvItems
			// 
			this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemId,
            this.colItemName,
            this.colItemDescription,
            this.colItemPrice,
            this.colItemTags,
            this.colItemPosId,
            this.colItemImageUrl,
            this.colItemStatus});
			this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvItems.Location = new System.Drawing.Point(3, 16);
			this.dgvItems.Name = "dgvItems";
			this.dgvItems.Size = new System.Drawing.Size(457, 273);
			this.dgvItems.TabIndex = 0;
			// 
			// gbxSurcounts
			// 
			this.gbxSurcounts.Controls.Add(this.dgvSurcounts);
			this.gbxSurcounts.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gbxSurcounts.Location = new System.Drawing.Point(321, 292);
			this.gbxSurcounts.Name = "gbxSurcounts";
			this.gbxSurcounts.Size = new System.Drawing.Size(463, 100);
			this.gbxSurcounts.TabIndex = 2;
			this.gbxSurcounts.TabStop = false;
			this.gbxSurcounts.Text = "Surcounts";
			// 
			// dgvSurcounts
			// 
			this.dgvSurcounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSurcounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSurcountName,
            this.colSurcountPrice});
			this.dgvSurcounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSurcounts.Location = new System.Drawing.Point(3, 16);
			this.dgvSurcounts.Name = "dgvSurcounts";
			this.dgvSurcounts.Size = new System.Drawing.Size(457, 81);
			this.dgvSurcounts.TabIndex = 0;
			// 
			// colSurcountName
			// 
			this.colSurcountName.HeaderText = "Name";
			this.colSurcountName.Name = "colSurcountName";
			// 
			// colSurcountPrice
			// 
			this.colSurcountPrice.HeaderText = "Amount";
			this.colSurcountPrice.Name = "colSurcountPrice";
			// 
			// gbxPayments
			// 
			this.gbxPayments.Controls.Add(this.dgvPayments);
			this.gbxPayments.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gbxPayments.Location = new System.Drawing.Point(0, 392);
			this.gbxPayments.Name = "gbxPayments";
			this.gbxPayments.Size = new System.Drawing.Size(784, 170);
			this.gbxPayments.TabIndex = 3;
			this.gbxPayments.TabStop = false;
			this.gbxPayments.Text = "Payments";
			// 
			// dgvPayments
			// 
			this.dgvPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPayments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPaymentType,
            this.colPaymentAmount,
            this.colPaymentStatus});
			this.dgvPayments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPayments.Location = new System.Drawing.Point(3, 16);
			this.dgvPayments.Name = "dgvPayments";
			this.dgvPayments.Size = new System.Drawing.Size(778, 151);
			this.dgvPayments.TabIndex = 0;
			// 
			// colPaymentType
			// 
			this.colPaymentType.HeaderText = "Transaction Type";
			this.colPaymentType.Name = "colPaymentType";
			// 
			// colPaymentAmount
			// 
			this.colPaymentAmount.HeaderText = "Amount";
			this.colPaymentAmount.Name = "colPaymentAmount";
			// 
			// colPaymentStatus
			// 
			this.colPaymentStatus.HeaderText = "Status";
			this.colPaymentStatus.Items.AddRange(new object[] {
            "pending",
            "accepted"});
			this.colPaymentStatus.Name = "colPaymentStatus";
			// 
			// colItemId
			// 
			this.colItemId.HeaderText = "Id";
			this.colItemId.Name = "colItemId";
			// 
			// colItemName
			// 
			this.colItemName.HeaderText = "Name";
			this.colItemName.Name = "colItemName";
			// 
			// colItemDescription
			// 
			this.colItemDescription.HeaderText = "Description";
			this.colItemDescription.Name = "colItemDescription";
			// 
			// colItemPrice
			// 
			this.colItemPrice.HeaderText = "Amount";
			this.colItemPrice.Name = "colItemPrice";
			// 
			// colItemTags
			// 
			this.colItemTags.HeaderText = "Tags";
			this.colItemTags.Name = "colItemTags";
			// 
			// colItemPosId
			// 
			this.colItemPosId.HeaderText = "POS Id";
			this.colItemPosId.Name = "colItemPosId";
			// 
			// colItemImageUrl
			// 
			this.colItemImageUrl.HeaderText = "Image URL";
			this.colItemImageUrl.Name = "colItemImageUrl";
			// 
			// colItemStatus
			// 
			this.colItemStatus.HeaderText = "Status";
			this.colItemStatus.Items.AddRange(new object[] {
            "pending",
            "accepted"});
			this.colItemStatus.Name = "colItemStatus";
			// 
			// SampleOrderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.gbxItems);
			this.Controls.Add(this.gbxSurcounts);
			this.Controls.Add(this.gbxGeneral);
			this.Controls.Add(this.gbxPayments);
			this.Name = "SampleOrderForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Order Details";
			this.gbxGeneral.ResumeLayout(false);
			this.gbxGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNotPayingTotal)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPayTotal)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSplitWays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPaySplits)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTip)).EndInit();
			this.gbxItems.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
			this.gbxSurcounts.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSurcounts)).EndInit();
			this.gbxPayments.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbxGeneral;
		private System.Windows.Forms.LinkLabel lblUri;
		private System.Windows.Forms.Label lblUriCaption;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblVersionCaption;
		private System.Windows.Forms.NumericUpDown nudNotPayingTotal;
		private System.Windows.Forms.Label lblNotPayingTotal;
		private System.Windows.Forms.NumericUpDown nudPayTotal;
		private System.Windows.Forms.Label lblPayTotal;
		private System.Windows.Forms.NumericUpDown nudSplitWays;
		private System.Windows.Forms.Label lblSplitWays;
		private System.Windows.Forms.NumericUpDown nudPaySplits;
		private System.Windows.Forms.Label lblPaySplits;
		private System.Windows.Forms.NumericUpDown nudTip;
		private System.Windows.Forms.Label lblTip;
		private System.Windows.Forms.TextBox tbxLocationId;
		private System.Windows.Forms.Label lblLocationId;
		private System.Windows.Forms.TextBox tbxCheckinId;
		private System.Windows.Forms.Label lblCheckinId;
		private System.Windows.Forms.TextBox tbxTransactionId;
		private System.Windows.Forms.Label lblTransactionId;
		private System.Windows.Forms.TextBox tbxInvoiceId;
		private System.Windows.Forms.Label lblInvoiceId;
		private System.Windows.Forms.ComboBox cbxStatus;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button btnRetrieve;
		private System.Windows.Forms.TextBox tbxOrderId;
		private System.Windows.Forms.Label lblOrderId;
		private System.Windows.Forms.GroupBox gbxItems;
		private System.Windows.Forms.GroupBox gbxSurcounts;
		private System.Windows.Forms.GroupBox gbxPayments;
		private System.Windows.Forms.DataGridView dgvItems;
		private System.Windows.Forms.DataGridView dgvSurcounts;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSurcountName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSurcountPrice;
		private System.Windows.Forms.DataGridView dgvPayments;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPaymentType;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPaymentAmount;
		private System.Windows.Forms.DataGridViewComboBoxColumn colPaymentStatus;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemDescription;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemPrice;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemTags;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemPosId;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemImageUrl;
		private System.Windows.Forms.DataGridViewComboBoxColumn colItemStatus;

	}
}