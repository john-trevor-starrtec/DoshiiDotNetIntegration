using DoshiiDotNetIntegration;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using WebSocketSharp;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class SocketCommunicationTests
    {
        DoshiiManager _manager;
		DoshiiDotNetIntegration.Interfaces.IDoshiiLogger _Logger;
		DoshiiDotNetIntegration.DoshiiLogManager LogManager;
		DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager PaymentManager;
        DoshiiDotNetIntegration.Interfaces.IOrderingManager OrderingManager;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication SocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication MockSocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication MockHttpComs;

        [SetUp]
        public void Init()
        {
			_Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
			LogManager = new DoshiiLogManager(_Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
            OrderingManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IOrderingManager>();
            _manager = MockRepository.GeneratePartialMock<DoshiiManager>(PaymentManager, _Logger, OrderingManager);
            MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            SocketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            _manager.m_HttpComs = MockHttpComs;
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
            DoshiiDotNetIntegration.Models.Transaction pendingTransaction =
                GenerateObjectsAndStringHelper.GenerateTransactionPending();
            _manager.Stub(x => x.GetTransaction(Arg<String>.Is.Anything)).IgnoreArguments().Return(pendingTransaction);
            _manager.Expect(
                x =>
                    x.SocketComsTransactionStatusEventHandler(Arg<DoshiiWebSocketsCommunication>.Is.Equal(MockSocketComs), Arg<TransactionEventArgs>.Matches(t => t.TransactionId == pendingTransaction.Id && t.Status == pendingTransaction.Status && t.Transaction.PaymentAmount == pendingTransaction.PaymentAmount)));
            MockSocketComs.ProcessSocketMessage(testSocketMessage);
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void ProcessSocketMessage_TransactionStatusMessage()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage =
                GenerateObjectsAndStringHelper.GenerateSocketMessage_TransactionStatus();
            DoshiiDotNetIntegration.Models.Transaction pendingTransaction =
                GenerateObjectsAndStringHelper.GenerateTransactionPending();
            _manager.Stub(x => x.GetTransaction(Arg<String>.Is.Anything)).IgnoreArguments().Return(pendingTransaction);
            _manager.Expect(x => x.SocketComsTransactionStatusEventHandler(Arg<DoshiiWebSocketsCommunication>.Is.Equal(MockSocketComs), Arg<TransactionEventArgs>.Matches(t => t.TransactionId == pendingTransaction.Id && t.Status == pendingTransaction.Status && t.Transaction.PaymentAmount == pendingTransaction.PaymentAmount)));
            MockSocketComs.ProcessSocketMessage(testSocketMessage);
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void TestLastSocketMessageTime_notReached()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateObjectsAndStringHelper.GenerateSocketMessage_AllData();
            MockSocketComs.m_LastSuccessfullSocketMessageTime = DateTime.MinValue;
            MockSocketComs.m_SocketConnectionTimeOutValue = 600;
            bool result = MockSocketComs.TestTimeOutValue();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void TestLastSocketMessageTime_beenReached()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateObjectsAndStringHelper.GenerateSocketMessage_AllData();
            MockSocketComs.m_LastSuccessfullSocketMessageTime = DateTime.Now;
            MockSocketComs.m_SocketConnectionTimeOutValue = 600;
            bool result = MockSocketComs.TestTimeOutValue();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void HanleOpenWebSocketsEvent()
        {
            MockSocketComs.Expect(x => x.SetLastSuccessfullSocketCommunicationTime());
            MockSocketComs.Expect(x => x.StartHeartbeatThread());
            _manager.Expect(
                x =>
                    x.SocketComsConnectionEventHandler(Arg<DoshiiWebSocketsCommunication>.Is.Equal(MockSocketComs),
                        Arg<EventArgs>.Is.Anything)).IgnoreArguments();

            _manager.Stub(x => x.SendConfigurationUpdate()).IgnoreArguments().Return(true);

            MockSocketComs.WebSocketsConnectionOnOpenEventHandler(this, new EventArgs());
            MockSocketComs.VerifyAllExpectations();
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void LastConnectionSuccessfulTimeGetsSet()
        {
            MockSocketComs.Expect(x => x.StartHeartbeatThread());
            _manager.Expect(
                x =>
                    x.SocketComsConnectionEventHandler(Arg<DoshiiWebSocketsCommunication>.Is.Equal(MockSocketComs),
                        Arg<EventArgs>.Is.Anything)).IgnoreArguments();

            _manager.Stub(x => x.SendConfigurationUpdate()).IgnoreArguments().Return(true);

            MockSocketComs.m_LastSuccessfullSocketMessageTime = DateTime.MinValue;
            Assert.AreEqual(MockSocketComs.m_LastSuccessfullSocketMessageTime, DateTime.MinValue);
            MockSocketComs.WebSocketsConnectionOnOpenEventHandler(this, new EventArgs());
            Assert.AreNotEqual(MockSocketComs.m_LastSuccessfullSocketMessageTime, DateTime.MinValue);
            MockSocketComs.VerifyAllExpectations();
            _manager.VerifyAllExpectations();
        }

        [Test]
        public void StartHeartBeatThread()
        {
            MockSocketComs.m_HeartBeatThread = null;
            MockSocketComs.StartHeartbeatThread();

            Assert.AreEqual(MockSocketComs.m_HeartBeatThread == null, false);
            
        }

        [Test]
        public void Connect_WhenWebSocketsConnectionIsNull_ShouldLog()
        {
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(typeof(DoshiiWebSocketsCommunication), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the was object")));
            
            MockSocketComs.m_WebSocketsConnection = null;
            MockSocketComs.Connect();

            _manager.VerifyAllExpectations();

        }

        [Test]
        public void Connect_ExceptionWhileConnecting_ShouldLog()
        {

            MockSocketComs.Stub(x => x.SetLastConnectionAttemptTime()).Throw(new Exception());
            _manager.mLog.mLog.Expect(x => x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(DoshiiWebSocketsCommunication)),Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<string>.Is.Anything, Arg<Exception>.Is.Anything));
            
            MockSocketComs.m_WebSocketsConnection = null;
            MockSocketComs.Connect();

            _manager.VerifyAllExpectations();

        }

        [Test]
        [ExpectedException(typeof(NotFiniteNumberException))]
        public void HeartBeatChecker_SocketReachedTimeOutValue()
        {
            MockSocketComs.Stub(x => x.TestTimeOutValue()).Return(false);
            _manager.Expect(x => x.SocketComsTimeOutValueReached(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything)).Throw(new NotFiniteNumberException());
            MockSocketComs.HeartBeatChecker();
        }

        
    }
}
