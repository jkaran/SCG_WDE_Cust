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
using Genesyslab.Desktop.Modules.Voice.Model.Agents;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pointel.CIS.Desktop.Core.Util
{
    /// <summary>
    /// Comment: contains the settings
    /// Last Modified: 13-APr-2016
    /// Created by: Pointel Inc
    /// </summary>
    internal class Settings
    {
        #region Field Declaration

        private static Settings _settings = null;
        private string phoneConnectionID = string.Empty;
        private string phoneConsultConnectionID = string.Empty;

        #endregion Field Declaration

        #region Properties

        public string PhoneConnectionID { get; set; }

        public string PhoneConsultConnectionID { get; set; }

        public CfgApplication ApplicationDetails { get; set; }

        private string _thisDN = string.Empty;

        public string ThisDN
        {
            get
            {
                if (_thisDN == string.Empty)
                {
                    try
                    {
                        ICollection<IMedia> medias = Settings.GetInstance().AgentDetails.Place.ListOfMedia;
                        if (medias != null)
                        {
                            foreach (IMedia media in medias)
                            {
                                IMediaVoice current = media as IMediaVoice;
                                if (current != null)
                                {
                                    _thisDN = current.ConfDN.Number;
                                    return current.ConfDN.Number;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    return _thisDN;
                }
                return null;
            }
        }

        public IAgent AgentDetails { get; set; }

        /// <summary>
        /// Gets or sets the cis configuration from CME.
        /// </summary>
        /// <value>
        /// The cis configuration.
        /// </value>
        public KeyValueCollection CISConfiguration
        {
            get;
            set;
        }

        #region PACallType,CloseCallType,CSOCallType default values

        public List<string> PACallType = new List<string>(new string[] {"SCG_ComServiceEng","SCG_ComServiceSpn","SCG_ResServiceEng","SCG_ResServiceSpn","SCG_ComServiceCantonese",
"SCG_ComServiceKorean","SCG_ComServiceMandarin","SCG_ComServiceVietnamese","SCG_ResServiceCantonese","SCG_ResServiceKorean","SCG_ResServiceMandarin","SCG_ResServiceVietnamese" });

        public List<string> CloseCallType = new List<string>(new string[] {"SCG_ComCloseEng","SCG_ComCloseSpn","SCG_ResCloseEng","SCG_ResCloseSpn","SCG_ComCloseCantonese","SCG_ComCloseKorean",
"SCG_ComCloseMandarin","SCG_ComCloseVietnamese","SCG_ResCloseCantonese","SCG_ResCloseKorean","SCG_ResCloseMandarin","SCG_ResCloseVietnamese" });

        public List<string> CSOCallType = new List<string>(new string[] { "SCG_ComServiceEng","SCG_ComServiceSpn","SCG_ResServiceEng","SCG_ResServiceSpn","SCG_ComServiceCantonese",
"SCG_ComServiceKorean","SCG_ComServiceMandarin","SCG_ComServiceVietnamese","SCG_ResServiceCantonese","SCG_ResServiceKorean","SCG_ResServiceMandarin","SCG_ResServiceVietnamese"});

        #endregion PACallType,CloseCallType,CSOCallType default values

        #region CustomerInfo Display Options

        /// <summary>
        /// Gets or sets the customer information call type section names.
        /// </summary>
        /// <value>
        /// The customer information call type section names.
        /// </value>
        public string[] CustomerInfo_CallTypeSectionNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display line seperator for every grid.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable call information bottom line]; otherwise, <c>false</c>.
        /// </value>
        public bool Enable_CallInfo_BottomLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default color for displaying customer info text.
        /// </summary>
        /// <value>
        /// The default color.
        /// </value>
        public string DefaultColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the emergency type calls.
        /// </summary>
        /// <value>
        /// The color of the emergency.
        /// </value>
        public string EmergencyColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the spanish language calls.
        /// </summary>
        /// <value>
        /// The color of the spanish.
        /// </value>
        public string SpanishColor
        {
            get;
            set;
        }

        #endregion CustomerInfo Display Options

        #region GetInstance

        internal static Settings GetInstance()
        {
            if (_settings == null)
            {
                _settings = new Settings();
                return _settings;
            }
            return _settings;
        }

        private Settings()
        {
        }

        #endregion GetInstance

        #region IPublisher Fields

        public IPublisher<CallInfoDataCollection> CallInfoDataPublisher { get; set; }

        #endregion IPublisher Fields

        #region CIS Web Service Request Properties

        public string ServiceUserName { get; set; }

        public string ServicePassword { get; set; }

        public short RequestOperationCode { get; set; }

        public string RequestChannelType { get; set; }

        public string RequestKeyType { get; set; }

        public string RequestDatabaseName { get; set; }

        public string RequestLanguageCode { get; set; }

        public string RequestCheckDigit { get; set; }

        public string RequestUrl { get; set; }

        #endregion CIS Web Service Request Properties

        #region CallInfo Data Collection

        private Dictionary<string, CallInfoDataCollection> callInfoData = new Dictionary<string, CallInfoDataCollection>();

        public Dictionary<string, CallInfoDataCollection> CallInfoData
        {
            get
            {
                return callInfoData;
            }
        }

        #endregion CallInfo Data Collection

        #region CIS Shrtcut keys

        /// <summary>
        /// Gets or sets the cis update hot key.
        /// </summary>
        /// <value>
        /// The cis update hot key.
        /// </value>
        public string CISUpdate_HotKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cis continue hot key.
        /// </summary>
        /// <value>
        /// The cis continue hot key.
        /// </value>
        public string CISContinue_HotKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cis search hot key.
        /// </summary>
        /// <value>
        /// The cis search hot key.
        /// </value>
        public string CISSearch_HotKey
        {
            get;
            set;
        }

        #endregion CIS Shrtcut keys

        /// <summary>
        /// Gets or sets the customer info popup event names.
        /// </summary>
        /// <value>
        /// The popup event names.
        /// </value>
        public string PopupEventNames { get; set; }

        /// <summary>
        /// Gets or sets the type of the call for customer info popup.
        /// </summary>
        /// <value>
        /// The type of the event call.
        /// </value>
        public string PopupCallType { get; set; }

        /// <summary>
        /// Gets or sets the agent global action codes.
        /// </summary>
        /// <value>
        /// The agent global action codes.
        /// </value>
        public string AgentLevelActionCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the channel level action codes.
        /// </summary>
        /// <value>
        /// The channel level action codes.
        /// </value>
        public string ChannelLevelActionCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the not ready action code filters.
        /// </summary>
        /// <value>
        /// The not ready action code filters.
        /// </value>
        public string[] NotReadyActionCodeFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to format account number if it length is less than 10 and makes 10 digit by adding zero's as prefix.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable account number format]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableAccountNumberFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable CIS buttons in voice Toolbar.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable voice bar buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool Enable_Voice_Bar_Buttons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  enable CIS buttons in CustomerInfo view
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable call information buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool Enable_CallInfo_Buttons { get; set; }

        /// <summary>
        /// Gets or sets the call type colorinfo list.
        /// </summary>
        /// <value>
        /// The call type colorinfo list.
        /// </value>
        public List<ColorInfo> CallType_Colorinfo_List { get; set; }

        /// <summary>
        /// Gets or sets the language colorinfo list.
        /// </summary>
        /// <value>
        /// The language colorinfo list.
        /// </value>
        public List<ColorInfo> Language_Colorinfo_List { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable cis error message toaster].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable cis error toaster]; otherwise, <c>false</c>.
        /// </value>
        public bool Enable_CIS_Error_Toaster { get; set; }

        #endregion Properties

        public bool PopupOnEventRinging { get; set; }
        public bool PopupOnEventEstablished { get; set; }
        public bool PopupOnEventDialing { get; set; }
        public bool PopupOnEventHeld { get; set; }
        public bool PopupOnEventRetrived { get; set; }
        public bool PopupOnEventPartyChanged { get; set; }
        public bool PopupOnEventAttachedDataChanged { get; set; }

        public bool PopupInboundCallType { get; set; }
        public bool PopupOutboundCallType { get; set; }
        public bool PopupConsultCallType { get; set; }

        public bool EnableConsultNotePad { get; set; }
        public bool EnableVoiceEmailAgentStatus { get; set; }

        public bool ManualPopupDisabled { get; set; }

        public string AttachKeyName { get; set; }
        public bool CanAddAttachData { get; set; }

        public string LoginStatusReasoncode { get; set; }
        public string LoginStatusReasonName { get; set; }
        public string LoginStatusWorkMode { get; set; }
        public string LoginStatusRequestAttribute { get; set; }

        public Dictionary<string, string> ConsultConnectionIds = new Dictionary<string, string>();

        #region ToString Method

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(this);
                    txt.Append(String.Format("{0} = {1}\n", name, value));
                }
            }
            catch (Exception)
            {
                return txt.ToString();
            }
            return txt.ToString();
        }

        #endregion ToString Method
    }
}