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


		public void Display()
		{
			InitialiseView();

			if (ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return;
			else
				return;
		}


		private void InitialiseView()
		{
		}


		
	}
}
