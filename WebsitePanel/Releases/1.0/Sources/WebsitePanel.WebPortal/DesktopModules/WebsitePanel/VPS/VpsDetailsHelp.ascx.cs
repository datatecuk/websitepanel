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

﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebsitePanel.EnterpriseServer;
using WebsitePanel.Providers.Common;

namespace WebsitePanel.Portal.VPS
{
    public partial class VpsDetailsHelp : WebsitePanelModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSummaryInfo();
            }
        }

        private void BindSummaryInfo()
        {
            // bind user details
            PackageInfo package = ES.Services.Packages.GetPackage(PanelSecurity.PackageId);
            if (package != null)
            {
                UserInfo user = ES.Services.Users.GetUserById(package.UserId);
                if (user != null)
                {
                    txtTo.Text = user.Email;
                }
            }

            // load template
            string content = ES.Services.VPS.GetVirtualMachineSummaryText(PanelRequest.ItemID);
            if (content != null)
                litContent.Text = content;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                ResultObject res = ES.Services.VPS.SendVirtualMachineSummaryLetter(
                    PanelRequest.ItemID, txtTo.Text.Trim(), txtBCC.Text.Trim());

                if (res.IsSuccess)
                {
                    // bind tree
                    messageBox.ShowSuccessMessage("VPS_ERROR_SEND_SUMMARY_LETTER");
                    return;
                }
                else
                {
                    // show error
                    messageBox.ShowMessage(res, "ERROR_SEND_SUMMARY_LETTER", "VPS");
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("VPS_ERROR_SEND_SUMMARY_LETTER", ex);
            }
        }
    }
}