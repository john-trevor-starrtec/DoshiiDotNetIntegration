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
using System.Runtime.InteropServices;
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
    /// To use this SDK you must;
    /// <list type="bullet">
    ///     <item>Instantiate the DoshiiManager</item>
    ///     <item>Call <see cref="Initialize"/> on the instance of the DoshiiManager</item>
    /// </list>
    /// </summary>
    /// DoshiiManager Usage.
    /// To update orders on Doshii use the following methods;
    /// <list type="bullet">
    ///     <item><see cref="UpdateOrder"/></item> 
    ///     <item><see cref="AddTableAllocation"/></item> 
    /// </list>
    /// To retrieve orders from Doshii use the following methods;
    /// <list type="bullet">
    ///     <item><see cref="GetOrder"/></item>
    ///     <item><see cref="GetOrders"/></item>
    /// </list>
    /// To keep the Menu on the Doshii up to date with the products available on the pos the following 5 methods should be used;
    /// <list type="bullet">
    ///     <item><see cref="UpdateProduct"/></item> 
    ///     <item><see cref="DeleteProduct"/></item> 
    ///     <item><see cref="UpdateSurcount"/></item> 
    ///     <item><see cref="DeleteSurcount"/></item> 
    ///     <item><see cref="UpdateMenu"/></item>
    /// </list>
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
        /// the membership manager
        /// </summary>
        private IMembershipModuleManager mMemberManager;

        /// <summary>
        /// The unique LocationId for the venue -- this can be retrieved from Doshii before enabling the integration.
        /// </summary>
        internal string LocationToken { get; set; }

        /// <summary>
        /// The Pos vendor name retrieved from Doshii.
        /// </summary>
        internal string Vendor { get; set; }

        /// <summary>
        /// This is unique for every POS company and should be retrieved from Doshii dashboard
        /// </summary>
        internal string SecretKey { get; set; }

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
        public DoshiiManager(IPaymentModuleManager paymentManager, IDoshiiLogger logger, IOrderingManager orderingManager, IMembershipModuleManager memberManager)
        {
            if (paymentManager == null)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IPaymentModuleManager needs to be instantiated as it is a core module");
                throw new ArgumentNullException("paymentManager", "IPaymentModuleManager needs to be instantiated as it is a core module");
            }
            if (orderingManager == null)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IOrderingManager needs to be instantiated as it is a core module");
                throw new ArgumentNullException("orderingManager", "IOrderingManager needs to be instantiated as it is a core module");
            }
            if (memberManager == null)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, "Doshii: Membership module not supported - IMembershipModuleManager needs to be instantiated to implement the member functionality");
            }    
			mPaymentManager = paymentManager;
            mOrderingManager = orderingManager;
            mMemberManager = memberManager;
            mLog = new DoshiiLogManager(logger);
			AutoMapperConfigurator.Configure();
        }

        /// <summary>
        /// This method MUST be called immediately after this class is instantiated to initialize communication with Doshii.
        /// <para/> It completed the following tasks;
        /// <list type="bullet">
        ///     <item>Initializes the WebSockets communications with Doshii</item>
        ///     <item>Initializes the HTTP communications with Doshii</item>
        /// </list>
        /// <para/>If this method returns false the Doshii integration CANNOT be used until this method has been called successfully. 
        /// </summary>
        /// <param name="token">
        /// The unique venue authentication token - This can be retrieved from the Doshii before integration, this value is unique for each venue. 
        /// </param>
        /// <param name="urlBase">
        /// The base URL for communication with the Doshii restful API 
        /// <para/>an example of the format for this URL is 'https://sandbox.doshii.co/pos/api/v2'
        /// </param>
        /// <param name="startWebSocketConnection">
        /// Should this instance of the class start the webSocket connection with doshii
        /// <para/>There should only be one webSockets connection to Doshii per venue
        /// <para/>The webSocket connection is only necessary for the ordering functionality of the Doshii integration and is not necessary for updating the Doshii menu. 
        /// </param>
        /// <param name="timeOutValueSecs">
        /// This is the amount of this the web sockets connection can be down before the integration assumes the connection has been lost. 
        /// </param>
        /// <returns>
        /// True if the initialize procedure was successful.
        /// <para/>False if the initialize procedure was unsuccessful.
        /// </returns>
        /// <exception cref="System.ArgumentException">An argument Exception will the thrown when there is an issue with one of the paramaters.</exception>
        public virtual bool Initialize(string locationToken, string vendor, string secretKey, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
        {
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: Version {2} with; {3}locationId {0}, {3}BaseUrl: {1}, {3}vendor: {4}, {3}secretKey: {5}", locationToken, urlBase, CurrentVersion(), Environment.NewLine, vendor, secretKey));
			
            if (string.IsNullOrWhiteSpace(urlBase))
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required urlBase");
				throw new ArgumentException("empty urlBase");
            }

            if (string.IsNullOrWhiteSpace(locationToken))
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required locationId");
                throw new ArgumentException("empty locationToken");
            }

            if (string.IsNullOrWhiteSpace(vendor))
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required vendor");
                throw new ArgumentException("empty vendor");
            }

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required secretKey");
                throw new ArgumentException("empty secretKey");
            }

			int timeout = timeOutValueSecs;

			if (timeout < 0)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - timeoutvaluesecs must be minimum 0");
                throw new ArgumentException("timeoutvaluesecs < 0");
            }
			else if (timeout == 0)
			{
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, String.Format("Doshii: will use default timeout of {0}", DoshiiManager.DefaultTimeout));
				timeout = DoshiiManager.DefaultTimeout;
			}

			LocationToken = locationToken;
            Vendor = vendor;
            SecretKey = secretKey;
            urlBase = FormatBaseUrl(urlBase);
            string socketUrl = BuildSocketUrl(urlBase, LocationToken);
            m_IsInitalized = InitializeProcess(socketUrl, urlBase, startWebSocketConnection, timeout);
            if (startWebSocketConnection)
            {
                try
                {
                    RefreshAllOrders();
                }
                catch (Exception ex)
                {
                    m_IsInitalized = false;
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: There was an exception refreshing all orders, Please check the baseUrl is correct", ex);
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Fatal, "Doshii: Initialization failed");
                }
                
            }
            return m_IsInitalized;
        }

        private string FormatBaseUrl(string baseUrl)
        {
            char last = baseUrl[baseUrl.Length - 1];
            if (last == '/')
            {
                return baseUrl.Substring(0, baseUrl.Length - 1);
            }
            else
            {
                return baseUrl;
            }
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
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, "Doshii: Initializing Doshii");

            m_HttpComs = new DoshiiHttpCommunication(UrlBase, mLog, this);

            if (StartWebSocketConnection)
            {
                try
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Info, string.Format(string.Format("socketUrl = {0}, timeOutValueSecs = {1}", socketUrl, timeOutValueSecs)));
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
			// baseApiUrl is for example https://sandbox.doshii.co/pos/v3
			// require socket url of wss://sandbox.doshii.co/pos/socket?token={token} in this example
			// so first, replace http with ws (this handles situation where using http/ws instead of https/wss
			string result = baseApiUrl.Replace("http", "ws");

			// next remove the /api/v2 section of the url
			int index = result.IndexOf("/v");
			if (index > 0 && index < result.Length)
			{
				result = result.Remove(index);
			}

		    index = result.IndexOf(".");
            if (index > 0 && index < result.Length)
            {
                result = result.Insert(index,"-socket");
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
                m_SocketComs.MemberCreatedEvent += new DoshiiWebSocketsCommunication.MemberCreatedEventHandler(SocketComsMemberCreatedEventHandler);
                m_SocketComs.MemberUpdatedEvent += new DoshiiWebSocketsCommunication.MemberUpdatedEventHandler(SocketComsMemberUpdatedEventHandler);
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
            m_SocketComs.MemberCreatedEvent -= new DoshiiWebSocketsCommunication.MemberCreatedEventHandler(SocketComsMemberCreatedEventHandler);
            m_SocketComs.MemberUpdatedEvent -= new DoshiiWebSocketsCommunication.MemberUpdatedEventHandler(SocketComsMemberUpdatedEventHandler);
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
        /// Checks all orders on the doshii system, 
        /// <para/>if there are any pending orders <see cref="HandleOrderCreated"/> will be called. 
        /// <para/>currently this method does not check the transactions as there should be no unlinked transactions for already created orders, order ahead only allows for 
        /// <para/>partners to make payments when they create an order else the payment is expected to be made by the customer on receipt of the order. 
        /// </summary>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown if there is an issue getting the orders from Doshii.</exception>
        internal void RefreshAllOrders()
        {
            
            try
            {
                //check unassigned orders
                mLog.LogMessage(this.GetType(), DoshiiLogLevels.Info, "Refreshing all orders.");
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
        /// <para/>this method will test that the order on doshii has not changed since it was original received by the pos. 
        /// <para/>It is the responsibility of the pos to ensure that the products on the order were not changed during the confirmation process as this will not 
        /// <para/>be checked by this method. 
        /// <para/>If this method is not successful then the order should not be committed on the pos and <see cref="RejectOrderAheadCreation"/> should be called.
        /// </summary>
        /// <param name="orderToAccept">
        /// The order that is being accepted
        /// </param>
        /// <returns>
        /// True if the order was recorded as accepted on Doshii
        /// <para/>False if the order was not recorded as accepted on Doshii.
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
                PutOrderCreatedResult(orderToAccept);
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
        /// Call this method to reject an order created by an order ahead partner,
        /// </summary>
        /// <param name="orderToReject">
        /// The pending Doshii order that will be rejected
        /// </param>
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
                PutOrderCreatedResult(order);
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


        internal virtual void SocketComsMemberCreatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.MemberEventArgs e)
        {
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: received a member created event with member Id '{0}'", e.MemberId));
            try
            {
                mMemberManager.CreateMemberOnPos(e.Member);
            }
            catch(MemberExistOnPosException ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create member with Id '{0}' on pos failed due to member already existing, now attempting to update existing member.", e.MemberId));
                try
                {
                    mMemberManager.UpdateMemberOnPos(e.Member);
                }
                catch (MemberDoesNotExistOnPosException nex)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update member with Id '{0}' on pos failed.", e.MemberId));
                }
            }
        }

        internal virtual void SocketComsMemberUpdatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.MemberEventArgs e)
        {
            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: received a member updated event for member Id '{0}'", e.MemberId));
            try
            {
                mMemberManager.UpdateMemberOnPos(e.Member);
            }
            catch (MemberDoesNotExistOnPosException ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update member with Id '{0}' on pos failed due to member not currently existing, now attempting to create existing member.", e.MemberId));
                try
                {
                    mMemberManager.CreateMemberOnPos(e.Member);
                }
                catch (MemberExistOnPosException nex)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create member with Id '{0}' on pos failed", e.MemberId));
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
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="transaction">
        /// The transaction that should be paid
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

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id)
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
        /// <param name="transaction">
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
        /// Attempts to add a pos transaction to doshii
        /// </summary>
        /// <param name="transaction">
        /// The transaction to add to Doshii
        /// </param>
        /// <returns>
        /// The transaction that was recorded on doshii if the request was successful
        /// <para/>Returns null if the request failed. 
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
        /// <para/>If there is no order corresponding to the Id, a blank order may be returned. 
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
        /// This method returns a consumer from Doshii corresponding to the CheckinId
        /// </summary>
        /// <param name="checkinId">
        /// The checkinId for the consumer that is being requiested. 
        /// </param>
        /// <returns>
        /// The consumer with the corresponding checkinId
        /// <para/>If there is no consumer corresponding to the checkinId, a blank consumer may be returned. 
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
        /// This method returns an order from Doshii corresponding to the doshiiOrderId
        /// <para/>This will only return orders that are unlinked
        /// <para/>If doshii has already linked the order on the pos then <see cref="GetOrder"/> should be called
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns>
        /// The order with the corresponding Id
        /// <para/>If there is no order corresponding to the Id, a blank order may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual Order GetOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetOrderFromDoshiiOrderId"));
            }
            try
            {
                return m_HttpComs.GetOrderFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

		/// <summary>
		/// Retrieves the current order list from Doshii.
		/// <para/>This method will only return orders that are linked to pos ordered in Doshii
		/// <para/>To get a list of unlinked orders call<see cref="GetUnlinkedOrders"/>
		/// </summary>
		/// <returns>
		/// The current list of linked orders available in Doshii.
		/// If there are no linked orders a blank IEnumerable is returned. 
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
        /// <returns>
        /// The current list of orders available in Doshii.
        /// <para/>If there are no unlinkedOrders a blank IEnumerable is returned.
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
        /// <para/>This method should only be called in relation to unlinkedOrders as this method will not return transactions related to linked orders. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns>
        /// <see cref="Transaction"/> that relate to the doshiiOrderId. 
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
		/// <returns>The current list of active Doshii transactions.</returns>
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
        /// <para/>This method should only be used to update orders that have been modified on the pos.
        /// <para/>This method should not be used to accept or reject pending orders from doshii.
        /// <para/>To accept a pending order ahead order from Doshii use <see cref="AcceptOrderAheadCreation"/>
        /// <para/>To reject a pending order ahead order from Doshii use <see cref="RejectOrderAheadCreation"/>
        /// </summary>
        /// <param name="order">
        /// The order must contain all the products / items,
        /// <para/>All surcharges,
        /// <para/>All discounts,
        /// <para/>Included in the check as this method will overwrite the order currently on Doshii 
        /// </param>
        /// <returns>
        /// The order that Doshii has recorded after the update.
        /// </returns>
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
        /// Confirms a pending order on Doshii
        /// </summary>
        /// <param name="order">
        /// The order to be confirmed. 
        /// </param>
        /// <returns></returns>
        internal virtual Order PutOrderCreatedResult(Order order)
        {
            if (order.Status == "accepted")
            {
                if (order.Id == null || string.IsNullOrEmpty(order.Id))
                {
                    throw new OrderUpdateException("the pos must set an order.Id for accepted orders.");
                }
            }
            
            var returnedOrder = new Order();

            try
            {
                returnedOrder = m_HttpComs.PutOrderCreatedResult(order);
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

        #region Membership

        public virtual Member GetMember(string memberId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMember"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMember"));
            }
            try
            {
                return m_HttpComs.GetMember(memberId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual IEnumerable<Member> GetMembers()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMembers"));
            }
            if (mMemberManager == null)
            {
                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMembers"));
            }
            try
            {
                return m_HttpComs.GetMembers();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool DeleteMember(Member member)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteMember"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteMember"));
            }
            try
            {
                return m_HttpComs.DeleteMember(member);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }


        public virtual Member UpdateMember(Member member)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            try
            {
                if (string.IsNullOrEmpty(member.Id))
                {
                    return m_HttpComs.PostMember(member);
                }
                else
                {
                    return m_HttpComs.PutMember(member);
                }
                
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual IEnumerable<Reward> GetRewardsForMember(string memberId, string orderId, decimal orderTotal)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetRewardsForMember"));
            }
            if (mMemberManager == null)
            {
                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetRewardsForMember"));
            }
            try
            {
                return m_HttpComs.GetRewardsForMember(memberId, orderId, orderTotal);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool SyncDoshiiMembersWithPosMembers()
        {
            try
            {
                List<Member> DoshiiMembersList = GetMembers().ToList();
                List<Member> PosMembersList = mMemberManager.GetMembersFromPos().ToList();

                var posMembersHashSet = new HashSet<string>(PosMembersList.Select(p => p.Id));
                var membersNotInPos = DoshiiMembersList.Where(p => !posMembersHashSet.Contains(p.Id));

                foreach (var mem in membersNotInPos)
                {
                    mMemberManager.CreateMemberOnPos(mem);
                }

                var doshiiMembersHashSet = new HashSet<string>(DoshiiMembersList.Select(p => p.Id));
                var membersNotInDoshii = PosMembersList.Where(p => !doshiiMembersHashSet.Contains(p.Id));

                foreach (var mem in membersNotInDoshii)
                {
                    mMemberManager.DeleteMemberOnPos(mem);
                }
                return true;
            }
            catch(Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception while attempting to sync Doshii members with the pos"), ex);
                return false;
            }
            
            
        }


        /// <summary>
        /// This method should be called to confirm that the reward is still available for the member and that the reward can be redeemed against the order. 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="reward"></param>
        /// <param name="order"></param>
        /// <returns>
        /// true - when the reward can be redeemed - the pos must then apply the reward to the order, accept payment for the order from the customer and then call rewardConfirm to confirm the use of the redard. 
        /// </returns>
        public virtual bool RedeemRewardForMember(Member member, Reward reward, Order order)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMember"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMember"));
            }
            try
            {
                UpdateOrder(order);
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception putting and order to Doshii for a rewards redeem"), ex);
                return false;
            }
            try
            {
                return m_HttpComs.RedeemRewardForMember(member.Id, reward.Id, order);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberCancel"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberCancel"));
            }
            try
            {
                return m_HttpComs.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemRewardForMemberConfirm(string memberId, string rewardId, Order order)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberConfirm"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberConfirm"));
            }
            try
            {
                return m_HttpComs.RedeemRewardForMemberConfirm(memberId, rewardId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemPointsForMember(Member member, App app, Order order, int points)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMember"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMember"));
            }
            
            try
            {
                order = UpdateOrder(order);
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception putting and order to Doshii for a rewards redeem"), ex);
                return false;
            }
            PointsRedeem pr = new PointsRedeem()
            {
                AppId = app.Id,
                OrderId = order.Id,
                Points = points
            };
            try
            {
                return m_HttpComs.RedeemPointsForMember(pr, member);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemPointsForMemberConfirm(Member member)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberConfirm"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberConfirm"));
            }
            try
            {
                return m_HttpComs.RedeemPointsForMemberConfirm(member);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemPointsForMemberCancel(Member member, string cancelReason)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberCancel"));
            }
            if (mMemberManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberCancel"));
            }
            try
            {
                return m_HttpComs.RedeemPointsForMemberCancel(member, cancelReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        #endregion

        #region tableAllocation and consumers

        /// <summary>
		/// Called by POS to add a table allocation to an order.
		/// </summary>
		/// <param name="posOrderId">The unique identifier of the order on the POS.</param>
        /// <param name="tableNames">A list of the tables to add to the allocaiton, if you want to remove the table allocaiton you should pass an empty list into this param.</param>
		/// <returns>The current order details in Doshii after upload.</returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public bool SetTableAllocationWithoutCheckin(string posOrderId, List<string> tableNames)
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos Allocating table '{0}' to order '{1}'", tableNames[0], posOrderId));

            Order order = null;
			try
			{
				order = mOrderingManager.RetrieveOrder(posOrderId);
			    order.Version = mOrderingManager.RetrieveOrderVersion(posOrderId);
			    order.CheckinId = mOrderingManager.RetrieveCheckinIdForOrder(posOrderId);
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

            if (!string.IsNullOrEmpty(order.CheckinId))
            {
                return ModifyTableAllocation(order.CheckinId, tableNames);
            }
            
            //create checkin
            Checkin checkinCreateResult = null;
            try
            {
                Checkin newCheckin = new Checkin();
                newCheckin.TableNames = tableNames;
                checkinCreateResult = m_HttpComs.PostCheckin(newCheckin);
                if (checkinCreateResult == null)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an error generating a new checkin through Doshii, the table allocation could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a attempting a table allocaiton for order.Id{0}", order.Id), ex);
            }
            
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: Order found, allocating table now"));

            order.CheckinId = checkinCreateResult.Id;
            Order returnedOrder = UpdateOrder(order);
            if (returnedOrder != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkinId"></param>
        /// <param name="tableNames">to remove an allocaiton from a checkin add this as an empty list. </param>
        /// <returns></returns>
        public bool ModifyTableAllocation(string checkinId, List<string> tableNames)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos modifying table allocation table '{0}' to checkin '{1}'", tableNames[0], checkinId));

            //create checkin
            Checkin checkinCreateResult = null;
            try
            {
                Checkin newCheckin = new Checkin();
                newCheckin.TableNames = tableNames;
                newCheckin.Id = checkinId;
                checkinCreateResult = m_HttpComs.PutCheckin(newCheckin);
                if (checkinCreateResult == null)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an error modifying a checkin through Doshii, modifying the table allocation could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation  for checkin {0} : {1}", checkinId, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a attempting a table allocaiton for for checkin {0}", checkinId), ex);
            }
            return true;
        }

        /// <summary>
        /// Calls the appropriate callback method in <see cref="Interfaces.IOrderingManager"/> to record the checkinId for an order on the pos. 
        /// </summary>
        /// <param name="order">
        /// The order that need to be recorded. 
        /// </param>
        internal virtual void RecordOrderCheckinId(string posOrderId, string checkinId)
        {
            try
            {
                mOrderingManager.RecordCheckinForOrder(posOrderId, checkinId);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("Doshii: Attempted to update a checkinId for an order that does not exist on the Pos, Order.id - {0}, checkinId - {1}", posOrderId, checkinId));
                //maybe we should call reject order here. 
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update a checkinId for an order on the pos, Order.Id - {0}, checkinId - {1}, {2}", posOrderId, checkinId, ex.ToString()));
                //maybe we should call reject order here. 
            }
        }

        public virtual bool CloseCheckin(string checkinId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

            mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos closing checkin '{0}'", checkinId));

            Checkin checkinCreateResult = null;
            try
            {
                checkinCreateResult = m_HttpComs.DeleteCheckin(checkinId);
                if (checkinCreateResult == null)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: There was an error attempting to close a checkin."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting to close checkin {0} - {1}", checkinId, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown while attempting to close a checkin {0}", checkinId), ex);
            }
            return true;
        }

        #endregion

        #region products and menu

        /// <summary>
        /// This method is used to update the pos menu on doshii,
        /// <para/>Calls to this method will replace the existing pos menu. 
        /// <para/>If you wish to update or create single products you should use <see cref="UpdateProduct"/> method
        /// <para/>If you wish to update or create single order level surcharges you should use the <see cref="UpdateSurcount"/> method
        /// <para/>if you wish to delete a single product you should use the <see cref="DeleteProduct "/> method
        /// <para/>if you wish to delete a single order level surcharge you should use the <see cref="DeleteSurcount "/> method
         /// </summary>
        /// <param name="menu">
        /// The full pos menu to be updated to doshii
        /// </param>
        /// <returns>
        /// If successful the full menu will be returned. 
        /// <para/>If unsuccessful null will be returned. 
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
        /// If successful the surcount as it exists on doshii will be returned
        /// <para/>If unsuccessful null will be returned. 
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
        /// The product to be created or updated
        /// </param>
        /// <returns>
        /// If successful the product as it exists on doshii will be returned
        /// <para/>If unsuccessful null will be returned. 
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
        /// <para/>false if the surcount was not deleted
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
        /// <para/>False if the product was not deleted
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

        #region Tables

        public virtual Table GetTable(string tableName)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTable"));
            }
            try
            {
                return m_HttpComs.GetTable(tableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual List<Table> GetTables()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTables"));
            }
            try
            {
                return m_HttpComs.GetTables().ToList();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual Table CreateTable(Table table)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "CreateTable"));
            }
            try
            {
                return m_HttpComs.PostTable(table);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual Table UpdateTable(Table table)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateTable"));
            }
            try
            {
                return m_HttpComs.PutTable(table);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual Table DeleteTable(Table table)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteTable"));
            }
            try
            {
                return m_HttpComs.DeleteTable(table);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual Table ReplaceTableListOnDoshii(List<Table> tableList)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "ReplaceTableListOnDoshii"));
            }
            try
            {
                return m_HttpComs.PutTables(tableList);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

#endregion

        /// <summary>
        /// This method is used to get the location information from doshii,
        /// <para/>This should be used to get the DoshiiId for the location - this is the string that can be given to partners to allow them to communicate with this venue through Doshii
        /// </summary>
        /// <returns>
        /// The location object representing this venue.
        /// <para/>Null will be returned if there is an error retrieving the location from doshii. 
        /// </returns>
        public Location GetLocation()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetLocation"));
            }
            try
            {
                return m_HttpComs.GetLocation();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

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

        private void ThrowDoshiiMembershipNotInitializedException(string methodName)
        {
            throw new DoshiiMembershipManagerNotInitializedException(
                string.Format("You must initialize the DoshiiMembership module before calling {0}", methodName));
        }

        public string CreateToken()
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var now = Math.Round((DateTime.Now - unixEpoch).TotalSeconds);
            var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                //{"locationId", LocationId}, //locationId of the location connected to Doshii
                {"locationToken", LocationToken},
                {"timestamp", now}
            };

            return string.Format("Bearer {0}", JWT.JsonWebToken.Encode(payload, SecretKey, JWT.JwtHashAlgorithm.HS256));
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
