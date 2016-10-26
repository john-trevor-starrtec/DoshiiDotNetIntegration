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
		private const string AuthToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJkb3NoaWkgc2VydmVyIiwic3ViIjp7ImZvciI6IkxPQ0FUSU9OX1RPS0VOIiwiaWQiOiIxOSJ9LCJleHAiOjE1MDI1OTcyNTB9.Wsy52MzZslAjXoCRLu-8igjMbGS0mfxuonCWcrK7nCs"; //"7B2RDcb7atv8QW6YqDCxqsoHvJc";

		#region Member variables

		/// <summary>
		/// The main view for the SampleDotNetPOS application.
		/// </summary>
		private SamplePOSMainForm mView;

		/// <summary>
		/// The logging mechanism for the SampleDotNetPOS application.
		/// </summary>
		private SampleLoggingManager mLog;

		/// <summary>
		/// The sample payment manager for the SampleDotNetPOS application.
		/// </summary>
<<<<<<< 7e61b0f8c2f16f00fdc0c72256076e067ea307fb
		private SamplePaymentModuleManager mPaymentManager;

=======
		private SampleTransactionManager mPaymentManager;

>>>>>>> testing is going well
        /// <summary>
        /// The sample payment manager for the SampleDotNetPOS application.
        /// </summary>
        private SampleOrderingManager mOrderingManager;

		/// <summary>
		/// The doshii logic manager.
		/// </summary>
		private DoshiiController _mController;

        private SampleConfigurationManager mConfigManager;

        private SampleReservationManager mReservationManager;

		/// <summary>
		/// Current list of orders in Doshii.
		/// </summary>
		private List<Order> mOrders;

		/// <summary>
		/// Current list of payments in Doshii.
		/// </summary>
		private List<Transaction> mPayments;

        /// <summary>
        /// Current list of bookings in Doshii.
        /// </summary>
        private List<Booking> mBookings;

        private List<Table> mTables;

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
			mLog = new SampleLoggingManager(this);
			mPaymentManager = new SampleTransactionManager();
            mOrderingManager = new SampleOrderingManager(this);
<<<<<<< 7e61b0f8c2f16f00fdc0c72256076e067ea307fb
            mConfigManager = new SampleConfigurationManager();
            mReservationManager = new SampleReservationManager();
            mReservationManager.AttachPresenter(this);
			mManager = new DoshiiManager(mPaymentManager, mLog, mOrderingManager, null, mReservationManager, mConfigManager);
=======
			_mController = new DoshiiController(mPaymentManager, mLog, mOrderingManager, null);
