using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using DoshiiDotNetIntegration;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class OperationLogicTest
    {

        DoshiiOperationLogic operationLogic;
        DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering orderingInterface;
        string socketUrl = "";
        string token = "";
        DoshiiDotNetIntegration.Enums.OrderModes orderMode;
        DoshiiDotNetIntegration.Enums.SeatingModes seatingMode;
        string urlBase = "";
        bool startWebSocketsConnection;
        bool removeTableAllocationsAfterPayment;
        int socketTimeOutSecs = 600;
        DoshiiOperationLogic mockOperationLogic;

        
        [SetUp]
        public void Init()
        {
            orderingInterface = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering>();
            operationLogic = new DoshiiOperationLogic(orderingInterface);
            mockOperationLogic = MockRepository.GeneratePartialMock<DoshiiOperationLogic>(orderingInterface);

            socketUrl = "wss://alpha.corp.doshii.co/pos/api/v1/socket";
            token = "QGkXTui42O5VdfSFid_nrFZ4u7A";
            orderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;
            seatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            urlBase = "https://alpha.corp.doshii.co/pos/api/v1";
            startWebSocketsConnection = false;
            removeTableAllocationsAfterPayment = false;
            socketTimeOutSecs = 600;
            operationLogic.m_HttpComs = MockRepository.GenerateMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
            operationLogic.m_SocketComs = MockRepository.GenerateMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, operationLogic, GenerateObjectsAndStringHelper.TestTimeOutValue);
            mockOperationLogic.m_HttpComs = MockRepository.GenerateMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
            mockOperationLogic.m_SocketComs = MockRepository.GenerateMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiWebSocketsCommunication>(GenerateObjectsAndStringHelper.TestSocketUrl, operationLogic, GenerateObjectsAndStringHelper.TestTimeOutValue);
        
        }
        
        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoSocketUrl()
        {
            operationLogic.Initialize("",token,orderMode, seatingMode, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoUrlBase()
        {
            operationLogic.Initialize(socketUrl, token, orderMode, seatingMode, "", startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoSocketTimeOutValue()
        {
            operationLogic.Initialize(socketUrl, token, orderMode, seatingMode, urlBase, startWebSocketsConnection, 0);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Initialze_NoToken()
        {
            operationLogic.Initialize(socketUrl, "", orderMode, seatingMode, urlBase, startWebSocketsConnection, socketTimeOutSecs);
        }

        [Test]
        public void findCurrentConsumer_ConsumerExistsInList()
        {
            List<DoshiiDotNetIntegration.Models.Consumer> list = new List<DoshiiDotNetIntegration.Models.Consumer>();
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer1());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer2());
            list.Add(GenerateObjectsAndStringHelper.GenerateConsumer3());

            bool exists = operationLogic.findCurrentConsumer(list, GenerateObjectsAndStringHelper.GenerateConsumer3());
            Assert.IsTrue(exists);
        }

        [Test]
        public void findCurrentConsumer_ConsumerIsNotInList()
        {
            List<DoshiiDotNetIntegration.Models.Consumer> list = new List<DoshiiDotNetIntegration.Models.Consumer>();
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
            operationLogic.SocketComsCheckOutEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs() { MeerkatConsumerId = GenerateObjectsAndStringHelper.TestCustomerId });
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsConnectionEventHandler_shouldCallRefreshConsumerData()
        {
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
            operationLogic.m_HttpComs.Expect(x => x.PutTableAllocation(tableAllocationEventArgs.TableAllocation.MeerkatConsumerId, tableAllocationEventArgs.TableAllocation.Id)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsTableAllocationEventHandler_ShouldCallConfirmTableAllocation_ThenRejectTableAllocation__WithTableDoesNotExist()
        {
            var tableAllocationEventArgs = GenerateObjectsAndStringHelper.GenerateTableAllocationEventArgs();
            tableAllocationEventArgs.TableAllocation.rejectionReason = DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.TableDoesNotExist;
            orderingInterface.Expect(x => x.ConfirmTableAllocation(ref tableAllocationEventArgs.TableAllocation)).Return(false);
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(tableAllocationEventArgs.TableAllocation.MeerkatConsumerId, tableAllocationEventArgs.TableAllocation.Id, tableAllocationEventArgs.TableAllocation.rejectionReason)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsTableAllocationEventHandler_ShouldCallConfirmTableAllocation_ThenRejectTableAllocation__NotWithTableDoesNotExist()
        {
            var tableAllocationEventArgs = GenerateObjectsAndStringHelper.GenerateTableAllocationEventArgs();
            tableAllocationEventArgs.TableAllocation.rejectionReason = DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.TableIsOccupied;
            orderingInterface.Expect(x => x.ConfirmTableAllocation(ref tableAllocationEventArgs.TableAllocation)).Return(false);
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(tableAllocationEventArgs.TableAllocation.MeerkatConsumerId, tableAllocationEventArgs.TableAllocation.Id, tableAllocationEventArgs.TableAllocation.rejectionReason)).Return(true);

            operationLogic.SocketComsTableAllocationEventHandler(new object(), tableAllocationEventArgs);
            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_CancelledStatus_ShouldCallRecordOrderUpdatedAtTime_AndOrderCancled()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "cancelled";

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.OrderCancled(ref order));
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "cancelled", Order = order });
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_CancelledStatus_ShouldCallRecordOrderUpdatedAtTime_AndConfirmOrderTotalsBeforePaymentRestaurantMode_AndRequestPaymentForOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "ready to pay";

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderTotalsBeforePaymentRestaurantMode(ref order));
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(order)).Return(true);
            mockOperationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "ready to pay", Order = order });
            orderingInterface.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "new";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            
            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_PaymentWithRemainder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "new";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(new DoshiiDotNetIntegration.Models.Order()
            {
                Id = GenerateObjectsAndStringHelper.TestOrderId,
                                                                                                                                                            CheckinId = GenerateObjectsAndStringHelper.TestCheckinId,
                                                                                                                                                            NotPayingTotal = "2"
                                                                                                                                                        }).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_BistroMode_FailedAvailability()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "new";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_RestaurantMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "new";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_NewStatus_RestaurantMode_FailedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "new";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();
            //need to test that it fails if the nonPaying total is != 0
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "pending";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_PaymentWithRemainder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "pending";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(new DoshiiDotNetIntegration.Models.Order()
            {
                Id = GenerateObjectsAndStringHelper.TestOrderId,
                CheckinId = GenerateObjectsAndStringHelper.TestCheckinId,
                NotPayingTotal = "2"
            }).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_BistroMode_FailedAvailability()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "pending";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderAvailabilityBistroMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();

            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_RestaurantMode_PassedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "pending";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            orderingInterface.Expect(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Expect(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(true);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            orderingInterface.VerifyAllExpectations();
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SocketComsOrderStatusEventHandler_PendingStatus_RestaurantMode_FailedAvailabliity()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "pending";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            orderingInterface.Stub(x => x.RecordOrderUpdatedAtTime(order));
            orderingInterface.Stub(x => x.ConfirmOrderForRestaurantMode(ref order)).Return(false);

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "rejected"))).Return(order).Repeat.Once();
            operationLogic.SocketComsOrderStatusEventHandler(new object(), new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs { OrderId = GenerateObjectsAndStringHelper.TestOrderId.ToString(), Status = "new", Order = order });

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_PutOrderException()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.NotFound });
            
            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_FullyPaid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();
            
            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_FullyPaid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_PartialPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.NotPayingTotal = "2";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }
        
        [Test]
        public void RequestPaymentForOrder_RestaurantMode_PartialPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.NotPayingTotal = "2";
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordPartialCheckPayment(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_FullyPaid_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_FullyPaid_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_BistroMode_PartialPayment_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPaymentBistroMode(ref order)).Return(true);

            operationLogic.RequestPaymentForOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RequestPaymentForOrder_RestaurantMode_PartialPayment_DontDeleteAllocation()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "waiting for payment"))).Return(order).Repeat.Once();

            orderingInterface.Expect(x => x.RecordFullCheckPayment(ref order)).Return(true);

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
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.ProductNotCreatedException))]
        public void AddNewProducts_ShouldCall_PostProductData()
        {
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();

            operationLogic.m_HttpComs.Expect(x => x.PostProductData(productList, false)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostProductData(productList, false)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.AddNewProducts(productList);
            operationLogic.AddNewProducts(productList);
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void UpdateProducts_ShouldCall_PutProductData()
        {
            var product = GenerateObjectsAndStringHelper.GenerateProduct1WithOptions();

            operationLogic.m_HttpComs.Expect(x => x.PutProductData(product)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutProductData(product)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.UpdateProcuct(product);
            operationLogic.UpdateProcuct(product);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteProducts_ShouldCall_DeleteProduct()
        {
            var productNoList = new List<string>();
            productNoList.Add("one");
            productNoList.Add("two");
            
            mockOperationLogic.Expect(x => x.DeleteProduct(productNoList[0])).Repeat.Once();
            mockOperationLogic.Expect(x => x.DeleteProduct(productNoList[0])).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            mockOperationLogic.DeleteProducts(productNoList);
            mockOperationLogic.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteAllProducts_ShouldCall_DeleteProductData()
        {
            operationLogic.m_HttpComs.Expect(x => x.DeleteProductData()).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.DeleteProductData()).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.DeleteAllProducts();
            operationLogic.DeleteAllProducts();

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetOrder_ShouldCall_GetOrder()
        {
            operationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString())).Return(GenerateObjectsAndStringHelper.GenerateOrderAccepted()).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString())).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());
            operationLogic.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void UpdateOrder_NewOrder_WhenOrderPaid_CallsPostOrder_CheckOutConsumerWithCheckinId()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "paid";
            order.Id = 0;

            operationLogic.m_HttpComs.Expect(x => x.PostOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "paid"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostOrder(order)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.NotFound }).Repeat.Once();
            orderingInterface.Expect(x => x.CheckOutConsumerWithCheckInId(order.CheckinId));
            
            operationLogic.UpdateOrder(order);
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void UpdateOrder_NewOrder_WhenOrderAccepted_CallsPostOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "accepted";
            order.Id = 0;

            operationLogic.m_HttpComs.Expect(x => x.PostOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            orderingInterface.Expect(x => x.RecordOrderId(order));

            operationLogic.UpdateOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void UpdateOrder_ExistingOrder_WhenOrderPaid_CallsPutOrder_CheckOutConsumerWithCheckinId()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "paid";

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "paid"))).Return(order).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PutOrder(order)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.NotFound }).Repeat.Once();
            orderingInterface.Expect(x => x.CheckOutConsumerWithCheckInId(order.CheckinId));
            orderingInterface.Expect(x => x.RecordOrderId(order));

            operationLogic.UpdateOrder(order);
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.ConflictWithOrderUpdateException))]
        public void UpdateOrder_ExistingOrder_NewOrder_ConflictUpdating()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Id = 0;

            operationLogic.m_HttpComs.Stub(x => x.PostOrder(order)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();
            
            operationLogic.UpdateOrder(order);
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.ConflictWithOrderUpdateException))]
        public void UpdateOrder_ExistingOrder_ExistingOrder_ConflictUpdating()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();

            operationLogic.m_HttpComs.Stub(x => x.PutOrder(order)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException() { StatusCode = System.Net.HttpStatusCode.Conflict }).Repeat.Once();

            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void UpdateOrder_ExistingOrder_WhenOrderAccepted_CallsPutOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            order.Status = "accepted";

            operationLogic.m_HttpComs.Expect(x => x.PutOrder(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == "accepted"))).Return(order).Repeat.Once();
            
            operationLogic.UpdateOrder(order);

            operationLogic.m_HttpComs.VerifyAllExpectations();
            
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetConsumer_CallsGetConsumer()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumer.MeerkatConsumerId)).Return(consumer).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumer.MeerkatConsumerId)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.GetConsumer(consumer.MeerkatConsumerId);
            operationLogic.GetConsumer(consumer.MeerkatConsumerId);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void SetTableAllocation_CallsPostTableAllocation()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.SetTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);
            operationLogic.SetTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteTableAllocation_CallsRejectTableAllocation()
        {
            var consumer = GenerateObjectsAndStringHelper.GenerateConsumer1();
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos)).Return(true).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);
            operationLogic.DeleteTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetCheckedInConsumersFromDoshii_CallsGetConsumers()
        {
            var consumerList = GenerateObjectsAndStringHelper.GenerateConsumerList();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumers()).Return(consumerList).Repeat.Once();
            operationLogic.m_HttpComs.Expect(x => x.GetConsumers()).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            operationLogic.GetCheckedInConsumersFromDoshii();
            operationLogic.GetCheckedInConsumersFromDoshii();

            operationLogic.m_HttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RefreshConsumerData_SetSeatingAndOrderConfigurationFails()
        {
            operationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            operationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            operationLogic.m_HttpComs.Expect(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode)).Return(false).Repeat.Once();
            operationLogic.m_SocketComs.Expect(x => x.CloseSocketConnection()).Repeat.Once();

            operationLogic.RefreshConsumerData();
            
            operationLogic.m_HttpComs.VerifyAllExpectations();
            operationLogic.m_SocketComs.VerifyAllExpectations();
        }

        [Test]
        public void RefreshConsumerData_SetSeatingAndOrderConfigurationPasses_RequestPaymentForOrder()
        {
            mockOperationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            mockOperationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            mockOperationLogic.m_HttpComs.Stub(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode)).Return(true).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetConsumers()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetTableAllocations()).Return(new List<DoshiiDotNetIntegration.Models.TableAllocation>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetOrders()).Return(new List<DoshiiDotNetIntegration.Models.Order>()).Repeat.Once();
            var orderList = GenerateObjectsAndStringHelper.GenerateOrderList();

            orderingInterface.Expect(x => x.GetCheckedInCustomersFromPos()).Return(GenerateObjectsAndStringHelper.GenerateConsumerList()).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            
            orderingInterface.Expect(x => x.GetOrderForCheckinId(GenerateObjectsAndStringHelper.GenerateConsumer1().CheckInId)).Return(orderList[0]).Repeat.Once();
            orderingInterface.Expect(x => x.GetOrderForCheckinId(GenerateObjectsAndStringHelper.GenerateConsumer2().CheckInId)).Return(orderList[1]).Repeat.Once();
            orderingInterface.Expect(x => x.GetOrderForCheckinId(GenerateObjectsAndStringHelper.GenerateConsumer3().CheckInId)).Return(orderList[2]).Repeat.Once();
            orderingInterface.Expect(x => x.GetOrderForCheckinId(GenerateObjectsAndStringHelper.GenerateConsumer4().CheckInId)).Return(orderList[3]).Repeat.Once();
            
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(orderList[0])).Return(true).Repeat.Once();
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(orderList[1])).Return(true).Repeat.Once();
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(orderList[2])).Return(true).Repeat.Once();
            mockOperationLogic.Expect(x => x.RequestPaymentForOrder(orderList[3])).Return(true).Repeat.Once();

            mockOperationLogic.RefreshConsumerData();

            mockOperationLogic.VerifyAllExpectations();
            orderingInterface.VerifyAllExpectations();
        }

        [Test]
        public void RefreshConsumerData_SetSeatingAndOrderConfigurationPasses_SocketComsConsumerCheckinEventHandler()
        {
            mockOperationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            mockOperationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            var consumerList = GenerateObjectsAndStringHelper.GenerateConsumerList();
            mockOperationLogic.m_HttpComs.Stub(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode)).Return(true).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetTableAllocations()).Return(new List<DoshiiDotNetIntegration.Models.TableAllocation>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetOrders()).Return(new List<DoshiiDotNetIntegration.Models.Order>()).Repeat.Once();
            
            mockOperationLogic.m_HttpComs.Expect(x => x.GetConsumers()).Return(consumerList).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumerList[0].MeerkatConsumerId)).Return(GenerateObjectsAndStringHelper.GenerateConsumer1()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumerList[1].MeerkatConsumerId)).Return(GenerateObjectsAndStringHelper.GenerateConsumer2()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumerList[2].MeerkatConsumerId)).Return(GenerateObjectsAndStringHelper.GenerateConsumer3()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetConsumer(consumerList[3].MeerkatConsumerId)).Return(GenerateObjectsAndStringHelper.GenerateConsumer4()).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsConsumerCheckinEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckInEventArgs>.Matches(y => y.CheckIn == consumerList[0].CheckInId))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsConsumerCheckinEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckInEventArgs>.Matches(y => y.CheckIn == consumerList[1].CheckInId))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsConsumerCheckinEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckInEventArgs>.Matches(y => y.CheckIn == consumerList[2].CheckInId))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsConsumerCheckinEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckInEventArgs>.Matches(y => y.CheckIn == consumerList[3].CheckInId))).Repeat.Once();
            
            
            
            mockOperationLogic.RefreshConsumerData();

            mockOperationLogic.m_HttpComs.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
            
        }

        [Test]
        public void RefreshConsumerData_SetSeatingAndOrderConfigurationPasses_SocketComsTableAllocationEventHandler()
        {
            mockOperationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            mockOperationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            var tableAllocationList = GenerateObjectsAndStringHelper.GenerateTableAllocationList();
            mockOperationLogic.m_HttpComs.Stub(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode)).Return(true).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetConsumers()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetOrders()).Return(new List<DoshiiDotNetIntegration.Models.Order>()).Repeat.Once();
                        
            mockOperationLogic.m_HttpComs.Expect(x => x.GetTableAllocations()).Return(tableAllocationList).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsTableAllocationEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs>.Matches(y => y.TableAllocation.CustomerId == tableAllocationList[0].CustomerId))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsTableAllocationEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs>.Matches(y => y.TableAllocation.CustomerId == tableAllocationList[1].CustomerId))).Repeat.Once();

            mockOperationLogic.RefreshConsumerData();

            mockOperationLogic.m_HttpComs.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
            
        }

        [Test]
        public void RefreshConsumerData_SetSeatingAndOrderConfigurationPasses_SocketComsOrderStatusEventHandler()
        {
            mockOperationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            mockOperationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.BistroMode;
            var orderList = GenerateObjectsAndStringHelper.GenerateOrderList();
            mockOperationLogic.m_HttpComs.Stub(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode)).Return(true).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetConsumers()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetTableAllocations()).Return(new List<DoshiiDotNetIntegration.Models.TableAllocation>()).Repeat.Once();

            mockOperationLogic.m_HttpComs.Expect(x => x.GetOrders()).Return(orderList).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.GenerateOrderPending().Id.ToString())).Return(GenerateObjectsAndStringHelper.GenerateOrderPending()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.GenerateOrderReadyToPay().Id.ToString())).Return(GenerateObjectsAndStringHelper.GenerateOrderReadyToPay()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Expect(x => x.GetOrder(GenerateObjectsAndStringHelper.GenerateOrderCancelled().Id.ToString())).Return(GenerateObjectsAndStringHelper.GenerateOrderCancelled()).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsOrderStatusEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs>.Matches(y => y.OrderId == GenerateObjectsAndStringHelper.GenerateOrderPending().Id.ToString()))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsOrderStatusEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs>.Matches(y => y.OrderId == GenerateObjectsAndStringHelper.GenerateOrderReadyToPay().Id.ToString()))).Repeat.Once();
            mockOperationLogic.Expect(x => x.SocketComsOrderStatusEventHandler(Arg<DoshiiOperationLogic>.Is.Anything, Arg<DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.OrderEventArgs>.Matches(y => y.OrderId == GenerateObjectsAndStringHelper.GenerateOrderCancelled().Id.ToString()))).Repeat.Once();

            mockOperationLogic.RefreshConsumerData();

            mockOperationLogic.m_HttpComs.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
            
        }

        [Test]
        public void RefreshConsumerData_RemoveCheckinsFromThePos()
        {
            mockOperationLogic.SeatingMode = DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation;
            mockOperationLogic.OrderMode = DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode;
            mockOperationLogic.m_HttpComs.Stub(x => x.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode)).Return(true).Repeat.Once();

            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(GenerateObjectsAndStringHelper.GenerateConsumerList());

            orderingInterface.Stub(x => x.GetCheckedInCustomersFromPos()).Return(GenerateObjectsAndStringHelper.GenerateConsumerList()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetConsumers()).Return(new List<DoshiiDotNetIntegration.Models.Consumer>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetTableAllocations()).Return(new List<DoshiiDotNetIntegration.Models.TableAllocation>()).Repeat.Once();
            mockOperationLogic.m_HttpComs.Stub(x => x.GetOrders()).Return(new List<DoshiiDotNetIntegration.Models.Order>()).Repeat.Once();
            //this should not be ignoring the arguments it should be making sure that what is passed in is what is being in the call
            mockOperationLogic.Expect(x => x.SocketComsCheckOutEventHandler(operationLogic, new DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs())).IgnoreArguments().Repeat.Times(3);

            mockOperationLogic.RefreshConsumerData();

            mockOperationLogic.m_HttpComs.VerifyAllExpectations();
            mockOperationLogic.VerifyAllExpectations();
        }
    }
}
