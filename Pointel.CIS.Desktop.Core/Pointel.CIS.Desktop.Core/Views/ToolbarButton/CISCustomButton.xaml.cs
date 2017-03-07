using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Inputs;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Util;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pointel.CIS.Desktop.Core.Views.ToolbarButton
{
    /// <summary>
    /// Interaction logic for CISVoiceCustomButton.xaml
    /// </summary>
    public partial class CISCustomButton : UserControl, ICISButton
    {
        #region Button911 Variables

        private readonly IObjectContainer Container;
        private ILogger log = null;
        private readonly ICommandManager CommandManager;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private Settings commonSettings = null;
        private IKeyboardManager keyBoardManager = null;

        #endregion Button911 Variables

        #region Constructor

        public CISCustomButton(ICISCustomButtonViewModel model, IObjectContainer container, ILogger logger)
        {
            commonSettings = Settings.GetInstance();
            this.Container = container;
            keyBoardManager = this.Container.Resolve<IKeyboardManager>();
            this.Model = model;
            log = logger.CreateChildLogger("CIS Button View");
            this.CommandManager = container.Resolve<ICommandManager>();
            log.Debug("CIS Button created");
            InitializeComponent();
            try
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
            catch (Exception generalException)
            {
                this.log.Error("Error Occurred while Registering CIS Shortcut Keys " + generalException.ToString());
            }
        }

        #endregion Constructor

        #region IView Members

        public void Destroy()
        {
        }

        public void Create()
        {
            try
            {
                ICase Case = Extensions.TryGetValue<string, object>(this.Context as IDictionary<string, object>, "Case") as ICase;
                if (Case != null && Case.MainInteraction != null)
                {
                    if (Case.MainInteraction.HasChildInteraction)
                    {
                        // collapse visibility of CIS Toolbar, since it is first agent consult call
                        CISToolbar.Visibility = Visibility.Collapsed;
                        log.Info("CIS Toolbar visibility collapsed for consult call, Interaction Id: " + this.Model.Interaction.InteractionId);
                    }
                    else
                    {
                        this.Model.Interaction = Case.MainInteraction;
                        log.Info("ActiveX on Page Create");
                        host = new System.Windows.Forms.Integration.WindowsFormsHost();
                        this.Model.CISObject = CIS.CISSettings.GetInstance();
                        //searchCIS = new AxCISIVRConnection.AxCISIVRConn();
                        host.Child = this.Model.CISObject.CISConnection;
                        wPanel.Children.Add(host);
                        log.Info("ActiveX on Page Created " + wPanel.Children.Count.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error on CIS Button Page Create " + generalException.ToString());
            }
        }

        public object Context { get; set; }

        #endregion IView Members

        #region Model

        public ICISCustomButtonViewModel Model
        {
            get
            {
                return this.DataContext as ICISCustomButtonViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        #endregion Model
    }
}