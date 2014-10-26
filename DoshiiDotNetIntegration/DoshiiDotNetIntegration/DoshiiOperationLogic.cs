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
        private CommunicationLogic.DoshiiWebSocketsCommunication m_SocketComs = null;

        /// <summary>
        /// holds the class for interacting with the Doshii http restful api
        /// </summary>
        private CommunicationLogic.DoshiiHttpCommunication m_HttpComs = null;

        /// <summary>
        /// holds the interface for doshiis interaction with the pos. 
        /// </summary>
        internal Interfaces.iDoshiiOrdering m_DoshiiInterface = null;

        /// <summary>
        /// the order mode for the venue
        /// </summary>
        private Enums.OrderModes OrderMode{ get; set; }

        /// <summary>
        /// the seating mode for the venue
        /// </summary>
        private Enums.SeatingModes SeatingMode { get; set; }
        
        /// <summary>
        /// the authentication token for the venue 
        /// </summary>
        private string AuthorizeToken { get; set; }

        /// <summary>
        /// holds a value indicating if table allocations should be removed after a full check payment. 
        /// </summary>
        private bool RemoveTableAllocationsAfterFullPayment { get; set; }

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
        public void Initialize(string socketUrl, string token, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string urlBase, bool startWebSocketConnection, bool removeTableAllocationsAfterFullPayment)
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

            RemoveTableAllocationsAfterFullPayment = removeTableAllocationsAfterFullPayment;
            AuthorizeToken = token;
            string socketUrlWithToken = string.Format("{0}?token={1}", socketUrl, token);
            InitializeProcess(socketUrlWithToken, orderMode, seatingMode, urlBase, startWebSocketConnection);
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
        private bool InitializeProcess(string socketUrl, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, bool StartWebSocketConnection)
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
                    m_SocketComs = new CommunicationLogic.DoshiiWebSocketsCommunication(socketUrl, this);
                    SubscribeToSocketEvents();
                    m_SocketComs.Initialize();
                }
                catch (Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Initializing Doshii failed"));
                }
                
            }
        
            return result;
        }

        public bool CreateTableAllocation(string paypalCustomerId, string tableName)
        {
            return m_HttpComs.PostTableAllocation(paypalCustomerId, tableName);
        }

        /// <summary>
        /// refreshes the current consumer checkins, allocations and orders. 
        /// </summary>
        public void RefreshConsumerData()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Refreshing consumers, allocation, and orders");
            List<Models.Consumer> currentlyCheckInConsumers = m_DoshiiInterface.GetCheckedInCustomersFromPos();
            List<Models.TableAllocation> initialTableAllocationList = m_HttpComs.GetTableAllocations(); 
            foreach (Models.TableAllocation ta in initialTableAllocationList)
            {
                if (ta.Status == "waiting_for_confirmation")
                {
                    CommunicationLogic.CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckInEventArgs();

                    newCheckinEventArgs.Consumer = m_HttpComs.GetConsumer(ta.PaypalCustomerId);

                    newCheckinEventArgs.Consumer.CheckInId = ta.Checkin.Id;
                    newCheckinEventArgs.CheckIn = ta.Checkin.Id;
                    newCheckinEventArgs.PaypalCustomerId = ta.PaypalCustomerId;
                    newCheckinEventArgs.Uri = newCheckinEventArgs.Consumer.PhotoUrl;
                    SocketComsConsumerCheckinEventHandler(this, newCheckinEventArgs);

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
            }
            //remove consumers that are not checked in. 
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
                    CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs();

                    checkOutEventArgs.ConsumerId = cus.PaypalCustomerId;

                    SocketComsCheckOutEventHandler(this, checkOutEventArgs);
                }
            }
            // get any pending orders
            List<Models.Order> initialOrderList = m_HttpComs.GetOrders();
            foreach (Models.Order order in initialOrderList)
            {
                m_DoshiiInterface.RecordOrderUpdatedAtTime(order);
                if (order.Status == "pending" || order.Status == "ready to pay" || order.Status == "cancelled")
                {
                    Models.Order orderToConfirm = m_HttpComs.GetOrder(order.Id.ToString());
                    CommunicationLogic.CommunicationEventArgs.OrderEventArgs args = new CommunicationLogic.CommunicationEventArgs.OrderEventArgs();
                    args.Order = orderToConfirm;
                    args.OrderId = orderToConfirm.Id.ToString();
                    args.status = orderToConfirm.Status;
                    SocketComsOrderStatusEventHandler(this, args);
                }
            }

            // REVIEW: (LIAM) -update all the current consumer orders with doshii - i'm not sure if this is necessary as the pos should be retrying order updates if the order update fails. 
        }

        /// <summary>
        /// subcribs to the socket communication events 
        /// </summary>
        private void SubscribeToSocketEvents()
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
            }
        }

        /// <summary>
        /// unsubcribs to the socket communication events 
        /// </summary>
        private void UnsubscribeFromSocketEvents()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: unscribing from socket events");
            m_SocketComs.ConsumerCheckinEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComsConsumerCheckinEventHandler);
            m_SocketComs.CreateOrderEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.OrderStatusEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.TableAllocationEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComsTableAllocationEventHandler);
            m_SocketComs.CheckOutEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CheckOutEventHandler(SocketComsCheckOutEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
        }
        #endregion

        #region socket communication event handlers

        /// <summary>
        /// handles the consumer checked event and raises a consumer checkOut event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketComsCheckOutEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs e)
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
        private void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            RefreshConsumerData();
        }

        /// <summary>
        /// handles a SocketComs_TableAllocationEvent established event and confirms or rejects the event with doshii 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketComsTableAllocationEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs e)
        {
            Models.TableAllocation tableAllocation = new Models.TableAllocation();
            tableAllocation = e.TableAllocation;
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received table allocation event for consumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
            if (m_DoshiiInterface.ConfirmTableAllocation(ref tableAllocation))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: confirming table allocaiton forconsumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.PutTableAllocation(tableAllocation.PaypalCustomerId, tableAllocation.Id);
                
            }
            else
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocaiton forconsumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.PaypalCustomerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.RejectTableAllocation(tableAllocation.PaypalCustomerId, tableAllocation.Id, tableAllocation);
            }
        }

        /// <summary>
        /// handles a SocketComs_OrderStatusEvent, confirms the order, and raises an appropriate event.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketComsOrderStatusEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
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
                    e.Order.Status = "waiting for payment";
                    returnedOrder = m_HttpComs.PutOrder(e.Order);
                    if (returnedOrder.Id != null && returnedOrder.Id == e.Order.Id)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order put for payment - '{0}'", e.Order.ToJsonString())); 
                        int nonPayingAmount = 0;
                        int.TryParse(e.Order.NotPayingTotal, out nonPayingAmount);

                        if (nonPayingAmount > 0)
                        {
                            if (OrderMode == Enums.OrderModes.BistroMode)
                            {
                                if (m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref e.Order) && RemoveTableAllocationsAfterFullPayment)
                                {
                                    m_HttpComs.DeleteTableAllocationWithCheckInId(e.Order.CheckinId);
                                }
                            }
                            else
                            {
                                m_DoshiiInterface.RecordPartialCheckPayment(ref e.Order);
                            }
                            
                        }
                        else
                        {
                            if (m_DoshiiInterface.RecordFullCheckPayment(ref e.Order) && RemoveTableAllocationsAfterFullPayment)
                            {
                                m_HttpComs.DeleteTableAllocationWithCheckInId(e.Order.CheckinId);
                            }
                        }
                    }
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
                            if (returnedOrder.Id != null && returnedOrder.Id == e.Order.Id)
                            {
                                returnedOrder.Status = "waiting for payment";
                                returnedOrder = m_HttpComs.PutOrder(returnedOrder);
                                if (returnedOrder.Id != null && returnedOrder.Id == e.Order.Id)
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
                                            m_HttpComs.DeleteTableAllocationWithCheckInId(e.Order.CheckinId);
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
        }


        /// <summary>
        /// handles the SocketComs_ConsumerCheckinEvent and records the checked in user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketComsConsumerCheckinEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckInEventArgs e)
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
        public List<Models.Product> GetAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all doshii products")); 
            return m_HttpComs.GetDoshiiProducts();
        }

        /// <summary>
        /// Adds a list of products to Doshii
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public bool AddNewProducts(List<Models.Product> productList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos adding new product list- '{0}'", JsonConvert.SerializeObject(productList))); 
            bool success = false;
            success = m_HttpComs.PostProductData(productList, false);
            return success;
        }

        /// <summary>
        /// adds a single product to doshii
        /// This method will delete the current product on doshii and replace it with the paramater if it already exists. 
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <param name="deleteAllProductsCurrentlyOnDoshii"></param>
        /// <returns></returns>
        internal bool AddNewProducts(Models.Product productToUpdate, bool deleteAllProductsCurrentlyOnDoshii)
        {
            bool success = false;
            //DeleteProduct(productToUpdate.PosId);
            List<Models.Product> productList = new List<Models.Product>();
            productList.Add(productToUpdate);
            success = m_HttpComs.PostProductData(productList, deleteAllProductsCurrentlyOnDoshii);
            return success;
        }

        /// <summary>
        /// updates a product in doshii.
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        public bool UpdateProcuct(Models.Product productToUpdate)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos updating product - '{0}'", productToUpdate.ToJsonStringForProductSync()));
            bool success = false;
            success = m_HttpComs.PutProductData(productToUpdate);
            return success;
        }
        
        /// <summary>
        /// deltes the provided list of products from doshii. 
        /// REVIEW:(Liam) this really needs to have a better method for measuring sucess, and a message about why things failed...
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public bool DeleteProducts(List<string> productIdList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product list- '{0}'", JsonConvert.SerializeObject(productIdList)));
            bool success = false;
            foreach (string pro in productIdList)
            {
                success = DeleteProduct(pro);
            }
            return success;
        }

        /// <summary>
        /// delete a product from doshii
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        internal bool DeleteProduct(string productId)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product from doshii - '{0}'", productId));
            bool success = false;
            success = m_HttpComs.DeleteProductData(productId);
            return success;
        }

        /// <summary>
        /// deletes all the products from doshii
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting all products from doshii"));
            
            bool success = false;
            success = m_HttpComs.DeleteProductData();
            return success;
        }

        #endregion

        #region ordering And Payment

        /// <summary>
        /// gets an order from doshii with the provided orderId, if there no order is no order matching the provided orderId on doshii a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Models.Order GetOrder(string orderId)
        {
            return m_HttpComs.GetOrder(orderId);
        }


        /// <summary>
        /// adds items to a doshii order
        /// </summary>
        /// <param name="order">
        /// The order must contain all the products included in the check as this method overwrites all the items recorded on doshii for this check. 
        /// </param>
        /// <returns></returns>
        public Models.Order UpdateOrder(Models.Order order)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos adding items to order - '{0}'", order.ToJsonString()));
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: cannot add products to order in bistro mode"));
                throw new NotSupportedException("Doshii: cannot add products to order in bistro mode");
            }
            order.Status = "accepted";
            Models.Order returnedOrder = new Models.Order();
            if (order.Id == null || order.Id == 0)
            {
                returnedOrder = m_HttpComs.PostOrder(order);
                if (returnedOrder.Id != null && returnedOrder.Id != 0)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                }
            }
            else
            {
                returnedOrder = m_HttpComs.PutOrder(order);
                if (returnedOrder.Id != null && returnedOrder.Id != 0)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                }
            }

            return returnedOrder;
        }

        /// <summary>
        /// This is not currently implemented as doshii can't currently respond to the payment with success or failure. 
        /// </summary>
        /// <returns></returns>
        private bool AddPayment(Models.Order order)
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
        public Models.Consumer GetConsumer(string paypalCustomerId)
        {
            return m_HttpComs.GetConsumer(paypalCustomerId);
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
        public bool SetTableAllocation(string customerId, string tableName)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos allocating table for customerId - '{0}', table '{1}'", customerId, tableName));
            bool success = false;
            if (SeatingMode == Enums.SeatingModes.DoshiiAllocation)
            {
                success = false;
            }
            else
            {
                success = m_HttpComs.PostTableAllocation(customerId, tableName);
            }
            return success;

        }

        #endregion

        /// <summary>
        /// returns a list of all the consumers currently in doshii. 
        /// </summary>
        /// <returns></returns>
        public List<Models.Consumer> GetCheckedInConsumersFromDoshii()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all checked in users"));
            return m_HttpComs.GetConsumers();
        }

    }
}
