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
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Controllers;

namespace DoshiiDotNetIntegration
{
    /// <summary>
    /// This class controls requests to Doshii from the Point of sale (POS) software.
    /// This class supports ordering operations, product operations, table operations, Member operations, and reservation operations including the following;
    /// <list type="bullet">
    ///   <item>Creating orders</item>
    ///   <item>Modifying existing orders</item>
    ///   <item>Setting the status of a consumer to a “checked-in” status</item>
    ///   <item>Creating products</item>re
    ///   <item>Modifying existing products</item>
    ///   <item>Deleting the products</item>
    ///   <item>Creating tables</item>re
    ///   <item>Modifying existing tables</item>
    ///   <item>Deleting tables</item>
    /// </list>
    /// To use this SDK you must;
    /// <list type="bullet">
    ///     <item>Implement the required interfaces</item>
    ///     <item>Instantiate the DoshiiController</item>
    ///     <item>Call <see cref="Initialize"/> on the instance of the DoshiiController</item>
    /// </list>
    /// </summary>
    /// DoshiiController Usage.
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
    /// To keep the venue tables up to date with the tables available on Doshii the following methods should be used
    /// <list type="bullet">
    ///     <item><see cref="UpdateTable"/></item> 
    ///     <item><see cref="CreateTable"/></item> 
    ///     <item><see cref="DeleteTable"/></item> 
    /// </list>
    /// to keep the venue members up to date with the members available on Doshii the following methods should be used
    /// <list type="bullet">
    ///     <item><see cref="UpdateMember"/></item> 
    ///     <item><see cref="DeleteMember"/></item> 
    /// </list>
    /// to retreive and redeem rewards for members use the following methods. 
    /// <list type="bullet">
    ///     <item><see cref="GetMember"/></item> 
    ///     <item><see cref="GetMembers"/></item> 
    ///     <item><see cref="GetRewardsForMember"/></item> 
    ///     <item><see cref="RedeemPointsForMember"/></item> 
    ///     <item><see cref="RedeemPointsForMemberCancel"/></item>
    ///     <item><see cref="RedeemPointsForMemberConfirm"/></item>
    ///     <item><see cref="RedeemRewardForMember"/></item>
    ///     <item><see cref="RedeemRewardForMemberCancel"/></item>
    ///     <item><see cref="RedeemRewardForMemberConfirm"/></item>
    /// </list>
    /// The process of redeeming rewards and points for a member follows the same patters,
    /// <list type="bullet">
    ///     <item>Get the rewards / Points available for a given member using <see cref="GetRewardsForMember"/> for rewards and <see cref="GetMember"/> for points</item> 
    ///     <item>Ensure that the rewards / points are still available to be redeemed by the member with <see cref="RedeemRewardForMember"/> for rewards and <see cref="RedeemPointsForMember"/></item> 
    ///     <item>If the above method returns true you should apply the reward / points to the order on the pos and within 30 - 60 secs call (If the time limit expires the redemption transaction is cancelled by Doshii and the pos user must start the redemption again)</item> 
    ///     <item><see cref="RedeemPointsForMemberConfirm"/> for points or <see cref="RedeemRewardForMemberConfirm"/> to confirm use of the reward.</item> 
    ///     <item>When the above step is completed there is no longer a method to give the points or rewards back to a member.</item>
    /// </list>
    /// To retreive and seat bookings use the following methods. 
    /// <list type="bullet">
    ///     <item><see cref="GetBooking"/></item> 
    ///     <item><see cref="GetBookings"/></item> 
    ///     <item><see cref="SeatBooking"/></item> 
    /// </list> 
    /// <remarks>
    /// The DoshiiController supports two communication protocols HTTP and Websockets. 
    /// The websockets protocol is used to open a websocket connection with the DoshiiAPI and once it is open, 
    /// the DoshiiController receives the notification event messages from DoshiiAPI. Events include when a user 
    /// creates an order event ect.. The HTTP protocol is used for all other operations including creating orders, 
    /// update orders, creating products e.t.c.)
    /// </remarks>
    public class DoshiiController : IDisposable
	{
		#region Constants

		/// <summary>
		/// Default timeout (in seconds) for the connection to the Doshii API -- 30.
		/// </summary>
		internal const int DefaultTimeout = 30;

		#endregion

		#region properties, constructors, Initialize, versionCheck

        
        private bool m_IsInitalized = false;

        /// <summary>
        /// A property indicating if initialize has been called on the doshii manager. 
        /// </summary>
        internal virtual bool IsInitalized
        {
            get { return m_IsInitalized; }
            set { m_IsInitalized = value; }
        }

		
        private SocketsController m_SocketComs = null;

        /// <summary>
        /// Holds an instance of CommunicationLogic.SocketsController class for interacting with the Doshii webSocket connection
        /// </summary>
        internal virtual SocketsController SocketComs
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
        /// Holds an instance of CommunicationLogic.HttpController class for interacting with the Doshii HTTP restful API
        /// </summary>
        internal HttpController _httpComs = null;

