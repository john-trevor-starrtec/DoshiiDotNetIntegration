using AutoMapper;
using System;
using System.Collections.Generic;
using System.Net;
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetSDKTests
{
    internal static class GenerateObjectsAndStringHelper
    {

        #region test fields

        internal static string TestBaseUrl = "https://Google.com.au";
        internal static string TestSocketUrl = "wss://google.com.au";
        internal static string TestCustomerId = "TestCustomerId";
        internal static string TestMeerkatConsumerId = "TestPaypalCustomerId";
        internal static string TestTableNumber = "TestTableNumber";
        internal static string TestToken = "TestToken";
        internal static string TestCheckinId = "TestCheckinId";
        internal static string TestTableName = "TestName";
        internal static string TestTableAllocationName = "TestAllocationName";
        internal static string TestTableAllocationStatus = "TableAllocationStatus";
        internal static string TestPaypalTabId = "TestPaypalTabId";
        internal static string TestLocationId = "TestLocationId";
        internal static string TestCheckinStatus = "TestCheckinStatus";
        internal static string TestGratuity = "TestGratuity";
        internal static Uri TestCheckinUrl = new Uri("c:\\impos\\");
        internal static string TestOrderId = "123";
        internal static int TestTimeOutValue = 600;
        internal static string TestProductId = "asd123";
        internal static string TestTransactionId = "tran1234";
        internal static string TestVersion = "asdfre";
        #endregion 

        #region responceMessages

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageSuccessNoData()
        {
            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }
        
        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageConfig()
        {
            var configuration = GenerateConfiguration(true, true);
            var jsonTransaction = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonConfiguration>(configuration);
            string json = jsonTransaction.ToJsonString();

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageTransactionPending()
        {
            var transaction = GenerateTransactionPending();
            var jsonTransaction = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonTransaction>(transaction);
            string json = jsonTransaction.ToJsonString();

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageTransactionComplete()
        {
            var transaction = GenerateTransactionComplete();
            var jsonTransaction = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonTransaction>(transaction);
            string json = jsonTransaction.ToJsonString();

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageTransactionWaiting()
        {
            var transaction = GenerateTransactionWaiting();
            var jsonTransaction = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(transaction);
            string json = jsonTransaction.ToJsonString();

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageOrderSuccess()
        {
			var order = GenerateOrderAccepted();
			var jsonOrder = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(order);
			string json = jsonOrder.ToJsonString();

			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageOrdersSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(Mapper.Map<List<DoshiiDotNetIntegration.Models.Json.JsonOrder>>(GenerateOrderList())),
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageTableAllocationSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(Mapper.Map<List<DoshiiDotNetIntegration.Models.Json.JsonTableAllocation>>(GenerateTableAllocationList())),
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessagePutTableAllocationSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessagePutTableAllocationFailure()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.InternalServerError,
                StatusDescription = "Internal Service Error",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageSuccessfulOrder()
        {
			var order = GenerateOrderAccepted();
			var jsonOrder = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(order);
			string json = jsonOrder.ToJsonString();

			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static TableOrder GenerateTableOrder()
        {
            var tableOrder = new TableOrder()
            {
                Order = GenerateOrderAccepted(),
                Table = GenerateTableAllocation()
            };
            return tableOrder;
        }
        #endregion

        #region Generate TestObjects and TestValues

        internal static Configuration GenerateConfiguration(bool checkoutOnPaid, bool deallocateTableOnPaid)
        {
            var configuration = new Configuration();
            configuration.DeallocateTableOnPaid = deallocateTableOnPaid;
            configuration.CheckoutOnPaid = checkoutOnPaid;

            return configuration;
        }
        
        internal static List<DoshiiDotNetIntegration.Models.Product> GenerateProductList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Product>();
            list.Add(GenerateProduct1WithOptions());
            list.Add(GenerateProduct2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Product GenerateProduct1WithOptions()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product1Department");
            var optionsList = new List<DoshiiDotNetIntegration.Models.ProductOptions>();
            var product = new DoshiiDotNetIntegration.Models.Product()
            {
                Id = "1",
                PosId = "1",
                Name = "Product1",
                Tags = tagsList,
                Price = 1.0M,
                Description = "The first Product",
                ProductOptions = GenerateProductOptionList(),
                Status = "pending"    
            };
            return product;
        }


        internal static DoshiiDotNetIntegration.Models.Product GenerateProduct2()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product2Department");
            var product = new DoshiiDotNetIntegration.Models.Product()
            {
                Id = "2",
                PosId = "2",
                Name = "Product2",
                Tags = tagsList,
                Price = 1.0M,
                Description = "The first Product",
                ProductOptions = null,
                Status = "pending"    
            };
            return product;
        }

        internal static List<DoshiiDotNetIntegration.Models.Variants> GenerateProductVarientList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Variants>();
            list.Add(GenerateProductVarient1());
            list.Add(GenerateProductVarient2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Variants GenerateProductVarient1()
        {
            var variant = new DoshiiDotNetIntegration.Models.Variants()
            {
                Name = "variant1",
                Price = 0.1M,
                PosId = "var1"
            };
            return variant;
        }

        internal static DoshiiDotNetIntegration.Models.Variants GenerateProductVarient2()
        {
            var variant = new DoshiiDotNetIntegration.Models.Variants()
            {
                Name = "variant2",
                Price = 0.1M,
                PosId = "var2"
            };
            return variant;
        }

        internal static List<DoshiiDotNetIntegration.Models.ProductOptions> GenerateProductOptionList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.ProductOptions>();
            list.Add(GenerateProductOption1());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.ProductOptions GenerateProductOption1()
        {
            var productOption = new DoshiiDotNetIntegration.Models.ProductOptions()
            {
                Name = "ProductOptions1",
                Min = 0,
                Max = 0,
                PosId = "10",
                Variants = GenerateProductVarientList(),
                Selected = new List<DoshiiDotNetIntegration.Models.Variants>()
            };
            return productOption;
        }

        internal static List<DoshiiDotNetIntegration.Models.Order> GenerateOrderList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Order>();
            list.Add(GenerateOrderAccepted());
            list.Add(GenerateOrderPending());
            list.Add(GenerateOrderReadyToPay());
            list.Add(GenerateOrderCancelled());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderAccepted()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "accepted"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderPaid()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "paid"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderWaitingForPayment()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "waiting_for_payment"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderPending()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 1,
                CheckinId = string.Format("{0}2",TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderReadyToPay()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 2,
                CheckinId = string.Format("{0}3", TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderCancelled()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 3,
                CheckinId = string.Format("{0}4", TestCheckinId),
                Status = "cancelled"
            };
            return order;
        }

        internal static List<DoshiiDotNetIntegration.Models.TableAllocation> GenerateTableAllocationList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.TableAllocation>();
            list.Add(GenerateTableAllocation());
            list.Add(GenerateTableAllocation2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.TableAllocation GenerateTableAllocation()
        {
            var tableAllocation = new DoshiiDotNetIntegration.Models.TableAllocation()
            {
                Id = TestTableName,
                Name = TestTableAllocationName,
                Status = TestTableAllocationStatus
            };
            return tableAllocation;
        }

        internal static DoshiiDotNetIntegration.Models.TableAllocation GenerateTableAllocation2()
        {
            var tableAllocation = new DoshiiDotNetIntegration.Models.TableAllocation()
            {
                Id = string.Format("{0}2",TestTableName),
                Name = string.Format("{0}2",TestTableAllocationName),
                Status = TestTableAllocationStatus
            };
            return tableAllocation;
        }

        internal static DoshiiDotNetIntegration.Models.Transaction GenerateTransactionPending()
        {
            var transaction = new DoshiiDotNetIntegration.Models.Transaction()
            {
                Id = TestTransactionId,
                OrderId = "1",
                Reference = "TestTransaction",
                Invoice = "1",
                PaymentAmount = 10.0M,
                AcceptLess = false,
                PartnerInitiated = false,
                Partner = "1",
                Status = "pending"
            };
            return transaction;
        }

        internal static DoshiiDotNetIntegration.Models.Transaction GenerateTransactionWaiting()
        {
            var transaction = new DoshiiDotNetIntegration.Models.Transaction()
            {
                Id = TestTransactionId,
                OrderId = "1",
                Reference = "waiting",
                Invoice = "",
                PaymentAmount = 10.0M,
                AcceptLess = false,
                PartnerInitiated = false,
                Partner = "1",
                Status = "waiting"
            };
            return transaction;
        }

        internal static DoshiiDotNetIntegration.Models.Transaction GenerateTransactionComplete()
        {
            var transaction = new DoshiiDotNetIntegration.Models.Transaction()
            {
                Id = TestTransactionId,
                OrderId = "1",
                Reference = "TestTransaction",
                Invoice = "1",
                PaymentAmount = 10.0M,
                AcceptLess = false,
                PartnerInitiated = false,
                Partner = "1",
                Status = "complete"
            };
            return transaction;
        }

        internal static DoshiiDotNetIntegration.Models.Configuration GenerateConfig()
        {
            var configuration = new DoshiiDotNetIntegration.Models.Configuration()
            {
                CheckoutOnPaid = true,
                DeallocateTableOnPaid = true
            };
            return configuration;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_OrderStatus()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            testSocketMessageData.Order = Mapper.Map<JsonOrder>(GenerateOrderPending()).ToJsonString();
            testSocketMessageData.EventName = "order_status";
            testSocketMessageData.OrderId = TestOrderId;
            testSocketMessageData.Status = "pending";
            testSocketMessageData.Uri = new Uri("http://www.TestDoshiiUri.com");
            return testSocketMessageData;

        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateBlankSocketMessage()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = new SocketMessage();
            if (testSocketMessage.Emit == null)
            {
                testSocketMessage.Emit = new List<object>();
            }
            return testSocketMessage;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateSocketMessage_OrderStatus()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateBlankSocketMessage();
            
            testSocketMessage.Emit.Add("order_status");
            testSocketMessage.Emit.Add(GenerateSocketMessageData_OrderStatus());
            return testSocketMessage;
        }

        internal static OrderEventArgs GenerateOrderEventArgs_pending()
        {
            OrderEventArgs testOrderEventArgs = new OrderEventArgs();
            testOrderEventArgs.Order = GenerateOrderPending();
            testOrderEventArgs.OrderId = TestOrderId;
            testOrderEventArgs.Status = "pending";
            return testOrderEventArgs;
        }

        internal static TransactionEventArgs GenerateTransactionEventArgs_pending(Transaction pendingTransaction)
        {
            TransactionEventArgs testtransactionEventArgs = new TransactionEventArgs();
            testtransactionEventArgs.Transaction = pendingTransaction;
            testtransactionEventArgs.Status = "pending";
            testtransactionEventArgs.TransactionId = TestTransactionId;
            return testtransactionEventArgs;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_TransactionCreated()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            testSocketMessageData.Order = Mapper.Map<JsonTransaction>(GenerateTransactionPending()).ToJsonString();
            testSocketMessageData.EventName = "transaction_created";
            testSocketMessageData.TransactionId = TestTransactionId;
            testSocketMessageData.Status = "pending";
            testSocketMessageData.Uri = new Uri("http://www.TestDoshiiUri.com");
            return testSocketMessageData;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateSocketMessage_TransactionCreated()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateBlankSocketMessage();
            testSocketMessage.Emit.Add("transaction_created");
            testSocketMessage.Emit.Add(GenerateSocketMessageData_TransactionCreated());
            return testSocketMessage;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_TransactionStatus()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            testSocketMessageData.Order = Mapper.Map<JsonTransaction>(GenerateTransactionPending()).ToJsonString();
            testSocketMessageData.EventName = "transaction_status";
            testSocketMessageData.TransactionId = TestTransactionId;
            testSocketMessageData.Status = "pending";
            testSocketMessageData.Uri = new Uri("http://www.TestDoshiiUri.com");
            return testSocketMessageData;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateSocketMessage_TransactionStatus()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateBlankSocketMessage();
            testSocketMessage.Emit.Add("transaction_status");
            testSocketMessage.Emit.Add(GenerateSocketMessageData_TransactionStatus());
            return testSocketMessage;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_Empty()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            return testSocketMessageData;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateSocketMessage_Empty()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateBlankSocketMessage();
            testSocketMessage.Emit.Add("not_supported");
            testSocketMessage.Emit.Add(GenerateSocketMessageData_Empty());
            return testSocketMessage;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_AllData()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            testSocketMessageData.Order = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(GenerateOrderAccepted()).ToJsonString() ;
            testSocketMessageData.CheckinId = TestCheckinId;
            testSocketMessageData.Consumer = "Test Consumer";
            testSocketMessageData.ConsumerId = TestCustomerId;
            testSocketMessageData.EventName = "not_supported";
            testSocketMessageData.Id = TestCheckinId;
            testSocketMessageData.MeerkatConsumerId = TestMeerkatConsumerId;
            testSocketMessageData.Name = TestTableName;
            testSocketMessageData.OrderId = TestOrderId;
            testSocketMessageData.Status = "pending";
            testSocketMessageData.TransactionId = TestTransactionId;
            testSocketMessageData.Uri = TestCheckinUrl;
            return testSocketMessageData;
        }

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessage GenerateSocketMessage_AllData()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateBlankSocketMessage();
            testSocketMessage.Emit.Add("not_supported");
            testSocketMessage.Emit.Add(GenerateSocketMessageData_Empty());
            return testSocketMessage;
        }

        #endregion
    }
}
