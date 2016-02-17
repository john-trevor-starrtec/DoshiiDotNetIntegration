using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using System;
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
        /// The authentication token for the venue 
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
        /// <param name="configuration">
        /// this is the configuration that configures the behaviour of the Doshii API while interacting with this pos. <see cref="Configuration"/> for details about the available settings. 
        /// </param>
        public virtual void Initialize(string token, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
        {
			// TODO: Remove socketUrl parameter and build it here based on urlBase?

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

            string socketUrl = String.Format("{0}/socket", urlBase.Replace("http", "ws"));
            AuthorizeToken = token;
            string socketUrlWithToken = string.Format("{0}?token={1}", socketUrl, token);
			InitializeProcess(socketUrlWithToken, urlBase, startWebSocketConnection, timeout);
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// </summary>
        /// <param name="socketUrl"></param>
        /// <param name="UrlBase"></param>
        /// <param name="StartWebSocketConnection"></param>
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
				m_SocketComs.OrderStatusEvent += new DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
                m_SocketComs.TransactionStatusEvent += new DoshiiWebSocketsCommunication.TransactionStatusEventHandler(SocketComsTransactionStatusEventHandler);
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
            m_SocketComs.OrderStatusEvent -= new DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.TransactionStatusEvent -= new DoshiiWebSocketsCommunication.TransactionStatusEventHandler(SocketComsTransactionStatusEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent -= new DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
            m_SocketComs.SocketCommunicationTimeoutReached -= new DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(SocketComsTimeOutValueReached);
        }
        #endregion

        #region socket communication event handlers

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a socket communication established event and calls refreshComsumerData. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            //send configuration
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
        internal virtual void SocketComsOrderStatusEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
			//this method is not implemented for Pay@table, we will need to implement it for orderAhead but it is now currently necessary. 
            throw new NotImplementedException();
            //when this method is reintroducted the following call must be included every time a order is received from Doshii 
            // m_DoshiiLogic.RecordOrderVersion(e.Order.Id, e.Order.Version);
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_TransactionStatusEvent, 
        /// Calls the appropriate method on the PaymentInterface to act on the transaction depending on the transaction status. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTransactionStatusEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TransactionEventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: received a transaction status event with status '{0}', for transaction Id '{1}', for order Id '{2}'", e.Transaction.Status, e.TransactionId, e.Transaction.OrderId));
            Transaction transactionFromPos = null;
			switch (e.Transaction.Status)
            {
                case "pending":
					try
                    {
                        transactionFromPos = mPaymentManager.ReadyToPay(e.Transaction);
                    }
                    catch(OrderDoesNotExistOnPosException)
                    {
						mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: A transaction was initiated on the Doshii API that does not exist on the system, orderid {0}", e.Transaction.OrderId));
                        break;
                    }

                    if (transactionFromPos != null)
                    {
                        RequestPaymentForOrder(transactionFromPos);
                    }
                    break;
                default:
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: unknown / unsupported transaction status from the Doshii API - '{0}'", e.Transaction.Status)); 
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API",e.Transaction.Status));
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
        internal virtual bool RequestPaymentForOrder(Transaction transaction)
        {
            var returnedTransaction = new Transaction();
            transaction.Status = "waiting";

            try
            {
                returnedTransaction = m_HttpComs.PostTransaction(transaction);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.NotFound)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not locate the order for order.Id{0}", transaction.OrderId));
                    mPaymentManager.CancelPayment(transaction);
                    return false;
                }
				else if (rex.StatusCode == HttpStatusCode.PaymentRequired)
				{
					// this just means that the partner failed to claim payment when requested
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: The partner could not claim the payment for for order.Id{0}", transaction.OrderId));
					mPaymentManager.CancelPayment(transaction);
                    return false; // Question: should this return false?? - the only call to this method doesn't listen to the result - I think if we ever need the result this should be false. 
				}
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

                mPaymentManager.RecordPayment(returnedTransaction);
                return true;
            }
            else
            {
                mPaymentManager.CancelPayment(transaction);
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
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update an order verison on the pos, OrderId - {0}, version - {1}, {2}", posOrderId, version, ex.ToString()));
            }
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
		/// Retrieves the current order list from Doshii.
		/// </summary>
		/// <returns>The current list of orders available in Doshii.</returns>
		public virtual System.Collections.Generic.IEnumerable<Order> GetOrders()
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
		/// Retrieves the list of payments from Doshii.
		/// </summary>
		/// <returns>The current list of Doshii payments.</returns>
		public virtual System.Collections.Generic.IEnumerable<Transaction> GetTransactions()
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
        /// This method will request payment for an order from the partner associated with the order. 
        /// This method will only request a payment for an order that is already associated with a partner on the Doshii Api.
        /// NOTE: it is not necessary to act on the return from this method it it's true - when true <see cref="IPaymentModuleManger.AcceptPayment"/> will be called
        /// if the payment request fails  <see cref="IPaymentModuleManger.CancelPayment"/> will be called. If the pos wishes to retry at this point the pos should react to the false return. 
        /// </summary>
        /// <param name="orderToPay">The order the pos wishes to request payment for</param>
        /// <param name="amountToPay">The amount the pos is requesting payment for</param>
        /// <param name="requestFullPayment">If true, the pos will only accept a payment that is equal to the amountToPay
        /// If false, the pos will accept any amount upto and including the amountToPay</param>
        /// <returns>
        /// True - if the payment request was successful
        /// false - if the payment request was not successful 
        ///  </returns>
        /// <exception cref="TransactionNotProcessecException">If the transaction could not be created and requested from Doshii</exception> 
        public virtual bool RequestPaymentForOrder(Order orderToPay, decimal amountToPay, bool requestFullPayment)
        {
            //This method is not useful for pay@table as the pay@table system will always be initiating the payment.
            // This may be useful for order ahead if the partner does not pay automatically when making the order.
            // It will be more useful for tab systems like Clipp, OneTab, that have created an order on the system that is expectecd to be paid later. 
            // It may also be useful for integrated eftpos - but at that point it will need to be modified to accept the partner that the pos wishes to collect the payment from
            // at the moment it's will only be requesting a payment from a payment system that is already associated with the order in the doshii API
            if (orderToPay == null)
            {
                throw new ArgumentNullException("orderToPay");
            }
            if (string.IsNullOrWhiteSpace(orderToPay.Id))
            {
                throw new TransactionRequestNotProcessedException("orderToPay.Id is not valid");
            }
            if (amountToPay <= 0)
            {
                throw new TransactionRequestNotProcessedException("you cannot request a payment for an amount below $0");
            }
            var transaction = new Transaction();
            transaction.OrderId = orderToPay.Id;
            transaction.AcceptLess = !requestFullPayment;
            transaction.PartnerInitiated = false;
            transaction.PaymentAmount = amountToPay;
            transaction.Status = "waiting";
            return RequestPaymentForOrder(transaction);
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
			var jsonOrder = Mapper.Map<JsonOrder>(order);
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", jsonOrder.ToJsonString()));
            
            if (order.Status != "paid")
            {
                order.Status = "accepted";
            }

            var returnedOrder = new Order();
            
            try
            {
                returnedOrder = m_HttpComs.PutOrder(order);
                if (returnedOrder.Id == "0")
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId while updating order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format("Doshii: order was returned from doshii without an orderId while updating order with id {0}", order.Id));
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
		public bool AddTableAllocation(string posOrderId, TableAllocation table)
		{
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos Allocating table '{0}' to order '{1}'", table.Name, posOrderId));

			Order order = null;
			try
			{
				order = mOrderingManager.RetrieveOrder(posOrderId);
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
			tableOrder.Order = (Order)order.Clone(); // I was thinking clone should be used here to limit order to this scope, but on second thought it probably isn't necessary
			tableOrder.Table = table; // so I haven't worried about cloning the table object here

			try
			{
				return m_HttpComs.PutOrderWithTableAllocation(tableOrder);
			}
			catch (RestfulApiErrorResponseException rex)
			{
                if (rex.StatusCode == HttpStatusCode.Conflict)
                {
                    mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("There was a conflict updating order.id {0} during table allocaiton", order.Id.ToString()));
                    throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0} during table Allocation", order.Id.ToString()));
                }
                throw new OrderUpdateException("Update order with table allocaiton not successful", rex);
			}
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a table allocation order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a attempting a table allocaiton for order.Id{0}", order.Id), ex);
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
                return m_HttpComs.DeleteTableAllocation(posOrderId);
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

		#region Configuration

		#endregion
	}
}
