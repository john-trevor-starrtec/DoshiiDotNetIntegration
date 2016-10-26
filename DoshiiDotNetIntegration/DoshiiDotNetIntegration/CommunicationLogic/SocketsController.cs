using AutoMapper;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Threading;
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Controllers;
using Microsoft.CSharp.RuntimeBinder;
using WebSocketSharp;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// This class is used internally by the SDK.
    /// This class manages the Web Socket communications between the pos and the Doshii API.
    /// When Instantiating this class you must;
    /// instantiate the class, 
    /// subscribe to the necessary events, 
    /// call initialize after calling the constructor if you wish to open a Socket connection with Doshii
    /// </summary>
    internal class SocketsController : IDisposable
    {

        #region fields and properties

        internal Models.Controllers _controllers { get; set; }
        
        
        /// <summary>
        /// Web socket object that will handle all the communications with doshii
        /// </summary>
		internal WebSocket _webSocketsConnection { get; set; }

        /// <summary>
        /// The thread that manages the socket heartBeat communication with Doshii API 
        /// </summary>
        internal Thread _heartBeatThread = null;

        /// <summary>
        /// This will hold the value for the last connected time so that there are not many connections established after the connection drops out. 
        /// </summary>
        internal DateTime _lastConnectionAttemptTime = DateTime.MinValue;

        /// <summary>
        /// this is used to hold the timeout value for the socket connection being unable to connect to the server.
        /// </summary>
        internal int _socketConnectionTimeOutValue { get; set; }

        /// <summary>
        /// this is used to calculate if the last successful socket connection is within the timeOut range. 
        /// </summary>
        internal DateTime _lastSuccessfullSocketMessageTime { get; set; }

		/// <summary>
		/// Callback to the POS logging mechanism.
		/// </summary>
		internal LoggingController _logger { get; private set; }

		/// <summary>
		/// string to add to the Ping message to send during heartbeat checks.
		/// </summary>
		private const string PingMessage = "primus::ping::";
        
        /// <summary>
        /// string to identify the Pong message received from Doshii during the heartBeat process. 
        /// </summary>
        private const string PongMessage = "primus::pong::";

        #endregion

        #region internal methods and events

        #region events

		internal delegate void OrderCreatedEventHandler(object sender, CommunicationEventArgs.OrderCreatedEventArgs e);
        
        /// <summary>
        /// Event will be raised when the state of an order created message is received from Doshii.
        /// </summary>
        internal event OrderCreatedEventHandler OrderCreatedEvent;

        internal delegate void TransactionCreatedEventHandler(object sender, CommunicationEventArgs.TransactionEventArgs e);
        
        /// <summary>
        /// Event will be raised when a transaction created message is received from Doshii
        /// </summary>
        internal virtual event TransactionCreatedEventHandler TransactionCreatedEvent;

        internal delegate void TransactionUpdatedEventHandler(object sender, CommunicationEventArgs.TransactionEventArgs e);
        
        /// <summary>
        /// Event will be raised when the state of a transaction is updated through doshii. 
        /// </summary>
        internal virtual event TransactionUpdatedEventHandler TransactionUpdatedEvent;

        internal delegate void MemberUpdatedEventHandler(object sender, CommunicationEventArgs.MemberEventArgs e);

        /// <summary>
        /// Event will be raised when a member updated through doshii. 
        /// </summary>
        internal virtual event MemberUpdatedEventHandler MemberUpdatedEvent;

        internal delegate void MemberCreatedEventHandler(object sender, CommunicationEventArgs.MemberEventArgs e);

        internal delegate void BookingCreatedEventHandler(object sender, CommunicationEventArgs.BookingEventArgs e);

        /// <summary>
        /// Event will be raised when a booking is created through doshii.
        /// </summary>
        internal virtual event BookingCreatedEventHandler BookingCreatedEvent;

        internal delegate void BookingUpdatedEventHandler(object sender, CommunicationEventArgs.BookingEventArgs e);

        /// <summary>
        /// Event will be raised when a booking is updated through doshii.
        /// </summary>
        internal virtual event BookingUpdatedEventHandler BookingUpdatedEvent;

        internal delegate void BookingDeletedEventHandler(object sender, CommunicationEventArgs.BookingEventArgs e);

        /// <summary>
        /// Event will be raised when a booking is deleted through doshii.
        /// </summary>
        internal virtual event BookingDeletedEventHandler BookingDeletedEvent;
        /// <summary>
        /// Event will be raised when a member is created through doshii. 
        /// </summary>
        internal virtual event MemberCreatedEventHandler MemberCreatedEvent;

        internal delegate void SocketCommunicationEstablishedEventHandler(object sender, EventArgs e);
        
        /// <summary>
        /// Event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        internal event SocketCommunicationEstablishedEventHandler SocketCommunicationEstablishedEvent;

        internal delegate void SocketCommunicationTimeoutReachedEventHandler(object sender, EventArgs e);
        
        /// <summary>
        /// Event will be raised when the socket communication with doshii times out. 
        /// </summary>
        internal event SocketCommunicationTimeoutReachedEventHandler SocketCommunicationTimeoutReached;

        #endregion

        #region methods

        /// <summary>
        /// Closes the socket connection
        /// </summary>
        internal virtual void CloseSocketConnection()
        {
            _webSocketsConnection.Close();
        }

        /// <summary>
        /// constructor, 
        /// The initialize method must be called after the constructor to initialize the connection
        /// </summary>
        /// <param name="webSocketUrl">
        /// The Url for the WebSockets connection
        /// </param>
		/// <param name="socketConnectionTimeOutValue">
		/// The timeout value before the SDK aleart the pos that the connection with Doshii has been lost. 
		/// </param>
        /// <param name="doshii">
        /// A <see cref="DoshiiController"/> instance. 
        /// </param>
		/// <param name="logManager">
		/// A <see cref="LoggingController"/> instance.
		/// </param>
        internal SocketsController(string webSocketUrl, int socketConnectionTimeOutValue, Models.Controllers controllers)
        {
            if (controllers.OrderingController == null)
            {
                throw new ArgumentNullException("orderingController");
            }
            if (controllers.TransactionController == null)
            {
                throw new ArgumentNullException("transactionController");
            }

			if (controllers.LoggingController == null)
			{
				throw new ArgumentNullException("logManager");
			}
            _controllers = controllers;
			_socketConnectionTimeOutValue = socketConnectionTimeOutValue;
            _logger = controllers.LoggingController;
           
            if (socketConnectionTimeOutValue < 10)
            {
				_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The socketConnectionTimeOutValue is set to less than 10 seconds, Times smaller than 10 seconds are not supported."));
				throw new ArgumentException("SocketConnectionTimeOutValue is less than 10");
            }

			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Info, string.Format("instantiating doshiiwebSocketComunication with socketUrl - '{0}'", webSocketUrl));
                
            if (string.IsNullOrWhiteSpace(webSocketUrl))
            {
				_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("cannot create an instance of SocketsController with a blank socketUrl"));
				throw new ArgumentException("webSocketUrl");
            }
                        
            _webSocketsConnection = new WebSocket(webSocketUrl);
            _webSocketsConnection.OnOpen += new EventHandler(WebSocketsConnectionOnOpenEventHandler);
            _webSocketsConnection.OnClose += new EventHandler<CloseEventArgs>(WebSocketsConnectionOnCloseEventHandler);
            _webSocketsConnection.OnMessage += new EventHandler<MessageEventArgs>(WebSocketsConnectionOnMessageEventHandler);
            _webSocketsConnection.OnError += new EventHandler<ErrorEventArgs>(WebSocketsConnectionOnErrorEventHandler);
        }

        /// <summary>
        /// Initializes the webScoket communication with doshii
        /// This method should not be called until the events emitted from this class are subscribed to. 
        /// </summary>
        internal virtual void Initialize()
        {
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("initializing doshii web-socket connection"));
            Connect();
        }

        /// <summary>
        /// Starts the heart beat thread to ensure the sockets connection is kept alive. 
        /// </summary>
        internal virtual void StartHeartbeatThread()
        {
            if (_heartBeatThread == null)
            {
                _heartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
                _heartBeatThread.IsBackground = true;
            }
            if (!_heartBeatThread.IsAlive)
            {
                _heartBeatThread.Start();
            }
            
        }

        /// <summary>
        /// Sets the last time a connection attempt was made. 
        /// </summary>
        internal virtual void SetLastConnectionAttemptTime()
        {
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Setting last connection attempt time: {0}", DateTime.Now.ToString())); 
            _lastConnectionAttemptTime = DateTime.Now;
        }

        /// <summary>
        /// Attempts to open a web-sockets connection with doshii
        /// </summary>
        internal virtual void Connect()
        {
            if (_webSocketsConnection != null && !_webSocketsConnection.IsAlive)
            {
                try
                {
                    if ((DateTime.Now.AddSeconds(-10) > _lastConnectionAttemptTime))
                    {
                        SetLastConnectionAttemptTime();
						_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Attempting Socket connection")); 
                        _webSocketsConnection.Connect();
                    }
                    
                }
                catch(Exception ex)
                {
					_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, "Doshii: There was an error initializing the web sockets connection to Doshii", ex);
                }
                
            }
            else
            {
				_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the was object"));
            }
        }

        /// <summary>
        /// This should NOT BE USED for anything other than heartbeats as the POS does not communication with Doshii though web sockets. 
        /// </summary>
        /// <param name="message">
        /// The message to be sent to Doshii
        /// </param>
		/// <returns>True on successful send; false otherwise.</returns>
        internal virtual bool SendMessage(string message)
        {
            try
            {
				_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Sending web-sockets message '{0}' to {1}", message, _webSocketsConnection.Url.ToString()));
                if (_webSocketsConnection.IsAlive)
                {
                    _webSocketsConnection.Send(message);
					return true;
                }
            }
            catch (Exception ex)
            {
				_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("Doshii: Exception while sending a webSocket message '{0}' to {1}", message, _webSocketsConnection.Url.ToString()), ex);
            }

			return false;
        }

        /// <summary>
        /// Sends a heart beat message to doshii every ten seconds. 
        /// </summary>
        internal virtual void HeartBeatChecker()
        {
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Starting web Socket heartbeat thread"));
            while (true)
            {
                Thread.Sleep(10000);
                if (!TestTimeOutValue())
                {
                    //raise event to signal that Timeout out is expired.
                    SocketCommunicationTimeoutReached(this, new EventArgs());
                }

                if (_webSocketsConnection.IsAlive)
                {
                    TimeSpan thisTimeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
                    double doubleForHeartbeat = thisTimeSpan.TotalMilliseconds;
                    string message = string.Format("\"{0}<{1}>\"", SocketsController.PingMessage, doubleForHeartbeat.ToString());
					if (SendMessage(message))
						SetLastSuccessfullSocketCommunicationTime();
                }
                else
                {
                    Initialize();
                }
            }
        }

        #endregion

        #endregion

        #region event handlers

        /// <summary>
        /// Handles the webSocket onError event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnErrorEventHandler(object sender, ErrorEventArgs e)
        {
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error with the web-sockets connection to {0} the error was {1}", _webSocketsConnection.Url.ToString(), e.Message));
            //StopHeartbeatThread();
            if (e.Message == "The WebSocket connection has already been closed.")
            {
                Initialize();
            }
        }

        /// <summary>
        /// Handles the webSocket onMessage event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnMessageEventHandler(object sender, MessageEventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            SocketMessage theMessage = new SocketMessage();
            if (e.Type == Opcode.Text)
            {
                string messageString = e.Data.ToString();
                // drop heart beat response
                if (messageString.Contains(String.Format("{0}", SocketsController.PongMessage)))
                {
					_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Received web-sockets message '{0}' from {1}", messageString, _webSocketsConnection.Url.ToString()));
                    return;
                }
                else
                {
                    theMessage = SocketMessage.deseralizeFromJson(messageString);
                }
                
            }

            if (e.Type == Opcode.Binary)
            {
                string messageString = e.RawData.ToString();
                // drop heartbeat message
                if (messageString.Contains(String.Format("{0}", SocketsController.PongMessage)))
                {
                    return;
                }
                else
                {
                    theMessage = SocketMessage.deseralizeFromJson(messageString);
                }
                
            }
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("WebScoket message received - '{0}'", theMessage.ToString()));
            ProcessSocketMessage(theMessage);

        }

        /// <summary>
        /// Processes the socket message received from Doshii and raises the appropriate events to alert subscribers. 
        /// </summary>
        /// <param name="theMessage">
        /// The socket message that was received. 
        /// </param>
        internal virtual void ProcessSocketMessage(SocketMessage theMessage)
        {
            dynamic dynamicSocketMessageData = theMessage.Emit[1];

            SocketMessageData messageData = new SocketMessageData();
            messageData.EventName = (string)theMessage.Emit[0];
            messageData.CheckinId = (string)dynamicSocketMessageData.checkinId;
            messageData.OrderId = (string)dynamicSocketMessageData.orderId;
            messageData.MeerkatConsumerId = (string)dynamicSocketMessageData.meerkatConsumerId;
            messageData.Status = (string)dynamicSocketMessageData.status;
            messageData.Name = (string)dynamicSocketMessageData.name;
            messageData.Id = (string)dynamicSocketMessageData.id;
            messageData.MemberId = (string)dynamicSocketMessageData.memberId;
            messageData.BookingId = (string)dynamicSocketMessageData.bookingId;
            messageData.Uri = (Uri)dynamicSocketMessageData.Uri;
            
            switch (messageData.EventName)
            {
                case "order_created":
                    var orderStatusEventArgs = new CommunicationEventArgs.OrderCreatedEventArgs();
                    orderStatusEventArgs.Order = _controllers.OrderingController.GetOrderFromDoshiiOrderId(messageData.Id);
                    if (orderStatusEventArgs.Order != null)
                    {
                        orderStatusEventArgs.TransactionList = _controllers.TransactionController.GetTransactionFromDoshiiOrderId(messageData.Id);
                        orderStatusEventArgs.OrderId = messageData.Id;
                        orderStatusEventArgs.Status = messageData.Status;

                        if (OrderCreatedEvent != null)
                        {
                            OrderCreatedEvent(this, orderStatusEventArgs);
                        }
                        else
                        {
                            _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the OrderCreateEvent"));
                        }
                    }
                    
                    break;
                case "transaction_created":
                    CommunicationEventArgs.TransactionEventArgs transactionCreatedEventArgs = new TransactionEventArgs();
                    transactionCreatedEventArgs.Transaction = _controllers.TransactionController.GetTransaction(messageData.Id);
                    transactionCreatedEventArgs.TransactionId = messageData.Id;
                    transactionCreatedEventArgs.Status = messageData.Status;
                    if (TransactionCreatedEvent != null)
                    {
                        TransactionCreatedEvent(this, transactionCreatedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the TransactionCreatedEvent"));
                    }
                    
                    break;
                case "transaction_updated":
                    CommunicationEventArgs.TransactionEventArgs transactionUpdtaedEventArgs = new TransactionEventArgs();
                    transactionUpdtaedEventArgs.Transaction = _controllers.TransactionController.GetTransaction(messageData.Id);
                    transactionUpdtaedEventArgs.TransactionId = messageData.Id;
                    transactionUpdtaedEventArgs.Status = messageData.Status;

                    if (TransactionUpdatedEvent != null)
                    {
                        TransactionUpdatedEvent(this, transactionUpdtaedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the TransactionUpdatedEvent"));
                    }
                    
                    break;
                case "member_created":
                    CommunicationEventArgs.MemberEventArgs memberCreatedEventArgs = new MemberEventArgs();
                    try
                    {
                        memberCreatedEventArgs.Member = _controllers.RewardController.GetMember(messageData.MemberId);
                        memberCreatedEventArgs.MemberId = messageData.MemberId;
                    }
                    catch
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The Pos is receiving member updates but the Member module has not been initialized."));
                    }
                    

                    if (MemberCreatedEvent != null)
                    {
                        MemberCreatedEvent(this, memberCreatedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the memberCreatedEventArgs"));
                    }

                    break;
                case "member_updated":
                    CommunicationEventArgs.MemberEventArgs memberUpdatedEventArgs = new MemberEventArgs();
                    try
                    {
                        memberUpdatedEventArgs.Member = _controllers.RewardController.GetMember(messageData.MemberId);
                        memberUpdatedEventArgs.MemberId = messageData.MemberId;
                    }
                    catch
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The Pos is receiving member updates but the Member module has not been initialized."));
                    }

                    if (MemberUpdatedEvent != null)
                    {
                        MemberUpdatedEvent(this, memberUpdatedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the TransactionUpdatedEvent"));
                    }

                    break;
                case "booking_created":
                    CommunicationEventArgs.BookingEventArgs bookingCreatedEventArgs = new BookingEventArgs();
                    try
                    {
                        bookingCreatedEventArgs.Booking = _controllers.ReservationController.GetBooking(messageData.BookingId);
                        bookingCreatedEventArgs.BookingId = messageData.BookingId;
                    }
                    catch
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The Pos is receiving booking updates but the Reservation module has not been initialized."));
                    }
                    if (BookingCreatedEvent != null)
                    {
                        BookingCreatedEvent(this, bookingCreatedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the BookingCreatedEvent"));
                    }
                    break;
                case "booking_updated":
                    CommunicationEventArgs.BookingEventArgs bookingUpdatedEventArgs = new BookingEventArgs();
                    try
                    {
                        bookingUpdatedEventArgs.Booking = _controllers.ReservationController.GetBooking(messageData.BookingId);
                        bookingUpdatedEventArgs.BookingId = messageData.BookingId;
                    }
                    catch
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The Pos is receiving booking updates but the Reservation module has not been initialized."));
                    }
                    if (BookingUpdatedEvent != null)
                    {
                        BookingUpdatedEvent(this, bookingUpdatedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the BookingUpdatedEvent"));
                    }
                    break;
                case "booking_deleted":
                    CommunicationEventArgs.BookingEventArgs bookingDeletedEventArgs = new BookingEventArgs();
                    try
                    {
                        bookingDeletedEventArgs.Booking = _controllers.ReservationController.GetBooking(messageData.BookingId);
                        bookingDeletedEventArgs.BookingId = messageData.BookingId;
                    }
                    catch
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("The Pos is receiving booking updates but the Reservation module has not been initialized."));
                    }
                    if (BookingDeletedEvent != null)
                    {
                        BookingDeletedEvent(this, bookingDeletedEventArgs);
                    }
                    else
                    {
                        _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Error, string.Format("no subscriber has subscribed to the BookingDeletedEvent"));
                    }
                    break;
                default:
                    _logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Received socket message is not a supported message. messageType - '{0}'", messageData.EventName));
                    break;
            }
        }

        /// <summary>
        /// Handles the webSocket onClose event, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnCloseEventHandler(object sender, CloseEventArgs e)
        {
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebSockets connection to {0} closed", _webSocketsConnection.Url.ToString()));
        }

        /// <summary>
        /// Handles the webScoket onOpen event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnOpenEventHandler(object sender, EventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
			_logger.LogMessage(typeof(SocketsController), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebSockets connection successfully open to {0}", _webSocketsConnection.Url.ToString()));
            StartHeartbeatThread();
            SocketCommunicationEstablishedEvent(this, e);
        }

        #endregion

        /// <summary>
        /// Sets the last time there was a successful sockets connection. 
        /// </summary>
        internal virtual void SetLastSuccessfullSocketCommunicationTime()
        {
            _lastSuccessfullSocketMessageTime = DateTime.Now;
        }

        /// <summary>
        /// Tests the time out value for the socket connection has not been reached.
        /// </summary>
        /// <returns></returns>
        internal virtual bool TestTimeOutValue()
        {
            if (_lastSuccessfullSocketMessageTime.AddSeconds((double)_socketConnectionTimeOutValue) < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		#region IDisposable Members

		/// <summary>
		/// Cleanly disposes of the instance and its member variables.
		/// </summary>
		public void Dispose()
		{
			_logger = null;
		    _controllers = null;
			_webSocketsConnection = null;
		}

		#endregion
	}
}
