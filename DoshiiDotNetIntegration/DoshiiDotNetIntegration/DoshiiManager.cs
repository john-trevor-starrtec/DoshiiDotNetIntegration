using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace DoshiiDotNetIntegration
{
    /// <summary>
    /// This class manages network operations (requests and responses) between Doshii and the Point of sale (POS) software.
    /// This class supports ordering and product operations including the following;
    /// <list type="bullet">
    ///   <item>Creating orders</item>
    ///   <item>Modifying existing orders</item>
    ///   <item>Setting the status of a consumer to a “checked-in” status</item>
    ///   <item>Creating products</item>re
    ///   <item>Modifying existing products</item>
    ///   <item>Deleting the products</item>
    /// </list>
    /// The DoshiiManager class must be instantiated by passing in an implementation of <see cref="IDoshiiOrdering"/>.
    /// To update orders on Doshii use the following methods;
    /// <see cref="UpdateOrder"/>, 
    /// <see cref="SetTableAllocation"/>, 
    /// <see cref="GetCheckedInConsumersFromDoshii"/>, 
    /// <see cref="GetOrder"/>.
    /// To keep the products on the Doshii products list up to date with the products available on the pos the following 5 methods should be used. 
    /// <see cref="AddNewProducts"/>, 
    /// <see cref="UpdateProcuct"/>, 
    /// <see cref="DeleteProducts"/>, 
    /// <see cref="DeleteAllProducts"/>, 
    /// <see cref="GetAllProducts"/>.  
    /// NOTE: Anytime an order is received through the DoshiiDotNetSDK the order.UpdatedAt string should be recorded by the POS and used when updating the order from Doshii. 
    /// </summary>
    /// <remarks>
    /// The DoshiiManager supports two communication protocols HTTP and Websockets. 
    /// The websockets protocol is used to open a websocket connection with the DoshiiAPI and once it is open, 
    /// the DoshiiManager receives the notification events messages from DoshiiAPI. Events include when a user 
    /// changes an order event e.t.c. The HTTP protocol is used for all other operations including creating orders, 
    /// update orders, creating products e.t.c.)
    /// </remarks>
    public class DoshiiManager : IDisposable
	{
		#region Constants

		/// <summary>
		/// Default timeout (in seconds) for the connection to the Doshii API -- 30.
		/// </summary>
		internal const int DefaultTimeout = 30;

		#endregion

		#region properties, constructors, Initialize, versionCheck

		/// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiWebSocketsCommunication class for interacting with the Doshii webSocket connection
        /// </summary>
        private DoshiiWebSocketsCommunication m_SocketComs = null;

        internal DoshiiWebSocketsCommunication SocketComs
        {
            get { return m_SocketComs; }
            set
            {
                if (m_SocketComs != null)
                {
                    UnsubscribeFromSocketEvents();
                }
                m_SocketComs = value;
                if (m_SocketComs != null)
                {
                    SubscribeToSocketEvents();
                    m_SocketComs.Initialize();
                }
            }
        }

        /// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiHttpCommunication class for interacting with the Doshii HTTP restful API
        /// </summary>
        internal DoshiiHttpCommunication m_HttpComs = null;

		/// <summary>
		/// The payment module manager is a core module manager for the Doshii platform.
		/// </summary>
		private IPaymentModuleManager mPaymentManager;

        /// <summary>
        /// The basic ordering manager is a core module manager for the Doshii platform.
        /// </summary>
        private IOrderingManager mOrderingManager;

		/// <summary>
		/// The logging manager for the Doshii SDK.
		/// </summary>
		internal DoshiiLogManager mLog;

        /// <summary>
        /// The unique token for the venue -- this can be retrieved from the Doshii website.
        /// </summary>
        internal string AuthorizeToken { get; set; }

        /// <summary>
        /// Gets the current Doshii version information.
        /// This method is automatically called and the results logged when this class in instantiated. 
        /// </summary>
        /// <returns></returns>
        protected static string CurrentVersion()
        {
            var versionStringBuilder = new StringBuilder();
            versionStringBuilder.Append("Doshii Integration Version: ");
            versionStringBuilder.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            versionStringBuilder.Append(Environment.NewLine);
            
            return versionStringBuilder.ToString();
        }

        /// <summary>
        /// Constructor.
        /// After the constructor is called it MUST be followed by a call to <see cref="Initialize"/> to start communication with the Doshii API
        /// </summary>
		/// <param name="paymentManager">The Transaction API callback mechanism.</param>
		/// <param name="logger">The logging mechanism callback to the POS.</param>
        public DoshiiManager(IPaymentModuleManager paymentManager, IDoshiiLogger logger, IOrderingManager orderingManager)
        {
			if (paymentManager == null)
				throw new ArgumentNullException("paymentManager", "IPaymentModuleManager needs to be instantiated as it is a core module");
            if (orderingManager == null)
                throw new ArgumentNullException("orderingManager", "IOrderingManager needs to be instantiated as it is a core module");
			mPaymentManager = paymentManager;
            mOrderingManager = orderingManager;
            mLog = new DoshiiLogManager(logger);
			AutoMapperConfigurator.Configure();
        }
        
        /// This method MUST be called immediately after this class is instantiated to initialize communication with doshii.
        /// Initializes the WebSockets communications with Doshii,
        /// Initializes the HTTP communications with Doshii,
        /// </summary>
        /// <param name="socketUrl">
        /// The socket URL for communication with doshii 
        /// The socket URL should not end in a '/'
        /// The socket URL must start with either 'ws' or 'wss' - The current Doshii integration uses 'wss'
        /// </param>
        /// <param name="token">
        /// The unique venue authentication token - This can be retrieved from the Doshii web site
        /// </param>
        /// <param name="urlBase">
        /// The base URL for communication with the Doshii restful API 
        /// The address should not end in a '/'
        /// Doshii currently uses HTTPS
        /// </param>
        /// <param name="startWebSocketConnection">
        /// Should this instance of the class start the webSocket connection with doshii
        /// There should only be one webSockets connection to Doshii per venue
        /// The webSocket connection is only necessary for the ordering functionality of the Doshii integration and is not necessary for updating the Doshii menu. 
        /// </param>
        /// <param name="timeOutValueSecs">
        /// This is the amount of this the web sockets connection can be down before the integration assumes the connection has been lost. 
        /// If this timeout value is reached the DohsiiManagement will call a method on <see cref="IDoshiiOrdering"/> that should disassociate all current doshii tabs and checkout all current doshii consumers, This
        /// will allow the tabs / orders / checks to be acted on in the pos without messages being sent to doshii to update doshii. After the disassociate occurs the user will no longer be able to access their tab / order on the Doshii app and this value is passed to the Doshii API upon communication initializations so doshii will close tabs when there has been no communication for this period of time. 
        /// NOTE: This differs from the time that is set on the Doshii back end that indicates how long a tab can be inactive for before a checkout message is sent to the pos indicating that the consumer no longer has a valie Doshii tab / order and any associated tab / order / person registered on the pos should be disassociated from Doshii.  
        /// </param>
        public virtual void Initialize(string token, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: Version {2} with; {3}token {0}, {3}BaseUrl: {1}", token, urlBase, CurrentVersion(), Environment.NewLine));
			
            if (string.IsNullOrWhiteSpace(urlBase))
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: Initialization failed - required urlBase");
				throw new ArgumentException("empty urlBase");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: Initialization failed - required token");
				throw new ArgumentException("empty token");
            }

			int timeout = timeOutValueSecs;

			if (timeout < 0)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: Initialization failed - timeoutvaluesecs must be minimum 0");
                throw new ArgumentException("timeoutvaluesecs < 0");
            }
			else if (timeout == 0)
			{
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, String.Format("Doshii will use default timeout of {0}", DoshiiManager.DefaultTimeout));
				timeout = DoshiiManager.DefaultTimeout;
			}

			AuthorizeToken = token;
			string socketUrl = BuildSocketUrl(urlBase, token);
			InitializeProcess(socketUrl, urlBase, startWebSocketConnection, timeout);
        }

		/// <summary>
		/// DO NOT USE, this method is for internal use only.
		/// Builds the socket URL from the supplied <paramref name="baseApiUrl"/>, including appending the supplied <paramref name="token"/> as a <c>GET</c> parameter.
		/// </summary>
		/// <param name="baseApiUrl">The base URL for the API. This is an HTTP address that points to the Doshii POS API, including version.</param>
		/// <param name="token">The Doshii authentication token for the POS implementation in the API.</param>
		/// <returns>The URL for the web socket connection in Doshii.</returns>
		internal virtual string BuildSocketUrl(string baseApiUrl, string token)
		{
			// baseApiUrl is for example https://sandbox.doshii.co/pos/api/v2
			// require socket url of wss://sandbox.doshii.co/pos/socket?token={token} in this example
			// so first, replace http with ws (this handles situation where using http/ws instead of https/wss
			string result = baseApiUrl.Replace("http", "ws");

			// next remove the /api/v2 section of the url
			int index = result.IndexOf("/api");
			if (index > 0 && index < result.Length)
			{
				result = result.Remove(index);
			}

			// finally append the socket endpoint and token parameter to the url and return the result
			result = String.Format("{0}/socket?token={1}", result, token);

			return result;
		}

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// </summary>
        /// <param name="socketUrl"></param>
        /// <param name="UrlBase"></param>
        /// <param name="StartWebSocketConnection"></param>
		/// <param name="timeOutValueSecs"></param>
        /// <returns></returns>
        internal virtual bool InitializeProcess(string socketUrl, string UrlBase, bool StartWebSocketConnection, int timeOutValueSecs)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: Initializing Doshii");

            bool result = true;

            m_HttpComs = new DoshiiHttpCommunication(UrlBase, AuthorizeToken, mLog, this);

            if (StartWebSocketConnection)
            {
                try
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format(string.Format("socketUrl = {0}, timeOutValueSecs = {1}", socketUrl, timeOutValueSecs)));
                    m_SocketComs = new DoshiiWebSocketsCommunication(socketUrl, timeOutValueSecs, mLog, this);
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format(string.Format("socket Comms are set")));
                    
                    SubscribeToSocketEvents();
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format(string.Format("socket events are subscribed to")));
                    
                    m_SocketComs.Initialize();
                }
                catch (Exception ex)
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Initializing Doshii failed, there was an exception that was {0}", ex.ToString()));
                }
            }

            return result;
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Subscribes to the socket communication events 
        /// </summary>
        internal virtual void SubscribeToSocketEvents()
        {
            if (m_SocketComs == null)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: The socketComs has not been initialized");
                throw new NotSupportedException("m_SocketComms is null");
            }
            else
            {
                UnsubscribeFromSocketEvents();
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: Subscribing to socket events");
                m_SocketComs.OrderCreatedEvent += new DoshiiWebSocketsCommunication.OrderCreatedEventHandler(SocketComsOrderCreatedEventHandler);
                m_SocketComs.TransactionCreatedEvent += new DoshiiWebSocketsCommunication.TransactionCreatedEventHandler(SocketComsTransactionCreatedEventHandler);
                m_SocketComs.TransactionUpdatedEvent += new DoshiiWebSocketsCommunication.TransactionUpdatedEventHandler(SocketComsTransactionUpdatedEventHandler);
				m_SocketComs.SocketCommunicationEstablishedEvent += new DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
                m_SocketComs.SocketCommunicationTimeoutReached += new DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(SocketComsTimeOutValueReached);
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Unsubscribes to the socket communication events 
        /// </summary>
        internal virtual void UnsubscribeFromSocketEvents()
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: Unsubscribing from socket events");
            m_SocketComs.OrderCreatedEvent -= new DoshiiWebSocketsCommunication.OrderCreatedEventHandler(SocketComsOrderCreatedEventHandler);
            m_SocketComs.TransactionCreatedEvent -= new DoshiiWebSocketsCommunication.TransactionCreatedEventHandler(SocketComsTransactionCreatedEventHandler);
            m_SocketComs.TransactionUpdatedEvent -= new DoshiiWebSocketsCommunication.TransactionUpdatedEventHandler(SocketComsTransactionUpdatedEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent -= new DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
            m_SocketComs.SocketCommunicationTimeoutReached -= new DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(SocketComsTimeOutValueReached);
        }
        #endregion

        #region socket communication event handlers

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a socket communication established event and calls <see cref="RefreshAllOrders()"/>. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        internal virtual void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            //RefreshAllOrders();
        }

        /// <summary>
        /// checks all orders on the doshii system, if there are any pending orders deals with them as if it received a socket message alerting to a pending order. 
        /// currently this method does not check the transactions as there should be no unlinked transactions for already created orders, order ahead only allows for 
        /// partners to make payments when they create an order else the payment is expected to be made by the customer on receipt of the order. 
        /// </summary>
        internal void RefreshAllOrders()
        {
            //check unassigned orders
            IEnumerable<Order> unassignedOrderList;
            try
            {
                unassignedOrderList = GetUnlinkedOrders();
                foreach (var order in unassignedOrderList)
                {
                    List<Transaction> transactionListForOrder = GetTransactionFromDoshiiOrderId(order.DoshiiId).ToList();
                    HandleOrderCreated(order, transactionListForOrder.ToList());
                }
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        
        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a socket communication timeOut event - this is when there has not been successful communication with doshii within the specified timeout period. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTimeOutValueReached(object sender, EventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, 
				"Doshii: SocketComsTimeoutValueReached");
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_OrderStatusEvent, 
        /// Records the Order.UpdatedAt value and calls the appropriate method on the OrderingInterface to act on the check. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsOrderCreatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
			if (!String.IsNullOrEmpty(e.Order.Id))
            {
                mLog.LogMessage(this.GetType(),DoshiiLogLevels.Fatal, "A preexisting order was passed to the order created event handler.");
                throw new NotSupportedException("Developer Error, An order with a posId was passed to the CreatedOrderEventHandler");
            }
            HandleOrderCreated(e.Order, e.TransactionList.ToList());

        }

        internal virtual void HandleOrderCreated(Order order, List<Transaction> transactionList)
        {
            if (transactionList == null)
            {
                transactionList = new List<Transaction>();
            }
            Order orderReturnedFromPos = null;
            if (transactionList.Count > 0)
            {
                if (order.Type == "delivery")
                {
                    Consumer consumer = GetConsumerForOrderCreated(order, transactionList);
                    if (consumer == null)
                    {
                        return;
                    }
                    orderReturnedFromPos = mOrderingManager.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList);
                }
                else if (order.Type == "pickup")
                {
                    Consumer consumer = GetConsumerForOrderCreated(order, transactionList);
                    if (consumer == null)
                    {
                        return;
                    }
                    orderReturnedFromPos = mOrderingManager.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList);
                }
                else
                {
                    RejectOrderFromOrderCreateMessage(order, transactionList);
                }
                
            }
            else
            {
                if (order.Type == "delivery")
                {
                    Consumer consumer = GetConsumerForOrderCreated(order, transactionList);
                    if (consumer == null)
                    {
                        return;
                    }
                    orderReturnedFromPos = mOrderingManager.ConfirmNewDeliveryOrder(order, consumer);
                }
                else if (order.Type == "pickup")
                {
                    Consumer consumer = GetConsumerForOrderCreated(order, transactionList);
                    if (consumer == null)
                    {
                        return;
                    }
                    orderReturnedFromPos = mOrderingManager.ConfirmNewPickupOrder(order, consumer);
                }
                else
                {
                    RejectOrderFromOrderCreateMessage(order, transactionList);
                }
                
            }
            if (orderReturnedFromPos == null)
            {
                RejectOrderFromOrderCreateMessage(order, transactionList);
            }
            else
            {
                //set order status to accepted post to doshii
                orderReturnedFromPos.Status = "accepted";
                try
                {
                    UpdateOrder(orderReturnedFromPos);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                    //if we get an error response at this point we should prob cancel the order on the pos and not continue and cancel the payments. 
                }
                //If there are transactions set to waiting and get response - should call request payment
                foreach (Transaction tran in transactionList)
                {
                    RecordTransactionVersion(tran);
                    tran.Status = "waiting";
                    try
                    {
                        RequestPaymentForOrderExistingTransaction(tran);
                    }
                    catch (Exception ex)
                    {
                        //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                    }
                }
            }
        }


        internal virtual Consumer GetConsumerForOrderCreated(Order order, List<Transaction> transactionList)
        {
            try
            {
                return GetConsumerFromCheckinId(order.CheckinId);
            }
            catch (Exception ex)
            {
                mLog.LogMessage(this.GetType(), DoshiiLogLevels.Error, string.Format("There was an exception when retreiving the consumer for a pending order doshiiOrderId - {0}. The order will be rejected", order.Id), ex);
                RejectOrderFromOrderCreateMessage(order, transactionList);
                return null;
            }
        }

        internal virtual void RejectOrderFromOrderCreateMessage(Order order, List<Transaction> transactionList)
        {
            //set order status to rejected post to doshii
            order.Status = "rejected";
            try
            {
                UpdateOrder(order);
            }
            catch (Exception ex)
            {
                //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
            }
            foreach (Transaction tran in transactionList)
            {
                tran.Status = "rejected";
                try
                {
                    RejectPaymentForOrder(tran);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                }
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_TransactionCreatedEvent, 
        /// Calls the appropriate method on the PaymentInterface to act on the transaction depending on the transaction status. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTransactionCreatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TransactionEventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: received a transaction status event with status '{0}', for transaction Id '{1}', for order Id '{2}'", e.Transaction.Status, e.TransactionId, e.Transaction.OrderId));
            Transaction transactionFromPos = null;
			switch (e.Transaction.Status)
            {
                case "pending":
                    HandelPendingTransactionReceived(e.Transaction);
                    break;
                default:
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a create transaction message was received for a transaction which state was not pending, Transaction status - '{0}'", e.Transaction.Status)); 
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API",e.Transaction.Status));
            }
		}

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_TransactionCreatedEvent, 
        /// Calls the appropriate method on the PaymentInterface to act on the transaction depending on the transaction status. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTransactionUpdatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TransactionEventArgs e)
        {
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: received a transaction status event with status '{0}', for transaction Id '{1}', for order Id '{2}'", e.Transaction.Status, e.TransactionId, e.Transaction.OrderId));
            
            switch (e.Transaction.Status)
            {
                case "pending":
                    HandelPendingTransactionReceived(e.Transaction);
                    break;
                case "cancelled":
                    mPaymentManager.RecordTransactionVersion(e.Transaction.Id, e.Transaction.Version);
                    mPaymentManager.CancelPayment(e.Transaction);
                    break;
                case "complete":
                    mPaymentManager.RecordTransactionVersion(e.Transaction.Id, e.Transaction.Version);
                    mPaymentManager.RecordSuccessfulPayment(e.Transaction);
                    break;
                default:
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a create transaction message was received for a transaction which state was not pending, Transaction status - '{0}'", e.Transaction.Status));
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API", e.Transaction.Status));
            }
        }

        internal void HandelPendingTransactionReceived(Transaction receivedTransaction)
        {
            Transaction transactionFromPos = null;
            try
            {
                transactionFromPos = mPaymentManager.ReadyToPay(receivedTransaction);
            }
            catch (OrderDoesNotExistOnPosException)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: A transaction was initiated on the Doshii API for an order that does not exist on the system, orderid {0}", receivedTransaction.OrderId));
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
                return;
            }

            if (transactionFromPos != null)
            {
                mPaymentManager.RecordTransactionVersion(receivedTransaction.Id, receivedTransaction.Version);
                RequestPaymentForOrderExistingTransaction(transactionFromPos);
            }
            else
            {
                receivedTransaction.Status = "rejected";
                RejectPaymentForOrder(receivedTransaction);
            }
        }
        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// This method requests a payment from Doshii
        /// calls <see cref="m_DoshiiInterface.CheckOutConsumerWithCheckInId"/> when order update was reject by doshii for a reason that means it should not be retired. 
        /// calls <see cref="m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordPartialCheckPayment(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordFullCheckPayment(ref order)"/>
        /// to record the payment in the pos. 
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="order">
        /// The order that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual bool RequestPaymentForOrderExistingTransaction(Transaction transaction)
        {
            var returnedTransaction = new Transaction();
            transaction.Status = "waiting";

            try
            {
                //as the transaction cannot currently be changed on doshii and transacitons are only created when a payment is made with an order the line below is not necessary unitll
                //doshii is enhanced to allow modifying of transactions. 
                //transaction.Version = mPaymentManager.RetrieveTransactionVersion(transaction.Id);
                returnedTransaction = m_HttpComs.PutTransaction(transaction);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not locate the transaction for order.Id{0}", transaction.OrderId), rex);
                }
				else if (rex.StatusCode == HttpStatusCode.PaymentRequired)
				{
				    // this just means that the partner failed to claim payment when requested
				    mLog.LogMessage(typeof (DoshiiManager), DoshiiLogLevels.Error,
				        string.Format("Doshii: The partner could not claim the payment for for order.Id{0}", transaction.OrderId), rex);
				}
				else
				{
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error,
                        string.Format("Doshii: There was an unknown exception while attempting to get a payment from doshii"), rex);
                }
                mPaymentManager.CancelPayment(transaction);
                return false;
            }
            catch (NullOrderReturnedException)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a postTransaction for order.Id{0}", transaction.OrderId));
                mPaymentManager.CancelPayment(transaction);
                return false;
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a postTransaction for order.Id {0} : {1}", transaction.OrderId, ex));
                mPaymentManager.CancelPayment(transaction);
                return false;
            }

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id && returnedTransaction.Status == "complete")
            {
				var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: transaction post for payment - '{0}'", jsonTransaction.ToJsonString()));

                mPaymentManager.RecordSuccessfulPayment(returnedTransaction);
                mPaymentManager.RecordTransactionVersion(returnedTransaction.Id, returnedTransaction.Version);
                return true;
            }
            else
            {
                mPaymentManager.CancelPayment(transaction);
                return false;
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// This method requests a payment from Doshii
        /// calls <see cref="m_DoshiiInterface.CheckOutConsumerWithCheckInId"/> when order update was reject by doshii for a reason that means it should not be retired. 
        /// calls <see cref="m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordPartialCheckPayment(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordFullCheckPayment(ref order)"/>
        /// to record the payment in the pos. 
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="order">
        /// The order that should be paid
        /// </param>
        /// <returns>
        /// True on successful payment; false otherwise.
        /// </returns>
        internal virtual bool RejectPaymentForOrder(Transaction transaction)
        {
            var returnedTransaction = new Transaction();
            transaction.Status = "rejected";

            try
            {
                returnedTransaction = m_HttpComs.PutTransaction(transaction);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not locate the transaction for transaction.Id{0}", transaction.OrderId));
                return false;
                
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putTransaction for transaction.Id {0} : {1}", transaction.OrderId, ex));
                return false;
            }

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id && returnedTransaction.Status == "complete")
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: transaction put for payment - '{0}'", jsonTransaction.ToJsonString()));
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ordering And Transaction

        internal void RecordOrderVersion(string posOrderId, string version)
        {
            try
            {
                mOrderingManager.RecordOrderVersion(posOrderId, version);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, string.Format("Doshii: Attempted to update an order version that does not exist on the Pos, OrderId - {0}, version - {1}", posOrderId, version));
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update an order version on the pos, OrderId - {0}, version - {1}, {2}", posOrderId, version, ex.ToString()));
            }
        }

        internal virtual void RecordTransactionVersion(Transaction transaction)
        {
            try
            {
                mPaymentManager.RecordTransactionVersion(transaction.Id, transaction.Version);
            }
            catch (TransactionDoesNotExistOnPosException nex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, string.Format("Doshii: Attempted to update a transaction version for a transaction that does not exist on the Pos, TransactionId - {0}, version - {1}", transaction.Id, transaction.Version));
                mPaymentManager.CancelPayment(transaction);
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update a transaction version on the pos, TransactionId - {0}, version - {1}, {2}", transaction.Id, transaction.Version, ex.ToString()));
                mPaymentManager.CancelPayment(transaction);
            }
        }

        /// <summary>
        /// attempts to add a pos transaction to doshii
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>
        /// the transaction that was recorded on doshii if the request was successful
        /// returns null if the request failed. 
        /// </returns>
        public Transaction RecordPosTransactionOnDoshii(Transaction transaction)
        {
            Transaction returnedTransaction;
            try
            {
                returnedTransaction = m_HttpComs.PostTransaction(transaction);
            }
            catch (Exception ex)
            {
                return null;
            }
            return returnedTransaction;
        }


        /// <summary>
        /// This method returns an order from Doshii corresponding to the OrderId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns></returns>
		public virtual Order GetOrder(string orderId)
		{
			try
			{
				return m_HttpComs.GetOrder(orderId);
			}
			catch (Exceptions.RestfulApiErrorResponseException rex)
			{
				throw rex;
			}
		}

        /// <summary>
        /// This method returns a consumer from Doshii corresponding to the ConsumerId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns></returns>
        public virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            try
            {
                return m_HttpComs.GetConsumerFromCheckinId(checkinId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns an order from Doshii corresponding to the OrderId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns></returns>
        public virtual Order GetOrderFromDoshiiOrderId(string orderId)
        {
            try
            {
                return m_HttpComs.GetOrder(orderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

		/// <summary>
		/// Retrieves the current order list from Doshii.
		/// </summary>
		/// <returns>The current list of orders available in Doshii.</returns>
		public virtual IEnumerable<Order> GetOrders()
		{
			try
			{
				return m_HttpComs.GetOrders();
			}
			catch (Exceptions.RestfulApiErrorResponseException rex)
			{
				throw rex;
			}
		}

        /// <summary>
        /// Retrieves the current unlinked order list from Doshii.
        /// </summary>
        /// <returns>The current list of orders available in Doshii.</returns>
        public virtual IEnumerable<Order> GetUnlinkedOrders()
        {
            try
            {
                return m_HttpComs.GetUnlinkedOrders();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns an transaction from Doshii corresponding to the transactionId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns></returns>
        public virtual Transaction GetTransaction(string transactionId)
        {
            try
            {
                return m_HttpComs.GetTransaction(transactionId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns an transaction from Doshii corresponding to the transactionId
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns></returns>
        public virtual IEnumerable<Transaction> GetTransactionFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return m_HttpComs.GetTransactionsFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

		/// <summary>
		/// Retrieves the list of payments from Doshii.
		/// </summary>
		/// <returns>The current list of Doshii payments.</returns>
		public virtual IEnumerable<Transaction> GetTransactions()
		{
			try
			{
				return m_HttpComs.GetTransactions();
			}
			catch (Exceptions.RestfulApiErrorResponseException rex)
			{
				throw rex;
			}
		}

        /// <summary>
        /// This method will update the Order on the Doshii API
        /// </summary>
        /// <param name="order">
        /// The order must contain all the products / items,
        /// All surcharges,
        /// All discounts,
        /// included in the check as this method will overwrite the order currently on Doshii 
        /// </param>
        /// <exception cref="ConflictWithOrderUpdateException">
        /// Indicates that there was a conflict between the order provided by the pos and the order currently on Doshii, 
        /// Usually this indicates that the order.updatedAt string is not = to the string on doshii,
        /// If this occurs you should call <see cref="GetOrder"/> with the Id of this order ensure that it is accurate and that the pos has recorded any pending items on the order and resent any required update to Doshii. 
        /// </exception>
        /// <returns></returns>
        public virtual Order UpdateOrder(Order order)
        {
            order.Version = mOrderingManager.RetrieveOrderVersion(order.Id);
            var jsonOrder = Mapper.Map<JsonOrder>(order);
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", jsonOrder.ToJsonString()));
            
            var returnedOrder = new Order();
            
            try
            {
                returnedOrder = m_HttpComs.PutOrder(order);
                if (returnedOrder.Id == "0" && returnedOrder.DoshiiId == "0")
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.Conflict)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                    throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                }
                throw new OrderUpdateException("Update order not successful", rex);
            }
            catch (NullOrderReturnedException Nex)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
                throw new OrderUpdateException(string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id), Nex);
            }
            catch (Exception ex)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0}", order.Id), ex);
            }
            
            return returnedOrder;
        }

        #endregion

        #region tableAllocation and consumers

		/// <summary>
		/// Called by POS to add a table allocation to an order.
		/// </summary>
		/// <param name="posOrderId">The unique identifier of the order on the POS.</param>
		/// <param name="table">The table to add in Doshii.</param>
		/// <returns>The current order details in Doshii after upload.</returns>
		public bool AddTableAllocation(string posOrderId, string tableName)
		{
            TableAllocation table = new TableAllocation();
            table.Name = tableName;

            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos Allocating table '{0}' to order '{1}'", table.Name, posOrderId));

            Order order = null;
			try
			{
				order = mOrderingManager.RetrieveOrder(posOrderId);
			    order.Version = mOrderingManager.RetrieveOrderVersion(posOrderId);
			    order.Status = "accepted";
			}
			catch (OrderDoesNotExistOnPosException dne)
			{
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, "Doshii: Order does not exist on POS during table allocation");
			    throw dne;
			}
			
			if (order == null)
			{
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, "Doshii: NULL Order returned from POS during table allocation");
				throw new OrderDoesNotExistOnPosException("Doshii: The pos returned a null order during table allocation", new NullOrderReturnedException());
            }

			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: Order found, allocating table now", table.Name, posOrderId));

			var tableOrder = new TableOrder();
			tableOrder.Order = order; 
			tableOrder.Table = table; 

			try
			{
				Order returnedOrder = m_HttpComs.PutOrderWithTableAllocation(tableOrder);
			    if (returnedOrder != null)
			    {
			        RecordOrderCheckinId(returnedOrder);
			        return true;
			    }
			    else
			    {
			        return false;
			    }
			}
			catch (RestfulApiErrorResponseException rex)
			{
			    if (rex.StatusCode == HttpStatusCode.Conflict)
			    {
			        mLog.LogMessage(typeof (DoshiiManager), DoshiiLogLevels.Warning,
			            string.Format("There was a conflict updating order.id {0} during table allocaiton", order.Id.ToString()));
			        throw new ConflictWithOrderUpdateException(
			            string.Format("There was a conflict updating order.id {0} during table Allocation", order.Id.ToString()));
			    }
			    else
			    {
                    throw new OrderUpdateException("Update order with table allocaiton not successful", rex);
			    }
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a attempting a table allocaiton for order.Id{0}", order.Id), ex);
            }
		}

        internal virtual void RecordOrderCheckinId(Order order)
        {
            try
            {
                mOrderingManager.RecordCheckinForOrder(order.Id, order.CheckinId);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, string.Format("Doshii: Attempted to update a checkinId for an order that does not exist on the Pos, Order.id - {0}, checkinId - {1}", order.Id, order.CheckinId));
                //maybe we should call reject order here. 
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update a checkinId for an order on the pos, Order.Id - {0}, checkinId - {1}, {2}", order.Id, order.CheckinId, ex.ToString()));
                //maybe we should call reject order here. 
            }
        }

        /// <summary>
        /// Deletes a table Allocation from Doshii
        /// </summary>
        /// <param name="doshiiCustomerId">
        /// The customer whos table allocation will be deleted
        /// </param>
        /// <param name="tableName">
        /// The tableName of the allocation that will be deleted. 
        /// </param>
        /// <param name="deleteReason">
        /// The reason the allocation has been refused or rejected. 
        /// </param>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown when any of the following responses are received.
        /// <item> HttpStatusCode.BadRequest </item> 
        /// <item> HttpStatusCode.Unauthorized </item> 
        /// <item> HttpStatusCode.Forbidden </item>
        /// <item> HttpStatusCode.InternalServerError </item>
        /// <item> HttpStatusCode.NotFound </item> 
        /// <item> HttpStatusCode.Conflict </item>
        /// </exception>
        public virtual bool DeleteTableAllocation(string posOrderId)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos DeAllocating table for Order Id - '{0}'",posOrderId));
            try
            {
                string checkinId = mOrderingManager.RetrieveCheckinIdForOrder(posOrderId);
                return m_HttpComs.DeleteTableAllocation(checkinId);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.Conflict)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("There was a conflict updating order.id {0} deleting a table allocaiton", posOrderId));
                    throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0} deleting a table Allocation", posOrderId));
                }
                throw new OrderUpdateException("Update order with table allocaiton not successful", rex);
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting to delete a table allocation order.Id{0} : {1}", posOrderId, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a attempting to delete a table allocaiton for order.Id{0}", posOrderId), ex);
            }
        }

        #endregion

        #region products and menu

        /// <summary>
        /// this method is used to update the pos menu on doshii,
        /// calls to this method will replace the existing pos menu on doshii with the menu paramater. 
        /// If you wish to update or create single products you should use <see cref="PutProduct"/> method
        /// If you wish to update or create single order level surcharges you should use the <see cref="PutSurcount"/> method
        /// if you wish to delete a single product you should use the <see cref="DeleteProduct "/> method
        /// if you wish to delete a single order level surcharge you should use the <see cref="DeleteSurcount "/> method
         /// </summary>
        /// <param name="menu">
        /// The full pos menu to be updated to doshii
        /// </param>
        /// <returns>
        /// if successful the full menu will be returned. 
        /// if unsuccessful null will be returned. 
        /// </returns>
        public Menu PutMenu(Menu menu)
        {
            Menu returnedMenu = null;
            try
            {
               returnedMenu = m_HttpComs.PutMenu(menu);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedMenu != null)
            {
                return returnedMenu;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method is used to create or update a pos surcount on the doshii system. 
        /// </summary>
        /// <param name="surcount">
        /// the surcount to be created or updated
        /// </param>
        /// <returns>
        /// if successful the surcount as it exists on doshii will be returned
        /// if unsuccessful null will be returned. 
        /// </returns>
        public Surcount PutSurcount(Surcount surcount)
        {
            if (surcount.Id == null || string.IsNullOrEmpty(surcount.Id))
            {
                mLog.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Surcounts must have an Id to be created or updated on Doshii");
            }
            Surcount returnedSurcharge = null;
            try
            {
                returnedSurcharge = m_HttpComs.PutSurcount(surcount, surcount.Id);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedSurcharge != null)
            {
                return returnedSurcharge;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method is used to create or update a pos product on the doshii system. 
        /// </summary>
        /// <param name="product">
        /// the product to be created or updated
        /// </param>
        /// <returns>
        /// if successful the product as it exists on doshii will be returned
        /// if unsuccessful null will be returned. 
        /// </returns>
        public Product PutProduct(Product product)
        {
            if (product.PosId == null || string.IsNullOrEmpty(product.PosId))
            {
                mLog.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Products must have an Id to be created or updated on Doshii");
            }
            Product returnedProduct = null;
            try
            {
                returnedProduct = m_HttpComs.PutProduct(product, product.PosId);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedProduct != null)
            {
                return returnedProduct;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// this method is used to delete a pos surcount from doshii
        /// </summary>
        /// <param name="posId">
        /// the Id of the surcount to be deleted
        /// </param>
        /// <returns></returns>
        public bool DeleteSurcount(string posId)
        {
            bool success;
            try
            {
                success = m_HttpComs.DeleteSurcount(posId);
            }
            catch(Exception ex)
            {
                return false;
            }
            return success;
        }

        /// <summary>
        /// This method is used to delete a pos product from doshii
        /// </summary>
        /// <param name="posId">
        /// the Id of the product to be deleted. 
        /// </param>
        /// <returns></returns>
        public bool DeleteProduct(string posId)
        {
            bool success;
            try
            {
                success = m_HttpComs.DeleteProduct(posId);
            }
            catch (Exception ex)
            {
                return false;
            }
            return success;
        }


        #endregion

        #region IDisposable Members

        /// <summary>
		/// Cleanly disposes of the memory allocated to the instance's member variables.
		/// </summary>
		public void Dispose()
		{
			mPaymentManager = null;
			mLog.Dispose();
			mLog = null;
		}

		#endregion

		
	}
}
