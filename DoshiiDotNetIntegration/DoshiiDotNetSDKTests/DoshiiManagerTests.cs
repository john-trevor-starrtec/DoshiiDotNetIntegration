using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using Rhino.Mocks.Constraints;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class DoshiiManagerTests
    {
		ITransactionManager paymentManager;
        IOrderingManager orderingManager;
        IRewardManager membershipManager;
        DoshiiDotNetIntegration.Interfaces.ILoggingManager _Logger;
        DoshiiDotNetIntegration.LoggingController LogManager;
        DoshiiController _controller;
        string locationToken = "";
        string urlBase = "";
        bool startWebSocketsConnection;
        int socketTimeOutSecs = 600;
        DoshiiController _mockController;
        string vendor = "testVendor";
        string secretKey = "wer";
        string checkinId = "checkinId";
        
        
        [SetUp]
        public void Init()
        {
			paymentManager = MockRepository.GenerateMock<ITransactionManager>();
            orderingManager = MockRepository.GenerateMock<IOrderingManager>();
            membershipManager = MockRepository.GenerateMock<IRewardManager>();
            _Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.ILoggingManager>();
            LogManager = new LoggingController(_Logger);
            _controller = new DoshiiController(paymentManager, _Logger, orderingManager, membershipManager);
            _mockController = MockRepository.GeneratePartialMock<DoshiiController>(paymentManager, _Logger, orderingManager, membershipManager);

            locationToken = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://sandbox.doshii.co/pos/v3";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

		[Test]
		public void BuildSocketUrl()
		{
            string expectedResult = String.Format("wss://sandbox-socket.doshii.co/pos/socket?token={0}", locationToken);
            string actualResult = _controller.BuildSocketUrl(urlBase, locationToken);
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContsuctNullPaymentManager()
		{
			var man = new DoshiiController(null, _Logger, orderingManager, null);
		}

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullLoggerManager()
        {
            var man = new DoshiiController(paymentManager, null, orderingManager, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullOrderManager()
        {
            var man = new DoshiiController(paymentManager, _Logger, null, null);
        }
        
        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoUrlBase()
        {
            _controller.Initialize(locationToken, vendor, secretKey, "", startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoVendor()
        {
            _controller.Initialize(locationToken, "", secretKey, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSecretKey()
        {
            _controller.Initialize(locationToken, vendor, "", urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _controller.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, -1);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _controller.Initialize("", vendor, secretKey, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        public void Initialze_StartSocketConnectIsTrue()
        {
            _mockController.Expect((x => x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<int>.Is.Anything))).Return(true);
            _mockController.Initialize(locationToken, vendor, secretKey, urlBase, true, socketTimeOutSecs);

            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void UnsubscribeFromSocketEvents_CallecWhenSocketCommsSet()
        {
            _mockController.Expect((x => x.UnsubscribeFromSocketEvents()));
            _mockController.SocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.SocketsController>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);

            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void GetSocketComms_returnsSocketComms()
        {
            var socketComms = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.SocketsController>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);
            _mockController.SocketComs = socketComms;

            Assert.AreEqual(socketComms, _mockController.SocketComs);
        }

        [Test]
        public void Initialze_SocketTimeOutIs0_SocketTimeOutSetToDefaultValue()
        {
            
            _mockController._logger.mLog.Expect(
                x =>
                    x.LogDoshiiMessage(typeof (DoshiiController), DoshiiLogLevels.Info,
                        String.Format("Doshii will use default timeout of {0}", DoshiiController.DefaultTimeout)));
            _mockController.Expect(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Equal(DoshiiController.DefaultTimeout))).Return(true);

            _mockController.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, 0);
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void Initialze_TokenGetsSetCorrectly()
        {

            _mockController.Stub(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Equal(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken)), Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);

            _mockController.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, 0);
            Assert.AreEqual(_mockController.LocationToken, locationToken);
        }

        [Test]
        public void InitialzeProcess_HttpAndSocketsAreInitialized()
        {
            _mockController.LocationToken = locationToken;
            _mockController.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, true, 30);
            Assert.AreNotEqual(_mockController.SocketComs, null);
            Assert.AreNotEqual(_mockController._httpComs, null);
        }

        [Test]
        public void InitialzeProcess_SocketsNotInitialized()
        {
            _mockController.LocationToken = locationToken;
            _mockController.SocketComs = null;
            _mockController.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, false, 30);
            Assert.AreEqual(_mockController.SocketComs, null);
            
        }

        [Test]
        public void InitialzeProcess_LogsExceptionFromSocketInitialize()
        {
            _mockController.Stub(
                x =>
                    x.SubscribeToSocketEvents()).Throw(new Exception());
            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(DoshiiController)), Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<String>.Is.Anything, Arg<Exception>.Is.Anything));

            _mockController.LocationToken = locationToken;
            _mockController.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, true, 30);

            _Logger.VerifyAllExpectations();
        }

        [Test]
        public void LogSocketTimeOutValueReached()
        {
            var MockSocketComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.SocketsController>(GenerateObjectsAndStringHelper.TestSocketUrl, GenerateObjectsAndStringHelper.TestTimeOutValue, LogManager, _controller);

            _Logger.Expect(
                x =>
                    x.LogDoshiiMessage(Arg<Type>.Is.Equal(typeof(DoshiiController)), Arg<DoshiiLogLevels>.Is.Equal(DoshiiLogLevels.Error), Arg<String>.Is.Equal("Doshii: SocketComsTimeoutValueReached"), Arg<Exception>.Is.Anything));

            _mockController.SocketComsTimeOutValueReached(MockSocketComs, new EventArgs());

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


            _mockController.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
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


            _mockController.RecordOrderVersion(GenerateObjectsAndStringHelper.TestOrderId,
                GenerateObjectsAndStringHelper.TestVersion);

            _Logger.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetUnlinkedOrders_CallGetUnlinkedOrder_throwsExceptionFromGetUnassignedOrders()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetUnlinkedOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            _mockController.Expect(x => x.GetTransactionFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());
                
            _mockController.RefreshAllOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromHttpGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            MockHttpComs.Expect(x => x.GetTransactionsFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());

            _mockController.RefreshAllOrders();
        }

        [Test]
        public void RefreshAllOrders_CallGetUnlinkedOrder()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            //_mockController.Expect(x => x.GetTransactionFromDoshiiOrderId(Arg<String>.Is.Anything));
            MockHttpComs.Expect(x => x.GetTransactionsFromDoshiiOrderId(Arg<String>.Is.Anything)).Return(GenerateObjectsAndStringHelper.GenerateTransactionList());
            _mockController.Expect(x => x.HandleOrderCreated(Arg<Order>.Is.Anything, Arg<List<Transaction>>.Is.Anything));


            _mockController.RefreshAllOrders();
            MockHttpComs.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        
        [Test]
        public void RequestPaymentForOrderExistingTransaction_ExceptionPaymentRequired()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            newException.StatusCode = HttpStatusCode.PaymentRequired;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            paymentManager.Expect(x => x.CancelPayment(Arg<Transaction>.Is.Anything));

            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);
            
            paymentManager.Expect(x => x.RecordSuccessfulPayment(completeTransaction));
            paymentManager.Expect(x => x.RecordTransactionVersion(completeTransaction.Id, completeTransaction.Version));

            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(null);

            paymentManager.Expect(x => x.CancelPayment(transaction));
            
            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            paymentManager.Expect(x => x.CancelPayment(transaction));

            bool success = _mockController.RequestPaymentForOrderExistingTransaction(transaction);
            Assert.AreEqual(success, false);
            paymentManager.VerifyAllExpectations();
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectTransaction_RestfulApiErrorResponseException()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            RestfulApiErrorResponseException newException = new RestfulApiErrorResponseException();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            
            bool success = _mockController.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPaymentForOrder_ExceptionOtherFromInterface()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Throw(new Exception());
            
            bool success = _mockController.RejectPaymentForOrder(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            bool success = _mockController.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, true);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPayment_NullTransactionReturned()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(null);

            bool success = _mockController.RejectPaymentForOrder(transaction);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction))
                .Return(completeTransaction);

            bool success = _mockController.RejectPaymentForOrder(transaction);
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

            _mockController.RecordTransactionVersion(transaction);
            
            paymentManager.VerifyAllExpectations();
        }

        [Test]
        public void RecordTransactionVersion_ThrowsException()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();

            paymentManager.Expect(x => x.RecordTransactionVersion(transaction.Id, transaction.Version))
                .Throw(new Exception());

            paymentManager.Expect(x => x.CancelPayment(transaction));

            _mockController.RecordTransactionVersion(transaction);

            paymentManager.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrderFromDoshiiOrderId_ThrowsException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrderFromDoshiiOrderId("1")).Throw(new RestfulApiErrorResponseException())
                .Return(GenerateObjectsAndStringHelper.GenerateOrderReadyToPay());

            _mockController.GetOrderFromDoshiiOrderId("1");
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsTransactionNotFoundException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new OrderDoesNotExistOnPosException());

            _mockController.RecordOrderCheckinId(order.Id, checkinId);
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new Exception());

            _mockController.RecordOrderCheckinId(order.Id, checkinId);
        }


        

        [Test]
        public void HandleOrderCreated_withTransactions_WrongType()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));
            
            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandleOrderCreated_withNoTransactions_WrongType()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            
            _mockController.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));

            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandleOrderCreated_withNoTransactions_WrongType_NullTransactionList()
        {
            List<Transaction> transactionList = null;
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.RejectOrderFromOrderCreateMessage(order, new List<Transaction>()));

            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetConsumerFromCheckinId_CallsHttpComsGetConsumerFromCheckinId()
        {
            string checkinId = "1";
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetConsumerFromCheckinId(checkinId))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetConsumerFromCheckinId(checkinId);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList));
            _mockController.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_NullConsumerReturned()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(null);
            orderingManager.Expect(
                x =>
                    x.ConfirmNewDeliveryOrderWithFullPayment(Arg<Order>.Is.Anything, Arg<Consumer>.Is.Anything,
                        Arg<List<Transaction>>.Is.Anything)).Repeat.Never();
            
            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumerForOrderCreated_GetConsumerSuccess()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;

            _mockController.Expect(x => x.GetConsumerFromCheckinId(order.CheckinId)).Throw(new Exception());
            _mockController.Expect(x => x.RejectOrderFromOrderCreateMessage(order, transactionList));
            
            _mockController.GetConsumerForOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            Transaction transactionFromPos = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Return(transactionFromPos);

            _mockController.Expect(x => x.RequestPaymentForOrderExistingTransaction(transactionFromPos)).Return(true);
             
            _mockController.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived_PosResurnsNull()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Return(null);

            _mockController.Expect(x => x.RejectPaymentForOrder(transaction)).Return(true);

            _mockController.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandelPendingTransactionReceived_OrderDoesNoteExistOnPos()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Throw(new OrderDoesNotExistOnPosException());

            _mockController.Expect(x => x.RejectPaymentForOrder(transaction)).Return(true);

            _mockController.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }
        
        [Test]
        public void HandleDeliveryOrderCreated_withoutTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrder(order, consumer));
            _mockController.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            
            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withTransactions_Success()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;


            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList));
            _mockController.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);

            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void HandlePickupOrderCreated_withTransactions_Success()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList));
            _mockController.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            
            
            _mockController.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void AcceptOrderAheadCreation_withoutTransactions()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController.LocationToken = locationToken;
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetOrderFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(order);
            _mockController.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(new List<Transaction>());
            _mockController.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos)).Return(orderReturnedFromPos);

            _mockController.AcceptOrderAheadCreation(orderReturnedFromPos);
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void AcceptOrderAheadCreation_withTransactions()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController.LocationToken = locationToken;
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetOrderFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(order);
            _mockController.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(transactionList);
            _mockController.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos)).Return(orderReturnedFromPos);
            _mockController.Expect(x => x.RequestPaymentForOrderExistingTransaction(Arg<Transaction>.Is.Anything)).Return(true);
            
            _mockController.AcceptOrderAheadCreation(orderReturnedFromPos);
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void RejectOrderAheadCreation_withTransactions()
        {
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController.LocationToken = locationToken;
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(GenerateObjectsAndStringHelper.GenerateTransactionList());
            _mockController.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos));
            _mockController.Expect(x => x.RejectPaymentForOrder(Arg<Transaction>.Is.Anything));
            
            _mockController.RejectOrderAheadCreation(orderReturnedFromPos);
            _mockController.VerifyAllExpectations();
        }

        [Test]
        public void RecordPosTransactionOnDoshii()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PostTransaction(transaction)).Return(transaction);

            _mockController.RecordPosTransactionOnDoshii(transaction);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrderCreatedResult_Success()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutOrderCreatedResult(order)).Return(order);

            _mockController.PutOrderCreatedResult(order);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrderNoPosId()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            order.Status = "accepted";
            order.Id = null;
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrder_ReturnedOrderNoPosId()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            Order returnedOrder = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            returnedOrder.Status = "accepted";
            returnedOrder.Id = "0";
            returnedOrder.DoshiiId = "0";

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Return(returnedOrder);

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrder_RestfulApiErrorResponseException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            Order returnedOrder = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            returnedOrder.Status = "accepted";
            returnedOrder.Id = "0";
            returnedOrder.DoshiiId = "0";

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new RestfulApiErrorResponseException());

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(ConflictWithOrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrder_Conflict()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            Order returnedOrder = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            returnedOrder.Status = "accepted";
            returnedOrder.Id = "0";
            returnedOrder.DoshiiId = "0";

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new RestfulApiErrorResponseException(){StatusCode = HttpStatusCode.Conflict});

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrder_NullResponse()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            Order returnedOrder = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            returnedOrder.Status = "accepted";
            returnedOrder.Id = "0";
            returnedOrder.DoshiiId = "0";

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new NullResponseDataReturnedException());

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrder_GeneralException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            Order returnedOrder = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            returnedOrder.Status = "accepted";
            returnedOrder.Id = "0";
            returnedOrder.DoshiiId = "0";

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockController);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new Exception());

            _mockController.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrder_CallGetOrder()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetOrder(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_CallGetOrders()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetOrders();

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transaction()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId);

        }
        
        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transactions()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetTransactions())
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetTransactions();

        }

        [Test]
        public void UpdateOrder_Calls_PutOrder()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            
            MockHttpComs.Expect(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Return(orderToUpdate);

            Order returnedOrder = _mockController.UpdateOrder(orderToUpdate);
            Assert.AreEqual(orderToUpdate, returnedOrder);
            
            _mockController.VerifyAllExpectations();

        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_Conflict()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.Conflict));

            _mockController.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_RestfulApiErrorResponseException_OtherThanConflict()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockController.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_NullOrderReturnedException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new NullResponseDataReturnedException());

            _mockController.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_GeneralException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new Exception());

            _mockController.UpdateOrder(orderToUpdate);
        }

        [Test]
        public void GetMember_Success()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetMember(GenerateObjectsAndStringHelper.TestMemberId));
            
            _mockController.GetMember(GenerateObjectsAndStringHelper.TestMemberId);
        
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMember_Failed_WithException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

        }

        [Test]
        public void GetMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            
            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockController.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            _mockController.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

        }


        [Test]
        public void GetMembers_Success()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetMembers());

            _mockController.GetMembers();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMembers_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetMembers();

        }

        [Test]
        public void GetMembers_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMembersList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);
            MockHttpComs.Stub(x => x.GetMember(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(GenerateObjectsAndStringHelper.GenerateMember1());
            MockHttpComs.Stub(x => x.GetMember(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(GenerateObjectsAndStringHelper.GenerateMember2());

            _mockController.GetMembers();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMembers_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            _mockController.GetMembers();

        }

        [Test]
        public void DeleteMember_Success()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1())).Return(true);

            _mockController.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void DeleteMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void DeleteMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockController.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void DeleteMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            _mockController.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void CreateMember_Success()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            member.Id = "";
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);
            MockHttpComs.Expect(x => x.PostMember(member)).Return(GenerateObjectsAndStringHelper.GenerateMember1());
            
            _mockController.UpdateMember(member);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void UpdateMember_Success()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutMember(GenerateObjectsAndStringHelper.GenerateMember1()));

            _mockController.UpdateMember(member);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void UpdateMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void UpdateMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockController.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }



        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void UpdateMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            _mockController.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }


        [Test]
        public void SyncMembersWithDoshii_DeleteMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            doshiiMemberList.RemoveAt(0);

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController._rewardManager = membershipManager;
            _mockController.IsInitalized = true;
            
            _mockController.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.DeleteMemberOnPos(GenerateObjectsAndStringHelper.GenerateMember1()));
            
            _mockController.SyncDoshiiMembersWithPosMembers();

            _mockController.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void SyncMembersWithDoshii_CreateMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            posMemberList.RemoveAt(0);

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController._rewardManager = membershipManager;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.CreateMemberOnPos(GenerateObjectsAndStringHelper.GenerateMember1()));

            _mockController.SyncDoshiiMembersWithPosMembers();

            _mockController.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void SyncMembersWithDoshii_UpdateMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            doshiiMemberList[0].Email = "newemail@email.com";

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController._rewardManager = membershipManager;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.UpdateMemberOnPos(doshiiMemberList[0]));

            _mockController.SyncDoshiiMembersWithPosMembers();

            _mockController.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void GetRewardsForMember_Success()
        {
            var rewardsList = GenerateObjectsAndStringHelper.GenerateRewardList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000)).Return(rewardsList);

            _mockController.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetRewardsForMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockController.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);
        }

        [Test]
        public void GetRewardsForMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageRewardsList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockController.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetRewardsForMember_ManagerNotInitializedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            _mockController.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

        }

        [Test]
        public void ReddemRewardForMember_returnedOrderIsNull()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(null);

            var result = _mockController.RedeemRewardForMember(member, reward, order);

            _mockController.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ReddemRewardForMember_UpdateOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Throw(new Exception());

            var result = _mockController.RedeemRewardForMember(member, reward, order);

            _mockController.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void ReddemRewardForMember_RedeemRewardForOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemRewardForMember(member.Id, reward.Id, order)).Throw(new RestfulApiErrorResponseException());

            _mockController.RedeemRewardForMember(member, reward, order);
        }

        [Test]
        public void ReddemRewardForMember_Success()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemRewardForMember(member.Id, reward.Id, order)).Return(true);

            var result = _mockController.RedeemRewardForMember(member, reward, order);

            MockHttpComs.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void RedeemRewardsForMemberCancel_Success()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(true);

            var result = _mockController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

            Assert.AreEqual(true, result);
            MockHttpComs.VerifyAllExpectations();
            
        }
        
        [Test]
        public void RedeemRewardsForMemberCancel_Failed()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(false);

            var result = _mockController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemRewardsForMemberCancel_WithException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Throw(new RestfulApiErrorResponseException());

            _mockController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

        }

        [Test]
        public void RedeemRewardsForMemberCancel_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            
            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything,Arg<string>.Is.Anything,Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemRewardsForMemberCancel_ManagerNotInitializedException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(true);

            _mockController.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

        }

        [Test]
        public void RedeemRewardsForMemberConfirm_Success()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Return(true);

            var result = _mockController.RedeemRewardForMemberConfirm(memberId, rewardId);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemRewardsForMemberConfirm_WithException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Throw(new RestfulApiErrorResponseException());

            _mockController.RedeemRewardForMemberConfirm(memberId, rewardId);

        }

        [Test]
        public void RedeemRewardsForMemberConfirm_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockController.RedeemRewardForMemberConfirm(memberId, rewardId);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemRewardsForMemberConfirm_ManagerNotInitializedException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Return(true);

            _mockController.RedeemRewardForMemberConfirm(memberId, rewardId);

        }


        [Test]
        public void RedeemPointsForMemberCancel_Success()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(true);

            var result = _mockController.RedeemPointsForMemberCancel(member, cancelReason);

            Assert.AreEqual(true, result);
            MockHttpComs.VerifyAllExpectations();

        }

        [Test]
        public void RedeemPointsForMemberCancel_Failed()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(false);

            var result = _mockController.RedeemPointsForMemberCancel(member, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemPointsForMemberCancel_WithException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Throw(new RestfulApiErrorResponseException());

            _mockController.RedeemPointsForMemberCancel(member, cancelReason);

        }

        [Test]
        public void RedeemPointsForMemberCancel_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockController.RedeemPointsForMemberCancel(member, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemPointsForMemberCancel_ManagerNotInitializedException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(true);

            _mockController.RedeemPointsForMemberCancel(member, cancelReason);

        }

        [Test]
        public void RedeemPointsForMemberConfirm_Success()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberConfirm(member)).Return(true);

            var result = _mockController.RedeemPointsForMemberConfirm(member);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemPointsForMemberConfirm_WithException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemPointsForMemberConfirm(member)).Throw(new RestfulApiErrorResponseException());

            _mockController.RedeemPointsForMemberConfirm(member);

        }

        [Test]
        public void RedeemPointsForMemberConfirm_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockController.RedeemPointsForMemberConfirm(member);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemPointsForMemberConfirm_ManagerNotInitializedException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;
            _mockController._rewardManager = null;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberConfirm(member)).Return(true);

            _mockController.RedeemPointsForMemberConfirm(member);

        }

        //****************************
        [Test]
        public void ReddemPointsForMember_returnedOrderIsNull()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(null);

            var result = _mockController.RedeemPointsForMember(member, app, order, points);

            _mockController.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ReddemPointsForMember_UpdateOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Throw(new Exception());

            var result = _mockController.RedeemPointsForMember(member, app, order, points);

            _mockController.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        /*[Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void ReddemPointsForMember_RedeemPointsForOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;
            var pr = new PointsRedeem()
            {
                AppId = app.Id,
                OrderId = order.Id,
                Points = points
            };

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemPointsForMember(Arg<PointsRedeem>.Is.Anything, Arg<Member>.Is.Anything)).Return(true);

            _mockController.RedeemPointsForMember(member, app, order, points);
        }*/

        [Test]
        public void ReddemPointsForMember_Success()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;
            var pr = new PointsRedeem()
            {
                AppId = app.Id,
                OrderId = order.Id,
                Points = points
            };

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            _mockController.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemPointsForMember(Arg<PointsRedeem>.Is.Anything, Arg<Member>.Is.Anything)).Return(true);

            var result = _mockController.RedeemPointsForMember(member, app, order, points);

            MockHttpComs.VerifyAllExpectations();
            _mockController.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(OrderDoesNotExistOnPosException))]
        public void AddTableAllocaiton_OrderDoesNotExistOnPos()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new OrderDoesNotExistOnPosException());


            _mockController.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void AddTableAllocaiton_CheckinUpdatedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PostCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            MockHttpComs.Stub(x => x.PutCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockController.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }
        
        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void AddTableAllocaiton_OrderUpdatedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.PostCheckin(Arg<Checkin>.Is.Anything)).Return(new Checkin() {Id = GenerateObjectsAndStringHelper.TestCheckinId});
            MockHttpComs.Stub(x => x.PutCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockController.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void RemoveTableAllocaitonFromCheckin_RestfulApiErrorResponseException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockController.ModifyTableAllocation(checkinId, new List<string>(), GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void RemoveTableAllocaitonFromCheckin_Exception()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _controller);
            _mockController._httpComs = MockHttpComs;
            _mockController.IsInitalized = true;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new Exception());

            _mockController.ModifyTableAllocation(checkinId, new List<string>(), GenerateObjectsAndStringHelper.TestCovers);
        }
    }
}
