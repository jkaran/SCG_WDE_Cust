using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Inputs;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CIS.Desktop.Core.Views.CustomerInfo;
using Pointel.CIS.Desktop.Core.Voice;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfoWithCIS
{
    /// <summary>
    /// Interaction logic for CustomerInfoInWorkSheetRegion.xaml
    /// </summary>
    public partial class CustomerInfoInWorkSheetRegion : UserControl, ICustomerInfoView
    {
        #region Field Declaration

        private Subscriber<CallInfoDataCollection> _callInfoData;
        private Settings commonSettings = Settings.GetInstance();
        private ILogger log;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private CIS.CISSettings cisObject = null;
        private IKeyboardManager keyBoardManager = null;
        private IObjectContainer Container = null;

        #endregion Field Declaration

        public CustomerInfoInWorkSheetRegion(ICustomerInfoViewModel model, IObjectContainer container)
        {
            InitializeComponent();
            this.Container = container;
            base.Width = double.NaN;
            base.Height = double.NaN;
            this.Model = model;
            keyBoardManager = this.Container.Resolve<IKeyboardManager>();
            this.log = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("CustomerInfo");
            this.log.Info("CustomerInfo()");
        }

        public ICustomerInfoViewModel Model
        {
            get
            {
                return this.DataContext as ICustomerInfoViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public object Context
        {
            get;
            set;
        }

        public string InteractionId
        {
            get;
            set;
        }

        public void Create()
        {
            try
            {
                log.Info("CustomerInfo view : Create()");
                ICase Case = Extensions.TryGetValue<string, object>(this.Context as IDictionary<string, object>, "Case") as ICase;
                if (Case != null && Case.MainInteraction.Type == "InteractionVoice")
                {
                    InteractionId = Case.MainInteraction.EntrepriseInteractionCurrent.Id;
                    _callInfoData = new Subscriber<CallInfoDataCollection>(commonSettings.CallInfoDataPublisher);
                    SubscribeAgentVoiceEvents.notepadVisiblityEvent += new ConsultNotepadVisiblityEvent(SubscribeAgentVoiceEvents_notepadEvent);
                    _callInfoData.Publisher.DataPublisher += Publisher_DataPublisher;
                    if (commonSettings.Enable_CallInfo_Buttons)
                    {
                        // Registering Shortcut Keys for CIS Commands
                        KeyGestureConverter keyGetstureConverter = new KeyGestureConverter();
                        if (!string.IsNullOrWhiteSpace(commonSettings.CISUpdate_HotKey))
                            keyBoardManager.RegisterInputBinding(this.Model.Update, (InputGesture)keyGetstureConverter.ConvertFrom(commonSettings.CISUpdate_HotKey), KeyBindingSource.Application);
                        if (!string.IsNullOrWhiteSpace(commonSettings.CISContinue_HotKey))
                            keyBoardManager.RegisterInputBinding(this.Model.Continue, (InputGesture)keyGetstureConverter.ConvertFrom(commonSettings.CISContinue_HotKey), KeyBindingSource.Application);
                        if (!string.IsNullOrWhiteSpace(commonSettings.CISSearch_HotKey))
                            keyBoardManager.RegisterInputBinding(this.Model.Search, (InputGesture)keyGetstureConverter.ConvertFrom(commonSettings.CISSearch_HotKey), KeyBindingSource.Application);
                    }

                    if (commonSettings.Enable_CallInfo_Buttons)
                    {
                        BtnGrid.Visibility = Visibility.Visible;
                    }
                    if (commonSettings.CallInfoData.Count > 0)
                    {
                        CallInfoDataCollection currentData = commonSettings.CallInfoData.TryGetValue(Case.MainInteraction.EntrepriseInteractionCurrent.Id);
                        if (currentData != null)
                        {
                            custInfo.Visibility = Visibility.Visible;
                            textBlockMessage.Visibility = Visibility.Collapsed;
                            Model.CallInfoDataCollection = currentData.CallInfoData;
                            Model.AddressDataCollection = currentData.AddressData;
                            Model.AccountDataCollection = currentData.AccountData;
                            Model.NoticeAmountDataCollection = currentData.NoticeAmountData;
                            if (Model.NoticeAmountDataCollection == null || Model.NoticeAmountDataCollection.Count <= 0)
                                ItemsControlNoticeAmountData.Visibility = Visibility.Collapsed;
                            else
                                ItemsControlNoticeAmountData.Visibility = Visibility.Visible;
                            Model.IVRDataCollection = currentData.IVRData;
                            Model.PhoneDataCollection = currentData.PhoneData;
                            Model.ConnectionID = currentData.ConnId;

                            log.Info("CustomerInfo Popup for the ConnectionId :" + currentData.ToString());
                        }
                        else
                        {
                            log.Info("CustomerInfo Popup Data is not available for the ConnectionId :" + Case.MainInteraction.EntrepriseInteractionCurrent.Id);
                            textBlockMessage.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        log.Info("CustomerInfo Popup Data is not available for the ConnectionId :" + Case.MainInteraction.EntrepriseInteractionCurrent.Id);
                        textBlockMessage.Visibility = Visibility.Visible;
                    }

                    if (Case != null)
                    {
                        this.Model.Interaction = Case.MainInteraction;
                        log.Info("ActiveX on Page Create");
                        host = new System.Windows.Forms.Integration.WindowsFormsHost();
                        cisObject = CIS.CISSettings.GetInstance();
                        //searchCIS = new AxCISIVRConnection.AxCISIVRConn();
                        host.Child = cisObject.CISConnection;
                        host.Visibility = Visibility.Collapsed;
                        wPanel.Children.Add(host);

                        log.Info("ActiveX on Page Created Count : " + wPanel.Children.Count.ToString());
                    }
                }
                else
                {
                    textBlockMessage.Visibility = Visibility.Visible;
                    log.Info("CustomerInfo Display is not enabled for the type : " + Case.MainInteraction.Type);
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error Occurred on Create() method : " + generalException.ToString());
            }
        }

        private void SubscribeAgentVoiceEvents_notepadEvent(string interactionId, bool enable, string notePadText)
        {
            log.Info("Notepad Event for the InteractionId: " + interactionId);
            if (this.InteractionId == interactionId || (commonSettings.ConsultConnectionIds.ContainsKey(this.InteractionId) && commonSettings.ConsultConnectionIds[this.InteractionId] == interactionId))
            {
                if (enable)
                {
                    log.Info("Notepad Text: " + notePadText);
                    this.NotesTab.Visibility = Visibility.Visible;
                    if (!string.IsNullOrEmpty(notePadText))
                        this.txtEditor.SetText(notePadText);
                    this.Height = double.NaN;
                    this.Width = double.NaN;
                }
                else
                {
                    log.Info("Removing custom notepad view for the interaction id: " + interactionId);
                    this.NotesTab.Visibility = Visibility.Collapsed;
                    this.txtEditor.Visibility = Visibility.Collapsed;
                    this.CustomerInfoTab.Focus();
                    this.TabControlCustomerInfo.SelectedIndex = 0;
                }
            }
            else
            {
                log.Info("Notepad update will not happen at this time, because current interaction id does not match with event data, Current Id :" + this.InteractionId + "\t interaction id from event :" + interactionId);
            }
        }

        public void Destroy()
        {
            try
            {
                if (commonSettings.ConsultConnectionIds.ContainsKey(this.InteractionId))
                    commonSettings.ConsultConnectionIds.Remove(this.InteractionId);

                if (_callInfoData != null)
                    _callInfoData.Publisher.DataPublisher -= Publisher_DataPublisher;
                SubscribeAgentVoiceEvents.notepadVisiblityEvent -= new ConsultNotepadVisiblityEvent(SubscribeAgentVoiceEvents_notepadEvent);
            }
            catch (Exception)
            {
            }
        }

        private void Publisher_DataPublisher(object sender, MessageArgument<CallInfoDataCollection> e)
        {
            try
            {
                log.Info("Publisher_DataPublisher:Screen popup update-Bind all collection of data");
                if (this.InteractionId == e.Message.ConnId || (commonSettings.ConsultConnectionIds.ContainsKey(this.InteractionId) && commonSettings.ConsultConnectionIds[this.InteractionId] == e.Message.ConnId))
                {
                    custInfo.Visibility = Visibility.Visible;
                    textBlockMessage.Visibility = Visibility.Collapsed;
                    Model.CallInfoDataCollection = e.Message.CallInfoData;
                    Model.AddressDataCollection = e.Message.AddressData;
                    Model.AccountDataCollection = e.Message.AccountData;
                    Model.NoticeAmountDataCollection = e.Message.NoticeAmountData;
                    if (Model.NoticeAmountDataCollection == null || Model.NoticeAmountDataCollection.Count <= 0)
                        ItemsControlNoticeAmountData.Visibility = Visibility.Collapsed;
                    else
                    {
                        log.Info("Publisher_DataPublisher:Notice amount collection is enabled.");
                        ItemsControlNoticeAmountData.Visibility = Visibility.Visible;
                    }
                    Model.PhoneDataCollection = e.Message.PhoneData;
                    Model.IVRDataCollection = e.Message.IVRData;
                    Model.ConnectionID = e.Message.ConnId;

                    log.Info("Update CustomerInfo Popup for the ConnectionId : " + e.Message.ToString());
                }
                else
                {
                    log.Info("Customer Info update will not happen at this time, because current interaction id does not match with event data, Current Id :" + this.InteractionId + "\t interaction id from event :" + e.Message.ConnId);
                    return;
                }
            }
            catch (Exception generalException)
            {
                this.log.Error("Error Occurred while updating CustomerInfo view :" + generalException.ToString());
            }
        }
    }
}