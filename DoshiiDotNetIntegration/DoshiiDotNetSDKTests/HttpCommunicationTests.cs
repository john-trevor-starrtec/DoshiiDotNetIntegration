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
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models.Json;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
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
        DoshiiDotNetIntegration.Interfaces.IMembershipModuleManager MembershipManager;

        [SetUp]
        public void Init()
        {
			AutoMapperConfigurator.Configure();
			Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
            LogManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.DoshiiLogManager>(Logger); //new DoshiiLogManager(Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
            OrderingManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IOrderingManager>();
            MembershipManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IMembershipModuleManager>();
            _manager = MockRepository.GenerateMock<DoshiiManager>(PaymentManager, Logger, OrderingManager, MembershipManager);
            _manager.mLog = LogManager;
            MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            HttpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoUrl()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication("", LogManager, _manager);
        }

        [Test]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_OperationLogic()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, null);
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_NoLogger()
		{
			var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, null, _manager);
		}

       [Test]
		public void Constructor_AllParamatersCorrect()
		{
			var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
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
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullResponseDataReturnedException))]
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
        public void GetOrderFromDoshiiOrderId_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetOrderFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        public void GetOrderFromDoshiiOrderId_ReturnedOrderIsRetreivedOrder()
        {
            var orderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var orderrResponse = MockHttpComs.GetOrderFromDoshiiOrderId(orderInput.Id);

            Assert.AreEqual(orderInput.Id, orderrResponse.Id);
        }

        [Test]
        public void GetOrderFromDoshiiOrderId_ReturnedOrderDataIsNull()
        {
            var OrderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(OrderInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order, GenerateObjectsAndStringHelper.TestOrderId.ToString()))));

            MockHttpComs.GetOrderFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetOrderFromDoshiiOrderId_ReturnedStatusIsNotOk()
        {
            var OrderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(OrderInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order, GenerateObjectsAndStringHelper.TestOrderId.ToString()))));

            MockHttpComs.GetOrderFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId.ToString());

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
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetUnlinkedOrders_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetUnlinkedOrders();

        }

        [Test]
        public void GetUnlinkedOrders_ReturnedOrderDataIsBlank()
        {
            var OrderInputList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageOrderSuccess();
            responseMessage.Data = "";
            Newtonsoft.Json.JsonConvert.SerializeObject(OrderInputList);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order))));

            MockHttpComs.GetUnlinkedOrders();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetUnlinkedOrders_ReturnedOrdersReturnIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order))));

            MockHttpComs.GetUnlinkedOrders();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }


        /*[Test]
        public void GetTransaction_ReturnedTransactionIsRetreivedTransaction()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var transactionResponse = MockHttpComs.GetTransaction(transactionInput.Id.ToString());

            Assert.AreEqual(transactionInput.Id, transactionResponse.Id);
        }*/

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
        public void GetTransactions_ReturnedTransactionIsOk()
        {
            var transactionListInput = GenerateObjectsAndStringHelper.GenerateTransactionList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionListInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactions();

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactionsFromDoshiiOrderId_ReturnedTransactionIsNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactionsFromDohsiiOrderId_ReturnedTransactionDataIsBlank()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetTransactionsFromDoshiiOrderId_ThrowsException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionPending();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        public void GetTransactionsFromDoshiiOrderId_ReturnedTransactionDataIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactionsFromDoshiiOrderID_ReturnedTransactionIsNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetTransactionsFromDoshiiOrderID_ReturnedTransactionIsOk()
        {
            var transactionListInput = GenerateObjectsAndStringHelper.GenerateTransactionList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionListInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetTransactionsFromDoshiiOrderId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumerFromCheckinId_ReturnedTransactionIsNotOk()
        {
            var consumerInput = GenerateObjectsAndStringHelper.GenerateConsumer();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageConsumer();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(consumerInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetConsumerFromCheckinId(GenerateObjectsAndStringHelper.GeneratePickupOrderPending().CheckinId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumerFromCheckinId_ReturnedTransactionDataIsBlank()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageConsumer();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetConsumerFromCheckinId(GenerateObjectsAndStringHelper.GeneratePickupOrderPending().CheckinId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetConsumerFromCheckinId_ThrowsException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageConsumer();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());

            MockHttpComs.GetConsumerFromCheckinId(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        public void GetConsumerFromCheckinId_ReturnedTransactionDataIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetConsumerFromCheckinId(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumerFromCheckinId_ReturnedTransactionIsOk()
        {
            var consumerInput = GenerateObjectsAndStringHelper.GenerateConsumer();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageConsumer();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(consumerInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.GetConsumerFromCheckinId(GenerateObjectsAndStringHelper.GeneratePickupOrderPending().CheckinId);

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
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullResponseDataReturnedException))]
        public void PostWaitingTransaction_ResponseNull()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            
            MockHttpComs.PutTransaction(transactionInput);

        }

        [Test]
        public void PutWaitingTransaction_ResponseOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var transactionResponseComplete = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionResponseComplete);
            responseMessage.Status = HttpStatusCode.OK;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PutTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void PutWaitingTransaction_ResponseOk_NoData()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = "";
            responseMessage.Status = HttpStatusCode.OK;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PutTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        public void PutWaitingTransaction_ResponseNotOk()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageTransactionComplete();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(transactionInput);
            responseMessage.Status = HttpStatusCode.Forbidden;
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", MockHttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Transaction, GenerateObjectsAndStringHelper.TestTransactionId.ToString()))));

            MockHttpComs.PutTransaction(transactionInput);

            MockHttpComs.VerifyAllExpectations();
            _manager.mLog.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullResponseDataReturnedException))]
        public void PutWaitingTransaction_ResponseNull()
        {
            var transactionInput = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            MockHttpComs.PutTransaction(transactionInput);

        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteTableAllocation_ThrowsException()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException());
            MockHttpComs.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
        }

        [Test]
        public void DeleteTableAllocation_ResponceMessageIsNull()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' to URL '{0}'", MockHttpComs.GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, GenerateObjectsAndStringHelper.TestOrderId))));
            
            bool result = MockHttpComs.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
            Assert.AreEqual(result, false);
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void DeleteTableAllocation_ResponceMessageIsNotNullButNotOk()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateResponseMessagePutTableAllocationFailure());
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", MockHttpComs.GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, GenerateObjectsAndStringHelper.TestOrderId))));

            bool result = MockHttpComs.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
            Assert.AreEqual(result, false);
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void DeleteTableAllocation_ResponceMessageIsOk()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateResponseMessagePutTableAllocationSuccess());
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiHttpCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was successful. Allocations have been removed", MockHttpComs.GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, GenerateObjectsAndStringHelper.TestOrderId))));

            bool result = MockHttpComs.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
            Assert.AreEqual(result, true);
            _manager.VerifyAllExpectations();
        }

    }
}
