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

using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Collections.Generic;

namespace Pointel.CIS.Desktop.Core.Configuration
{
    /// <summary>
    /// Comment: Reads the CME configuration
    /// Last Modified: 20-Jun-2016
    /// Created by: Pointel Inc
    /// </summary>
    internal class ReadConfigurationData
    {
        #region Field Declaration

        private Log logger;
        private Settings commonSettings = null;

        #endregion Field Declaration

        #region Constructor

        public ReadConfigurationData()
        {
            logger = Log.GenInstance();
            commonSettings = Settings.GetInstance();
        }

        #endregion Constructor

        #region ReadCISIntegrationConfigurations

        public KeyValueCollection ReadCISIntegrationConfigurations(IAgent agent, CfgApplication application)
        {
            KeyValueCollection CISData = null;
            try
            {
                logger.Info("Reading cis-integration Configuration");

                logger.Info("Check application level configuration");
                if (application != null)
                {
                    //Reading Agent Level Configurations
                    if (application.Options.ContainsKey("cis-integration"))
                    {
                        CISData = application.Options.GetAsKeyValueCollection("cis-integration");
                        this.logger.Info((CISData != null ? "Read Configuration at Application Level" : " No configuration found at Application Level"));
                    }
                    else
                    {
                        this.logger.Info("CIS Integration Configuration is not found at Application Level.");
                    }

                    //Reading AgentGroupLevel Configurations
                    if (agent.AgentGroupsForAgent != null)
                    {
                        foreach (CfgAgentGroup agentgroup in agent.AgentGroupsForAgent)
                        {
                            if (agentgroup.GroupInfo.UserProperties.ContainsKey("cis-integration"))
                            {
                                KeyValueCollection tempconfig = agentgroup.GroupInfo.UserProperties.GetAsKeyValueCollection("cis-integration");
                                if (tempconfig != null)
                                {
                                    if (CISData != null)
                                    {
                                        foreach (string key in tempconfig.AllKeys)
                                        {
                                            if (CISData.ContainsKey(key))
                                            {
                                                CISData.Set(key, tempconfig.GetAsString(key));
                                            }
                                            else
                                            {
                                                CISData.Add(key, tempconfig.GetAsString(key));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CISData = tempconfig;
                                    }
                                    logger.Info("CIS Integration configuration taken from Agent Group : " + agentgroup.GroupInfo.Name);
                                    break;
                                }
                            }
                            else
                            {
                                logger.Info("No CIS Integration configuration found at Agent Group : " + agentgroup.GroupInfo.Name);
                            }
                        }
                    }

                    //Reading Agent Level Configurations
                    if (agent.ConfPerson.UserProperties.ContainsKey("cis-integration"))
                    {
                        KeyValueCollection tempconfig = agent.ConfPerson.UserProperties.GetAsKeyValueCollection("cis-integration");
                        if (tempconfig != null)
                        {
                            if (CISData != null)
                            {
                                foreach (string key in tempconfig.AllKeys)
                                {
                                    if (CISData.ContainsKey(key))
                                    {
                                        CISData.Set(key, tempconfig.GetAsString(key));
                                    }
                                    else
                                    {
                                        CISData.Add(key, tempconfig.GetAsString(key));
                                    }
                                }
                            }
                            else
                            {
                                CISData = tempconfig;
                            }
                        }
                    }
                    else
                    {
                        logger.Info("No cis Integration configuration found at Agent Level");
                    }
                }
                else
                    logger.Info("No Configuration at application level");

                if (CISData != null)
                    logger.Info("CIS Configuration \n" + CISData.ToString());
                else
                    logger.Info("CIS Configuration not found");
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while reading CIS Configuration " + generalException.ToString());
            }

            return CISData;
        }

        #endregion ReadCISIntegrationConfigurations

        #region Initialize Properties

        public void IntializeCISProperty()
        {
            try
            {
                logger.Info("CIS Properties Reading configuration...");
                if (commonSettings.CISConfiguration != null)
                {
                    if (ValidateKeys("popup.event.calltype"))
                    {
                        commonSettings.PopupCallType = commonSettings.CISConfiguration.GetAsString("popup.event.calltype").ToLower();
                        if (commonSettings.PopupCallType.Contains("inbound"))
                            commonSettings.PopupInboundCallType = true;
                        if (commonSettings.PopupCallType.Contains("consult"))
                            commonSettings.PopupConsultCallType = true;
                        if (commonSettings.PopupCallType.Contains("outbound"))
                            commonSettings.PopupOutboundCallType = true;
                    }
                    else
                    {
                        commonSettings.PopupCallType = "inbound,consult,outbound";
                        commonSettings.PopupInboundCallType = true;
                        commonSettings.PopupConsultCallType = true;
                        commonSettings.PopupOutboundCallType = true;
                    }

                    if (ValidateKeys("popup.event"))
                    {
                        commonSettings.PopupEventNames = commonSettings.CISConfiguration.GetAsString("popup.event").ToLower();
                        if (commonSettings.PopupEventNames.Contains("eventringing"))
                            commonSettings.PopupOnEventRinging = true;
                        if (commonSettings.PopupEventNames.Contains("eventestablished"))
                            commonSettings.PopupOnEventEstablished = true;
                        if (commonSettings.PopupEventNames.Contains("eventdialing"))
                            commonSettings.PopupOnEventDialing = true;
                        if (commonSettings.PopupEventNames.Contains("eventheld"))
                            commonSettings.PopupOnEventHeld = true;
                        if (commonSettings.PopupEventNames.Contains("eventretrieved"))
                            commonSettings.PopupOnEventRetrived = true;
                        if (commonSettings.PopupEventNames.Contains("eventpartychanged"))
                            commonSettings.PopupOnEventPartyChanged = true;
                        if (commonSettings.PopupEventNames.Contains("eventattacheddatachanged"))
                            commonSettings.PopupOnEventAttachedDataChanged = true;
                    }

                    if (ValidateKeys("scg.attach-data.add.key-name"))
                    {
                        commonSettings.AttachKeyName = commonSettings.CISConfiguration.GetAsString("scg.attach-data.add.key-name");
                    }
                    else
                    {
                        commonSettings.AttachKeyName = "IsLeadUpdated";
                    }

                    if (string.Compare(commonSettings.CISConfiguration.GetAsString("scg.attach-data.add.enable"), "false", true) == 0)
                    {
                        commonSettings.CanAddAttachData = false;
                    }
                    else
                    {
                        commonSettings.CanAddAttachData = true;
                    }

                    if (string.Compare(commonSettings.CISConfiguration.GetAsString("manual.popup"), "false", true) == 0)
                    {
                        commonSettings.ManualPopupDisabled = true;
                    }

                    if (ValidateKeys("update.service.request.username"))
                    {
                        commonSettings.ServiceUserName = commonSettings.CISConfiguration.GetAsString("update.service.request.username");
                    }

                    if (ValidateKeys("update.service.request.url"))
                    {
                        commonSettings.RequestUrl = commonSettings.CISConfiguration.GetAsString("update.service.request.url");
                        logger.Info("SCG Webservice URL : " + commonSettings.RequestUrl);
                    }
                    if (ValidateKeys("update.service.request.password"))
                    {
                        //If no public key configured, application will take the password without decryption
                        commonSettings.ServicePassword = (ValidateKeys("update.service.request.password.public-key") ?
                            Encryption.DecryptRijndael(commonSettings.CISConfiguration.GetAsString("update.service.request.password"),
                            commonSettings.CISConfiguration.GetAsString("update.service.request.password.public-key")) :
                            commonSettings.CISConfiguration.GetAsString("update.service.request.password"));
                    }

                    if (ValidateKeys("update.service.request.operation-code"))
                    {
                        commonSettings.RequestOperationCode = Convert.ToInt16(commonSettings.CISConfiguration.GetAsString("update.service.request.operation-code"));
                    }
                    if (ValidateKeys("update.service.request.channel-type"))
                    {
                        commonSettings.RequestChannelType = commonSettings.CISConfiguration.GetAsString("update.service.request.channel-type");
                    }
                    else
                        commonSettings.RequestChannelType = "";

                    if (ValidateKeys("update.service.request.key-type"))
                    {
                        commonSettings.RequestKeyType = commonSettings.CISConfiguration.GetAsString("update.service.request.key-type");
                    }
                    else
                        commonSettings.RequestKeyType = "";

                    if (ValidateKeys("update.service.request.database-name"))
                    {
                        commonSettings.RequestDatabaseName = commonSettings.CISConfiguration.GetAsString("update.service.request.database-name");
                    }
                    else
                        commonSettings.RequestDatabaseName = "";

                    if (ValidateKeys("update.service.request.language-code"))
                    {
                        commonSettings.RequestLanguageCode = commonSettings.CISConfiguration.GetAsString("update.service.request.language-code");
                    }
                    else
                    {
                        commonSettings.RequestLanguageCode = "";
                    }
                    if (ValidateKeys("update.service.request.check-digit"))
                    {
                        commonSettings.RequestCheckDigit = commonSettings.CISConfiguration.GetAsString("update.service.request.check-digit");
                    }
                    else
                    {
                        commonSettings.RequestCheckDigit = "";
                    }

                    #region CallTypes

                    if (ValidateKeys("call-info.call-type.section-name"))
                    {
                        commonSettings.CustomerInfo_CallTypeSectionNames = commonSettings.CISConfiguration.GetAsString("call-info.call-type.section-name").Split(',');
                        if (commonSettings.CustomerInfo_CallTypeSectionNames.Length != 0)
                        {
                            foreach (string sectionname in commonSettings.CustomerInfo_CallTypeSectionNames)
                            {
                                if (sectionname == commonSettings.CISConfiguration.GetAsString("call-info.pa.call-type.section-name"))
                                {
                                    KeyValueCollection pacalltype = ReadAnnexConfiguration(commonSettings.AgentDetails, commonSettings.ApplicationDetails,
                                    commonSettings.CISConfiguration.GetAsString("call-info.pa.call-type.section-name"));
                                    if (pacalltype != null)
                                    {
                                        commonSettings.PACallType = SetListCallType(pacalltype, "key-name");
                                    }
                                    else
                                    {
                                        this.logger.Info("call-info.PACallType configuration section '" + commonSettings.CISConfiguration.GetAsString("call-info.pa.call-type.section-name") + "' not found");
                                    }
                                }
                                else if (sectionname == commonSettings.CISConfiguration.GetAsString("call-info.close.call-type.section-name"))
                                {
                                    KeyValueCollection closecalltype = ReadAnnexConfiguration(commonSettings.AgentDetails, commonSettings.ApplicationDetails,
                                        commonSettings.CISConfiguration.GetAsString("call-info.close.call-type.section-name"));
                                    if (closecalltype != null)
                                    {
                                        commonSettings.CloseCallType = SetListCallType(closecalltype, "key-name");
                                    }
                                    else
                                    {
                                        this.logger.Info("call-info.CloseCallType configuration section '" + commonSettings.CISConfiguration.GetAsString("call-info.close.call-type.section-name") + "' not found");
                                    }
                                }
                                else if (sectionname == commonSettings.CISConfiguration.GetAsString("call-info.cso.call-type.section-name"))
                                {
                                    KeyValueCollection csocalltype = ReadAnnexConfiguration(commonSettings.AgentDetails, commonSettings.ApplicationDetails,
                                        commonSettings.CISConfiguration.GetAsString("call-info.cso.call-type.section-name"));
                                    if (csocalltype != null)
                                    {
                                        commonSettings.CSOCallType = SetListCallType(csocalltype, "key-name");
                                    }
                                    else
                                    {
                                        this.logger.Info("call-info.CSOCallType configuration section '" + commonSettings.CISConfiguration.GetAsString("call-info.cso.call-type.section-name") + "' not found");
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.logger.Info("call-info.call.type is null");
                        }
                    }

                    #endregion CallTypes

                    #region LanguageColorCode

                    KeyValueCollection languageColorCode = ReadAnnexConfiguration(commonSettings.AgentDetails, commonSettings.ApplicationDetails, "call-info.color-code");
                    if (languageColorCode != null && languageColorCode.Count != 0)
                    {
                        if (languageColorCode.ContainsKey("default-color-code") && !string.IsNullOrWhiteSpace(languageColorCode.GetAsString("default-color-code")))
                            commonSettings.DefaultColor = languageColorCode["default-color-code"].ToString();
                        else
                            commonSettings.DefaultColor = "#5E656B";
                        //if (languageColorCode.ContainsKey("emergeny-color-code"))
                        //    commonSettings.EmergencyColor = languageColorCode["emergeny-color-code"].ToString();

                        //if (languageColorCode.ContainsKey("spanish-color-code"))
                        //    commonSettings.SpanishColor = languageColorCode["spanish-color-code"].ToString();

                        if (languageColorCode.ContainskeyAndValue("call-type.color-code"))
                        {
                            string[] splitBysemicolon = languageColorCode.GetAsString("call-type.color-code").Split(';');
                            if (splitBysemicolon != null && splitBysemicolon.Length > 0)
                            {
                                commonSettings.CallType_Colorinfo_List = new List<ColorInfo>();
                                foreach (string value in splitBysemicolon)
                                {
                                    try
                                    {
                                        commonSettings.CallType_Colorinfo_List.Add(new ColorInfo()
                                        {
                                            userdataKey = "",
                                            matchValues = value.Substring(0, value.IndexOf('@') - 1).Split(','),
                                            color = value.Substring(value.IndexOf('@') + 1, value.Length - value.IndexOf('@') - 1)
                                        });
                                    }
                                    catch (Exception)
                                    {
                                        this.logger.Info("call type color code format is not valid");
                                    }
                                }
                            }
                        }
                        if (languageColorCode.ContainskeyAndValue("language.color-code"))
                        {
                            string[] splitBysemicolon = languageColorCode.GetAsString("language.color-code").Split(';');
                            if (splitBysemicolon != null && splitBysemicolon.Length > 0)
                            {
                                commonSettings.Language_Colorinfo_List = new List<ColorInfo>();
                                foreach (string value in splitBysemicolon)
                                {
                                    try
                                    {
                                        commonSettings.Language_Colorinfo_List.Add(new ColorInfo()
                                        {
                                            userdataKey = value.Substring(0, value.IndexOf('/')),
                                            matchValues = value.Substring(value.IndexOf('/') + 1, value.IndexOf('@') - value.IndexOf('/') - 1).Split(','),
                                            color = value.Substring(value.IndexOf('@') + 1, value.Length - value.IndexOf('@') - 1)
                                        });
                                    }
                                    catch (Exception)
                                    {
                                        this.logger.Info("language color code format is not valid");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Info("CIS color-code are not Initialized because CIS color-code section was not found");
                        commonSettings.DefaultColor = "#5E656B";
                    }

                    #endregion LanguageColorCode

                    if (ValidateKeys("call-info.enable.display.bottom-line"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("call-info.enable.display.bottom-line")))
                            commonSettings.Enable_CallInfo_BottomLine = true;
                    }

                    #region CIS Keys Shortcuts

                    if (ValidateKeys("cis.keyboard.update-hotkey"))
                    {
                        commonSettings.CISUpdate_HotKey = commonSettings.CISConfiguration.GetAsString("cis.keyboard.update-hotkey");
                        this.logger.Info("Continue Shortcut key " + commonSettings.CISContinue_HotKey);
                    }
                    else
                    {
                        this.logger.Info("No Keyboard HotKey found for the CIS Update");
                    }
                    if (ValidateKeys("cis.keyboard.continue-hotkey"))
                    {
                        commonSettings.CISContinue_HotKey = commonSettings.CISConfiguration.GetAsString("cis.keyboard.continue-hotkey");
                        this.logger.Info("Continue Shortcut key " + commonSettings.CISContinue_HotKey);
                    }
                    else
                    {
                        this.logger.Info("No Keyboard HotKey found for the CIS Continue");
                    }
                    if (ValidateKeys("cis.keyboard.search-hotkey"))
                    {
                        commonSettings.CISSearch_HotKey = commonSettings.CISConfiguration.GetAsString("cis.keyboard.search-hotkey");
                        this.logger.Info("Continue Shortcut key " + commonSettings.CISContinue_HotKey);
                    }
                    else
                    {
                        this.logger.Info("No Keyboard HotKey found for the CIS Search");
                    }

                    #endregion CIS Keys Shortcuts

                    if (ValidateKeys("scg.agent-status.enabled-actions-global"))
                    {
                        commonSettings.AgentLevelActionCodes = commonSettings.CISConfiguration.GetAsString("scg.agent-status.enabled-actions-global");
                        this.logger.Info("scg.agent-status.enabled-actions-global : " + commonSettings.AgentLevelActionCodes);
                    }
                    else
                    {
                        //commonSettings.AgentLevelActionCodes = "ready,notready,notreadyreason,aftercallwork,dnd,logoff";
                        this.logger.Info("No Global Agent Status action configured, default value taken");
                    }

                    if (ValidateKeys("scg.agent-status.enabled-actions-by-channel"))
                    {
                        commonSettings.ChannelLevelActionCodes = commonSettings.CISConfiguration.GetAsString("scg.agent-status.enabled-actions-by-channel");
                        this.logger.Info("scg.agent-status.enabled-actions-by-channel : " + commonSettings.ChannelLevelActionCodes);
                    }
                    else
                    {
                        //commonSettings.ChannelLevelActionCodes = "ready,notready,notreadyreason,aftercallwork,dnd,logoff";
                        this.logger.Info("No Channel Level Agent Status action configured, default value taken");
                    }

                    if (ValidateKeys("scg.agent-status.not-ready-reasons"))
                    {
                        commonSettings.NotReadyActionCodeFilters = commonSettings.CISConfiguration.GetAsString("scg.agent-status.not-ready-reasons").Split(',');
                        this.logger.Info("NotReady ActionCode Filters : " + commonSettings.NotReadyActionCodeFilters);
                    }
                    else
                    {
                        this.logger.Info("NotReady ActionCode filters are not configured");
                    }
                    if (ValidateKeys("call-info.enable.voice-bar.buttons"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("call-info.enable.voice-bar.buttons")))
                            commonSettings.Enable_Voice_Bar_Buttons = true;
                    }
                    if (ValidateKeys("call-info.enable.buttons"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("call-info.enable.buttons")))
                            commonSettings.Enable_CallInfo_Buttons = true;
                    }
                    if (ValidateKeys("enable.cis.connection.error.toaster-popup"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("enable.cis.connection.error.toaster-popup")))
                            commonSettings.Enable_CIS_Error_Toaster = true;
                    }
                    if (ValidateKeys("scg.enable.format.account-number"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("scg.enable.format.account-number")))
                            commonSettings.EnableAccountNumberFormat = true;
                    }
                    if (ValidateKeys("scg.enable.agent-status.display"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("scg.enable.agent-status.display")))
                            commonSettings.EnableVoiceEmailAgentStatus = true;
                    }
                    if (ValidateKeys("scg.enable.consult.notepad"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("scg.enable.consult.notepad")))
                            commonSettings.EnableConsultNotePad = true;
                    }
                    else
                    {
                        commonSettings.EnableConsultNotePad = true;
                    }

                    // Agent Login Status Configurations
                    //***********************************************************************************************************
                    if (ValidateKeys("scg.login-status.reason-code"))
                        commonSettings.LoginStatusReasoncode = commonSettings.CISConfiguration.GetAsString("scg.login-status.reason-code");
                    else
                        commonSettings.LoginStatusReasoncode = "0";
                    if (ValidateKeys("scg.login-status.reason-name"))
                        commonSettings.LoginStatusReasonName = commonSettings.CISConfiguration.GetAsString("scg.login-status.reason-name");
                    else
                        commonSettings.LoginStatusReasonName = "UnAvailable";

                    if (ValidateKeys("scg.login-status.work-mode"))
                        commonSettings.LoginStatusWorkMode = commonSettings.CISConfiguration.GetAsString("scg.login-status.work-mode").Trim().ToLower();
                    else
                        commonSettings.LoginStatusWorkMode = "aux-work";

                    if (ValidateKeys("scg.login-status.request-attribute"))
                        commonSettings.LoginStatusRequestAttribute = commonSettings.CISConfiguration.GetAsString("scg.login-status.request-attribute").Trim().ToLower();
                    else
                        commonSettings.LoginStatusRequestAttribute = "extensions,reasons";

                    //***********************************************************************************************************
                }
                else
                    logger.Info("CIS Properties are not Initialized because CIS Configuration not found");

                logger.Info("CIS Properties : \n " + commonSettings.ToString());
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while IntializeCISProperty " + generalException.ToString());
            }
        }

        #endregion Initialize Properties

        #region ValidateKeys

        private bool ValidateKeys(string key)
        {
            if (commonSettings.CISConfiguration.ContainskeyAndValue(key))
            {
                logger.Info(key + " = " + commonSettings.CISConfiguration.GetAsString(key));
                return true;
            }
            else
            {
                logger.Info("The value for " + key + " not found in configuration");
                return false;
            }
        }

        #endregion ValidateKeys

        #region ReadAnnexConfiguration

        public KeyValueCollection ReadAnnexConfiguration(IAgent agent, CfgApplication myApplication, string sectionName)
        {
            try
            {
                logger.Info("ReadAnnexConfiguration : Reading CIS Annex Configuration");
                logger.Info("ReadAnnexConfiguration : Section Name : " + sectionName);
                KeyValueCollection CISAnnexConfig = null;

                #region Reading General Configurations

                try
                {
                    if (myApplication != null)
                    {
                        //Reading Agent Level Configurations
                        if (myApplication.UserProperties.ContainsKey(sectionName))
                        {
                            CISAnnexConfig = myApplication.UserProperties.GetAsKeyValueCollection(sectionName);
                            this.logger.Info("ReadAnnexConfiguration : " + (CISAnnexConfig != null ? "Read " + sectionName + " Configuration at Application Level" : " No configuration found at Application Level"));
                        }
                        else
                        {
                            this.logger.Info("ReadAnnexConfiguration : " + sectionName + " Configuration is not found at Application Level.");
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadAnnexConfiguration : Error occurred while reading configuration at application level :" + generalException.ToString());
                }

                //Reading AgentGroupLevel Configurartions

                try
                {
                    if (agent.AgentGroupsForAgent != null)
                    {
                        foreach (CfgAgentGroup agentgroup in agent.AgentGroupsForAgent)
                        {
                            if (agentgroup.GroupInfo.UserProperties.ContainsKey(sectionName))
                            {
                                KeyValueCollection agentGroupConfig = agentgroup.GroupInfo.UserProperties.GetAsKeyValueCollection(sectionName);
                                if (agentGroupConfig != null)
                                {
                                    if (CISAnnexConfig != null)
                                    {
                                        foreach (string key in agentGroupConfig.AllKeys)
                                        {
                                            if (CISAnnexConfig.ContainsKey(key))
                                            {
                                                CISAnnexConfig.Set(key, agentGroupConfig.GetAsString(key));
                                            }
                                            else
                                            {
                                                CISAnnexConfig.Add(key, agentGroupConfig.GetAsString(key));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CISAnnexConfig = agentGroupConfig;
                                    }
                                    logger.Info("ReadAnnexConfiguration : " + sectionName + " configuration taken from Agent Group : " + agentgroup.GroupInfo.Name);
                                    break;
                                }
                            }
                            else
                            {
                                logger.Info("ReadAnnexConfiguration : " + sectionName + " configuration is not found at Agent Group : " + agentgroup.GroupInfo.Name);
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadAnnexConfiguration : Error occurred while reading configuration at AgentGroup level :" + generalException.ToString());
                }

                //Reading Agent Level Configurations

                try
                {
                    if (agent.ConfPerson.UserProperties.ContainsKey(sectionName))
                    {
                        KeyValueCollection agentConfig = agent.ConfPerson.UserProperties.GetAsKeyValueCollection(sectionName);
                        if (agentConfig != null)
                        {
                            if (CISAnnexConfig != null)
                            {
                                foreach (string key in agentConfig.AllKeys)
                                {
                                    if (CISAnnexConfig.ContainsKey(key))
                                    {
                                        CISAnnexConfig.Set(key, agentConfig.GetAsString(key));
                                    }
                                    else
                                    {
                                        CISAnnexConfig.Add(key, agentConfig.GetAsString(key));
                                    }
                                }
                            }
                            else
                            {
                                CISAnnexConfig = agentConfig;
                            }
                        }
                    }
                    else
                    {
                        logger.Info("ReadAnnexConfiguration : " + sectionName + " configuration is not found at Agent Level");
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadAnnexConfiguration : Error occurred while reading configuration at Agent level :" + generalException.ToString());
                }

                #endregion Reading General Configurations

                if (CISAnnexConfig != null)
                    logger.Info("ReadAnnexConfiguration : " + sectionName + " Configuration Data : " + CISAnnexConfig.ToString());

                return CISAnnexConfig;
            }
            catch (Exception generalException)
            {
                logger.Error("ReadAnnexConfiguration : Error Occurred while reading CIS Annex Configuration : " + generalException.ToString());
            }
            return null;
        }

        #endregion ReadAnnexConfiguration

        #region SetCallTypeKeys

        public List<string> SetListCallType(KeyValueCollection kvp, string keyPrefix)
        {
            List<string> lst = new List<string>();
            for (int i = 1; i <= kvp.Count; i++)
            {
                if (kvp.ContainsKey(keyPrefix + i))
                {
                    lst.Add(kvp.GetAsString(keyPrefix + i));
                }
            }
            return lst;
        }

        #endregion SetCallTypeKeys
    }
}