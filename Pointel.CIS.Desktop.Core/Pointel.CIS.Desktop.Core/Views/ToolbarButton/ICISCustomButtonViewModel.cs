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
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Pointel.CIS.Desktop.Core.Commands;

namespace Pointel.CIS.Desktop.Core.Views.ToolbarButton
{ /// <summary>
    /// Comment: CIS CustomButtonViewModel
    /// Last Modified: 13-Apr-2016
    /// Created by: Pointel Inc
    /// </summary>
    public interface ICISCustomButtonViewModel
    {
        CIS.CISSettings CISObject { get; set; }
        IInteraction Interaction
        {
            get;
            set;
        }

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


    }
}
