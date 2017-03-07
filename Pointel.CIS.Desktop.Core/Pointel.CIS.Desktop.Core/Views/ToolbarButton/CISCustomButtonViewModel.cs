
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
using Genesyslab.Desktop.Infrastructure.Configuration;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Infrastructure.ExceptionAnalyze;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Commands;
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Pointel.CIS.Desktop.Core.Views.ToolbarButton
{
    public class CISCustomButtonViewModel : ICISCustomButtonViewModel ,INotifyPropertyChanged
    {
        #region Field Declaration
        ILogger log = null;
        IConfigManager ConfigManager;
        private Settings commonSettings = null;
        CISIntegrationUtility _cisIntegrationUtility;
        #endregion

        #region CISCustomButtonViewModel
        public CISCustomButtonViewModel(ILogger log, IConfigManager configManager)
        {
            this.log = log.CreateChildLogger("CISCustomButtonViewModel");
            commonSettings = Settings.GetInstance();
            _cisIntegrationUtility = CISIntegrationUtility.GetInstance();
            this.Update = new CISCommand(UpdateAction);
            this.Continue = new CISCommand(ContinueAction);
            this.Search = new CISCommand(SearchAction);
            ConfigManager = configManager;

           
        }
        #endregion

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
        #endregion

        #region CISObject
        public CIS.CISSettings CISObject
        {
            get;
            set;
        }
        #endregion

        #region Interaction
        public Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction Interaction
        {
            get;
            set;
        }
        #endregion

       private void SearchAction()
        {
            _cisIntegrationUtility.CISSearch(this.CISObject);
        }
       private void UpdateAction()
       {
           _cisIntegrationUtility.CISUpdate(this.CISObject, this.Interaction);
       }
       private void ContinueAction()
       {
           _cisIntegrationUtility.CISContinue(this.CISObject);
       }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}