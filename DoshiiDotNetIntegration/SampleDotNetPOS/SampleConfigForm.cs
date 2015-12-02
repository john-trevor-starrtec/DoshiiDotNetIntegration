using DoshiiDotNetIntegration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleDotNetPOS
{
	public partial class SampleConfigForm : Form
	{
		public SampleConfigForm()
		{
			InitializeComponent();
		}


		public Configuration Display(Configuration configuration)
		{
			InitialiseView(configuration);

			if (ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return ReadView();
			else
				return configuration;
		}


		private void InitialiseView(Configuration config)
		{
			cbCheckoutOnPaid.Checked = config.CheckoutOnPaid;
			cbDeallocateTableOnPaid.Checked = config.DeallocateTableOnPaid;
		}


		private Configuration ReadView()
		{
			var config = new Configuration();
			config.CheckoutOnPaid = cbCheckoutOnPaid.Checked;
			config.DeallocateTableOnPaid = cbDeallocateTableOnPaid.Checked;
			return config;
		}
	}
}
