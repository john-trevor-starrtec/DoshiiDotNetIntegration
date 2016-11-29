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
            _controllers.ConfigurationManager = _configurationManager;
            _mockHttpController = MockRepository.GeneratePartialMock<HttpController>(GenerateObjectsAndStringHelper.TestBaseUrl, _controllers);

            _doshiiController = new DoshiiController(_configurationManager);
            _mockDoshiiController = MockRepository.GeneratePartialMock<DoshiiController>(_configurationManager);

            _doshiiController.Initialize(false);
            
            _doshiiController._controllers = _controllers;

            _checkinController = MockRepository.GeneratePartialMock<CheckinController>(_controllers, _mockHttpController);
            _consumerController = MockRepository.GeneratePartialMock<ConsumerController>(_controllers, _mockHttpController);
            _loggingController = MockRepository.GeneratePartialMock<LoggingController>(_controllers, _mockHttpController);
            _menuController = MockRepository.GeneratePartialMock<MenuController>(_controllers, _mockHttpController);
            _orderingController = MockRepository.GeneratePartialMock<OrderingController>(_controllers, _mockHttpController);
            _reservationController = MockRepository.GeneratePartialMock<ReservationController>(_controllers, _mockHttpController);
            _rewardController = MockRepository.GeneratePartialMock<RewardController>(_controllers, _mockHttpController);
            _tableController = MockRepository.GeneratePartialMock<TableController>(_controllers, _mockHttpController);
            _transactionController = MockRepository.GeneratePartialMock<TransactionController>(_controllers, _mockHttpController);
            
            _doshiiController._controllers.TransactionController = _transactionController;
            _doshiiController._controllers.OrderingController = _orderingController;
            _doshiiController._controllers.MenuController = _menuController;
            _doshiiController._controllers.TableController = _tableController;
            _doshiiController._controllers.CheckinController = _checkinController;
            _doshiiController._controllers.ConsumerController = _consumerController;
            _doshiiController._controllers.LoggingController = _loggingController;
            _doshiiController._controllers.ReservationController = _reservationController;
            _doshiiController._controllers.RewardController = _rewardController;
        }

        [Test]
        public void AcceptOrderAheadCreation_success()
        {
            _orderingController.Expect(x => x.AcceptOrderAheadCreation(GenerateObjectsAndStringHelper.GenerateOrderAccepted())).Return(true);


            _doshiiController.AcceptOrderAheadCreation(GenerateObjectsAndStringHelper.GenerateOrderAccepted());
            _mockDoshiiController.VerifyAllExpectations();
            
            /*_mockDoshiiController._logger.mLog.Expect(
                x =>
                    x.LogDoshiiMessage(typeof(DoshiiController), DoshiiLogLevels.Info,
                        String.Format("Doshii will use default timeout of {0}", DoshiiController.DefaultTimeout)));
            _mockDoshiiController.Expect(
                x =>
                    x.InitializeProcess(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<bool>.Is.Anything,
                        Arg<int>.Is.Equal(DoshiiController.DefaultTimeout))).Return(true);

            _mockDoshiiController.Initialize(locationToken, vendor, secretKey, urlBase, startWebSocketsConnection, 0);
            _mockDoshiiController.VerifyAllExpectations();*/
        }

    }
}
