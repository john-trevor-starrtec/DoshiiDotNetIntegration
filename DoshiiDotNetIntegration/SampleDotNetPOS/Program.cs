using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleDotNetPOS
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var form = new SamplePOSMainForm();
			var presenter = new SampleDotNetPOSPresenter(form);
			Application.Run(presenter.Launch());
		}
	}
}
