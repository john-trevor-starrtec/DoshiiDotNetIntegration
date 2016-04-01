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
    /// To keep the Menu on the Doshii up to date with the products available on the pos the following 5 methods should be used. 
    /// <see cref="UpdateProduct"/>, 
    /// <see cref="DeleteProduct"/>, 
    /// <see cref="UpdateSurcount"/>, 
    /// <see cref="DeleteSurcount"/>, 
    /// <see cref="UpdateMenu"/>.  
    /// To use this SDK you must;
    /// instanciate the DoshiiManager,
    /// Call <see cref="Initialize"/> on the instance of the DoshiiManager
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
        /// A field indicating if initialize has been called on the doshii manager. 
        /// </summary>
        private bool m_IsInitalized = false;

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
        /// The unique token for the venue -- this can be retrieved from Doshii before enabling the integration.
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
        /// <param name="orderingManager">The Ordering API callback mechanism</param>
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

        /// <summary>
        /// This method MUST be called immediately after this class is instantiated to initialize communication with doshii.
        /// Initializes the WebSockets communications with Doshii,
        /// Initializes the HTTP communications with Doshii.
        /// If this method returns false the Doshii integration CANNOT be used until this method has been called successfully. 
        /// </summary>
        /// <param name="token">
        /// The unique venue authentication token - This can be retrieved from the Doshii before integration, this value is unique for each venue. 
        /// </param>
        /// <param name="urlBase">
        /// The base URL for communication with the Doshii restful API 
        /// The address should not end in a '/'
        /// an example of the format for this URL is 'https://sandbox.doshii.co/pos/api/v2'
        /// Doshii currently uses HTTPS
        /// </param>
        /// <param name="startWebSocketConnection">
        /// Should this instance of the class start the webSocket connection with doshii
        /// There should only be one webSockets connection to Doshii per venue
        /// The webSocket connection is only necessary for the ordering functionality of the Doshii integration and is not necessary for updating the Doshii menu. 
        /// </param>
        /// <param name="timeOutValueSecs">
        /// This is the amount of this the web sockets connection can be down before the integration assumes the connection has been lost. 
        /// </param>
        /// <returns>
        /// True if the initialize procedure was successful.
        /// False if the initialize procedure was unsuccessful.
        /// </returns>
        /// <exception cref="System.ArgumentException">An argument Exception will the thrown when there is an issue with one of the paramaters.</exception>
        public virtual bool Initialize(string token, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
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
            m_IsInitalized = InitializeProcess(socketUrl, urlBase, startWebSocketConnection, timeout);
            return m_IsInitalized;
        }

        /// <summary>
        /// Completes the Initialize process
        /// </summary>
        /// <param name="socketUrl">
        /// The Url for the socket connection
        /// </param>
        /// <param name="UrlBase">
        /// The base Url for the HTTP connection
        /// </param>
        /// <param name="StartWebSocketConnection">
        /// Indicates if this instance of the DoshiiManager should start a WebnSocket connection with Doshii
        /// </param>
        /// <param name="timeOutValueSecs">
        /// Indicates how long the Socket connection can be down before the SDK will assume the integration is no longer working. 
        /// </param>
        /// <returns>
        /// True if the initialize process was successful
        /// False if the initialize process failed. 
        /// </returns>
        internal virtual bool InitializeProcess(string socketUrl, string UrlBase, bool StartWebSocketConnection, int timeOutValueSecs)
        {
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: Initializing Doshii");

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
                    return false;
                }
            }

            return true;
        }

		/// <summary>
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
        /// Unsubscribes from the socket communication events 
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
        /// Handles a socket communication established event and calls <see cref="RefreshAllOrders()"/>. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        internal virtual void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            try
            {
                RefreshAllOrders();
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: There was an exception while trying to retrieve all the orders from Doshii.", ex);
            }
            
        }

        /// <summary>
        /// checks all orders on the doshii system, if there are any pending orders deals with them as if it received a socket message alerting to a pending order. 
        /// currently this method does not check the transactions as there should be no unlinked transactions for already created orders, order ahead only allows for 
        /// partners to make payments when they create an order else the payment is expected to be made by the customer on receipt of the order. 
        /// </summary>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown if there is an issue getting the orders from Doshii.</exception>
        internal void RefreshAllOrders()
        {
            
            try
            {
                //check unassigned orders
                IEnumerable<Order> unassignedOrderList;
                unassignedOrderList = GetUnlinkedOrders();
                foreach (Order order in unassignedOrderList)
                {
                    if (order.Status == "pending")
                    {
                        List<Transaction> transactionListForOrder = GetTransactionFromDoshiiOrderId(order.DoshiiId).ToList();
                        HandleOrderCreated(order, transactionListForOrder.ToList());
                    }
                }
                //Check assigned orders
                //This is not yet implemented as its not necessary when only OrderAhead is a possibility. 
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        
        /// <summary>
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

        /// <summary>
        /// This method calls the appropriate callback method on the <see cref="Interfaces.IOrderingManager"/> to confirm an order when an order created event is received from Doshii. 
        /// </summary>
        /// <param name="order">
        /// the order that has been created
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the new created order. 
        /// </param>
        internal virtual void HandleOrderCreated(Order order, List<Transaction> transactionList)
        {
            if (transactionList == null)
            {
                transactionList = new List<Transaction>();
            }
            Consumer consumer = GetConsumerForOrderCreated(order, transactionList);
            if (consumer == null)
            {
                return;
            }
            if (transactionList.Count > 0)
            {
                
                if (order.Type == "delivery")
                {
                    mOrderingManager.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList);
                }
                else if (order.Type == "pickup")
                {
                    mOrderingManager.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList);
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
                    
                    mOrderingManager.ConfirmNewDeliveryOrder(order, consumer);
                }
                else if (order.Type == "pickup")
                {
                    mOrderingManager.ConfirmNewPickupOrder(order, consumer);
                }
                else
                {
                    RejectOrderFromOrderCreateMessage(order, transactionList);
                }
                
            }
        }

        /// <summary>
        /// call this method to accept an order created by an order ahead partner, 
        /// this method will test that the order on doshii has not changed since it was original received by the pos. 
        /// It is the responsibility of the pos to ensure that the products on the order were not changed during the confirmation process as this will not 
        /// be checked by this method. 
        /// If this method is not successful then the order should not be commuted on the pos and <see cref="RejectOrderAheadCreation"/> should be called.
        /// </summary>
        /// <param name="orderToAccept">
        /// The order that is being accepted
        /// </param>
        /// <returns>
        /// True if the order was recorded as accepted on Doshii
        /// False if the order was not recorded as accepted on Doshii.
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public bool AcceptOrderAheadCreation(Order orderToAccept)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AcceptOrderAheadCreation"));
            }
            
            Order orderOnDoshii = GetOrderFromDoshiiOrderId(orderToAccept.DoshiiId);
            List<Transaction> transactionList = GetTransactionFromDoshiiOrderId(orderToAccept.DoshiiId).ToList();

            //test on doshii has changed. 
            if (orderOnDoshii.Version != orderToAccept.Version)
            {
                return false;
            }
            
            orderToAccept.Status = "accepted";
            try
            {
                ConfirmCreatedOrder(orderToAccept);
            }
            catch (Exception ex)
            {
                return false;
                //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                //if we get an error response at this point we should prob cancel the order on the pos and not continue and cancel the payments. 
            }
            //If there are transactions set to waiting and get response - should call request payment
            foreach (Transaction tran in transactionList)
            {
                RecordTransactionVersion(tran);
                tran.OrderId = orderToAccept.Id;
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
            return true;
        }

        /// <summary>
        /// call this method to reject an order created by an order ahead partner,
        /// </summary>
        /// <param name="orderToReject"></param>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public void RejectOrderAheadCreation(Order orderToReject)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RejectOrderAheadCreation"));
            }
            List<Transaction> transactionList = GetTransactionFromDoshiiOrderId(orderToReject.DoshiiId).ToList();
            //test order to accept is equal to the order on doshii
            RejectOrderFromOrderCreateMessage(orderToReject, transactionList);
        }

        /// <summary>
        /// Gets the consumer related to the order,
        /// If there is a problem getting the consumer from Doshii the order is rejected by the SDK
        /// </summary>
        /// <param name="order">
        /// The order the consumer is needed for
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the pending order
        /// </param>
        /// <returns>
        /// The consumer related to the order. 
        /// </returns>
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

        /// <summary>
        /// This method rejects an unlinked order on the Doshii API, the transactions related to the order will also be rejected. 
        /// </summary>
        /// <param name="order">
        /// The pending order to be rejected
        /// </param>
        /// <param name="transactionList">
        /// The transaction list to be rejected
        /// </param>
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
                    break;
                default:
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a create transaction message was received for a transaction which state was not pending, Transaction status - '{0}'", e.Transaction.Status));
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API", e.Transaction.Status));
            }
        }

        /// <summary>
        /// Handels the Pending transaction received event by calling the appropriate callback methods on the <see cref="Interfaces.IPaymentModuleManager"/> Interface. 
        /// </summary>
        /// <param name="receivedTransaction">
        /// The pending transaction that needs to be processed. 
        /// </param>
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

        /// <summary>
        /// calls the appropriate callback method on <see cref="Interfaces.IOrderingManager"/> to record the order version.
        /// </summary>
        /// <param name="posOrderId">
        /// the PosId of the order to be recorded
        /// </param>
        /// <param name="version">
        /// the version of the order to be recorded.
        /// </param>
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

        /// <summary>
        /// calls the appropriate callback method on <see cref="Interfaces.IPaymentModuleManager"/> to record the order version.
        /// </summary>
        /// <param name="transaction">
        /// the transaction to be recorded
        /// </param>
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
        /// <param name="transaction">
        /// The transaction to add to Doshii
        /// </param>
        /// <returns>
        /// the transaction that was recorded on doshii if the request was successful
        /// returns null if the request failed. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public Transaction RecordPosTransactionOnDoshii(Transaction transaction)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RecordPosTransactionOnDoshii"));
            }
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
        /// <returns>
        /// The order with the corresponding Id
        /// If there is no order corresponding to the Id, a blank order may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
		public virtual Order GetOrder(string orderId)
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetOrder"));
            }
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
        /// <returns>
        /// The consumer with the corresponding checkinId
        /// If there is no consumer corresponding to the checkinId, a blank consumer may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetConsumerFromCheckinId"));
            }
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
        /// <returns>
        /// The order with the corresponding Id
        /// If there is no order corresponding to the Id, a blank order may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual Order GetOrderFromDoshiiOrderId(string orderId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetOrderFromDoshiiOrderId"));
            }
            try
            {
                return m_HttpComs.GetOrderFromDoshiiOrderId(orderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

		/// <summary>
		/// Retrieves the current order list from Doshii.
		/// </summary>
		/// <returns>
		/// The current list of orders available in Doshii.
		/// If there are no orders a blank IEnumerable is returned. 
		/// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
		public virtual IEnumerable<Order> GetOrders()
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetOrders"));
            }
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
        /// <returns>The current list of orders available in Doshii.
        /// If there are no unlinkedOrders a blank IEnumerable is returned.
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual IEnumerable<Order> GetUnlinkedOrders()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetUnlinkedOrders"));
            }
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
        /// This method returns a transaction from Doshii corresponding to the transactionId
        /// </summary>
        /// <param name="transactionId">
        /// The Id of the transaction that is being requested. 
        /// </param>
        /// <returns>
        /// The transaction relating to the transacitonId
        /// If there is no transaction relating to the transacitonId a blank transaction will be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual Transaction GetTransaction(string transactionId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTransaction"));
            }
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
        /// This method returns a transaction from Doshii corresponding to the order with the doshiiOrderId
        /// This method should only be called in relation to unlinkedOrders as this method will not return transactions related to linked orders. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns>
        /// An IEnumerable of transactions that relate to the doshiiOrderId. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual IEnumerable<Transaction> GetTransactionFromDoshiiOrderId(string doshiiOrderId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTransactionFromDoshiiOrderId"));
            }
            try
            {
                return m_HttpComs.GetTransactionsFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                //this means there were no transactions for the unlinked order. 
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    List<Transaction> emplyTransactionList = new List<Transaction>();
                    return emplyTransactionList;
                }
                else
                {
                    throw rex;
                }
            }
        }

		/// <summary>
		/// Retrieves the list of active transactions in Doshii.
		/// </summary>
		/// <returns>The current list of Doshii payments.</returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
		public virtual IEnumerable<Transaction> GetTransactions()
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTransactions"));
            }
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
        /// <returns>
        /// The order that Doshii has recorded after the update.
        /// </returns>
        /// /// <exception cref="ConflictWithOrderUpdateException">
        /// Indicates that there was a conflict between the order provided by the pos and the order currently on Doshii, 
        /// Usually this indicates that the order.updatedAt string is not = to the string on doshii,
        /// If this occurs you should call <see cref="GetOrder"/> with the Id of this order ensure that it is accurate and that the pos has recorded any pending items on the order and resent any required update to Doshii. 
        /// </exception>
        /// <exception cref="OrderUpdateException">There was an issue updating the order on Doshii, See exception for details.</exception>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual Order UpdateOrder(Order order)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateOrder"));
            }
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

        /// <summary>
        /// confirms a pending order on Doshii
        /// </summary>
        /// <param name="order">
        /// The order to be confirmed. 
        /// </param>
        /// <returns></returns>
        internal virtual Order ConfirmCreatedOrder(Order order)
        {
            order.Version = mOrderingManager.RetrieveOrderVersion(order.Id);
            var jsonOrder = Mapper.Map<JsonOrder>(order);
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", jsonOrder.ToJsonString()));

            var returnedOrder = new Order();

            try
            {
                returnedOrder = m_HttpComs.PutConfirmOrderCreated(order);
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
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
		public bool AddTableAllocation(string posOrderId, string tableName)
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }
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

        /// <summary>
        /// Calls the appropriate callback method in <see cref="Interfaces.IOrderingManager"/> to record the checkinId for an order on the pos. 
        /// </summary>
        /// <param name="order">
        /// The order that need to be recorded. 
        /// </param>
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
        /// <returns>
        /// True if successful
        /// False if unsuccessful
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual bool DeleteTableAllocation(string posOrderId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteTableAllocation"));
            }
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
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public Menu UpdateMenu(Menu menu)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMenu"));
            }
            Menu returnedMenu = null;
            try
            {
               returnedMenu = m_HttpComs.PostMenu(menu);
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
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public Surcount UpdateSurcount(Surcount surcount)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateSurcount"));
            }
            if (surcount.Id == null || string.IsNullOrEmpty(surcount.Id))
            {
                mLog.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Surcounts must have an Id to be created or updated on Doshii");
            }
            Surcount returnedSurcharge = null;
            try
            {
                returnedSurcharge = m_HttpComs.PutSurcount(surcount);
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
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public Product UpdateProduct(Product product)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateProduct"));
            }
            if (product.PosId == null || string.IsNullOrEmpty(product.PosId))
            {
                mLog.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Products must have an Id to be created or updated on Doshii");
            }
            Product returnedProduct = null;
            try
            {
                returnedProduct = m_HttpComs.PutProduct(product);
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
        /// <returns>
        /// True if the surcount was deleted
        /// false if the surcount was not deleted
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public bool DeleteSurcount(string posId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteSurcount"));
            }
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
        /// <returns>
        /// True if the product was deleted
        /// False if the product was not deleted
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public bool DeleteProduct(string posId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteProduct"));
            }
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

        /// <summary>
        /// throws the DoshiiManagerNotInitializedException
        /// </summary>
        /// <param name="methodName">
        /// The name of the method that has been called before Initialize. 
        /// </param>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        private void ThrowDoshiiManagerNotInitializedException(string methodName)
        {
            throw new DoshiiManagerNotInitializedException(
                string.Format("You must initialize the DoshiiManager instance before calling {0}", methodName));
        }

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
