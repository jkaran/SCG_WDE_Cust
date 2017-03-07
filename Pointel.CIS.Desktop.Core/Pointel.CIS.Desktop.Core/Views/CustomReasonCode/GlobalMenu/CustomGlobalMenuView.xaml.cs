using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Configuration;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Inputs;
using Genesyslab.Desktop.Modules.Core.DisplayFormat;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.SDK.Configurations;
using Genesyslab.Desktop.Modules.Windows.Event;
using Genesyslab.Desktop.WPFCommon;
using Genesyslab.Desktop.WPFCommon.Controls;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CustomGlobalStatusMenu.GlobalMenu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tomers.WPF.Localization;

namespace Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu
{
    /// <summary>
    /// Interaction logic for CustomGlobalMenuView.xaml
    /// </summary>
    public partial class CustomGlobalMenuView : UserControl, ICustomGlobalMenuView
    {
        #region Fields

        public static IAgent agent;
        private IConfigManager _configManger;
        private Settings _settings;
        private IViewEventManager _viewEventManager;
        private IList<CfgActionCode> _voiceActionCodes = null;
        private Binding binding;
        private IConfigurationService confService;
        private IObjectContainer Container;
        private bool isINOFS = false;
        private IKeyboardManager keyBoardManager = null;
        private ILogger logger;
        public System.Windows.Input.ICommand MediaLogOffCommand { get { return new LogOffCommand(); } }
        public System.Windows.Input.ICommand MediaLogOnCommand { get { return new LogOnCommand(); } }
        public System.Windows.Input.ICommand MediaReadyCommand { get { return new ReadyCommand(); } }
        public System.Windows.Input.ICommand MediaStatusDndOnCommand { get { return new DndOnCommand(); } }
        public System.Windows.Input.ICommand MediaStatusNotReadyActionCodeCommand { get { return new NotReadyActionCodeCommand(); } }
        public System.Windows.Input.ICommand MediaStatusNotReadyAfterCallWorkCommand { get { return new AfterCallWorkCommand(); } }
        public System.Windows.Input.ICommand MediaStatusNotReadyCommand { get { return new NotReadyCommand(); } }

        private Timer contextMenuTimer = new Timer(3000);

        #endregion Fields

        #region Constructor

        private static readonly string[] defaultCommands = new string[]
		{
			StateMenuCommand.ReadyCommand,
			StateMenuCommand.NotReadyCommand,
			StateMenuCommand.NotReadyReasonCommand,
			StateMenuCommand.AfterCallWorkCommand,
			StateMenuCommand.DndCommand,
			StateMenuCommand.LogOnCommand,
			StateMenuCommand.LogOffCommand
		};

        public CustomGlobalMenuView(IObjectContainer _container, IConfigurationService _confService, IViewEventManager viewEventManager)
        {
            InitializeComponent();
            this.DataContext = this;
            Container = _container;
            this.logger = this.Container.Resolve<ILogger>().CreateChildLogger("ToolbarCustomButtonView");
            keyBoardManager = this.Container.Resolve<IKeyboardManager>();
            _configManger = this.Container.Resolve<IConfigManager>();
            confService = _confService;
            agent = Container.Resolve<IAgent>();
            _settings = Settings.GetInstance();
            // agent.MediaCapacityChangedEvent += agent_MediaCapacityChangedEvent;
            _viewEventManager = viewEventManager;
            agent.LoggedOn += agent_LoggedOn;
            agent.LoggedOff += agent_LoggedOff;
            agent.MyState.PropertyChanged += MyState_PropertyChanged;
            ReadCustomActionCodes customCodes = new ReadCustomActionCodes();
            _voiceActionCodes = customCodes.ReadActionCodes("Voice", confService.ConfigService);
            SortActionCodeList();
            FilterReasonCodes();
            binding = new Binding("IsHighlighted");
            binding.Converter = new MagicModeConverter();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MenuItem), 1);

            this.Container.Resolve<IViewEventManager>().Subscribe(new Action<object>(this.MyEventHandler));

