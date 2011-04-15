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

namespace WebsitePanel.Portal
{
    /// <summary>
    /// Summary description for ReportsHelper
    /// </summary>
    public class ReportsHelper
    {
        #region Bandwidth Report
        DataSet dsBandwidthReport;

        public int GetPackagesBandwidthPagedCount(int packageId, string sStartDate, string sEndDate)
        {
            return (int)dsBandwidthReport.Tables[0].Rows[0][0];
        }

        public DataTable GetPackagesBandwidthPaged(int packageId, int maximumRows, int startRowIndex, string sortColumn,
            string sStartDate, string sEndDate)
        {

            dsBandwidthReport = ES.Services.Packages.GetPackagesBandwidthPaged(PanelSecurity.SelectedUserId,
                packageId, DateTime.Parse(sStartDate), DateTime.Parse(sEndDate),
                sortColumn, startRowIndex, maximumRows);
            return dsBandwidthReport.Tables[1];
        }
        #endregion

        #region DiskSpace Report
        DataSet dsDiskspaceReport;

        public int GetPackagesDiskspacePagedCount(int packageId)
        {
            return (int)dsDiskspaceReport.Tables[0].Rows[0][0];
        }

        public DataTable GetPackagesDiskspacePaged(int packageId, int maximumRows, int startRowIndex, string sortColumn)
        {

            dsDiskspaceReport = ES.Services.Packages.GetPackagesDiskspacePaged(
                PanelSecurity.SelectedUserId, packageId,
                sortColumn, startRowIndex, maximumRows);
            return dsDiskspaceReport.Tables[1];
        }
        #endregion
    }
}