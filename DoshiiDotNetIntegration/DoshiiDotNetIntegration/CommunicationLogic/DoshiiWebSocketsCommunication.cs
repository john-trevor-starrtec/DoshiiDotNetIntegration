using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// faciliates the web sockets communication with the doshii application
    /// </summary>
    internal class DoshiiWebSocketsCommunication 
    {

        #region fields and properties

        /// <summary>
        /// web socket object that will handle all the communications with doshii
        /// </summary>
        WebSocket m_WebSocketsConnection = null;

        /// <summary>
        /// field to indicate if the socket connection was successfully opened. 
        /// </summary>
        private bool m_SocketsConnectedSuccessfully = false;

        /// <summary>
        /// a doshii logic instance
        /// </summary>
        private DoshiiOperationLogic m_DoshiiLogic;

        /// <summary>
        /// A thread to send heartbeat socketmessage to doshii. 
        /// </summary>
        private Thread m_HeartBeatThread;

        /// <summary>
        /// this is used to hold the timeout value for the scoket connection being unable to connect to the server.
        /// </summary>
        private int m_SocketConnectionTimeOutValue;

        /// <summary>
        /// this is used to calculate if the last successfull cosket connection is within the timeOut range. 
        /// </summary>
        private DateTime m_LastSuccessfullSocketMessageTime;

        #endregion

        #region internal methods and events

        #region events

        internal delegate void CreatedOrderEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        /// <summary>
        /// event will be raised when a new order is created on doshii
        /// </summary>
        internal event CreatedOrderEventHandler CreateOrderEvent;
        
        internal delegate void OrderStatusEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        /// <summary>
        /// event will be raised when the state of an order has changed through doshii
        /// </summary>
        internal event OrderStatusEventHandler OrderStatusEvent;
        
        internal delegate void ConsumerCheckInEventHandler(object sender, CommunicationEventArgs.CheckInEventArgs e);
        /// <summary>
        /// event will be raised when a consumer checks in through doshii
        /// </summary>
        internal event ConsumerCheckInEventHandler ConsumerCheckinEvent;
        
        internal delegate void TableAllocationEventHandler(object sender, CommunicationEventArgs.TableAllocationEventArgs e);
        /// <summary>
        /// event will be raised when a table allocaiton event occurs through doshii
        /// </summary>
        internal event TableAllocationEventHandler TableAllocationEvent;
        
        internal delegate void CheckOutEventHandler(object sender, CommunicationEventArgs.CheckOutEventArgs e);
        /// <summary>
        /// event will be raised when a consumer is checked out from doshii
        /// </summary>
        internal event CheckOutEventHandler CheckOutEvent;
        
        internal delegate void SocketCommunicationEstablishedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        internal event SocketCommunicationEstablishedEventHandler SocketCommunicationEstablishedEvent;

        internal delegate void SocketCommunicationTimeoutReachedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        internal event SocketCommunicationTimeoutReachedEventHandler SocketCommunicationTimeoutReached;

        #endregion

        #region methods

        /// <summary>
        /// constructor, the initialize method must be called after the constructor to initialize the connection
        /// </summary>
        /// <param name="webSocketUrl"></param>
        /// <param name="doshii"></param>
        internal DoshiiWebSocketsCommunication(string webSocketUrl, DoshiiOperationLogic doshii, int socketConnectionTimeOutValue)
        {
            if (doshii == null)
            {
                throw new NotSupportedException("doshii");
            }

            m_SocketConnectionTimeOutValue = socketConnectionTimeOutValue;
            m_DoshiiLogic = doshii;
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("instanciating doshiiwebSocketComunication with socketUrl - '{0}'", webSocketUrl));
                
            if (string.IsNullOrWhiteSpace(webSocketUrl))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("cannot create an instance of DoshiiWebSocketsCommunication with a blank socketUrl"));
                throw new NotSupportedException("webSocketUrl");
            }
            
            
            m_WebSocketsConnection = new WebSocket(webSocketUrl);
            m_WebSocketsConnection.OnOpen += new EventHandler(WebSocketsConnectionOnOpenEventHandler);
            m_WebSocketsConnection.OnClose += new EventHandler<CloseEventArgs>(WebSocketsConnectionOnCloseEventHandler);
            m_WebSocketsConnection.OnMessage += new EventHandler<MessageEventArgs>(WebSocketsConnectionOnMessageEventHandler);
            m_WebSocketsConnection.OnError += new EventHandler<ErrorEventArgs>(WebSocketsConnectionOnErrorEventHandler);

            
        }

        /// <summary>
        /// initalizes the webScoket communication with doshii
        /// this method should not be called until the events emitted from this class are subscribed to. 
        /// </summary>
        internal void Initialize()
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("initializing doshii websocket connection"));
            Connect();
            
            //wait to ensure successful socket connection
            Thread.Sleep(3000);

            if (!m_SocketsConnectedSuccessfully)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: could not extablish and socket connection"); 
                throw new EntryPointNotFoundException("socket connection could not be extablished with Doshii");
            }

            m_HeartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
            m_HeartBeatThread.Start();
        }

        /// <summary>
        /// attempts to open a websockets connection with doshii
        /// </summary>
        private void Connect()
        {
            if (m_WebSocketsConnection != null)
            {
                try
                {
                    m_WebSocketsConnection.Connect();
                }
                catch(Exception ex)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error initializing the web sockets conneciton to Doshii"), ex);
                }
                
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the ws object"));
            }
        }

        /// <summary>
        /// this should not be used for anything other than heartbeats as the pos does not communication with Doshii though web sockets, the pos uses http for sending messages to doshii. 
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(string message)
        {
            try
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Sending websockets message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()));
                m_WebSocketsConnection.Send(message);
            }
            catch (Exception ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: Exception while sending a webSocket message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()), ex);
            }
            
        }

        /// <summary>
        /// sends a heart beat message to doshii every ten seconds. 
        /// </summary>
        private void HeartBeatChecker()
        {
            while (true)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("webScoket heartBeat started"));
                Thread.Sleep(10000);
                TimeSpan thisTimeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
                double doubleForHeartbeat = thisTimeSpan.TotalMilliseconds;
                string message = string.Format("\"primus::ping::<{0}>\"", doubleForHeartbeat.ToString());
                SendMessage(message);
                if (!TestTimeOutValue())
                {
                    //raise event for isgnal that Timeout out is expired.
                    SocketCommunicationTimeoutReached(this, new EventArgs());
                }
            }
        }

        #endregion

        #endregion

        #region event handlers

        /// <summary>
        /// handles the webSocket onError event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebSocketsConnectionOnErrorEventHandler(object sender, ErrorEventArgs e)
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error with the websockets connection to {0} the error was (1)", m_WebSocketsConnection.Url.ToString(), e.Message));
            if (e.Message == "The WebSocket connection has already been closed.")
            {
                Initialize();
            }
        }

        /// <summary>
        /// handles the webSocket onMessage event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebSocketsConnectionOnMessageEventHandler(object sender, MessageEventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            DoshiiDotNetIntegration.Models.SocketMessage theMessage = new Models.SocketMessage();
            if (e.Type == Opcode.Text)
            {
                string messageString = e.Data.ToString();
                // drop heart beat responce
                if (messageString.Contains("primus::pong::<"))
                {
                    return;
                }
                else
                {
                    theMessage = DoshiiDotNetIntegration.Models.SocketMessage.deseralizeFromJson(messageString);
                }
                
            }

            if (e.Type == Opcode.Binary)
            {
                string messageString = e.RawData.ToString();
                // drop heartbeat message
                if (messageString.Contains("primus::pong::<"))
                {
                    return;
                }
                else
                {
                    theMessage = DoshiiDotNetIntegration.Models.SocketMessage.deseralizeFromJson(messageString);
                }
                
            }
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("WebScoket message received - '{0}'", theMessage.ToString()));
            dynamic dynamicSocketMessageData = theMessage.Emit[1];

            Models.SocketMessageData messageData = new Models.SocketMessageData();

            messageData.EventName = (string)theMessage.Emit[0];
            messageData.CheckinId = (string)dynamicSocketMessageData.checkinId;
            messageData.OrderId = (string)dynamicSocketMessageData.orderId;
            messageData.PaypalCustomerId = (string)dynamicSocketMessageData.paypalCustomerId;
            messageData.Status = (string)dynamicSocketMessageData.status;
            messageData.Name = (string)dynamicSocketMessageData.name;
            messageData.Id = (string)dynamicSocketMessageData.id;
            
            string uriString = (string)dynamicSocketMessageData.uri;
            if (!string.IsNullOrWhiteSpace(uriString))
            {
                messageData.Uri = new Uri((string)dynamicSocketMessageData.uri);
            }
            
            switch (messageData.EventName)
            {
                case "table_allocation":
                    CommunicationEventArgs.TableAllocationEventArgs allocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                    allocationEventArgs.TableAllocation = new Models.TableAllocation();

                    allocationEventArgs.TableAllocation.CustomerId = messageData.ConsumerId;
                    allocationEventArgs.TableAllocation.Id = messageData.Id;
                    allocationEventArgs.TableAllocation.Name = messageData.Name;
                    allocationEventArgs.TableAllocation.PaypalCustomerId = messageData.PaypalCustomerId;
                    if (messageData.Status == "waiting_for_confirmation")
                    {
                        allocationEventArgs.TableAllocation.Status = "waiting_for_confirmation";
                    }
                    else
                    {
                        allocationEventArgs.TableAllocation.Status = "confirmed";
                    }
                            
                    TableAllocationEvent(this, allocationEventArgs);
                    break;
                case "order_create":
                    CommunicationEventArgs.OrderEventArgs createOrderEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    createOrderEventArgs.Order = m_DoshiiLogic.GetOrder(messageData.OrderId);
                    createOrderEventArgs.OrderId = messageData.OrderId;
                    createOrderEventArgs.status = messageData.Status;    
                    CreateOrderEvent(this, createOrderEventArgs);
                    break;
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.Order = m_DoshiiLogic.GetOrder(messageData.OrderId);
                    orderStatusEventArgs.OrderId = messageData.OrderId;
                    orderStatusEventArgs.status = messageData.Status;    
                
                    OrderStatusEvent(this, orderStatusEventArgs);
                    break;
                case "consumer_checkin":
                    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                    newCheckinEventArgs.CheckIn = messageData.CheckinId;
                    newCheckinEventArgs.PaypalCustomerId = messageData.PaypalCustomerId;
                    newCheckinEventArgs.Uri = messageData.Uri;
                    newCheckinEventArgs.Consumer = m_DoshiiLogic.GetConsumer(messageData.PaypalCustomerId);
                    newCheckinEventArgs.Consumer.CheckInId = newCheckinEventArgs.CheckIn;

                    ConsumerCheckinEvent(this, newCheckinEventArgs);
                    break;
                case "consumer_checkout":
                    CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs();

                    checkOutEventArgs.ConsumerId = messageData.PaypalCustomerId;

                    CheckOutEvent(this, checkOutEventArgs);
                    break;
                default:
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Received socket message is not a supported message. messageType - '{0}'", messageData.EventName));
                    break;
            }
            
        }

        /// <summary>
        /// handles the webSocket onClose event, 
        /// automaticall tries to reconnect to doshii
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebSocketsConnectionOnCloseEventHandler(object sender, CloseEventArgs e)
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection to {0} closed", m_WebSocketsConnection.Url.ToString()));
            Initialize();
        }

        /// <summary>
        /// handles the webScoket onOpen event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebSocketsConnectionOnOpenEventHandler(object sender, EventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            m_SocketsConnectedSuccessfully = true;
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection successfully open to {0}", m_WebSocketsConnection.Url.ToString()));
            SocketCommunicationEstablishedEvent(this, e);
        }

        #endregion

        private void SetLastSuccessfullSocketCommunicationTime()
        {
            m_LastSuccessfullSocketMessageTime = DateTime.Now;
        }

        private bool TestTimeOutValue()
        {
            if (m_LastSuccessfullSocketMessageTime.AddSeconds((double)m_SocketConnectionTimeOutValue) < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
