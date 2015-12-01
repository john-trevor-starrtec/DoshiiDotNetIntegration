using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class OperationLogicTest
    {
		IPaymentModuleManager paymentManager;
		IDoshiiLogger logger;
        DoshiiManager _manager;
        string socketUrl = "";
        string token = "";
        string urlBase = "";
        bool startWebSocketsConnection;
        int socketTimeOutSecs = 600;
        DoshiiManager _mockManager;

        
        [SetUp]
        public void Init()
        {
			paymentManager = MockRepository.GenerateMock<IPaymentModuleManager>();
			logger = MockRepository.GenerateMock<IDoshiiLogger>();
			_manager = new DoshiiManager(paymentManager, logger);
            _mockManager = MockRepository.GeneratePartialMock<DoshiiManager>(paymentManager, logger);

            socketUrl = "wss://alpha.corp.doshii.co/pos/api/v1/socket";
            token = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://alpha.corp.doshii.co/pos/api/v1";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContructNullPaymentManager()
		{
			var man = new DoshiiManager(null, logger);
		}
        
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketUrl()
        {
            _manager.Initialize("",token, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoUrlBase()
        {
            _manager.Initialize(socketUrl, token, "", startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _manager.Initialize(socketUrl, token, urlBase, startWebSocketsConnection, -1);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _manager.Initialize(socketUrl, "", urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }
    }
}