>>>>>>> testing is going well
			mOrders = new List<Order>();
			mPayments = new List<Transaction>();
            mBookings = new List<Booking>();
            mTables = new List<Table>();

            mView.UpdateOrderCountLabel(mOrders.Count);
			mView.UpdatePaymentCountLabel(mPayments.Count);
            mView.UpdateBookingsCountLabel(mBookings.Count);
            mView.UpdateTablesCountLable(mTables.Count);
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
		public void Initialise(string apiAddress, string vendor, string secretKey, string locationToken)
		{
<<<<<<< 7e61b0f8c2f16f00fdc0c72256076e067ea307fb
            mConfigManager.Initialise(apiAddress, vendor, secretKey, locationToken);

            mManager.Initialize(true);

			// refresh the order list in memory
			//mOrders = mManager.GetOrders().ToList<Order>();
			//mOrders.AddRange(mManager.GetUnlinkedOrders());
=======
			_mController.Initialize(SampleDotNetPOSPresenter.AuthToken, vendor, secretKey, apiAddress, true, 0);

			// refresh the order list in memory
			mOrders = _mController.GetOrders().ToList<Order>();
			mOrders.AddRange(_mController.GetUnlinkedOrders());
>>>>>>> testing is going well

			// retrieve any payment transactions for current orders
			mPayments.Clear();
			foreach (var order in mOrders)
			{
				mPayments.AddRange(_mController.GetTransactionFromDoshiiOrderId(order.DoshiiId));
			}
            // retrieve any bookings.
            mBookings = mManager.GetBookings(DateTime.Today, DateTime.Today.AddDays(2));
            mTables = mManager.GetTables();

            // update the view labels for count of orders and payments
            mView.UpdateOrderCountLabel(mOrders.Count);
			mView.UpdatePaymentCountLabel(mPayments.Count);
            mView.UpdateBookingsCountLabel(mBookings.Count);
            mView.UpdateTablesCountLable(mTables.Count);
        }

        #endregion

        #region Ordering

        /// <summary>
        /// Opens a form that displays the current order list.
        /// </summary>
        public void DisplayOrderList()
		{
			using (var view = new SampleOrderListForm())
			{
				view.AttachPresenter(this);

				try
				{
					foreach (var order in mOrders)
					{
						view.AddOrder(order);
					}

					view.ShowDialog();
				}
				finally
				{
					view.RemovePresenter();
				}
			}
		}

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
				order = _mController.UpdateOrder(order);
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
			{
				mOrders.Add(order);
				mView.UpdateOrderCountLabel(mOrders.Count);
			}
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
			{
				mOrders.Remove(order);
				mView.UpdateOrderCountLabel(mOrders.Count);
			}
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
		/// <returns>The payment with the supplied <paramref name="transactionId"/> if found; or <c>null</c> otherwise.</returns>
		public Transaction RetrieveTransaction(string transactionId)
		{
			return mPayments.FirstOrDefault(o => o.Id == transactionId);
		}

		/// <summary>
		/// Retrieves the current list of transactions for the order with the supplied <paramref name="orderId"/>.
		/// </summary>
		/// <param name="orderId">The Id of the order being queried.</param>
		/// <returns>The list of payments for the order with the supplied <paramref name="orderId"/>.</returns>
		public IEnumerable<Transaction> RetrieveOrderTransactions(string orderId)
		{
			return mPayments.Where(o => o.OrderId == orderId);
		}

		/// <summary>
		/// Removes a transaction from the list.
		/// </summary>
		/// <param name="transactionId">The Id of the payment to be removed.</param>
		public void RemoveTransaction(string transactionId)
		{
			var payment = RetrieveTransaction(transactionId);
			if (payment != null)
			{
				mPayments.Remove(payment);
				mView.UpdatePaymentCountLabel(mPayments.Count);
			}
		}

		/// <summary>
		/// Adds the payment if it doesn't already exist, or updates it otherwise.
		/// </summary>
		/// <param name="transaction">Details of the payment.</param>
		public void AddOrUpdateTransaction(Transaction transaction)
		{
			int index = mPayments.IndexOf(transaction);
			if (index < 0)
			{
				mPayments.Add(transaction);
				mView.UpdatePaymentCountLabel(mPayments.Count);
			}
			else
				mPayments[index] = transaction;
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

        #region Bookings

        public Booking GetBooking(string id)
        {
            return mManager.GetBooking(id);
        }


        public void DisplayBookingsList()
        {
            using (var view = new SampleBookingListForm())
            {
                view.AttachPresenter(this);
                try
                {
                    foreach (var booking in mBookings)
                        view.AddBooking(booking);
                    view.ShowDialog();
                }
                finally
                {
                    view.RemovePresenter();
                }
            }
        }


        internal void DisplayTableList()
        {
            using (var view = new SampleTableListForm())
            {
                view.AttachPresenter(this);
                try
                {
                    foreach (var table in mTables)
                        view.AddTable(table);
                    view.ShowDialog();
                }
                finally
                {
                    view.RemovePresenter();
                }
            }
        }

        /// <summary>
        /// Sends a POS-generated update of an table to the Doshii Manager.
        /// </summary>
        /// <returns>The updated details for the table.</returns>
        public Table SendTable()
        {
            var table = GenerateTable();

            if (table != null)
            {
                table = mManager.CreateTable(table);
                AddOrUpdateTable(table);
            }

            return table;
        }

        public Table UpdateTable(Table table)
        {
            table = EditTable(table);
            if (table != null)
            {
                table = mManager.UpdateTable(table);
                AddOrUpdateTable(table);
            }
            return table;
        }

        public Checkin SendBookingCheckin(Booking booking)
        {
            var checkin = GenerateBookingCheckin(booking);
            if (checkin != null)
            {
                if (mManager.SeatBooking(booking.Id, checkin))
                    return checkin;
            }
            return null;
        }

        /// <summary>
        /// Adds the table if it doesn't already exist, otherwise updates it.
        /// </summary>
        /// <param name="table">The table to be added or updated.</param>
        private void AddOrUpdateTable(Table table)
        {
            int index = mTables.IndexOf(table);
            if (index < 0)
            {
                mTables.Add(table);
                mView.UpdateTablesCountLable(mTables.Count);
            }
            else
                mTables[index] = table;
        }

        private Table GenerateTable()
        {
            using (var view = new SampleTableForm())
            {
                view.AttachPresenter(this);
                try
                {
                    if (view.ShowDialog() == DialogResult.OK)
                    {
                        return view.ReadForm();
                    }
                }
                finally
                {
                    view.RemovePresenter();
                }
            }
            return null;
        }

        private Table EditTable(Table table)
        {
            using (var view = new SampleTableForm())
            {
                view.AttachPresenter(this);
                try
                {
                    view.UpdateView(table);
                    if (view.ShowDialog() == DialogResult.OK)
                    {
                        return view.ReadForm();
                    }
                }
                finally
                {
                    view.RemovePresenter();
                }
            }
            return null;
        }


        private Checkin GenerateBookingCheckin(Booking booking)
        {
            using (var view = new SampleArriveForm())
            {
                view.AttachPresenter(this);
                view.UpdateView(booking);
                if (view.ShowDialog() == DialogResult.OK)
                {
                    return view.ReadForm();
                }
            }
            return null;
        }

        internal Booking RetrieveBooking(string bookingId)
        {
            return mBookings.FirstOrDefault(b => b.Id == bookingId);
        }

        internal void UpdateBooking(Booking booking)
        {
            int index = mBookings.FindIndex(b => b.Id == booking.Id);
            if (index >= 0)
                mBookings[index] = booking;
            else
            {
                mBookings.Add(booking);
                mView.UpdateBookingsCountLabel(mBookings.Count);
            }
        }

        internal void DeleteBooking(string id)
        {
            int index = mBookings.FindIndex(b => b.Id == id);
            if (index >= 0)
            {
                mBookings.RemoveAt(index);
                mView.UpdateBookingsCountLabel(mBookings.Count);
            }
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

			if (_mController != null)
			{
				_mController.Dispose();
				_mController = null;
			}

			if (mOrderingManager != null)
			{
				mOrderingManager.Dispose();
				mOrderingManager = null;
			}
            
            if (mReservationManager != null)
            {
                mReservationManager.Dispose();
                mReservationManager = null;
            }
		}

		#endregion
	}
}
