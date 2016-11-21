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
		IPaymentModuleManager paymentManager;
        IOrderingManager orderingManager;
        IMembershipModuleManager membershipManager;
        DoshiiDotNetIntegration.Interfaces.IDoshiiLogger _Logger;
        DoshiiDotNetIntegration.DoshiiLogManager LogManager;
        DoshiiManager _manager;
        string locationToken = "";
        string urlBase = "";
        bool startWebSocketsConnection;
        int socketTimeOutSecs = 600;
        DoshiiManager _mockManager;
        string vendor = "testVendor";
        string secretKey = "wer";
        string checkinId = "checkinId";
        
        
        [SetUp]
        public void Init()
        {
			paymentManager = MockRepository.GenerateMock<IPaymentModuleManager>();
            orderingManager = MockRepository.GenerateMock<IOrderingManager>();
            membershipManager = MockRepository.GenerateMock<IMembershipModuleManager>();
            _Logger = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.IDoshiiLogger>();
            LogManager = new DoshiiLogManager(_Logger);
            _manager = new DoshiiManager(paymentManager, _Logger, orderingManager, membershipManager);
            _mockManager = MockRepository.GeneratePartialMock<DoshiiManager>(paymentManager, _Logger, orderingManager, membershipManager);

            locationToken = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://sandbox.doshii.co/pos/v3";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

		[Test]
		public void BuildSocketUrl()
		{
            string expectedResult = String.Format("wss://sandbox-socket.doshii.co/pos/socket?token={0}", locationToken);
            string actualResult = _manager.BuildSocketUrl(urlBase, locationToken);
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContsuctNullPaymentManager()
		{
			var man = new DoshiiManager(null, _Logger, orderingManager, null);
		}

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullLoggerManager()
        {
            var man = new DoshiiManager(paymentManager, null, orderingManager, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContsuctNullOrderManager()
        {
            var man = new DoshiiManager(paymentManager, _Logger, null, null);
        }
        
        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoUrlBase()
        {
            _manager.Initialize(locationToken, vendor, secretKey, "", startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoVendor()
        {
            _manager.Initialize(locationToken, "", secretKey, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSecretKey()
        {
            _manager.Initialize(locationToken, vendor, "", urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            _manager.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, -1);
        }

        [Test]
		[ExpectedException(typeof(ArgumentException))]
        public void Initialze_NoToken()
        {
            _manager.Initialize("", vendor, secretKey, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        public void Initialze_StartSocketConnectIsTrue()
        {
            _mockManager.Expect((x => x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<int>.Is.Anything))).Return(true);
            _mockManager.Initialize(locationToken, vendor, secretKey, urlBase, true, socketTimeOutSecs);

            _mockManager.VerifyAllExpectations();
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

            _mockManager.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, 0);
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void Initialze_TokenGetsSetCorrectly()
        {

            _mockManager.Stub(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Equal(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken)), Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);

            _mockManager.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, 0);
            Assert.AreEqual(_mockManager.LocationToken, locationToken);
        }

        [Test]
        public void InitialzeProcess_HttpAndSocketsAreInitialized()
        {
            _mockManager.LocationToken = locationToken;
            _mockManager.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, true, 30);
            Assert.AreNotEqual(_mockManager.SocketComs, null);
            Assert.AreNotEqual(_mockManager.m_HttpComs, null);
        }

        [Test]
        public void InitialzeProcess_SocketsNotInitialized()
        {
            _mockManager.LocationToken = locationToken;
            _mockManager.SocketComs = null;
            _mockManager.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, false, 30);
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

            _mockManager.LocationToken = locationToken;
            _mockManager.InitializeProcess(GenerateObjectsAndStringHelper.BuildSocketUrl(urlBase, locationToken), urlBase, true, 30);

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetUnlinkedOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            _mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());
                
            _mockManager.RefreshAllOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RefreshAllOrders_CallGetUnlinkedOrder_throwsExceptionFromHttpGetTransactionFromDoshiiOrderId()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetUnlinkedOrders()).Return(orderList);
            MockHttpComs.Expect(x => x.GetTransactionsFromDoshiiOrderId(Arg<String>.Is.Anything)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RefreshAllOrders();
        }

        [Test]
        public void RefreshAllOrders_CallGetUnlinkedOrder()
        {
            IEnumerable<Order> orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutTransaction(transaction)).Throw(newException);
            
            bool success = _mockManager.RejectPaymentForOrder(transaction);
            Assert.AreEqual(success, false);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectPaymentForOrder_ExceptionOtherFromInterface()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrderFromDoshiiOrderId("1")).Throw(new RestfulApiErrorResponseException())
                .Return(GenerateObjectsAndStringHelper.GenerateOrderReadyToPay());

            _mockManager.GetOrderFromDoshiiOrderId("1");
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsTransactionNotFoundException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new OrderDoesNotExistOnPosException());

            _mockManager.RecordOrderCheckinId(order.Id, checkinId);
        }

        [Test]
        public void RecordCheckinForOrder_ThrowsException()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            orderingManager.Expect(x => x.RecordCheckinForOrder(order.Id, order.CheckinId))
                .Throw(new Exception());

            _mockManager.RecordOrderCheckinId(order.Id, checkinId);
        }


        

        [Test]
        public void HandleOrderCreated_withTransactions_WrongType()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            order.Type = "wrongType";
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList));
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
        public void GetConsumerForOrderCreated_GetConsumerSuccess()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            paymentManager.Expect(x => x.ReadyToPay(transaction)).Throw(new OrderDoesNotExistOnPosException());

            _mockManager.Expect(x => x.RejectPaymentForOrder(transaction)).Return(true);

            _mockManager.HandelPendingTransactionReceived(transaction);
            paymentManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }
        
        [Test]
        public void HandleDeliveryOrderCreated_withoutTransactions_PosReturnsNull()
        {
            List<Transaction> transactionList = new List<Transaction>();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewDeliveryOrder(order, consumer));
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void HandleDeliveryOrderCreated_withTransactions_Success()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GenerateDeliveryOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;


            orderingManager.Expect(x => x.ConfirmNewDeliveryOrderWithFullPayment(order, consumer, transactionList));
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            MockHttpComs.Expect(x => x.PutOrder(orderReturnedFromPos)).Return(orderReturnedFromPos);

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            orderingManager.Expect(x => x.ConfirmNewPickupOrderWithFullPayment(order, consumer, transactionList));
            _mockManager.Expect(x => x.GetConsumerForOrderCreated(order, transactionList)).Return(consumer);
            
            
            _mockManager.HandleOrderCreated(order, transactionList);
            orderingManager.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void AcceptOrderAheadCreation_withoutTransactions()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.LocationToken = locationToken;
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.GetOrderFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(order);
            _mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(new List<Transaction>());
            _mockManager.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos)).Return(orderReturnedFromPos);

            _mockManager.AcceptOrderAheadCreation(orderReturnedFromPos);
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void AcceptOrderAheadCreation_withTransactions()
        {
            List<Transaction> transactionList = GenerateObjectsAndStringHelper.GenerateTransactionList();
            Order order = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            Consumer consumer = GenerateObjectsAndStringHelper.GenerateConsumer();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.LocationToken = locationToken;
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.GetOrderFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(order);
            _mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(transactionList);
            _mockManager.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos)).Return(orderReturnedFromPos);
            _mockManager.Expect(x => x.RequestPaymentForOrderExistingTransaction(Arg<Transaction>.Is.Anything)).Return(true);
            
            _mockManager.AcceptOrderAheadCreation(orderReturnedFromPos);
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void RejectOrderAheadCreation_withTransactions()
        {
            Order orderReturnedFromPos = GenerateObjectsAndStringHelper.GeneratePickupOrderPending();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.LocationToken = locationToken;
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.GetTransactionFromDoshiiOrderId(orderReturnedFromPos.DoshiiId)).Return(GenerateObjectsAndStringHelper.GenerateTransactionList());
            _mockManager.Expect(x => x.PutOrderCreatedResult(orderReturnedFromPos));
            _mockManager.Expect(x => x.RejectPaymentForOrder(Arg<Transaction>.Is.Anything));
            
            _mockManager.RejectOrderAheadCreation(orderReturnedFromPos);
            _mockManager.VerifyAllExpectations();
        }

        [Test]
        public void RecordPosTransactionOnDoshii()
        {
            Transaction transaction = GenerateObjectsAndStringHelper.GenerateTransactionWaiting();
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.PostTransaction(transaction)).Return(transaction);

            _mockManager.RecordPosTransactionOnDoshii(transaction);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrderCreatedResult_Success()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutOrderCreatedResult(order)).Return(order);

            _mockManager.PutOrderCreatedResult(order);
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void PutOrderCreatedResult_SuccessfulOrderNoPosId()
        {
            Order order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            order.Status = "accepted";
            order.Id = null;
            
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.PutOrderCreatedResult(order);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Return(returnedOrder);

            _mockManager.PutOrderCreatedResult(order);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new RestfulApiErrorResponseException());

            _mockManager.PutOrderCreatedResult(order);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new RestfulApiErrorResponseException(){StatusCode = HttpStatusCode.Conflict});

            _mockManager.PutOrderCreatedResult(order);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new NullResponseDataReturnedException());

            _mockManager.PutOrderCreatedResult(order);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _mockManager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PutOrderCreatedResult(order)).Return(order).Throw(new Exception());

            _mockManager.PutOrderCreatedResult(order);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrder_CallGetOrder()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetOrder(GenerateObjectsAndStringHelper.TestOrderId);

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_CallGetOrders()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetOrders())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetOrders();

        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transaction()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetTransaction(GenerateObjectsAndStringHelper.TestTransactionId);

        }
        
        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_Transactions()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetTransactions())
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetTransactions();

        }

        [Test]
        public void UpdateOrder_Calls_PutOrder()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            
            MockHttpComs.Expect(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Return(orderToUpdate);

            Order returnedOrder = _mockManager.UpdateOrder(orderToUpdate);
            Assert.AreEqual(orderToUpdate, returnedOrder);
            
            _mockManager.VerifyAllExpectations();

        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_Conflict()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new NullResponseDataReturnedException());

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void UpdateOrder_PutOrder_GeneralException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.PutOrder(Arg<Order>.Matches(o => o.Id == orderToUpdate.Id && o.Status == orderToUpdate.Status)))
                .Throw(new Exception());

            _mockManager.UpdateOrder(orderToUpdate);
        }

        [Test]
        public void GetMember_Success()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetMember(GenerateObjectsAndStringHelper.TestMemberId));
            
            _mockManager.GetMember(GenerateObjectsAndStringHelper.TestMemberId);
        
            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMember_Failed_WithException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

        }

        [Test]
        public void GetMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            
            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockManager.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            _mockManager.GetMember(GenerateObjectsAndStringHelper.TestMemberId);

        }


        [Test]
        public void GetMembers_Success()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetMembers());

            _mockManager.GetMembers();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMembers_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetMembers();

        }

        [Test]
        public void GetMembers_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMembersList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);
            MockHttpComs.Stub(x => x.GetMember(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(GenerateObjectsAndStringHelper.GenerateMember1());
            MockHttpComs.Stub(x => x.GetMember(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(GenerateObjectsAndStringHelper.GenerateMember2());

            _mockManager.GetMembers();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMembers_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            _mockManager.GetMembers();

        }

        [Test]
        public void DeleteMember_Success()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1())).Return(true);

            _mockManager.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void DeleteMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void DeleteMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockManager.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void DeleteMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            _mockManager.DeleteMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void CreateMember_Success()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            member.Id = "";
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);
            MockHttpComs.Expect(x => x.PostMember(member)).Return(GenerateObjectsAndStringHelper.GenerateMember1());
            
            _mockManager.UpdateMember(member);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void UpdateMember_Success()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.PutMember(GenerateObjectsAndStringHelper.GenerateMember1()));

            _mockManager.UpdateMember(member);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void UpdateMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }

        [Test]
        public void UpdateMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockManager.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

            MockHttpComs.VerifyAllExpectations();
        }



        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void UpdateMember_ManagerNotInitializedException()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            _mockManager.UpdateMember(GenerateObjectsAndStringHelper.GenerateMember1());

        }


        [Test]
        public void SyncMembersWithDoshii_DeleteMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            doshiiMemberList.RemoveAt(0);

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.mMemberManager = membershipManager;
            _mockManager.IsInitalized = true;
            
            _mockManager.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.DeleteMemberOnPos(GenerateObjectsAndStringHelper.GenerateMember1()));
            
            _mockManager.SyncDoshiiMembersWithPosMembers();

            _mockManager.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void SyncMembersWithDoshii_CreateMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            posMemberList.RemoveAt(0);

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.mMemberManager = membershipManager;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.CreateMemberOnPos(GenerateObjectsAndStringHelper.GenerateMember1()));

            _mockManager.SyncDoshiiMembersWithPosMembers();

            _mockManager.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void SyncMembersWithDoshii_UpdateMemberOnPos()
        {
            var posMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();
            var doshiiMemberList = GenerateObjectsAndStringHelper.GenerateMemberList();

            doshiiMemberList[0].Email = "newemail@email.com";

            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageMember();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.mMemberManager = membershipManager;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.GetMembers()).Return(doshiiMemberList);
            membershipManager.Expect(x => x.GetMembersFromPos()).Return(posMemberList);
            membershipManager.Expect(x => x.UpdateMemberOnPos(doshiiMemberList[0]));

            _mockManager.SyncDoshiiMembersWithPosMembers();

            _mockManager.VerifyAllExpectations();
            membershipManager.VerifyAllExpectations();
        }

        [Test]
        public void GetRewardsForMember_Success()
        {
            var rewardsList = GenerateObjectsAndStringHelper.GenerateRewardList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000)).Return(rewardsList);

            _mockManager.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetRewardsForMember_Failed_WithException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new RestfulApiErrorResponseException());

            _mockManager.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);
        }

        [Test]
        public void GetRewardsForMember_Success_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageRewardsList();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(
                x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(responseMessage);

            _mockManager.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetRewardsForMember_ManagerNotInitializedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            _mockManager.GetRewardsForMember(GenerateObjectsAndStringHelper.GenerateMember1().Id, GenerateObjectsAndStringHelper.TestOrderId, 1000);

        }

        [Test]
        public void ReddemRewardForMember_returnedOrderIsNull()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(null);

            var result = _mockManager.RedeemRewardForMember(member, reward, order);

            _mockManager.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ReddemRewardForMember_UpdateOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Throw(new Exception());

            var result = _mockManager.RedeemRewardForMember(member, reward, order);

            _mockManager.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void ReddemRewardForMember_RedeemRewardForOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemRewardForMember(member.Id, reward.Id, order)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RedeemRewardForMember(member, reward, order);
        }

        [Test]
        public void ReddemRewardForMember_Success()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var reward = GenerateObjectsAndStringHelper.GenerateRewardAbsolute();

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemRewardForMember(member.Id, reward.Id, order)).Return(true);

            var result = _mockManager.RedeemRewardForMember(member, reward, order);

            MockHttpComs.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void RedeemRewardsForMemberCancel_Success()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(true);

            var result = _mockManager.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

            Assert.AreEqual(true, result);
            MockHttpComs.VerifyAllExpectations();
            
        }
        
        [Test]
        public void RedeemRewardsForMemberCancel_Failed()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(false);

            var result = _mockManager.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

        }

        [Test]
        public void RedeemRewardsForMemberCancel_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            
            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything,Arg<string>.Is.Anything,Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockManager.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

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
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason)).Return(true);

            _mockManager.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);

        }

        [Test]
        public void RedeemRewardsForMemberConfirm_Success()
        {

            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Return(true);

            var result = _mockManager.RedeemRewardForMemberConfirm(memberId, rewardId);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemRewardsForMemberConfirm_WithException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RedeemRewardForMemberConfirm(memberId, rewardId);

        }

        [Test]
        public void RedeemRewardsForMemberConfirm_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockManager.RedeemRewardForMemberConfirm(memberId, rewardId);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemRewardsForMemberConfirm_ManagerNotInitializedException()
        {
            var memberId = GenerateObjectsAndStringHelper.TestMemberId;
            var rewardId = GenerateObjectsAndStringHelper.TestRewardId;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            MockHttpComs.Expect(x => x.RedeemRewardForMemberConfirm(memberId, rewardId)).Return(true);

            _mockManager.RedeemRewardForMemberConfirm(memberId, rewardId);

        }


        [Test]
        public void RedeemPointsForMemberCancel_Success()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(true);

            var result = _mockManager.RedeemPointsForMemberCancel(member, cancelReason);

            Assert.AreEqual(true, result);
            MockHttpComs.VerifyAllExpectations();

        }

        [Test]
        public void RedeemPointsForMemberCancel_Failed()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(false);

            var result = _mockManager.RedeemPointsForMemberCancel(member, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemPointsForMemberCancel_WithException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RedeemPointsForMemberCancel(member, cancelReason);

        }

        [Test]
        public void RedeemPointsForMemberCancel_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockManager.RedeemPointsForMemberCancel(member, cancelReason);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemPointsForMemberCancel_ManagerNotInitializedException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var cancelReason = GenerateObjectsAndStringHelper.TestCancelResaon;
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberCancel(member, cancelReason)).Return(true);

            _mockManager.RedeemPointsForMemberCancel(member, cancelReason);

        }

        [Test]
        public void RedeemPointsForMemberConfirm_Success()
        {

            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberConfirm(member)).Return(true);

            var result = _mockManager.RedeemPointsForMemberConfirm(member);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void RedeemPointsForMemberConfirm_WithException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.RedeemPointsForMemberConfirm(member)).Throw(new RestfulApiErrorResponseException());

            _mockManager.RedeemPointsForMemberConfirm(member);

        }

        [Test]
        public void RedeemPointsForMemberConfirm_MakesRequest()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponseMessageSuccess();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Expect(x => x.MakeRequest(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(responseMessage);

            var result = _mockManager.RedeemPointsForMemberConfirm(member);

            MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void RedeemPointsForMemberConfirm_ManagerNotInitializedException()
        {
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;
            _mockManager.mMemberManager = null;

            MockHttpComs.Expect(x => x.RedeemPointsForMemberConfirm(member)).Return(true);

            _mockManager.RedeemPointsForMemberConfirm(member);

        }

        //****************************
        [Test]
        public void ReddemPointsForMember_returnedOrderIsNull()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(null);

            var result = _mockManager.RedeemPointsForMember(member, app, order, points);

            _mockManager.VerifyAllExpectations();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ReddemPointsForMember_UpdateOrderThrowsException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var member = GenerateObjectsAndStringHelper.GenerateMember1();
            var app = GenerateObjectsAndStringHelper.GenerateApp1();
            var points = GenerateObjectsAndStringHelper.TestMemberPoints;

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Throw(new Exception());

            var result = _mockManager.RedeemPointsForMember(member, app, order, points);

            _mockManager.VerifyAllExpectations();
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemPointsForMember(Arg<PointsRedeem>.Is.Anything, Arg<Member>.Is.Anything)).Return(true);

            _mockManager.RedeemPointsForMember(member, app, order, points);
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

            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            _mockManager.Expect(x => x.UpdateOrder(order)).Return(order);
            MockHttpComs.Expect(x => x.RedeemPointsForMember(Arg<PointsRedeem>.Is.Anything, Arg<Member>.Is.Anything)).Return(true);

            var result = _mockManager.RedeemPointsForMember(member, app, order, points);

            MockHttpComs.VerifyAllExpectations();
            _mockManager.VerifyAllExpectations();
            Assert.AreEqual(true, result);
        }

        [Test]
        [ExpectedException(typeof(OrderDoesNotExistOnPosException))]
        public void AddTableAllocaiton_OrderDoesNotExistOnPos()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new OrderDoesNotExistOnPosException());


            _mockManager.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void AddTableAllocaiton_CheckinUpdatedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PostCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            MockHttpComs.Stub(x => x.PutCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockManager.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }
        
        [Test]
        [ExpectedException(typeof(OrderUpdateException))]
        public void AddTableAllocaiton_OrderUpdatedException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.PostCheckin(Arg<Checkin>.Is.Anything)).Return(new Checkin() {Id = GenerateObjectsAndStringHelper.TestCheckinId});
            MockHttpComs.Stub(x => x.PutCheckin(Arg<Checkin>.Is.Anything)).Throw(new CheckinUpdateException());
            orderingManager.Stub(x => x.RetrieveOrder(GenerateObjectsAndStringHelper.TestOrderId))
                .Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            _mockManager.SetTableAllocationWithoutCheckin(GenerateObjectsAndStringHelper.TestOrderId, new List<string> { GenerateObjectsAndStringHelper.GenerateTableAllocation().Name }, GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void RemoveTableAllocaitonFromCheckin_RestfulApiErrorResponseException()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new RestfulApiErrorResponseException(HttpStatusCode.BadRequest));

            _mockManager.ModifyTableAllocation(checkinId, new List<string>(), GenerateObjectsAndStringHelper.TestCovers);
        }

        [Test]
        [ExpectedException(typeof(CheckinUpdateException))]
        public void RemoveTableAllocaitonFromCheckin_Exception()
        {
            var MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, LogManager, _manager);
            _mockManager.m_HttpComs = MockHttpComs;
            _mockManager.IsInitalized = true;

            MockHttpComs.Stub(x => x.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestOrderId))
                .Throw(new Exception());

            _mockManager.ModifyTableAllocation(checkinId, new List<string>(), GenerateObjectsAndStringHelper.TestCovers);
        }
    }
}
