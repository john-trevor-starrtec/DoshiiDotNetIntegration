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
    public partial class SampleTableForm : Form
    {
        /// <summary>
        /// The current application presenter.
        /// </summary>
        private SampleDotNetPOSPresenter mPresenter;

        public SampleTableForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Attaches the presenter to the view.
        /// </summary>
        /// <param name="presenter">The presenter to be attached.</param>
        public void AttachPresenter(SampleDotNetPOSPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException("presenter");
            mPresenter = presenter;
        }

        /// <summary>
        /// Removes the presenter from the view.
        /// </summary>
        public void RemovePresenter()
        {
            mPresenter = null;
        }

        /// <summary>
        /// Updates the view with the table details.
        /// </summary>
        /// <param name="order">The table details to be displayed.</param>
        public void UpdateView(Table table)
        {
            tbxName.Text = table.Name;
            tbxMaxCovers.Text = table.Covers.ToString();
            cbxIsActive.Checked = table.IsActive;
            cbxIsCommunal.Checked = table.Criteria.IsCommunal;
            cbxCanMerge.Checked = table.Criteria.CanMerge;
            cbxIsOutdoor.Checked = table.Criteria.IsOutdoor;
            cbxIsSmoking.Checked = table.Criteria.IsSmoking;
            //lblUri.Text = table.uri;
        }

        /// <summary>
        /// Reads back the details of the order as entered in the view.
        /// </summary>
        /// <returns>The table details currently displayed in the view.</returns>
        public Table ReadForm()
        {
            var table = new Table();

            table.Name = tbxName.Text;
            table.Covers = int.Parse(tbxMaxCovers.Text);
            table.IsActive = cbxIsActive.Checked;
            table.Criteria.IsCommunal = cbxIsCommunal.Checked;
            table.Criteria.CanMerge = cbxCanMerge.Checked;
            table.Criteria.IsOutdoor = cbxIsOutdoor.Checked;
            table.Criteria.IsSmoking = cbxIsSmoking.Checked;
            return table;
        }

 
    }
}
