using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
		public void BuildSocketUrl()
		{
			string expectedResult = String.Format("wss://alpha.corp.doshii.co/pos/socket?token={0}", token);
			string actualResult = _manager.BuildSocketUrl(urlBase, token);
			Assert.AreEqual(expectedResult, actualResult);
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
            _manager.Initialize(token, "", startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _manager.Initialize(token, urlBase, startWebSocketsConnection, -1);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _manager.Initialize("", urlBase, startWebSocketsConnection, socketTimeOutSecs);
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

            _mockManager.Initialize(token, urlBase, startWebSocketsConnection, 0);
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void Initialze_TokenGetsSetCorrectly()
        {

            _mockManager.Stub(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Equal(string.Format("{0}?token={1}", String.Format("{0}/socket", urlBase.Replace("http", "ws")), token)), Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);

            _mockManager.Initialize(token, urlBase, startWebSocketsConnection, 0);
            Assert.AreEqual(_mockManager.AuthorizeToken, token);
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
        public void GetUnlinkedOrders_CallGetUnlinkedOrder_throwsExceptionFromGetUnassignedOrders()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetUnlinkedOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            _mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());
                
            _mockManager.RefreshAllOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromHttpGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            MockHttpComs.Expect(x => x.GetTransactionsFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RefreshAllOrders();
        }

        [Test]
        public void RefreshAllOrders_CallGetUnlinkedOrder()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            //_mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(Arg<String>.Is.Anything));
            MockHttpComs.Expect(x => x.GetTransactionsFromDoshiiOrderId(Arg<String>.Is.Anything)).Return(GenerateObjectsAndStringHelper.GenerateTransactionList());
            _mockManager.Expect(x => x.HandleOrderCreated(Arg<Order>.Is.Anything, Arg<List<Transaction>>.Is.Anything));


            _mockManager.RefreshAllOrders();
            MockHttpComs.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        
        [Test]
        public void RequestPaymentForOrderExistingTransaction_ExceptionPaymentRequired()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.PaymentRequired;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrderExistingTransaction_ExceptionOther()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrderExistingTransaction_ExceptionOtherFromInterface()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrderExistingTransaction_Success()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            Transaction completeTransaction = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            completeTransaction.Id = transaction.Id;
            completeTransaction.Version = "1";
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);
            
            paymentManager.Expect(x => x.RecordSuccessfulPayment(completeTransaction));
            paymentManager.Expect(x => x.RecordTransactionVersion(completeTransaction.Id, completeTransaction.Version));

            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, true);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrderExistingTransaction_NullTransactionReturned()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            Transaction completeTransaction = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            completeTransaction.Id = transaction.Id;
            completeTransaction.Version = "1";
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(null);

            paymentManager.Expect(x => x.CancelPayment(transaction));
            
            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrderExistingTransaction_ReturnedTransactionNotSameTransaction()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            Transaction completeTransaction = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            completeTransaction.Id = "1";
            completeTransaction.Version = "1";
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            paymentManager.Expect(x => x.CancelPayment(transaction));

            bool success = _mockManager.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectTransaction_RestfulApiErrorResponseException()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            
            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPaymentForOrder_ExceptionOtherFromInterface()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Throw(new Exception());
            
            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectTransaction_Success()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            Transaction completeTransaction = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            completeTransaction.Id = transaction.Id;
            completeTransaction.Version = "1";
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.BadGateway;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, true);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPayment_NullTransactionReturned()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(null);

            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPayment_ReturnedTransactionNotSameTransaction()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            Transaction completeTransaction = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            completeTransaction.Id = "1";
            completeTransaction.Version = "1";
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }


        [Test]
        public void RecordTransactionVersion_ThrowsTransactionNotFoundException()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            
            paymentManager.Expect(x => x.RecordTransactionVersion(transaction.Id, transaction.Version))
                .Throw(new TransactionDoesNotExistOnPosException());
            
            paymentManager.Expect(x => x.CancelPayment(transaction));

            _mockManager.RecordTransactionVersion(transaction);
            
            paymentManager.VerifyAllExpectations();
        }

        [Test]
        public void RecordTransactionVersion_ThrowsException()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();

            paymentManager.Expect(x => x.RecordTransactionVersion(transaction.Id, transaction.Version))
                .Throw(new Exception());

            paymentManager.Expect(x => x.CancelPayment(transaction));

            _mockManager.RecordTransactionVersion(transaction);

            paymentManager.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrderFromDoshiiOrderId_ThrowsException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            MockHttpComs.Expect(x => x.GetOrder("1")).Throw(new RestfulApiErrorResponseException())
                .Return(GenerateObjectsAndStringHelper.GenerateOrderReadyToPay());

            _mockManager.GetOrderFromDoshiiOrderId("1");
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsTransactionNotFoundException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new OrderDoesNotExistOnPosException());

            _mockManager.RecordOrderCheckinId(order);
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new Exception());

            _mockManager.RecordOrderCheckinId(order);
        }


        [Test]
        public void HandleDeliveryOrderCreated_withTransactions_Success()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList)).Return(orderReturnedFromPos);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            paymentManager.Stub(x => x.RecordTransactionVersion(transactionList[0].Id, transactionList[0].Version));
            _mockManager.Expect(x => x.RecordTransactionVersion(transactionList[0]));
            _mockManager.Expect(x => x.RequestPaymentForOrderExistingTransaction(transactionList[0])).Return(true);

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandlePickupOrderCreated_withTransactions_Success()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList)).Return(orderReturnedFromPos);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            paymentManager.Stub(x => x.RecordTransactionVersion(transactionList[0].Id, transactionList[0].Version));
            _mockManager.Expect(x => x.RecordTransactionVersion(transactionList[0]));
            _mockManager.Expect(x => x.RequestPaymentForOrderExistingTransaction(transactionList[0])).Return(true);

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleOrderCreated_withTransactions_WrongType()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;


            _mockManager.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleOrderCreated_withNoTransactions_WrongType()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;


            _mockManager.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleOrderCreated_withNoTransactions_WrongType_NullTransactionList()
        {
            List<Transaction> transactionList = null;
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;


            _mockManager.Expect(x => x.RejectOrderFromOrderCreateMessage(order, new List<Transaction>()));

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetConsumerFromCheckinId_CallsHttpComsGetConsumerFromCheckinId()
        {
            string checkinId = "1";
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;


            MockHttpComs.Expect(x => x.GetConsumerFromCheckinId(checkinId))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetConsumerFromCheckinId(checkinId);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList)).Return(null);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            _mockManager.Expect(x => x.RejectPaymentForOrder(transactionList[0])).Return(true);
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_NullConsumerReturned()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(null);
            orderingManager.Expect(
                x =>
                    x.ConfirmNewDeliveryOrderWithFullPayment(Arg<Order>.Is.Anything, Arg<Consumer>.Is.Anything,
                        Arg<List<Transaction>>.Is.Anything)).Repeat.Never();
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandlePickupOrderCreated_withTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList)).Return(null);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            _mockManager.Expect(x => x.RejectPaymentForOrder(transactionList[0])).Return(true);
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumerForOrderCreated_GetConsumerSuccess()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            _mockManager.Expect(x => x.GetConsumerFromCheckinId(order.CheckinId)).Throw(new Exception());
            _mockManager.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));
            
            _mockManager.GetConsumerForOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            Transaction transactionFromPos = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Return(transactionFromPos);

            _mockManager.Expect(x => x.RequestPaymentForOrderExistingTransaction(transactionFromPos)).Return(true);
             
            _mockManager.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived_PosResurnsNull()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Return(null);

            _mockManager.Expect(x => x.RejectPaymentForOrder(transaction)).Return(true);

            _mockManager.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived_OrderDoesNoteExistOnPos()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Throw(new OrderDoesNotExistOnPosException());

            _mockManager.Expect(x => x.RejectPaymentForOrder(transaction)).Return(true);

            _mockManager.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withoutTransactions_Success()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrder(order, consumer)).Return(orderReturnedFromPos);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandlePickupOrderCreated_withoutTransactions_Success()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewPickupOrder(order, consumer)).Return(orderReturnedFromPos);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withoutTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrder(order, consumer)).Return(null);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandlePickupOrderCreated_withoutTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestToken, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;

            orderingManager.Expect(x => x.ConfirmNewPickupOrder(order, consumer)).Return(null);
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);

            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
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


            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation().Name);
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

            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation().Name);
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

            _mockManager.AddTableAllocation(GenerateObjectsAndStringHelper.TestOrderId, GenerateObjectsAndStringHelper.GenerateTableAllocation().Name);
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
    }
}
