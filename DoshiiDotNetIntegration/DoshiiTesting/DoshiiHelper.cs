using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Modles;

namespace DoshiiTesting
{
    public class DoshiiHelper : DoshiiDotNetIntegration.Doshii
    {

        private List<Consumer> MyConsumerList = new List<Consumer>();

        public List<product> MyCompleteProductList = new List<product>();

        public List<product> DoshiiProductList = new List<product>();
        
        public DoshiiHelper(string socketUrl, string token, OrderModes orderMode, SeatingModes seatingMode, string UrlBase, bool startWebsocketsConnection)
            : base(socketUrl, token, orderMode, seatingMode, UrlBase, startWebsocketsConnection)
        {
            //GenerateProductList();
        }

        public override void LogDoshiiError(DoshiiLogLevels logLevel, string message, Exception ex = null)
        {
            Console.WriteLine(string.Format("{0}: {1}", logLevel.ToString(), message));
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
        protected override bool ConfirmTableAllocation(table_allocation tableAllocation)
        {
            return true;
        }

        /// <summary>
        /// This method will receive the order that has been paid partially by doshii - this will only get called if you are using restaurant mode.
        /// </summary>
        /// <returns></returns>
        protected override void RecordPartialCheckPayment(order order)
        {

        }

        protected override void RecordFullCheckPaymentBistroMode(order order)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// this method should record that a check has been fully paid by doshii, if bistro mode is being used it is at this point the order should be formally recorded in the system as the payment is now confirmed. 
        /// </summary>
        /// <returns></returns>
        protected override void RecordFullCheckPayment(order order)
        {

        }

        /// <summary>
        /// this method sould record that the contained order has been cancled. 
        /// </summary>
        /// <param name="order"></param>
        protected override void OrderCancled(order order)
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
        protected override bool ConfirmOrderAvailabilityBistroMode(order order)
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
        protected override bool ConfirmOrderForRestaurantMode(order order)
        {
            return true;
        }

        /// <summary>
        /// this method is used to confirm the order on doshii accuratly represents the order on the the pos
        /// the pos is the source of truth for the order.
        /// if the order is not correct the pos should update the order object to to represent the correct order. 
        /// </summary>
        /// <param name="order"></param>
        protected override void ConfirmOrderTotalsBeforePaymentRestaurantMode(order order)
        {

        }

        /// <summary>
        /// this method should be used to record on the pos a customer has checked in. 
        /// REVIEW: (liam) It might be a very good idea in the constructor to be given a path where images can be saved and in the method that calls this method to save the user images for use by the pos,
        /// rather than giving the pos the URL of the image and expecting them to get the pic. 
        /// </summary>
        /// <param name="consumer"></param>
        protected override void recordCheckedInUser(Consumer consumer)
        {
            MyConsumerList.Add(consumer);
        }

        public override List<Consumer> GetCheckedInCustomersFromPos()
        {
            return MyConsumerList;
        }

        
        
        private List<product> GenerateProductList()
        {
            MyCompleteProductList = new List<product>();
            MyCompleteProductList.Add(GenerateCarltonProduct());
            MyCompleteProductList.Add(GenerateSteakProduct());

            return MyCompleteProductList;
        }

        private product_options GenerateCookingTypesProductOption()
        {

            product_options po = new product_options();
            po.max = 1;
            po.min = 1;
            po.name = "Cooking Types";
            po.pos_id = "1";
                 
            List<variants> cookingList = new List<variants>();

            cookingList.Add(GenerateNoCostVarient("WellDone", "001"));
            cookingList.Add(GenerateNoCostVarient("MediumWell", "002"));
            cookingList.Add(GenerateNoCostVarient("Medium", "003"));
            cookingList.Add(GenerateNoCostVarient("MediumRear", "004"));
            cookingList.Add(GenerateNoCostVarient("Rear", "005"));
            cookingList.Add(GenerateNoCostVarient("Blue", "006"));

            po.variants = cookingList;
            return po;
        }

        private product_options GenerateTopingsProductOption()
        {
            product_options poi = new product_options();
            poi.max = 1;
            poi.min = 1;
            poi.name = "Toppings";
            poi.pos_id = "2";
            
            List<variants> ToppingList = new List<variants>();

            ToppingList.Add(GenerateCostVarient("Tomato", "007", 100));
            ToppingList.Add(GenerateCostVarient("Pepper", "008", 110));
            ToppingList.Add(GenerateCostVarient("Mustard", "009", 120));
            ToppingList.Add(GenerateCostVarient("Cream", "010", 130));

            poi.variants = ToppingList;
            return poi;
        }

        private product GenerateSteakProduct()
        {
            product newProduct = new product();
            newProduct.description = "Steak";
            newProduct.name = "STeak";
            newProduct.pos_id = "000001";
            newProduct.price = "2500";
            newProduct.product_options.Add(GenerateCookingTypesProductOption());
            newProduct.product_options.Add(GenerateTopingsProductOption());

            return newProduct;
        }

        private product GenerateCarltonProduct()
        {
            product newProduct = new product();
            newProduct.description = "CarltonPot";
            newProduct.name = "CarltonPot";
            newProduct.pos_id = "000002";
            newProduct.price = "450";

            return newProduct;
        }

        private variants GenerateNoCostVarient(string varientName, string varientNumber)
        {
            return GenerateCostVarient(varientName, varientNumber, 0);
        }

        private variants GenerateCostVarient(string varientName, string varientNumber, int varientCost)
        {
            variants newVarient = new variants();
            newVarient.name = varientName;
            newVarient.pos_id = varientNumber;
            newVarient.price = varientCost;

            return newVarient;
        }
    }
}
