using DoshiiDotNetIntegration;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class SocketCommunicationTests
    {
        DoshiiManager _manager;
		DoshiiDotNetIntegration.Interfaces.IDoshiiLogger Logger;
		DoshiiDotNetIntegration.DoshiiLogManager LogManager;
		DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager PaymentManager;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication SocketComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication MockSocketComs;

        [SetUp]
        public void Init()
        {
			Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
			LogManager = new DoshiiLogManager(Logger);
			PaymentManager = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager>();
            _manager = MockRepository.GenerateMock<DoshiiManager>(PaymentManager, Logger);
            MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            SocketComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
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
    }
}
