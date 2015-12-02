using AutoMapper;
using NUnit.Framework;
using DoshiiDotNetIntegration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetSDKTests
{
	/// <summary>
	/// Test fixture for the <see cref="DoshiiDotNetIntegration.Helpers.AutoMapperConfigurator"/> class.
	/// </summary>
	[TestFixture]
	public class AutoMapperConfiguratorTest
	{
		/// <summary>
		/// Tests the configuration of the AutoMapper.
		/// </summary>
		[Test]
		public void Configure()
		{
			AutoMapperConfigurator.Configure();
			Mapper.AssertConfigurationIsValid();
		}
	}
}
