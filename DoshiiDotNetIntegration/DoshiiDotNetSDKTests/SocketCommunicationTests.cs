using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net;
using WebSocketSharp;
using DoshiiDotNetIntegration;
using WebSocketSharp;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class SocketCommunicationTests
    {
        DoshiiManagement _management;
        DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering OrderingInterface;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication SocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication MockSocketComs;

        [SetUp]
        public void Init()
        {
            OrderingInterface = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering>();
            _management = MockRepository.GenerateMock<DoshiiManagement>(OrderingInterface);
            MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, _management, GenerateObjectsAndStringHelper.TestTimeOutValue);
            SocketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, _management, GenerateObjectsAndStringHelper.TestTimeOutValue);
            
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_NoUrl()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication("", _management, GenerateObjectsAndStringHelper.TestTimeOutValue);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_NoOperationLogic()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, null, GenerateObjectsAndStringHelper.TestTimeOutValue);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_TimeoutLessThank10Secs()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, null, 0);
        }

        [Test]
        public void Constructor_AllParamatersCorrect()
        {
            var socketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, _management, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_SocketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_SocketConnectionTimeOutValue, GenerateObjectsAndStringHelper.TestTimeOutValue);
            Assert.AreEqual(socketComs.m_DoshiiLogic, _management);
            Assert.AreNotEqual(socketComs.m_WebSocketsConnection, null);
            
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

        //[Test]
        ////cant these that.Connect gets called on the webSockets class need to investigate 
        //public void Connect_ShouldCall_SetLastConnectionTime()
        //{
        //    MockSocketComs.m_LastConnectionAttemptTime = DateTime.MinValue;

        //    MockSocketComs.m_WebSocketsConnection = MockRepository.GenerateStub<WebSocket>("ws://www.google.com", new string[0]);
        //    MockSocketComs.Expect(x => x.SetLastConnectionAttemptTime()).Repeat.Once();
            
        //    MockSocketComs.Connect();
        //    MockSocketComs.VerifyAllExpectations();
        //}
    }
}
