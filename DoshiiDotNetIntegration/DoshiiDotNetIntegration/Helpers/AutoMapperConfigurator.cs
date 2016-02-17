using AutoMapper;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Globalization;
using System.Linq;

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
				AutoMapperConfigurator.MapOrderObjects();

				AutoMapperConfigurator.IsConfigured = true;
			}
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Variants model object and its
		/// JSON equivalent data transfer object.
		/// </summary>
		private static void MapVariantsObjects()
		{
			// Mapping from Variants to JsonVariants
			// src = Variants, dest = JsonVariants, opt = Mapping Option
			Mapper.CreateMap<Variants, JsonVariants>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Price)));

			// src = JsonVariants, dest = Variants
			Mapper.CreateMap<JsonVariants, Variants>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrency(src.Price)));
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Product model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapProductObjects()
		{
			// src = ProductOptions, dest = JsonProductOptions
			Mapper.CreateMap<ProductOptions, JsonProductOptions>()
				.ForMember(dest => dest.SerializeSelected, opt => opt.Ignore())
				.ForMember(dest => dest.Selected, opt => opt.MapFrom(src => src.Selected.ToList<Variants>()))
				.ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.ToList<Variants>()));

			// src = JsonProductOptions, dest = ProductOptions
			Mapper.CreateMap<JsonProductOptions, ProductOptions>();

			// src = Product, dest = JsonProduct
			Mapper.CreateMap<Product, JsonProduct>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Price)))
				.ForMember(dest => dest.ProductOptions, opt => opt.MapFrom(src => src.ProductOptions.ToList<ProductOptions>()))
				.ForMember(dest => dest.SerializeRejectionReason, opt => opt.Ignore())
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList<string>()));

			// src = JsonProduct, dest = Product
			Mapper.CreateMap<JsonProduct, Product>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrency(src.Price)));
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Surcount model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapSurcountObjects()
		{
			// src = Surcount, dest = JsonSurcount
			Mapper.CreateMap<Surcount, JsonSurcount>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.Price)));

			// src = JsonSurcount, dest = Surcount
			Mapper.CreateMap<JsonSurcount, Surcount>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrency(src.Price)));
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Transaction model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapTransactionObjects()
		{
			// src = Transaction, dest = JsonTransaction
			Mapper.CreateMap<Transaction, JsonTransaction>()
				.ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrencyToString(src.PaymentAmount)));

			// src = JsonTransaction, dest = Transaction
			Mapper.CreateMap<JsonTransaction, Transaction>()
				.ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => AutoMapperConfigurator.MapCurrency(src.PaymentAmount)));
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
		}

		/// <summary>
		/// This function creates a bi-directional object mapping between the Order model objects and their
		/// JSON equivalent data transfer objects.
		/// </summary>
		private static void MapOrderObjects()
		{
			// src = Order, dest = JsonOrder
		    Mapper.CreateMap<Order, JsonOrder>()
		        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
		        .ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));
				
			// src = JsonOrder, dest = Order
		    Mapper.CreateMap<JsonOrder, Order>();
				
			// src = OrderToPut, dest = JsonOrderToPut
			Mapper.CreateMap<OrderToPut, JsonOrderToPut>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.ToList<Product>()))
				.ForMember(dest => dest.Surcounts, opt => opt.MapFrom(src => src.Surcounts.ToList<Surcount>()));

			// src = JsonOrderToPut, dest = OrderToPut
			Mapper.CreateMap<JsonOrderToPut, OrderToPut>();

			// src = TableOrder, dest = JsonTableOrder
			Mapper.CreateMap<TableOrder, JsonTableOrder>();

			// src = JsonTableOrder, dest = TableOrder
			Mapper.CreateMap<JsonTableOrder, TableOrder>();
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
				int result;
				if (Int32.TryParse(cents, out result))
					return result / AutoMapperConfigurator.CentsPerDollar;
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
			int result = (int)Math.Floor(amount * AutoMapperConfigurator.CentsPerDollar);
			return result.ToString();
		}
	}
}
