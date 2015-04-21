﻿using System;
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
    public class DoshiiWebSocketsCommunication 
    {

        #region fields and properties

        /// <summary>
        /// web socket object that will handle all the communications with doshii
        /// </summary>
        public  WebSocket m_WebSocketsConnection = null;

        /// <summary>
        /// a doshii logic instance
        /// </summary>
        public  DoshiiOperationLogic m_DoshiiLogic;

        /// <summary>
        /// A thread to send heartbeat socketmessage to doshii. 
        /// </summary>
        public  Thread m_HeartBeatThread = null;

        /// <summary>
        /// This will hold the value for the last connected time so that there are not many connections established after the connection drops out. 
        /// </summary>
        public  DateTime m_LastConnectionAttemptTime = DateTime.MinValue;

        /// <summary>
        /// this is used to hold the timeout value for the scoket connection being unable to connect to the server.
        /// </summary>
        public  int m_SocketConnectionTimeOutValue;

        /// <summary>
        /// this is used to calculate if the last successfull cosket connection is within the timeOut range. 
        /// </summary>
        public  DateTime m_LastSuccessfullSocketMessageTime;

        #endregion

        #region public  methods and events

        #region events

        public  delegate void CreatedOrderEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        /// <summary>
        /// event will be raised when a new order is created on doshii
        /// </summary>
        public  event CreatedOrderEventHandler CreateOrderEvent;
        
        public  delegate void OrderStatusEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        /// <summary>
        /// event will be raised when the state of an order has changed through doshii
        /// </summary>
        public  event OrderStatusEventHandler OrderStatusEvent;
        
        public  delegate void ConsumerCheckInEventHandler(object sender, CommunicationEventArgs.CheckInEventArgs e);
        /// <summary>
        /// event will be raised when a consumer checks in through doshii
        /// </summary>
        public  event ConsumerCheckInEventHandler ConsumerCheckinEvent;
        
        public  delegate void TableAllocationEventHandler(object sender, CommunicationEventArgs.TableAllocationEventArgs e);
        /// <summary>
        /// event will be raised when a table allocaiton event occurs through doshii
        /// </summary>
        public  event TableAllocationEventHandler TableAllocationEvent;
        
        public  delegate void CheckOutEventHandler(object sender, CommunicationEventArgs.CheckOutEventArgs e);
        /// <summary>
        /// event will be raised when a consumer is checked out from doshii
        /// </summary>
        public  event CheckOutEventHandler CheckOutEvent;
        
        public  delegate void SocketCommunicationEstablishedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        public  event SocketCommunicationEstablishedEventHandler SocketCommunicationEstablishedEvent;

        public  delegate void SocketCommunicationTimeoutReachedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        public  event SocketCommunicationTimeoutReachedEventHandler SocketCommunicationTimeoutReached;

        #endregion

        #region methods

        public virtual void ClostSocketConnection()
        {
            m_WebSocketsConnection.Close();
        }

        /// <summary>
        /// constructor, the initialize method must be called after the constructor to initialize the connection
        /// </summary>
        /// <param name="webSocketUrl"></param>
        /// <param name="doshii"></param>
        public DoshiiWebSocketsCommunication(string webSocketUrl, DoshiiOperationLogic doshii, int socketConnectionTimeOutValue)
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
        public virtual void Initialize()
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("initializing doshii websocket connection"));
            Connect();
        }

        public virtual void StartHeartbeatThread()
        {
            if (m_HeartBeatThread == null)
            {
                m_HeartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
                m_HeartBeatThread.IsBackground = true;
            }
            if (!m_HeartBeatThread.IsAlive)
            {
                m_HeartBeatThread.Start();
            }
            
        }

        public virtual void SetLastConnectionAttemptTime()
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Setting last connection attempt time: {0}", DateTime.Now.ToString())); 
            m_LastConnectionAttemptTime = DateTime.Now;
        }

        /// <summary>
        /// attempts to open a websockets connection with doshii
        /// </summary>
        public virtual void Connect()
        {
            if (m_WebSocketsConnection != null && !m_WebSocketsConnection.IsAlive)
            {
                try
                {
                    if ((DateTime.Now.AddSeconds(-10) > m_LastConnectionAttemptTime))
                    {
                        SetLastConnectionAttemptTime();
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Attempting Socket connection")); 
                        m_WebSocketsConnection.Connect();
                    }
                    
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
        public virtual void SendMessage(string message)
        {
            try
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Sending websockets message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()));
                if (m_WebSocketsConnection.IsAlive)
                {
                    m_WebSocketsConnection.Send(message);
                }
            }
            catch (Exception ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: Exception while sending a webSocket message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()), ex);
            }
            
        }

        /// <summary>
        /// sends a heart beat message to doshii every ten seconds. 
        /// </summary>
        public virtual void HeartBeatChecker()
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Starting web Socket heartbeat thread"));
            while (true)
            {
                Thread.Sleep(10000);
                if (!TestTimeOutValue())
                {
                    //raise event for isgnal that Timeout out is expired.
                    SocketCommunicationTimeoutReached(this, new EventArgs());
                }
                if (m_WebSocketsConnection.IsAlive)
                {
                    TimeSpan thisTimeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
                    double doubleForHeartbeat = thisTimeSpan.TotalMilliseconds;
                    string message = string.Format("\"primus::ping::<{0}>\"", doubleForHeartbeat.ToString());
                    SendMessage(message);
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
        /// handles the webSocket onError event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void WebSocketsConnectionOnErrorEventHandler(object sender, ErrorEventArgs e)
        {
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error with the websockets connection to {0} the error was {1}", m_WebSocketsConnection.Url.ToString(), e.Message));
            //StopHeartbeatThread();
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
        public virtual void WebSocketsConnectionOnMessageEventHandler(object sender, MessageEventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            DoshiiDotNetIntegration.Models.SocketMessage theMessage = new Models.SocketMessage();
            if (e.Type == Opcode.Text)
            {
                string messageString = e.Data.ToString();
                // drop heart beat responce
                if (messageString.Contains("primus::pong::<"))
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Received websockets message '{0}' from {1}", messageString, m_WebSocketsConnection.Url.ToString()));
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
                    createOrderEventArgs.Status = messageData.Status;    
                    CreateOrderEvent(this, createOrderEventArgs);
                    break;
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.Order = m_DoshiiLogic.GetOrder(messageData.OrderId);
                    orderStatusEventArgs.OrderId = messageData.OrderId;
                    orderStatusEventArgs.Status = messageData.Status;    
                
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
        public virtual void WebSocketsConnectionOnCloseEventHandler(object sender, CloseEventArgs e)
        {
            //StopHeartbeatThread();
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection to {0} closed", m_WebSocketsConnection.Url.ToString()));
            //Initialize();
            
        }

        /// <summary>
        /// handles the webScoket onOpen event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void WebSocketsConnectionOnOpenEventHandler(object sender, EventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection successfully open to {0}", m_WebSocketsConnection.Url.ToString()));
            StartHeartbeatThread();
            SocketCommunicationEstablishedEvent(this, e);
        }

        #endregion

        public virtual void SetLastSuccessfullSocketCommunicationTime()
        {
            m_LastSuccessfullSocketMessageTime = DateTime.Now;
        }

        public virtual bool TestTimeOutValue()
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
