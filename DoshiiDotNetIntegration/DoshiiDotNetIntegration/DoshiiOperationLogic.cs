using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;


namespace DoshiiDotNetIntegration
{
    public class DoshiiOperationLogic 
    {

        #region properties, constructors, Initialize, versionCheck

        /// <summary>
        /// holds the class for interacting with the Doshii webSocket connection
        /// </summary>
        public CommunicationLogic.DoshiiWebSocketsCommunication m_SocketComs = null;

        /// <summary>
        /// holds the class for interacting with the Doshii http restful api
        /// </summary>
        public CommunicationLogic.DoshiiHttpCommunication m_HttpComs = null;

        /// <summary>
        /// holds the interface for doshiis interaction with the pos. 
        /// </summary>
        public  Interfaces.iDoshiiOrdering m_DoshiiInterface = null;

        /// <summary>
        /// the order mode for the venue
        /// </summary>
        public  Enums.OrderModes OrderMode{ get; set; }

        /// <summary>
        /// the seating mode for the venue
        /// </summary>
        public  Enums.SeatingModes SeatingMode { get; set; }
        
        /// <summary>
        /// the authentication token for the venue 
        /// </summary>
        public  string AuthorizeToken { get; set; }

        /// <summary>
        /// holds a value indicating if table allocations should be removed after a full check payment. 
        /// </summary>
        public  bool RemoveTableAllocationsAfterFullPayment { get; set; }

        /// <summary>
        /// get the current version
        /// </summary>
        /// <returns></returns>
        protected static string CurrnetVersion()
        {
            var versionStringBuilder = new StringBuilder();
            versionStringBuilder.Append("Doshii Integration Version: ");
            versionStringBuilder.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            versionStringBuilder.Append(Environment.NewLine);
            
            return versionStringBuilder.ToString();
        }

