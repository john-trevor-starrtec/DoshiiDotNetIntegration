using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Net;
using AutoMapper.Internal;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class DoshiiManagerTests
    {
		IPaymentModuleManager paymentManager;
        IOrderingManager orderingManager;
        DoshiiDotNetIntegration.Interfaces.IDoshiiLogger _Logger;
        DoshiiDotNetIntegration.DoshiiLogManager LogManager;
        DoshiiManager _manager;
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
            _Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
            LogManager = new DoshiiLogManager(_Logger);
            _manager = new DoshiiManager(paymentManager, _Logger, orderingManager);
            _mockManager = MockRepository.GeneratePartialMock<DoshiiManager>(paymentManager, _Logger, orderingManager);

            token = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://alpha.corp.doshii.co/pos/api/v1";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContsuctNullPaymentManager()
		{
			var man = new DoshiiManager(null, _Logger, orderingManager);
		}

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullOrderManager()
        {
            var man = new DoshiiManager(paymentManager, _Logger, null);
        }
        
        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoUrlBase()
        {
            _manager.Initialize(token, "", startWebSocketsConnection, socketTimeOutSecs, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _manager.Initialize(token, urlBase, startWebSocketsConnection, -1, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _manager.Initialize("", urlBase, startWebSocketsConnection, socketTimeOutSecs, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialze_NullConfiguration()
        {
            _mockManager.Initialize(token, urlBase, startWebSocketsConnection, socketTimeOutSecs, null);
        }

        [Test]
        public void UnsubscribeFromSocketEvents_CallecWhenSocketCommsSet()
        {
            _mockManager.Expect((x => x.UnsubscribeFromSocketEvents()));
            _mockManager.SocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);

            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void GetSocketComms_returnsSocketComms()
        {
            var socketComms = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);
            _mockManager.SocketComs = socketComms;

            Assert.AreEqual(socketComms, _mockManager.SocketComs);
        }

        [Test]
        public void Initialze_SocketTimeOutIs0_SocketTimeOutSetToDefaultValue()
        {
            
            _mockManager.mLog.mLog.Expect(
                x =>
                    x.LogDoshiiMessage(typeof (DoshiiManager), DoshiiLogLevels.Info,
                        String.Format("Doshii will use default timeout of {0}", DoshiiManager.DefaultTimeout)));
            _mockManager.Expect(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Equal(DoshiiManager.DefaultTimeout))).Return(true);

            _mockManager.Initialize(token, urlBase, startWebSocketsConnection, 0, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void Initialze_TokenGetsSetCorrectly()
        {

            _mockManager.Stub(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Equal(string.Format("{0}?token={1}", String.Format("{0}/socket", urlBase.Replace("http", "ws")), token)), Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);

            _mockManager.Initialize(token, urlBase, startWebSocketsConnection, 0, GenerateObjectsAndStringHelper.GenerateConfiguration(true, true));
            Assert.AreEqual(_mockManager.AuthorizeToken, token);
            Assert.AreEqual(_mockManager.mConfiguration.CheckoutOnPaid, true);
            Assert.AreEqual(_mockManager.mConfiguration.DeallocateTableOnPaid, true);
        }

        [Test]
        public void InitialzeProcess_HttpAndSocketsAreInitialized()
        {
            _mockManager.AuthorizeToken = token;
            _mockManager.InitializeProcess(String.Format("{0}/socket", urlBase.Replace("http", "ws")), urlBase, true, 30);
            Assert.AreNotEqual(_mockManager.SocketComs, null);
            Assert.AreNotEqual(_mockManager.m_HttpComs, null);
        }

        [Test]
        public void InitialzeProcess_SocketsNotInitialized()
        {
            _mockManager.AuthorizeToken = token;
            _mockManager.SocketComs = null;
            _mockManager.InitializeProcess(String.Format("{0}/socket", urlBase.Replace("http", "ws")), urlBase, false, 30);
            Assert.AreEqual(_mockManager.SocketComs, null);
            
        }

        [Test]
        public void InitialzeProcess_LogsExceptionFromSocketInitialize()
        {
            _mockManager.Stub(
                x =>
                    x.SubscribeToSocketEvents()).Throw(new Exception());
            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(DoshiiManager)), Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<String>.Is.Anything, Arg<Exception>.Is.Anything));
            
            _mockManager.AuthorizeToken = token;
            _mockManager.InitializeProcess(String.Format("{0}/socket", urlBase.Replace("http", "ws")), urlBase, true, 30);

            _Logger.VerifyAllExpectations();
        }

        [Test]
        public void SendConfigurationAfterSocketComsConnection()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.Expect(x => x.SendConfigurationUpdate()).Return(true);
            _mockManager.SocketComsConnectionEventHandler(this, new EventArgs());

            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void LogSocketTimeOutValueReached()
        {
            var MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _manager);

            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(DoshiiManager)), Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<String>.Is.Equal("Doshii: SocketComsTimeoutValueReached"), Arg<Exception>.Is.Anything));

            _mockManager.SocketComsTimeOutValueReached(MockSocketComs, new EventArgs());

            _Logger.VerifyAllExpectations();
        }

        [Test]
        public void RecordOrderVersion_ThrowsOrderDoesNotExistOnPosException()
        {
            orderingManager.Expect(
                x =>
                    x.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
                        GenerateObjectsAndStringHelper.TestVersion)).Throw(new OrderDoesNotExistOnPosException());

            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Anything, Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Info), Arg<String>.Is.Equal(string.Format("Doshii: Attempted to update an order version that does not exist on the Pos, OrderId - {0}, version - {1}", GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.TestVersion)), Arg<Exception>.Is.Anything));


            _mockManager.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
                GenerateObjectsAndStringHelper.TestVersion);

            _Logger.VerifyAllExpectations();
        }

        [Test]
        public void RecordOrderVersion_ThrowsException()
        {
            orderingManager.Expect(
                x =>
                    x.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
                        GenerateObjectsAndStringHelper.TestVersion)).Throw(new Exception());

            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Anything, Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<String>.Is.Anything, Arg<Exception>.Is.Anything));


            _mockManager.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
                GenerateObjectsAndStringHelper.TestVersion);

            _Logger.VerifyAllExpectations();
        }


        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrder_CallGetOrder()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetOrder(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_CallGetOrders()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetOrders();

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transaction()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId);

        }
        
        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transactions()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetTransactions())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetTransactions();

        }

        [Test]
        public void UpdateOrder_Calls_PutOrder()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            
            MockHttpComs.Expect(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Return(orderToUpdate);

            Order returnedOrder = _mockManager.UpdateOrder(orderToUpdate);
            Assert.AreEqual(orderToUpdate, returnedOrder);
            
            _mockManager.VerifyAllExpectations();

        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_ReturnsOrderId_EqualZero()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            Order orderToReturn = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            orderToReturn.Id = "0";
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Return(orderToReturn);

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(ConflictWithOrderUpdateException))]
        public void UpdateOrder_PutOrder_Conflict()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.Conflict));

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_RestfulApiErrorResponseException_OtherThanConflict()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_NullOrderReturnedException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new NullOrderReturnedException());

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_GeneralException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new Exception());

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderDoesNotExistOnPosException))]
        public void AddTableAllocaiton_OrderDoesNotExistOnPos()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new OrderDoesNotExistOnPosException());


            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation());
        }

        [Test]
        [ExpectedException(typeof(OrderDoesNotExistOnPosException))]
        public void AddTableAllocaiton_OrderReturnedFromPosIsNull()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(null);


            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation());
        }

        [Test]
        [ExpectedException(typeof(ConflictWithOrderUpdateException))]
        public void AddTableAllocaiton_PutOrderWithTableAllocationConflict()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.PutOrderWithTableAllocation(Arg<TableOrder>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.Conflict));
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation());
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void AddTableAllocaiton_PutOrderWithTableAllocationException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.PutOrderWithTableAllocation(Arg<TableOrder>.Is.Anything))
                .Throw(new Exception());
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation());
        }

        [Test]
        public void DeleteTableAllocaiton_Successful()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(true);

            bool success = _mockManager.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(success, true);
        }

        [Test]
        [ExpectedException(typeof(ConflictWithOrderUpdateException))]
        public void DeleteTableAllocaiton_Conflict()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.Conflict));

            _mockManager.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void DeleteTableAllocaiton_RestfulApiErrorResponseException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockManager.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void DeleteTableAllocaiton_Exception()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new Exception());

            _mockManager.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId);
        }

        [Test]
        public void GetConfig_Success()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetConfig()).Return(new Configuration(true, true));
                
            Configuration testConfig = _mockManager.GetConfiguration();
            Assert.AreEqual(testConfig.CheckoutOnPaid, true);
            Assert.AreEqual(testConfig.DeallocateTableOnPaid, true);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetConfig_RestfulApiErrorResponseException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Stub(x => x.GetConfig()).Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockManager.GetConfiguration();
        }

        [Test]
        public void UpdateConfiguration_Success()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(
                x =>
                    x.PutConfiguration(
                        Arg<Configuration>.Matches(c => c.CheckoutOnPaid == true && c.DeallocateTableOnPaid == true))).Return(true);

            
            _mockManager.mConfiguration = new Configuration(false, false);
            Assert.AreEqual(_mockManager.mConfiguration.CheckoutOnPaid, false);
            Assert.AreEqual(_mockManager.mConfiguration.DeallocateTableOnPaid, false);
            _mockManager.UpdateConfiguration(new Configuration(true, true));

            Assert.AreEqual(_mockManager.mConfiguration.CheckoutOnPaid, true);
            Assert.AreEqual(_mockManager.mConfiguration.DeallocateTableOnPaid, true);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void UpdateConfiguration_RestfulApiErrorResponseException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(
                x =>
                    x.PutConfiguration(
                        Arg<Configuration>.Matches(c => c.CheckoutOnPaid == true && c.DeallocateTableOnPaid == true)))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));
            _mockManager.UpdateConfiguration(new Configuration(true, true));
        }



        
        
        
       

        
        
       

    }
}