		/// <summary>
		/// The logging manager for the Doshii SDK.
		/// </summary>
		internal Models.Controllers _controllers { get; set; }
        
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
        /// <param name="configurationManager">An instance of IConfigurationManager that provides pos side configuration to the sdk.</param>
        public DoshiiController(IConfigurationManager configurationManager)
        {
            _controllers = new Models.Controllers();
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager", "IConfigurationManager needs to be instantiated as it is a core module");
            }
            _controllers.ConfigurationManager = configurationManager;
            _controllers.TransactionManager = configurationManager.GetTransactionManagerFromPos();
            _controllers.OrderingManager = configurationManager.GetOrderingManagerFromPos();
            _controllers.RewardManager = configurationManager.GetRewardManagerFromPos();
            _controllers.ReservationManager = configurationManager.GetReservationManagerFromPos();

            if (configurationManager.GetLoggingManagerFromPos() == null)
            {
                throw new ArgumentNullException("logger", "ILoggingManager needs to be instantiated as it is a core module");
            }
            _controllers.LoggingController = new LoggingController(configurationManager.GetLoggingManagerFromPos());
            if (_controllers.TransactionManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IConfigurationManager.GetTransactionManager() cannot return null, it is a core module");
                throw new ArgumentNullException("paymentManager", "ITransactionManager needs to be instantiated as it is a core module");
            }
            if (_controllers.OrderingManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IConfigurationManager.GetOrderingManager() cannot return null, it is a core module");
                throw new ArgumentNullException("orderingManager", "IOrderingManager needs to be instantiated as it is a core module");
            }
            if (_controllers.RewardManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: Membership module not supported - IConfigurationManager.GetRewardManager() returned null");
            }
            if (_controllers.ReservationManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: Reservation module not supported - IConfigurationManager.GetReservationManager() returned null");
            }
            

			AutoMapperConfigurator.Configure();
        }

        /// <summary>
        /// This method MUST be called immediately after this class is instantiated to initialize communication with Doshii.
        /// <para/> It completes the following tasks;
        /// <list type="bullet">
        ///     <item>Initializes the WebSockets communications with Doshii</item>
        ///     <item>Initializes the HTTP communications with Doshii</item>
        /// </list>
        /// <para/>If this method returns false the Doshii integration CANNOT be used until this method has been called successfully. 
        /// </summary>
        /// <param name="startWebSocketConnection">should this instance start the web sockets connection.
        /// NOTE: there should only be One web socket connection per venue.</param>
        /// <returns>true is the initialization was successful false if not. </returns>
        /// <exception cref="System.ArgumentException">An argument Exception will the thrown when there is an issue with the interfaces provided in the IConfigurationManager implementation.</exception>
        public virtual bool Initialize(bool startWebSocketConnection)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: Version {2} with; {3}locationId {0}, {3}BaseUrl: {1}, {3}vendor: {4}, {3}secretKey: {5}", _controllers.ConfigurationManager.GetLocationTokenFromPos(), _controllers.ConfigurationManager.GetBaseUrlFromPos(), CurrentVersion(), Environment.NewLine, _controllers.ConfigurationManager.GetVendorFromPos(), _controllers.ConfigurationManager.GetSecretKeyFromPos()));
			
            if (string.IsNullOrWhiteSpace(_controllers.ConfigurationManager.GetBaseUrlFromPos()))
            {
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required urlBase");
				throw new ArgumentException("empty urlBase");
            }

            if (string.IsNullOrWhiteSpace(_controllers.ConfigurationManager.GetLocationTokenFromPos()))
            {
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required locationId");
                throw new ArgumentException("empty locationToken");
            }