        /// <summary>
        /// constructor
        /// </summary>
        public DoshiiOperationLogic(Interfaces.iDoshiiOrdering doshiiInterface)
        {
            
            if (doshiiInterface == null)
            {
                throw new NullReferenceException("doshiiInterface is Null");
            }
            m_DoshiiInterface = doshiiInterface;
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Instanciating Doshii Class");
        }
        /// initalizes the webSocket communicaiton with Doshii,
        /// initalizes the Http communicaiton with Doshii,
        /// </summary>
        /// <param name="socketUrl">
        /// the socket Url for communication with doshii (the address should not end in a '/')
        /// </param>
        /// <param name="token">
        /// the per venue token
        /// </param>
        /// <param name="orderMode">
        /// 1 = restaurant mode, 2 = bistro mode
        /// </param>
        /// <param name="seatingMode">
        /// 1= pos Allocation, 2 = venue allocaiton
        /// </param>
        /// <param name="urlBase">
        /// the base url for communication with the doshii restfull api (the address should not end in a '/')
        /// </param>
        /// <param name="startWebSocketConnection"></param>
        public virtual void Initialize(string socketUrl, string token, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string urlBase, bool startWebSocketConnection, bool removeTableAllocationsAfterFullPayment, int timeOutValueSecs)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Info, string.Format("Doshii: Version {5} with; {6} sourceUrl: {0}, {6}token {1}, {6}orderMode {2}, {6}seatingMode: {3},{6}BaseUrl: {4}{6}", socketUrl, token, orderMode.ToString(), seatingMode.ToString(), urlBase, CurrnetVersion(), Environment.NewLine));
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Info, string.Format("Doshii: Versioning Info: {0}, token {1}, orderMode {2}, seatingMode: {3}, BaseUrl: {4}", socketUrl, token, orderMode.ToString(), seatingMode.ToString(), urlBase));

            // REVIEW: (LIAM) this should be using regex to test the string is a valid ws address. 
            if (string.IsNullOrWhiteSpace(socketUrl))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required sockerUrl");
                throw new NotSupportedException("empty socketUrl");
            }

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required urlBase");
                throw new NotSupportedException("empty socketUrl");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required token");
                throw new NotSupportedException("token");
            }

            if (timeOutValueSecs <= 0)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - timeoutvaluesecs must be larger than 0");
                throw new NotSupportedException("timeoutvaluesecs");
            }

            RemoveTableAllocationsAfterFullPayment = removeTableAllocationsAfterFullPayment;
            AuthorizeToken = token;
            string socketUrlWithToken = string.Format("{0}?token={1}", socketUrl, token);
            InitializeProcess(socketUrlWithToken, orderMode, seatingMode, urlBase, startWebSocketConnection, timeOutValueSecs);
        }

        /// <summary>
        /// starts the http and socket communications
        /// </summary>
        /// <param name="socketUrl"></param>
        /// <param name="orderMode"></param>
        /// <param name="seatingMode"></param>
        /// <param name="UrlBase"></param>
        /// <param name="StartWebSocketConnection"></param>
        /// <returns></returns>
        public virtual bool InitializeProcess(string socketUrl, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, bool StartWebSocketConnection, int timeOutValueSecs)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Initializing Doshii");

            bool result = true;

            OrderMode = orderMode;
            SeatingMode = seatingMode;

            m_HttpComs = new CommunicationLogic.DoshiiHttpCommunication(UrlBase, this, AuthorizeToken);

            if (StartWebSocketConnection)
            {
                try
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socketUrl = {0}, timeOutValueSecs = {1}", socketUrl, timeOutValueSecs)));
                    m_SocketComs = new CommunicationLogic.DoshiiWebSocketsCommunication(socketUrl, this, timeOutValueSecs);
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socket Comms are set")));
                    
                    SubscribeToSocketEvents();
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socket events are subscribed to")));
                    
                    m_SocketComs.Initialize();
                }
                catch (Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format(string.Format("Initializing Doshii failed, there was an exception that was {0}", ex.ToString())));
                }
                
            }
            return result;
        }

        /// <summary>
        /// refreshes the current consumer checkins, allocations and orders. 
        /// </summary>
        public virtual void RefreshConsumerData()
        {
            if (m_HttpComs.SetSeatingAndOrderConfiguration(SeatingMode, OrderMode))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Refreshing consumers, allocation, and orders");
                List<Models.Consumer> currentlyCheckInConsumers = m_DoshiiInterface.GetCheckedInCustomersFromPos();
                
                if (OrderMode == Enums.OrderModes.BistroMode)
                {
                    foreach (Models.Consumer currentConsumer in currentlyCheckInConsumers)
                    {
                        Models.Order consumerOrder = m_DoshiiInterface.GetOrderForCheckinId(currentConsumer.CheckInId);
                        if (consumerOrder != null)
                        {
                            RequestPaymentForOrder(consumerOrder);
                        }
                        
                    }
                }
                currentlyCheckInConsumers = m_DoshiiInterface.GetCheckedInCustomersFromPos();
                List<Models.Consumer> currentlyCheckedInDoshiiConsumers = m_HttpComs.GetConsumers();
                foreach(Models.Consumer doshiiCon in currentlyCheckedInDoshiiConsumers)
                {
                    if (!findCurrentConsumer(currentlyCheckInConsumers, doshiiCon))
                    {
                        CommunicationLogic.CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckInEventArgs();

                        newCheckinEventArgs.Consumer = m_HttpComs.GetConsumer(doshiiCon.PaypalCustomerId);

                        newCheckinEventArgs.Consumer.CheckInId = doshiiCon.CheckInId;
                        newCheckinEventArgs.CheckIn = doshiiCon.CheckInId;
                        newCheckinEventArgs.PaypalCustomerId = doshiiCon.PaypalCustomerId;
                        newCheckinEventArgs.Uri = doshiiCon.PhotoUrl;
                        SocketComsConsumerCheckinEventHandler(this, newCheckinEventArgs);
                    }
                }
                List<Models.TableAllocation> initialTableAllocationList = m_HttpComs.GetTableAllocations();
                foreach (Models.TableAllocation ta in initialTableAllocationList)
                {
                    CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs args = new CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs();
                    args.TableAllocation = new Models.TableAllocation();
                    args.TableAllocation.CustomerId = ta.CustomerId;
                    args.TableAllocation.Id = ta.Id;
                    args.TableAllocation.Name = ta.Name;
                    args.TableAllocation.Status = ta.Status;
                    args.TableAllocation.PaypalCustomerId = ta.PaypalCustomerId;
                    args.TableAllocation.Checkin = ta.Checkin;

                    SocketComsTableAllocationEventHandler(this, args);
                }
                //remove consumers that are not checked in. 
                foreach (Models.Consumer localCon in currentlyCheckInConsumers)
                {
                    if (!findCurrentConsumer(currentlyCheckedInDoshiiConsumers, localCon))
                    {
                        CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs();

                        checkOutEventArgs.ConsumerId = localCon.PaypalCustomerId;

                        SocketComsCheckOutEventHandler(this, checkOutEventArgs);
                    }
                }
                // get any pending orders
                List<Models.Order> initialOrderList = m_HttpComs.GetOrders();
                foreach (Models.Order order in initialOrderList)
                {
                    
                    if (order.Status == "pending" || order.Status == "ready to pay" || order.Status == "cancelled")
                    {
                        m_DoshiiInterface.RecordOrderUpdatedAtTime(order);
                        Models.Order orderToConfirm = m_HttpComs.GetOrder(order.Id.ToString());
                        CommunicationLogic.CommunicationEventArgs.OrderEventArgs args = new CommunicationLogic.CommunicationEventArgs.OrderEventArgs();
                        args.Order = orderToConfirm;
                        args.OrderId = orderToConfirm.Id.ToString();
                        args.status = orderToConfirm.Status;
                        SocketComsOrderStatusEventHandler(this, args);
                    }
                }
            }
            else
            {
                m_SocketComs.ClostSocketConnection();
            }
        }

        public virtual bool findCurrentConsumer(List<Models.Consumer> consumersList, Models.Consumer currentConsumer)
        {
            bool consumerFound = false;
            foreach (Models.Consumer localCon in consumersList)
            {
                if (currentConsumer.PaypalCustomerId == localCon.PaypalCustomerId)
                {
                    consumerFound = true;
                    break;
                }
            }
            return consumerFound;
        }

        /// <summary>
        /// subcribs to the socket communication events 
        /// </summary>
        public virtual void SubscribeToSocketEvents()
        {
            if (m_SocketComs == null)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: The socketComs has not been initialized");
                throw new NotSupportedException("m_SocketComms is null");
            }
            else
            {
                UnsubscribeFromSocketEvents();
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Subscribing to socket events");
                m_SocketComs.ConsumerCheckinEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComsConsumerCheckinEventHandler);
                m_SocketComs.CreateOrderEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComsOrderStatusEventHandler);
                m_SocketComs.OrderStatusEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
                m_SocketComs.TableAllocationEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComsTableAllocationEventHandler);
                m_SocketComs.CheckOutEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.CheckOutEventHandler(SocketComsCheckOutEventHandler);
                m_SocketComs.SocketCommunicationEstablishedEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
                m_SocketComs.SocketCommunicationTimeoutReached += new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(ScoketComsTimeOutValueReached);
            }
        }

        /// <summary>
        /// unsubcribs to the socket communication events 
        /// </summary>
        public virtual void UnsubscribeFromSocketEvents()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: unscribing from socket events");
            m_SocketComs.ConsumerCheckinEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComsConsumerCheckinEventHandler);
            m_SocketComs.CreateOrderEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.OrderStatusEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.TableAllocationEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComsTableAllocationEventHandler);
            m_SocketComs.CheckOutEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CheckOutEventHandler(SocketComsCheckOutEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
            m_SocketComs.SocketCommunicationTimeoutReached -= new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(ScoketComsTimeOutValueReached);
        }
        #endregion

        #region socket communication event handlers

        /// <summary>
        /// handles the consumer checked event and raises a consumer checkOut event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsCheckOutEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received comsumer checkout event for consumerId '{0}'", e.ConsumerId));
            if (!string.IsNullOrWhiteSpace(e.ConsumerId))
            {
                m_DoshiiInterface.CheckOutConsumer(e.ConsumerId);
            }
        }

        /// <summary>
        /// handles a socket communicaiton established event and raises a refreshComsumerData event. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            RefreshConsumerData();
        }

        /// <summary>
        /// handles a socket communicaiton timeOut event - this is when there has not been a successfull comunication with doshii within the specified timeout period. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ScoketComsTimeOutValueReached(object sender, EventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: the web sockets connection with the doshii server is not currently available.");
            m_DoshiiInterface.DissociateDoshiiChecks();
        }

        /// <summary>
        /// handles a SocketComs_TableAllocationEvent established event and confirms or rejects the event with doshii 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsTableAllocationEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs e)
        {
            Models.TableAllocation tableAllocation = new Models.TableAllocation();
            tableAllocation = e.TableAllocation;
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received table allocation event for consumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
            if (m_DoshiiInterface.ConfirmTableAllocation(ref tableAllocation))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: confirming table allocaiton for consumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.PutTableAllocation(tableAllocation.PaypalCustomerId, tableAllocation.Id);
                
            }
            else if (tableAllocation.rejectionReason == Enums.TableAllocationRejectionReasons.TableDoesNotExist)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocaiton for consumer '{0}' and table '{1}' checkInId '{2}' Table dose not exist", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.RejectTableAllocation(tableAllocation.PaypalCustomerId, tableAllocation.Id, tableAllocation.rejectionReason);
            }
            else
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocaiton for consumer '{0}' and table '{1}' checkInId '{2}' Table is occupied", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.RejectTableAllocation(tableAllocation.PaypalCustomerId, tableAllocation.Id, tableAllocation.rejectionReason);
            }
        }

        /// <summary>
        /// handles a SocketComs_OrderStatusEvent, confirms the order, and raises an appropriate event.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsOrderStatusEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received order status event with status '{0}', for order '{1}'", e.Order.Status, e.Order.ToJsonString()));
            Models.Order returnedOrder = new Models.Order();
            switch (e.Order.Status)
            {
                case "cancelled":
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);
                    m_DoshiiInterface.OrderCancled(ref e.Order);
                    break;
                
                case "ready to pay":
                    m_DoshiiInterface.ConfirmOrderTotalsBeforePaymentRestaurantMode(ref e.Order);
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);
                    RequestPaymentForOrder(e.Order);
                    break;
                case "new":
                case "pending":
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);    
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        if (m_DoshiiInterface.ConfirmOrderAvailabilityBistroMode(ref e.Order))
                        {
                            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order availibility confirmed for bistroMode - '{0}'", e.Order.ToJsonString()));
                             
                            e.Order.Status = "accepted";
                            returnedOrder = m_HttpComs.PutOrder(e.Order);
                            if (returnedOrder.Id == e.Order.Id)
                            {
                                returnedOrder.Status = "waiting for payment";
                                returnedOrder = m_HttpComs.PutOrder(returnedOrder);
                                if (returnedOrder.Id == e.Order.Id)
                                {
                                    int nonPayingAmount = 0;
                                    int.TryParse(returnedOrder.NotPayingTotal, out nonPayingAmount);

                                    if (nonPayingAmount > 0)
                                    {
                                        throw new NotSupportedException("Doshii: partial payment in bistro mode");
                                    }
                                    else
                                    {
                                        if (m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref returnedOrder) && RemoveTableAllocationsAfterFullPayment)
                                        {
                                            m_HttpComs.DeleteTableAllocationWithCheckInId(e.Order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order rejected for bistro mode - '{0}'", e.Order.ToJsonString())); 
                            e.Order.Status = "rejected";
                            m_HttpComs.PutOrder(e.Order);
                        }
                    }
                    else
                    {
                        if (m_DoshiiInterface.ConfirmOrderForRestaurantMode(ref e.Order))
                        {
                            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order confirmed for restaurant mode - '{0}'", e.Order.ToJsonString())); 
                            e.Order.Status = "accepted";
                            m_HttpComs.PutOrder(e.Order);
                            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: the order has now being returned"));
                        }
                        else
                        {
                            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order rejected for restaurant mode - '{0}'", e.Order.ToJsonString())); 
                            e.Order.Status = "rejected";
                            m_HttpComs.PutOrder(e.Order);
                        }
                    }
                    break;
                default:
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: unkonwn order status - '{0}'", e.Order.ToJsonString())); 
                    throw new NotSupportedException(e.Order.Status.ToString());

            }
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: SocketComsOrderStatusEventHandler has returned"));
        }

        public virtual bool RequestPaymentForOrder(Models.Order order)
        {
            Models.Order returnedOrder = new Models.Order();
            order.Status = "waiting for payment";
            try
            {
                returnedOrder = m_HttpComs.PutOrder(order);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                }
            }
            
            
            if (returnedOrder.Id == order.Id)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order put for payment - '{0}'", order.ToJsonString()));
                int nonPayingAmount = 0;
                int.TryParse(order.NotPayingTotal, out nonPayingAmount);

                if (nonPayingAmount > 0)
                {
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        if (m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) && RemoveTableAllocationsAfterFullPayment)
                        {
                            m_HttpComs.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid);
                        }
                    }
                    else
                    {
                        if (m_DoshiiInterface.RecordPartialCheckPayment(ref order) && RemoveTableAllocationsAfterFullPayment)
                        {
                            m_HttpComs.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid);
                        }
                        
                    }

                }
                else
                {
                    if (m_DoshiiInterface.RecordFullCheckPayment(ref order) && RemoveTableAllocationsAfterFullPayment)
                    {
                        m_HttpComs.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// handles the SocketComs_ConsumerCheckinEvent and records the checked in user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsConsumerCheckinEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckInEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: checkIn event received for consumer - '{0}' with id '{1}'", e.Consumer.Name, e.Consumer.PaypalCustomerId));
            m_DoshiiInterface.recordCheckedInUser(ref e.Consumer);
        }

        #endregion
        
        #region product sync methods

        /// <summary>
        /// This method will return all the products currently in Doshii, if the request fails the failure message will be logged and the products list will be empty. 
        /// </summary>
        /// <returns></returns>
        public virtual List<Models.Product> GetAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all doshii products")); 
            return m_HttpComs.GetDoshiiProducts();
        }

        /// <summary>
        /// Adds a list of products to Doshii
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public virtual void AddNewProducts(List<Models.Product> productList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos adding new product list- '{0}'", JsonConvert.SerializeObject(productList))); 
            try
            {
                m_HttpComs.PostProductData(productList, false);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new Exceptions.ProductNotCreatedException();
                }
                else
                {
                    throw rex;
                }
                
            }
            
        }

        /// <summary>
        /// adds a single product to doshii
        /// This method will delete the current product on doshii and replace it with the paramater if it already exists. 
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <param name="deleteAllProductsCurrentlyOnDoshii"></param>
        /// <returns></returns>
        public virtual void AddNewProducts(Models.Product productToUpdate, bool deleteAllProductsCurrentlyOnDoshii)
        {
            List<Models.Product> productList = new List<Models.Product>();
            productList.Add(productToUpdate);
            try
            {
                m_HttpComs.PostProductData(productList, deleteAllProductsCurrentlyOnDoshii);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// updates a product in doshii.
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        public virtual void UpdateProcuct(Models.Product productToUpdate)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos updating product - '{0}'", productToUpdate.ToJsonStringForProductSync()));
            try
            {
                m_HttpComs.PutProductData(productToUpdate);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        
        /// <summary>
        /// deltes the provided list of products from doshii. 
        /// REVIEW:(Liam) this really needs to have a better method for measuring sucess, and a message about why things failed...
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public virtual void DeleteProducts(List<string> productIdList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product list- '{0}'", JsonConvert.SerializeObject(productIdList)));
            try
            {
                foreach (string pro in productIdList)
                {
                    DeleteProduct(pro);
                }
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// delete a product from doshii
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual void DeleteProduct(string productId)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product from doshii - '{0}'", productId));
            try
            {
                m_HttpComs.DeleteProductData(productId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// deletes all the products from doshii
        /// </summary>
        /// <returns></returns>
        public virtual void DeleteAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting all products from doshii"));
            try
            {
                m_HttpComs.DeleteProductData();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        #endregion

        #region ordering And Payment

        /// <summary>
        /// gets an order from doshii with the provided orderId, if there no order is no order matching the provided orderId on doshii a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public virtual Models.Order GetOrder(string orderId)
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
        /// adds items to a doshii order
        /// </summary>
        /// <param name="order">
        /// The order must contain all the products included in the check as this method overwrites all the items recorded on doshii for this check. 
        /// </param>
        /// <returns></returns>
        public virtual Models.Order UpdateOrder(Models.Order order)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", order.ToJsonString()));
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: cannot add products to order in bistro mode"));
                throw new NotSupportedException("Doshii: cannot add products to order in bistro mode");
            }
            if (order.Status != "paid")
            {
                order.Status = "accepted";
            }
            Models.Order returnedOrder = new Models.Order();
            if (order.Id == 0)
            {
                order.UpdatedAt = DateTime.Now.ToString();
                try
                {
                    returnedOrder = m_HttpComs.PostOrder(order);
                    if (returnedOrder.Id == 0)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                    }
                    throw rex;
                }
                // record the order.Id in the pos so the post can put another order if more items are added from the pos. 
                m_DoshiiInterface.RecordOrderId(returnedOrder);
            }
            else
            {
                try
                {
                    returnedOrder = m_HttpComs.PutOrder(order);
                    if (returnedOrder.Id == 0)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                    }
                    throw rex;
                }
            }

            return returnedOrder;
        }

        /// <summary>
        /// This is not currently implemented as doshii can't currently respond to the payment with success or failure. 
        /// </summary>
        /// <returns></returns>
        public virtual bool AddPayment(Models.Order order)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos adding payment to order - '{0}'", order.ToJsonString()));
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: paying from the pos in bistro mode is not supported"));
                throw new NotSupportedException("Doshii: paying from the pos in bistro mode is not supported");
            }
            throw new NotImplementedException();
        }

        #endregion

        #region tableAllocation and consumers

        /// <summary>
        /// gets the consumer from doshii with the provided paypalConsumerId
        /// </summary>
        /// <param name="paypalCustomerId"></param>
        /// <returns></returns>
        public virtual Models.Consumer GetConsumer(string paypalCustomerId)
        {
            try
            {
                return m_HttpComs.GetConsumer(paypalCustomerId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// attempts to add a table allocation to doshii
        /// </summary>
        /// <param name="customerId">
        /// the payPayCustomerId for the customer to be allocated. 
        /// </param>
        /// <param name="tableName">
        /// the name of the table to be allocated.
        /// </param>
        /// <returns></returns>
        public virtual void SetTableAllocation(string customerId, string tableName)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos allocating table for customerId - '{0}', table '{1}'", customerId, tableName));
            try
            {
                m_HttpComs.PostTableAllocation(customerId, tableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        public virtual void DeleteTableAllocation(string customerId, string tableName, Enums.TableAllocationRejectionReasons deleteReason)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos DeAllocating table for customerId - '{0}', table '{1}'", customerId, tableName));
            try
            {
                m_HttpComs.RejectTableAllocation(customerId, tableName, deleteReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        #endregion

        /// <summary>
        /// returns a list of all the consumers currently in doshii. 
        /// </summary>
        /// <returns></returns>
        public virtual List<Models.Consumer> GetCheckedInConsumersFromDoshii()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all checked in users"));
            try
            {
                return m_HttpComs.GetConsumers();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

    }
}
