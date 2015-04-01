using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
    public interface iDoshiiOrdering
    {
        /// <summary>
        /// This method should return a list of all the current doshii checked in customers registered in the pos. 
        /// </summary>
        /// <returns></returns>
        List<Models.Consumer> GetCheckedInCustomersFromPos();

        /// <summary>
        /// this method should return the order associated with the checkin Id paramater
        /// the method should return null if there is no order associated with the checkinId
        /// this method will be called when the order mode is changed from restaurant mode to bistro mode and there are active checkins
        /// a payment will then be requested for the active orders from doshii so there are no open restaurant orders when the integration is in bistro mode. 
        /// </summary>
        /// <returns></returns>
        Models.Order GetOrderForCheckinId(string CheckinId);

        /// <summary>
        /// This method will receive the table allocation object, and should either accept or reject the allocation. 
        /// if the allocation fails the reasoncode property should be populated. 
        /// </summary>
        /// <param name="tableAllocation"></param>
        /// <returns>
        /// true - if the allocation was successful
        /// false - if the allocation failed,
        /// </returns>
        bool ConfirmTableAllocation(ref Models.TableAllocation tableAllocation);

        /// <summary>
        /// this method should set the customer relating to the paypal customer id that is passed in as no longer at the venue. 
        /// </summary>
        /// <param name="consumerId"></param>
        void CheckOutConsumer(string consumerId);

        /// <summary>
        /// This method will receive the order that has been paid partially by doshii - this will only get called if you are using restaurant mode.
        /// </summary>
        /// <returns></returns>
        bool RecordPartialCheckPayment(ref Models.Order order);

        /// <summary>
        /// this method should record that a check has been fully paid by doshii. 
        /// </summary>
        /// <returns></returns>
        bool RecordFullCheckPayment(ref Models.Order order);

        /// <summary>
        /// this method should record that a check has been fully paid in bistro mode, the order should then be generated on the system and printed to the kitchen. 
        /// </summary>
        /// <param name="order"></param>
        bool RecordFullCheckPaymentBistroMode(ref Models.Order order);


        /// <summary>
        /// this method sould record that the contained order has been cancled. 
        /// </summary>
        /// <param name="order"></param>
        void OrderCancled(ref Models.Order order);

        /// <summary>
        /// this method should will be called when the web sockets communication is down for longer than the timeout period,
        /// this method must dissociate all current doshii checks / tabs / checkins from currently opened checks. 
        /// </summary>
        void DissociateDoshiiChecks();

        /// <summary>
        /// this method should check the availability of the products that have been ordered. 
        /// This method will only be called in bistro mode. 
        /// If the price of any of the products are incorrect or the products are not available a rejection reason should be added to the product in question. 
        /// as this is in bistro mode the order should not be formally created on the pos system untill a payment is completed and the paid message is received, but any products that have 
        /// limited quantities should be reserved for this order as there is no opportunity to reject the order after it has been accepted on this step. 
        /// 
        /// This may be the first time the pos receives the orderId for this order, so the pos needs to check if it already has the order id recorded against the order and if not it should record the order against the order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// true - if the entire order was accepted
        /// false - if the any part of the order was rejected. 
        /// </returns>
        bool ConfirmOrderAvailabilityBistroMode(ref Models.Order order);

        /// <summary>
        /// this method is used to check the availability of the products that have been ordered.
        /// This method will only be called in restaurant mode.
        /// if the price of any of the prodducts are incorrect or the products are not available a rejection reason should be added to the product in question
        /// As this is in restaurant mode the order should be formally created on the pos when this order is accepted, paymend is expected at the end of the customer experience at the venue rather than 
        /// with each order. 
        /// 
        /// This may be the first time the pos receives the orderId for this order, so the pos needs to check if it already has the order id recorded against the order and if not it should record the order against the order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool ConfirmOrderForRestaurantMode(ref Models.Order order);

        /// <summary>
        /// this method is used to confirm the order on doshii accuratly represents the order on the the pos
        /// the pos is the source of truth for the order.
        /// if the order is not correct the pos should update the order object to to represent the correct order. 
        /// If the order cannot be located on the pos the pos should throw a 
        /// </summary>
        /// <param name="order"></param>
        void ConfirmOrderTotalsBeforePaymentRestaurantMode(ref Models.Order order);

        /// <summary>
        /// this method should be used to record on the pos a customer has checked in. 
        /// REVIEW: (liam) It might be a very good idea in the constructor to be given a path where images can be saved and in the method that calls this method to save the user images for use by the pos,
        /// rather than giving the pos the URL of the image and expecting them to get the pic. 
        /// </summary>
        /// <param name="consumer"></param>
        void recordCheckedInUser(ref Models.Consumer consumer);

        /// <summary>
        /// this method should be overridden so that the doshii logs appear in the regular system logs of your system, 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void LogDoshiiMessage(Enums.DoshiiLogLevels logLevel, string message, Exception ex = null);

        /// <summary>
        /// this method should record the order updatedAt time, The updated at time is used as a signature to prevent concurance issue between the pos and the doshii service. 
        /// </summary>
        /// <param name="order"></param>
        void RecordOrderUpdatedAtTime(Models.Order order);

        /// <summary>
        /// this method should record the order.Id, the order id is necessary when put is called to update an order with doshii,
        /// after post is called to update an order where the initial order is made from the pos this method will be called to set the order.id on the pos so the next time the order is updated from the pos the 
        /// order.id will be available so put can be used. 
        /// </summary>
        /// <param name="order"></param>
        void RecordOrderId(Models.Order order);

        /// <summary>
        /// this method should retreive the order updatedAt time, the updated at time is used a a signature to prevent concurance issues between the pos the the doshii service. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        string GetOrderUpdatedAtTime(Models.Order order);

        /// <summary>
        /// this method should be used to update an order on the Doshii service when it has been changed on the pos,
        /// when implementing this method doshiiLodic.UpdateOrder MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Models.Order UpdateDoshiiOrder(DoshiiOperationLogic doshiiLogic, Models.Order order);

        /// <summary>
        /// this method should be used to set a table allocation from the pos when the pos is set to allocation mode. 
        /// when implementing this method dhoshiiLogic.SetTableAllocation MUST be called. 
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        bool SetTableAllocation(DoshiiOperationLogic doshiiLogic, string customerId, string tableName);

        /// <summary>
        /// this method should be used to get all the checkedInConsumersFromDoshii. 
        /// when implementing this method dhoshiiLogic.GetCheckedInConsumersFromDoshii MUST be called. 
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <returns></returns>
        List<Models.Consumer> GetCheckedInConsumersFromDoshii(DoshiiOperationLogic doshiiLogic);

    }
}
