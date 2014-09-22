using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;

namespace DoshiiTesting
{
    public class DoshiiHelper : DoshiiDotNetIntegration.Doshii
    {

        private List<Consumer> MyConsumerList = new List<Consumer>();

        public List<Product> MyCompleteProductList = new List<Product>();

        public List<Product> DoshiiProductList = new List<Product>();
        
        public DoshiiHelper(string socketUrl, string token, OrderModes orderMode, SeatingModes seatingMode, string UrlBase, bool startWebsocketsConnection)
            : base()
        {
            base.Initialize(socketUrl, token, orderMode, seatingMode, UrlBase, startWebsocketsConnection, true);
        }

        public override void LogDoshiiError(DoshiiLogLevels logLevel, string message, Exception ex = null)
        {
            Console.WriteLine(string.Format("{0}: {1}", logLevel.ToString(), message));
        }

        protected override void CheckOutConsumer(string consumerId)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// This method will receive the table allocation object, and should either accept or reject the allocation. 
        /// if the allocation fails the reasoncode property should be populated. 
        /// </summary>
        /// <param name="tableAllocation"></param>
        /// <returns>
        /// true - if the allocation was successful
        /// false - if the allocation failed,
        /// </returns>
        protected override bool ConfirmTableAllocation(ref TableAllocation tableAllocation)
        {
            return true;
        }

        /// <summary>
        /// This method will receive the order that has been paid partially by doshii - this will only get called if you are using restaurant mode.
        /// </summary>
        /// <returns></returns>
        protected override void RecordPartialCheckPayment(ref Order order)
        {

        }

        protected override bool RecordFullCheckPaymentBistroMode(ref Order order)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// this method should record that a check has been fully paid by doshii, if bistro mode is being used it is at this point the order should be formally recorded in the system as the payment is now confirmed. 
        /// </summary>
        /// <returns></returns>
        protected override bool RecordFullCheckPayment(ref Order order)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// this method sould record that the contained order has been cancled. 
        /// </summary>
        /// <param name="order"></param>
        protected override void OrderCancled(ref Order order)
        {

        }

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
        protected override bool ConfirmOrderAvailabilityBistroMode(ref Order order)
        {
            return true;
        }

        /// <summary>
        /// this method is used to check the availability of the products that have been ordered.
        /// This method will only be called in restaurant mode.
        /// if the price of any of the prodducts are incorrect or the products are not available a rejection reason should be added to the product in question
        /// As this is in restaurant mode the order should be formally created on the pos when this order is accepted, paymend is expected at the end of the customer experience at the venue rather than 
        /// with each order. 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        protected override bool ConfirmOrderForRestaurantMode(ref Order order)
        {
            return true;
        }

        /// <summary>
        /// this method is used to confirm the order on doshii accuratly represents the order on the the pos
        /// the pos is the source of truth for the order.
        /// if the order is not correct the pos should update the order object to to represent the correct order. 
        /// </summary>
        /// <param name="order"></param>
        protected override void ConfirmOrderTotalsBeforePaymentRestaurantMode(ref Order order)
        {

        }

        /// <summary>
        /// this method should be used to record on the pos a customer has checked in. 
        /// REVIEW: (liam) It might be a very good idea in the constructor to be given a path where images can be saved and in the method that calls this method to save the user images for use by the pos,
        /// rather than giving the pos the URL of the image and expecting them to get the pic. 
        /// </summary>
        /// <param name="consumer"></param>
        protected override void recordCheckedInUser(ref Consumer consumer)
        {
            MyConsumerList.Add(consumer);
        }

        public override List<Consumer> GetCheckedInCustomersFromPos()
        {
            return MyConsumerList;
        }

        
        
        private List<Product> GenerateProductList()
        {
            throw new NotImplementedException();
        }

        private ProductOptions GenerateCookingTypesProductOption()
        {

            ProductOptions po = new ProductOptions();
            po.Max = 1;
            po.Min = 1;
            po.Name = "Cooking Types";
            po.PosId = "1";
                 
            List<Variants> cookingList = new List<Variants>();

            cookingList.Add(GenerateNoCostVarient("WellDone", "001"));
            cookingList.Add(GenerateNoCostVarient("MediumWell", "002"));
            cookingList.Add(GenerateNoCostVarient("Medium", "003"));
            cookingList.Add(GenerateNoCostVarient("MediumRear", "004"));
            cookingList.Add(GenerateNoCostVarient("Rear", "005"));
            cookingList.Add(GenerateNoCostVarient("Blue", "006"));

            po.Variants = cookingList;
            return po;
        }

        private ProductOptions GenerateTopingsProductOption()
        {
            ProductOptions poi = new ProductOptions();
            poi.Max = 1;
            poi.Min = 1;
            poi.Name = "Toppings";
            poi.PosId = "2";
            
            List<Variants> ToppingList = new List<Variants>();

            ToppingList.Add(GenerateCostVarient("Tomato", "007", 100));
            ToppingList.Add(GenerateCostVarient("Pepper", "008", 110));
            ToppingList.Add(GenerateCostVarient("Mustard", "009", 120));
            ToppingList.Add(GenerateCostVarient("Cream", "010", 130));

            poi.Variants = ToppingList;
            return poi;
        }

        private Product GenerateSteakProduct()
        {
            throw new NotImplementedException();
        }

        private Product GenerateCarltonProduct()
        {
            throw new NotImplementedException();
        }

        private Variants GenerateNoCostVarient(string varientName, string varientNumber)
        {
            return GenerateCostVarient(varientName, varientNumber, 0);
        }

        private Variants GenerateCostVarient(string varientName, string varientNumber, int varientCost)
        {
            throw new NotImplementedException();
        }
    }
}
