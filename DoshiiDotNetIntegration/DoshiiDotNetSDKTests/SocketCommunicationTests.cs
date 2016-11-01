using DoshiiDotNetIntegration;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using WebSocketSharp;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class SocketCommunicationTests
    {
        DoshiiController _controller;
		DoshiiDotNetIntegration.Interfaces.ILoggingManager _Logger;
		DoshiiDotNetIntegration.LoggingController LogManager;
		DoshiiDotNetIntegration.Interfaces.ITransactionManager PaymentManager;
        DoshiiDotNetIntegration.Interfaces.IOrderingManager OrderingManager;
        DoshiiDotNetIntegration.CommunicationLogic.SocketsController SocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.SocketsController MockSocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.HttpController MockHttpComs;
        IRewardManager MemberbershipManager;

        [SetUp]
        public void Init()
        {
			_Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.ILoggingManager>();
			LogManager = new LoggingController(_Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.ITransactionManager>();
            OrderingManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IOrderingManager>();
            MemberbershipManager = MockRepository.GenerateMock<IRewardManager>();
            _controller = MockRepository.GeneratePartialMock<DoshiiController>(PaymentManager, _Logger, OrderingManager, MemberbershipManager);
            
            MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.SocketsController>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);
            SocketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);
            _controller._httpComs = MockHttpComs;
            _controller.SocketComs = MockSocketComs;
            
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoUrl()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController("", GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);
        }

        [Test]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NoOperationLogic()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, null);
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_NoLogManager()
		{
			var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, null, _controller);
		}

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Constructor_TimeoutLessThan10Secs()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController(GenerateObjectsAndStringHelper.TestSocketUrl, 0, LogManager, _controller);
        }

        [Test]
        public void Constructor_AllParamatersCorrect()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.SocketsController(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);
            Assert.AreEqual(socketComs._socketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs._socketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_DoshiiLogic, _controller);
			Assert.AreEqual(socketComs._logger, LogManager);
            Assert.IsNotNull(socketComs._webSocketsConnection);
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
            SocketComs._lastConnectionAttemptTime = DateTime.MinValue;
            SocketComs.SetLastConnectionAttemptTime();

            Assert.GreaterOrEqual(SocketComs._lastConnectionAttemptTime, testDateTime);
        }

        [Test]
        public void TestLastSocketMessageTime_notReached()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateObjectsAndStringHelper.GenerateSocketMessage_AllData();
            MockSocketComs._lastSuccessfullSocketMessageTime = DateTime.MinValue;
            MockSocketComs._socketConnectionTimeOutValue = 600;
            bool result = MockSocketComs.TestTimeOutValue();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void TestLastSocketMessageTime_beenReached()
        {
            DoshiiDotNetIntegration.Models.Json.SocketMessage testSocketMessage = GenerateObjectsAndStringHelper.GenerateSocketMessage_AllData();
            MockSocketComs._lastSuccessfullSocketMessageTime = DateTime.Now;
            MockSocketComs._socketConnectionTimeOutValue = 600;
            bool result = MockSocketComs.TestTimeOutValue();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void HanleOpenWebSocketsEvent()
        {
            MockSocketComs.Expect(x => x.SetLastSuccessfullSocketCommunicationTime());
            MockSocketComs.Expect(x => x.StartHeartbeatThread());
            _controller.Expect(
                x =>
                    x.SocketComsConnectionEventHandler(Arg<SocketsController>.Is.Equal(MockSocketComs),
                        Arg<EventArgs>.Is.Anything)).IgnoreArguments();

            MockSocketComs.WebSocketsConnectionOnOpenEventHandler(this, new EventArgs());
            MockSocketComs.VerifyAllExpectations();
            _controller.VerifyAllExpectations();
        }

        [Test]
        public void LastConnectionSuccessfulTimeGetsSet()
        {
            MockSocketComs.Expect(x => x.StartHeartbeatThread());
            _controller.Expect(
                x =>
                    x.SocketComsConnectionEventHandler(Arg<SocketsController>.Is.Equal(MockSocketComs),
                        Arg<EventArgs>.Is.Anything)).IgnoreArguments();

            MockSocketComs._lastSuccessfullSocketMessageTime = DateTime.MinValue;
            Assert.AreEqual(MockSocketComs._lastSuccessfullSocketMessageTime, DateTime.MinValue);
            MockSocketComs.WebSocketsConnectionOnOpenEventHandler(this, new EventArgs());
            Assert.AreNotEqual(MockSocketComs._lastSuccessfullSocketMessageTime, DateTime.MinValue);
            MockSocketComs.VerifyAllExpectations();
            _controller.VerifyAllExpectations();
        }

        [Test]
        public void StartHeartBeatThread()
        {
            MockSocketComs._heartBeatThread = null;
            MockSocketComs.StartHeartbeatThread();

            Assert.AreEqual(MockSocketComs._heartBeatThread == null, false);
            
        }

        [Test]
        public void Connect_WhenWebSocketsConnectionIsNull_ShouldLog()
        {
            _controller._logger.mLog.Expect(x => x.LogDoshiiMessage(typeof(SocketsController), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the was object")));
            
            MockSocketComs._webSocketsConnection = null;
            MockSocketComs.Connect();

            _controller.VerifyAllExpectations();

        }

        [Test]
        public void Connect_ExceptionWhileConnecting_ShouldLog()
        {

            MockSocketComs.Stub(x => x.SetLastConnectionAttemptTime()).Throw(new Exception());
            _controller._logger.mLog.Expect(x => x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(SocketsController)),Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<string>.Is.Anything, Arg<Exception>.Is.Anything));
            
            MockSocketComs._webSocketsConnection = null;
            MockSocketComs.Connect();

            _controller.VerifyAllExpectations();

        }

        [Test]
        [ExpectedException(typeof(NotFiniteNumberException))]
        public void HeartBeatChecker_SocketReachedTimeOutValue()
        {
            MockSocketComs.Stub(x => x.TestTimeOutValue()).Return(false);
            _controller.Expect(x => x.SocketComsTimeOutValueReached(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything)).Throw(new NotFiniteNumberException());
            MockSocketComs.HeartBeatChecker();
        }

        
    }
}
