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
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Enterprise.Model.ServiceModel;
using Genesyslab.Enterprise.Services;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols;
using Pointel.CIS.Desktop.Core.CIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Pointel.CIS.Desktop.Core.Util
{
    /// <summary>
    /// Comment: Utility or CIS Intergration
    /// Last Modified: 20-Jun-2016
    /// Created by: Pointel Inc
    /// </summary>
    public class CISIntegrationUtility
    {
        #region Fields

        private ILogger log = null;
        private Settings commonSettings = null;
        private static CISIntegrationUtility CISUtility;

        #endregion Fields

        #region GetInstance

        public static CISIntegrationUtility GetInstance()
        {
            if (CISUtility == null)
                CISUtility = new CISIntegrationUtility();

            return CISUtility;
        }

        #endregion GetInstance

        #region Constructor

        private CISIntegrationUtility()
        {
            this.log = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("CISCustomButtonViewModel");
            commonSettings = Settings.GetInstance();
        }

        #endregion Constructor

        #region CISContinue

        public void CISContinue(CIS.CISSettings CISObject)
        {
            try
            {
                log.Info("Agent clicked Continue button to search account in CIS application ");
                CIS.CISIntegration integrate = new CIS.CISIntegration();
                integrate.SearchCISAccount(commonSettings.PhoneConnectionID);
            }
            catch (Exception generalException)
            {
                log.Error("Error occurred while initiating CIS Integration " + generalException.ToString());
            }
        }

        #endregion CISContinue

        #region CISUpdate

        public void CISUpdate(CIS.CISSettings CISObject, IInteraction currentInteraction)
        {
            try
            {
                log.Info("Agent clicked to update button to update CustomerInfo View ");
                string accountID = CISSettings.GetInstance().CISConnection.CISLastAccountAccessed();
                //string accountID = "123654789";
                if (accountID.Equals("0000000000"))
                {
                    log.Info("CIS Search Update Result " + accountID);
                    new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                    "Account ID From CIS " + accountID, null, null);
                }
                else
                {
                    //new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                    //"Account ID From CIS " + accountID, null, null);
                    log.Info("Account ID From CIS: " + accountID);

                    if (!InteractionExtensions.IsIdle(currentInteraction.EntrepriseInteractionCurrent))
                    {
                        if (!InteractionExtensions.IsConsultCall(currentInteraction.EntrepriseInteractionCurrent))
                        {
                            Task.Run(() => UpdateInteractionWithServiceResponse(accountID, currentInteraction));
                        }
                        else
                        {
                            this.log.Info("Can not update Voice Interaction data because current call type is consult");
                        }
                    }
                    else
                    {
                        this.log.Info("Can not update Voice Interaction data because call is ended." + currentInteraction.InteractionId);
                        Task.Run(() => UpdateInteractionWithServiceResponse(accountID, currentInteraction, true));
                    }
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error occurred while receiving data from CIS  " + generalException.ToString());
                if (commonSettings.Enable_CIS_Error_Toaster)
                {
                    new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                       "CIS Update : " + generalException.Message, null, null);
                }
            }
        }

        #endregion CISUpdate

        #region CISSearch

        public void CISSearch(CIS.CISSettings CISObject)
        {
            try
            {
                log.Info("Agent clicked Search button to search account in CIS application ");
                log.Info("CIS Search response from CIS applicaiton " + CISSettings.GetInstance().CISConnection.CISSearch());
            }
            catch (Exception generalException)
            {
                log.Error("Error occurred while popup search window in CIS  " + generalException.ToString());
                if (commonSettings.Enable_CIS_Error_Toaster)
                {
                    new ExceptionAnalyzer(ContainerAccessPoint.Container).PublishError(AlertSection.Public,
                       "CIS Search : " + generalException.Message, null, null);
                }
            }
        }

        #endregion CISSearch

        #region CollectCISResponseData

        private string CollectCISResponseData(string propertyName, RESPONSE clientResponse)
        {
            string result = string.Empty;
            try
            {
                PropertyInfo[] props = typeof(SCREEN_POP_DATA).GetProperties();
                IEnumerable<string> data = from c in props
                                           select c.Name;
                if (data.Contains(propertyName))
                {
                    result = Convert.ToString(typeof(SCREEN_POP_DATA).GetProperty(propertyName).GetValue(clientResponse.screen_pop_response_data.screen_pop_data, null));
                    log.Info("Response from CIS Service " + propertyName + ":" + result);
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error occurred while collecting CIS response data " + generalException.ToString());
            }
            return result;
        }

        #endregion CollectCISResponseData

        #region CollectCISPopupData

        private string CollectCISPopupData(string propertyName, RESPONSE clientResponse)
        {
            string result = string.Empty;
            try
            {
                PropertyInfo[] props = typeof(SCREEN_POP_RESPONSE_DATA).GetProperties();
                IEnumerable<string> data = from c in props
                                           select c.Name;

                if (data.Contains(propertyName))
                {
                    result = Convert.ToString(typeof(SCREEN_POP_RESPONSE_DATA).GetProperty(propertyName).GetValue(clientResponse.screen_pop_response_data, null));
                    log.Info("Response from CIS Service " + propertyName + ":" + result);
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error occurred while collecting CIS popup data " + generalException.ToString());
            }
            return result;
        }

        #endregion CollectCISPopupData

        #region WebServiceCall

        public void UpdateInteractionWithServiceResponse(string accountID, IInteraction interaction, bool consult = false)
        {
            try
            {
                this.log.Info("UpdateInteractionWithServiceResponse: AccountId =" + accountID + "\t InteractionId =" + interaction.InteractionId);
                SCREEN_POP_REQUEST_DATA requestData = new SCREEN_POP_REQUEST_DATA();
                requestData.req_channel_type = commonSettings.RequestChannelType;
                requestData.req_check_digit = commonSettings.RequestCheckDigit;
                requestData.req_database_name = commonSettings.RequestDatabaseName;
                requestData.req_key_type = commonSettings.RequestKeyType;
                requestData.req_language_code = commonSettings.RequestLanguageCode;
                requestData.req_operation_cd = commonSettings.RequestOperationCode;
                requestData.req_account_id = Convert.ToInt64(accountID);

                SCREEN_POP_RESPONSE_DATA responseData = new SCREEN_POP_RESPONSE_DATA();

                REQUEST request = new REQUEST();
                request.screen_pop_request_data = requestData;

                E92660CCPortTypeClient client = new E92660CCPortTypeClient();
                if (!string.IsNullOrEmpty(commonSettings.RequestUrl))
                {
                    log.Info("Setting Web Service Url - " + commonSettings.RequestUrl);
                    client.Endpoint.Address = new EndpointAddress(commonSettings.RequestUrl);
                }
                RESPONSE clientResponse = null;

                using (new OperationContextScope(client.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(
                        new SecurityHeader(commonSettings.ServiceUserName, commonSettings.ServicePassword));

                    try
                    {
                        if (request.screen_pop_request_data != null)
                            log.Info(request.screen_pop_request_data.ToString());
                        else
                            log.Info("screen_pop_request_data is null");
                    }
                    catch (Exception logError) { log.Error("Error occurred while printing Request Log " + logError.ToString()); }

                    clientResponse = client.createScreenPopRequest(request);
                    //clientResponse = GetResponse();
                    if (clientResponse != null)
                    {
                        log.Info("**********************WebService Response******************************");
                        try
                        {
                            if (clientResponse.screen_pop_response_data != null)
                            {
                                this.log.Info(clientResponse.screen_pop_response_data.ToString());
                            }
                            else
                            {
                                this.log.Info("Screen_Pop_Response_Data is Null");
                            }
                        }
                        catch (Exception responseError) { log.Error("Error occurred while printing Response Log " + responseError.ToString()); }
                        log.Info("*************************************************************************");
                        responseData = clientResponse.screen_pop_response_data;
                        SCREEN_POP_DATA objCustInfo = responseData.screen_pop_data;
                        //Update Call Data with SCG KVP's  \
                        Genesyslab.Enterprise.Commons.Collections.KeyValueCollection CISUpdateData = new Genesyslab.Enterprise.Commons.Collections.KeyValueCollection();
                        if (interaction != null)
                            if (commonSettings.CISConfiguration.ContainskeyAndValue("call-info.type.identity.user-data.key-name"))
                                CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.type.identity.user-data.key-name"),
                                    commonSettings.CISConfiguration.ContainskeyAndValue("call-info.type.identity.update.value") ?
                                    commonSettings.CISConfiguration.GetAsString("call-info.type.identity.update.value") :
                                    "Y");

                        #region SCG Config Update

                        string resultValue = string.Empty;
                        if (objCustInfo != null)
                        {
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_RESP_ACCOUNT_ID, responseData.resp_account_id.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_48HR_COUNT, objCustInfo.screen_pop_48hr_count);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_ASC_BILL_CYC_ID, objCustInfo.screen_pop_asc_bill_cyc_id.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BASE, objCustInfo.screen_pop_base);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_CLS_DESC_CD, objCustInfo.screen_pop_ba_cls_desc_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_FRGN_LNG_CD, objCustInfo.screen_pop_ba_frgn_lng_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_PAID_DT, objCustInfo.screen_pop_ba_paid_dt);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_OPEN_DT, objCustInfo.screen_pop_ba_open_dt);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_SPCL_LDGR_SW, objCustInfo.screen_pop_ba_spcl_ldgr_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_STAT_CD, objCustInfo.screen_pop_ba_stat_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_TERM_DT, objCustInfo.screen_pop_ba_term_dt);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_TY_CD, objCustInfo.screen_pop_ba_ty_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CARE_SW, objCustInfo.screen_pop_care_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CASH_ONLY_SW, objCustInfo.screen_pop_cash_only_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CF_ID, objCustInfo.screen_pop_cf_id.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CF_TY_CD, objCustInfo.screen_pop_cf_ty_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CHECK_DIGIT, objCustInfo.screen_pop_check_digit);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CORE_AGGR_BILL_CD, objCustInfo.screen_pop_core_aggr_bill_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CURR_BAL_DUE, objCustInfo.screen_pop_curr_bal_due.ToString());
                            //CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CUSTOMER_NAME, objCustInfo.screen_pop_customer_name);
                            //CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_ADDRESS, objCustInfo.screen_pop_cust_address);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_ID, objCustInfo.screen_pop_cust_id.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_SEGMENT, objCustInfo.screen_pop_cust_segment);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_TY_CD, objCustInfo.screen_pop_cust_ty_cd);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_ELIGIBLE_SW, objCustInfo.screen_pop_eligible_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_EMAIL_ADDR, objCustInfo.screen_pop_email_addr);
                            //CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_HOME_PHONE, objCustInfo.screen_pop_home_phone);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_HOME_PHONE_EXT, objCustInfo.screen_pop_home_phone_ext);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_IC_ESTB_DT, objCustInfo.screen_pop_ic_estb_dt);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_LPP_SW, objCustInfo.screen_pop_lpp_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ADDRESS, objCustInfo.screen_pop_mail_address);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_CITY, objCustInfo.screen_pop_mail_city);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_STATE, objCustInfo.screen_pop_mail_state);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ZIP, objCustInfo.screen_pop_mail_zip);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ZIP4, objCustInfo.screen_pop_mail_zip4);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_MED_BASELINE_SW, objCustInfo.screen_pop_med_baseline_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_NBR_OF_DIALS, objCustInfo.screen_pop_nbr_of_dials.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_NO_VC_ORDR_QTY, objCustInfo.screen_pop_no_vc_ordr_qty);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_OFFER_AMORT_SW, objCustInfo.screen_pop_offer_amort_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_OFFER_RECERT_SW, objCustInfo.screen_pop_offer_recert_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_OFF_GREATER6, objCustInfo.screen_pop_off_greater6);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_ON_DEMAND_PAY, objCustInfo.screen_pop_on_demand_pay);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_ON_SIMPLE_PAY, objCustInfo.screen_pop_on_simple_pay);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_OVERDUE_COUNT, objCustInfo.screen_pop_overdue_count);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_OVER_ONE_YR, objCustInfo.screen_pop_over_one_yr);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_READ_OK_SW, objCustInfo.screen_pop_read_ok_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_SBA_CD, objCustInfo.screen_pop_sba_cd);
                            //CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_SPOUSE_NAME, objCustInfo.screen_pop_spouse_name);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_THIRD_PARTY_SW, objCustInfo.screen_pop_third_party_sw);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_VC_ORDR_QTY, objCustInfo.screen_pop_vc_ordr_qty);
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_YTD_VARIANCE, objCustInfo.screen_pop_ytd_variance.ToString());
                            CISUpdateData.Add(CISConstants.INBOUND_SCG_KVP_KEY_SCREEN_POP_ZIP_CODE, objCustInfo.screen_pop_zip_code);

                            if (ValidateKeys("call-info.account.bill-number.user-data.key-name"))
                            {
                                if (!CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")))
                                    CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"), accountID);
                                //CIS Service removes prefix "0"
                                //CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"), responseData.resp_account_id.ToString());
                            }
                            if (ValidateKeys("call-info.account.customer.user-data.key-name"))
                            {
                                if (!CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")))
                                    CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name"), objCustInfo.screen_pop_customer_name);
                            }
                            if (ValidateKeys("call-info.account.spouse.user-data.key-name"))
                            {
                                if (!CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")))
                                    CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name"), objCustInfo.screen_pop_spouse_name);
                            }
                            if (ValidateKeys("call-info.address.address.user-data.key-name"))
                            {
                                if (!CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name")))
                                    CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name"), objCustInfo.screen_pop_cust_address);
                            }
                            if (ValidateKeys("call-info.contact.phone.user-data.key-name"))
                            {
                                if (!CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")))
                                    CISUpdateData.Add(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name"), objCustInfo.screen_pop_home_phone);
                            }
                            // attaching new key for enabling agent to update customerinfo on eventattacheddatachanged event.
                            if (commonSettings.CanAddAttachData)
                                CISUpdateData.Add(commonSettings.AttachKeyName, "true");

                            KeyValueCollection LocalUpdateData = new KeyValueCollection();
                            if (interaction != null)
                            {
                                UpadateCustomerInfoCollection(interaction.EntrepriseLastInteractionEvent, CISUpdateData);
                                //updating userdata collection in CISCallDetails
                                if (CISSettings.GetInstance().CISCallDetails.ContainsKey(interaction.EntrepriseInteractionCurrent.Id.ToString()))
                                {
                                    foreach (var keyValue in CISUpdateData.AllKeys)
                                    {
                                        LocalUpdateData.Add(keyValue, CISUpdateData[keyValue]);
                                    }
                                    CISSettings.GetInstance().CISCallDetails[interaction.EntrepriseInteractionCurrent.Id.ToString()] = LocalUpdateData;
                                }
                            }

                            //custom code to update attach data once web-service returns response
                            if (!consult)
                            {
                                var UpdateData = ContainerAccessPoint.Container.Resolve<IEnterpriseServiceProvider>().Resolve<IVoiceService>("voiceService").SetAttachedData(interaction.EntrepriseInteractionCurrent, CISUpdateData);
                            }
                            else
                            {
                                dynamic consultEvent = interaction.EntrepriseLastInteractionEvent;
                                ConnectionId conid = consultEvent.ConnID;
                                var requestAttachUserData = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Userdata.RequestUpdateUserData.Create(Settings.GetInstance().ThisDN, conid, LocalUpdateData);
                                IMessage responseFromreequest = Settings.GetInstance().AgentDetails.FirstMediaVoice.Channel.Protocol.Request(requestAttachUserData);

                                log.Info("RequestAttachUserData response : " + responseFromreequest.ToString());
                            }

                        #endregion SCG Config Update
                        }
                        else
                            log.Info("Null Response is recieved from WebService");
                    }
                }
            }
            catch (Exception generalException)
            {
                this.log.Error("UpdateInteractionWithServiceResponse: Error Occurred while updating interaction data using service response:" + generalException.ToString());
            }
        }

        #endregion WebServiceCall

        #region UpadateCustomerInfoCollection

        public void UpadateCustomerInfoCollection(dynamic myEvent, Genesyslab.Enterprise.Commons.Collections.KeyValueCollection CISUpdateData)
        {
            try
            {
                log.Info("UpadateCustomerInfoCollection: Update the response data to customer info collection");
                //  IInteraction interaction = ContainerAccessPoint.Container.Resolve<IInteractionManager>().GetInteractionById(interactionId);
                CallInfoDataCollection popupdata = new CallInfoDataCollection();
                popupdata.ConnId = myEvent.ConnID.ToString();
                if (myEvent != null && CISUpdateData.Count > 0)
                {
                    KeyValueCollection userData = myEvent.UserData;
                    string actualCallType = string.Empty;
                    string callType = string.Empty;

                    actualCallType = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.actual-call-type.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.actual-call-type.user-data.key-name")]) :
                        GetAttributeValue("call-info.actual-call-type.attribute.key-name", myEvent));
                    log.Info("UpadateCustomerInfoCollection:ActualCallType -" + actualCallType);

                    callType = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.user-data.key-name")]) :
                        GetAttributeValue("call-info.type.calltype.attribute.key-name", myEvent));
                    log.Info("UpadateCustomerInfoCollection:CallType -" + callType);

                    #region Call Type Grid Data

                    #region Based On actual call type

                    string color = string.Empty;
                    if (actualCallType != null)
                    {
                        log.Info("UpadateCustomerInfoCollection actual call type :" + actualCallType);
                        if (commonSettings.CallType_Colorinfo_List != null && commonSettings.CallType_Colorinfo_List.Count > 0)
                        {
                            foreach (var item in commonSettings.CallType_Colorinfo_List)
                            {
                                if (item.matchValues != null && item.matchValues.Length > 0 && string.IsNullOrEmpty(color))
                                    foreach (var key in item.matchValues)
                                    {
                                        if (actualCallType.Contains(key))
                                        {
                                            log.Info("UpadateCustomerInfoCollection actual call contains :" + key);
                                            color = item.color;
                                            log.Info("UpadateCustomerInfoCollection actual call colour :" + color);
                                            break;
                                        }
                                    }
                            }
                        }
                        if (!string.IsNullOrEmpty(color))
                            popupdata.CallInfoData.Add(new MyListItem()
                            {
                                OptionValue = actualCallType,
                                OptionName = (ValidateKeys("call-info.type.calltype.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.display-name") : "Call Type"),
                                ColorName = color
                            });
                        else if (commonSettings.Language_Colorinfo_List != null && commonSettings.Language_Colorinfo_List.Count > 0)
                        {
                            log.Info("UpadateCustomerInfoCollection actual call colour is an empty...");
                            string languagevalue = string.Empty;
                            string languageColor = string.Empty;

                            foreach (var item in commonSettings.Language_Colorinfo_List)
                            {
                                if (!string.IsNullOrWhiteSpace(item.userdataKey))
                                {
                                    log.Info("UpadateCustomerInfoCollection language userdata key name :" + item.userdataKey);
                                    languagevalue = userData.GetAsString(item.userdataKey);
                                    log.Info("UpadateCustomerInfoCollection language value :" + languagevalue);
                                    if (!string.IsNullOrWhiteSpace(languagevalue) && item.matchValues != null && item.matchValues.Length > 0 && string.IsNullOrEmpty(languageColor))
                                        foreach (var key in item.matchValues)
                                        {
                                            if (languagevalue.Contains(key))
                                            {
                                                languageColor = item.color;
                                                log.Info("UpadateCustomerInfoCollection language color :" + languageColor);
                                                break;
                                            }
                                        }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(languageColor))
                            {
                                popupdata.CallInfoData.Add(new MyListItem()
                                    {
                                        OptionValue = actualCallType,
                                        OptionName = (ValidateKeys("call-info.type.calltype.display-name") ?
                                commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.display-name") : "Call Type"),
                                        ColorName = languageColor
                                    });
                            }
                            else
                            {
                                log.Info("UpadateCustomerInfoCollection language color is an empty...");
                                popupdata.CallInfoData.Add(new MyListItem()
                                {
                                    OptionValue = actualCallType,
                                    OptionName = (ValidateKeys("call-info.type.calltype.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.display-name") : "Call Type"),
                                    ColorName = commonSettings.DefaultColor
                                });
                            }
                        }
                    }

                    #endregion Based On actual call type

                    // Display Identity Authenticated
                    popupdata.CallInfoData.Add(new MyListItem()
                    {
                        OptionValue = (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.identity.user-data.key-name")) ?
                        Convert.ToString(CISUpdateData[commonSettings.CISConfiguration.GetAsString("call-info.type.identity.user-data.key-name")]) :
                        GetAttributeValue("call-info.type.identity.attribute.key-name", myEvent)),
                        OptionName = (ValidateKeys("call-info.type.identity.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.type.identity.display-name") : "Identify Authenticated"),
                        ColorName = commonSettings.DefaultColor
                    });

                    if (!string.IsNullOrEmpty(actualCallType) && commonSettings.PACallType.Contains(actualCallType))
                    {
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.exit-point.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.ivr.exit-point.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.display-name") : "IVR Exit Point"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.reason.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.reason.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.reason.display-name") : "Reason"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.care.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.care.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.care.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.care.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.care.display-name") : "CARE/ESAP Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else if (!string.IsNullOrEmpty(actualCallType) && commonSettings.CloseCallType.Contains(actualCallType))
                    {
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.exit-point.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.ivr.exit-point.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.display-name") : "IVR Exit Point"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.reason.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.date.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.date.display-name") : "Date"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.safe.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.safe.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.safe.display-name") : "Safe Access Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.soft.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.soft.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.soft.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.soft.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.soft.display-name") : "Soft Close Notification"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else if (!string.IsNullOrEmpty(actualCallType) && commonSettings.CSOCallType.Contains(actualCallType))
                    {
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.exit-point.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.ivr.exit-point.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.display-name") : "IVR Exit Point"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.reason.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.applicance.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.applicance.display-name") : "Appliance"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.safe.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.safe.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.safe.display-name") : "Safe Access Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else
                    {
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.care.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.care.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.care.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.care.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.care.display-name") : "CARE/ESAP Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.credit.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.credit.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.credit.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.credit.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.credit.display-name") : "Credit Phrase Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.safe.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.safe.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.safe.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.safe.display-name") : "Safe Access Offered"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }

                    #endregion Call Type Grid Data

                    #region Account Grid Data

                    #region Bill Account Number

                    if (commonSettings.CISConfiguration.ContainskeyAndValue("call-info.cash-indicator.value")
                                   && Regex.IsMatch(commonSettings.CISConfiguration.GetAsString("call-info.cash-indicator.value"), @"^\d+$"))
                    {
                        var value = int.Parse(commonSettings.CISConfiguration.GetAsString("call-info.cash-indicator.value"));
                        if (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")))
                        {
                            popupdata.AccountData.Add(new MyListItem()
                            {
                                OptionValue = StringUtils.formatAccountNumberSCG((CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"))),
                                value),
                                OptionName = (ValidateKeys("call-info.account.bill-number.display-name") ?
                                    commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.display-name") : "Bill Account Number"),
                                ColorName = commonSettings.DefaultColor
                            });
                            log.Info("*******Before Updated Account Number: " + (CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"))) + "*****");
                            log.Info("*******Updated Account Number: " + StringUtils.formatAccountNumberSCG((CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"))),
                                value) + "*****");
                        }
                        else
                        {
                            popupdata.AccountData.Add(new MyListItem()
                            {
                                OptionValue = StringUtils.formatAccountNumberSCG((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")) ?
                                Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")]) :
                                GetAttributeValue("call-info.account.bill-number.attribute.key-name", myEvent)), value),
                                OptionName = (ValidateKeys("call-info.account.bill-number.display-name") ?
                                commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.display-name") : "Bill Account Number"),
                                ColorName = commonSettings.DefaultColor
                            });
                        }
                    }

                    #endregion Bill Account Number

                    #region Customer Name

                    if (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")))
                    {
                        popupdata.AccountData.Add(new MyListItem()
                    {
                        OptionValue = CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")),
                        OptionName = (ValidateKeys("call-info.account.customer.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.account.customer.display-name") : "Customer Name"),
                        ColorName = commonSettings.DefaultColor
                    });
                    }
                    else
                    {
                        popupdata.AccountData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")]) :
                            GetAttributeValue("call-info.account.customer.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.account.customer.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.account.customer.display-name") : "Customer Name"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }

                    #endregion Customer Name

                    #region Spouse

                    if (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")))
                    {
                        popupdata.AccountData.Add(new MyListItem()
                        {
                            OptionValue = CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")),
                            OptionName = (ValidateKeys("call-info.account.spouse.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.display-name") : "Spouse"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else
                    {
                        popupdata.AccountData.Add(new MyListItem()
                                    {
                                        OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")) ?
                                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")]) :
                                        GetAttributeValue("call-info.account.spouse.attribute.key-name", myEvent)),
                                        OptionName = (ValidateKeys("call-info.account.spouse.display-name") ?
                                        commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.display-name") : "Spouse"),
                                        ColorName = commonSettings.DefaultColor
                                    });
                    }

                    #endregion Spouse

                    #endregion Account Grid Data

                    #region Address Grid Data

                    if (!string.IsNullOrWhiteSpace(callType) && callType.ToUpper() == "OUTBOUND")
                    {
                        string mailAddress1 = string.Empty;
                        string mailAddress2 = string.Empty;
                        string mailAddress3 = string.Empty;
                        string outboundAddress = string.Empty;

                        mailAddress1 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address1.user-data.key-name")) ?
                             Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address1.user-data.key-name")]) :
                             GetAttributeValue("call-info.out.address.address1.attribute.key-name", myEvent);
                        mailAddress2 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address2.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address2.user-data.key-name")]) :
                           GetAttributeValue("call-info.out.address.address2.attribute.key-name", myEvent);
                        mailAddress3 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address3.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address3.user-data.key-name")]) :
                           GetAttributeValue("call-info.out.address.address3.attribute.key-name", myEvent);
                        outboundAddress = mailAddress1 + " " + mailAddress2 + " " + mailAddress3;

                        popupdata.AddressData.Add(new MyListItem()
                        {
                            OptionName = (ValidateKeys("call-info.address.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.address.display-name") : "Address"),
                            OptionValue = outboundAddress,
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else
                    {
                        if (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name")))
                        {
                            popupdata.AddressData.Add(new MyListItem()
                            {
                                OptionName = (ValidateKeys("call-info.address.display-name") ?
                                commonSettings.CISConfiguration.GetAsString("call-info.address.display-name") : "Address"),
                                OptionValue = CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name")),
                                ColorName = commonSettings.DefaultColor
                            });
                        }
                        else
                        {
                            popupdata.AddressData.Add(new MyListItem()
                            {
                                OptionName = (ValidateKeys("call-info.address.display-name") ?
                                commonSettings.CISConfiguration.GetAsString("call-info.address.display-name") : "Address"),
                                OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name")) ?
                                Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.address.address.user-data.key-name")]) :
                                GetAttributeValue("call-info.address.address.attribute.key-name", myEvent)),
                                ColorName = commonSettings.DefaultColor
                            });
                        }
                    }

                    #endregion Address Grid Data

                    #region Notice Amount Grid Data

                    if (!string.IsNullOrWhiteSpace(callType) && callType.ToUpper() == "OUTBOUND")
                    {
                        popupdata.NoticeAmountData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.notice-amount.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.notice-amount.user-data.key-name")]) :
                            GetAttributeValue("call-info.notice-amount.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.notice-amount.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.notice-amount.display-name") : "Notice Amount"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }

                    #endregion Notice Amount Grid Data

                    #region Phone Grid Data

                    popupdata.PhoneData.Add(new MyListItem()
                    {
                        OptionValue = StringUtils.aniToPhoneNumber((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.contact.mobile.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.contact.mobile.user-data.key-name")]) :
                        GetAttributeValue("call-info.contact.mobile.attribute.key-name", myEvent))),
                        OptionName = (ValidateKeys("call-info.contact.mobile.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.contact.mobile.display-name") : "Calling Phone Number"),
                        ColorName = commonSettings.DefaultColor
                    });

                    #region Home Phone

                    if (CISUpdateData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")))
                    {
                        popupdata.PhoneData.Add(new MyListItem()
                        {
                            OptionValue = StringUtils.formatPhoneNumberWithAreaCode(CISUpdateData.GetAsString(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name"))),
                            OptionName = (ValidateKeys("call-info.contact.phone.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.display-name") : "Home Phone"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }
                    else
                    {
                        popupdata.PhoneData.Add(new MyListItem()
                        {
                            OptionValue = StringUtils.formatPhoneNumberWithAreaCode((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")]) :
                            GetAttributeValue("call-info.contact.phone.attribute.key-name", myEvent))),
                            OptionName = (ValidateKeys("call-info.contact.phone.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.display-name") : "Home Phone"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }

                    #endregion Home Phone

                    #endregion Phone Grid Data

                    #region IVR Grid Data

                    if (commonSettings.PACallType.Contains((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")]) : string.Empty)))
                    {
                    }
                    else if (commonSettings.CloseCallType.Contains((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")]) : string.Empty)))
                    {
                    }
                    else if (commonSettings.CSOCallType.Contains((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.call-type.user-data.key-name")]) : string.Empty)))
                    {
                    }
                    else
                    {
                        popupdata.IVRData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.exit-point.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.ivr.exit-point.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.ivr.exit-point.display-name") : "IVR Exit Point"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.IVRData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.reason.user-data.key-name")]) :
                            GetAttributeValue("call-info.ivr.reason.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.reason.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.reason.display-name") : "Reason"),
                            ColorName = commonSettings.DefaultColor
                        });
                    }

                    popupdata.IVRData.Add(new MyListItem()
                    {
                        OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice1.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice1.user-data.key-name")]) :
                        GetAttributeValue("call-info.ivr.sservice1.attribute.key-name", myEvent)),
                        OptionName = (ValidateKeys("call-info.ivr.sservice1.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice1.display-name") : "Self Service 1"),
                        ColorName = commonSettings.DefaultColor
                    });
                    popupdata.IVRData.Add(new MyListItem()
                    {
                        OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice2.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice2.user-data.key-name")]) :
                        GetAttributeValue("call-info.ivr.sservice2.attribute.key-name", myEvent)),
                        OptionName = (ValidateKeys("call-info.ivr.sservice2.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice2.display-name") : "Self Service 2"),
                        ColorName = commonSettings.DefaultColor
                    });
                    popupdata.IVRData.Add(new MyListItem()
                    {
                        OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice3.user-data.key-name")) ?
                        Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice3.user-data.key-name")]) :
                        GetAttributeValue("call-info.ivr.sservice3.attribute.key-name", myEvent)),
                        OptionName = (ValidateKeys("call-info.ivr.sservice3.display-name") ?
                        commonSettings.CISConfiguration.GetAsString("call-info.ivr.sservice3.display-name") : "Self Service 3"),
                        ColorName = commonSettings.DefaultColor
                    });

                    #endregion IVR Grid Data

                    log.Info("UpadateCustomerInfoCollection:Attach the response data to call info data");

                    //if (!commonSettings.CallInfoData.ContainsKey(popupdata.ConnId))
                    //    commonSettings.CallInfoData.Add(popupdata.ConnId, popupdata);
                    //else
                    //{
                    //    commonSettings.CallInfoData[popupdata.ConnId] = popupdata;
                    //}

                    Application.Current.Dispatcher.Invoke(() =>
                       {
                           log.Info("UpadateCustomerInfoCollection:Publish the Popup data to customer info");
                           commonSettings.CallInfoDataPublisher.PublishData(popupdata);
                       });
                }
            }
            catch (Exception generalException)
            {
                log.Error("UpadateCustomerInfoCollection: " + generalException.Message);
            }
        }

        #endregion UpadateCustomerInfoCollection

        #region ValidateKeys

        private bool ValidateKeys(string key)
        {
            if (commonSettings.CISConfiguration.ContainskeyAndValue(key))
            {
                log.Info(key + " = " + commonSettings.CISConfiguration.GetAsString(key));
                return true;
            }
            else
            {
                log.Info("The value for " + key + " not found in configuration");
                return false;
            }
        }

        #endregion ValidateKeys

        #region GetAttributeValue

        private string GetAttributeValue(string key, IMessage voiceEvent)
        {
            try
            {
                dynamic voiceCall = voiceEvent;
                if (commonSettings.CISConfiguration.ContainskeyAndValue(key))
                {
                    string attribKey = commonSettings.CISConfiguration.GetAsString(key).ToLower();
                    switch (attribKey)
                    {
                        case "ani":
                            return voiceCall.ANI;

                        case "thisdn":
                            return voiceCall.ThisDN;

                        case "otherdn":
                            return voiceCall.OtherDN;

                        case "agentid":
                            return voiceCall.AgentID;

                        case "dnis":
                            return voiceCall.DNIS;

                        case "connid":
                            return voiceCall.ConnID.ToString();

                        default:
                            this.log.Info("Attribute Value Not Found for the Attribute :" + attribKey);
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                this.log.Error("Error Occurred while getting attribute value " + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetAttributeValue

        #region Dummy GetResponse

        public RESPONSE GetResponse()
        {
            #region ResponseObject

            RESPONSE response = new RESPONSE();
            response.screen_pop_response_data = new SCREEN_POP_RESPONSE_DATA();
            response.screen_pop_response_data.resp_scrn_pop_oper_cd = 60;
            response.screen_pop_response_data.resp_return_code = "00";
            response.screen_pop_response_data.resp_error_msg = "";
            response.screen_pop_response_data.resp_appl_error_cd = 0;
            response.screen_pop_response_data.resp_system_error_cd = 0;
            response.screen_pop_response_data.resp_account_id = 1231846033;

            SCREEN_POP_DATA screenPopupData = new SCREEN_POP_DATA();

            screenPopupData.screen_pop_home_phone = "0000000000";
            screenPopupData.screen_pop_home_phone_ext = "";
            screenPopupData.screen_pop_customer_name = "MARICELA  AGUILERA";
            screenPopupData.screen_pop_spouse_name = "JOSE L AGUILERA";
            screenPopupData.screen_pop_cust_address = "801 SCHIPPER ST SPC 51 ARV  93203";
            screenPopupData.screen_pop_cust_id = 122189296;
            screenPopupData.screen_pop_cust_ty_cd = "IC";
            screenPopupData.screen_pop_cf_id = 123184600;
            screenPopupData.screen_pop_zip_code = "93203";
            screenPopupData.screen_pop_mail_address = "801 SCHIPPER ST SPC 51";
            screenPopupData.screen_pop_mail_city = "ARVIN";
            screenPopupData.screen_pop_mail_state = "CA";
            screenPopupData.screen_pop_mail_zip = "93203";
            screenPopupData.screen_pop_mail_zip4 = "2139";
            screenPopupData.screen_pop_check_digit = "9";
            screenPopupData.screen_pop_on_simple_pay = "N";
            screenPopupData.screen_pop_on_demand_pay = "N";
            screenPopupData.screen_pop_curr_bal_due = 0.00m;
            screenPopupData.screen_pop_eligible_sw = "Y";
            screenPopupData.screen_pop_base = "BA";
            screenPopupData.screen_pop_ba_term_dt = "";
            screenPopupData.screen_pop_ba_paid_dt = "";
            screenPopupData.screen_pop_ba_open_dt = "2012-08-27";
            screenPopupData.screen_pop_ic_estb_dt = "2005-05-17";
            screenPopupData.screen_pop_overdue_count = "00";
            screenPopupData.screen_pop_48hr_count = "00";
            screenPopupData.screen_pop_over_one_yr = "Y";
            screenPopupData.screen_pop_off_greater6 = "N";
            screenPopupData.screen_pop_read_ok_sw = "Y";
            screenPopupData.screen_pop_nbr_of_dials = 4;
            screenPopupData.screen_pop_offer_amort_sw = "N";
            screenPopupData.screen_pop_ytd_variance = 0.00m;
            screenPopupData.screen_pop_offer_recert_sw = "N";
            screenPopupData.screen_pop_lpp_sw = "N";
            screenPopupData.screen_pop_care_sw = "Y";
            screenPopupData.screen_pop_med_baseline_sw = "N";
            screenPopupData.screen_pop_third_party_sw = "N";
            screenPopupData.screen_pop_ba_frgn_lng_cd = "SP";
            screenPopupData.screen_pop_cust_segment = "";
            screenPopupData.screen_pop_core_aggr_bill_cd = "N";
            screenPopupData.screen_pop_sba_cd = "N";
            screenPopupData.screen_pop_ba_ty_cd = "C";
            screenPopupData.screen_pop_cash_only_sw = "N";
            screenPopupData.screen_pop_ba_spcl_ldgr_sw = "N";
            screenPopupData.screen_pop_vc_ordr_qty = 0;
            screenPopupData.screen_pop_no_vc_ordr_qty = 0;
            screenPopupData.screen_pop_cf_ty_cd = "R";
            screenPopupData.screen_pop_asc_bill_cyc_id = 20;
            screenPopupData.screen_pop_ba_stat_cd = "AC";
            screenPopupData.screen_pop_ba_cls_desc_cd = "";
            screenPopupData.screen_pop_email_addr = "";
            response.screen_pop_response_data.screen_pop_data = screenPopupData;

            #endregion ResponseObject

            return response;
        }

        #endregion Dummy GetResponse
    }
}