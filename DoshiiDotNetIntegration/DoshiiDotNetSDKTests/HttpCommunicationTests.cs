using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Exceptions;
using Newtonsoft;
using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Models.Json;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class HttpCommunicationTests
    {
        DoshiiManager _manager;
		DoshiiDotNetIntegration.Interfaces.IDoshiiLogger Logger;
		DoshiiDotNetIntegration.DoshiiLogManager LogManager;
		DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager PaymentManager;
        DoshiiDotNetIntegration.Interfaces.IOrderingManager OrderingManager;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication HttpComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication MockHttpComs;

        [SetUp]
        public void Init()
        {
			AutoMapperConfigurator.Configure();
			Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
            LogManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.DoshiiLogManager>(Logger); //new DoshiiLogManager(Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
            OrderingManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IOrderingManager>();
            _manager = MockRepository.GenerateMock<DoshiiManager>(PaymentManager, Logger, OrderingManager);
            _manager.mLog = LogManager;
            MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            HttpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoUrl()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication("", GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
        }

        [Test]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_OperationLogic()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, null);
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_NoLogger()
		{
			var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, null, _manager);
		}

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoToken()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, "", LogManager, _manager);
        }

		[Test]
		public void Constructor_AllParamatersCorrect()
		{
			var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
			Assert.AreEqual(httpComs.m_Token, GenerateObjectsAndStringHelper.TestToken);
			Assert.AreEqual(httpComs.m_DoshiiLogic, _manager);
			Assert.AreEqual(httpComs.mLog, LogManager);
			Assert.AreEqual(httpComs.m_DoshiiUrlBase, GenerateObjectsAndStringHelper.TestBaseUrl);
		}

        
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PutOrder_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PutOrder(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

        }

        [Test]
        public void PutOrder_SuccessfullResponse_ReturnsResponseOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var responseOrder = MockHttpComs.PutOrder(order);

            Assert.AreEqual(responseOrder.Id, order.Id);
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Accepted()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("accepted")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Rejected()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("rejected")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullOrderReturnedException))]
        public void PutOrder_NullOrderReturned_ExceptionThrown()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            MockHttpComs.PutOrder(order);

        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PutProductData_MakeRequest_emptyUrl()
        {
            HttpComs.MakeRequest("", WebRequestMethods.Http.Put, "");
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PutProductData_MakeRequest_NoWebVerb()
        {
            HttpComs.MakeRequest(GenerateObjectsAndStringHelper.TestBaseUrl, "", "");
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetOrder_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

        }

        [Test]
        public void GetOrder_ReturnedOrderIsRetreivedOrder()
        {
            var orderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var orderrResponse = MockHttpComs.GetOrder(orderInput.Id.ToString());

            Assert.AreEqual(orderInput.Id, orderrResponse.Id);
        }

        [Test]
        public void GetOrder_ReturnedOrderDataIsNull()
        {
            var OrderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(OrderInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order, GenerateObjectsAndStringHelper.TestOrderId.ToString()))));

            MockHttpComs.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetOrder_ReturnedStatusIsNotOk()
        {
            var OrderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(OrderInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order, GenerateObjectsAndStringHelper.TestOrderId.ToString()))));

            MockHttpComs.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetOrders_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetOrders();

        }

        [Test]
        public void GetOrders_ReturnedOrdersAreRetreivedOrders()
        {
            var orderListInput = GenerateObjectsAndStringHelper.GenerateOrderList();
            var orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(orderList);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var orderResponse = MockHttpComs.GetOrders();
            List<Order> orderResponseList = orderResponse.ToList();

            Assert.AreEqual(orderListInput[0].Id, orderResponseList[0].Id);
            Assert.AreEqual(orderListInput[1].Id, orderResponseList[1].Id);
            Assert.AreEqual(orderListInput[2].Id, orderResponseList[2].Id);
            Assert.AreEqual(orderListInput[3].Id, orderResponseList[3].Id);
        }

        [Test]
        public void GetOrders_ReturnedOrderDataIsBlank()
        {
            var OrderInputList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = "";
            Newtonsoft.Json.JsonConvert.SerializeObject(OrderInputList);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order))));

            MockHttpComs.GetOrders();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetOrders_ReturnedOrdersReturnIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order))));

            MockHttpComs.GetOrders();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransaction_ReturnedTransactionIsRetreivedTransaction()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var transactionResponse = MockHttpComs.GetTransaction(transactionInput.Id.ToString());

            Assert.AreEqual(transactionInput.Id, transactionResponse.Id);
        }

        [Test]
        public void GetTransaction_ReturnedTransactionDataIsBlank()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetTransaction_ThrowsException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());
           
            MockHttpComs.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransaction_ReturnedTransactionDataIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransaction(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransaction_ReturnedTransactionIsNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransaction(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactions_ReturnedTransactionDataIsBlank()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactions();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetTransactions_ThrowsException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());

            MockHttpComs.GetTransactions();

        }

        [Test]
        public void GetTransactions_ReturnedTransactionDataIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactions();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactions_ReturnedTransactionIsNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactions();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }


        [Test]
        public void PostWaitingTransaction_ResponseOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var transactionResponseComplete = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionResponseComplete);
            responseMessage.Status = HttpStatusCode.OK;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PostTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void PostWaitingTransaction_ResponseOk_NoData()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = "";
            responseMessage.Status = HttpStatusCode.OK;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PostTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void PostWaitingTransaction_ResponseNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PostTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullOrderReturnedException))]
        public void PostWaitingTransaction_ResponseNull()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            
            MockHttpComs.PostTransaction(transactionInput);

        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void CreateOrderWithTableAllocaiton_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PutOrderWithTableAllocation(GenerateObjectsAndStringHelper.GenerateTableOrder());

        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteTableAllocation_ThrowsException()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());
            MockHttpComs.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
        }

    }
}
