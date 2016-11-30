using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Controllers;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using Rhino.Mocks.Constraints;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class DoshiiControllerTests
    {
        private IConfigurationManager _configurationManager;
        ITransactionManager _paymentManager;
        IOrderingManager _orderingManager;
        IRewardManager _membershipManager;
        ILoggingManager _logger;
        IReservationManager _reservationManager;
        DoshiiDotNetIntegration.Controllers.LoggingController LogManager;
        DoshiiController _doshiiController;
        CheckinController _checkinController;
        ConsumerController _consumerController;
        LoggingController _loggingController;
        MenuController _menuController;
        OrderingController _orderingController;
        ReservationController _reservationController;
        RewardController _rewardController;
        TableController _tableController;
        TransactionController _transactionController;
        DoshiiController _mockDoshiiController;
        Controllers _controllers;
        HttpController _mockHttpController;

        [SetUp]
        public void Init()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _paymentManager = MockRepository.GenerateMock<ITransactionManager>();
            _orderingManager = MockRepository.GenerateMock<IOrderingManager>();
            _membershipManager = MockRepository.GenerateMock<IRewardManager>();
            _logger = MockRepository.GenerateMock<ILoggingManager>();
            _reservationManager = MockRepository.GenerateMock<IReservationManager>();
            LogManager = new LoggingController(_logger);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return(GenerateObjectsAndStringHelper.TestBaseUrl);
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return(GenerateObjectsAndStringHelper.TestToken);
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSecretKey);
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(600);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return(GenerateObjectsAndStringHelper.TestVendor);


            _controllers = new Controllers();

            

            _loggingController = MockRepository.GeneratePartialMock<LoggingController>(_logger);
            _controllers.LoggingController = _loggingController;
            _controllers.OrderingManager = _orderingManager;
            _controllers.TransactionManager = _paymentManager;
            _controllers.RewardManager = _membershipManager;
            _controllers.ReservationManager = _reservationManager;
                
            
            _controllers.ConfigurationManager = _configurationManager;
            _mockHttpController = MockRepository.GeneratePartialMock<HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, _controllers);

            _doshiiController = new DoshiiController(_configurationManager);
            //_mockDoshiiController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            _doshiiController.Initialize(false);
            
            _doshiiController._controllers = _controllers;

            _checkinController = MockRepository.GeneratePartialMock<CheckinController>(_controllers, _mockHttpController);
            _consumerController = MockRepository.GeneratePartialMock<ConsumerController>(_controllers, _mockHttpController);
            
            _menuController = MockRepository.GeneratePartialMock<MenuController>(_controllers, _mockHttpController);
            _transactionController = MockRepository.GeneratePartialMock<TransactionController>(_controllers, _mockHttpController);
            _controllers.TransactionController = _transactionController;
            _orderingController = MockRepository.GeneratePartialMock<OrderingController>(_controllers, _mockHttpController);
            _controllers.OrderingController = _orderingController;
            _reservationController = MockRepository.GeneratePartialMock<ReservationController>(_controllers, _mockHttpController);
            _rewardController = MockRepository.GeneratePartialMock<RewardController>(_controllers, _mockHttpController);
            _tableController = MockRepository.GeneratePartialMock<TableController>(_controllers, _mockHttpController);
            
            
            _doshiiController._controllers.OrderingController = _orderingController;
            _doshiiController._controllers.MenuController = _menuController;
            _doshiiController._controllers.TableController = _tableController;
            _doshiiController._controllers.CheckinController = _checkinController;
            _doshiiController._controllers.ConsumerController = _consumerController;
            
            _doshiiController._controllers.ReservationController = _reservationController;
            _doshiiController._controllers.RewardController = _rewardController;
        }


        #region initialization

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConfigurationManager_throws_ArgumentNullException()
        {
            var testController = new DoshiiController(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PosReturnsNullLoggingManager_throws_ArgumentNullException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(null);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            var testController = new DoshiiController(_configurationManager);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PosReturnsNullTransactionManager_throws_ArgumentNullException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(null);

            var testController = new DoshiiController(_configurationManager);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PosReturnsNullOrderingManager_throws_ArgumentNullException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(null);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            var testController = new DoshiiController(_configurationManager);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InitiliziseProcess_urlBaseEmpty_throws_ArgumentException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return("");
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return(GenerateObjectsAndStringHelper.TestToken);
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSecretKey);
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(600);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return(GenerateObjectsAndStringHelper.TestVendor);

            var testController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            testController.Stub(
                x =>
                    x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);
            testController.Initialize(false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InitiliziseProcess_LocastionTokenEmpty_throws_ArgumentException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return(GenerateObjectsAndStringHelper.TestBaseUrl);
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return("");
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSecretKey);
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(600);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return(GenerateObjectsAndStringHelper.TestVendor);

            var testController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            testController.Stub(
                x =>
                    x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);
            testController.Initialize(false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InitiliziseProcess_VendorEmpty_throws_ArgumentException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return(GenerateObjectsAndStringHelper.TestBaseUrl);
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return("");
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSecretKey);
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(600);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return("");

            var testController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            testController.Stub(
                x =>
                    x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);
            testController.Initialize(false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InitiliziseProcess_SecretKeyEmpty_throws_ArgumentException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return(GenerateObjectsAndStringHelper.TestBaseUrl);
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return("");
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return("");
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(600);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return(GenerateObjectsAndStringHelper.TestVendor);

            var testController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            testController.Stub(
                x =>
                    x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);
            testController.Initialize(false);
        }


        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InitiliziseProcess_TimeoutLessThan0_throws_ArgumentException()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);

            _configurationManager.Stub(x => x.GetBaseUrlFromPos()).Return(GenerateObjectsAndStringHelper.TestBaseUrl);
            _configurationManager.Stub(x => x.GetLocationTokenFromPos())
                .Return("");
            _configurationManager.Stub(x => x.GetLoggingManagerFromPos()).Return(_logger);
            _configurationManager.Stub(x => x.GetOrderingManagerFromPos()).Return(_orderingManager);
            _configurationManager.Stub(x => x.GetReservationManagerFromPos()).Return(_reservationManager);
            _configurationManager.Stub(x => x.GetRewardManagerFromPos()).Return(_membershipManager);
            _configurationManager.Stub(x => x.GetSecretKeyFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSecretKey);
            _configurationManager.Stub(x => x.GetSocketTimeOutFromPos()).Return(-1);
            _configurationManager.Stub(x => x.GetSocketUrlFromPos())
                .Return(GenerateObjectsAndStringHelper.TestSocketUrl);
            _configurationManager.Stub(x => x.GetTransactionManagerFromPos()).Return(_paymentManager);
            _configurationManager.Stub(x => x.GetVendorFromPos()).Return(GenerateObjectsAndStringHelper.TestVendor);

            var testController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);
            testController.Stub(
                x =>
                    x.InitializeProcess(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Anything)).Return(true);
            testController.Initialize(false);
        }

        #endregion


        #region ordering and transactions
        [Test]
        public void AcceptOrderAheadCreation_success()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.AcceptOrderAheadCreation(orderToSend)).Return(true);

            Boolean result = _doshiiController.AcceptOrderAheadCreation(orderToSend);
            _orderingController.VerifyAllExpectations();
            Assert.AreEqual(true, result);

        }

        [Test]
        public void AcceptOrderAheadCreation_failed()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.AcceptOrderAheadCreation(orderToSend)).Return(false);

            Boolean result = _doshiiController.AcceptOrderAheadCreation(orderToSend);
            _orderingController.VerifyAllExpectations();
            Assert.AreEqual(false, result);

        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void AcceptOrderAheadCreation_initilizationFailed_exceptionThrown()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _doshiiController.IsInitalized = false;

            _doshiiController.AcceptOrderAheadCreation(orderToSend);
        }

        [Test]
        public void RejectOrderAheadCreation_success()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.RejectOrderAheadCreation(orderToSend));

            _doshiiController.RejectOrderAheadCreation(orderToSend);
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        public void RejectOrderAheadCreation_failed()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.RejectOrderAheadCreation(orderToSend));

            _doshiiController.RejectOrderAheadCreation(orderToSend);
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void RejectOrderAheadCreation_initilizationFailed_exceptionThrown()
        {
            Order orderToSend = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _doshiiController.IsInitalized = false;

            _doshiiController.RejectOrderAheadCreation(orderToSend);
        }


        [Test]
        public void RecordPosTransactionOnDoshii_success()
        {
            Transaction transactionToSend = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            _transactionController.Expect(x => x.RecordPosTransactionOnDoshii(transactionToSend));

            _doshiiController.RecordPosTransactionOnDoshii(transactionToSend);
            _transactionController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void RecordPosTransactionOnDoshii_initilizationFailed_exceptionThrown()
        {
            Transaction transactionToSend = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            _doshiiController.IsInitalized = false;

            _doshiiController.RecordPosTransactionOnDoshii(transactionToSend);
        }

        [Test]
        public void GetOrder_success()
        {
            _orderingController.Expect(x => x.GetOrder("12")).Return(GenerateObjectsAndStringHelper.GeneratePickupOrderPending());

            _doshiiController.GetOrder("12");
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetOrder_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetOrder("12");
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrder_throwsException()
        {
            _orderingController.Expect(x => x.GetOrder("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetOrder("12");
        }

        [Test]
        public void GetConsumerFromCheckinId_success()
        {
            Transaction transactionToSend = GenerateObjectsAndStringHelper.GenerateTransactionComplete();
            _consumerController.Expect(x => x.GetConsumerFromCheckinId("12")).Return(GenerateObjectsAndStringHelper.GenerateConsumer());

            _doshiiController.GetConsumerFromCheckinId("12");
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetConsumerFromCheckinId_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetConsumerFromCheckinId("12");
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetConsumerFromCheckinId_throwsException()
        {
            _consumerController.Expect(x => x.GetConsumerFromCheckinId("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetConsumerFromCheckinId("12");
        }

        [Test]
        public void GetOrders_success()
        {
            _orderingController.Expect(x => x.GetOrders()).Return(new List<Order>());

            _doshiiController.GetOrders();
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetOrders_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetOrders();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetOrders_throwsException()
        {
            _orderingController.Expect(x => x.GetOrders()).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetOrder("12");
        }

        [Test]
        public void GetTransaction_success()
        {
            _transactionController.Expect(x => x.GetTransaction("12")).Return(GenerateObjectsAndStringHelper.GenerateTransactionPending());

            _doshiiController.GetTransaction("12");
            _transactionController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetTransaction_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetTransaction("12");
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetTransaction_throwsException()
        {
            _transactionController.Expect(x => x.GetTransaction("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetTransaction("12");
        }

        [Test]
        public void GetTransactionFromDoshiiOrderId_success()
        {
            _transactionController.Expect(x => x.GetTransactionFromDoshiiOrderId("12")).Return(new List<Transaction>());

            _doshiiController.GetTransactionFromDoshiiOrderId("12");
            _transactionController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetTransactionFromDoshiiOrderId_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetTransactionFromDoshiiOrderId("12");
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetTransactionFromDoshiiOrderId_throwsException()
        {
            _transactionController.Expect(x => x.GetTransactionFromDoshiiOrderId("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetTransactionFromDoshiiOrderId("12");
        }

        [Test]
        public void GetTransactions_success()
        {
            _transactionController.Expect(x => x.GetTransactions()).Return(new List<Transaction>());

            _doshiiController.GetTransactions();
            _transactionController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetTransactions_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetTransactions();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetTransactions_throwsException()
        {
            _transactionController.Expect(x => x.GetTransactions()).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetTransactions();
        }

        [Test]
        public void UpdateOrder_success()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.UpdateOrder(orderToUpdate)).Return(orderToUpdate);

            _doshiiController.UpdateOrder(orderToUpdate);
            _orderingController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void UpdateOrder_initilizationFailed_exceptionThrown()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _doshiiController.IsInitalized = false;

            _doshiiController.UpdateOrder(orderToUpdate);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void UpdateOrder_throwsException()
        {
            Order orderToUpdate = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            _orderingController.Expect(x => x.UpdateOrder(orderToUpdate)).Throw(new RestfulApiErrorResponseException());

            _doshiiController.UpdateOrder(orderToUpdate);
        }


            #endregion

        #region membership

        [Test]
        public void GetMember_success()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.GetMember("12")).Return(memberToUpdate);

            _doshiiController.GetMember("12");
            _rewardController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetMember_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetMember("12");
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMember_RewardInitilizationFailed_exceptionThrown()
        {
            _doshiiController._controllers.RewardManager = null;

            _doshiiController.GetMember("12");
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMember_throwsException()
        {
            _rewardController.Expect(x => x.GetMember("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetMember("12");
        }

        [Test]
        public void GetMembers_success()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.GetMembers()).Return(new List<Member>());

            _doshiiController.GetMembers();
            _rewardController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void GetMembers_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.GetMembers();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void GetMembers_RewardInitilizationFailed_exceptionThrown()
        {
            _doshiiController._controllers.RewardManager = null;

            _doshiiController.GetMembers();
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void GetMembers_throwsException()
        {
            _rewardController.Expect(x => x.GetMembers()).Throw(new RestfulApiErrorResponseException());

            _doshiiController.GetMembers();
        }

        [Test]
        public void DeleteMember_success()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.DeleteMember(memberToUpdate)).Return(true);

            _doshiiController.DeleteMember(memberToUpdate);
            _rewardController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void DeleteMember_initilizationFailed_exceptionThrown()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _doshiiController.IsInitalized = false;

            _doshiiController.DeleteMember(memberToUpdate);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void DeleteMember_RewardInitilizationFailed_exceptionThrown()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _doshiiController._controllers.RewardManager = null;

            _doshiiController.DeleteMember(memberToUpdate);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void DeleteMember_throwsException()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.GetMember("12")).Throw(new RestfulApiErrorResponseException());

            _doshiiController.DeleteMember(memberToUpdate);
        }

        [Test]
        public void UpdateMember_success()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.UpdateMember(memberToUpdate)).Return(memberToUpdate);

            _doshiiController.UpdateMember(memberToUpdate);
            _rewardController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void UpdateMember_initilizationFailed_exceptionThrown()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _doshiiController.IsInitalized = false;

            _doshiiController.UpdateMember(memberToUpdate);
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void UpdateMember_RewardInitilizationFailed_exceptionThrown()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _doshiiController._controllers.RewardManager = null;

            _doshiiController.UpdateMember(memberToUpdate);
        }

        [Test]
        [ExpectedException(typeof(RestfulApiErrorResponseException))]
        public void UpdateMember_throwsException()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.UpdateMember(memberToUpdate)).Throw(new RestfulApiErrorResponseException());

            _doshiiController.UpdateMember(memberToUpdate);
        }

        [Test]
        [ExpectedException(typeof(MemberIncompleteException))]
        public void UpdateMember_NotComplete_throwsException()
        {
            Member memberToUpdate = GenerateObjectsAndStringHelper.GenerateMember1();
            _rewardController.Expect(x => x.UpdateMember(memberToUpdate)).Throw(new MemberIncompleteException());

            _doshiiController.UpdateMember(memberToUpdate);
        }

        [Test]
        public void SyncDoshiiWithPosMembers_success()
        {
            _rewardController.Expect(x => x.SyncDoshiiMembersWithPosMembers()).Return(true);

            _doshiiController.SyncDoshiiMembersWithPosMembers();
            _rewardController.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiManagerNotInitializedException))]
        public void SyncDoshiiWithPosMembers_initilizationFailed_exceptionThrown()
        {
            _doshiiController.IsInitalized = false;

            _doshiiController.SyncDoshiiMembersWithPosMembers();
        }

        [Test]
        [ExpectedException(typeof(DoshiiMembershipManagerNotInitializedException))]
        public void SyncDoshiiWithPosMembers_RewardInitilizationFailed_exceptionThrown()
        {
            _doshiiController._controllers.RewardManager = null;

            _doshiiController.SyncDoshiiMembersWithPosMembers();
        }

        
#endregion

    }
}
