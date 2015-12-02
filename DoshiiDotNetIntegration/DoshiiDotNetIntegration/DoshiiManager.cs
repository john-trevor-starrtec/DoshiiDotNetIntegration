using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Net;
using System.Reflection;
using System.Text;

namespace DoshiiDotNetIntegration
{
    /// <summary>
    /// This class manages network operations (requests and responses) between Doshii and the Point of sale (POS) software.
    /// This class supports ordering and product operations including the following;
    /// <list type="bullet">
    ///   <item>Creating orders</item>
    ///   <item>Modifying existing orders</item>
    ///   <item>Setting the status of a consumer to a “checked-in” status</item>
    ///   <item>Creating products</item>
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
		private const int DefaultTimeout = 30;

		#endregion

		#region properties, constructors, Initialize, versionCheck

		/// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiWebSocketsCommunication class for interacting with the Doshii webSocket connection
        /// </summary>
        private DoshiiWebSocketsCommunication m_SocketComs = null;

        /// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiHttpCommunication class for interacting with the Doshii HTTP restful API
        /// </summary>
        private DoshiiHttpCommunication m_HttpComs = null;

		/// <summary>
		/// The payment module manager is the core module manager for the Doshii platform.
		/// </summary>
		private IPaymentModuleManager mPaymentManager;

		/// <summary>
		/// The logging manager for the Doshii SDK.
		/// </summary>
		private DoshiiLogManager mLog;

        /// <summary>
        /// The authentication token for the venue 
        /// </summary>
        private string AuthorizeToken { get; set; }

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
        public DoshiiManager(IPaymentModuleManager paymentManager, IDoshiiLogger logger)
        {
			if (paymentManager == null)
				throw new ArgumentNullException("paymentManager", "IPaymentModuleManager needs to be instantiated as it is a core module");

			mPaymentManager = paymentManager;
			mLog = new DoshiiLogManager(logger);
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
        public virtual void Initialize(string socketUrl, string token, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
        {
			// TODO: Remove socketUrl parameter and build it here based on urlBase?

			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: Version {3} with; {4} sourceUrl: {0}, {4}token {1}, {4}BaseUrl: {2}", socketUrl, token, urlBase, CurrentVersion(), Environment.NewLine));
			
            if (string.IsNullOrWhiteSpace(socketUrl))
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, "Doshii: Initialization failed - required sockerUrl");
                throw new ArgumentException("empty socketUrl");
            }

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
        private bool InitializeProcess(string socketUrl, string UrlBase, bool StartWebSocketConnection, int timeOutValueSecs)
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
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format(string.Format("Initializing Doshii failed, there was an exception that was {0}", ex.ToString())));
                }
            }

            return result;
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Subscribes to the socket communication events 
        /// </summary>
        private void SubscribeToSocketEvents()
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
        private void UnsubscribeFromSocketEvents()
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, "Doshii: Unsubscribing from socket events");
            m_SocketComs.OrderStatusEvent -= new DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.TransactionStatusEvent += new DoshiiWebSocketsCommunication.TransactionStatusEventHandler(SocketComsTransactionStatusEventHandler);
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
            switch (transactionFromPos.Status)
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
        private bool RequestPaymentForOrder(Transaction transaction)
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
                    
                }
				else if (rex.StatusCode == HttpStatusCode.PaymentRequired)
				{
					// this just means that the partner failed to claim payment when requested
                    mPaymentManager.CancelPayment(transaction);
					return false; // Question: should this return false?? - the only call to this method doesn't listen to the result - I think if we ever need the result this should be false. 
				}
            }
            catch (NullOrderReturnedException)
            {
				mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a postTransaction for order.Id{0}", transaction.OrderId));
            }
            catch (Exception ex)
            {
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a postTransaction for order.Id {0} : {1}", transaction.OrderId, ex));
            }

            if (returnedTransaction != null && returnedTransaction.Id == transaction.Id && returnedTransaction.Status == "complete")
            {
				var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: transaction post for payment - '{0}'", jsonTransaction.ToJsonString()));

                mPaymentManager.AcceptPayment(returnedTransaction);
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
            if (order.Id == "0")
            {
				// new order
                order.UpdatedAt = DateTime.Now;
                try
                {
                    returnedOrder = m_HttpComs.PutOrder(order);
                    if (returnedOrder.Id == "0")
                    {
						mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == HttpStatusCode.Conflict)
                    {
                        throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id));
                    }
                    else if (rex.StatusCode == HttpStatusCode.NotFound)
                    {

                    }

                    throw rex;
                }
                catch (NullOrderReturnedException)
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a PostOrder for order.CheckinId {0}", order.CheckinId));
                }
                catch (Exception ex)
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.CheckinId {0} : {1}", order.CheckinId, ex));
                }
            }
            else
            {
                try
                {
                    returnedOrder = m_HttpComs.PutOrder(order);
                    if (returnedOrder.Id == "0")
                    {
						mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == HttpStatusCode.Conflict)
                    {
                        throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                    }
                    else if (rex.StatusCode == HttpStatusCode.NotFound)
                    {
                        
                    }

                    throw rex;
                }
                catch (NullOrderReturnedException)
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
                }
                catch (Exception ex)
                {
					mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
                }
            }

            return returnedOrder;
        }

        #endregion

        #region tableAllocation and consumers

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
        public virtual void DeleteTableAllocation(string doshiiCustomerId, string tableName, TableAllocationRejectionReasons deleteReason)
        {
			mLog.LogMessage(typeof(DoshiiManager), DoshiiLogLevels.Debug, string.Format("Doshii: pos DeAllocating table for doshiiCustomerId - '{0}', table '{1}'", doshiiCustomerId, tableName));
            try
            {
                m_HttpComs.DeleteTableAllocationWithCheckInId(doshiiCustomerId, deleteReason);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
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

		/// <summary>
		/// Retrieves the current configuration from Doshii.
		/// </summary>
		/// <returns>The current configuration from Doshii.</returns>
		public Configuration GetConfiguration()
		{
			try
			{
				return m_HttpComs.GetConfig();
			}
			catch (RestfulApiErrorResponseException rex)
			{
				throw rex;
			}
		}

		/// <summary>
		/// Updates the supplied <paramref name="configuration"/> in Doshii.
		/// </summary>
		/// <param name="configuration">The new configuration to be sent to Doshii.</param>
		/// <returns>True on successful update; false otherwise.</returns>
		public bool UpdateConfiguration(Configuration configuration)
		{
			try
			{
				return m_HttpComs.PutConfiguration(configuration);
			}
			catch (RestfulApiErrorResponseException rex)
			{
				throw rex;
			}
		}

		#endregion
	}
}
