using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;
using SampleDotNetPOS.POSImpl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleDotNetPOS
{
	/// <summary>
	/// The presenter class for the main form of the SampleDotNetPOS application is the main driver of the sample application.
	/// It forms the controller in a traditional MVC workflow, and communicates between the view and its actions.
	/// </summary>
	public class SampleDotNetPOSPresenter : IDisposable
	{
		/// <summary>
		/// Application authorisation token for Sample .NET POS application.
		/// </summary>
		private const string AuthToken = "7B2RDcb7atv8QW6YqDCxqsoHvJc";

		#region Member variables

		/// <summary>
		/// The main view for the SampleDotNetPOS application.
		/// </summary>
		private SamplePOSMainForm mView;

		/// <summary>
		/// The logging mechanism for the SampleDotNetPOS application.
		/// </summary>
		private SampleDoshiiLogger mLog;

		/// <summary>
		/// The sample payment manager for the SampleDotNetPOS application.
		/// </summary>
		private SamplePaymentModuleManager mPaymentManager;

        /// <summary>
        /// The sample payment manager for the SampleDotNetPOS application.
        /// </summary>
        private SampleOrderingManager mOrderingManager;

		/// <summary>
		/// The doshii logic manager.
		/// </summary>
		private DoshiiManager mManager;

		/// <summary>
		/// Current list of orders in Doshii.
		/// </summary>
		private List<Order> mOrders;

		/// <summary>
		/// Current list of payments in Doshii.
		/// </summary>
		private List<Transaction> mPayments;

		#endregion

		#region Initialisation

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="view">The main view of the application.</param>
		public SampleDotNetPOSPresenter(SamplePOSMainForm view)
		{
			if (view == null)
				throw new ArgumentNullException("view");

			mView = view;
			mLog = new SampleDoshiiLogger(this);
			mPaymentManager = new SamplePaymentModuleManager();
            mOrderingManager = new SampleOrderingManager(this);
			mManager = new DoshiiManager(mPaymentManager, mLog, mOrderingManager);
			mOrders = new List<Order>();
			mPayments = new List<Transaction>();
		}

		/// <summary>
		/// Launches the application and returns the main form for the application.
		/// </summary>
		/// <returns>The main form of the application.</returns>
		public Form Launch()
		{
			mView.AttachPresenter(this);
			mPaymentManager.AttachPresenter(this);
			return mView;
		}

		/// <summary>
		/// Initialises the application.
		/// </summary>
		/// <param name="apiAddress">The selected API address in the view.</param>
		/// <param name="locationToken">The entered location token in the view.</param>
		public void Initialise(string apiAddress, string locationToken)
		{
			string wss = String.Format("{0}/socket", apiAddress.Replace("http", "ws"));
			mManager.Initialize(wss, SampleDotNetPOSPresenter.AuthToken, apiAddress, true, 0, new Configuration(true, true));
			mOrders = mManager.GetOrders().ToList<Order>();
			mPayments = mManager.GetTransactions().ToList<Transaction>();
		}

		#endregion

		#region Configuration

		/// <summary>
		/// Retrieves the configuration for Doshii.
		/// </summary>
		/// <returns>The configuration for Doshii; or <c>null</c> if not found.</returns>
		public Configuration RetrieveConfiguration()
		{
			try
			{
				return mManager.GetConfiguration();
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Edits the configuration in Doshii.
		/// </summary>
		/// <param name="configuration">Current configuration object to be edited.</param>
		/// <returns>True on successful update; false otherwise.</returns>
		public bool EditConfiguration(Configuration configuration)
		{
			using (var form = new SampleConfigForm())
			{
				var config = form.Display(configuration);
				if (config != null)
				{
					if (config.CheckoutOnPaid != configuration.CheckoutOnPaid || config.DeallocateTableOnPaid != configuration.DeallocateTableOnPaid)
					{
						try
						{
							return mManager.UpdateConfiguration(config);
						}
						catch
						{

						}
					}
				}
			}

			return false;
		}

		#endregion

		#region Ordering

		/// <summary>
		/// Retrieves an order from the current list of orders by <paramref name="orderId"/>.
		/// </summary>
		/// <param name="orderId">The ID of the order to be retrieved.</param>
		/// <returns>The order if available; or <c>null</c> otherwise.</returns>
		public Order RetrieveOrder(string orderId)
		{
			return mOrders.FirstOrDefault(o => o.Id == orderId);
		}

		/// <summary>
		/// Sends a POS-generated update of an order to the Doshii Manager.
		/// </summary>
		/// <returns>The updated details for the order.</returns>
		public Order SendOrder()
		{
			var order = GenerateOrder();

			if (order != null)
			{
				order = mManager.UpdateOrder(order);
				AddOrUpdateOrder(order);
			}

			return order;
		}

		/// <summary>
		/// Adds the order if it doesn't already exist, otherwise updates it.
		/// </summary>
		/// <param name="order">The order to be added or updated.</param>
		public void AddOrUpdateOrder(Order order)
		{
			int index = mOrders.IndexOf(order);
			if (index < 0)
				mOrders.Add(order);
			else
				mOrders[index] = order;
		}

		/// <summary>
		/// Removes the order from the list.
		/// </summary>
		/// <param name="orderId">The Id of the order.</param>
		public void RemoveOrder(string orderId)
		{
			var order = RetrieveOrder(orderId);
			if (order != null)
				mOrders.Remove(order);
		}

		/// <summary>
		/// Generates a sample order via user input.
		/// </summary>
		/// <returns>The details for the order.</returns>
		private Order GenerateOrder()
		{
			using (var form = new SampleOrderForm())
			{
				form.AttachPresenter(this);

				try
				{
					if (form.ShowDialog() == DialogResult.OK)
					{
						var order = form.ReadForm();
						return order;
					}
				}
				finally
				{
					form.RemovePresenter();
				}
			}

			return null;
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Retrieves the transaction with corresponding <paramref name="transactionId"/> from the current list.
		/// </summary>
		/// <param name="transactionId">The Id of the transaction being requested from the current list.</param>
		/// <returns></returns>
		public Transaction RetrieveTransaction(string transactionId)
		{
			return mPayments.FirstOrDefault(o => o.Id == transactionId);
		}

		/// <summary>
		/// Removes a transaction from the list.
		/// </summary>
		/// <param name="transactionId">The Id of the payment to be removed.</param>
		public void RemoveTransaction(string transactionId)
		{
			var payment = RetrieveTransaction(transactionId);
			if (payment != null)
				mPayments.Remove(payment);
		}

		#endregion

		#region Logging

		/// <summary>
		/// Logs a message to the logging mechanism.
		/// </summary>
		/// <param name="callingClass">The calling class type.</param>
		/// <param name="message">The message to print.</param>
		/// <param name="level">The level to log at.</param>
		/// <param name="e">An optional exception.</param>
		public void LogMessage(Type callingClass, string message, DoshiiLogLevels level, Exception e = null)
		{
			mLog.LogDoshiiMessage(callingClass, level, message, e);
		}

		/// <summary>
		/// Sends a <paramref name="message"/> to display in the application view at the appropriate logging <paramref name="level"/>.
		/// </summary>
		/// <param name="message">Message to be sent to the view.</param>
		/// <param name="level">The level to log the message at.</param>
		public void SendMessage(string message, DoshiiLogLevels level)
		{
			var style = SampleDotNetPOSPresenter.StyleFromLevel(level);
			mView.WriteMessage(message, style.TextColour, style.BackColour, style.Style);
		}

		/// <summary>
		/// Returns a style for a logging message in the view that is appropriate for the supplied <paramref name="level"/>.
		/// </summary>
		/// <param name="level">The level of the logging message.</param>
		/// <returns>The appropriate style for the supplied logging <paramref name="level"/>.</returns>
		private static LogStyle StyleFromLevel(DoshiiLogLevels level)
		{
			var style = new LogStyle();

			switch (level)
			{
				case DoshiiLogLevels.Debug:
					style.TextColour = null;
					style.BackColour = null;
					style.Style = FontStyle.Italic;
					break;
				case DoshiiLogLevels.Info:
					style.TextColour = null;
					style.BackColour = null;
					style.Style = FontStyle.Regular;
					break;
				case DoshiiLogLevels.Warning:
					style.TextColour = Color.FromKnownColor(KnownColor.Orange);
					style.BackColour = null;
					style.Style = FontStyle.Regular;
					break;
				case DoshiiLogLevels.Error:
					style.TextColour = Color.FromKnownColor(KnownColor.Crimson);
					style.BackColour = null;
					style.Style = FontStyle.Bold;
					break;
				case DoshiiLogLevels.Fatal:
					style.TextColour = Color.FromKnownColor(KnownColor.WhiteSmoke);
					style.BackColour = Color.FromKnownColor(KnownColor.Crimson);
					style.Style = FontStyle.Bold;
					break;
			}

			return style;
		}

		/// <summary>
		/// Internal class that defines a display style for a logging message in the presenter's view.
		/// </summary>
		private class LogStyle
		{
			/// <summary>
			/// The text colour of the style defines the colour of the font for a logged message in the presenter's view.
			/// If this is <c>null</c>, the component's default forecolour is used instead.
			/// </summary>
			public Color? TextColour { get; set; }

			/// <summary>
			/// The back colour of the style defines the highlight colour of the font for a logged message in the presenter's view.
			/// If this is <c>null</c>, the component's default backcolour is used instead.
			/// </summary>
			public Color? BackColour { get; set; }

			/// <summary>
			/// The font style of the style defines the style applied to the font for a logged message in the presenter's view.
			/// </summary>
			public FontStyle Style { get; set; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes cleanly of the instance.
		/// </summary>
		public void Dispose()
		{
			if (mView != null)
			{
				mView.RemovePresenter();
				mView.Dispose();
				mView = null;
			}

			if (mLog != null)
			{
				mLog.Dispose();
				mLog = null;
			}

			if (mPaymentManager != null)
			{
				mPaymentManager.Dispose();
				mPaymentManager = null;
			}

			if (mManager != null)
			{
				mManager.Dispose();
				mManager = null;
			}
		}

		#endregion
	}
}
