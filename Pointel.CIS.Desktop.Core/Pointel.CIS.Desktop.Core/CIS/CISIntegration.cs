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

using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Infrastructure.ExceptionAnalyze;
using Genesyslab.Platform.Commons.Collections;
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Collections.Generic;

namespace Pointel.CIS.Desktop.Core.CIS
{
    /// <summary>
    /// Comment: Provides Search CIS Account method
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class CISIntegration
    {
        #region Field Declaration

        private Log logger;
        private Settings commonSettings = null;

        #endregion Field Declaration

        #region Constructor

        public CISIntegration()
        {
            logger = Log.GenInstance();
            commonSettings = Settings.GetInstance();
        }

        #endregion Constructor

        #region SearchCISAccount

        public void SearchCISAccount(string connID)
        {
            try
            {
                logger.Info("CIS account Search entry, ConnId:" + connID);
                if (string.IsNullOrEmpty(connID))
                    logger.Error("Given ConnId is Null");
                else
                {
                    logger.Info("CISCallDetails Count " + CISSettings.GetInstance().CISCallDetails.Count.ToString());
                    foreach (KeyValuePair<string, KeyValueCollection> callDetail in CISSettings.GetInstance().CISCallDetails)
                    {
                        logger.Info("Call Details " + callDetail.Key);
                        if (callDetail.Key == connID)
                        {
                            logger.Info("CIS Search Result " + CISSettings.GetInstance().CISConnection.CISConnect(GetParameterValue("1", "continue", callDetail.Value),
                                GetParameterValue("2", "continue", callDetail.Value), GetParameterValue("3", "continue", callDetail.Value), GetParameterValue("4", "continue", callDetail.Value)));
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("SearchCISAccount: Error occured on CISConnect Invoking, Exception: " + generalException.ToString());
                if (Pointel.CIS.Desktop.Core.Util.Settings.GetInstance().Enable_CIS_Error_Toaster)
                    new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                        "CIS Search : " + generalException.Message, null, null);
            }
        }

        #endregion SearchCISAccount

        #region GetParameterValue

        private string GetParameterValue(string searchIndex, string searchType, KeyValueCollection userData)
        {
            string result = string.Empty;
            try
            {
                switch (searchIndex)
                {
                    case "1":
                        if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.user-data.key-name." + searchIndex))
                        {
                            result = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)) ?
                               Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)]) : string.Empty);
                        }
                        else if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.default.value." + searchIndex))
                        {
                            result = commonSettings.CISConfiguration.GetAsString(searchType + ".search.default.value." + searchIndex);
                        }
                        break;

                    case "2":
                        if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.user-data.key-name." + searchIndex))
                        {
                            result = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)) ?
                               Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)]) : string.Empty);
                        }
                        else if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.default.value." + searchIndex))
                        {
                            result = commonSettings.CISConfiguration.GetAsString(searchType + ".search.default.value." + searchIndex);
                        }
                        break;

                    case "3":
                        if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.user-data.key-name." + searchIndex))
                        {
                            result = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)) ?
                               Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)]) : string.Empty);
                        }
                        else if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.default.value." + searchIndex))
                        {
                            result = commonSettings.CISConfiguration.GetAsString(searchType + ".search.default.value." + searchIndex);
                        }
                        break;

                    case "4":
                        if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.user-data.key-name." + searchIndex))
                        {
                            result = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)) ?
                               Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString(searchType + ".search.user-data.key-name." + searchIndex)]) : string.Empty);
                        }
                        else if (commonSettings.CISConfiguration.ContainsKey(searchType + ".search.default.value." + searchIndex))
                        {
                            result = commonSettings.CISConfiguration.GetAsString(searchType + ".search.default.value." + searchIndex);
                        }
                        break;
                }

                logger.Info("***************************************************************");
                logger.Info("Parameter " + searchIndex + ":  " + result);
                logger.Info("***************************************************************");
            }
            catch (Exception generalException)
            {
                logger.Error("Error on creating Query String " + generalException.ToString());
            }
            return result;
        }

        #endregion GetParameterValue
    }
}