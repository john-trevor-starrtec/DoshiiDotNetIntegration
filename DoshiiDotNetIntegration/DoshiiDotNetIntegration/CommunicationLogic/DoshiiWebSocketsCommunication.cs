using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;

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

        #endregion

        #region internal methods and events

        #region events

        internal event EventHandler<CommunicationEventArgs.OrderEventArgs> CreateOrderEvent;

        internal event EventHandler<CommunicationEventArgs.OrderEventArgs> OrderStatusEvent;

        internal event EventHandler<CommunicationEventArgs.CheckInEventArgs> ConsumerCheckinEvent;

        internal event EventHandler<CommunicationEventArgs.TableAllocationEventArgs> TableAllocationEvent;



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

            initialize(DoshiiLogic.GetCheckedInCustomersFromPos());
            
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
                        if (ta.customerId == cus.paypalCustomerId)
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
                        ConsumerCheckinEvent(this, newCheckinEventArgs);

                        //raise allocation event
                        CommunicationEventArgs.TableAllocationEventArgs AllocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                        AllocationEventArgs.TableAllocation.customerId = ta.customerId;
                        AllocationEventArgs.TableAllocation.id = ta.id;
                        AllocationEventArgs.TableAllocation.name = ta.name;
                        AllocationEventArgs.TableAllocation.status = ta.status;

                        TableAllocationEvent(this, AllocationEventArgs);
                    }
                }
            }
            List<Modles.order> initialOrderList = GetOrders();
            foreach (Modles.order order in initialOrderList)
            {
                if (order.status == Enums.OrderStates.pending || order.status == Enums.OrderStates.readytopay || order.status == Enums.OrderStates.cancelled)
                {
                    CommunicationEventArgs.OrderEventArgs args = new CommunicationEventArgs.OrderEventArgs();
                    args.order = order;
                    args.OrderId = order.id;
                    args.status = order.status;
                    CreateOrderEvent(this, args);
                }
            }
        }

        private void Connect()
        {
            if (ws != null)
            {
                ws.Connect();
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
        internal void SendMessage(string message)
        {
            DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("sending websockets message {0} to {1}", message, ws.Url.ToString()));
            ws.Send(message);
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
            DoshiiDotNetIntegration.Modles.SocketMessage theMessage = new Modles.SocketMessage();
            if (e.Type == Opcode.Text)
            {
                theMessage = DoshiiDotNetIntegration.Modles.SocketMessage.deseralizeFromJson(e.Data.ToString());
            }

            if (e.Type == Opcode.Binary)
            {
                theMessage = DoshiiDotNetIntegration.Modles.SocketMessage.deseralizeFromJson(e.RawData.ToString());
            }
            switch (theMessage.EventName)
            {
                case "table_allocation":
                    CommunicationEventArgs.TableAllocationEventArgs AllocationEventArgs = new CommunicationEventArgs.TableAllocationEventArgs();

                    AllocationEventArgs.TableAllocation.customerId = theMessage.consumerId;
                    AllocationEventArgs.TableAllocation.id = theMessage.id;
                    AllocationEventArgs.TableAllocation.name = theMessage.name;
                    if (theMessage.status == "waiting_for_confirmation")
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
                    createOrderEventArgs.order = GetOrder(theMessage.order);
                    createOrderEventArgs.OrderId = theMessage.orderId;
                    switch (theMessage.status)
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
                            throw new NotSupportedException(theMessage.status);
                    }
                                        
                    CreateOrderEvent(this, createOrderEventArgs);
                    break;
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.order = GetOrder(theMessage.order);
                    orderStatusEventArgs.OrderId = theMessage.orderId;
                    switch (theMessage.status)
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
                            throw new NotSupportedException(theMessage.status);
                    }

                    OrderStatusEvent(this, orderStatusEventArgs);
                    break;
                case "consumer_checkin":
                    CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationEventArgs.CheckInEventArgs();
                    newCheckinEventArgs.checkin = theMessage.checkin;
                    newCheckinEventArgs.consumer = theMessage.consumer;
                    newCheckinEventArgs.paypalCustomerId = theMessage.paypalCustomerId;
                    newCheckinEventArgs.uri = theMessage.uri;
                    newCheckinEventArgs.consumerObject = GetConsumer(theMessage.paypalCustomerId);

                    ConsumerCheckinEvent(this, newCheckinEventArgs);
                    break;
                default:
                    throw new NotSupportedException(theMessage.EventName);
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
