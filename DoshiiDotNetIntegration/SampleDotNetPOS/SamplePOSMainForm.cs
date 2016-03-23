using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleDotNetPOS
{
	/// <summary>
	/// The main form of the Sample .NET POS application.
	/// This form displays a series of buttons that the end user can press to see the effect of each function
	/// available to the POS via the Doshii .NET SDK.
	/// </summary>
	/// <remarks>
	/// As the POS provider, your job will be to implement the interfaces in the POSImpl namespace of this sample
	/// application. This sample blindly accepts what the SDK informs it to do and outputs some sample data to 
	/// the text fields in this form, however you will be required to handle each call in a manner appropriate
	/// to your point of sale software.
	/// </remarks>
	public partial class SamplePOSMainForm : Form
	{
		/// <summary>
		/// The authorisation token for the Sample .NET POS application for communication
		/// with the Doshii RESTful API.
		/// </summary>
		/// <remarks>
		/// Note: This token should NOT be used in your implementation.
		/// Doshii will provide POS providers with a unique token which identifies them in their communication
		/// with the RESTful API. Communication using the Sample .NET POS application token will not be sent to
		/// live partner applications.
		/// </remarks>
		private const string SamplePOSAuthToken = "7B2RDcb7atv8QW6YqDCxqsoHvJc";

		/// <summary>
		/// Presenter controlling the view.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Synchronisation lock object for the logger.
		/// </summary>
		private static readonly object logLock = new object();

		/// <summary>
		/// Constructor.
		/// </summary>
		public SamplePOSMainForm()
		{
			InitializeComponent();
			this.Text = String.Format("Sample .NET POS v{0}", Assembly.GetExecutingAssembly().GetName().Version);
			lblCopyright.Text = String.Format("\u00a9 Copyright {0:yyyy} Doshii Pty Ltd. All Rights Reserved.", DateTime.Today);

			cbxApiAddress.SelectedIndex = 0;
            LocationToken = "7B2RDcb7atv8QW6YqDCxqsoHvJc";

			EnableDisableControls(true);
		}

		/// <summary>
		/// Attaches the supplied <paramref name="presenter"/> to the view.
		/// </summary>
		/// <param name="presenter">The presenter controlling the view.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the supplied <paramref name="presenter"/>
		/// is <c>null</c> - use a call to <see cref="SampleDotNetPOS.SamplePOSMainForm.RemovePresenter()"/>
		/// to remove the reference instead.</exception>
		public void AttachPresenter(SampleDotNetPOSPresenter presenter)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");
			mPresenter = presenter;
		}

		/// <summary>
		/// Removes the presenter reference from the view.
		/// </summary>
		public void RemovePresenter()
		{
			mPresenter = null;
		}

		/// <summary>
		/// Gets/Sets the API address for the POS.
		/// </summary>
		public string SelectedPosApiUrl
		{
			get
			{
				return cbxApiAddress.Text;
			}

			set
			{
				cbxApiAddress.Text = value;
			}
		}

		/// <summary>
		/// Gets/Sets the location token for the POS.
		/// </summary>
		public string LocationToken
		{
			get
			{
				return tbxLocationToken.Text;
			}

			set
			{
				tbxLocationToken.Text = value;
			}
		}

		/// <summary>
		/// Writes a <paramref name="message"/> to the view in the supplied <paramref name="textColour"/>
		/// and <paramref name="style"/>. Handles multi-threading by passing off to an invoked delegate method
		/// if required.
		/// </summary>
		/// <param name="message">The message to be written to the view.</param>
		/// <param name="textColour">The colour to write the message in.</param>
		/// <param name="backColour">The colour to highlight the message in.</param>
		/// <param name="style">The style to write the message in.</param>
		public void WriteMessage(string message, Color? textColour, Color? backColour, FontStyle style)
		{
			if (InvokeRequired)
				BeginInvoke(new LogDelegate(UpdateLog), new object[] { message, textColour, backColour, style });
			else
				UpdateLog(message, textColour, backColour, style);
		}

		/// <summary>
		/// Updates the label that displays the current number of orders currently in memory.
		/// </summary>
		/// <param name="count">The number of orders.</param>
		public void UpdateOrderCountLabel(int count)
		{
			if (count == 0)
			{
				lblOrderCount.Visible = false;
			}
			else
			{
				lblOrderCount.Visible = true;
				lblOrderCount.Text = String.Format("{0} Order{1}", count, count == 1 ? String.Empty : "s");
			}
		}

		/// <summary>
		/// Updates the label that displays the current number of transactions currently in memory.
		/// </summary>
		/// <param name="count">The number of orders.</param>
		public void UpdatePaymentCountLabel(int count)
		{
			if (count == 0)
			{
				lblPaymentCount.Visible = false;
			}
			else
			{
				lblPaymentCount.Visible = true;
				lblPaymentCount.Text = String.Format("{0} Payment{1}", count, count == 1 ? String.Empty : "s");
			}
		}

		/// <summary>
		/// Delegate that performs logging in the view.
		/// </summary>
		/// <param name="message">The message to be written to the view.</param>
		/// <param name="textColour">The colour to write the message in.</param>
		/// <param name="backColour">The colour to highlight the message in.</param>
		/// <param name="style">The style to write the message in.</param>
		private delegate void LogDelegate(string message, Color? textColour, Color? backColour, FontStyle style);

		/// <summary>
		/// Updates the application log.
		/// </summary>
		/// <param name="message">The message to be written to the view.</param>
		/// <param name="textColour">The colour to write the message in.</param>
		/// <param name="backColour">The colour to highlight the message in.</param>
		/// <param name="style">The style to write the message in.</param>
		private void UpdateLog(string message, Color? textColour, Color? backColour, FontStyle style)
		{
			lock (logLock)
			{
				int start = rtbLog.Text.Length;
				int length = message.Length;

				rtbLog.AppendText(message);

				rtbLog.SelectionStart = start;
				rtbLog.SelectionLength = length;

				if (textColour.HasValue)
					rtbLog.SelectionColor = textColour.Value;
				if (backColour.HasValue)
					rtbLog.SelectionBackColor = backColour.Value;
				rtbLog.SelectionFont = new Font(rtbLog.Font, style);

				rtbLog.SelectionLength = 0;
				rtbLog.SelectionStart = rtbLog.Text.Length;

				rtbLog.Refresh();
			}
		}

		/// <summary>
		/// Callback for when the LOG button is clicked.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void btnTestLogging_Click(object sender, EventArgs e)
		{
			Type t = typeof(SamplePOSMainForm);

			string message = "This is a test debug message";
			mPresenter.LogMessage(t, message, DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Debug);

			message = "This is a test info message";
			mPresenter.LogMessage(t, message, DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Info);

			message = "This is a test warning message";
			mPresenter.LogMessage(t, message, DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning);

			message = "This is a test error message with exception";
			try
			{
				throw new Exception("Test Exception");
			}
			catch (Exception ex)
			{
				mPresenter.LogMessage(t, message, DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Error, ex);
			}

			message = "This is a test fatal message with exception";
			try
			{
				throw new Exception("Test Exception");
			}
			catch (Exception ex)
			{
				mPresenter.LogMessage(t, message, DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Fatal, ex);
			}
		}

		/// <summary>
		/// Callback for when the Initialise button is pressed.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void btnInitialise_Click(object sender, EventArgs e)
		{
			if (mPresenter != null)
			{
				if (String.IsNullOrWhiteSpace(SelectedPosApiUrl))
				{
					MessageBox.Show("Please select or enter the API address for the POS.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					cbxApiAddress.Focus();
					Application.DoEvents();
					return;
				}

				if (String.IsNullOrWhiteSpace(LocationToken))
				{
					MessageBox.Show("Please enter the location token for the POS.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					tbxLocationToken.Focus();
					Application.DoEvents();
					return;
				}

				mPresenter.Initialise(SelectedPosApiUrl, LocationToken);

				EnableDisableControls(false);
			}
		}

		/// <summary>
		/// Enables or disables the controls on the form based on whether the initialise step has occurred.
		/// </summary>
		/// <param name="enableInitialiseControls">True indicates that initialisation is required; false otherwise.</param>
		private void EnableDisableControls(bool enableInitialiseControls)
		{
			bool enableNonInitialisationControls = !enableInitialiseControls;

			cbxApiAddress.Enabled = enableInitialiseControls;
			tbxLocationToken.Enabled = enableInitialiseControls;
			btnInitialise.Enabled = enableInitialiseControls;

			btnTestLogging.Enabled = enableNonInitialisationControls;
			btnViewOrders.Enabled = enableNonInitialisationControls;
		}

		/// <summary>
		/// Callback for when the VIEW ORDERS button is pressed.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void btnViewOrders_Click(object sender, EventArgs e)
		{
			if (mPresenter != null)
			{
				mPresenter.DisplayOrderList();
			}
		}
	}
}
