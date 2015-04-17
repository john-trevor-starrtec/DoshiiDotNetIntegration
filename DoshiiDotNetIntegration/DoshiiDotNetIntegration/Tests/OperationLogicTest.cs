using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace DoshiiDotNetIntegration.Tests
{
    [TestFixture]
    public class OperationLogicTest
    {

        DoshiiOperationLogic operationLogic;
        Interfaces.iDoshiiOrdering orderingInterface;
        string socketUrl = "";
        string token = "";
        Enums.OrderModes orderMode;
        Enums.SeatingModes seatingMode;
        string urlBase = "";
        bool startWebSocketsConnection;
        bool removeTableAllocationsAfterPayment;
        int socketTimeOutSecs;
        CommunicationLogic.DoshiiHttpCommunication m_httpComs;


        [SetUp]
        public void Init()
        {
            orderingInterface = MockRepository.GenerateMock<Interfaces.iDoshiiOrdering>();
            operationLogic = new DoshiiOperationLogic(orderingInterface);
            

            socketUrl = "wss://alpha.corp.doshii.co/pos/api/v1/socket";
            token = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            orderMode = Enums.OrderModes.RestaurantMode;
            seatingMode = Enums.SeatingModes.DoshiiAllocation;
            urlBase = "https://alpha.corp.doshii.co/pos/api/v1";
            startWebSocketsConnection = false;
            removeTableAllocationsAfterPayment = false;
            socketTimeOutSecs = 600;

        }
        
        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoSocketUrl()
        {
            operationLogic.Initialize("",token,orderMode, seatingMode, urlBase, startWebSocketsConnection, removeTableAllocationsAfterPayment, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoUrlBase()
        {
            operationLogic.Initialize(socketUrl, token, orderMode, seatingMode, "", startWebSocketsConnection, removeTableAllocationsAfterPayment, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            operationLogic.Initialize(socketUrl, token, orderMode, seatingMode, urlBase, startWebSocketsConnection, removeTableAllocationsAfterPayment, 0);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoToken()
        {
            operationLogic.Initialize(socketUrl, "", orderMode, seatingMode, urlBase, startWebSocketsConnection, removeTableAllocationsAfterPayment, socketTimeOutSecs);
        }

        [Test]
        public void createTableAllocation_CorrectData()
        {
            //Arrange
            var mocker = new MockRepository();
            m_httpComs = mocker.CreateMock<CommunicationLogic.DoshiiHttpCommunication>("xyz", operationLogic, "test");
            m_httpComs.Stub(x => x.MakeRequest("test", "test", "")).Return(new CommunicationLogic.DoshiHttpResponceMessages { Status = System.Net.HttpStatusCode.OK, Data = "", ErrorMessage = "", Message = "", StatusDescription = "ok"});

            //m_httpComs.Expect(x => x.PostTableAllocation("test", "test")).IgnoreArguments().Return(true);
            m_httpComs.Expect(x => x.MakeRequest("test", "test", "")).Return(new CommunicationLogic.DoshiHttpResponceMessages { Status = System.Net.HttpStatusCode.OK, Data = "", ErrorMessage = "", Message = "", StatusDescription = "ok" });

            operationLogic.m_HttpComs = m_httpComs;
            //Act
            mocker.ReplayAll();
            operationLogic.CreateTableAllocation("TestCustomerId", "tableNo");
            
            //Assert
            IList<object[]> argumentsSentPostTableAllocation = m_httpComs.GetArgumentsForCallsMadeOn(x => x.PostTableAllocation("test", "test"));
            var call1Arg1 = (string)argumentsSentPostTableAllocation[0][0];
            var call1Arg2 = (string)argumentsSentPostTableAllocation[0][1];


            Assert.AreEqual("CustomerId", call1Arg1);
            Assert.AreEqual("tableNo", call1Arg2);
        }


    }
}
