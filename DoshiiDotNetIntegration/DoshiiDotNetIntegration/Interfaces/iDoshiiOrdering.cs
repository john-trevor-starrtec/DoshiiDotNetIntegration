using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// This interface allows the pos to implement ALL ordering operations available in the Doshii integration.  
    /// </summary>
    public interface iDoshiiOrdering
    {
        /// <summary>
        /// This method should return a list of all the current doshii checked in customers registered in the Pos. 
        /// </summary>
        /// <returns></returns>
        List<Models.Consumer> GetCheckedInCustomersFromPos();

        /// <summary>
        /// This method should return the order associated with the CheckIn Id parameter
        /// The method should return null if there is no order associated with the checkinId
        /// </summary>
        /// <returns></returns>
        Models.Order GetOrderForCheckinId(string CheckinId);

        /// <summary>
        /// This method will receive the table allocation object, and should either accept or reject the allocation. 
        /// if the allocation fails the ReasonCode property on the table allocation object should be set. 
        /// </summary>
        /// <param name="tableAllocation"></param>
        /// <returns>
        /// true - if the allocation was successful
        /// false - if the allocation failed,
        /// </returns>
        bool ConfirmTableAllocation(ref Models.TableAllocation tableAllocation);

        /// <summary>
        /// This method should set the consumer relating to the MeerKatConsumerId that is passed in as no longer at the venue.
        /// If the consumer has an open Doshii tab the tab /check/ order should be disassociated from Doshii and become a native pos tab / order /check
        /// </summary>
        /// <param name="consumerId"></param>
        void CheckOutConsumer(string consumerId);

        /// <summary>
        /// This method should set the consumer relating to the CheckInId parameter as no longer at the venue
        /// If the consumer has an open Doshii tab the tab /check/ order should be disassociated from Doshii and become a native pos tab / order /check
        /// </summary>
        /// <param name="checkInId"></param>
        void CheckOutConsumerWithCheckInId(string checkInId);

        /// <summary>
        /// This method will receive the order that has been paid partially by doshii - this will only get called if you are using restaurant mode or if you have switched from restaurant mode to bistro mode when there are opened orders.
        /// The amount that has been paid from doshii is represented in the PayTotal property and is represented in cents. 
        /// /// After this method has been called the Order will be closed on Doshii and the SDK will call checkoutConsumerWithCheckInID to dicsaciate the order / tab / check from Doshii 
        /// If there is an unpaid amount on the check / tab / order at this point it must be dealt with from the pos. 
        /// </summary>
        /// <returns></returns>
        bool RecordPartialCheckPayment(ref Models.Order order);

        /// <summary>
        /// this method should record that a check has been fully paid by doshii. 
        /// The amount that has been paid from doshii is represented in the PayTotal property and is represented in cents. 
        /// After this method has been called the Order will be closed on Doshii and the SDK will call checkoutConsumerWithCheckInID to dicsaciate the order / tab / check from Doshii 
        /// If there is an unpaid amount on the check / tab / order at this point it must be dealt with from the pos. 
        /// </summary>
        /// <returns></returns>
        bool RecordFullCheckPayment(ref Models.Order order);

        /// <summary>
        /// This method should record that a check has been fully paid in bistro mode.
        /// This method needs to record the payment against the check and process the order. 
        /// After this method has been called the Order will be closed on Doshii so the table Allocation should be removed from the pos within this method
        /// </summary>
        /// <param name="order"></param>
        bool RecordFullCheckPaymentBistroMode(ref Models.Order order);


        /// <summary>
        /// This method should record that the order has been canceled.  
        /// </summary>
        /// <param name="order"></param>
        void OrderCancled(ref Models.Order order);

        /// <summary>
        /// This method will be called when the web sockets communication is down for longer than the timeout period - passed into DoshiiOperationLogics constructor. 
        /// This method must dissociate all current doshii checks / tabs / orders from the Doshii integration and convert them to native pos tabs. 
        /// </summary>
        void DissociateDoshiiChecks();

        /// <summary>
        /// This method should check the availability of the requested order. 
        /// The pos must check the prices of the items are correct, 
        /// The pos must check the products that have been requested are available to be sold. 
        /// This method will only be called in bistro mode. 
        /// If a product is rejected a rejection reason should be added to the product on the order and false should be returned. 
        /// As this is in bistro mode the order should not be created on the pos system until a payment is completed and the paid message is received, but any products that have 
        /// limited quantities should be reserved for this order as there is no opportunity to reject the order after it has been accepted on this step. 
        /// 
        /// This may be the first time the pos receives the orderId for this order, so the pos needs to check if it already has the order id recorded against the order and if not it should record the OrderId against the order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// true - if the entire order was accepted
        /// false - if the any part of the order was rejected. 
        /// </returns>
        bool ConfirmOrderAvailabilityBistroMode(ref Models.Order order);

        /// <summary>
        /// This method is used to check the availability of the products that have been ordered.
        /// The pos must check the prices of the items are correct, 
        /// The pos must check the products that have been requested are available to be sold. 
        /// This method will only be called in restaurant mode.
        /// If a product is rejected a rejection reason should be added to the product on the order and false should be returned. 
        /// As this is in restaurant mode the order should be formally created on the pos when this order is accepted, payment is expected at the end of the consumers experience at the venue rather than 
        /// with each order. 
        /// 
        /// This may be the first time the pos receives the orderId for this order, so the pos needs to check if it already has the order id recorded against the order and if not it should record the order against the order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool ConfirmOrderForRestaurantMode(ref Models.Order order);

        /// <summary>
        /// This method is used to confirm the order on doshii accurately represents the order on the pos.
        /// If the order is not correct the pos should update the order object to represent the correct order. 
        /// If the order cannot be located on the pos the pos should throw an Exceptions.OrderDoesNotExistOnPosException
        /// </summary>
        /// <param name="order"></param>
        void ConfirmOrderTotalsBeforePaymentRestaurantMode(ref Models.Order order);

        /// <summary>
        /// This method should be used to record the checked in consumer on the pos. 
        /// </summary>
        /// <param name="consumer"></param>
        void RecordCheckedInUser(ref Models.Consumer consumer);

        /// <summary>
        /// This method should log Doshii log messages in the pos logger
        /// This is the method that records all doshii logs, There is no separate file created for Doshii logs they should be logged by the Pos implementing the integration. 
        /// Please check Enums.DoshiiLogLevels for the different log levels implemented by doshii. 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void LogDoshiiMessage(Enums.DoshiiLogLevels logLevel, string message, Exception ex = null);

        /// <summary>
        /// This method should record the order updatedAt time, 
        /// The updated at time is used in all order updates with Doshii to prevent concurrence issue between the pos and the doshii service. 
        /// </summary>
        /// <param name="order"></param>
        void RecordOrderUpdatedAtTime(Models.Order order);

        /// <summary>
        /// This method should record the order.Id, the order id is necessary to update all orders that currently exist on Doshii,
        /// </summary>
        /// <param name="order"></param>
        void RecordOrderId(Models.Order order);

        /// <summary>
        /// This method should retrieve the order updatedAt time for the order,
        /// The updated at time is used in all order updates with Doshii to prevent concurrence issue between the pos and the doshii service.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        string GetOrderUpdatedAtTime(Models.Order order);

        /// <summary>
        /// This method should be used to update an order on the Doshii service when it has been changed on the pos,
        /// When implementing this method doshiiLodic.UpdateOrder MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Models.Order UpdateDoshiiOrder(DoshiiManagement doshiiLogic, Models.Order order);

        /// <summary>
        /// This method should be used to set a table allocation from the pos when the integration is set to PosAllocation mode. 
        /// When implementing this method dhoshiiLogic.SetTableAllocation MUST be called. 
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        bool SetTableAllocation(DoshiiManagement doshiiLogic, string customerId, string tableName);

        /// <summary>
        /// This method should be used to get all the checkedInConsumersFromDoshii. 
        /// When implementing this method dhoshiiLogic.GetCheckedInConsumersFromDoshii MUST be called. 
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <returns></returns>
        List<Models.Consumer> GetCheckedInConsumersFromDoshii(DoshiiManagement doshiiLogic);

    }
}
