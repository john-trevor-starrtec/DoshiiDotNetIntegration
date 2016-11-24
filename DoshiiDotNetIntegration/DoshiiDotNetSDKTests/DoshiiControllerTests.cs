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
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using Rhino.Mocks.Constraints;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class DoshiiControllerTests
    {
        ITransactionManager _paymentManager;
        IOrderingManager _orderingManager;
        IRewardManager _membershipManager;
        ILoggingManager _logger;
        IReservationManager _reservationManager;
        DoshiiDotNetIntegration.Controllers.LoggingController LogManager;
        DoshiiController _controller;
        CheckinController _checkinController;
        ConsumerController _consumerController;
        LoggingController _loggingController;
        MenuController _menuController;
        OrderingController _orderingController;
        ReservationController _reservationController;
        RewardController _rewardController;
        TableController _tableController;
        TransactionController _transactionController;

        [SetUp]
        public void Init()
        {
            _paymentManager = MockRepository.GenerateMock<ITransactionManager>();
            _orderingManager = MockRepository.GenerateMock<IOrderingManager>();
            _membershipManager = MockRepository.GenerateMock<IRewardManager>();
            _logger = MockRepository.GenerateMock<ILoggingManager>();
            _reservationManager = MockRepository.GenerateMock<IReservationManager>();


            LogManager = new LoggingController(_logger);
            _controller = new DoshiiController();
            _mockController = MockRepository.GeneratePartialMock<DoshiiController>(paymentManager, _Logger, orderingManager, membershipManager);

            locationToken = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            urlBase = "https://sandbox.doshii.co/pos/v3";
            startWebSocketsConnection = false;
            socketTimeOutSecs = 600;
        }

    }
}
