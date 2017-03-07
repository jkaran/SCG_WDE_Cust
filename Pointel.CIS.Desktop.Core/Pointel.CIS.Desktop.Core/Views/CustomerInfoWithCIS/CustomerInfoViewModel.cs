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
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfo
{
    /// <summary>
    /// Comment: ViewModel for CustomerInfo.xaml
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    public class CustomerInfoViewModel : ICustomerInfoViewModel, INotifyPropertyChanged
    {
        #region Field Declaration

        private ObservableCollection<IMyListItem> _DataCollection;
        private ObservableCollection<IMyListItem> _AccountCollection;
        private ObservableCollection<IMyListItem> _AddressCollection;
        private ObservableCollection<IMyListItem> _PhoneCollection;
        private ObservableCollection<IMyListItem> _IVRCollection;
        private ObservableCollection<IMyListItem> _NoticeAmountDataCollection;
        private string connectionID = string.Empty;
        private Log log;
        private Settings commonSettings;
        private CIS.CISSettings cisObject = null;
        private CISIntegrationUtility _cisIntegrationUtility;

        #endregion Field Declaration

        #region Constructor

        public CustomerInfoViewModel()
        {
            commonSettings = Settings.GetInstance();
            log = Log.GenInstance();
            if (Settings.GetInstance().Enable_CallInfo_BottomLine)
                this.BorderThickness = "0,0,0,1";

            cisObject = CIS.CISSettings.GetInstance();
            // this.WindowTitle = Settings.GetInstance().CustomerInfoWndTitle;
            this.Update = new CISCommand(CISUpdate);
            this.Continue = new CISCommand(CISContinue);
            this.Search = new CISCommand(CISSearch);
            _cisIntegrationUtility = CISIntegrationUtility.GetInstance();
        }

        #endregion Constructor

        #region CISCaseViewModel Members

        /// <summary>
        /// Gets or sets my collection.
        /// </summary>
        /// <value>
        /// My collection.
        /// </value>
        public ObservableCollection<IMyListItem> CallInfoDataCollection
        {
            get { return _DataCollection; }
            set { if (_DataCollection != value) { _DataCollection = value; OnPropertyChanged("CallInfoDataCollection"); } }
        }

        public ObservableCollection<IMyListItem> AccountDataCollection
        {
            get { return _AccountCollection; }
            set { if (_AccountCollection != value) { _AccountCollection = value; OnPropertyChanged("AccountDataCollection"); } }
        }

        public ObservableCollection<IMyListItem> AddressDataCollection
        {
            get { return _AddressCollection; }
            set { if (_AddressCollection != value) { _AddressCollection = value; OnPropertyChanged("AddressDataCollection"); } }
        }

        public ObservableCollection<IMyListItem> PhoneDataCollection
        {
            get { return _PhoneCollection; }
            set { if (_PhoneCollection != value) { _PhoneCollection = value; OnPropertyChanged("PhoneDataCollection"); } }
        }

        public ObservableCollection<IMyListItem> IVRDataCollection
        {
            get { return _IVRCollection; }
            set { if (_IVRCollection != value) { _IVRCollection = value; OnPropertyChanged("IVRDataCollection"); } }
        }

        public string ConnectionID
        {
            get { return connectionID; }
            set { if (connectionID != value) { connectionID = value; OnPropertyChanged("ConnectionID"); } }
        }

        public ObservableCollection<IMyListItem> NoticeAmountDataCollection
        {
            get { return _NoticeAmountDataCollection; }
            set { if (_NoticeAmountDataCollection != value) { _NoticeAmountDataCollection = value; OnPropertyChanged("NoticeAmountDataCollection"); } }
        }

        public ICase Case
        {
            get;
            set;
        }

        private string borderThickness = "0,0,0,0";

        public string BorderThickness
        {
            get
            {
                return borderThickness;
            }
            set
            {
                borderThickness = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        private string title = "Customer Info";

        public string WindowTitle
        {
            get
            {
                return title;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    title = value;
                    OnPropertyChanged("WindowTitle");
                }
            }
        }

        #endregion CISCaseViewModel Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        #region Commands

        public CISCommand Update
        {
            get;
            set;
        }

        public CISCommand Continue
        {
            get;
            set;
        }

        public CISCommand Search
        {
            get;
            set;
        }

        #endregion Commands

        #region CISContinue

        private void CISContinue()
        {
            log.Info("_cisIntegrationUtility + CIS Continue ");
            _cisIntegrationUtility.CISContinue(this.CISObject);
        }

        #endregion CISContinue

        #region CISUpdate

        private void CISUpdate()
        {
            _cisIntegrationUtility.CISUpdate(this.CISObject, this.Interaction);
        }

        #endregion CISUpdate

        #region CISSearch

        private void CISSearch()
        {
            _cisIntegrationUtility.CISSearch(this.CISObject);
        }

        #endregion CISSearch

        #region CISObject

        public CIS.CISSettings CISObject
        {
            get;
            set;
        }

        #endregion CISObject

        #region Interaction

        public Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction Interaction
        {
            get;
            set;
        }

        #endregion Interaction
    }
}