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
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.CIS.Desktop.Core.Commands
{
     /// <summary>
    /// Comment: Specifies call details
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    public class MyCallDetails
    {
        #region Constructor
        public MyCallDetails(KeyValueCollection userData, string connectionId, object customerScreenObject)
        {
            calldata = userData;
            connectionID = connectionId;
            screenobject = customerScreenObject;
        } 
        #endregion

        #region Properties
        KeyValueCollection calldata;
        public KeyValueCollection CallData
        {
            get { return calldata; }
            set { if (calldata != null) { calldata = value; } }
        }

        string connectionID;
        public string ConnID
        {
            get { return connectionID; }
            set { if (connectionID != null) { connectionID = value; } }
        }

        object screenobject;
        public object ScreenObject
        {
            get { return screenobject; }
            set { if (screenobject != null) { screenobject = value; } }
        } 
        #endregion
    }
}
