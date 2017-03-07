using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.Windows.Event;
using Genesyslab.Platform.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfo
{
    /// <summary>
    /// Interaction logic for CustoInfoSideButton.xaml
    /// </summary>

    public partial class CustomerInfoSideButton : UserControl, ICustomerInfoSideButton
    {
        private IObjectContainer objContainer;
        private IViewEventManager viewEventManager;
        private ILogger log = null;
        private object caseObject;
        private ICase caseDetails;
        private IViewManager viewManager;

        public CustomerInfoSideButton(IObjectContainer _container, IViewEventManager _viewEventManager, IViewManager _viewManager, ILogger logger)
        {
            this.objContainer = _container;
            this.viewEventManager = _viewEventManager;
            this.viewManager = _viewManager;
            log = logger.CreateChildLogger("CustomerInfo Side Button View");
            log.Info("CustomerInfoSideButton :Load the CustomerInfoSideButton View");
            InitializeComponent();
            Width = Double.NaN;
            Height = Double.NaN;
        }

        public object Context
        {
            get;
            set;
        }

        public void Create()
        {
        }

        public void Destroy()
        {
        }

        private void splitToggleButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("CustomerInfoSideButton:splitToggleButton_Click() -Toggle Button is Clicked..");

            (Context as IDictionary<string, object>).TryGetValue("Case", out caseObject);
            caseDetails = caseObject as ICase;
            viewEventManager.Publish(new GenericEvent()
            {
                Target = GenericContainerView.ContainerView,
                Context = caseDetails.CaseId,
                Action = new GenericAction[]
                {
                    new GenericAction ()
                    {
                        Action = ActionGenericContainerView.ShowHidePanelRight,
                        Parameters = new object[] { splitToggleButton.IsChecked ?? false ? Visibility.Visible : Visibility.Collapsed, "MyCustomerInfo" }
                    },
                    new GenericAction ()
                    {
                        Action = ActionGenericContainerView.ActivateThisPanel,
                        Parameters = new object[] { "MyCustomerInfo" }
                    }
                }
            });
        }

        private void splitToggleButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.splitToggleButton.IsChecked.HasValue || !this.splitToggleButton.IsChecked.Value)
                return;
            this.splitToggleButton.IsChecked = new bool?(false);
        }
    }
}