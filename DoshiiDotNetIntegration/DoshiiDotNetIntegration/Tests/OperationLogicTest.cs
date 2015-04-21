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
        int socketTimeOutSecs = 600;
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
            operationLogic.m_HttpComs = MockRepository.GenerateMock<CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
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
        public void SetTableAllocation_ShouldCallPostTableAllocation_withSameParams()
        {
            operationLogic.m_HttpComs.Expect(x => x.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableNumber)).Return(true);

            operationLogic.SetTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableNumber);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }
        
        [Test]
        public void findCurrentConsumer_ConsumerExistsInList()
        {
            List<Models.Consumer> list = new List<Models.Consumer>();
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer1());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer2());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer3());

            bool exists = operationLogic.findCurrentConsumer(list, GenerateObjectsAndStringHelper.GenerateConsumer3());
            Assert.IsTrue(exists);
        }

        [Test]
        public void findCurrentConsumer_ConsumerIsNotInList()
        {
            List<Models.Consumer> list = new List<Models.Consumer>();
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer1());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer2());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer3());

            bool exists = operationLogic.findCurrentConsumer(list, GenerateObjectsAndStringHelper.GenerateConsumer4());
            Assert.IsFalse(exists);
        }

        [Test]
        public void SocketComsCheckOutEventHandler_shouldCallCheckoutConsumer_withConsumerCheckinId()
        {
            orderingInterface.Expect(x => x.CheckOutConsumer(GenerateObjectsAndStringHelper.TestCustomerId));
            operationLogic.SocketComsCheckOutEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs() { ConsumerId = GenerateObjectsAndStringHelper.TestCustomerId });
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsConnectionEventHandler_shouldCallRefreshConsumerData()
        {
            var mockOperationLogic = MockRepository.GeneratePartialMock<DoshiiOperationLogic>(orderingInterface);
            mockOperationLogic.Expect(x => x.RefreshConsumerData()).IgnoreArguments();

            mockOperationLogic.SocketComsConnectionEventHandler(new Object(), new EventArgs());

            mockOperationLogic.VerifyAllExpectations();
        }

        [Test]
        public void ScoketComsTimeOutValueReached_ShouldCallDissociateDoshiiChecks()
        {
            orderingInterface.Expect(x => x.DissociateDoshiiChecks());
            operationLogic.ScoketComsTimeOutValueReached(new object(), new EventArgs());
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsTableAllocationEventHandler_ShouldCallConfirmTableAllocation_ThenPutTableAllocation()
        {
            var tableAllocationEventArgs = GenerateObjectsAndStringHelper.GenerateTableAllocationEventArgs();
            orderingInterface.Expect(x => x.ConfirmTableAllocation(ref tableAllocationEventArgs.TableAllocation)).Return(true);
            operationLogic.m_HttpComs.Expect(x => x.PutTableAllocation(tableAllocationEventArgs.TableAllocation.PaypalCustomerId, tableAllocationEventArgs.TableAllocation.Id)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsTableAllocationEventHandler_ShouldCallConfirmTableAllocation_ThenRejectTableAllocation__WithTableDoesNotExist()
        {
            var tableAllocationEventArgs = GenerateObjectsAndStringHelper.GenerateTableAllocationEventArgs();
            tableAllocationEventArgs.TableAllocation.rejectionReason = Enums.TableAllocationRejectionReasons.TableDoesNotExist;
            orderingInterface.Expect(x => x.ConfirmTableAllocation(ref tableAllocationEventArgs.TableAllocation)).Return(false);
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(tableAllocationEventArgs.TableAllocation.PaypalCustomerId, tableAllocationEventArgs.TableAllocation.Id, tableAllocationEventArgs.TableAllocation.rejectionReason)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsTableAllocationEventHandler_ShouldCallConfirmTableAllocation_ThenRejectTableAllocation__NotWithTableDoesNotExist()
        {
            var tableAllocationEventArgs = GenerateObjectsAndStringHelper.GenerateTableAllocationEventArgs();
            tableAllocationEventArgs.TableAllocation.rejectionReason = Enums.TableAllocationRejectionReasons.TableIsOccupied;
            orderingInterface.Expect(x => x.ConfirmTableAllocation(ref tableAllocationEventArgs.TableAllocation)).Return(false);
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(tableAllocationEventArgs.TableAllocation.PaypalCustomerId, tableAllocationEventArgs.TableAllocation.Id, tableAllocationEventArgs.TableAllocation.rejectionReason)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_CancelledStatus_ShouldCallRecordOrderUpdatedAtTime_AndOrderCancled()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "cancelled";

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.OrderCancled(ref order));
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "cancelled", Order = order });
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_CancelledStatus_ShouldCallRecordOrderUpdatedAtTime_AndConfirmOrderTotalsBeforePaymentRestaurantMode_AndRequestPaymentForOrder()
        {
            var mockOperationLogic = MockRepository.GeneratePartialMock<DoshiiOperationLogic>(orderingInterface);
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "ready to pay";

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderTotalsBeforePaymentRestaurantMode(ref order));
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(order)).Return(true);
            mockOperationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "ready to pay", Order = order });
            orderingInterface.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "new";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            
            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);
            
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_PaymentWithRemainder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "new";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(new Models.Order(){ Id = GenerateObjectsAndStringHelper.TestOrderId,
                                                                                                                                                            CheckinId = GenerateObjectsAndStringHelper.TestCheckinId,
                                                                                                                                                            NotPayingTotal = "2"
                                                                                                                                                        }).Repeat.Once();
            
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_FailedAvailability()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "new";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();
            
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_RestaurantMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "new";
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_RestaurantMode_FailedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "new";
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;

            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();
            //need to test that it fails if the nonPaying total is != 0
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "pending";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_PaymentWithRemainder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "pending";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(new Models.Order()
            {
                Id = GenerateObjectsAndStringHelper.TestOrderId,
                CheckinId = GenerateObjectsAndStringHelper.TestCheckinId,
                NotPayingTotal = "2"
            }).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_FailedAvailability()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "pending";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_RestaurantMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "pending";
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_RestaurantMode_FailedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "pending";
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;

            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }
    }
}
