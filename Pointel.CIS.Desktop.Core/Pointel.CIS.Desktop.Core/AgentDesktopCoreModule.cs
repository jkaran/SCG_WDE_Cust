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

using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.SDK.Configurations;
using Genesyslab.Desktop.Modules.Windows.Event;
using Genesyslab.Desktop.Modules.Windows.Views.Toolbar.MyWorkplace.PlaceStatus;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Configuration;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CIS.Desktop.Core.Views.CustomerInfo;
using Pointel.CIS.Desktop.Core.Views.CustomerInfoWithCIS;
using Pointel.CIS.Desktop.Core.Views.CustomReasonCode;
using Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu;
using Pointel.CIS.Desktop.Core.Views.ToolbarButton;
using Pointel.CIS.Desktop.Core.Voice;
using Pointel.CustomGlobalStatusMenu.GlobalMenu;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Pointel.CIS.Desktop.Core
{ /// <summary>
    /// Comment: Registering AgentDesktopCoreModule module and views
    /// Last Modified: 13-Apr-2016
    /// Created by: Pointel Inc
    /// </summary>
    ///
    public class AgentDesktopCoreModule : IModule
    {
        #region Fields

        private IObjectContainer container;
        private ILogger logger;
        private IViewEventManager eventManager;
        private IConfigurationService configurationService;
        private ReadConfigurationData configuration = null;
        private IViewManager viewManager;
        private Settings commonSettings = null;
        internal static SubscribeAgentVoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        public AgentDesktopCoreModule(IObjectContainer container, ILogger log, IViewEventManager eventManager, IViewManager view)
        {
            try
            {
                this.container = container;
                this.eventManager = eventManager;
                this.viewManager = view;
                this.logger = log.CreateChildLogger("AgentDesktopCoreModule");
                this.logger.Info("AgentDesktopCoreModule()");
                this.logger.Info("Assembly Name : " + Assembly.GetExecutingAssembly().GetName().Name);
                this.logger.Info("Assembly Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                Log.GenInstance().CreateLogger(this.logger);
            }
            catch (Exception generalEXception)
            {
                this.logger.Error("Error occured while loading CIS integration core " + generalEXception.ToString());
            }
        }

        #endregion Constructor

        #region Initialize

        public void Initialize()
        {
            try
            {
                this.logger.Info("Initialize()");
                configurationService = container.Resolve<IConfigurationService>();

                this.eventManager.Subscribe(CoreEventHandler);
                commonSettings = Settings.GetInstance();
                commonSettings.CallInfoDataPublisher = new Publisher<CallInfoDataCollection>();

                //Read CIS Configuration
                commonSettings.AgentDetails = container.Resolve<IAgent>();
                commonSettings.ApplicationDetails = configurationService.MyApplication;
                configuration = new ReadConfigurationData();
                commonSettings.CISConfiguration = configuration.ReadCISIntegrationConfigurations(commonSettings.AgentDetails, commonSettings.ApplicationDetails);

                if (commonSettings.CISConfiguration != null)
                {
                    configuration.IntializeCISProperty();

                    if (commonSettings.CISConfiguration.ContainskeyAndValue("enable.integration"))
                    {
                        if (Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("enable.integration")))
                        {
                            this.container.RegisterType<ICustomerInfoViewModel, CustomerInfoViewModel>();
                            this.container.RegisterType<ICustomGlobalMenuView, CustomGlobalMenuView>();
                            this.container.RegisterType<IPlaceStatusViewModel, CustomPlaceStatusViewModel>();
                            //custom global status menu
                            logger.Debug("Registering Custom AgentStatus Menu in Toolbar window......");
                            this.viewManager.ViewsByRegionName["CustomToolBarRegion"].
                                     Insert(0, new ViewActivator() { ViewType = typeof(ICustomGlobalMenuView), ViewName = "CustomGlobalStatusMenu", ActivateView = true });

                            if (commonSettings.Enable_Voice_Bar_Buttons)
                            {
                                //CIS buttons in Voice Toolbar window
                                logger.Debug("Registering CIS Buttons in Voice Toolbar view");
                                container.RegisterType<ICISButton, CISCustomButton>();
                                container.RegisterType<ICISCustomButtonViewModel, CISCustomButtonViewModel>();
                                viewManager.ViewsByRegionName["InteractionVoiceCustomButtonRegion"].
                                    Insert(0, new ViewActivator() { ViewType = typeof(ICISButton), ViewName = "CISButton", ActivateView = true });
                            }
                            else
                            {
                                logger.Info("CIS button in Voice Toolbar view is disabled");
                            }

                            logger.Debug("Registering CustomerInfo Side Button View......");
                            container.RegisterType<ICustomerInfoSideButton, CustomerInfoSideButton>();
                            viewManager.ViewsByRegionName["CaseViewSideButtonRegion"].Add(new ViewActivator()
                            {
                                ViewType = typeof(ICustomerInfoSideButton),
                                ViewName = "CustomInfoButton",
                                //SortIndexOrder=3,
                                ActivateView = true
                            });
                            logger.Debug("Registering CustomerInfo View in InteractionWorksheetRegion.....");
                            container.RegisterType<ICustomerInfoView, CustomerInfoInWorkSheetRegion>();
                            viewManager.ViewsByRegionName["InteractionWorksheetRegion"].Add(new ViewActivator()
                            {
                                ViewType = typeof(CustomerInfoInWorkSheetRegion),
                                ViewName = "MyCustomerInfo",
                                ActivateView = true
                            });
                        }
                        else
                            logger.Info("CIS Integration is disabled");
                    }
                    else
                        logger.Info("Enabling CIS Integration Key is not found");
                }
                else
                    this.logger.Info("Unable to load CIS Integration becuase CIS Configuration is not found");
            }
            catch (Exception generalException)
            {
                this.logger.Info("Error at AgentDesktopCoreModule Initialize() :" + generalException.Message);
            }
        }

        #endregion Initialize

        #region CoreEventHandler

        private void CoreEventHandler(object eventObject)
        {
            string eventMessage = eventObject as string;
            try
            {
                if (eventMessage != null)
                {
                    switch (eventMessage)
                    {
                        case "loggin":
                            logger.Debug("loggin Event Trap");
                            break;

                        case "Login":
                            try
                            {
                                logger.Debug("login Event Trap");
                                //Subscribe Voice Events
                                voiceEvents = new SubscribeAgentVoiceEvents(this.container);
                                SetAgentLoginStatus.GetInstance().SetInitialVoiceMediaStatus();
                                // Launch CIS Application at WDE Login
                                LaunchCISApplication();
                            }
                            catch (Exception generalException)
                            {
                                logger.Error("Error at CoreEventHandler() :" + generalException.Message);
                            }
                            break;

                        case "Logout":
                            logger.Debug("logout Event Trap");
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("CoreEventHandler: Error occurred, Exception :" + generalException.ToString());
            }
        }

        #endregion CoreEventHandler

        #region Launch CIS Application

        private void LaunchCISApplication()
        {
            try
            {
                //CIS Application Launch
                if (commonSettings.CISConfiguration.ContainskeyAndValue("enable.cis-launch") &&
                        Convert.ToBoolean(commonSettings.CISConfiguration.GetAsString("enable.cis-launch")))
                {
                    if (commonSettings.CISConfiguration.ContainskeyAndValue("cis-launch.process-name"))
                    {
                        if (commonSettings.CISConfiguration.ContainskeyAndValue("cis-launch.application-name"))
                        {
                            string filePath = string.Empty;
                            if (commonSettings.CISConfiguration.ContainskeyAndValue("cis-launch.application-path"))
                            {
                                filePath = commonSettings.CISConfiguration.GetAsString("cis-launch.application-path").Trim();
                                try
                                {
                                    Process cisLaunch = new Process();
                                    ProcessStartInfo startInfo = new ProcessStartInfo();
                                    startInfo.FileName = filePath + @"\" + commonSettings.CISConfiguration.GetAsString("cis-launch.process-name");
                                    startInfo.Arguments = commonSettings.CISConfiguration.GetAsString("cis-launch.application-name");
                                    cisLaunch.StartInfo = startInfo;
                                    cisLaunch.Start();
                                }
                                catch (Exception generalException)
                                {
                                    this.logger.Error("Error occurred while launching CIS application " + generalException.ToString());
                                }
                            }
                            else
                            {
                                //MessageBox.Show("Application path is not found, it is mandatory to launch CIS Application");
                                this.logger.Info("CIS Application path is not configured");
                            }
                        }
                        else
                            this.logger.Info("CIS Application name not found");
                    }
                    this.logger.Info("CIS Applicaiton process name not found");
                }
                else
                    this.logger.Info("CIS Application Launch is not enabled");
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred on Launching CIS Application : " + generalException.ToString());
            }
        }

        #endregion Launch CIS Application
    }
}