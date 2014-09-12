using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace DoshiiDotNetIntegration
{
    public abstract class Doshii 
    {

        #region properties, constructors, Initialize, versionCheck

        private CommunicationLogic.DoshiiWebSocketsCommunication SocketComs = null;
        private CommunicationLogic.DoshiiHttpCommunication HttpComs = null;

        private Enums.OrderModes OrderMode;
        private Enums.SeatingModes SeatingMode;
        private string AuthorizeToken;

        protected static string CurrnetVersion()
        {
            
            var versionStringBuilder = new StringBuilder();
            versionStringBuilder.Append("Doshii Integration Version: ");
            versionStringBuilder.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            versionStringBuilder.Append(Environment.NewLine);
            
            return versionStringBuilder.ToString();
        }

        protected Doshii()
        {
        }

        protected void Initialize(string socketUrl, string token, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, bool StartWebSocketConnection)
        {
            LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Initializing Doshii with sourceUrl: {0}, token {1}, orderMode {2}, seatingMode: {3}, BaseUrl: {4}", socketUrl, token, orderMode.ToString(), seatingMode.ToString(), UrlBase));
            AuthorizeToken = token;
            string socketUrlWithToken = string.Format("{0}?token={1}", socketUrl, token);
            InitializeProcess(socketUrlWithToken, orderMode, seatingMode, UrlBase, StartWebSocketConnection);
        }

        private bool InitializeProcess(string socketUrl, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, bool StartWebSocketConnection)
        {
            LogDoshiiError(Enums.DoshiiLogLevels.Debug, "initializing Doshii");

            bool result = true;

            OrderMode = orderMode;
            SeatingMode = seatingMode;

            //generate class for http communication. 
            HttpComs = new CommunicationLogic.DoshiiHttpCommunication(UrlBase, this, AuthorizeToken);

            if (StartWebSocketConnection)
            {
                // initialize socket connection
                SocketComs = new CommunicationLogic.DoshiiWebSocketsCommunication(socketUrl, HttpComs, this);
                // subscribe to scoket events
                SubscribeToSocketEvents();
                SocketComs.initialize(GetCheckedInCustomersFromPos());
            }
                                    
            return result;
        }

        private void SubscribeToSocketEvents()
        {
            if (SocketComs == null)
            {
                LogDoshiiError(Enums.DoshiiLogLevels.Debug, "the socketComs has not been initialized");
            }
            else
            {
                SocketComs.ConsumerCheckinEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComs_ConsumerCheckinEvent);
                SocketComs.CreateOrderEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComs_CreateOrderEvent);
                SocketComs.OrderStatusEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComs_OrderStatusEvent);
                SocketComs.TableAllocationEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComs_TableAllocationEvent);
            }
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// This method should return a list of all the current doshii checked in customers registered in the pos. 
        /// </summary>
        /// <returns></returns>
        public abstract List<Modles.Consumer> GetCheckedInCustomersFromPos();

        /// <summary>
        /// This method will receive the table allocation object, and should either accept or reject the allocation. 
        /// if the allocation fails the reasoncode property should be populated. 
        /// </summary>
        /// <param name="tableAllocation"></param>
        /// <returns>
        /// true - if the allocation was successful
        /// false - if the allocation failed,
        /// </returns>
        protected abstract bool ConfirmTableAllocation(Modles.table_allocation tableAllocation);

        /// <summary>
        /// This method will receive the order that has been paid partially by doshii - this will only get called if you are using restaurant mode.
        /// </summary>
        /// <returns></returns>
        protected abstract void RecordPartialCheckPayment(Modles.order order);

        /// <summary>
        /// this method should record that a check has been fully paid by doshii. 
        /// </summary>
        /// <returns></returns>
        protected abstract void RecordFullCheckPayment(Modles.order order);

        /// <summary>
        /// this method should record that a check has been fully paid in bistro mode, the order should then be generated on the system and printed to the kitchen. 
        /// </summary>
        /// <param name="order"></param>
        protected abstract void RecordFullCheckPaymentBistroMode(Modles.order order);


        /// <summary>
        /// this method sould record that the contained order has been cancled. 
        /// </summary>
        /// <param name="order"></param>
        protected abstract void OrderCancled(Modles.order order);

        /// <summary>
        /// this method should check the availability of the products that have been ordered. 
        /// This method will only be called in bistro mode. 
        /// If the price of any of the products are incorrect or the products are not available a rejection reason should be added to the product in question. 
        /// as this is in bistro mode the order should not be formally created on the pos system untill a payment is completed and the paid message is received, but any products that have 
        /// limited quantities should be reserved for this order as there is no opportunity to reject the order after it has been accepted on this step. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// true - if the entire order was accepted
        /// false - if the any part of the order was rejected. 
        /// </returns>
        protected abstract bool ConfirmOrderAvailabilityBistroMode(Modles.order order);

        /// <summary>
        /// this method is used to check the availability of the products that have been ordered.
        /// This method will only be called in restaurant mode.
        /// if the price of any of the prodducts are incorrect or the products are not available a rejection reason should be added to the product in question
        /// As this is in restaurant mode the order should be formally created on the pos when this order is accepted, paymend is expected at the end of the customer experience at the venue rather than 
        /// with each order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        protected abstract bool ConfirmOrderForRestaurantMode(Modles.order order);

        /// <summary>
        /// this method is used to confirm the order on doshii accuratly represents the order on the the pos
        /// the pos is the source of truth for the order.
        /// if the order is not correct the pos should update the order object to to represent the correct order. 
        /// </summary>
        /// <param name="order"></param>
        protected abstract void ConfirmOrderTotalsBeforePaymentRestaurantMode(Modles.order order);

        /// <summary>
        /// this method should be used to record on the pos a customer has checked in. 
        /// REVIEW: (liam) It might be a very good idea in the constructor to be given a path where images can be saved and in the method that calls this method to save the user images for use by the pos,
        /// rather than giving the pos the URL of the image and expecting them to get the pic. 
        /// </summary>
        /// <param name="consumer"></param>
        protected abstract void recordCheckedInUser(Modles.Consumer consumer);

        /// <summary>
        /// this method should be overridden so that the doshii logs appear in the regular system logs of your system, 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public abstract void LogDoshiiError(Enums.DoshiiLogLevels logLevel, string message, Exception ex = null);

        #endregion

        #region socket communication event handlers

        private void SocketComs_TableAllocationEvent(object sender, CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs e)
        {
            Modles.table_allocation tableAllocation = new Modles.table_allocation();
            tableAllocation = e.TableAllocation;
            if (ConfirmTableAllocation(tableAllocation))
            {
                HttpComs.PutTableAllocation(tableAllocation.paypalCustomerId, tableAllocation.name);
                
            }
            else
            {
                HttpComs.RejectTableAllocation(tableAllocation.paypalCustomerId, tableAllocation.name, tableAllocation);
            }
        }

        private void SocketComs_OrderStatusEvent(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            switch (e.order.status)
            {
                case Enums.OrderStates.paid:
                    if (e.order.notPayingTotal > 0)
                    {
                        if (OrderMode == Enums.OrderModes.BistroMode)
                        {
                            throw new NotSupportedException("partial payment in bistro mode");
                        }
                        RecordPartialCheckPayment(e.order);
                    }
                    else
                    {
                        RecordFullCheckPayment(e.order);
                    }
                    break;
                case Enums.OrderStates.cancelled:
                    OrderCancled(e.order);
                    break;
                case Enums.OrderStates.pending:
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        if (ConfirmOrderAvailabilityBistroMode(e.order))
                        {
                            e.order.status = Enums.OrderStates.waitingforpayment;
                            HttpComs.PutOrder(e.order);
                        }
                        else
                        {
                            e.order.status = Enums.OrderStates.rejected;
                            HttpComs.PutOrder(e.order);
                        }
                    }
                    else
                    {
                        if (ConfirmOrderForRestaurantMode(e.order))
                        {
                            e.order.status = Enums.OrderStates.accepted;
                            HttpComs.PutOrder(e.order);
                        }
                        else
                        {
                            e.order.status = Enums.OrderStates.rejected;
                            HttpComs.PutOrder(e.order);
                        }
                    }
                    break;
                case Enums.OrderStates.readytopay:
                    ConfirmOrderTotalsBeforePaymentRestaurantMode(e.order);
                    e.order.status = Enums.OrderStates.waitingforpayment;
                    HttpComs.PutOrder(e.order);
                    break;
                default:
                    throw new NotSupportedException(e.order.status.ToString());

            }
        }

        private void SocketComs_CreateOrderEvent(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            switch (e.order.status)
            {
                case Enums.OrderStates.pending:
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        if (ConfirmOrderAvailabilityBistroMode(e.order))
                        {
                            e.order.status = Enums.OrderStates.waitingforpayment;
                            HttpComs.PutOrder(e.order);
                        }
                        else
                        {
                            e.order.status = Enums.OrderStates.rejected;
                            HttpComs.PutOrder(e.order);
                        }
                    }
                    else
                    {
                        if (ConfirmOrderForRestaurantMode(e.order))
                        {
                            e.order.status = Enums.OrderStates.accepted;
                            HttpComs.PutOrder(e.order);
                        }
                        else
                        {
                            e.order.status = Enums.OrderStates.rejected;
                            HttpComs.PutOrder(e.order);
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(e.order.status.ToString());
            }
        }

        private void SocketComs_ConsumerCheckinEvent(object sender, CommunicationLogic.CommunicationEventArgs.CheckInEventArgs e)
        {
            recordCheckedInUser(e.consumerObject);
        }

        #endregion
        
        #region product sync methods

        public List<Modles.product> GetAllProducts()
        {
            return HttpComs.GetDoshiiProducts();
        }

        public bool AddNewProducts(List<Modles.product> productList)
        {
            bool success = false;
            success = HttpComs.PostProductData(productList, true);
            return success;
        }

        public bool AddNewProducts(Modles.product productToUpdate, bool deleteAllProductsCurrentlyOnDoshii)
        {
            bool success = false;
            List<Modles.product> productList = new List<Modles.product>();
            productList.Add(productToUpdate);
            success = HttpComs.PostProductData(productList, deleteAllProductsCurrentlyOnDoshii);
            return success;
        }

        public bool UpdateProcucts(Modles.product productToUpdate)
        {
            bool success = false;
            success = HttpComs.PutProductData(productToUpdate);
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
            bool success = false;
            foreach (string pro in productIdList)
            {
                success = DeleteProducts(pro);
            }
            return success;
        }

        public bool DeleteProducts(string productId)
        {
            bool success = false;
            success = HttpComs.DeleteProductData(productId);
            return success;
        }

        public bool DeleteAllProducts()
        {
            bool success = false;
            success = HttpComs.DeleteProductData();
            return success;
        }

        #endregion

        #region ordering And Payment

        public bool AddItemsToOrder(Modles.order order)
        {
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                return false;
            }
            order.status = Enums.OrderStates.accepted;
            return HttpComs.PutOrder(order);
        }

        public bool AddPayment()
        {
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        #endregion

        #region tableAllocation

        public bool TableAllocationConfirmDoshiiAllocation(string customerId, string tableName)
        {
            bool success = false;
            if (SeatingMode == Enums.SeatingModes.DoshiiAllocation)
            {
                success = HttpComs.PutTableAllocation(customerId, tableName);
            }
            else
            {
                success = false;
            }
            return success;
            
        }

        public bool TableAllocationFromPos(string customerId, string tableName)
        {
            bool success = false;
            if (SeatingMode == Enums.SeatingModes.DoshiiAllocation)
            {
                success = false;
            }
            else
            {
                success = HttpComs.PutTableAllocation(customerId, tableName);
            }
            return success;

        }

        #endregion

        public List<Modles.Consumer> GetCheckedInConsumersFromDoshii()
        {
            return HttpComs.GetConsumers();
        }

    }
}
