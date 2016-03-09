namespace SampleDotNetPOS
{
	partial class SamplePOSMainForm
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
            this.ssStatusBar = new System.Windows.Forms.StatusStrip();
            this.lblCopyright = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblOrderCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPaymentCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnInitialise = new System.Windows.Forms.Button();
            this.tbxLocationToken = new System.Windows.Forms.TextBox();
            this.lblLocationToken = new System.Windows.Forms.Label();
            this.cbxApiAddress = new System.Windows.Forms.ComboBox();
            this.lblApiAddress = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pnlCommands = new System.Windows.Forms.Panel();
            this.btnViewOrders = new System.Windows.Forms.Button();
            this.btnTestLogging = new System.Windows.Forms.Button();
            this.ssStatusBar.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssStatusBar
            // 
            this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCopyright,
            this.lblOrderCount,
            this.lblPaymentCount});
            this.ssStatusBar.Location = new System.Drawing.Point(0, 538);
            this.ssStatusBar.Name = "ssStatusBar";
            this.ssStatusBar.Size = new System.Drawing.Size(784, 24);
            this.ssStatusBar.TabIndex = 4;
            // 
            // lblCopyright
            // 
            this.lblCopyright.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(60, 19);
            this.lblCopyright.Text = "Copyright";
            // 
            // lblOrderCount
            // 
            this.lblOrderCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblOrderCount.Name = "lblOrderCount";
            this.lblOrderCount.Size = new System.Drawing.Size(55, 19);
            this.lblOrderCount.Text = "0 Orders";
            // 
            // lblPaymentCount
            // 
            this.lblPaymentCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblPaymentCount.Name = "lblPaymentCount";
            this.lblPaymentCount.Size = new System.Drawing.Size(72, 19);
            this.lblPaymentCount.Text = "0 Payments";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnInitialise);
            this.pnlTop.Controls.Add(this.tbxLocationToken);
            this.pnlTop.Controls.Add(this.lblLocationToken);
            this.pnlTop.Controls.Add(this.cbxApiAddress);
            this.pnlTop.Controls.Add(this.lblApiAddress);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(784, 41);
            this.pnlTop.TabIndex = 1;
            // 
            // btnInitialise
            // 
            this.btnInitialise.Location = new System.Drawing.Point(697, 5);
            this.btnInitialise.Name = "btnInitialise";
            this.btnInitialise.Size = new System.Drawing.Size(75, 23);
            this.btnInitialise.TabIndex = 4;
            this.btnInitialise.Text = "Initialise";
            this.btnInitialise.UseVisualStyleBackColor = true;
            this.btnInitialise.Click += new System.EventHandler(this.btnInitialise_Click);
            // 
            // tbxLocationToken
            // 
            this.tbxLocationToken.Location = new System.Drawing.Point(363, 7);
            this.tbxLocationToken.Name = "tbxLocationToken";
            this.tbxLocationToken.Size = new System.Drawing.Size(328, 20);
            this.tbxLocationToken.TabIndex = 3;
            // 
            // lblLocationToken
            // 
            this.lblLocationToken.AutoSize = true;
            this.lblLocationToken.Location = new System.Drawing.Point(272, 9);
            this.lblLocationToken.Name = "lblLocationToken";
            this.lblLocationToken.Size = new System.Drawing.Size(85, 13);
            this.lblLocationToken.TabIndex = 2;
            this.lblLocationToken.Text = "Location Token:";
            // 
            // cbxApiAddress
            // 
            this.cbxApiAddress.FormattingEnabled = true;
            this.cbxApiAddress.Items.AddRange(new object[] {
            "http://sandbox.doshii.co/pos/api/v2",
            "http://sandbox.corp.doshii.co/pos/api/v2",
            "http://localhost:3000/pos/api/v2",
            "http://sandbox.doshii.co/pos/api/v1",
            "http://localhost:3000/pos/api/v1"});
            this.cbxApiAddress.Location = new System.Drawing.Point(86, 6);
            this.cbxApiAddress.Name = "cbxApiAddress";
            this.cbxApiAddress.Size = new System.Drawing.Size(180, 21);
            this.cbxApiAddress.TabIndex = 1;
            // 
            // lblApiAddress
            // 
            this.lblApiAddress.AutoSize = true;
            this.lblApiAddress.Location = new System.Drawing.Point(12, 9);
            this.lblApiAddress.Name = "lblApiAddress";
            this.lblApiAddress.Size = new System.Drawing.Size(68, 13);
            this.lblApiAddress.TabIndex = 0;
            this.lblApiAddress.Text = "API Address:";
            // 
            // rtbLog
            // 
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 41);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(784, 331);
            this.rtbLog.TabIndex = 2;
            this.rtbLog.Text = "";
            // 
            // pnlCommands
            // 
            this.pnlCommands.Controls.Add(this.btnViewOrders);
            this.pnlCommands.Controls.Add(this.btnTestLogging);
            this.pnlCommands.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCommands.Location = new System.Drawing.Point(0, 372);
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(784, 166);
            this.pnlCommands.TabIndex = 3;
            // 
            // btnViewOrders
            // 
            this.btnViewOrders.Location = new System.Drawing.Point(12, 35);
            this.btnViewOrders.Name = "btnViewOrders";
            this.btnViewOrders.Size = new System.Drawing.Size(92, 23);
            this.btnViewOrders.TabIndex = 1;
            this.btnViewOrders.Text = "VIEW ORDERS";
            this.btnViewOrders.UseVisualStyleBackColor = true;
            this.btnViewOrders.Click += new System.EventHandler(this.btnViewOrders_Click);
            // 
            // btnTestLogging
            // 
            this.btnTestLogging.Location = new System.Drawing.Point(12, 6);
            this.btnTestLogging.Name = "btnTestLogging";
            this.btnTestLogging.Size = new System.Drawing.Size(92, 23);
            this.btnTestLogging.TabIndex = 0;
            this.btnTestLogging.Text = "TEST LOG";
            this.btnTestLogging.UseVisualStyleBackColor = true;
            this.btnTestLogging.Click += new System.EventHandler(this.btnTestLogging_Click);
            // 
            // SamplePOSMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.ssStatusBar);
            this.Name = "SamplePOSMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sample .NET POS";
            this.ssStatusBar.ResumeLayout(false);
            this.ssStatusBar.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlCommands.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip ssStatusBar;
		private System.Windows.Forms.ToolStripStatusLabel lblCopyright;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.RichTextBox rtbLog;
		private System.Windows.Forms.Panel pnlCommands;
		private System.Windows.Forms.Button btnTestLogging;
		private System.Windows.Forms.TextBox tbxLocationToken;
		private System.Windows.Forms.Label lblLocationToken;
		private System.Windows.Forms.ComboBox cbxApiAddress;
		private System.Windows.Forms.Label lblApiAddress;
		private System.Windows.Forms.Button btnInitialise;
		private System.Windows.Forms.ToolStripStatusLabel lblOrderCount;
		private System.Windows.Forms.ToolStripStatusLabel lblPaymentCount;
		private System.Windows.Forms.Button btnViewOrders;
	}
}

