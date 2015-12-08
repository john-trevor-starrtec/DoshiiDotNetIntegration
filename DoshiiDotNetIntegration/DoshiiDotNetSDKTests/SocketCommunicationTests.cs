using DoshiiDotNetIntegration;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using DoshiiDotNetIntegration.Models.Json;
using WebSocketSharp;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class SocketCommunicationTests
    {
        DoshiiManager _manager;
		DoshiiDotNetIntegration.Interfaces.IDoshiiLogger Logger;
		DoshiiDotNetIntegration.DoshiiLogManager LogManager;
		DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager PaymentManager;
        DoshiiDotNetIntegration.Interfaces.IOrderingManager OrderingManager;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication SocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication MockSocketComs;

        [SetUp]
        public void Init()
        {
			Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
			LogManager = new DoshiiLogManager(Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
            OrderingManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IOrderingManager>();
            _manager = MockRepository.GeneratePartialMock<DoshiiManager>(PaymentManager, Logger, OrderingManager);
            MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            SocketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            _manager.SocketComs = MockSocketComs;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoUrl()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication("", GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
        }

        [Test]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NoOperationLogic()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, null);
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_NoLogManager()
		{
			var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, null, _manager);
		}

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Constructor_TimeoutLessThan10Secs()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, 0, LogManager, _manager);
        }

        [Test]
        public void Constructor_AllParamatersCorrect()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            Assert.AreEqual(socketComs.m_SocketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_SocketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_DoshiiLogic, _manager);
			Assert.AreEqual(socketComs.mLog, LogManager);
            Assert.IsNotNull(socketComs.m_WebSocketsConnection);
        }

        [Test]
        public void Initialise_ScoketCommection()
        {
            MockSocketComs.Expect(x => x.Connect()).Repeat.Once();

            MockSocketComs.Initialize();

            MockSocketComs.VerifyAllExpectations();
        }

        [Test]
        public void SetLastConnectionAttemptTime()
        {
            var testDateTime = DateTime.Now;
            SocketComs.m_LastConnectionAttemptTime = DateTime.MinValue;
            SocketComs.SetLastConnectionAttemptTime();

            Assert.GreaterOrEqual(SocketComs.m_LastConnectionAttemptTime, testDateTime);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ProcessSocketMessage_OrderStatusMessage()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage =
                GenerateObjectsAndStringHelper.GenerateSocketMessage_OrderStatus();
            _manager.Stub(x => x.GetOrder(Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateOrderPending());
            _manager.Expect(x => x.SocketComsOrderStatusEventHandler(MockSocketComs, GenerateObjectsAndStringHelper.GenerateOrderEventArgs_pending()));

            MockSocketComs.ProcessSocketMessage(testSocketMessage);
        }

        [Test]
        public void ProcessSocketMessage_TransactionCreatedMessage()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage =
                GenerateObjectsAndStringHelper.GenerateSocketMessage_TransactionCreated();

            _manager.Stub(x => x.GetTransaction(Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateTransactionPending());
            //adding ignore arguments here feels like a little bit of a cheat - i'll have to try and make it work without it. 
            _manager.Expect(x => x.SocketComsTransactionStatusEventHandler(MockSocketComs, GenerateObjectsAndStringHelper.GenerateTransactionEventArgs_pending())).IgnoreArguments();
            MockSocketComs.ProcessSocketMessage(testSocketMessage);
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void ProcessSocketMessage_TransactionStatusMessage()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage =
                GenerateObjectsAndStringHelper.GenerateSocketMessage_TransactionStatus();

            _manager.Stub(x => x.GetTransaction(Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateTransactionPending());
            //adding ignore arguments here feels like a little bit of a cheat - i'll have to try and make it work without it. 
            _manager.Expect(x => x.SocketComsTransactionStatusEventHandler(MockSocketComs, GenerateObjectsAndStringHelper.GenerateTransactionEventArgs_pending())).IgnoreArguments();
            MockSocketComs.ProcessSocketMessage(testSocketMessage);
            _manager.VerifyAllExpectations();
        }
    }
}
