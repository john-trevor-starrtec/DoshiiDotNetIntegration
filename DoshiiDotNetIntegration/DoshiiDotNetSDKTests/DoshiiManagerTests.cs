using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class DoshiiManagerTests
    {
		IPaymentModuleManager paymentManager;
        IOrderingManager orderingManager;
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
            orderingManager = MockRepository.GenerateMock<IOrderingManager>();
			logger = MockRepository.GenerateMock<IDoshiiLogger>();
            _manager = new DoshiiManager(paymentManager, logger, orderingManager);
            _mockManager = MockRepository.GeneratePartialMock<DoshiiManager>(paymentManager, logger, orderingManager);

            socketUrl = "wss://alpha.corp.doshii.co/pos/api/v1/socket";
            token = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://alpha.corp.doshii.co/pos/api/v1";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContsuctNullPaymentManager()
		{
			var man = new DoshiiManager(null, logger, orderingManager);
		}

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullOrderManager()
        {
            var man = new DoshiiManager(paymentManager, logger, null);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketUrl()
        {
            _manager.Initialize("", token, urlBase, startWebSocketsConnection, socketTimeOutSecs, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoUrlBase()
        {
            _manager.Initialize(socketUrl, token, "", startWebSocketsConnection, socketTimeOutSecs, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _manager.Initialize(socketUrl, token, urlBase, startWebSocketsConnection, -1, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _manager.Initialize(socketUrl, "", urlBase, startWebSocketsConnection, socketTimeOutSecs, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }


    }
}
