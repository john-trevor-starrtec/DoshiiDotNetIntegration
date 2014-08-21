using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DoshiiDotNetIntegration
{
    public class Doshii : LoggingBase
    {

        #region properties, constructors, Initialize, versionCheck

        private CommunicationLogic.DoshiiWebSocketsCommunication SocketComs;
        private CommunicationLogic.DoshiiHttpCommunication HttpComs;

        private Enums.OrderModes OrderMode;
        private Enums.SeatingModes SeatingMode;


        public static string CurrnetVersion()
        {

            var versionStringBuilder = new StringBuilder();
            versionStringBuilder.Append("Doshii Integration Version: ");
            versionStringBuilder.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            versionStringBuilder.Append(Environment.NewLine);
            versionStringBuilder.Append("Log4Net.Net version: ");
            versionStringBuilder.Append(log4net.AssemblyInfo.Version);
            versionStringBuilder.Append(Environment.NewLine);
           
            return versionStringBuilder.ToString();
        }

        public bool initialize(string socketUrl, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, List<Modles.Consumer> currentlyCheckInConsumers)
        {
            bool result = false;

            OrderMode = orderMode;
            SeatingMode = seatingMode;

            //generate class for http communication. 
            HttpComs = new CommunicationLogic.DoshiiHttpCommunication(UrlBase);

            // initialize socket connection
            SocketComs = new CommunicationLogic.DoshiiWebSocketsCommunication(socketUrl, HttpComs);
            // subscribe to scoket events
            SubscribeToSocketEvents();
            SocketComs.initialize(currentlyCheckInConsumers);
            
            return result;
        }

        private void SubscribeToSocketEvents()
        {
            if (SocketComs == null)
            {
                log.Error("the socketComs has not been initialized");
            }
            else
            {
                SocketComs.ConsumerCheckinEvent += new EventHandler<CommunicationLogic.CommunicationEventArgs.CheckInEventArgs>(SocketComs_ConsumerCheckinEvent);
                SocketComs.CreateOrderEvent += new EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs>(SocketComs_CreateOrderEvent);
                SocketComs.OrderStatusEvent += new EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs>(SocketComs_OrderStatusEvent);
                SocketComs.TableAllocationEvent += new EventHandler<CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs>(SocketComs_TableAllocationEvent);
            }
        }

        #endregion

        #region events and eventHandlers

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> CheckFullyPaidEvent;

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> CheckPartiallyPaidEvent;

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> OrderCancledByClientEvent;

        /// <summary>
        /// this event should be used in bistro mode when an order is received from doshii, 
        /// the order should not be formally created on the pos untill a successfull payment event is received for this order
        /// </summary>
        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> OrderAvailabilityConfirmationEvent;

        /// <summary>
        /// this event will only be fired in restaurant mode and can be formally created by the pos when it confirms the availability of the items ordered,
        /// as the payment will not be completed untill the customer is finished at the venue. 
        /// </summary>
        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> RestaurantOrderConfirmationEvent;

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.OrderEventArgs> ConfirmCheckAmountForPaymentEvent;

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs> TableAllocationEvent;

        public event EventHandler<CommunicationLogic.CommunicationEventArgs.CheckInEventArgs> ConsumerCheckinEvent;

        #region events
        #endregion
        #region socket communication event handlers

        void SocketComs_TableAllocationEvent(object sender, CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs e)
        {
            TableAllocationEvent(this, e);
        }

        void SocketComs_OrderStatusEvent(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            switch (e.order.status)
            {
                case Enums.OrderStates.paid:
                    if (e.order.notPayingTotal > 0)
                    {
                        CheckPartiallyPaidEvent(this, e);
                    }
                    else
                    {
                        CheckPartiallyPaidEvent(this, e);
                    }
                    break;
                case Enums.OrderStates.cancelled:
                    OrderCancledByClientEvent(this, e);
                    break;
                case Enums.OrderStates.pending:
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        OrderAvailabilityConfirmationEvent(this, e);
                    }
                    else
                    {
                        RestaurantOrderConfirmationEvent(this, e);
                    }
                    break;
                case Enums.OrderStates.readytopay:
                    ConfirmCheckAmountForPaymentEvent(this, e);
                    break;
                default:
                    throw new NotSupportedException(e.order.status.ToString());

            }
        }

        void SocketComs_CreateOrderEvent(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            switch (e.order.status)
            {
                case Enums.OrderStates.pending:
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        OrderAvailabilityConfirmationEvent(this, e);
                    }
                    else
                    {
                        RestaurantOrderConfirmationEvent(this, e);
                    }
                    break;
                default:
                    throw new NotSupportedException(e.order.status.ToString());
            }
        }

        void SocketComs_ConsumerCheckinEvent(object sender, CommunicationLogic.CommunicationEventArgs.CheckInEventArgs e)
        {
            ConsumerCheckinEvent(this, e);
        }

        #endregion
        #endregion




        #region menuUpload

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

        public bool AddNewProducts(Modles.product productToUpdate)
        {
            bool success = false;
            success = HttpComs.PostProductData(productToUpdate, true);
            return success;
        }

        public bool UpdateProcucts(Modles.product productToUpdate)
        {
            bool success = false;
            success = HttpComs.PostProductData(productToUpdate, false);
            return success;
        }
        
        /// <summary>
        /// deltes the provided list of products from doshii. 
        /// REVIEW:(Liam) this really needs to have a better method for measuring sucess, and a message about why things failed...
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public bool DeleteProducts(List<Modles.product> productList)
        {
            bool success = false;
            foreach (Modles.product pro in productList)
            {
                success = DeleteProducts(pro);
            }
            return success;
        }

        public bool DeleteProducts(Modles.product productToUpdate)
        {
            bool success = false;
            success = HttpComs.DeleteProductData(productToUpdate.pos_Id);
            return success;
        }

        public bool DeleteAllProducts()
        {
            bool success = false;
            success = HttpComs.DeleteProductData();
            return success;
        }

        #endregion

        #region individual product upload
        
        #endregion

        #endregion

        #region ordering And Payment

        public bool ConfirmOrder(Modles.order order)
        {
            order.status = Enums.OrderStates.accepted;
            return HttpComs.PutOrder(order);
        }

        public bool RejectOrder(Modles.order order)
        {
            order.status = Enums.OrderStates.rejected;
            return HttpComs.PutOrder(order);
            
        }

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

        public bool ConfirmPaymentFromDoshii()
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region tableAllocation

        public bool ConfirmTableAllocation()
        {
            if (SeatingMode == Enums.SeatingModes.PosAllocation)
            {
                return false;
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public bool AllocateTableFromPos()
        {
            if (SeatingMode == Enums.SeatingModes.DoshiiAllocation)
            {
                return false;
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        #endregion

    }
}