            if (_settings.CISConfiguration.ContainskeyAndValue("scg.custom.status-button.text"))
                StatusBtn.Content = _settings.CISConfiguration.GetAsString("scg.custom.status-button.text");
            else
                StatusBtn.Content = "Agent Status";
            try
            {
                // Registering Shortcut Keys for Agent Status Commands
                KeyGestureConverter keyGetstureConverter = new KeyGestureConverter();

                #region HotKey Region

                if (_configManger.ContainsKey("keyboard.hotkey.agent-ready") && !string.IsNullOrEmpty(_configManger["keyboard.hotkey.agent-ready"].ToString()))
                    keyBoardManager.RegisterInputBinding(MediaReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom(_configManger["keyboard.hotkey.agent-ready"].ToString()), KeyBindingSource.WindowsHotkey);
                if (_configManger.ContainsKey("keyboard.hotkey.agent-not-ready") && !string.IsNullOrEmpty(_configManger["keyboard.hotkey.agent-not-ready"].ToString()))
                    keyBoardManager.RegisterInputBinding(MediaStatusNotReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom(_configManger["keyboard.hotkey.agent-not-ready"].ToString()), KeyBindingSource.WindowsHotkey);

                #endregion HotKey Region

                contextMenuTimer.Elapsed += contextMenuTimer_Elapsed;
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:Error Occurred while Registering Agent Shortcut Keys " + generalException.ToString());
            }
        }

        private void SortActionCodeList()
        {
            if (_voiceActionCodes != null)
            {
                _voiceActionCodes = _voiceActionCodes.OrderBy(o => o.Name).ToList();
            }
        }

        private void contextMenuTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                contextMenuTimer.Stop();
                if (agent.MyState.State == Status.Logout)
                    return;
                else if (agent.MyState.State == Status.OutOfService)
                {
                    DisableMenuItem("ofs");
                }
                else
                    DisableMenuItem("logon");
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:contextMenuTimer_Elapsed() : " + generalException.ToString());
            }
        }

        private void agent_LoggedOff(object sender, EventArgs e)
        {
            DisableMenuItem("logoff");
            contextMenuTimer.Start();
        }

        private void agent_LoggedOn(object sender, EventArgs e)
        {
            DisableMenuItem("logon");
        }

        private void FilterReasonCodes()
        {
            try
            {
                this.logger.Info("FilterReasonCodes: Filtering NotReady Reason Codes....");
                if (_settings.NotReadyActionCodeFilters != null && _settings.NotReadyActionCodeFilters.Length > 0)
                {
                    var data1 = _voiceActionCodes.ToList();

                    IList<CfgActionCode> FilterCodes = new List<CfgActionCode>();
                    foreach (string filter in _settings.NotReadyActionCodeFilters)
                    {
                        foreach (CfgActionCode code in data1)
                        {
                            if (code.Name.Equals(filter, System.StringComparison.OrdinalIgnoreCase))
                            {
                                FilterCodes.Add(code);
                                break;
                            }
                        }
                    }

                    if (FilterCodes.Count > 0)
                    {
                        _voiceActionCodes.Clear();
                        foreach (CfgActionCode actionCode in FilterCodes)
                        {
                            _voiceActionCodes.Add(actionCode);
                        }
                    }
                }
                else
                {
                    this.logger.Info("FilterReasonCodes: No filter configuration found");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:FilterReasonCodes() :" + generalException.ToString());
            }
        }

        private void MyEventHandler(object obj)
        {
            try
            {
                string str2;
                string str = obj as string;
                if ((str != null) && ((str2 = str) != null))
                {
                    if (!(str2 == "Login"))
                    {
                        if (!(str2 == "Logout"))
                        {
                            return;
                        }
                    }
                    else
                    {
                        KeyGestureConverter keyGetstureConverter = new KeyGestureConverter();
                        if (_configManger.ContainsKey("keyboard.shortcut.state.ready") && !string.IsNullOrEmpty(_configManger["keyboard.shortcut.state.ready"].ToString()))
                            keyBoardManager.RegisterInputBinding(MediaReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom(_configManger["keyboard.shortcut.state.ready"].ToString()), KeyBindingSource.Application);
                        else
                            keyBoardManager.RegisterInputBinding(MediaReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom("Ctrl+Alt+R"), KeyBindingSource.Application);

                        if (_configManger.ContainsKey("keyboard.shortcut.state.not-ready") && !string.IsNullOrEmpty(_configManger["keyboard.shortcut.state.not-ready"].ToString()))
                            keyBoardManager.RegisterInputBinding(MediaStatusNotReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom(_configManger["keyboard.shortcut.state.not-ready"].ToString()), KeyBindingSource.Application);
                        else
                            keyBoardManager.RegisterInputBinding(MediaStatusNotReadyCommand, (InputGesture)keyGetstureConverter.ConvertFrom("Ctrl+Alt+N"), KeyBindingSource.Application);

                        if (_configManger.ContainsKey("keyboard.shortcut.state.not-ready-after-call-work") && !string.IsNullOrEmpty(_configManger["keyboard.shortcut.state.not-ready-after-call-work"].ToString()))
                            keyBoardManager.RegisterInputBinding(MediaStatusNotReadyAfterCallWorkCommand, (InputGesture)keyGetstureConverter.ConvertFrom(_configManger["keyboard.shortcut.state.not-ready-after-call-work"].ToString()), KeyBindingSource.Application);
                        else
                            keyBoardManager.RegisterInputBinding(MediaStatusNotReadyAfterCallWorkCommand, (InputGesture)keyGetstureConverter.ConvertFrom("Ctrl+Alt+Z"), KeyBindingSource.Application);
                        //WindowsModule.cs Line no 209
                        //foreach (string str3 in WindowsOptions.Default.GetHotkeyAgentNotReadyWithReasonActionCodes())
                        //{
                        //    keyBoardManager.RegisterInputBinding(MediaStatusNotReadyActionCodeCommand, WindowsOptions.Default.GetHotkeyAgentNotReadyWithReason(str3), KeyBindingSource.WindowsHotkey, str3, string.Format("ApplicationCore.AgentStatusNotReadyCommand({0})", str3), string.Format("keyboard.hotkey.agent-not-ready-with-reason.{0}", str3));
                        //}
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:Error occuring in shortcut keys : " + generalException.ToString());
            }
        }

        private void MyState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var MyAgentStatus = sender as IMyAgentState;
                if (e != null && e.PropertyName == "State")
                {
                    if (MyAgentStatus != null && MyAgentStatus.State == Status.OutOfService && !isINOFS)
                    {
                        DisableMenuItem("ofs");
                    }
                    else
                    {
                        if (MyAgentStatus != null)
                        {
                            if (MyAgentStatus.State == Status.Logout || MyAgentStatus.State == Status.LogoutDndOn)
                                DisableMenuItem("logoff");
                            else if (MyAgentStatus.State == Status.NotReady || MyAgentStatus.State == Status.NotReadyActionCode
                                || MyAgentStatus.State == Status.NotReadyAfterCallWork || MyAgentStatus.State == Status.PartialReady
                                || MyAgentStatus.State == Status.Ready)
                                DisableMenuItem("logon");
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:MyState_PropertyChanged() :" + generalException.Message);
            }
        }

        private void SetInitialState()
        {
            try
            {
                if (agent.MyState.State == Status.Logout || agent.MyState.State == Status.LogoutDndOn)
                    DisableMenuItem("logoff");
                else if (agent.MyState.State == Status.NotReady || agent.MyState.State == Status.NotReadyActionCode
                    || agent.MyState.State == Status.NotReadyAfterCallWork || agent.MyState.State == Status.PartialReady
                    || agent.MyState.State == Status.Ready)
                    DisableMenuItem("logon");
                else if (agent.MyState.State == Status.OutOfService)
                    DisableMenuItem("ofs");
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetInitialState() : " + generalException.ToString());
            }
        }

        #endregion Constructor

        #region Context

        public object Context
        {
            get;
            set;
        }

        #endregion Context

        #region Create

        public void Create()
        {
            try
            {
                agent.NotReadyActionCodes.Clear();
                if (_settings.AgentLevelActionCodes == null)
                {
                    SetContextMenuReady();
                    SetContextMenuNoReady();
                    SetContextMenu();
                    SetContextMenuDND();
                    SetContextMenuACW();
                    SetContextMenuLogOn();
                    SetContextMenuLogOff();
                }
                else
                {
                    string[] array = GetValueAsStringArray(_settings.AgentLevelActionCodes, defaultCommands);
                    for (int i = 0; i < array.Length; i++)
                    {
                        string value = array[i];
                        if (StateMenuCommand.ReadyCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuReady();
                        }
                        else if (StateMenuCommand.NotReadyCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuNoReady();
                        }
                        else if (StateMenuCommand.DndCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuDND();
                        }
                        else if (StateMenuCommand.AfterCallWorkCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuACW();
                        }
                        else if (StateMenuCommand.NotReadyReasonCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenu();
                        }
                        else if (StateMenuCommand.LogOnCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuLogOn();
                        }
                        else if (StateMenuCommand.LogOffCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                        {
                            SetContextMenuLogOff();
                        }
                    }
                }
                SetInitialState();
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:Create() : " + generalException.ToString());
            }
            // SetHeaderStatus(agent);
        }

        #endregion Create

        public static string[] GetValueAsStringArray(string value, string[] defaultValue)
        {
            string text = value;
            if (text == null)
            {
                return defaultValue;
            }
            string[] array = text.Split(new char[]
	{
		',',
		';',
		'|',
		'/',
		'\\'
	}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }
            return array;
        }

        #region SetContextMenuLogOff

        private void SetContextMenuLogOff()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Command = MediaLogOffCommand;
                iWMenuItem.CommandParameter = "LogOff";
                iWMenuItem.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Menu.Channel.LogOff", "Header", null);
                iWMenuItem.IsEnabled = true;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuLogOff() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuLogOff

        #region SetContextMenuLogOn

        private void SetContextMenuLogOn()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Command = this.MediaLogOnCommand;
                iWMenuItem.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Menu.Channel.LogOn", "Header", null);
                iWMenuItem.IsEnabled = true;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuLogOn() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuLogOn

        #region SetContextMenuACW

        private void SetContextMenuACW()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Command = this.MediaStatusNotReadyAfterCallWorkCommand;
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.AfterCallWork", "Header", null);
                MagicImage magicImage = new MagicImage();
                magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                magicImage.Height = 16.0;
                magicImage.Width = 16.0;
                magicImage.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");   //Ready -#34B349   NoReady - #FFA339
                iWMenuItem.IsEnabled = true;
                if (!iWMenuItem.IsEnabled)
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork.Disabled", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork.Disabled", "ResourceKey");
                }
                else
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork", "ResourceKey");
                }
                iWMenuItem.Icon = magicImage;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuACW() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuACW

        #region SetContextMenuDND

        private void SetContextMenuDND()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.Dnd", "Header", null);
                iWMenuItem.Command = this.MediaStatusDndOnCommand;
                MagicImage magicImage = new MagicImage();
                magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                magicImage.Height = 16.0;
                magicImage.Width = 16.0;
                magicImage.Foreground = (Brush)new BrushConverter().ConvertFrom("#E81100");   //Ready -#34B349   NoReady - #FFA339     DND-#E81100
                iWMenuItem.IsEnabled = true;
                if (!iWMenuItem.IsEnabled)
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb.Disabled", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb.Disabled", "ResourceKey");
                }
                else
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb", "ResourceKey");
                }
                iWMenuItem.Icon = magicImage;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuDND() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuDND

        #region SetContextMenuReady

        private void SetContextMenuReady()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Command = MediaReadyCommand;
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.Ready", "Header", new object[] { "Break" });
                MagicImage magicImage = new MagicImage();
                magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                magicImage.Height = 16.0;
                magicImage.Width = 16.0;
                magicImage.Foreground = (Brush)new BrushConverter().ConvertFrom("#34B349");   //Ready -#34B349   NoReady - #FFA339
                // magicImage.IsMouseCapturedChanged += magicImage_IsMouseCapturedChanged;
                iWMenuItem.IsEnabled = true;
                if (!iWMenuItem.IsEnabled)
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready.Disabled", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready.Disabled", "ResourceKey");
                }
                else
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready", "ResourceKey");
                }
                iWMenuItem.Icon = magicImage;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuReady() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuReady

        #region SetContextMenuNoReady

        private void SetContextMenuNoReady()
        {
            try
            {
                IWMenuItem iWMenuItem = new IWMenuItem();
                iWMenuItem.Command = this.MediaStatusNotReadyCommand;
                iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.NotReady", "Header", new object[] { "Break" });
                MagicImage magicImage = new MagicImage();
                magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                magicImage.Height = 16.0;
                magicImage.Width = 16.0;
                magicImage.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");   //Ready -#34B349   NoReady - #FFA339
                // magicImage.IsMouseCapturedChanged += magicImage_IsMouseCapturedChanged;
                iWMenuItem.IsEnabled = true;
                if (!iWMenuItem.IsEnabled)
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "ResourceKey");
                }
                else
                {
                    magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "Source");
                    magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "ResourceKey");
                }
                iWMenuItem.Icon = magicImage;
                AgentStateContextMenu.Items.Add(iWMenuItem);
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenuNoReady() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenuNoReady

        #region SetContextMenu

        private void SetContextMenu()
        {
            try
            {
                foreach (CfgActionCode cfgActionCode in _voiceActionCodes)
                {
                    if (cfgActionCode != null)
                    {
                        IFormattedCfgActionCode formattedCfgActionCode = Container.Resolve<IFormattedCfgActionCode>();
                        formattedCfgActionCode.Name = cfgActionCode.Name;
                        string formattedString = formattedCfgActionCode.FormattedString;
                        formattedCfgActionCode.Release();
                        IWMenuItem iWMenuItem = new IWMenuItem();
                        iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.NotReadyActionCode", "Header", new object[]
							{
								formattedString
							});
                        if (cfgActionCode.UserProperties.ContainsKey("interaction-workspace"))
                        {
                            KeyValueCollection userProperties = cfgActionCode.UserProperties.GetAsKeyValueCollection("interaction-workspace");
                            if (userProperties.ContainsKey("enable.logout") && userProperties.GetAsString("enable.logout").ToLower() == "true")
                            {
                                iWMenuItem.Header = cfgActionCode.Name;
                                iWMenuItem.Command = this.MediaLogOffCommand;
                                iWMenuItem.CommandParameter = new object[] { cfgActionCode.Name, cfgActionCode.Code, userProperties };
                            }
                            else
                            {
                                iWMenuItem.Command = this.MediaStatusNotReadyActionCodeCommand;
                                iWMenuItem.CommandParameter = new object[] { cfgActionCode.Code, cfgActionCode.Name };
                            }
                        }
                        else
                        {
                            iWMenuItem.Command = this.MediaStatusNotReadyActionCodeCommand;
                            iWMenuItem.CommandParameter = new object[] { cfgActionCode.Code, cfgActionCode.Name };
                        }

                        MagicImage magicImage = new MagicImage();
                        magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                        magicImage.Height = 16.0;
                        magicImage.Width = 16.0;
                        magicImage.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");
                        iWMenuItem.IsEnabled = true;
                        if (!iWMenuItem.IsEnabled)
                        {
                            magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "Source");
                            magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "ResourceKey");
                        }
                        else
                        {
                            magicImage.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "Source");
                            magicImage.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "ResourceKey");
                        }
                        if (cfgActionCode.Subcodes != null)
                        {
                            List<CfgSubcode> subList = cfgActionCode.Subcodes.OrderBy(o => o.Name).ToList();
                            foreach (CfgSubcode cd in subList)
                            {
                                IWMenuItem temp = new IWMenuItem();
                                if (cfgActionCode.UserProperties.ContainsKey("interaction-workspace"))
                                {
                                    KeyValueCollection userProperties = cfgActionCode.UserProperties.GetAsKeyValueCollection("interaction-workspace");
                                    if (userProperties.ContainsKey("enable.logout") && userProperties.GetAsString("enable.logout").ToLower() == "true")
                                    {
                                        temp.Command = this.MediaLogOffCommand;
                                        temp.CommandParameter = new object[] { cd.Name, cd.Code, userProperties };
                                    }
                                    else
                                    {
                                        temp.Command = this.MediaStatusNotReadyActionCodeCommand;
                                        temp.CommandParameter = new object[] { cd.Code, cd.Name };
                                    }
                                }
                                else
                                {
                                    temp.Command = this.MediaStatusNotReadyActionCodeCommand;
                                    temp.CommandParameter = new object[] { cd.Code, cd.Name };
                                }
                                temp.Header = cd.Name;
                                MagicImage magicImage1 = new MagicImage();
                                magicImage1.SetBinding(MagicImage.MagicModeProperty, binding);
                                magicImage1.Height = 16.0;
                                magicImage1.Width = 16.0;
                                magicImage1.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");
                                if (!temp.IsEnabled)
                                {
                                    magicImage1.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "Source");
                                    magicImage1.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "ResourceKey");
                                }
                                else
                                {
                                    magicImage1.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "Source");
                                    magicImage1.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "ResourceKey");
                                }
                                temp.Icon = magicImage1;

                                iWMenuItem.Items.Add(temp);
                                CfgActionCode actionCode = new CfgActionCode(Container.Resolve<IConfigurationService>().ConfigService);
                                actionCode.Code = cd.Code;
                                actionCode.Name = cd.Name;
                                actionCode.UserProperties = cfgActionCode.UserProperties;
                                actionCode.Type = cfgActionCode.Type;
                                actionCode.Tenant = cfgActionCode.Tenant;
                                actionCode.State = cfgActionCode.State;
                                agent.NotReadyActionCodes.Add(actionCode);
                            }
                        }
                        agent.NotReadyActionCodes.Add(cfgActionCode);
                        iWMenuItem.Icon = magicImage;
                        AgentStateContextMenu.Items.Add(iWMenuItem);
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetContextMenu() : " + generalException.ToString());
            }
        }

        #endregion SetContextMenu

        public void Destroy()
        {
        }

        public void DispatchIfNecessary(Action action)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(action);
            else
                action.Invoke();
        }

        private void DisableMenuItem(string menuItemName)
        {
            DispatchIfNecessary(() =>
            {
                switch (menuItemName)
                {
                    case "logoff":
                        isINOFS = false;
                        foreach (IWMenuItem item in AgentStateContextMenu.Items)
                        {
                            if ((item.Header as TextBlock).Text.ToString().ToLower() != "log on")
                            {
                                SetDisable(item);
                            }
                            else
                            {
                                SetEnable(item);
                            }
                        }
                        break;

                    case "logon":
                        isINOFS = false;
                        foreach (IWMenuItem item in AgentStateContextMenu.Items)
                        {
                            if ((item.Header as TextBlock).Text.ToString().ToLower() == "log on")
                            {
                                SetDisable(item);
                            }
                            else
                            {
                                SetEnable(item);
                            }
                        }
                        break;

                    case "ofs":
                        isINOFS = true;
                        foreach (IWMenuItem item in AgentStateContextMenu.Items)
                        {
                            //if ((item.Header as TextBlock).Text.ToString().ToLower() != "log on")
                            //{
                            SetDisable(item);
                            //}
                            //else
                            //{
                            //    SetEnable(item);
                            //}
                        }
                        break;
                }
            });
        }

        private void SetDisable(IWMenuItem item)
        {
            try
            {
                item.IsEnabled = false;
                var image = new MagicImage()
                {
                    Height = 16,
                    Width = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                if (item.Command is NotReadyActionCodeCommand || item.Command is NotReadyCommand || (item.CommandParameter != null && item.CommandParameter.ToString() != "LogOff"))
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#BBBDBE");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady.Disabled", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is AfterCallWorkCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#BBBDBE");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork.Disabled", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork.Disabled", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is ReadyCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#BBBDBE");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready.Disabled", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready.Disabled", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is DndOnCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#BBBDBE");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb.Disabled", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb.Disabled", "ResourceKey");
                    item.Icon = image;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetDisable() : " + generalException.ToString());
            }
        }

        private void SetEnable(IWMenuItem item)
        {
            try
            {
                item.IsEnabled = true;
                var image = new MagicImage()
                {
                    Height = 16,
                    Width = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                if (item.Command is NotReadyActionCodeCommand || item.Command is NotReadyCommand || (item.CommandParameter != null && item.CommandParameter.ToString() != "LogOff"))
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.NotReady", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is AfterCallWorkCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.AfterCallWork", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is ReadyCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#34B349");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.Ready", "ResourceKey");
                    item.Icon = image;
                }
                else if (item.Command is DndOnCommand)
                {
                    image.Foreground = (Brush)new BrushConverter().ConvertFrom("#E81100");
                    image.Source = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb", "Source");
                    image.ResourceKey = LanguageDictionary.Current.Translate<string>("Common.Images.Main.Agent.State.DoNotDisturb", "ResourceKey");
                    item.Icon = image;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CustomGlobalMenuView:SetEnable() : " + generalException.ToString());
            }
        }
    }
}