using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to manage the SDK to manage the business logic handling partner ordering. 
    /// </summary>
    internal class OrderingController
    {
        /// <summary>
        /// prop for the local <see cref="Controllers"/> instance. 
        /// </summary>
        internal Models.Controllers _controllers;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="httpComs"></param>
        internal OrderingController(Models.Controllers controller, HttpController httpComs)
        {
            if (controller == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllers = controller;
            if (_controllers.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (_controllers.OrderingManager == null)
            {

                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IOrderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllers.TransactionController == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - TransactionController cannot be null");
                throw new NullReferenceException("transactionController cannot be null");
            }
            if (httpComs == null)
            {
                _controllers.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        /// <summary>
        /// calls the appropriate callback method on <see cref="Interfaces.IOrderingManager"/> to record the order version.
        /// </summary>
        /// <param name="posOrderId">
        /// the PosId of the order to be recorded
        /// </param>
        /// <param name="version">
        /// the version of the order to be recorded.
        /// </param>
        internal virtual void RecordOrderVersion(string posOrderId, string version)
        {
            try
            {
                _controllers.OrderingManager.RecordOrderVersion(posOrderId, version);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Info, string.Format("Doshii: Attempted to update an order version that does not exist on the Pos, OrderId - {0}, version - {1}", posOrderId, version));
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update an order version on the pos, OrderId - {0}, version - {1}, {2}", posOrderId, version, ex.ToString()));
            }
        }

        /// <summary>
        /// This method returns an order from Doshii corresponding to the OrderId
        /// </summary>
        /// <param name="orderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns>
        /// The order with the corresponding Id
        /// <para/>If there is no order corresponding to the Id, a blank order may be returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an exception while making the request to doshii.</exception>
        internal virtual Order GetOrder(string orderId)
        {
            try
            {
                return _httpComs.GetOrder(orderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns an order from Doshii corresponding to the doshiiOrderId
        /// <para/>This will only return orders that are unlinked
        /// <para/>If doshii has already linked the order on the pos then <see cref="GetOrder"/> should be called
        /// </summary>
        /// <param name="doshiiOrderId">
        /// The Id of the order that is being requested. 
        /// </param>
        /// <returns>
        /// The order with the corresponding Id
        /// <para/>If there is no order corresponding to the Id, a blank order may be returned. 
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual OrderWithConsumer GetOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            try
            {
                return _httpComs.GetOrderFromDoshiiOrderId(doshiiOrderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method returns all order for Doshii that have been provided with a PosId, 
        /// If the order has not yet been processed by the pos and an Id has not been provided you should use <see cref="GetUnlinkedOrders"/> to retreive the order. 
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<Order> GetOrders()
        {
            try
            {
                return _httpComs.GetOrders();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// Retrieves the current unlinked order list from Doshii.
        /// </summary>
        /// <returns>
        /// The current list of orders available in Doshii.
        /// <para/>If there are no unlinkedOrders a blank IEnumerable is returned.
        /// </returns>
        /// <exception cref="DoshiiManagerNotInitializedException">Thrown when Initialize has not been successfully called before this method was called.</exception>
        internal virtual IEnumerable<OrderWithConsumer> GetUnlinkedOrders()
        {
            try
            {
                return _httpComs.GetUnlinkedOrders();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempt to update an order on Doshii
        /// </summary>
        /// <param name="order">The order to update.</param>
        /// <returns></returns>
        internal virtual Order UpdateOrder(Order order)
        {
            order.Version = _controllers.OrderingManager.RetrieveOrderVersion(order.Id);
            var jsonOrder = Mapper.Map<JsonOrder>(order);
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", jsonOrder.ToJsonString()));

            var returnedOrder = new Order();

            try
            {
                returnedOrder = _httpComs.PutOrder(order);
                if (returnedOrder.Id == "0" && returnedOrder.DoshiiId == "0")
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw new OrderUpdateException("Update order not successful", rex);
            }
            catch (NullResponseDataReturnedException Nex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
                throw new OrderUpdateException(string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id), Nex);
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0}", order.Id), ex);
            }

            return returnedOrder;
        }

        /// <summary>
        /// Confirms a pending order on Doshii
        /// </summary>
        /// <param name="order">
        /// The order to be confirmed. 
        /// </param>
        /// <returns></returns>
        internal virtual Order PutOrderCreatedResult(Order order)
        {
            if (order.Status == "accepted")
            {
                if (order.Id == null || string.IsNullOrEmpty(order.Id))
                {
                    throw new OrderUpdateException("the pos must set an order.Id for accepted orders.");
                }
            }

            var returnedOrder = new Order();

            try
            {
                returnedOrder = _httpComs.PutOrderCreatedResult(order);
                if (returnedOrder.Id == "0" && returnedOrder.DoshiiId == "0")
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                    throw new OrderUpdateException(string.Format("Doshii: order was returned from doshii without an doshiiOrderId while updating order with id {0}", order.Id));
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == HttpStatusCode.Conflict)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                    throw new ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                }
                throw new OrderUpdateException("Update order not successful", rex);
            }
            catch (NullResponseDataReturnedException Nex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
                throw new OrderUpdateException(string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id), Nex);
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0}", order.Id), ex);
            }

            return returnedOrder;
        }

        /// <summary>
        /// Checks all orders on the doshii system, 
        /// <para/>if there are any pending orders <see cref="HandleOrderCreated"/> will be called. 
        /// <para/>currently this method does not check the transactions as there should be no unlinked transactions for already created orders, order ahead only allows for 
        /// <para/>partners to make payments when they create an order else the payment is expected to be made by the customer on receipt of the order. 
        /// </summary>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown if there is an issue getting the orders from Doshii.</exception>
        internal virtual void RefreshAllOrders()
        {

            try
            {
                //check unassigned orders
                _controllers.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Info, "Refreshing all orders.");
                IEnumerable<OrderWithConsumer> unassignedOrderList;
                unassignedOrderList = GetUnlinkedOrders();
                foreach (OrderWithConsumer order in unassignedOrderList)
                {
                    if (order.Status == "pending")
                    {
                        List<Transaction> transactionListForOrder = _controllers.TransactionController.GetTransactionFromDoshiiOrderId(order.DoshiiId).ToList();
                        HandleOrderCreated(order, transactionListForOrder.ToList());
                    }
                }
                //Check assigned orders
                //This is not yet implemented as its not necessary when only OrderAhead is a possibility. 
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method calls the appropriate callback method on the <see cref="Interfaces.IOrderingManager"/> to confirm an order when an order created event is received from Doshii. 
        /// </summary>
        /// <param name="order">
        /// the order that has been created
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the new created order. 
        /// </param>
        internal virtual void HandleOrderCreated(OrderWithConsumer orderWithConsumer, List<Transaction> transactionList)
        {
            var orderWithoutConsumer = Mapper.Map<Order>(orderWithConsumer);
            if (transactionList == null)
            {
                transactionList = new List<Transaction>();
            }
            if (orderWithConsumer.Consumer == null)
            {
                _controllers.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Error, string.Format("Doshii: An order created event was received with DoshiiId - {0} but the order does not have a consumer, the Order has been rejected", orderWithoutConsumer.DoshiiId));
                RejectOrderFromOrderCreateMessage(orderWithoutConsumer, transactionList);
                return;
            }
            if (transactionList.Count > 0)
            {

                if (orderWithConsumer.Type == "delivery")
                {
                    _controllers.OrderingManager.ConfirmNewDeliveryOrderWithFullPayment(orderWithoutConsumer, orderWithConsumer.Consumer, transactionList);
                }
                else if (orderWithConsumer.Type == "pickup")
                {
                    _controllers.OrderingManager.ConfirmNewPickupOrderWithFullPayment(orderWithoutConsumer, orderWithConsumer.Consumer, transactionList);
                }
                else
                {
                    _controllers.OrderingManager.ConfirmNewUnknownTypeOrderWithFullPayment(orderWithoutConsumer, orderWithConsumer.Consumer, transactionList);
                }

            }
            else
            {
                if (orderWithConsumer.Type == "delivery")
                {

                    _controllers.OrderingManager.ConfirmNewDeliveryOrder(orderWithoutConsumer, orderWithConsumer.Consumer);
                }
                else if (orderWithConsumer.Type == "pickup")
                {
                    _controllers.OrderingManager.ConfirmNewPickupOrder(orderWithoutConsumer, orderWithConsumer.Consumer);
                }
                else
                {
                    _controllers.OrderingManager.ConfirmNewUnknownTypeOrder(orderWithoutConsumer, orderWithConsumer.Consumer);
                }

            }
        }

        /// <summary>
        /// Used to accept an Order created for a partner through the orderAhead interface. 
        /// </summary>
        /// <param name="orderToAccept"></param>
        /// <returns></returns>
        internal virtual bool AcceptOrderAheadCreation(Order orderToAccept)
        {
            OrderWithConsumer orderOnDoshii = GetOrderFromDoshiiOrderId(orderToAccept.DoshiiId);
            List<Transaction> transactionList = _controllers.TransactionController.GetTransactionFromDoshiiOrderId(orderToAccept.DoshiiId).ToList();

            //test on doshii has changed. 
            if (orderOnDoshii.Version != orderToAccept.Version)
            {
                return false;
            }

            orderToAccept.Status = "accepted";
            try
            {
                PutOrderCreatedResult(orderToAccept);
            }
            catch (Exception ex)
            {
                return false;
                //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                //if we get an error response at this point we should prob cancel the order on the pos and not continue and cancel the payments. 
            }
            //If there are transactions set to waiting and get response - should call request payment
            foreach (Transaction tran in transactionList)
            {
                _controllers.TransactionController.RecordTransactionVersion(tran);
                tran.OrderId = orderToAccept.Id;
                tran.Status = "waiting";
                try
                {
                    _controllers.TransactionController.RequestPaymentForOrderExistingTransaction(tran);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                }
            }
            return true;
        }

        /// <summary>
        /// use to reject an order created by a partner through the orderAhead interface. 
        /// </summary>
        /// <param name="orderToReject"></param>
        internal virtual void RejectOrderAheadCreation(Order orderToReject)
        {
            List<Transaction> transactionList = _controllers.TransactionController.GetTransactionFromDoshiiOrderId(orderToReject.DoshiiId).ToList();
            //test order to accept is equal to the order on doshii
            RejectOrderFromOrderCreateMessage(orderToReject, transactionList);
        }

        /// <summary>
        /// This method rejects an unlinked order on the Doshii API, the transactions related to the order will also be rejected. 
        /// </summary>
        /// <param name="order">
        /// The pending order to be rejected
        /// </param>
        /// <param name="transactionList">
        /// The transaction list to be rejected
        /// </param>
        internal virtual void RejectOrderFromOrderCreateMessage(Order order, List<Transaction> transactionList)
        {
            //set order status to rejected post to doshii
            order.Status = "rejected";
            try
            {
                PutOrderCreatedResult(order);
            }
            catch (Exception ex)
            {
                //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
            }
            foreach (Transaction tran in transactionList)
            {
                tran.Status = "rejected";
                try
                {
                    _controllers.TransactionController.RejectPaymentForOrder(tran);
                }
                catch (Exception ex)
                {
                    //although there could be an conflict exception from this method it is not currently possible for partners to update order ahead orders so for the time being we don't need to handle it. 
                }
            }
        }

        /// <summary>
        /// Calls the appropriate callback method in <see cref="Interfaces.IOrderingManager"/> to record the checkinId for an order on the pos. 
        /// </summary>
        /// <param name="order">
        /// The order that need to be recorded. 
        /// </param>
        internal virtual void RecordOrderCheckinId(string posOrderId, string checkinId)
        {
            try
            {
                _controllers.OrderingManager.RecordCheckinForOrder(posOrderId, checkinId);
            }
            catch (OrderDoesNotExistOnPosException nex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, string.Format("Doshii: Attempted to update a checkinId for an order that does not exist on the Pos, Order.id - {0}, checkinId - {1}", posOrderId, checkinId));
                //maybe we should call reject order here. 
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: Exception while attempting to update a checkinId for an order on the pos, Order.Id - {0}, checkinId - {1}, {2}", posOrderId, checkinId, ex.ToString()));
                //maybe we should call reject order here. 
            }
        }
    }
}
