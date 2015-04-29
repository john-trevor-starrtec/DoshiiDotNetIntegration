using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.Tests
{
    public static class GenerateObjectsAndStringHelper
    {

        #region test fields

        public static string TestBaseUrl = "https://Google.com.au";
        public static string TestSocketUrl = "wss://google.com.au";
        public static string TestCustomerId = "TestCustomerId";
        public static string TestPayPalCustomerId = "TestPaypalCustomerId";
        public static string TestTableNumber = "TestTableNumber";
        public static string TestToken = "TestToken";
        public static string TestCheckinId = "TestCheckinId";
        public static string TestTableName = "TestName";
        public static string TestTableAllocationName = "TestAllocationName";
        public static string TestTableAllocationStatus = "TableAllocationStatus";
        public static string TestPaypalTabId = "TestPaypalTabId";
        public static string TestLocationId = "TestLocationId";
        public static string TestCheckinStatus = "TestCheckinStatus";
        public static string TestGratuity = "TestGratuity";
        public static Uri TestCheckinUrl = new Uri("c:\\impos\\");
        public static int TestOrderId = 123;
        public static int TestTimeOutValue = 600;
        public static string TestProductId = "asd123";

        #endregion 

        #region responceMessages

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageConsumerSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = GenerateConsumer1().ToJsonString(),
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageConsumersSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(GenerateConsumerList()),
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageOrderSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = GenerateOrderAccepted().ToJsonString(),
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageOrdersSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(GenerateOrderList()),
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageTableAllocationSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(GenerateTableAllocationList()),
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessagePutTableAllocationSuccess()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessagePutTableAllocationFailure()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.InternalServerError,
                StatusDescription = "Internal Service Error",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

        public static CommunicationLogic.DoshiHttpResponceMessages GenerateResponceMessageSuccessfulOrder()
        {
            return new CommunicationLogic.DoshiHttpResponceMessages()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = GenerateOrderAccepted().ToJsonString(),
                ErrorMessage = "",
                Message = ""
            };
        }

        #endregion

        #region Generate TestObjects and TestValues

        public static List<Models.Product> GenerateProductList()
        {
            var list = new List<Models.Product>();
            list.Add(GenerateProduct1WithOptions());
            list.Add(GenerateProduct2());
            return list;
        }
        
        public static Models.Product GenerateProduct1WithOptions()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product1Department");
            var optionsList = new List<Models.ProductOptions>();
            var product = new Models.Product()
            {
                Id = "1",
                PosId = "1",
                Name = "Product1",
                Tags = tagsList,
                Price = "100",
                Description = "The first Product",
                ProductOptions = GenerateProductOptionList(),
                AdditionalInstructions = "No aditional instructions",
                RejectionReason = "",
                Status = "pending"    
            };
            return product;
        }


        public static Models.Product GenerateProduct2()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product2Department");
            var product = new Models.Product()
            {
                Id = "2",
                PosId = "2",
                Name = "Product2",
                Tags = tagsList,
                Price = "100",
                Description = "The first Product",
                ProductOptions = null,
                AdditionalInstructions = "No aditional instructions",
                RejectionReason = "",
                Status = "pending"    
            };
            return product;
        }
        
        public static List<Models.Variants> GenerateProductVarientList()
        {
            var list = new List<Models.Variants>();
            list.Add(GenerateProductVarient1());
            list.Add(GenerateProductVarient2());
            return list;
        }

        public static Models.Variants GenerateProductVarient1()
        {
            var variant = new Models.Variants()
            {
                Name = "variant1",
                Price = "10",
                PosId = "var1"
            };
            return variant;
        }

        public static Models.Variants GenerateProductVarient2()
        {
            var variant = new Models.Variants()
            {
                Name = "variant2",
                Price = "10",
                PosId = "var2"
            };
            return variant;
        }

        public static List<Models.ProductOptions> GenerateProductOptionList()
        {
            var list = new List<Models.ProductOptions>();
            list.Add(GenerateProductOption1());
            return list;
        }

        public static Models.ProductOptions GenerateProductOption1()
        {
            var productOption = new Models.ProductOptions()
            {
                Name = "ProductOptions1",
                Min = 0,
                Max = 0,
                PosId = "10",
                Variants = GenerateProductVarientList(),
                Selected = new List<Models.Variants>()
            };
            return productOption;
        }

        public static CommunicationLogic.CommunicationEventArgs.CheckInEventArgs GenerateCheckinEventArgs()
        {
            var checkInArgs = new CommunicationLogic.CommunicationEventArgs.CheckInEventArgs();
            checkInArgs.Consumer = GenerateConsumer1();
            checkInArgs.CheckIn = checkInArgs.Consumer.CheckInId;
            checkInArgs.PaypalCustomerId = checkInArgs.Consumer.PaypalCustomerId;
            checkInArgs.Uri = TestCheckinUrl;
            return checkInArgs;
        }


        public static List<Models.Order> GenerateOrderList()
        {
            var list = new List<Models.Order>();
            list.Add(GenerateOrderAccepted());
            list.Add(GenerateOrderPending());
            list.Add(GenerateOrderReadyToPay());
            list.Add(GenerateOrderCancelled());
            return list;
        }

        public static Models.Order GenerateOrderAccepted()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "accepted"
            };
            return order;
        }

        public static Models.Order GenerateOrderPaid()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "paid"
            };
            return order;
        }

        public static Models.Order GenerateOrderWaitingForPayment()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "waiting for payment"
            };
            return order;
        }

        public static Models.Order GenerateOrderPending()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId + 1,
                CheckinId = string.Format("{0}2",TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        public static Models.Order GenerateOrderReadyToPay()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId + 2,
                CheckinId = string.Format("{0}3", TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        public static Models.Order GenerateOrderCancelled()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId + 3,
                CheckinId = string.Format("{0}4", TestCheckinId),
                Status = "cancelled"
            };
            return order;
        }



        public static CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs GenerateTableAllocationEventArgs()
        {
            var allocationEventArgs = new CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs()
            {
                TableAllocation = GenerateTableAllocation()
            };
            return allocationEventArgs;
        }

        public static List<Models.TableAllocation> GenerateTableAllocationList()
        {
            var list = new List<Models.TableAllocation>();
            list.Add(GenerateTableAllocation());
            list.Add(GenerateTableAllocation2());
            return list;
        }

        public static Models.TableAllocation GenerateTableAllocation()
        {
            var tableAllocation = new Models.TableAllocation()
            {
                Id = TestTableName,
                Name = TestTableAllocationName,
                CustomerId = TestCustomerId,
                PaypalCustomerId = TestCustomerId,
                Status = TestTableAllocationStatus,
                Checkin = GenerateCheckin(),
                rejectionReason = Enums.TableAllocationRejectionReasons.TableDoesNotExist
            };
            return tableAllocation;
        }

        public static Models.TableAllocation GenerateTableAllocation2()
        {
            var tableAllocation = new Models.TableAllocation()
            {
                Id = string.Format("{0}2",TestTableName),
                Name = string.Format("{0}2",TestTableAllocationName),
                CustomerId = string.Format("{0}2",TestCustomerId),
                PaypalCustomerId = string.Format("{0}2",TestCustomerId),
                Status = TestTableAllocationStatus,
                Checkin = GenerateCheckin(),
                rejectionReason = Enums.TableAllocationRejectionReasons.TableDoesNotExist
            };
            return tableAllocation;
        }

        public static Models.Checkin GenerateCheckin()
        {
            var checkin = new Models.Checkin()
            {
                Id = TestCheckinId,
                PaypalTabId = TestPaypalTabId,
                ConsumerId = TestCustomerId,
                LocationId = TestLocationId,
                Status = TestCheckinStatus,
                ExpirationDate = DateTime.Now,
                Gratuity = TestGratuity,
                UpdatedAt = DateTime.Now,
                PaypalCustomerId = TestPayPalCustomerId
            };
            return checkin;
        }

        public static Models.Consumer GenerateConsumer1()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "123";
            consumer.Id = 1;
            consumer.Name = "John Doe";
            consumer.PaypalCustomerId = "NC123NV";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer2()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "456";
            consumer.Id = 2;
            consumer.Name = "Jayne Doe";
            consumer.PaypalCustomerId = "ACB123AB";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer3()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "789";
            consumer.Id = 3;
            consumer.Name = "Bulkan e";
            consumer.PaypalCustomerId = "axy765xa";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer4()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "101";
            consumer.Id = 4;
            consumer.Name = "Mary Jane";
            consumer.PaypalCustomerId = "bgr531gb";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static List<Models.Consumer> GenerateConsumerList()
        {
            var list = new List<Models.Consumer>();
            list.Add(GenerateConsumer1());
            list.Add(GenerateConsumer2());
            list.Add(GenerateConsumer3());
            list.Add(GenerateConsumer4());
            return list;
        }
        #endregion
    }
}
