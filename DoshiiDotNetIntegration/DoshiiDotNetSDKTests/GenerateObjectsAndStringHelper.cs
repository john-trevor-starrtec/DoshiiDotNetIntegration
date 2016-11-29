using AutoMapper;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Net;
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using Newtonsoft.Json;

namespace DoshiiDotNetSDKTests
{
    internal static class GenerateObjectsAndStringHelper
    {

        #region test fields

        internal static string TestVendor = "testVendor";
        internal static string TestBaseUrl = "https://Google.com.au";
        internal static string TestSocketUrl = "wss://google.com.au";
        internal static string TestCustomerId = "TestCustomerId";
        internal static string TestMeerkatConsumerId = "TestPaypalCustomerId";
        internal static string TestTableNumber = "TestTableNumber";
        internal static string TestToken = "TestToken";
        internal static string TestSecretKey = "secretKey";
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
        internal static int TestCovers = 2;
        internal static int TestTimeOutValue = 600;
        internal static string TestProductId = "asd123";
        internal static string TestTransactionId = "tran1234";
        internal static string TestVersion = "asdfre";
        internal static string TestMemberId = "345";
        internal static string TestRewardId = "654";
        internal static string TestCancelResaon = "consumer changed their mind.";
        internal static int TestMemberPoints = 250;
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

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageMembersList()
        {
            var membersList = GenerateMemberList();
            string json = JsonConvert.SerializeObject(membersList);

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageRewardsList()
        {
            var rewardList = GenerateRewardList();
            string json = JsonConvert.SerializeObject(rewardList);

            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageMember()
        {
            var member = GenerateMember1();
            var jsonMember = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonMember>(member);
            string json = jsonMember.ToJsonString();

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

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageConsumer()
        {
            var transaction = GenerateConsumer();
            var jsonTransaction = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonConsumer>(transaction);
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

        internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageSuccess()
        {
            var order = GenerateOrderAccepted();
            
            return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = "",
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
                PosId = "1",
                Name = "Product1",
                Tags = tagsList,
                UnitPrice = 1.0M,
                Description = "The first Product",
                ProductOptions = GenerateProductOptionList(),
            };
            return product;
        }


        internal static DoshiiDotNetIntegration.Models.Product GenerateProduct2()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product2Department");
            var product = new DoshiiDotNetIntegration.Models.Product()
            {
                PosId = "2",
                Name = "Product2",
                Tags = tagsList,
                UnitPrice = 1.0M,
                Description = "The first Product",
                ProductOptions = null,
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
            };
            return productOption;
        }

        internal static List<DoshiiDotNetIntegration.Models.Order> GenerateOrderList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Order>();
            list.Add(GenerateOrderAccepted());
            list.Add(GenerateDeliveryOrderPending());
            list.Add(GenerateOrderReadyToPay());
            list.Add(GenerateOrderCancelled());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderAccepted()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                DoshiiId = TestOrderId,
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
                DoshiiId = TestOrderId,
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
                DoshiiId = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "waiting_for_payment"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateDeliveryOrderPending()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 1,
                DoshiiId = TestOrderId,
                CheckinId = string.Format("{0}2",TestCheckinId),
                Status = "pending",
                Type = "delivery"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GeneratePickupOrderPending()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 1,
                DoshiiId = TestOrderId,
                CheckinId = string.Format("{0}2", TestCheckinId),
                Status = "pending",
                Type = "pickup"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderReadyToPay()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 2,
                DoshiiId = TestOrderId,
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
                DoshiiId = TestOrderId,
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

        internal static Consumer GenerateConsumer()
        {
            var consumer = new Consumer()
            {
                Name = "testName",
                Phone = "0402513654",
                Address = GenerateAddress1(),
                Anonymous = false,
                Email = "test@test.com.au",
                Notes = "",
                PhotoUrl = ""
            };
            return consumer;
        }


        internal static List<DoshiiDotNetIntegration.Models.Transaction> GenerateTransactionList()
        {
            List<Transaction> transactionList = new List<Transaction>();
            transactionList.Add(GenerateTransactionComplete());
            transactionList.Add(GenerateTransactionPending());
            transactionList.Add(GenerateTransactionWaiting());
            return transactionList;
        }

        internal static List<Reward> GenerateRewardList()
        {
            var rewardList = new List<Reward>();
            rewardList.Add(GenerateRewardAbsolute());
            rewardList.Add(GenerateRewardPercentage());
            return rewardList;
        }

        internal static Reward GenerateRewardPercentage()
        {
            var reward = new Reward()
            {
                AppName = "Collect",
                CreatedAt = DateTime.Now,
                Description = "10% off",
                Id = "1",
                Name = "10%",
                SurcountAmount = 10,
                SurcountType = "percentage",
                UpdatedAt = DateTime.Now,
                Uri = new Uri("http://www.test.com")

            };
            return reward;
        }

        internal static Reward GenerateRewardAbsolute()
        {
            var reward = new Reward()
            {
                AppName = "Collect",
                CreatedAt = DateTime.Now,
                Description = "$5 off",
                Id = "2",
                Name = "$5",
                SurcountAmount = 5,
                SurcountType = "absolute",
                UpdatedAt = DateTime.Now,
                Uri = new Uri("http://www.test.com")

            };
            return reward;
        }
        
        internal static List<Member> GenerateMemberList()
        {
            var memberList = new List<Member>();
            memberList.Add(GenerateMember1());
            memberList.Add(GenerateMember2());
            return memberList;
        }

        internal static Member GenerateMember1()
        {
            var member = new Member()
            {
                Address = GenerateAddress1(),
                Apps = GenerateAppList(),
                CreatedAt = DateTime.Now,
                Email = "teste@test.com.au",
                Id = "1",
                Name = "Test Name",
                Phone = "1236547898",
                Ref = "1234",
                UpdatedAt = DateTime.Now,
                Uri = new Uri("http://www.test.com")
            };
            return member;
        }

        internal static Member GenerateMember2()
        {
            var member = new Member()
            {
                Address = GenerateAddress1(),
                Apps = GenerateAppList(),
                CreatedAt = DateTime.Now,
                Email = "test2@test.com.au",
                Id = "2",
                Name = "another Name",
                Phone = "12398747898",
                Ref = "4567",
                UpdatedAt = DateTime.Now,
                Uri = new Uri("http://www.test.com")
            };
            return member;
        }


        internal static List<App> GenerateAppList()
        {
            var appList = new List<App>();
            appList.Add(GenerateApp1());
            appList.Add(GenerateApp2());
            return appList;
        }

        internal static App GenerateApp1()
        {
            var app = new App()
            {
                Id = "1",
                Name = "Ordering App",
                Points = 123
            };
            return app;
        }

        internal static App GenerateApp2()
        {
            var app = new App()
            {
                Id = "2",
                Name = "Membership App",
                Points = 123
            };
            return app;
        }
        internal static Address GenerateAddress1()
        {
            var address = new Address()
            {
                City = "Melbourne",
                Country = "Australia",
                Line1 = "34 smith street",
                Line2 = "",
                PostalCode = "3013",
                State = "vic"
            };
            return address;
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

        internal static DoshiiDotNetIntegration.Models.Json.SocketMessageData GenerateSocketMessageData_OrderStatus()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessageData testSocketMessageData = new SocketMessageData();
            testSocketMessageData.Order = Mapper.Map<JsonOrder>(GenerateDeliveryOrderPending()).ToJsonString();
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
            testOrderEventArgs.Order = GenerateDeliveryOrderPending();
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

        public static string BuildSocketUrl(string baseApiUrl, string token)
        {
            // baseApiUrl is for example https://sandbox.doshii.co/pos/v3
            // require socket url of wss://sandbox.doshii.co/pos/socket?token={token} in this example
            // so first, replace http with ws (this handles situation where using http/ws instead of https/wss
            string result = baseApiUrl.Replace("http", "ws");

            // next remove the /api/v2 section of the url
            int index = result.IndexOf("/v");
            if (index > 0 && index < result.Length)
            {
                result = result.Remove(index);
            }

            index = result.IndexOf(".");
            if (index > 0 && index < result.Length)
            {
                result = result.Insert(index, "-socket");
            }

            // finally append the socket endpoint and token parameter to the url and return the result
            result = String.Format("{0}/socket?token={1}", result, token);

            return result;
        }
    }
}
