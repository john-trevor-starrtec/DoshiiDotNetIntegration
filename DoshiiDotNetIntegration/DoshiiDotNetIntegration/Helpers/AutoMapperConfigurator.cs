using AutoMapper;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Globalization;
using System.Linq;
using DoshiiDotNetIntegration.Exceptions;

namespace DoshiiDotNetIntegration.Helpers
{
	/// <summary>
	/// This helper class is used to configure the AutoMapper mappings of objects
	/// between the raw JSON communication objects and the model objects used by 
	/// the SDK on behalf of the POS.
	/// </summary>
	/// <remarks>
	/// This helper class is for internal use only within the SDK and should not
	/// be required from the POS. Its sole responsibility is to map the objects
	/// used in communication to their data model equivalents, which should not
	/// be changed by the POS implementation.
	/// </remarks>
	internal static class AutoMapperConfigurator
	{
		/// <summary>
		/// Flag to indicate whether the mapping has already been configured within 
		/// the application.
		/// </summary>
		private static bool IsConfigured = false;

		/// <summary>
		/// The Date/Time format used in the JSON conversion throughout the application.
		/// </summary>
		private const string DateTimeFormat = "dd/MM/yyyy'T'HH:mm:ss.fffK";

		/// <summary>
		/// The number of cents in one dollar.
		/// </summary>
		private const decimal CentsPerDollar = 100.0M;

		/// <summary>
		/// This method can be called by the application to configure the object
		/// mappings used within the application.
		/// </summary>
		internal static void Configure()
		{
			if (!AutoMapperConfigurator.IsConfigured)
			{
				AutoMapperConfigurator.MapVariantsObjects();
				AutoMapperConfigurator.MapProductObjects();
				AutoMapperConfigurator.MapSurcountObjects();
				AutoMapperConfigurator.MapTransactionObjects();
				AutoMapperConfigurator.MapTableAllocationObjects();
			    AutoMapperConfigurator.MapConsumerObjects();
				AutoMapperConfigurator.MapOrderObjects();
                AutoMapperConfigurator.MapMenuObjects();
			    AutoMapperConfigurator.MapLocationObjects();
			    AutoMapperConfigurator.MapAddressObjects();
                AutoMapperConfigurator.MapAppObjects();
                AutoMapperConfigurator.MapMemberObjects();
                AutoMapperConfigurator.MapRewardObjects();
                AutoMapperConfigurator.MapPointsRedeemObjects();
                AutoMapperConfigurator.MapCheckInObjects();
                AutoMapperConfigurator.MapTableCriteraObjects();
                AutoMapperConfigurator.MapTableObjects();
                AutoMapperConfigurator.MapBookingObjects();

                AutoMapperConfigurator.IsConfigured = true;
			}
		}

        /// <summary>
        /// This function creates a bi-directional object mapping between the Booking model objects and their
        /// JSON equivalent data transfer objects.
        /// </summary>
        private static void MapBookingObjects()
        {
            Mapper.CreateMap<Booking, JsonBooking>()
                .ForMember(dest => dest.TableNames, opt => opt.MapFrom(src => src.TableNames.ToList<string>()))
                .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapIntegerToString(src.Covers)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToUtcTime(src.UpdatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToUtcTime(src.CreatedAt)))
                .ForMember(dest => dest.Uri, opt => opt.Ignore());

