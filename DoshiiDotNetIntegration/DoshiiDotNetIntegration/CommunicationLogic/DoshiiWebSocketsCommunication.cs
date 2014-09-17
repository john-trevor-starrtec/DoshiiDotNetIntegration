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

        #region properties

        /// <summary>
        /// web socket object that will handle all the communications with doshii
        /// </summary>
        WebSocket WebSocketsConnection = null;

        private Doshii DoshiiLogic;

        private CommunicationLogic.DoshiiHttpCommunication HttpComs = null;

        /// <summary>
        /// A thread to send heartbeat socketmessage to doshii. 
        /// </summary>
        private Thread HeartBeatThread;

        /// <summary>
        /// Holds a value for the last time a socket message was sent or received to determine if it is necessary to send a heartbeat message
        /// </summary>
        private DateTime LastMessageTime;
        
        #endregion

        #region internal methods and events

        #region events

        internal delegate void CreatedOrderEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        internal event CreatedOrderEventHandler CreateOrderEvent;
        internal delegate void OrderStatusEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        internal event OrderStatusEventHandler OrderStatusEvent;
        internal delegate void ConsumerCheckInEventHandler(object sender, CommunicationEventArgs.CheckInEventArgs e);
        internal event ConsumerCheckInEventHandler ConsumerCheckinEvent;
        internal delegate void TableAllocationEventHandler(object sender, CommunicationEventArgs.TableAllocationEventArgs e);
        internal event TableAllocationEventHandler TableAllocationEvent;
        internal delegate void CheckOutEventHandler(object sender, CommunicationEventArgs.CheckOutEventArgs e);
        internal event CheckOutEventHandler CheckOutEvent;



        #endregion

        #region methods

        internal DoshiiWebSocketsCommunication(string webSocketURL, DoshiiHttpCommunication httpComs, Doshii doshii)
        {
            HttpComs = httpComs;
            WebSocketsConnection = new WebSocket(webSocketURL);
            WebSocketsConnection.OnOpen += new EventHandler(WebSocketsConnectionOnOpenEventHandler);
            WebSocketsConnection.OnClose += new EventHandler<CloseEventArgs>(WebSocketsConnectionOnCloseEventHandler);
            WebSocketsConnection.OnMessage += new EventHandler<MessageEventArgs>(WebSocketsConnectionOnMessageEventHandler);
            WebSocketsConnection.OnError += new EventHandler<ErrorEventArgs>(WebSocketsConnectionOnErrorEventHandler);

            DoshiiLogic = doshii;
        }

        internal void Initialize()
        {
            Connect();
            List<Models.TableAllocation> initialTableAllocationList = GetTableAllocations();
            foreach (Models.TableAllocation ta in initialTableAllocationList)
            {
                if (ta.Status == "waiting_for_confirmation")
                {
                    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();

                    newCheckinEventArgs.Consumer = GetConsumer(ta.PaypalCustomerId);
                    newCheckinEventArgs.Consumer.CheckInId = newCheckinEventArgs.CheckIn;
                    newCheckinEventArgs.CheckIn = ta.Checkin.Id;
                    newCheckinEventArgs.PaypalCustomerId = ta.PaypalCustomerId;
                    newCheckinEventArgs.Uri = newCheckinEventArgs.Consumer.PhotoUrl;
                    ConsumerCheckinEvent(this, newCheckinEventArgs);
                        
                    CommunicationEventArgs.TableAllocationEventArgs args = new CommunicationEventArgs.TableAllocationEventArgs();
                    args.TableAllocation = new Models.TableAllocation();
                    args.TableAllocation.CustomerId = ta.CustomerId;
                    args.TableAllocation.Id = ta.Id;
                    args.TableAllocation.Name = ta.Name;
                    args.TableAllocation.Status = ta.Status;
                    args.TableAllocation.PaypalCustomerId = ta.PaypalCustomerId;

                    TableAllocationEvent(this, args);
                        
                    
                    //if (!customerFound)
                    //{
                    //    //REVIEW: (liam) for the time being I'm going to assume that we always have the customers checked in, this is not likely to be the case but will help get the skeleton of the code together quicker.

                    //    //GetConsumer
                    //    Models.Consumer customer = HttpComs.GetConsumer(ta.PaypalCustomerId);
                    //    //raise checkin event
                    //    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                    //    newCheckinEventArgs.CheckIn = ta.Id;
                    //    newCheckinEventArgs.PaypalCustomerId = customer.PaypalCustomerId;
                    //    newCheckinEventArgs.Uri = customer.PhotoUrl;
                    //    newCheckinEventArgs.Consumer = customer;
                    //    if (ConsumerCheckinEvent != null)
                    //    {
                    //        ConsumerCheckinEvent(this, newCheckinEventArgs);
                    //    }


                    //    //raise allocation event
                    //    CommunicationEventArgs.TableAllocationEventArgs AllocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                    //    AllocationEventArgs.TableAllocation = new Models.TableAllocation();

                    //    AllocationEventArgs.TableAllocation.CustomerId = ta.CustomerId;
                    //    AllocationEventArgs.TableAllocation.Id = ta.Id;
                    //    AllocationEventArgs.TableAllocation.Name = ta.Name;
                    //    AllocationEventArgs.TableAllocation.Status = ta.Status;
                    //    AllocationEventArgs.TableAllocation.PaypalCustomerId = ta.PaypalCustomerId;

                    //    TableAllocationEvent(this, AllocationEventArgs);
                    //}
                }
                else if (ta.Status == "confirmed")
                {
                    
                    //confirm that table allocation exists. 
                }

            }
            //remove consumers that are not checked in. 
            List<Models.Consumer> currentlyCheckInConsumers = DoshiiLogic.GetCheckedInCustomersFromPos();
            foreach (Models.Consumer cus in currentlyCheckInConsumers)
            {
                bool customerFound = false;
                foreach (Models.TableAllocation ta in initialTableAllocationList)
                {
                    if (ta.PaypalCustomerId == cus.PaypalCustomerId)
                    {
                        customerFound = true;
                    }
                }
                if (!customerFound)
                {
                    //raise allocation event
                    CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationEventArgs.CheckOutEventArgs();

                    checkOutEventArgs.ConsumerId = cus.PaypalCustomerId;

                    CheckOutEvent(this, checkOutEventArgs);
                }
            }
            List<Models.Order> initialOrderList = GetOrders();
            foreach (Models.Order order in initialOrderList)
            {
                if (order.Status == "pending" || order.Status == "ready to pay" || order.Status == "cancelled")
                {
                    Models.Order orderToConfirm = GetOrder(order.Id.ToString());
                    CommunicationEventArgs.OrderEventArgs args = new CommunicationEventArgs.OrderEventArgs();
                    args.Order = orderToConfirm;
                    args.OrderId = orderToConfirm.Id.ToString();
                    args.status = orderToConfirm.Status;
                    CreateOrderEvent(this, args);
                }
            }
            HeartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
            HeartBeatThread.Start();
        }

        private void Connect()
        {
            if (WebSocketsConnection != null)
            {
                try
                {
                    WebSocketsConnection.Connect();
                    SetLastMessageTime();
                }
                catch(Exception ex)
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error initializing the web sockets conneciton to Doshii"), ex);
                }
                
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the ws object"));
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
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Sending websockets message '{0}' to {1}", message, WebSocketsConnection.Url.ToString()));
                WebSocketsConnection.Send(message);
            }
            catch (Exception ex)
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Doshii: Exception while sending a webSocket message '{0}' to {1}", message, WebSocketsConnection.Url.ToString()), ex);
            }
            
        }

        private void HeartBeatChecker()
        {
            while (true)
            {
                Thread.Sleep(3000);
                if (DateTime.Now.Add(new TimeSpan(0,0,-15)) > LastMessageTime)
                {
                    SendHeartBeat();
                }
            }
        }

        private void SendHeartBeat()
        {
            SetLastMessageTime();
            DateTime utcTime = LastMessageTime.ToUniversalTime();
            TimeSpan thisTimeSpan = new TimeSpan(utcTime.Ticks);
            double doubleForHeartbeat = thisTimeSpan.TotalMilliseconds;
            string nowString = JsonConvert.SerializeObject(LastMessageTime);
            string message = string.Format("\"primus::ping::<{0}>\"", doubleForHeartbeat.ToString());
            SendMessage(message);
        }

        private void SetLastMessageTime()
        {
            LastMessageTime = DateTime.Now;
        }

        #endregion

        #endregion

        #region event handlers

        private void WebSocketsConnectionOnErrorEventHandler(object sender, ErrorEventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error with the websockets connection to {0} the error was", WebSocketsConnection.Url.ToString(), e.Message));
        }

        private void WebSocketsConnectionOnMessageEventHandler(object sender, MessageEventArgs e)
        {
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
                    createOrderEventArgs.Order = GetOrder(messageData.OrderId);
                    createOrderEventArgs.OrderId = messageData.OrderId;
                    createOrderEventArgs.status = messageData.Status;    
                    CreateOrderEvent(this, createOrderEventArgs);
                    break;
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.Order = GetOrder(messageData.OrderId);
                    orderStatusEventArgs.OrderId = messageData.OrderId;
                    orderStatusEventArgs.status = messageData.Status;    
                
                    OrderStatusEvent(this, orderStatusEventArgs);
                    break;
                case "consumer_checkin":
                    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                    newCheckinEventArgs.CheckIn = messageData.CheckinId;
                    newCheckinEventArgs.PaypalCustomerId = messageData.PaypalCustomerId;
                    newCheckinEventArgs.Uri = messageData.Uri;
                    newCheckinEventArgs.Consumer = GetConsumer(messageData.PaypalCustomerId);
                    newCheckinEventArgs.Consumer.CheckInId = newCheckinEventArgs.CheckIn;

                    ConsumerCheckinEvent(this, newCheckinEventArgs);
                    break;
                default:
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Received socket message is not a supported message. messageType - '{0}'", messageData.EventName));
                    break;
            }
            
        }

        private void WebSocketsConnectionOnCloseEventHandler(object sender, CloseEventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection to {0} closed", WebSocketsConnection.Url.ToString()));
            Initialize();
        }

        private void WebSocketsConnectionOnOpenEventHandler(object sender, EventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection open to {0}", WebSocketsConnection.Url.ToString()));
        }

        #endregion

        private Models.Consumer GetConsumer(string paypalCustomerId)
        {
            return HttpComs.GetConsumer(paypalCustomerId);
        }

        private Models.Order GetOrder(string orderId)
        {
            return HttpComs.GetOrder(orderId);
        }

        private List<Models.Order> GetOrders()
        {
            return HttpComs.GetOrders();
        }

        private List<Models.TableAllocation> GetTableAllocations()
        {
            return HttpComs.GetTableAllocations();
        }
    }
}
