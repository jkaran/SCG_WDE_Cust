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
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Pointel.CIS.Desktop.Core.Voice
{
    /// <summary>
    /// Comment: Agent Voice Events Subscribed here
    /// Last Modified: 20-Jun-2016
    /// Created by: Pointel Inc
    /// </summary>
    ///

    public delegate void ConsultNotepadVisiblityEvent(string interactionId, bool enable, string notePadText);

    internal class SubscribeAgentVoiceEvents
    {
        #region Field Declaration

        public static event ConsultNotepadVisiblityEvent notepadVisiblityEvent;

        private EventEstablished eventEstablished = null;
        private EventRinging eventRinging = null;
        private EventDialing eventDialing = null;
        private EventReleased eventReleased = null;
        private Log logger;
        private Task currentCallTask = null;
        private IObjectContainer container;
        private CIS.CISIntegration cisPopup = null;
        private Settings commonSettings = null;

        #endregion Field Declaration

        #region Constructor

        public SubscribeAgentVoiceEvents(IObjectContainer Container)
        {
            commonSettings = Settings.GetInstance();
            logger = Log.GenInstance();
            container = Container;
            cisPopup = new CIS.CISIntegration();
            CIS.CISSettings.GetInstance().CISCallDetails = new System.Collections.Generic.Dictionary<string, KeyValueCollection>();
            RegisterAgentEvent();
        }

        #endregion Constructor

        #region RegisterAgentEvent

        private void RegisterAgentEvent()
        {
            try
            {
                logger.Info("Register to receive Voice Events ");
                Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteractionManager manager = this.container.Resolve<
                    Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteractionManager>();

                manager.InteractionEvent += new EventHandler<Genesyslab.Desktop.Infrastructure.
                    EventArgs<Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction>>(manager_InteractionEvent);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occured while registering voice events " + generalException.ToString());
            }
        }

        #endregion RegisterAgentEvent

        #region manager_InteractionEvent

        private void manager_InteractionEvent(object sender, Genesyslab.Desktop.Infrastructure.
            EventArgs<Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction> e)
        {
            try
            {
                Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction events = e.Value;
                if (events != null)
                {
                    ReceiveCalls(events.EntrepriseLastInteractionEvent, events);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occured while publishing current interaction event " + generalException.ToString());
            }
        }

        #endregion manager_InteractionEvent

        #region ReceiveCalls

        // CustomerInfo refresh EventAttachedDataChanged event removed on 23-NOV-2016 as per ram's request.
        /// <summary>
        /// Handles Voice Interaction Events and pop customer Info
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="currentInteraction">The current interaction.</param>
        public void ReceiveCalls(IMessage events, IInteraction currentInteraction)
        {
            try
            {
                if (events != null)
                {
                    logger.Info("Agent Voice Event " + events.ToString());

                    switch (events.Id)
                    {
                        case EventRinging.MessageId:
                            eventRinging = (EventRinging)events;
                            if (eventRinging.UserData != null)
                            {
                                try
                                {
                                    if (commonSettings.PopupOnEventRinging &&
                                        (eventRinging.CallType == CallType.Inbound && commonSettings.PopupInboundCallType)
                                       || (eventRinging.CallType == CallType.Consult && commonSettings.PopupConsultCallType)
                                        || (eventRinging.CallType == CallType.Outbound && commonSettings.PopupOutboundCallType))
                                    {
                                        logger.Info("CIS Integration at Event Ringing ");
                                        currentCallTask = new Task(() => this.ScreenPopup(eventRinging));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(eventRinging.ConnID));
                                        }
                                    }
                                    commonSettings.PhoneConsultConnectionID = Convert.ToString(eventRinging.ConnID);
                                }
                                catch (Exception genralException)
                                {
                                    logger.Error("Error occured while popup Customer Info View " + genralException.Message);
                                }   
                            }
                            else
                            {
                                logger.Warn("No User data found ");
                            }
                            break;

                        case EventEstablished.MessageId:
                            eventEstablished = (EventEstablished)events;
                            if (eventEstablished.UserData != null)
                            {
                                try
                                {
                                    if (commonSettings.PopupOnEventEstablished)
                                    {
                                        if ((eventEstablished.CallType == CallType.Outbound || eventEstablished.CallType == CallType.Unknown) &&
                                            commonSettings.PopupOutboundCallType)
                                        {
                                            logger.Info("CIS Integration at Event Established ");
                                            currentCallTask = new Task(() => this.ScreenPopup(eventEstablished));
                                            currentCallTask.Start();
                                            if (commonSettings.ManualPopupDisabled)
                                            {
                                                cisPopup.SearchCISAccount(Convert.ToString(eventEstablished.ConnID));
                                            }
                                        }
                                        else if ((eventEstablished.CallType == CallType.Inbound && commonSettings.PopupInboundCallType)
                                       || ((eventEstablished.CallType == CallType.Consult && commonSettings.PopupConsultCallType)))
                                        {
                                            logger.Info("CIS Integration at Event Established ");
                                            currentCallTask = new Task(() => this.ScreenPopup(eventEstablished));
                                            currentCallTask.Start();
                                            if (commonSettings.ManualPopupDisabled)
                                            {
                                                cisPopup.SearchCISAccount(Convert.ToString(eventEstablished.ConnID));
                                            }
                                        }
                                    }
                                    // attaching key for identifying whether the lead is updated the customerinfo or not.
                                    if (commonSettings.CanAddAttachData)
                                        currentInteraction.SetAttachedData(commonSettings.AttachKeyName, "false");
                                    // Publishing Notepad Text for consult call
                                    PublishNotepadText(eventEstablished);
                                }
                                catch (Exception genralException)
                                {
                                    logger.Error("Error occured while popup Customer Info View " + genralException.Message);
                                }
                            }
                            else
                            {
                                logger.Warn("No User data found ");
                            }
                            break;

                        case EventAttachedDataChanged.MessageId:
                            EventAttachedDataChanged attachedDataChanged = (EventAttachedDataChanged)events;

                            if (commonSettings.PopupOnEventAttachedDataChanged && attachedDataChanged.UserData != null && attachedDataChanged.UserData.ContainsKey(commonSettings.AttachKeyName) &&
                                Convert.ToBoolean(attachedDataChanged.UserData[commonSettings.AttachKeyName]))
                            {
                                if ((attachedDataChanged.CallType == CallType.Inbound && commonSettings.PopupInboundCallType)
                                    || (attachedDataChanged.CallType == CallType.Consult && commonSettings.PopupConsultCallType)
                                    || (attachedDataChanged.CallType == CallType.Outbound && commonSettings.PopupOutboundCallType))
                                {
                                    logger.Info("CustomerInfo refresh at attached data changed event.. ");
                                    currentCallTask = new Task(() => this.ScreenPopup(attachedDataChanged));
                                    currentCallTask.Start();
                                    if (commonSettings.ManualPopupDisabled)
                                    {
                                        cisPopup.SearchCISAccount(Convert.ToString(attachedDataChanged.ConnID));
                                    }
                                    if (commonSettings.CanAddAttachData)
                                        currentInteraction.SetAttachedData(commonSettings.AttachKeyName, "false");
                                }
                            }
                            break;

                        case EventPartyChanged.MessageId:
                            EventPartyChanged eventPartyChanged = (EventPartyChanged)events;
                            try
                            {
                                if (!commonSettings.ConsultConnectionIds.ContainsKey(eventPartyChanged.PreviousConnID.ToString()))
                                {
                                    commonSettings.ConsultConnectionIds.Add(eventPartyChanged.PreviousConnID.ToString(), eventPartyChanged.ConnID.ToString());
                                }
                                if (commonSettings.PopupOnEventPartyChanged &&
                                    ((eventPartyChanged.CallType == CallType.Inbound && commonSettings.PopupInboundCallType) ||
                                    (eventPartyChanged.CallType == CallType.Outbound && commonSettings.PopupOutboundCallType)))
                                {
                                    logger.Info("CIS Integration at Event Party Changed ");
                                    currentCallTask = new Task(() => this.ScreenPopup(eventPartyChanged));
                                    currentCallTask.Start();
                                    if (commonSettings.ManualPopupDisabled)
                                    {
                                        cisPopup.SearchCISAccount(Convert.ToString(eventPartyChanged.ConnID));
                                    }
                                }
                                HideCustomNotepadView(eventPartyChanged);
                            }
                            catch (Exception genralException)
                            {
                                logger.Error("Error occured while popup Customer Info View " + genralException.Message);
                            }
                            break;

                        case EventHeld.MessageId:
                            EventHeld callOnHold = (EventHeld)events;
                            try
                            {
                                if (commonSettings.PopupOnEventHeld)
                                {
                                    if (callOnHold.CallType == CallType.Inbound && commonSettings.PopupInboundCallType)
                                    {
                                        logger.Info("CIS Integration at Inbound Event Held ");
                                        currentCallTask = new Task(() => this.ScreenPopup(callOnHold));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(callOnHold.ConnID));
                                        }
                                    }
                                    else if (callOnHold.CallType == CallType.Consult && commonSettings.PopupConsultCallType)
                                    {
                                        logger.Info("CIS Integration at Consult Event Held ");
                                        currentCallTask = new Task(() => this.ScreenPopup(callOnHold));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(callOnHold.ConnID));
                                        }
                                    }
                                    else if (callOnHold.CallType == CallType.Outbound && commonSettings.PopupOutboundCallType)
                                    {
                                        logger.Info("CIS Integration at Outbound Event Held ");
                                        currentCallTask = new Task(() => this.ScreenPopup(callOnHold));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(callOnHold.ConnID));
                                        }
                                    }
                                }
                            }
                            catch (Exception generalException)
                            {
                                logger.Error("Error occured while setting flag for EventHeld " + generalException.ToString());
                            }
                            break;

                        case EventRetrieved.MessageId:
                            EventRetrieved retrievedCall = (EventRetrieved)events;
                            try
                            {
                                if (commonSettings.PopupOnEventRetrived)
                                {
                                    if (retrievedCall.CallType == CallType.Inbound && commonSettings.PopupInboundCallType)
                                    {
                                        logger.Info("CIS Integration at Inbound Event Retrieved ");
                                        currentCallTask = new Task(() => this.ScreenPopup(retrievedCall));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(retrievedCall.ConnID));
                                        }
                                    }
                                    else if (retrievedCall.CallType == CallType.Consult && commonSettings.PopupConsultCallType)
                                    {
                                        logger.Info("CIS Integration at Consult Event Retrieved ");
                                        currentCallTask = new Task(() => this.ScreenPopup(retrievedCall));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(retrievedCall.ConnID));
                                        }
                                    }
                                    else if (retrievedCall.CallType == CallType.Outbound && commonSettings.PopupOutboundCallType)
                                    {
                                        logger.Info("CIS Integration at Outbound Event Retrieved ");
                                        currentCallTask = new Task(() => this.ScreenPopup(retrievedCall));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(retrievedCall.ConnID));
                                        }
                                    }
                                }
                            }
                            catch (Exception generalException)
                            {
                                logger.Error("Error occured while setting flag for EventHeld " + generalException.ToString());
                            }
                            break;

                        case EventDialing.MessageId:
                            eventDialing = (EventDialing)events;
                            try
                            {
                                if (eventDialing.CallType == CallType.Outbound || eventDialing.CallType == CallType.Unknown)
                                {
                                    if (commonSettings.PopupOnEventDialing && commonSettings.PopupOutboundCallType)
                                    {
                                        logger.Info("CIS Integration at Event Dialing ");
                                        currentCallTask = new Task(() => this.ScreenPopup(eventDialing));
                                        currentCallTask.Start();
                                        if (commonSettings.ManualPopupDisabled)
                                        {
                                            cisPopup.SearchCISAccount(Convert.ToString(eventDialing.ConnID));
                                        }
                                    }
                                }
                            }
                            catch (Exception genralException)
                            {
                                logger.Error("Error occured while popup Customer Info View " + genralException.Message);
                            }
                            break;

                        case EventReleased.MessageId:
                            eventReleased = (EventReleased)events;
                            try
                            {
                                if (CIS.CISSettings.GetInstance().CISCallDetails.ContainsKey(eventReleased.ConnID.ToString()))
                                {
                                    logger.Info("Removed Continue Call Data from Collection " + eventReleased.CallType.ToString() + " : " + eventReleased.CallState.ToString());
                                    CIS.CISSettings.GetInstance().CISCallDetails.Remove(eventReleased.ConnID.ToString());
                                }
                                else if (CIS.CISSettings.GetInstance().CISCallDetails.ContainsKey(commonSettings.PhoneConsultConnectionID))
                                {
                                    logger.Info("Removed Continue Call Data from Collection for consult " + commonSettings.PhoneConsultConnectionID + " : " + eventReleased.CallState.ToString());
                                    CIS.CISSettings.GetInstance().CISCallDetails.Remove(commonSettings.PhoneConsultConnectionID);
                                }

                                if (commonSettings.CallInfoData.Count > 0)
                                {
                                    if (commonSettings.CallInfoData.ContainsKey(eventReleased.ConnID.ToString()))
                                        commonSettings.CallInfoData.Remove(eventReleased.ConnID.ToString());
                                    else if (commonSettings.CallInfoData.ContainsKey(commonSettings.PhoneConsultConnectionID))
                                        commonSettings.CallInfoData.Remove(commonSettings.PhoneConsultConnectionID);
                                }
                            }
                            catch (Exception genralException)
                            {
                                logger.Error("Error occured while popup Customer Info View " + genralException.Message);
                            }
                            break;

                        default:
                            logger.Info("Unhandled Event " + events.Name);
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occured while receiving voice events " + generalException.ToString());
            }
            finally
            {
            }
        }

        #endregion ReceiveCalls

        #region ScreenPopup

        private void ScreenPopup(IMessage voiceEvent)
        {
            try
            {
                //Logic to get data from incoming call
                dynamic myEvent = null;
                switch (voiceEvent.Id)
                {
                    case EventRinging.MessageId:
                        myEvent = (EventRinging)voiceEvent;
                        break;

                    case EventEstablished.MessageId:
                        myEvent = (EventEstablished)voiceEvent;
                        break;

                    case EventRetrieved.MessageId:
                        myEvent = (EventRetrieved)voiceEvent;
                        break;

                    case EventHeld.MessageId:
                        myEvent = (EventHeld)voiceEvent;
                        break;

                    case EventDialing.MessageId:
                        myEvent = (EventDialing)voiceEvent;
                        break;

                    case EventReleased.MessageId:
                        myEvent = (EventReleased)voiceEvent;
                        break;

                    case EventPartyChanged.MessageId:
                        myEvent = (EventPartyChanged)voiceEvent;
                        break;

                    case EventAttachedDataChanged.MessageId:
                        myEvent = (EventAttachedDataChanged)voiceEvent;
                        break;

                    default:
                        break;
                }
                CallInfoDataCollection popupdata = new CallInfoDataCollection();
                popupdata.ConnId = myEvent.ConnID.ToString();
                if (myEvent != null)
                {
                    logger.Info("ScreenPopup collecting popup  Event : " + myEvent.ToString());
                    KeyValueCollection userData = myEvent.UserData;
                    if (userData != null)
                    {
                        string actualCallType = string.Empty;
                        string callType = string.Empty;

                        actualCallType = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.actual-call-type.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.actual-call-type.user-data.key-name")]) :
                            GetAttributeValue("call-info.actual-call-type.attribute.key-name", myEvent));
                        // callType = "outbound";
                        callType = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.calltype.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.calltype.attribute.key-name", myEvent));

                        #region Call Type Grid Data

                        #region Based On actual call type

                        string color = string.Empty;
                        if (actualCallType != null)
                        {
                            logger.Info("ScreenPopup actual call type :" + actualCallType);
                            if (commonSettings.CallType_Colorinfo_List != null && commonSettings.CallType_Colorinfo_List.Count > 0)
                            {
                                foreach (var item in commonSettings.CallType_Colorinfo_List)
                                {
                                    if (item.matchValues != null && item.matchValues.Length > 0 && string.IsNullOrEmpty(color))
                                        foreach (var key in item.matchValues)
                                        {
                                            if (actualCallType.Contains(key))
                                            {
                                                logger.Info("ScreenPopup actual call contains :" + key);
                                                color = item.color;
                                                logger.Info("ScreenPopup actual call colour :" + color);
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
                                logger.Info("ScreenPopup actual call colour is an empty...");
                                string languagevalue = string.Empty;
                                string languageColor = string.Empty;

                                foreach (var item in commonSettings.Language_Colorinfo_List)
                                {
                                    if (!string.IsNullOrWhiteSpace(item.userdataKey))
                                    {
                                        logger.Info("ScreenPopup language userdata key name :" + item.userdataKey);
                                        languagevalue = userData.GetAsString(item.userdataKey);
                                        logger.Info("ScreenPopup language value :" + languagevalue);
                                        if (!string.IsNullOrWhiteSpace(languagevalue) && item.matchValues != null && item.matchValues.Length > 0 && string.IsNullOrEmpty(languageColor))
                                            foreach (var key in item.matchValues)
                                            {
                                                if (languagevalue.ToLower().Contains(key.ToLower()))
                                                {
                                                    languageColor = item.color;
                                                    logger.Info("ScreenPopup language color :" + languageColor);
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
                                    logger.Info("ScreenPopup language color is an empty...");
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

                        popupdata.CallInfoData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.type.identity.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.type.identity.user-data.key-name")]) :
                            GetAttributeValue("call-info.type.identity.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.type.identity.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.type.identity.display-name") : "Identify Authenticated"),
                            ColorName = commonSettings.DefaultColor
                        });

                        if (!string.IsNullOrEmpty(actualCallType) && commonSettings.PACallType.Contains(actualCallType))
                        {
                            logger.Info("Invoking PACallType");
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
                            logger.Info("Invoking CloseCallType");
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
                            commonSettings.CISConfiguration.GetAsString("call-info.type..soft.display-name") : "Soft Close Notification"),
                                ColorName = commonSettings.DefaultColor
                            });
                        }
                        else if (!string.IsNullOrEmpty(actualCallType) && commonSettings.CSOCallType.Contains(actualCallType))
                        {
                            logger.Info("Invoking CSOCallType");
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
                            logger.Info("call type is different : " + actualCallType + " continuing data collection..");
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

                        var value = int.Parse(commonSettings.CISConfiguration.GetAsString("call-info.cash-indicator.value"));
                        logger.Info("AccountNumber from Userdata : " + commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name"));
                        logger.Info("Collected account number " + StringUtils.formatAccountNumberSCG((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")]) :
                           GetAttributeValue("call-info.account.bill-number.attribute.key-name", myEvent)), value));
                        popupdata.AccountData.Add(new MyListItem()
                        {
                            OptionValue = StringUtils.formatAccountNumberSCG((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.user-data.key-name")]) :
                           GetAttributeValue("call-info.account.bill-number.attribute.key-name", myEvent)), value),
                            OptionName = (ValidateKeys("call-info.account.bill-number.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.account.bill-number.display-name") : "Bill Account Number"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.AccountData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.customer.user-data.key-name")]) :
                           GetAttributeValue("call-info.account.customer.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.account.customer.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.account.customer.display-name") : "Customer Name"),
                            ColorName = commonSettings.DefaultColor
                        });
                        popupdata.AccountData.Add(new MyListItem()
                        {
                            OptionValue = (userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")) ?
                           Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.user-data.key-name")]) :
                           GetAttributeValue("call-info.account.spouse.attribute.key-name", myEvent)),
                            OptionName = (ValidateKeys("call-info.account.spouse.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.account.spouse.display-name") : "Spouse"),
                            ColorName = commonSettings.DefaultColor
                        });

                        #endregion Account Grid Data

                        #region Address Grid Data

                        if (!string.IsNullOrEmpty(callType) && callType.ToUpper() == "OUTBOUND")
                        {
                            string mailAddress1 = string.Empty;
                            string mailAddress2 = string.Empty;
                            string mailAddress3 = string.Empty;
                            if (ValidateKeys("call-info.out.address.address1.user-data.key-name"))
                                mailAddress1 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address1.user-data.key-name")) ?
                                 Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address1.user-data.key-name")]) :
                                 GetAttributeValue("call-info.out.address.address1.attribute.key-name", myEvent);
                            if (ValidateKeys("call-info.out.address.address2.user-data.key-name"))
                                mailAddress2 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address2.user-data.key-name")) ?
                                   Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address2.user-data.key-name")]) :
                                   GetAttributeValue("call-info.out.address.address2.attribute.key-name", myEvent);
                            if (ValidateKeys("call-info.out.address.address3.user-data.key-name"))
                                mailAddress3 = userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.out.address.address3.user-data.key-name")) ?
                                   Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.out.address.address3.user-data.key-name")]) :
                                   GetAttributeValue("call-info.out.address.address3.attribute.key-name", myEvent);
                            var outboundAddress = mailAddress1 + " " + mailAddress2 + " " + mailAddress3;
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

                        #endregion Address Grid Data

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

                        popupdata.PhoneData.Add(new MyListItem()
                        {
                            OptionValue = StringUtils.formatPhoneNumberWithAreaCode((userData.ContainsKey(commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")) ?
                            Convert.ToString(userData[commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.user-data.key-name")]) :
                            GetAttributeValue("call-info.contact.phone.attribute.key-name", myEvent))),
                            OptionName = (ValidateKeys("call-info.contact.phone.display-name") ?
                            commonSettings.CISConfiguration.GetAsString("call-info.contact.phone.display-name") : "Home Phone"),
                            ColorName = commonSettings.DefaultColor
                        });

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

                        if (!commonSettings.CallInfoData.ContainsKey(popupdata.ConnId))
                            commonSettings.CallInfoData.Add(popupdata.ConnId, popupdata);
                        else
                        {
                            commonSettings.CallInfoData[popupdata.ConnId] = popupdata;
                        }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            commonSettings.CallInfoDataPublisher.PublishData(popupdata);
                        });
                    }
                    else
                        this.logger.Info("UserData is null...");
                }
                try
                {
                    logger.Info("***** Call Data Count " + CIS.CISSettings.GetInstance().CISCallDetails.Count.ToString());
                    if (CIS.CISSettings.GetInstance().CISCallDetails.Count == 0)
                    {
                        logger.Info("New Continue Call Data ");
                        commonSettings.PhoneConnectionID = Convert.ToString(myEvent.ConnID);
                        CIS.CISSettings.GetInstance().CISCallDetails.Add(myEvent.ConnID.ToString(), myEvent.UserData);
                    }
                    else
                    {
                        if (CIS.CISSettings.GetInstance().CISCallDetails.ContainsKey(myEvent.ConnID.ToString()))
                        {
                            logger.Info("Continue Call Data updated ");
                            CIS.CISSettings.GetInstance().CISCallDetails.Remove(myEvent.ConnID.ToString());
                            commonSettings.PhoneConnectionID = Convert.ToString(myEvent.ConnID);
                            CIS.CISSettings.GetInstance().CISCallDetails.Add(myEvent.ConnID.ToString(), myEvent.UserData);
                        }
                        else
                        {
                            logger.Info("New Continue Call Data at For Each ");
                            commonSettings.PhoneConnectionID = Convert.ToString(myEvent.ConnID);
                            CIS.CISSettings.GetInstance().CISCallDetails.Add(myEvent.ConnID.ToString(), myEvent.UserData);
                        }
                    }
                }
                catch (Exception callException)
                {
                    logger.Error("Error occured while collecting data for Continue process " + callException.ToString());
                    CIS.CISSettings.GetInstance().CISCallDetails.Add(myEvent.ConnID.ToString(), myEvent.UserData);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while popup Customer Info " + generalException.ToString());
            }
            finally
            {
                currentCallTask = null;
                GC.Collect();
            }
        }

        #endregion ScreenPopup

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
                            this.logger.Info("Attribute Value Not Found for the Attribute :" + attribKey);
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error Occurred while getting attribute value " + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetAttributeValue

        #region PublishNotepadText

        public void PublishNotepadText(EventEstablished eventEstablished)
        {
            if (eventEstablished.CallType == CallType.Consult && commonSettings.EnableConsultNotePad && commonSettings.PhoneConsultConnectionID == eventEstablished.ConnID.ToString() && notepadVisiblityEvent != null)
            {
                if (eventEstablished.UserData.ContainsKey("IWAttachedDataInformation"))
                {
                    KeyValueCollection attachData = eventEstablished.UserData["IWAttachedDataInformation"] as KeyValueCollection;
                    string notePadText = string.Empty;
                    if (attachData.ContainsKey("GCS_TransferringNotepad"))
                    {
                        notePadText = attachData["GCS_TransferringNotepad"].ToString();
                        notepadVisiblityEvent(eventEstablished.ConnID.ToString(), true, notePadText);
                    }
                    else
                    {
                        logger.Info("Attached data does not contain TransferringNotepad Key...");
                    }
                }
                else
                {
                    logger.Warn("Eventestablished does not contain attached data information for consult notepad text..");
                }
            }
        }

        #endregion PublishNotepadText

        #region HideCustomNotepadView

        public void HideCustomNotepadView(EventPartyChanged eventPartyChanged)
        {
            if (notepadVisiblityEvent != null)
            {
                notepadVisiblityEvent(eventPartyChanged.PreviousConnID.ToString(), false, null);
            }
        }

        #endregion HideCustomNotepadView
    }
}