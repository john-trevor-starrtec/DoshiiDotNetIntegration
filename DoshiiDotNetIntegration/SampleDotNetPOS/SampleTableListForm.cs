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
    public partial class SampleTableListForm : Form
    {
        /// <summary>
        /// The presenter controlling the view.
        /// </summary>
        private SampleDotNetPOSPresenter mPresenter;

        public SampleTableListForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Attaches the supplied <paramref name="presenter"/> to the view.
        /// </summary>
        /// <param name="presenter">The presenter to control the view.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the supplied <paramref name="presenter"/> is <c>null</c> -
        /// use a call to <see cref="RemovePresenter()"/> to remove the reference to the presenter instead.</exception>
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
        /// Adds a table to the list.
        /// </summary>
        /// <param name="table"></param>
        public void AddTable(Table table)
        {
            int index = dgvBookings.Rows.Add(
                    table.Name,
                    table.Covers,
                    table.IsActive,
                    table.Criteria.IsCommunal,
                    table.Criteria.CanMerge,
                    table.Criteria.IsSmoking,
                    table.Criteria.IsOutdoor
                );
            dgvBookings.Rows[index].Tag = table;
        }

        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            var table = mPresenter.SendTable();
            if (table != null)
                AddTable(table);
        }

        private void dgvBookings_SelectionChanged(object sender, EventArgs e)
        {
            btnEditTable.Enabled = dgvBookings.SelectedRows != null && dgvBookings.SelectedRows.Count > 0;
            btnDeleteTable.Enabled = btnEditTable.Enabled;
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows != null && dgvBookings.SelectedRows.Count > 0)
            {
                var table = mPresenter.UpdateTable(dgvBookings.SelectedRows[0].Tag as Table);
                if (table != null)
                {
                    dgvBookings.SelectedRows[0].Cells[0].Value = table.Name;
                    dgvBookings.SelectedRows[0].Cells[1].Value = table.Covers;
                    dgvBookings.SelectedRows[0].Cells[2].Value = table.IsActive;
                    dgvBookings.SelectedRows[0].Cells[3].Value = table.Criteria.IsCommunal;
                    dgvBookings.SelectedRows[0].Cells[4].Value = table.Criteria.CanMerge;
                    dgvBookings.SelectedRows[0].Cells[5].Value = table.Criteria.IsSmoking;
                    dgvBookings.SelectedRows[0].Cells[6].Value = table.Criteria.IsOutdoor;
                    dgvBookings.SelectedRows[0].Tag = table;
                }
            }
        }
    }
}
