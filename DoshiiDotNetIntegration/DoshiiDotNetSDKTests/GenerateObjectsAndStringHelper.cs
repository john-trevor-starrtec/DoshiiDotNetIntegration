using AutoMapper;
using System;
using System.Collections.Generic;
using System.Net;

namespace DoshiiDotNetSDKTests
{
    internal static class GenerateObjectsAndStringHelper
    {

        #region test fields

        internal static string TestBaseUrl = "https://Google.com.au";
        internal static string TestSocketUrl = "wss://google.com.au";
        internal static string TestCustomerId = "TestCustomerId";
        internal static string TestMeerkatConsumerId = "TestPaypalCustomerId";
        internal static string TestTableNumber = "TestTableNumber";
        internal static string TestToken = "TestToken";
        internal static string TestCheckinId = "TestCheckinId";
        internal static string TestTableName = "TestName";
        internal static string TestTableAllocationName = "TestAllocationName";
        internal static string TestTableAllocationStatus = "TableAllocationStatus";
        internal static string TestPaypalTabId = "TestPaypalTabId";
        internal static string TestLocationId = "TestLocationId";
        internal static string TestCheckinStatus = "TestCheckinStatus";
        internal static string TestGratuity = "TestGratuity";
        internal static Uri TestCheckinUrl = new Uri("c:\\impos\\");
        internal static string TestOrderId = "123";
        internal static int TestTimeOutValue = 600;
        internal static string TestProductId = "asd123";

        #endregion 

        #region responceMessages

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageOrderSuccess()
        {
			var order = GenerateOrderAccepted();
			var jsonOrder = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(order);
			string json = jsonOrder.ToJsonString();

			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageOrdersSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(Mapper.Map<List<DoshiiDotNetIntegration.Models.Json.JsonOrder>>(GenerateOrderList())),
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageTableAllocationSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(Mapper.Map<List<DoshiiDotNetIntegration.Models.Json.JsonTableAllocation>>(GenerateTableAllocationList())),
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessagePutTableAllocationSuccess()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessagePutTableAllocationFailure()
        {
			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.InternalServerError,
                StatusDescription = "Internal Service Error",
                Data = "",
                ErrorMessage = "",
                Message = ""
            };
        }

		internal static DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage GenerateResponseMessageSuccessfulOrder()
        {
			var order = GenerateOrderAccepted();
			var jsonOrder = Mapper.Map<DoshiiDotNetIntegration.Models.Json.JsonOrder>(order);
			string json = jsonOrder.ToJsonString();

			return new DoshiiDotNetIntegration.CommunicationLogic.DoshiHttpResponseMessage()
            {
                Status = HttpStatusCode.OK,
                StatusDescription = "OK",
                Data = json,
                ErrorMessage = "",
                Message = ""
            };
        }

        #endregion

        #region Generate TestObjects and TestValues

        internal static List<DoshiiDotNetIntegration.Models.Product> GenerateProductList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Product>();
            list.Add(GenerateProduct1WithOptions());
            list.Add(GenerateProduct2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Product GenerateProduct1WithOptions()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product1Department");
            var optionsList = new List<DoshiiDotNetIntegration.Models.ProductOptions>();
            var product = new DoshiiDotNetIntegration.Models.Product()
            {
                Id = "1",
                PosId = "1",
                Name = "Product1",
                Tags = tagsList,
                Price = 1.0M,
                Description = "The first Product",
                ProductOptions = GenerateProductOptionList(),
                Status = "pending"    
            };
            return product;
        }


        internal static DoshiiDotNetIntegration.Models.Product GenerateProduct2()
        {
            var tagsList = new List<string>();
            tagsList.Add("Product2Department");
            var product = new DoshiiDotNetIntegration.Models.Product()
            {
                Id = "2",
                PosId = "2",
                Name = "Product2",
                Tags = tagsList,
                Price = 1.0M,
                Description = "The first Product",
                ProductOptions = null,
                Status = "pending"    
            };
            return product;
        }

        internal static List<DoshiiDotNetIntegration.Models.Variants> GenerateProductVarientList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Variants>();
            list.Add(GenerateProductVarient1());
            list.Add(GenerateProductVarient2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Variants GenerateProductVarient1()
        {
            var variant = new DoshiiDotNetIntegration.Models.Variants()
            {
                Name = "variant1",
                Price = 0.1M,
                PosId = "var1"
            };
            return variant;
        }

        internal static DoshiiDotNetIntegration.Models.Variants GenerateProductVarient2()
        {
            var variant = new DoshiiDotNetIntegration.Models.Variants()
            {
                Name = "variant2",
                Price = 0.1M,
                PosId = "var2"
            };
            return variant;
        }

        internal static List<DoshiiDotNetIntegration.Models.ProductOptions> GenerateProductOptionList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.ProductOptions>();
            list.Add(GenerateProductOption1());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.ProductOptions GenerateProductOption1()
        {
            var productOption = new DoshiiDotNetIntegration.Models.ProductOptions()
            {
                Name = "ProductOptions1",
                Min = 0,
                Max = 0,
                PosId = "10",
                Variants = GenerateProductVarientList(),
                Selected = new List<DoshiiDotNetIntegration.Models.Variants>()
            };
            return productOption;
        }

        internal static List<DoshiiDotNetIntegration.Models.Order> GenerateOrderList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.Order>();
            list.Add(GenerateOrderAccepted());
            list.Add(GenerateOrderPending());
            list.Add(GenerateOrderReadyToPay());
            list.Add(GenerateOrderCancelled());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderAccepted()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "accepted"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderPaid()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "paid"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderWaitingForPayment()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId,
                Status = "waiting_for_payment"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderPending()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 1,
                CheckinId = string.Format("{0}2",TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderReadyToPay()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 2,
                CheckinId = string.Format("{0}3", TestCheckinId),
                Status = "pending"
            };
            return order;
        }

        internal static DoshiiDotNetIntegration.Models.Order GenerateOrderCancelled()
        {
            var order = new DoshiiDotNetIntegration.Models.Order()
            {
                Id = TestOrderId + 3,
                CheckinId = string.Format("{0}4", TestCheckinId),
                Status = "cancelled"
            };
            return order;
        }

        internal static List<DoshiiDotNetIntegration.Models.TableAllocation> GenerateTableAllocationList()
        {
            var list = new List<DoshiiDotNetIntegration.Models.TableAllocation>();
            list.Add(GenerateTableAllocation());
            list.Add(GenerateTableAllocation2());
            return list;
        }

        internal static DoshiiDotNetIntegration.Models.TableAllocation GenerateTableAllocation()
        {
            var tableAllocation = new DoshiiDotNetIntegration.Models.TableAllocation()
            {
                Id = TestTableName,
                Name = TestTableAllocationName,
                Status = TestTableAllocationStatus
            };
            return tableAllocation;
        }

        internal static DoshiiDotNetIntegration.Models.TableAllocation GenerateTableAllocation2()
        {
            var tableAllocation = new DoshiiDotNetIntegration.Models.TableAllocation()
            {
                Id = string.Format("{0}2",TestTableName),
                Name = string.Format("{0}2",TestTableAllocationName),
                Status = TestTableAllocationStatus
            };
            return tableAllocation;
        }

        #endregion
    }
}
