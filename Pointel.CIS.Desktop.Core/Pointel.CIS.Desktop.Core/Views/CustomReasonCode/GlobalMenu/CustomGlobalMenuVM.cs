using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.DisplayFormat;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.SDK.Configurations;
using Genesyslab.Desktop.Modules.Windows;
using Genesyslab.Desktop.Modules.Windows.Views.Toolbar;
using Genesyslab.Desktop.WPFCommon;
using Genesyslab.Desktop.WPFCommon.Controls;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tomers.WPF.Localization;
using System.Windows.Media;

namespace Pointel.CustomGlobalStatusMenu.GlobalMenu
{
    public class CustomGlobalMenuVM : INotifyPropertyChanged
    {
        #region Fields and properties
        
        private Genesyslab.Desktop.Infrastructure.DependencyInjection.IObjectContainer Container;

        private string sourceStatus;

        public string SourceStatus
        {
            get { return sourceStatus; }
            set
            {
                sourceStatus = value;
                NotifyPropertyChanged();
            }
        }
        private string resourceKeyStatus;

        public string ResourceKeyStatus
        {
            get { return resourceKeyStatus; }
            set
            {
                resourceKeyStatus = value;
                NotifyPropertyChanged();
            }
        }
        private IAgentStateItem agentStatusItem;

        public IAgentStateItem AgentStatusItem
        {
            get { return agentStatusItem; }
            set
            {
                agentStatusItem = value;
                SourceStatus = agentStatusItem.SourceStatus;
                ResourceKeyStatus = agentStatusItem.ResourceKeyStatus;
                NotifyPropertyChanged();
            }
        }

       
        #endregion
        public CustomGlobalMenuVM(Genesyslab.Desktop.Infrastructure.DependencyInjection.IObjectContainer Container)
        {
            // TODO: Complete member initialization
            this.Container = Container;
            agentStatusItem = Container.Resolve<IAgentStateItem>();
         
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