            Mapper.CreateMap<JsonBooking, Booking>()
                .ForMember(dest => dest.TableNames, opt => opt.MapFrom(src => src.TableNames.ToList<string>()))
                .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapStringToInteger(src.Covers)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.UpdatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.CreatedAt)))
                .ForMember(dest => dest.Uri, opt => opt.Ignore());
        }

        private static void MapTableObjects()
        {
            // src = Table, dest = JsonTable
            Mapper.CreateMap<Table, JsonTable>()
            .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapIntegerToString(src.Covers)));

            // src = JsonTable, dest = Table
            Mapper.CreateMap<JsonTable, Table>()
            .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapStringToInteger(src.Covers)));

        }
        
        private static void MapTableCriteraObjects()
        {
            // src = Order, dest = JsonOrder
            Mapper.CreateMap<TableCriteria, JsonTableCriteria>();
                
            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonTableCriteria, TableCriteria>();
                
        }
        
        private static void MapCheckInObjects()
        {
            // src = Order, dest = JsonOrder
            Mapper.CreateMap<Checkin, JsonCheckin>()
                .ForMember(dest => dest.TableNames, opt => opt.MapFrom(src => src.TableNames.ToList<string>()))
                .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapIntegerToString(src.Covers)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToUtcTime(src.UpdatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToUtcTime(src.CreatedAt)))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToUtcTime(src.CompletedAt)));

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonCheckin, Checkin>()
                .ForMember(dest => dest.Covers, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapStringToInteger(src.Covers)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.UpdatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.CreatedAt)))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.CompletedAt)));
        }

        private static void MapPointsRedeemObjects()
        {
            // Mapping from Variants to JsonOrderVariants
            // src = PointsRedeem, dest = JsonPointsRedeem, opt = Mapping Option
            Mapper.CreateMap<PointsRedeem, JsonPointsRedeem>()
                .ForMember(dest => dest.Points, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapIntegerToString(src.Points)));

            // src = JsonPointsRedeem, dest = PointsRedeem
            Mapper.CreateMap<JsonPointsRedeem, PointsRedeem>()
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => AutoMapperConfigurator.MapStringToInteger(src.Points)));
        }
        
        private static void MapRewardObjects()
        {
            // Mapping from Variants to JsonOrderVariants
            // src = Reward, dest = JsonReward, opt = Mapping Option
            Mapper.CreateMap<Reward, JsonReward>()
                .ForMember(dest => dest.SurcountAmount, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapSurchargeAmountToString(src.SurcountAmount, src.SurcountType)));

            // src = JsonReward, dest = Reward
            Mapper.CreateMap<JsonReward, Reward>()
                .ForMember(dest => dest.SurcountAmount, opt => opt.MapFrom(src => AutoMapperConfigurator.MapSurchargeAmountToDouble(src.SurcountAmount, src.SurcountType)));
        }
        
        private static void MapAddressObjects()
        {
            // Mapping from Variants to JsonOrderVariants
            // src = Address, dest = JsonAddress, opt = Mapping Option
            Mapper.CreateMap<Address, JsonAddress>();

            // src = JsonAddress, dest = Address
            Mapper.CreateMap<JsonAddress, Address>();
        }

        private static void MapMemberObjects()
        {
            // Mapping from Variants to JsonOrderVariants
            // src = Member, dest = JsonMember, opt = Mapping Option
            Mapper.CreateMap<Member, JsonMember>()
                .ForMember(dest => dest.Apps, opt => opt.MapFrom(src => src.Apps.ToList<App>()));

            // src = JsonAddress, dest = Address
            Mapper.CreateMap<JsonMember, Member>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.UpdatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.CreatedAt)));

            // Mapping from Variants to JsonOrderVariants
            // src = Member, dest = JsonMember, opt = Mapping Option
            Mapper.CreateMap<Member, JsonMemberToUpdate>();

            // src = JsonAddress, dest = Address
            Mapper.CreateMap<JsonMemberToUpdate, Member>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Uri, opt => opt.Ignore())
                .ForMember(dest => dest.Apps, opt => opt.Ignore());

        }

        private static void MapAppObjects()
        {
            // Mapping from Variants to JsonOrderVariants
            // src = App, dest = JsonApp, opt = Mapping Option
            Mapper.CreateMap<App, JsonApp>()
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Points)));

            // src = JsonApp, dest = App
            Mapper.CreateMap<JsonApp, App>()
                .ForMember(dest => dest.Points, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Points)));
        }


		/// <summary>
		/// This function creates a bi-directional object mapping between the Variants model object and its
		/// JSON equivalent data transfer object.
		/// </summary>
		private static void MapVariantsObjects()
		{
			// Mapping from Variants to JsonOrderVariants
			// src = Variants, dest = JsonOrderVariants, opt = Mapping Option
			Mapper.CreateMap<Variants, JsonOrderVariants>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Price)));

			// src = JsonOrderVariants, dest = Variants
			Mapper.CreateMap<JsonOrderVariants, Variants>()
                .ForMember(dest => dest.Price, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Price)));

            // src = Variants, dest = JsonOrderVariants, opt = Mapping Option
            Mapper.CreateMap<Variants, JsonMenuVariants>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Price)));

            // src = JsonOrderVariants, dest = Variants
            Mapper.CreateMap<JsonMenuVariants, Variants>()
                //.ForMember(dest => dest.SelectedOptionalVariant, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Price)));
		}


        /// <summary>
		/// This function creates a bi-directional object mapping between the Product model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapProductObjects()
		{
			// src = ProductOptions, dest = JsonOrderProductOptions
			Mapper.CreateMap<ProductOptions, JsonOrderProductOptions>()
				.ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.ToList<Variants>()));

			// src = JsonOrderProductOptions, dest = ProductOptions
			Mapper.CreateMap<JsonOrderProductOptions, ProductOptions>();

			// src = Product, dest = JsonOrderProduct
			Mapper.CreateMap<Product, JsonOrderProduct>()
				.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.UnitPrice)))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => AutoMapperConfigurator.MapQuantityToString(src.Quantity)))
                .ForMember(dest => dest.TotalAfterSurcounts, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.TotalAfterSurcounts)))
                .ForMember(dest => dest.TotalBeforeSurcounts, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.TotalBeforeSurcounts)))
				.ForMember(dest => dest.ProductOptions, opt => opt.MapFrom(src => src.ProductOptions.ToList<ProductOptions>()))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList<string>()));

			// src = JsonOrderProduct, dest = Product
		    
            Mapper.CreateMap<JsonOrderProduct, Product>()
            .ForMember(dest => dest.UnitPrice,
                opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.UnitPrice)))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => AutoMapperConfigurator.MapQuantity(src.Quantity)))
            .ForMember(dest => dest.TotalAfterSurcounts,
                opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.TotalAfterSurcounts)))
            .ForMember(dest => dest.TotalBeforeSurcounts,
                opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.TotalBeforeSurcounts)));
		   
            
                
            // src = ProductOptions, dest = JsonOrderProductOptions
            Mapper.CreateMap<ProductOptions, JsonMenuProductOptions>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.ToList<Variants>()));

            // src = JsonOrderProductOptions, dest = ProductOptions
		    Mapper.CreateMap<JsonMenuProductOptions, ProductOptions>();
                
            // src = ProductOptions, dest = JsonOrderProductOptions
            Mapper.CreateMap<ProductOptions, JsonOrderProductOptions>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.ToList<Variants>()));

            // src = JsonOrderProductOptions, dest = ProductOptions
            Mapper.CreateMap<JsonOrderProductOptions, ProductOptions>()
                .ForMember(dest => dest.Max, opt => opt.Ignore())
                .ForMember(dest => dest.Min, opt => opt.Ignore());

            // src = Product, dest = JsonOrderProduct
            Mapper.CreateMap<Product, JsonMenuProduct>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.UnitPrice)))
                .ForMember(dest => dest.ProductOptions, opt => opt.MapFrom(src => src.ProductOptions.ToList<ProductOptions>()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList<string>()));

            // src = JsonOrderProduct, dest = Product
		    Mapper.CreateMap<JsonMenuProduct, Product>()
                .ForMember(dest => dest.UnitPrice, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.UnitPrice)))
                .ForMember(dest => dest.TotalAfterSurcounts, opt => opt.Ignore())
                .ForMember(dest => dest.TotalBeforeSurcounts, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.Ignore());
		}

        /// <summary>
        /// This function creates a bi-directional object mapping between the Menu model objects and their
        /// JSON equivalent data transfer objects.
        /// </summary>
        private static void MapMenuObjects()
        {
            // src = Menu, dest = JsonMenu
            Mapper.CreateMap<Menu, JsonMenu>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products.ToList<Product>()))
                .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));

            // src = JsonMenu, dest = Menu
            Mapper.CreateMap<JsonMenu, Menu>();

        }

        /// <summary>
        /// This function creates a bi-directional object mapping between the Location model objects and their
        /// JSON equivalent data transfer objects.
        /// </summary>
        private static void MapLocationObjects()
        {
            // src = Locaiton, dest = JsonLocaiton
            Mapper.CreateMap<Location, JsonLocation>();
                
            // src = JsonLocation, dest = Location
            Mapper.CreateMap<JsonLocation, Location>();

        }

		/// <summary>
		/// This function creates a bi-directional object mapping between the Surcount model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapSurcountObjects()
		{
			// src = Surcount, dest = JsonOrderSurcount
			Mapper.CreateMap<Surcount, JsonOrderSurcount>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Value)))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Amount)));

			// src = JsonOrderSurcount, dest = Surcount
			Mapper.CreateMap<JsonOrderSurcount, Surcount>()
                .ForMember(dest => dest.Value, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Value)))
                .ForMember(dest => dest.Amount, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Amount)));

            // src = Surcount, dest = JsonOrderSurcount
		    Mapper.CreateMap<Surcount, JsonMenuSurcount>();
                
            // src = JsonOrderSurcount, dest = Surcount
            Mapper.CreateMap<JsonMenuSurcount, Surcount>()
                .ForMember(dest => dest.RewardId, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.Ignore());
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Transaction model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapTransactionObjects()
		{
			// src = Transaction, dest = JsonTransaction
			Mapper.CreateMap<Transaction, JsonTransaction>()
                .ForMember(dest => dest.Tip, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Tip)))
				.ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.PaymentAmount)));

			// src = JsonTransaction, dest = Transaction
			Mapper.CreateMap<JsonTransaction, Transaction>()
                .ForMember(dest => dest.Tip, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.Tip)))
				.ForMember(dest => dest.PaymentAmount, opt => opt.ResolveUsing(src => AutoMapperConfigurator.MapCurrency(src.PaymentAmount)));
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Table Allocation model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapTableAllocationObjects()
		{
			// src = TableAllocation, dest = JsonTableAllocation
			Mapper.CreateMap<TableAllocation, JsonTableAllocation>();

			// src = JsonTableAllocation, dest = TableAllocation
			Mapper.CreateMap<JsonTableAllocation, TableAllocation>();

            // src = TableAllocation, dest = JsonTableAllocation
		    Mapper.CreateMap<TableAllocation, JsonTableAllocationForCreate>();
		        

            // src = JsonTableAllocation, dest = TableAllocation
            Mapper.CreateMap<JsonTableAllocationForCreate, TableAllocation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore());
		}

        /// <summary>
        /// This function creates a bi-directional object mapping between the Consumer model objects and their
        /// JSON equivalent data transfer objects.
        /// </summary>
	    private static void MapConsumerObjects()
	    {
            // src = Consumer, dest = JsonConsumer
            Mapper.CreateMap<Consumer, JsonConsumer>();

            // src = JsonConsumer, dest = Consumer
            Mapper.CreateMap<JsonConsumer, Consumer>();
	    }

		/// <summary>
		/// This function creates a bi-directional object mapping between the Order model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapOrderObjects()
		{
            Mapper.CreateMap<OrderWithConsumer, JsonOrderWithConsumer>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)))
                .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));

            Mapper.CreateMap<JsonOrderWithConsumer, OrderWithConsumer>()
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)));
            
            Mapper.CreateMap<OrderWithConsumer, Order>();
                
            // src = Order, dest = JsonOrder
		    Mapper.CreateMap<Order, JsonOrder>()
		        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)))
		        .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));
		    	
			// src = JsonOrder, dest = Order
		    Mapper.CreateMap<JsonOrder, Order>()
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)));
                
            // src = Order, dest = JsonOrder
            Mapper.CreateMap<Order, JsonOrderToPut>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
                .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonOrderToPut, Order>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.DoshiiId, opt => opt.Ignore())
             .ForMember(dest => dest.Type, opt => opt.Ignore())
             .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
             .ForMember(dest => dest.CheckinId, opt => opt.Ignore())
             .ForMember(dest => dest.LocationId, opt => opt.Ignore())
             .ForMember(dest => dest.Uri, opt => opt.Ignore())
             .ForMember(dest => dest.RequiredAt, opt => opt.Ignore());

            // src = Order, dest = JsonOrder
            Mapper.CreateMap<Order, JsonUnlinkedOrderToPut>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
                .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonUnlinkedOrderToPut, Order>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.DoshiiId, opt => opt.Ignore())
             .ForMember(dest => dest.Type, opt => opt.Ignore())
             .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
             .ForMember(dest => dest.CheckinId, opt => opt.Ignore())
             .ForMember(dest => dest.LocationId, opt => opt.Ignore())
             .ForMember(dest => dest.Uri, opt => opt.Ignore())
             .ForMember(dest => dest.RequiredAt, opt => opt.Ignore());
				
			// src = TableOrder, dest = JsonTableOrder
			Mapper.CreateMap<TableOrder, JsonTableOrder>();

			// src = JsonTableOrder, dest = TableOrder
			Mapper.CreateMap<JsonTableOrder, TableOrder>();

            // src = Order, dest = JsonOrder
            Mapper.CreateMap<OrderWithNoPriceProperties, JsonOrder>()
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)))
                .ForMember(dest => dest.Surcounts, opt => opt.Ignore());

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonOrder, OrderWithNoPriceProperties>()
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)));

            // src = Order, dest = JsonOrder
            Mapper.CreateMap<OrderWithNoPriceProperties, Order>()
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)))
                .ForMember(dest => dest.Surcounts, opt => opt.Ignore());

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<Order, OrderWithNoPriceProperties>()
                .ForMember(dest => dest.RequiredAt, opt => opt.MapFrom(src => AutoMapperConfigurator.ToLocalTime(src.RequiredAt)));

            // src = Order, dest = JsonOrder
            Mapper.CreateMap<Order, JsonOrderIdSimple>();

            // src = JsonOrder, dest = Order
            Mapper.CreateMap<JsonOrderIdSimple, Order>()
             .ForMember(dest => dest.DoshiiId, opt => opt.Ignore())
             .ForMember(dest => dest.Type, opt => opt.Ignore())
             .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
             .ForMember(dest => dest.CheckinId, opt => opt.Ignore())
             .ForMember(dest => dest.LocationId, opt => opt.Ignore())
             .ForMember(dest => dest.Uri, opt => opt.Ignore())
             .ForMember(dest => dest.RequiredAt, opt => opt.Ignore())
             .ForMember(dest => dest.Items, opt => opt.Ignore())
             .ForMember(dest => dest.MemberId, opt => opt.Ignore())
             .ForMember(dest => dest.Status, opt => opt.Ignore())
             .ForMember(dest => dest.Surcounts, opt => opt.Ignore())
             .ForMember(dest => dest.Version, opt => opt.Ignore())
             .ForMember(dest => dest.Phase, opt => opt.Ignore());
		}

		/// <summary>
		/// Converts the supplied <paramref name="dateTime"/> string into the equivalent struct value.
		/// This relies upon <paramref name="dateTime"/> being non-null and non-empty, as well as it having the appropriate date/time format.
		/// </summary>
		/// <param name="dateTime">The date/time string to be converted.</param>
		/// <returns>The struct value equivalent if <paramref name="dateTime"/> is of the required format; 
		/// or <c>DateTime.MinValue</c> otherwise.</returns>
		/// <seealso cref="DoshiiDotNetIntegration.Helpers.AutoMapperConfigurator.DateTimeFormat"/>
		private static DateTime MapDateTime(string dateTime)
		{
			if (!String.IsNullOrEmpty(dateTime))
			{
				DateTime result;
				if (DateTime.TryParseExact(dateTime, AutoMapperConfigurator.DateTimeFormat, CultureInfo.CurrentCulture, 
					DateTimeStyles.AssumeUniversal, out result))
					return result;
			}

			return DateTime.MinValue;
		}

	    /// <summary>
		/// Converts the supplied <paramref name="cents"/> integer string into a decimal monetary value.
		/// This relies upon <paramref name="cents"/> being a non-empty string representation of a number of cents.
		/// The result will contain dollars and cents representation.
		/// </summary>
		/// <example>
		/// <code>
		/// MapCurrency("179"); // returns 1.79
		/// MapCurrency("-63"); // returns -0.63
		/// MapCurrency("13201"); // returns 132.01
		/// </code>
		/// </example>
		/// <param name="cents">The number of cents to be converted.</param>
		/// <returns>The decimal dollars / cents representation of the supplied <paramref name="cents"/> string; 
		/// or <c>0.0</c> if the function fails to convert the supplied string.</returns>
		private static decimal MapCurrency(string cents)
		{
			if (!String.IsNullOrEmpty(cents))
			{
				decimal result;
			    if (Decimal.TryParse(cents, out result))
			    {
			        return result/AutoMapperConfigurator.CentsPerDollar;
			    }
			    else
			    {
			        throw new NotValidCurrencyAmountException(string.Format("{0} cannot be converted into a decimal amount.", cents));
			    }
			}
            return 0.0M;
		}


        private static decimal MapPercentage(string percentage)
        {
            if (!String.IsNullOrEmpty(percentage))
            {
                decimal result;
                if (Decimal.TryParse(percentage, out result))
                {
                    return result;
                }
                else
                {
                    throw new NotValidCurrencyAmountException(string.Format("{0} cannot be converted into a decimal amount.", percentage));
                }
            }
            return 0.0M;
        }

		/// <summary>
		/// Converts the supplied <paramref name="amount"/> into an integer representation of the number of cents.
		/// </summary>
		/// <remarks>
		/// Note that any decimals after the whole cents are truncated rather than rounded by this function.
		/// </remarks>
		/// <example>
		/// <code>
		/// MapCurrencyToString(1.79); // returns "179"
		/// MapCurrencyToString(-0.63); // returns "63"
		/// MapCurrencyToString(132.01); // returns "13201"
		/// </code>
		/// </example>
		/// <param name="amount">The dollar and cents value to be converted.</param>
		/// <returns>The string representation of the number of cents represented by <paramref name="amount"/>.</returns>
		private static string MapCurrencyToString(decimal amount)
		{
			decimal result = (decimal)Math.Floor(amount * AutoMapperConfigurator.CentsPerDollar);
			return result.ToString();
		}

        private static string MapPercentageToString(decimal amount)
        {
            return amount.ToString();
        }

        private static string MapQuantityToString(decimal quantity)
        {
            return quantity.ToString();
        }

        private static decimal MapQuantity(string quantity)
        {
            if (!String.IsNullOrEmpty(quantity))
            {
                decimal result;
                if (Decimal.TryParse(quantity, out result))
                {
                    return result;
                }
                else
                {
                    throw new NotValidCurrencyAmountException(string.Format("{0} cannot be converted into a decimal quantity.", quantity));
                }
            }
            
            return 0.0M;
        }

	    private static DateTime? ToLocalTime(DateTime? utcTime)
	    {
	        if (utcTime == null)
	        {
	            return null;
	        }
	        else
	        {
	            DateTime localTime = (DateTime) utcTime;
	            return localTime.ToLocalTime();
	        }
            
	    }

        private static DateTime? ToUtcTime(DateTime? localTime)
        {
            if (localTime == null)
            {
                return null;
            }
            else
            {
                DateTime utcTime = (DateTime)localTime;
                return utcTime.ToUniversalTime();
            }

        }

        private static string MapSurchargeAmountToString(decimal value, string rewardType)
        {
            decimal result;
            if (rewardType == "absolute")
            {
                return MapCurrencyToString(value);
            }
            else
            {
                return MapPercentageToString(value);
            }
        }

        private static decimal MapSurchargeAmountToDouble(string value, string rewardType)
        {
            if (rewardType == "absolute")
            {
                return MapCurrency(value);
            }
            else
            {
                return MapPercentage(value);
            }
        }

        private static int MapStringToInteger(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                int result;
                if (int.TryParse(value, out result))
                {
                    return result;
                }
                else
                {
                    throw new NotValidCurrencyAmountException(string.Format("{0} cannot be converted into a decimal amount.", value));
                }
            }
            return 0;
        }

        private static string MapIntegerToString(int value)
        {
            return value.ToString();
        }
	}
}
