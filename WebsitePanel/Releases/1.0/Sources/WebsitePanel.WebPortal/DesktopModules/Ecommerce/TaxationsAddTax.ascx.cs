// Copyright (c) 2010, SMB SAAS Systems Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  SMB SAAS Systems Inc.  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using WebsitePanel.Portal;
using WebsitePanel.Ecommerce.EnterpriseServer;

namespace WebsitePanel.Ecommerce.Portal
{
	public partial class TaxationsAddTax : ecModuleBase
	{
		public const string ALL_COUNTRIES = "*";
        public const string ALL_STATES = "*";

		const string CurrencyValidationType = "1";
		const string PercentageValidationType = "2,3";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				SetTaxAmountValidationType(ddlTaxType.SelectedValue);
				// countries
				PortalUtils.LoadCountriesDropDownList(ddlCountries, String.Empty);
				//
				LoadCountryStates();
			}
		}

		protected void ddlTaxType_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTaxAmountValidationType(ddlTaxType.SelectedValue);
		}

        protected void chkAllStates_CheckedChanged(object sender, EventArgs e)
        {
			plStateProvince.Visible = !chkAllStates.Checked;
            reqStateProvince.Enabled = !chkAllStates.Checked;
        }

		protected void btnAddTax_Click(object sender, EventArgs e)
		{
			AddTax();
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			RedirectToBrowsePage();
		}

		protected void ddlCountries_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadCountryStates();
		}

		private void SetTaxAmountValidationType(string validationType)
		{
			PercentageAmountCompareValidator.Enabled = PercentageValidationType.Contains(validationType);
			FixedAmountCompareValidator.Enabled = CurrencyValidationType.Contains(validationType);
		}

		private void LoadCountryStates()
		{
			PortalUtils.LoadStatesDropDownList(ddlStates, ddlCountries.SelectedValue);
			//
			if (ddlStates.Items.Count > 1)
			{
				ddlStates.Visible = true;
				//
				reqStateProvince.ControlToValidate = ddlStates.ID;
				//
				txtStateProvince.Visible = false;
			}
			else
			{
				ddlStates.Visible = false;
				//
				reqStateProvince.ControlToValidate = txtStateProvince.ID;
				//
				txtStateProvince.Visible = true;
			}
			//
			chkAllStates.Checked = String.Equals(ddlCountries.SelectedValue, ALL_COUNTRIES);
			chkAllStates_CheckedChanged(null, EventArgs.Empty);
		}

		private void AddTax()
		{
			// ensure page validity
			if (!Page.IsValid)
				return;

			try
			{
				//
				string description = txtDescription.Text.Trim();
				//
				string country = ddlCountries.SelectedValue;
				//
                string state = String.Empty;
                if (chkAllStates.Checked)
                    state = ALL_STATES;
                else
				    state = (ddlStates.Visible) ? ddlStates.SelectedValue : txtStateProvince.Text.Trim();
				//
				int taxTypeId = Convert.ToInt32(ddlTaxType.SelectedValue);
				//
				bool active = Convert.ToBoolean(rblTaxStatus.SelectedValue);
				//
				decimal amount = Convert.ToDecimal(txtTaxAmount.Text.Trim());
				//
				int result = StorehouseHelper.AddTaxation(country, state, description, taxTypeId, 
					amount, active);
				//
				if (result < 0)
				{
					//
					ShowResultMessage(Keys.ModuleName, result, country, state);
					//
					return;
				}
				//
				RedirectToBrowsePage();
			}
			catch (Exception ex)
			{
				ShowErrorMessage("SAVE_TAX", ex);
			}
		}
	}
}