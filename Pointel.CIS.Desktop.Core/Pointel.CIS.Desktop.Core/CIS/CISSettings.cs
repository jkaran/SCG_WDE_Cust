using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Infrastructure.ExceptionAnalyze;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using System.Collections.Generic;

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

namespace Pointel.CIS.Desktop.Core.CIS
{
    /// <summary>
    /// Comment: Contains CIS setting details Last Modified: 15-Feb-2015 Created by: Pointel Inc
    /// </summary>
    public class CISSettings
    {
        #region Field Declaration

        private static CISSettings cisSettings = null;
        private ILogger _logger;
        private Dictionary<string, KeyValueCollection> callDetails = null;
        private AxCISIVRConnection.AxCISIVRConn cisConnection;

        #endregion Field Declaration

        #region Constructor

        private CISSettings()
        {
            _logger = ContainerAccessPoint.Container.Resolve<ILogger>();
        }

        #endregion Constructor

        #region GetInstance

        public static CISSettings GetInstance()
        {
            if (cisSettings == null)
            {
                cisSettings = new CISSettings();
            }
            return cisSettings;
        }

        #endregion GetInstance

        #region Properties

        public Dictionary<string, KeyValueCollection> CISCallDetails
        {
            get { return callDetails; }
            set { callDetails = value; }
        }

        public AxCISIVRConnection.AxCISIVRConn CISConnection
        {
            get
            {
                try
                {
                    return (cisConnection == null ? cisConnection = new AxCISIVRConnection.AxCISIVRConn() : cisConnection);
                }
                catch (System.Exception generalException)
                {
                    _logger.Error("CISSettings : Unable to access the CIS Integration COM Object" + generalException.ToString());
                    if (Pointel.CIS.Desktop.Core.Util.Settings.GetInstance().Enable_CIS_Error_Toaster)
                        new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                         "Unable to access the CIS Integration COM Object", null, null);
                    return null;
                }
            }
        }

        #endregion Properties
    }
}