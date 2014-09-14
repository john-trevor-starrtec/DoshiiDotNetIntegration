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
        WebSocket ws = null;

        private Doshii DoshiiLogic;

        private CommunicationLogic.DoshiiHttpCommunication HttpComs = null;

        private Thread HeartBeatThread;



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
            ws = new WebSocket(webSocketURL);
            ws.OnOpen += new EventHandler(ws_OnOpen);
            ws.OnClose += new EventHandler<CloseEventArgs>(ws_OnClose);
            ws.OnMessage += new EventHandler<MessageEventArgs>(ws_OnMessage);
            ws.OnError += new EventHandler<ErrorEventArgs>(ws_OnError);

            DoshiiLogic = doshii;

            //initialize(DoshiiLogic.GetCheckedInCustomersFromPos());
            
        }

        internal void initialize(List<Modles.Consumer> currentlyCheckInConsumers)
        {
            Connect();
            List<Modles.table_allocation> initialTableAllocationList = GetTableAllocations();
            foreach (Modles.table_allocation ta in initialTableAllocationList)
            {
                if (ta.status == Enums.AllocationStates.waiting_for_confirmation)
                {
                    bool customerFound = false;
                    foreach (Modles.Consumer cus in currentlyCheckInConsumers)
                    {
                        if (ta.paypalCustomerId == cus.paypalCustomerId)
                        {
                            customerFound = true;
                            CommunicationEventArgs.TableAllocationEventArgs args = new CommunicationEventArgs.TableAllocationEventArgs();
                            args.TableAllocation = ta;
                            TableAllocationEvent(this, args);
                        }
                    }
                    if (!customerFound)
                    {
                        //REVIEW: (liam) for the time being I'm going to assume that we always have the customers checked in, this is not likely to be the case but will help get the skeleton of the code together quicker.

                        //GetConsumer
                        Modles.Consumer customer = HttpComs.GetConsumer(ta.paypalCustomerId);
                        //raise checkin event
                        CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                        newCheckinEventArgs.checkin = ta.id;
                        newCheckinEventArgs.consumer = customer.name;
                        newCheckinEventArgs.paypalCustomerId = customer.paypalCustomerId;
                        newCheckinEventArgs.uri = customer.PhotoUrl;
                        newCheckinEventArgs.consumerObject = customer;
                        if (ConsumerCheckinEvent != null)
                        {
                            ConsumerCheckinEvent(this, newCheckinEventArgs);
                        }


                        //raise allocation event
                        CommunicationEventArgs.TableAllocationEventArgs AllocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                        AllocationEventArgs.TableAllocation = new Modles.table_allocation();

                        AllocationEventArgs.TableAllocation.customerId = ta.customerId;
                        AllocationEventArgs.TableAllocation.id = ta.id;
                        AllocationEventArgs.TableAllocation.name = ta.name;
                        AllocationEventArgs.TableAllocation.status = ta.status;
                        AllocationEventArgs.TableAllocation.paypalCustomerId = ta.paypalCustomerId;

                        TableAllocationEvent(this, AllocationEventArgs);
                    }
                }
                else if (ta.status == Enums.AllocationStates.confirmed)
                {
                    
                    //confirm that table allocation exists. 
                }

            }
            //remove consumers that are not checked in. 
            foreach (Modles.Consumer cus in currentlyCheckInConsumers)
            {
                bool customerFound = false;
                foreach (Modles.table_allocation ta in initialTableAllocationList)
                {
                    if (ta.paypalCustomerId == cus.paypalCustomerId)
                    {
                        customerFound = true;
                    }
                }
                if (!customerFound)
                {
                    //raise allocation event
                    CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationEventArgs.CheckOutEventArgs();

                    checkOutEventArgs.ConsumerId = cus.paypalCustomerId;

                    CheckOutEvent(this, checkOutEventArgs);
                }
            }
            List<Modles.order> initialOrderList = GetOrders();
            foreach (Modles.order order in initialOrderList)
            {
                if (order.status == Enums.OrderStates.pending || order.status == Enums.OrderStates.readytopay || order.status == Enums.OrderStates.cancelled)
                {
                    CommunicationEventArgs.OrderEventArgs args = new CommunicationEventArgs.OrderEventArgs();
                    args.order = order;
                    args.OrderId = order.id.ToString();
                    args.status = order.status;
                    CreateOrderEvent(this, args);
                }
            }
            HeartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
            HeartBeatThread.Start();
        }

        private void Connect()
        {
            if (ws != null)
            {
                ws.Connect();
                SetLastMessageTime();
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Attempted to open a web socket connection before initializing the ws object"));
            }
        }

        /// <summary>
        /// this should not be used as the pos does not communication with Doshii though web sockets, the pos uses http for sending messages to doshii. 
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(string message)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("sending websockets message {0} to {1}", message, ws.Url.ToString()));
            ws.Send(message);
        }

        private DateTime LastMessageTime;


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

        private void ws_OnError(object sender, ErrorEventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("there was an error with the websockets connection to {0} the error was", ws.Url.ToString(), e.Message));
        }

        private void ws_OnMessage(object sender, MessageEventArgs e)
        {
            SetLastMessageTime();
            DoshiiDotNetIntegration.Modles.SocketMessage theMessage = new Modles.SocketMessage();
            if (e.Type == Opcode.Text)
            {
                string messageString = e.Data.ToString();
                if (messageString.Contains("primus::pong::<"))
                {
                    return;
                }
                else
                {
                    theMessage = DoshiiDotNetIntegration.Modles.SocketMessage.deseralizeFromJson(messageString);
                }
                
            }

            if (e.Type == Opcode.Binary)
            {
                string messageString = e.RawData.ToString();
                if (messageString.Contains("primus::pong::<"))
                {
                    return;
                }
                else
                {
                    theMessage = DoshiiDotNetIntegration.Modles.SocketMessage.deseralizeFromJson(messageString);
                }
                
            }

            dynamic SMD = theMessage.emit[1];

            Modles.SocketMessageData Md = new Modles.SocketMessageData();

            Md.EventName = (string)theMessage.emit[0];
            Md.checkinId = (string)SMD.checkinId;
            Md.orderId = (string)SMD.orderId;
            Md.paypalCustomerId = (string)SMD.paypalCustomerId;
            Md.status = (string)SMD.status;
            Md.name = (string)SMD.name;
            Md.id = (string)SMD.id;
            
            string uriString = (string)SMD.uri;
            if (!string.IsNullOrWhiteSpace(uriString))
            {
                Md.uri = new Uri((string)SMD.uri);
            }
            

            switch (Md.EventName)
            {
                case "table_allocation":
                    CommunicationEventArgs.TableAllocationEventArgs AllocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                    AllocationEventArgs.TableAllocation = new Modles.table_allocation();

                    AllocationEventArgs.TableAllocation.customerId = Md.consumerId;
                    AllocationEventArgs.TableAllocation.id = Md.id;
                    AllocationEventArgs.TableAllocation.name = Md.name;
                    AllocationEventArgs.TableAllocation.paypalCustomerId = Md.paypalCustomerId;
                    if (Md.status == "waiting_for_confirmation")
                    {
                        AllocationEventArgs.TableAllocation.status = Enums.AllocationStates.waiting_for_confirmation;
                    }
                    else
                    {
                        AllocationEventArgs.TableAllocation.status = Enums.AllocationStates.confirmed;
                    }
                            
                    TableAllocationEvent(this, AllocationEventArgs);
                    break;
                case "order_create":
                    CommunicationEventArgs.OrderEventArgs createOrderEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    createOrderEventArgs.order = GetOrder(Md.order);
                    createOrderEventArgs.OrderId = Md.orderId;
                    switch (Md.status)
                    {
                        case "accepted":
                            createOrderEventArgs.status = Enums.OrderStates.accepted; 
                            break;
                        case "rejected":
                            createOrderEventArgs.status = Enums.OrderStates.rejected;
                            break;
                        case "waitingforpayment":
                            createOrderEventArgs.status = Enums.OrderStates.waitingforpayment;
                            break;
                        case "paid":
                            createOrderEventArgs.status = Enums.OrderStates.paid;
                            break;
                        case "pending":
                            createOrderEventArgs.status = Enums.OrderStates.pending;
                            break;
                        case "readytopay":
                            createOrderEventArgs.status = Enums.OrderStates.readytopay;
                            break;
                        case "cancelled":
                            createOrderEventArgs.status = Enums.OrderStates.cancelled;
                            break;
                        default:
                            throw new NotSupportedException(Md.status);
                    }
                                        
                    CreateOrderEvent(this, createOrderEventArgs);
                    break;
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.order = GetOrder(Md.order);
                    orderStatusEventArgs.OrderId = Md.orderId;
                    switch (Md.status)
                    {
                        case "accepted":
                            orderStatusEventArgs.status = Enums.OrderStates.accepted;
                            break;
                        case "rejected":
                            orderStatusEventArgs.status = Enums.OrderStates.rejected;
                            break;
                        case "waitingforpayment":
                            orderStatusEventArgs.status = Enums.OrderStates.waitingforpayment;
                            break;
                        case "paid":
                            orderStatusEventArgs.status = Enums.OrderStates.paid;
                            break;
                        case "pending":
                            orderStatusEventArgs.status = Enums.OrderStates.pending;
                            break;
                        case "readytopay":
                            orderStatusEventArgs.status = Enums.OrderStates.readytopay;
                            break;
                        case "cancelled":
                            orderStatusEventArgs.status = Enums.OrderStates.cancelled;
                            break;
                        default:
                            throw new NotSupportedException(Md.status);
                    }

                    OrderStatusEvent(this, orderStatusEventArgs);
                    break;
                case "consumer_checkin":
                    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                    newCheckinEventArgs.checkin = Md.checkinId;
                    newCheckinEventArgs.consumer = Md.consumer;
                    newCheckinEventArgs.paypalCustomerId = Md.paypalCustomerId;
                    newCheckinEventArgs.uri = Md.uri;
                    newCheckinEventArgs.consumerObject = GetConsumer(Md.paypalCustomerId);

                    ConsumerCheckinEvent(this, newCheckinEventArgs);
                    break;
                default:
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Received socket message is not a supported message. messageType - '{0}'", Md.EventName));
                    break;
            }
            
        }

        private void ws_OnClose(object sender, CloseEventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("webScokets connection to {0} closed", ws.Url.ToString()));
            initialize(DoshiiLogic.GetCheckedInCustomersFromPos());
        }

        private void ws_OnOpen(object sender, EventArgs e)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("webScokets connection open to {0}", ws.Url.ToString()));
        }

        #endregion

        private Modles.Consumer GetConsumer(string paypalCustomerId)
        {
            return HttpComs.GetConsumer(paypalCustomerId);
        }

        private Modles.order GetOrder(string orderId)
        {
            return HttpComs.GetOrder(orderId);
        }

        private List<Modles.order> GetOrders()
        {
            return HttpComs.GetOrders();
        }

        private List<Modles.table_allocation> GetTableAllocations()
        {
            return HttpComs.GetTableAllocations();
        }
    }
}