            if (string.IsNullOrWhiteSpace(_controllers.ConfigurationManager.GetVendorFromPos()))
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required vendor");
                throw new ArgumentException("empty vendor");
            }

            if (string.IsNullOrWhiteSpace(_controllers.ConfigurationManager.GetSecretKeyFromPos()))
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - required secretKey");
                throw new ArgumentException("empty secretKey");
            }

            int timeout = _controllers.ConfigurationManager.GetSocketTimeOutFromPos();

			if (timeout < 0)
            {
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - timeoutvaluesecs must be minimum 0");
                throw new ArgumentException("timeoutvaluesecs < 0");
            }
			else if (timeout == 0)
			{
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, String.Format("Doshii: will use default timeout of {0}", DoshiiController.DefaultTimeout));
				timeout = DoshiiController.DefaultTimeout;
			}

            string socketUrl = BuildSocketUrl(FormatBaseUrl(_controllers.ConfigurationManager.GetSocketUrlFromPos()), _controllers.ConfigurationManager.GetLocationTokenFromPos());
            m_IsInitalized = InitializeProcess(socketUrl, FormatBaseUrl(_controllers.ConfigurationManager.GetBaseUrlFromPos()), startWebSocketConnection, timeout);
            if (startWebSocketConnection)
            {
                try
                {
                    _controllers.OrderingController.RefreshAllOrders();
                }
                catch (Exception ex)
                {
                    m_IsInitalized = false;
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: There was an exception refreshing all orders, Please check the baseUrl is correct", ex);
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed");
                }
                
            }
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
        /// Indicates if this instance of the DoshiiController should start a WebnSocket connection with Doshii
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
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, "Doshii: Initializing Doshii");

            _httpComs = new HttpController(UrlBase, _controllers);
            _controllers.TransactionController = new TransactionController(_controllers, _httpComs);
            _controllers.OrderingController = new OrderingController(_controllers, _httpComs);
            _controllers.MenuController = new MenuController(_controllers, _httpComs);
            _controllers.TableController = new TableController(_controllers, _httpComs);
            _controllers.CheckinController = new CheckinController(_controllers, _httpComs);
            _controllers.ConsumerController = new ConsumerController(_controllers, _httpComs);
            if (_controllers.ReservationManager != null)
            {
                _controllers.ReservationController = new ReservationController(_controllers, _httpComs);
            }
            else
            {
                _controllers.ReservationController = null;
            }
            if (_controllers.RewardManager != null)
            {
                _controllers.RewardController = new RewardController(_controllers, _httpComs);
            }
            else
            {
                _controllers.RewardController = null;
            }
            
            if (StartWebSocketConnection)
            {
                try
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, string.Format(string.Format("socketUrl = {0}, timeOutValueSecs = {1}", socketUrl, timeOutValueSecs)));
                    m_SocketComs = new SocketsController(socketUrl, timeOutValueSecs, _controllers);
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(string.Format("socket Comms are set")));

                    SubscribeToSocketEvents();
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(string.Format("socket events are subscribed to")));

                    m_SocketComs.Initialize();
                }
                catch (Exception ex)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Initializing Doshii failed, there was an exception that was {0}", ex.ToString()));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// formats the base URL for use in the sdk. 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        internal virtual string FormatBaseUrl(string baseUrl)
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
		/// Appends the supplied venue token to the supplied socket URL. 
		/// </summary>
		/// <param name="baseApiUrl">The base URL for the API. This is an HTTP address that points to the Doshii POS API, including version.</param>
		/// <param name="token">The Doshii authentication token for the POS implementation in the API.</param>
		/// <returns>The URL for the web socket connection in Doshii.</returns>
		internal virtual string BuildSocketUrl(string socketUrl, string token)
		{
			// finally append the socket endpoint and token parameter to the url and return the result
            return String.Format("{0}?token={1}", socketUrl, token);
        }

        /// <summary>
        /// Subscribes to the socket communication events 
        /// </summary>
        internal virtual void SubscribeToSocketEvents()
        {
            if (m_SocketComs == null)
            {
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: The socketComs has not been initialized");
                throw new NotSupportedException("m_SocketComms is null");
            }
            else
            {
                UnsubscribeFromSocketEvents();
				_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, "Doshii: Subscribing to socket events");
                m_SocketComs.OrderCreatedEvent += new SocketsController.OrderCreatedEventHandler(SocketComsOrderCreatedEventHandler);
                m_SocketComs.TransactionCreatedEvent += new SocketsController.TransactionCreatedEventHandler(SocketComsTransactionCreatedEventHandler);
                m_SocketComs.TransactionUpdatedEvent += new SocketsController.TransactionUpdatedEventHandler(SocketComsTransactionUpdatedEventHandler);
				m_SocketComs.SocketCommunicationEstablishedEvent += new SocketsController.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
                m_SocketComs.SocketCommunicationTimeoutReached += new SocketsController.SocketCommunicationTimeoutReachedEventHandler(SocketComsTimeOutValueReached);
                m_SocketComs.MemberCreatedEvent += new SocketsController.MemberCreatedEventHandler(SocketComsMemberCreatedEventHandler);
                m_SocketComs.MemberUpdatedEvent += new SocketsController.MemberUpdatedEventHandler(SocketComsMemberUpdatedEventHandler);
                m_SocketComs.BookingCreatedEvent += new SocketsController.BookingCreatedEventHandler(SocketComsBookingCreatedEventHandler);
                m_SocketComs.BookingUpdatedEvent += new SocketsController.BookingUpdatedEventHandler(SocketComsBookingUpdatedEventHandler);
                m_SocketComs.BookingDeletedEvent += new SocketsController.BookingDeletedEventHandler(SocketComsBookingDeletedEventHandler);
            }
        }

        /// <summary>
        /// Unsubscribes from the socket communication events 
        /// </summary>
        internal virtual void UnsubscribeFromSocketEvents()
        {
			_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, "Doshii: Unsubscribing from socket events");
            m_SocketComs.OrderCreatedEvent -= new SocketsController.OrderCreatedEventHandler(SocketComsOrderCreatedEventHandler);
            m_SocketComs.TransactionCreatedEvent -= new SocketsController.TransactionCreatedEventHandler(SocketComsTransactionCreatedEventHandler);
            m_SocketComs.TransactionUpdatedEvent -= new SocketsController.TransactionUpdatedEventHandler(SocketComsTransactionUpdatedEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent -= new SocketsController.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
            m_SocketComs.SocketCommunicationTimeoutReached -= new SocketsController.SocketCommunicationTimeoutReachedEventHandler(SocketComsTimeOutValueReached);
            m_SocketComs.MemberCreatedEvent -= new SocketsController.MemberCreatedEventHandler(SocketComsMemberCreatedEventHandler);
            m_SocketComs.MemberUpdatedEvent -= new SocketsController.MemberUpdatedEventHandler(SocketComsMemberUpdatedEventHandler);
            m_SocketComs.BookingCreatedEvent -= new SocketsController.BookingCreatedEventHandler(SocketComsBookingCreatedEventHandler);
            m_SocketComs.BookingUpdatedEvent -= new SocketsController.BookingUpdatedEventHandler(SocketComsBookingUpdatedEventHandler);
            m_SocketComs.BookingDeletedEvent -= new SocketsController.BookingDeletedEventHandler(SocketComsBookingDeletedEventHandler);
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
			_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            try
            {
                _controllers.OrderingController.RefreshAllOrders();
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: There was an exception while trying to retrieve all the orders from Doshii.", ex);
            }
            
        }
        
        /// <summary>
        /// Handles a socket communication timeOut event - this is when there has not been successful communication with doshii within the specified timeout period. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTimeOutValueReached(object sender, EventArgs e)
        {
			_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, 
				"Doshii: SocketComsTimeoutValueReached");
        }

        /// <summary>
        /// Handles a SocketComs_OrderStatusEvent, 
        /// Records the Order.UpdatedAt value and calls the appropriate method on the OrderingInterface to act on the check. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsOrderCreatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderCreatedEventArgs e)
        {
			if (!String.IsNullOrEmpty(e.Order.Id))
            {
                _controllers.LoggingController.LogMessage(this.GetType(),DoshiiLogLevels.Fatal, "A preexisting order was passed to the order created event handler.");
                throw new NotSupportedException("Developer Error, An order with a posId was passed to the CreatedOrderEventHandler");
            }
            _controllers.OrderingController.HandleOrderCreated(e.Order, e.TransactionList.ToList());
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
			_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: received a transaction status event with status '{0}', for transaction Id '{1}', for order Id '{2}'", e.Transaction.Status, e.TransactionId, e.Transaction.OrderId));
            switch (e.Transaction.Status)
            {
                case "pending":
                    _controllers.TransactionController.HandelPendingTransactionReceived(e.Transaction);
                    break;
                default:
					_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a create transaction message was received for a transaction which state was not pending, Transaction status - '{0}'", e.Transaction.Status)); 
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API",e.Transaction.Status));
            }
		}

        /// <summary>
        /// Handles a SocketComs.MemberCreatedEvent, 
        /// Calls the appropriate method on the RewardInterface to create the member on the pos. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsMemberCreatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.MemberEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: received a member created event with member Id '{0}'", e.MemberId));
            try
            {
                _controllers.RewardManager.CreateMemberOnPos(e.Member);
            }
            catch(MemberExistOnPosException ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create member with Id '{0}' on pos failed due to member already existing, now attempting to update existing member.", e.MemberId));
                try
                {
                    _controllers.RewardManager.UpdateMemberOnPos(e.Member);
                }
                catch (MemberDoesNotExistOnPosException nex)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update member with Id '{0}' on pos failed.", e.MemberId));
                }
            }
        }

        /// <summary>
        /// Handles a SocketComs.MemberUpdatedEvent, 
        /// Calls the appropriate method on the RewardInterface to update the member on the pos. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsMemberUpdatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.MemberEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: received a member updated event for member Id '{0}'", e.MemberId));
            try
            {
                _controllers.RewardManager.UpdateMemberOnPos(e.Member);
            }
            catch (MemberDoesNotExistOnPosException ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update member with Id '{0}' on pos failed due to member not currently existing, now attempting to create existing member.", e.MemberId));
                try
                {
                    _controllers.RewardManager.CreateMemberOnPos(e.Member);
                }
                catch (MemberExistOnPosException nex)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create member with Id '{0}' on pos failed", e.MemberId));
                }
            }
        }

        /// <summary>
        /// Handles a SocketComs.BookingCreatedEvent, 
        /// Calls the appropriate method on the ReservationInterface to create the booking on the pos. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsBookingCreatedEventHandler(object sender, BookingEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii:: received a booking created event for booking id '{0}'", e.BookingId));
            try
            {
                _controllers.ReservationManager.CreateBookingOnPos(e.Booking);
            }
            catch (BookingExistOnPosException bex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create booking with Id '{0}' on pos failed due to booking already existing, now attempting to update existing booking.", e.BookingId));
                try
                {
                    _controllers.ReservationManager.UpdateBookingOnPos(e.Booking);
                }
                catch (BookingDoesNotExistOnPosException nex)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update booking with Id '{0}' on pos failed.", e.BookingId));
                }
            }
        }

        /// <summary>
        /// Handles a SocketComs.BookingUpdatedEvent, 
        /// Calls the appropriate method on the ReservationInterface to update the booking on the pos. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsBookingUpdatedEventHandler(object sender, BookingEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii:: received a booking updated event for booking id '{0}'", e.BookingId));
            try
            {
                _controllers.ReservationManager.UpdateBookingOnPos(e.Booking);
            }
            catch (BookingDoesNotExistOnPosException bex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to update booking with Id '{0}' on pos failed due to booking already existing, now attempting to create new booking.", e.BookingId));
                try
                {
                    _controllers.ReservationManager.CreateBookingOnPos(e.Booking);
                }
                catch (BookingExistOnPosException nex)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to create booking with Id '{0}' on pos failed.", e.BookingId));
                }
            }
        }

        /// <summary>
        /// Handles a SocketComs.BookingDeletedEvent, 
        /// Calls the appropriate method on the ReservationInterface to Delete the booking on the pos. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsBookingDeletedEventHandler(object sender, BookingEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii:: received a booking deleted event for booking id '{0}'", e.BookingId));
            try
            {
                _controllers.ReservationManager.DeleteBookingOnPos(e.Booking);
            }
            catch (BookingDoesNotExistOnPosException bex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: attempt to delete booking with Id '{0}' on pos failed due to booking not existing.", e.BookingId));
            }

        }

        /// <summary>
        /// Handles a SocketComs.TransactionCreatedEvent, 
        /// Calls the appropriate method on the PaymentInterface to act on the transaction depending on the transaction status. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void SocketComsTransactionUpdatedEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TransactionEventArgs e)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: received a transaction status event with status '{0}', for transaction Id '{1}', for order Id '{2}'", e.Transaction.Status, e.TransactionId, e.Transaction.OrderId));
            
            switch (e.Transaction.Status)
            {
                case "pending":
                    _controllers.TransactionController.HandelPendingTransactionReceived(e.Transaction);
                    break;
                case "cancelled":
                    _controllers.TransactionManager.RecordTransactionVersion(e.Transaction.Id, e.Transaction.Version);
                    _controllers.TransactionManager.CancelPayment(e.Transaction);
                    break;
                case "complete":
                    _controllers.TransactionManager.RecordTransactionVersion(e.Transaction.Id, e.Transaction.Version);
                    break;
                default:
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a create transaction message was received for a transaction which state was not pending, Transaction status - '{0}'", e.Transaction.Status));
                    throw new NotSupportedException(string.Format("cannot process transaction with state {0} from the API", e.Transaction.Status));
            }
        }

        #endregion

        #region ordering And Transaction

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
        public virtual bool AcceptOrderAheadCreation(Order orderToAccept)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AcceptOrderAheadCreation"));
            }

            return _controllers.OrderingController.AcceptOrderAheadCreation(orderToAccept);
        }

        /// <summary>
        /// Call this method to reject an order created by an order ahead partner,
        /// </summary>
        /// <param name="orderToReject">
        /// The pending Doshii order that will be rejected
        /// </param>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        public virtual void RejectOrderAheadCreation(Order orderToReject)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RejectOrderAheadCreation"));
            }
            _controllers.OrderingController.RejectOrderAheadCreation(orderToReject);
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
        public virtual Transaction RecordPosTransactionOnDoshii(Transaction transaction)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RecordPosTransactionOnDoshii"));
            }
            return _controllers.TransactionController.RecordPosTransactionOnDoshii(transaction);
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
            return _controllers.OrderingController.GetOrder(orderId);
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
            return _controllers.ConsumerController.GetConsumerFromCheckinId(checkinId);
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
		    return _controllers.OrderingController.GetOrders();
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
            return _controllers.TransactionController.GetTransaction(transactionId);
        }

        /// <summary>
        /// This method returns a transaction from Doshii corresponding to the order with the doshiiOrderId
        /// <para/>This method should only be called in relation to unlinkedOrders as this method will not return transactions related to linked orders. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The DoshiiId of the order that is being requested. 
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
            return _controllers.TransactionController.GetTransactionFromDoshiiOrderId(doshiiOrderId);
            
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
		    return _controllers.TransactionController.GetTransactions();
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
            order.Version = _controllers.OrderingManager.RetrieveOrderVersion(order.Id);
            var jsonOrder = Mapper.Map<JsonOrder>(order);
			_controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", jsonOrder.ToJsonString()));
            
            var returnedOrder = new Order();

            return _controllers.OrderingController.UpdateOrder(order);
        }

        #endregion

        #region Membership

        /// <summary>
        /// The method retrieves the member from Doshii corresponding to the memberId parameter
        /// </summary>
        /// <param name="memberId">The memberId of the member to retrieve</param>
        /// <returns>The member corrosponding to the memberId parameter, NULL if there is no member associated with the memberId paramater on Doshii</returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        /// <exception cref="RestfulApiErrorResponseException">Where there is an exception making the request to Doshii.</exception>
        public virtual Member GetMember(string memberId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMember"));
            }
            return _controllers.RewardController.GetMember(memberId);
        }

        /// <summary>
        /// Returns all the Doshii members for the organization 
        /// </summary>
        /// <returns>
        /// IEnumerable of all the members registered in the orginisation. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual IEnumerable<Member> GetMembers()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMembers"));
            }
            if (_controllers.RewardManager == null)
            {
                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetMembers"));
            }
            return _controllers.RewardController.GetMembers();
        }

        /// <summary>
        /// Deleted the member from Doshii that was provided as the member paramater. 
        /// </summary>
        /// <param name="member"></param>
        /// <returns>
        /// True if the member was deleted, 
        /// False if there was a problem deleting the member. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool DeleteMember(Member member)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteMember"));
            }
            return _controllers.RewardController.DeleteMember(member);
        }

        /// <summary>
        /// updates the corresponding member on Doshii with the member provided in the member paramater if the member.Id prop is not empty.
        /// Creates a member on Doshii with the props provided in the member paramater if the member.Id prop is empty.  
        /// </summary>
        /// <param name="member">
        /// the member that should be created or updated. 
        /// </param>
        /// <returns></returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual Member UpdateMember(Member member)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            return _controllers.RewardController.UpdateMember(member);
        }

        /// <summary>
        /// The method compares the members on the pos with the members on Doshii, 
        /// This method will delete members on the pos that do not exist on Doshii, and update members on the pos that differ to the members on Doshii.  
        /// </summary>
        /// <returns>
        /// True if the action was successful, 
        /// False if the action was unsuccessful. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool SyncDoshiiMembersWithPosMembers()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMember"));
            }
            return _controllers.RewardController.SyncDoshiiMembersWithPosMembers();
        }

        /// <summary>
        /// This method will return all the rewards available for a member of the orginisation on Doshii.
        /// The following process is used to retreive the rewards.
        /// <list type="bullet">
        ///   <item>Retreives the order from the pos with the orderId and attempts to update the order on Doshii</item>
        ///   <item>Requests the rewards from Doshii with the provided memberId, and orderTotal.</item>
        /// </list>
        /// </summary>
        /// <param name="memberId">The Id of the member to request rewards for</param>
        /// <param name="orderId">The Id of the order that the member may use the requested rewards on</param>
        /// <param name="orderTotal">The current total of the order the member may use the rewards on</param>
        /// <returns></returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        /// <exception cref="RestfulApiErrorResponseException">Throw if these was an issue communicating with Doshii.</exception>
        public virtual IEnumerable<Reward> GetRewardsForMember(string memberId, string orderId, decimal orderTotal)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetRewardsForMember"));
            }
            if (_controllers.RewardManager == null)
            {
                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetRewardsForMember"));
            }
            return _controllers.RewardController.GetRewardsForMember(memberId, orderId, orderTotal);
        }

        /// <summary>
        /// This method should be called to confirm that the reward is still available for the member and that the reward can be redeemed against the order.
        /// after calling this method successfully the pas must 
        /// <list type="bullet">
        ///   <item>apply the reward to the order</item>
        ///   <item>accept payment for the order from the customer.</item>
        ///   <item>call <see cref="RedeemRewardForMemberConfirm"/> to confirm the use of the redard</item>
        /// </list>
        /// </summary>
        /// <param name="member">The member attempting to redeem the reward.</param>
        /// <param name="reward">The rewards the member is attempting to redeem.</param>
        /// <param name="order">The order the reward should be applied to.</param>
        /// <returns>
        /// true - when the reward can be redeemed - the pos must  
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemRewardForMember(Member member, Reward reward, Order order)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMember"));
            }
            return _controllers.RewardController.RedeemRewardForMember(member, reward, order);
        }

        /// <summary>
        /// Use this method to cancel the redemption of a reward when the reward redemption process has already progressed passed a call to <see cref="RedeemRewardForMember"/> 
        /// </summary>
        /// <param name="memberId">the member Id of the member previously redeeming the reward.</param>
        /// <param name="rewardId">The reward Id of the reward that needs to be canceled</param>
        /// <param name="cancelReason">The reason the reward redemption has been canceled.</param>
        /// <returns>
        /// True if the redemption was successfully canceled
        /// False if the redemption was not successfully canceled. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberCancel"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberCancel"));
            }
            return _controllers.RewardController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);
        }

        /// <summary>
        /// This method is use to confirm the use of a reward by a member.
        /// This is the last step in redeeming the reward. 
        /// If this method returns true, you can no longer cancel the redemption of the reward. 
        /// </summary>
        /// <param name="memberId">
        /// the Id of the member redeeming the reward
        /// </param>
        /// <param name="rewardId">
        /// the Id of the reward to be redeemed
        /// </param>
        /// <returns>
        /// True if the reward was successfully redeemed
        /// False if the reward was not successfully redeemed. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberConfirm"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemRewardForMemberConfirm"));
            }
            return _controllers.RewardController.RedeemRewardForMemberConfirm(memberId, rewardId);
        }

        /// <summary>
        /// This method should be called to confirm that the points the member is attempting to redeem are still available. 
        /// after calling this method successfully the pas must 
        /// <list type="bullet">
        ///   <item>apply the points to the order</item>
        ///   <item>accept payment for the order from the customer.</item>
        ///   <item>call <see cref="RedeemPointsForMemberConfirm"/> to confirm the use of the points</item>
        /// </list>
        /// </summary>
        /// <param name="member">the member that would like to redeem points.</param>
        /// <param name="app">The app that has the points that the member wishes to redeem</param>
        /// <param name="order">The order that the member wishes to redeem the points against.</param>
        /// <param name="points">The amount of points the member wishes to redeem</param>
        /// <returns>
        /// True if the points are still available to redeem
        /// False if the points are no longer available to redeem. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemPointsForMember(Member member, App app, Order order, int points)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMember"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMember"));
            }
            return _controllers.RewardController.RedeemPointsForMember(member, app, order, points);
        }

        /// <summary>
        /// This method is use to confirm the use of a points by a member.
        /// This is the last step in redeeming the points. 
        /// If this method returns true, you can no longer cancel the redemption of the points. 
        /// </summary>
        /// <param name="memberId">
        /// The member that is redeeming the points.
        /// </param>
        /// <returns>
        /// True is the points were redeemed,
        /// False if the points redemption failed. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemPointsForMemberConfirm(string memberId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberConfirm"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberConfirm"));
            }
            return _controllers.RewardController.RedeemPointsForMemberConfirm(memberId);
        }

        /// <summary>
        /// This method is use to cancel the redemption of member points after a call to <see cref="RedeemPointsForMember"/> has been made. 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        /// <exception cref="DoshiiMembershipManagerNotInitializedException">Thrown when the <see cref="IRewardManager"/> was not implemented by the pos.</exception>
        public virtual bool RedeemPointsForMemberCancel(string memberId, string cancelReason)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberCancel"));
            }
            if (_controllers.RewardManager == null)
            {

                ThrowDoshiiMembershipNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "RedeemPointsForMemberCancel"));
            }
            return _controllers.RewardController.RedeemPointsForMemberCancel(memberId, cancelReason);
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
        public virtual bool SetTableAllocationWithoutCheckin(string posOrderId, List<string> tableNames, int covers)
		{
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

		    return _controllers.TableController.SetTableAllocationWithoutCheckin(posOrderId, tableNames, covers);
		}

        /// <summary>
        /// This method is used to modify the table allocation of a checkin at the venue, 
        /// If you wish to un-allocate the checkin from a table without allocating it to another table you should pass an empty list to the tableNames parameter.
        /// </summary>
        /// <param name="checkinId">The checkinId to be modified</param>
        /// <param name="tableNames">The table names to be allocated to the checkin</param>
        /// <param name="covers">The amount of covers associated with the allocation.</param>
        /// <returns>
        /// True if the table allocation was successful
        /// False if the allocation change was not successful.
        /// </returns>
        public virtual bool ModifyTableAllocation(string checkinId, List<string> tableNames, int covers)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

            return _controllers.TableController.ModifyTableAllocation(checkinId, tableNames, covers);
        }

        /// <summary>
        /// this method should be called when a checkin has finished at the venue, eg the checkins check has been completly paid. 
        /// </summary>
        /// <param name="checkinId">
        /// The checkinId that represents the checkin at the venue. 
        /// </param>
        /// <returns></returns>
        public virtual bool CloseCheckin(string checkinId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "AddTableAllocation"));
            }

            return _controllers.CheckinController.CloseCheckin(checkinId);
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
        public virtual Menu UpdateMenu(Menu menu)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateMenu"));
            }
            return _controllers.MenuController.UpdateMenu(menu);
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
        public virtual Surcount UpdateSurcount(Surcount surcount)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateSurcount"));
            }
            return _controllers.MenuController.UpdateSurcount(surcount);
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
        public virtual Product UpdateProduct(Product product)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateProduct"));
            }
            return _controllers.MenuController.UpdateProduct(product);
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
        public virtual bool DeleteSurcount(string posId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteSurcount"));
            }
            return _controllers.MenuController.DeleteSurcount(posId);
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
        public virtual bool DeleteProduct(string posId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteProduct"));
            }
            return _controllers.MenuController.DeleteProduct(posId);
        }


        #endregion

        #region Tables

        /// <summary>
        /// gets the table registered at the venue in doshii with the provided tableName
        /// </summary>
        /// <param name="tableName">
        /// The name of the table that should be returned from doshii
        /// </param>
        /// <returns>
        /// The table as it is registered in Doshii, 
        /// Null if there is no table registered in doshii for that name. 
        /// </returns>
        public virtual Table GetTable(string tableName)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTable"));
            }
            return _controllers.TableController.GetTable(tableName);
        }

        /// <summary>
        /// gets all the tables registered at the venue in Doshii
        /// </summary>
        /// <returns>
        /// All the tables registered at the venue in Doshii. 
        /// </returns>
        public virtual List<Table> GetTables()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetTables"));
            }
            return _controllers.TableController.GetTables();
        }

        /// <summary>
        /// creates a table on Doshii
        /// </summary>
        /// <param name="table">
        /// The table that should be created in Doshii
        /// </param>
        /// <returns>
        /// The table that was created in Doshii. 
        /// </returns>
        public virtual Table CreateTable(Table table)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "CreateTable"));
            }
            return _controllers.TableController.CreateTable(table);
        }

        /// <summary>
        /// updates a table for the venue on Doshii
        /// </summary>
        /// <param name="table">
        /// the table that should be updated. 
        /// </param>
        /// <param name="oldTableName">
        /// The table name before the table was updated if the table name was changed, or the current table name if the name was not changed. 
        /// </param>
        /// <returns>
        /// The updated table from Doshii
        /// </returns>
        public virtual Table UpdateTable(Table table, string oldTableName)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "UpdateTable"));
            }
            return _controllers.TableController.UpdateTable(table, oldTableName);
        }

        /// <summary>
        /// Deletes a table from Doshii
        /// </summary>
        /// <param name="tableName">
        /// The name of the table that will be deleted. 
        /// </param>
        /// <returns>
        /// The table that was deleted. 
        /// </returns>
        public virtual Table DeleteTable(string tableName)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "DeleteTable"));
            }
            return _controllers.TableController.DeleteTable(tableName);
        }

        /// <summary>
        /// This method will delete all the current tables on Doshii and replace it with the tables provided in the tableList paramater. 
        /// </summary>
        /// <param name="tableList">The list of tables that are to be saved on Doshii</param>
        /// <returns>
        /// The current list of tables in Doshii
        /// </returns>
        public virtual List<Table> ReplaceTableListOnDoshii(List<Table> tableList)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "ReplaceTableListOnDoshii"));
            }
            return _controllers.TableController.ReplaceTableListOnDoshii(tableList);
        }

        #endregion

        #region Reservations and Bookings

        /// <summary>
        /// This method is used to get a booking for a specific id.
        /// </summary>
        /// <param name="bookingId">
        /// The booking Id for the requested booking
        /// </param>
        /// <returns>
        /// The requested booking from Doshii if it exists. 
        /// Null if the booking does not exist. 
        /// </returns>
        public virtual Booking GetBooking(String bookingId)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(), "GetBooking"));
            }
            return _controllers.ReservationController.GetBooking(bookingId);
        }

        /// <summary>
        /// This method is used to get all bookings within a specified date range.
        /// </summary>
        /// <param name="from">
        /// The start of the date range
        /// </param>
        /// <param name="to">
        /// The end of the date range. 
        /// </param>
        /// <returns></returns>
        public virtual List<Booking> GetBookings(DateTime from, DateTime to)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(), "GetBookings"));
            }
            return _controllers.ReservationController.GetBookings(from, to);
        }

          /// <summary>
        /// This method is used by the POS to seat a checkin with a booking.
          /// </summary>
          /// <param name="bookingId">the id of the booking to be seated</param>
          /// <param name="checkin">the checkin that should be associated with the booking</param>
          /// <param name="posOrderId">the posOrderId for the booking that will be seated, this can be NULL if there is no order associated with the table.</param>
          /// <returns>True if the booking was seated.</returns>
        public bool SeatBooking(String bookingId, Checkin checkin, String posOrderId = null)
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "SeatBooking"));
            }
            return _controllers.ReservationController.SeatBooking(bookingId, checkin, posOrderId);
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
        public virtual Location GetLocation()
        {
            if (!m_IsInitalized)
            {
                ThrowDoshiiManagerNotInitializedException(string.Format("{0}.{1}", this.GetType(),
                    "GetLocation"));
            }
            try
            {
                return _httpComs.GetLocation();
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
        internal virtual void ThrowDoshiiManagerNotInitializedException(string methodName)
        {
            throw new DoshiiManagerNotInitializedException(
                string.Format("You must initialize the DoshiiController instance before calling {0}", methodName));
        }

        private void ThrowDoshiiMembershipNotInitializedException(string methodName)
        {
            throw new DoshiiMembershipManagerNotInitializedException(
                string.Format("You must initialize the DoshiiMembership module before calling {0}", methodName));
        }

        

        #region IDisposable Members

        /// <summary>
		/// Cleanly disposes of the memory allocated to the instance's member variables.
		/// </summary>
		public virtual void Dispose()
		{
			_controllers.TransactionManager = null;
			_controllers.LoggingController.Dispose();
			_controllers.LoggingController = null;
		}

		#endregion

		
	}
}
