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
using DoshiiDotNetIntegration.Models.Json;
using DoshiiDotNetIntegration.Helpers;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class HttpCommunicationTests
    {
        DoshiiManager _manager;
		DoshiiDotNetIntegration.Interfaces.IDoshiiLogger Logger;
		DoshiiDotNetIntegration.DoshiiLogManager LogManager;
		DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager PaymentManager;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication HttpComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication MockHttpComs;

        [SetUp]
        public void Init()
        {
			AutoMapperConfigurator.Configure();
			Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
			LogManager = new DoshiiLogManager(Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
			_manager = MockRepository.GenerateMock<DoshiiManager>(PaymentManager, Logger);
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

            MockHttpComs.VerifyAllExpectations();
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
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Paid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderPaid();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("paid")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_WaitingForPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccessfulOrder();
            responseMessage.Data = Mapper.Map<JsonOrder>(order).ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("waiting_for_payment")))).Return(responseMessage);

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

            MockHttpComs.VerifyAllExpectations();
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
    }
}
