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

        [Test]
        public void RequestPaymentForOrder_PutOrderException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = true;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() {StatusCode = System.Net.HttpStatusCode.NotFound });
            orderingInterface.Expect(x => x.CheckOutConsumerWithCheckInId(order.CheckinId));

            operationLogic.RequestPaymentForOrder(order);

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_FullyPaid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = true;
            
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            
            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Once();
            
            operationLogic.RequestPaymentForOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_FullyPaid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = true;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Once();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_PartialPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.NotPayingTotal = "2";
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = true;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Once();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }
        
        [Test]
        public void RequestPaymentForOrder_RestaurantMode_PartialPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.NotPayingTotal = "2";
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = true;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordPartialCheckPayment(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Once();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_FullyPaid_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = false;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Never();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_FullyPaid_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = false;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Never();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_PartialPayment_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.BistroMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = false;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Never();
            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_PartialPayment_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            operationLogic.OrderMode = Enums.OrderModes.RestaurantMode;
            operationLogic.RemoveTableAllocationsAfterFullPayment = false;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.DeleteTableAllocationWithCheckInId(order.CheckinId, Enums.TableAllocationRejectionReasons.tableHasBeenPaid)).Return(true).Repeat.Never();

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsConsumerCheckinEventHandler_ShouldCallRecordCheckedInUser()
        {
            var checkinArgs = GenerateObjectsAndStringHelper.GenerateCheckinEventArgs();

            orderingInterface.Expect(x => x.RecordCheckedInUser(ref checkinArgs.Consumer));

            operationLogic.SocketComsConsumerCheckinEventHandler(new object(), checkinArgs);

            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void GetAllProducts_ShouldCall_GetDoshiiProducts()
        {
            operationLogic.m_HttpComs.Expect(x => x.GetDoshiiProducts()).Return(GenerateObjectsAndStringHelper.GenerateProductList());
            operationLogic.GetAllProducts();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.ProductNotCreatedException))]
        public void AddNewProducts_ShouldCall_PostProductData()
        {
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();

            operationLogic.m_HttpComs.Expect(x => x.PostProductData(productList, false)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostProductData(productList, false)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() {StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.AddNewProducts(productList);
            operationLogic.AddNewProducts(productList);
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void UpdateProducts_ShouldCall_PutProductData()
        {
            var product = GenerateObjectsAndStringHelper.GenerateProduct1WithOptions();

            operationLogic.m_HttpComs.Expect(x => x.PutProductData(product)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutProductData(product)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.UpdateProcuct(product);
            operationLogic.UpdateProcuct(product);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void DeleteProducts_ShouldCall_DeleteProduct()
        {
            var productNoList = new List<string>();
            productNoList.Add("one");
            productNoList.Add("two");
            var mockOperationLogic = MockRepository.GeneratePartialMock<DoshiiOperationLogic>(orderingInterface);

            mockOperationLogic.Expect(x => x.DeleteProduct(productNoList[0])).Repeat.Once();
            mockOperationLogic.Expect(x => x.DeleteProduct(productNoList[0])).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            mockOperationLogic.DeleteProducts(productNoList);
            mockOperationLogic.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void DeleteAllProducts_ShouldCall_DeleteProductData()
        {
            operationLogic.m_HttpComs.Expect(x => x.DeleteProductData()).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.DeleteProductData()).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.DeleteAllProducts();
            operationLogic.DeleteAllProducts();

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void GetOrder_ShouldCall_GetOrder()
        {
            operationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString())).Return(GenerateObjectsAndStringHelper.GenerateOrder()).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString())).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());
            operationLogic.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void UpdateOrder_NewOrder_WhenOrderPaid_CallsPostOrder_CheckOutConsumerWithCheckinId()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "paid";
            order.Id = 0;

            operationLogic.m_HttpComs.Expect(x => x.PostOrder(Arg<Models.Order>.Matches(y => y.Status == "paid"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostOrder(order)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.NotFound }).Repeat.Once();
            orderingInterface.Expect(x => x.CheckOutConsumerWithCheckInId(order.CheckinId));
            
            operationLogic.UpdateOrder(order);
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void UpdateOrder_NewOrder_WhenOrderAccepted_CallsPostOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "accepted";
            order.Id = 0;

            operationLogic.m_HttpComs.Expect(x => x.PostOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordOrderId(order));

            operationLogic.UpdateOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void UpdateOrder_ExistingOrder_WhenOrderPaid_CallsPutOrder_CheckOutConsumerWithCheckinId()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "paid";
            
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "paid"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(order)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.NotFound }).Repeat.Once();
            orderingInterface.Expect(x => x.CheckOutConsumerWithCheckInId(order.CheckinId));
            orderingInterface.Expect(x => x.RecordOrderId(order));

            operationLogic.UpdateOrder(order);
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.ConflictWithOrderUpdateException))]
        public void UpdateOrder_ExistingOrder_NewOrder_ConflictUpdating()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Id = 0;
            
            operationLogic.m_HttpComs.Stub(x => x.PostOrder(order)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();
            
            operationLogic.UpdateOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.ConflictWithOrderUpdateException))]
        public void UpdateOrder_ExistingOrder_ExistingOrder_ConflictUpdating()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            
            operationLogic.m_HttpComs.Stub(x => x.PutOrder(order)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void UpdateOrder_ExistingOrder_WhenOrderAccepted_CallsPutOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrder();
            order.Status = "accepted";
            
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void GetConsumer_CallsGetConsumer()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumer.PaypalCustomerId)).Return(consumer).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumer.PaypalCustomerId)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.GetConsumer(consumer.PaypalCustomerId);
            operationLogic.GetConsumer(consumer.PaypalCustomerId);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void SetTableAllocation_CallsPostTableAllocation()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.SetTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);
            operationLogic.SetTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void DeleteTableAllocation_CallsRejectTableAllocation()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos)).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);
            operationLogic.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        
        [Test]
        [ExpectedException(typeof(Exceptions.RestfulApiErrorResponseException))]
        public void GetCheckedInConsumersFromDoshii_CallsGetConsumers()
        {
            var consumerList = GenerateObjectsAndStringHelper.GenerateConsumerList();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumers()).Return(consumerList).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumers()).IgnoreArguments().Throw(new Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.GetCheckedInConsumersFromDoshii();
            operationLogic.GetCheckedInConsumersFromDoshii();

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }
    }
}
