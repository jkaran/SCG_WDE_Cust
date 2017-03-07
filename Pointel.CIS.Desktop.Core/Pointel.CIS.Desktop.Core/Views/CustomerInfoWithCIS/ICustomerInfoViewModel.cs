using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Pointel.CIS.Desktop.Core.Commands;

/*
 Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using System.Collections.ObjectModel;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfo
{
    /// <summary>
    /// Comment: Interface for CustomerInfoViewModel
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    public interface ICustomerInfoViewModel
    {
        ICase Case { get; set; }

        string BorderThickness { get; set; }

        string WindowTitle { get; set; }

        string ConnectionID { get; set; }

        ObservableCollection<IMyListItem> CallInfoDataCollection { get; set; }

        ObservableCollection<IMyListItem> AccountDataCollection { get; set; }

        ObservableCollection<IMyListItem> AddressDataCollection { get; set; }

        ObservableCollection<IMyListItem> PhoneDataCollection { get; set; }

        ObservableCollection<IMyListItem> IVRDataCollection { get; set; }

        ObservableCollection<IMyListItem> NoticeAmountDataCollection { get; set; }

        #region CISObject

        CIS.CISSettings CISObject
        {
            get;
            set;
        }

        #endregion CISObject

        CISCommand Update
        {
            get;
            set;
        }

        CISCommand Continue
        {
            get;
            set;
        }

        CISCommand Search
        {
            get;
            set;
        }

        #region Interaction

        Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction Interaction
        {
            get;
            set;
        }

        #endregion Interaction
    }
}