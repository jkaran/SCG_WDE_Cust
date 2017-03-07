using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Collections;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.DisplayFormat;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.SDK.Configurations;
using Genesyslab.Desktop.Modules.Windows.Views.Login.MediaInformation;
using Genesyslab.Desktop.Modules.Windows.Views.Toolbar.MyWorkplace.PlaceStatus;
using Genesyslab.Desktop.Modules.Windows.Views.Toolbar.MyWorkplace.PlaceStatus.Forward;
using Genesyslab.Desktop.WPFCommon;
using Genesyslab.Desktop.WPFCommon.Controls;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Microsoft.Practices.Composite.Wpf.Commands;
using Microsoft.Practices.Unity;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CustomGlobalStatusMenu.GlobalMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tomers.WPF.Localization;

namespace Pointel.CIS.Desktop.Core.Views.CustomReasonCode
{
    internal class CustomPlaceStatusViewModel : IPlaceStatusViewModel, System.ComponentModel.INotifyPropertyChanged
    {
        private readonly IUnityContainer container;

        private readonly ILogger log;

        private readonly IAgent agent;

        private readonly IViewManager viewManager;

        private System.Collections.ObjectModel.ObservableCollection<IMediaStatusItem> mediaStatusItemList;

        private DelegateCommand<object> mediaLogOnCommand;

        private DelegateCommand<object> defaultMediaLogOffCommand;

        public System.Windows.Input.ICommand mediaLogOffCommand { get { return new LogOffCommand(); } }

        private DelegateCommand<object> mediaStatusReadyCommand;

        private DelegateCommand<object> mediaStatusNotReadyCommand;

        private DelegateCommand<object> mediaStatusNotReadyActionCodeCommand;

        private DelegateCommand<object> mediaStatusDndOnCommand;

        private DelegateCommand<object> mediaStatusDndOffCommand;

        private DelegateCommand<object> mediaStatusNotReadyAfterCallWorkCommand;

        private DelegateCommand<object> mediaForwardCommand;

        private DelegateCommand<object> mediaCancelForwardCommand;

        private Settings _settings;

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

        private string header;

        //private IList<CfgActionCode> voiceActionCodes = null;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public IAgent Agent
        {
            get
            {
                return this.agent;
            }
        }

        public string Header
        {
            get
            {
                return this.header;
            }
            private set
            {
                this.header = value;
                this.OnPropertyChanged("Header");
            }
        }

        public ICommand MediaLogOnCommand
        {
            get
            {
                return this.mediaLogOnCommand;
            }
        }

        public ICommand MediaLogOffCommand
        {
            get
            {
                return this.defaultMediaLogOffCommand;
            }
        }

        public ICommand MediaStatusReadyCommand
        {
            get
            {
                return this.mediaStatusReadyCommand;
            }
        }

        public ICommand MediaStatusNotReadyCommand
        {
            get
            {
                return this.mediaStatusNotReadyCommand;
            }
        }

        public ICommand MediaStatusNotReadyActionCodeCommand
        {
            get
            {
                return this.mediaStatusNotReadyActionCodeCommand;
            }
        }

        public ICommand MediaStatusDndOnCommand
        {
            get
            {
                return this.mediaStatusDndOnCommand;
            }
        }

        public ICommand MediaStatusDndOffCommand
        {
            get
            {
                return this.mediaStatusDndOffCommand;
            }
        }

        public ICommand MediaStatusNotReadyAfterCallWorkCommand
        {
            get
            {
                return this.mediaStatusNotReadyAfterCallWorkCommand;
            }
        }

        public ICommand MediaForwardCommand
        {
            get
            {
                return this.mediaForwardCommand;
            }
        }

        public ICommand MediaCancelForwardCommand
        {
            get
            {
                return this.mediaCancelForwardCommand;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IMediaStatusItem> MediaStatusItemList
        {
            get
            {
                return this.mediaStatusItemList;
            }
        }

        public CustomPlaceStatusViewModel(IAgent agent, IUnityContainer container, ILogger log, IViewManager viewManager, IConfigurationService confService)
        {
            this.agent = agent;
            this.container = container;
            this.log = (this.log = log.CreateChildLogger("CustomPlaceStatusViewModel"));
            this.log.Debug("CustomPlaceStatusViewModel()");
            this.viewManager = viewManager;
            this.mediaLogOnCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaLogOnCommandHandler));
            this.defaultMediaLogOffCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaLogOffCommandHandler));
            this.mediaStatusReadyCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusReadyCommandHandler));
            this.mediaStatusNotReadyCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusNotReadyCommandHandler));
            this.mediaStatusNotReadyActionCodeCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusNotReadyActionCodeCommandHandler));
            this.mediaStatusDndOnCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusDndOnCommandHandler));
            this.mediaStatusDndOffCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusDndOffCommandHandler));
            this.mediaStatusNotReadyAfterCallWorkCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaStatusNotReadyAfterCallWorkCommandHandler));
            this.mediaForwardCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaForwardCommandCommandHandler));
            this.mediaCancelForwardCommand = new DelegateCommand<object>(new System.Action<object>(this.MediaCancelForwardCommandCommandHandler));
            this.mediaStatusItemList = new DispatchedObservableCollection<IMediaStatusItem>();
            agent.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.Agent_PropertyChanged);
            agent.Place.ListOfMedia.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.ListOfMedia_CollectionChanged);
            this.GetMediaStatusItem();
            LanguageContext.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.Instance_PropertyChanged);
            this.InitHeader();
            _settings = Settings.GetInstance();
        }

        ~CustomPlaceStatusViewModel()
        {
            try
            {
                this.agent.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.Agent_PropertyChanged);
                this.agent.Place.ListOfMedia.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.ListOfMedia_CollectionChanged);
                LanguageContext.Instance.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.Instance_PropertyChanged);
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel() Destructor : " + generalException.ToString());
            }
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName.ToString() == "Culture" || e.PropertyName.ToString() == "Dictionary")
                {
                    this.InitHeader();
                }
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel:Instance_PropertyChanged() : " + generalException.ToString());
            }
        }

        private void Agent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ("Place".Equals(e.PropertyName) && this.agent.Place != null)
            {
                this.agent.Place.ListOfMedia.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.ListOfMedia_CollectionChanged);
            }
        }

        private void ListOfMedia_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.mediaStatusItemList.Clear();
            this.GetMediaStatusItem();
        }

        private void InitHeader()
        {
            this.Header = LanguageDictionary.Current.Translate<string>("Windows.PlaceStatusView.Header", "Header");
        }

        protected void OnPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(info));
            }
        }

        public void SetContextMenu(ContextMenu contextMenu, object parameter)
        {
            //contextMenu.IsEnabled = false;
            IMediaStatusItem mediaStatusItem = parameter as IMediaStatusItem;
            if (mediaStatusItem == null || contextMenu == null || !mediaStatusItem.Media.MediaState.IsActive)
            {
                return;
            }
            Binding binding = new Binding("IsHighlighted");
            binding.Converter = new MagicModeConverter();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MenuItem), 1);
            contextMenu.Items.Clear();
            string[] array = GetValueAsStringArray(_settings.ChannelLevelActionCodes, CustomPlaceStatusViewModel.defaultCommands);
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string value = array2[i];
                if (StateMenuCommand.ReadyCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    IWMenuItem iWMenuItem = new IWMenuItem();
                    iWMenuItem.Command = this.mediaStatusReadyCommand;
                    iWMenuItem.CommandParameter = parameter;
                    iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.Ready", "Header", null);
                    MagicImage magicImage = new MagicImage();
                    magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                    magicImage.Height = 16.0;
                    magicImage.Width = 16.0;
                    iWMenuItem.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
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
                    contextMenu.Items.Add(iWMenuItem);
                }
                else if (StateMenuCommand.NotReadyCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    IWMenuItem iWMenuItem = new IWMenuItem();
                    iWMenuItem.Command = this.MediaStatusNotReadyCommand;
                    iWMenuItem.CommandParameter = parameter;
                    iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.NotReady", "Header", null);

                    MagicImage magicImage = new MagicImage();
                    magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                    magicImage.Height = 16.0;
                    magicImage.Width = 16.0;
                    iWMenuItem.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
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
                    contextMenu.Items.Add(iWMenuItem);
                }
                else if (StateMenuCommand.DndCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (mediaStatusItem.Media.IsItCapableDnd)
                    {
                        IWMenuItem iWMenuItem = new IWMenuItem();
                        iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.Dnd", "Header", null);
                        iWMenuItem.Command = this.MediaStatusDndOnCommand;
                        iWMenuItem.CommandParameter = parameter;
                        MagicImage magicImage = new MagicImage();
                        magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                        magicImage.Height = 16.0;
                        magicImage.Width = 16.0;
                        iWMenuItem.IsEnabled = !mediaStatusItem.Media.IsOutOfService;
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
                        contextMenu.Items.Add(iWMenuItem);
                    }
                }
                else if (StateMenuCommand.AfterCallWorkCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (mediaStatusItem.Media.IsItCapableAfterCallWork)
                    {
                        IWMenuItem iWMenuItem = new IWMenuItem();
                        iWMenuItem.Command = this.MediaStatusNotReadyAfterCallWorkCommand;
                        iWMenuItem.CommandParameter = parameter;
                        iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("State.Channel.AfterCallWork", "Header", null);
                        MagicImage magicImage = new MagicImage();
                        magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                        magicImage.Height = 16.0;
                        magicImage.Width = 16.0;
                        iWMenuItem.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
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
                        contextMenu.Items.Add(iWMenuItem);
                    }
                }
                else if (StateMenuCommand.NotReadyReasonCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    string[] array3 = _settings.NotReadyActionCodeFilters;
                    string[] array4 = new string[this.agent.NotReadyActionCodes.Count];
                    Hashtable hashtable = new Hashtable();
                    int num = 0;
                    foreach (CfgActionCode current in this.agent.NotReadyActionCodes)
                    {
                        hashtable.Add(current.Name, current);
                        array4[num] = current.Name;
                        num++;
                    }
                    if (array3 == null || (array3 != null && array3.Length == 0))
                    {
                        array3 = array4;
                    }
                    string[] array5 = array3;
                    for (int j = 0; j < array5.Length; j++)
                    {
                        string text = array5[j];
                        CfgActionCode cfgActionCode = (CfgActionCode)hashtable[text];
                        if (cfgActionCode != null && cfgActionCode.DBID > 0)
                        {
                            IFormattedCfgActionCode formattedCfgActionCode = this.container.Resolve<IFormattedCfgActionCode>();
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
                                    iWMenuItem.Command = this.mediaLogOffCommand;
                                    iWMenuItem.CommandParameter = new object[] { cfgActionCode.Name, cfgActionCode.Code, userProperties, mediaStatusItem.Media };
                                }
                                else
                                {
                                    iWMenuItem.Command = this.MediaStatusNotReadyActionCodeCommand;
                                    iWMenuItem.CommandParameter = new object[]
                            {
                                parameter,
                                cfgActionCode.Code,
                                cfgActionCode.Name
                            };
                                }
                            }
                            else
                            {
                                iWMenuItem.Command = this.MediaStatusNotReadyActionCodeCommand;
                                iWMenuItem.CommandParameter = new object[]
                            {
                                parameter,
                                cfgActionCode.Code,
                                 cfgActionCode.Name
                            };
                            }

                            MagicImage magicImage = new MagicImage();
                            magicImage.SetBinding(MagicImage.MagicModeProperty, binding);
                            magicImage.Height = 16.0;
                            magicImage.Width = 16.0;
                            iWMenuItem.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
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
                                List<CfgSubcode> subCodes = cfgActionCode.Subcodes.OrderBy(o => o.Name).ToList();
                                foreach (CfgSubcode cd in subCodes)
                                {
                                    // iWMenuItem.Items.Add(new IWMenuItem().Header = cd.Name);
                                    IWMenuItem temp = new IWMenuItem();
                                    if (cfgActionCode.UserProperties.ContainsKey("interaction-workspace"))
                                    {
                                        KeyValueCollection userProperties = cfgActionCode.UserProperties.GetAsKeyValueCollection("interaction-workspace");
                                        if (userProperties.ContainsKey("enable.logout") && userProperties.GetAsString("enable.logout").ToLower() == "true")
                                        {
                                            temp.Command = this.mediaLogOffCommand;
                                            temp.CommandParameter = new object[] { cd.Name, cd.Code, userProperties, mediaStatusItem.Media };
                                        }
                                        else
                                        {
                                            temp.Command = this.MediaStatusNotReadyActionCodeCommand;
                                            temp.CommandParameter = new object[]
                                            {
                                            parameter,
                                            cd.Code,
                                            cd.Name
                                            };
                                        }
                                    }
                                    else
                                    {
                                        temp.Command = this.MediaStatusNotReadyActionCodeCommand;
                                        temp.CommandParameter = new object[]
                                        {
                                        parameter,
                                        cd.Code,
                                        cd.Name
                                        };
                                    }
                                    temp.Header = cd.Name;
                                    MagicImage magicImage1 = new MagicImage();
                                    magicImage1.SetBinding(MagicImage.MagicModeProperty, binding);
                                    magicImage1.Height = 16.0;
                                    magicImage1.Width = 16.0;
                                    magicImage1.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFA339");
                                    temp.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
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
                                    CfgActionCode actionCode = new CfgActionCode(this.container.Resolve<IConfigurationService>().ConfigService);
                                    actionCode.Code = cd.Code;
                                    actionCode.Name = cd.Name;
                                }
                            }
                            iWMenuItem.Icon = magicImage;
                            contextMenu.Items.Add(iWMenuItem);
                        }
                        else
                        {
                            this.log.Warn("not found the ActionCode '" + text + "'");
                        }
                    }
                }
                else if (StateMenuCommand.LogOnCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    IWMenuItem iWMenuItem = new IWMenuItem();
                    iWMenuItem.Command = this.MediaLogOnCommand;
                    iWMenuItem.CommandParameter = parameter;
                    iWMenuItem.VerticalAlignment = VerticalAlignment.Center;
                    iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Menu.Channel.LogOn", "Header", null);
                    iWMenuItem.IsEnabled = mediaStatusItem.IsPossibleLogOn;
                    contextMenu.Items.Add(iWMenuItem);
                }
                else if (StateMenuCommand.LogOffCommand.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                {
                    IWMenuItem iWMenuItem = new IWMenuItem();
                    iWMenuItem.Command = this.defaultMediaLogOffCommand;
                    iWMenuItem.CommandParameter = parameter;
                    iWMenuItem.VerticalAlignment = VerticalAlignment.Center;
                    iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Menu.Channel.LogOff", "Header", null);
                    iWMenuItem.IsEnabled = (mediaStatusItem.IsPossibleLogOff && !mediaStatusItem.Media.IsOutOfService);
                    contextMenu.Items.Add(iWMenuItem);
                }
            }
            if (contextMenu.Items.IsEmpty)
            {
                contextMenu.Visibility = Visibility.Collapsed;
                return;
            }
            contextMenu.Visibility = Visibility.Visible;
        }

        public void SetForwardContextMenu(ContextMenu contextMenu, object parameter)
        {
            try
            {
                IMediaStatusItem mediaStatusItem = parameter as IMediaStatusItem;
                if (mediaStatusItem == null || contextMenu == null)
                {
                    return;
                }
                contextMenu.Items.Clear();
                IMedia media = mediaStatusItem.Media;
                if (media.Type == "MediaVoice")
                {
                    IWMenuItem iWMenuItem = new IWMenuItem();
                    iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Windows.PlaceStatusView.ContextMenu.Forward", "Header", null);
                    iWMenuItem.Command = this.MediaForwardCommand;
                    iWMenuItem.CommandParameter = parameter;
                    iWMenuItem.VerticalAlignment = VerticalAlignment.Center;
                    Binding binding = new Binding("Media.IsItPossibleSetForward");
                    binding.Source = mediaStatusItem;
                    iWMenuItem.SetBinding(UIElement.IsEnabledProperty, binding);
                    contextMenu.Items.Add(iWMenuItem);
                    if (!string.IsNullOrEmpty(mediaStatusItem.Forward))
                    {
                        iWMenuItem = new IWMenuItem();
                        iWMenuItem.Header = LanguageDictionary.Current.TranslateFormat("Windows.PlaceStatusView.ContextMenu.CancelForward", "Header", null);
                        iWMenuItem.Command = this.MediaCancelForwardCommand;
                        iWMenuItem.CommandParameter = parameter;
                        iWMenuItem.VerticalAlignment = VerticalAlignment.Center;
                        binding = new Binding("Media.IsItPossibleCancelForward");
                        binding.Source = mediaStatusItem;
                        iWMenuItem.SetBinding(UIElement.IsEnabledProperty, binding);
                        contextMenu.Items.Add(iWMenuItem);
                    }
                }
            }
            catch (Exception genernalException)
            {
                log.Error("CustomPlaceStatusViewModel:SetForwardContextMenu() : " + genernalException.ToString());
            }
        }

        protected void GetMediaStatusItem()
        {
            try
            {
                foreach (IMedia current in this.agent.Place.ListOfMedia)
                {
                    if (current.Type == "MediaVoice")
                    {
                        current.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.media_PropertyChanged);
                        current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.media_PropertyChanged);
                        this.log.DebugFormat("mediaStatusItemList::MediaVoice {0} adding ...", new object[]
					{
						current.LongName
					});
                        if (!current.MediaState.IsActive)
                        {
                            if (this.log.IsDebugEnabled)
                            {
                                this.log.DebugFormat("Media {0} in passive \"Disaster Recovery\" state and wont be displayed", new object[]
							{
								current.LongName
							});
                                continue;
                            }
                            continue;
                        }
                    }
                    IMediaStatusItem mediaStatusItem = this.container.Resolve<IMediaStatusItem>();
                    mediaStatusItem.Media = current;
                    IMediaExtension mediaExtension = this.container.Resolve<IMediaExtension>(current.Type);
                    if (mediaExtension != null)
                    {
                        mediaStatusItem.ImageSourceMediaKey = mediaExtension.GetMediaImageDefinitionKey(current);
                    }
                    this.mediaStatusItemList.Add(mediaStatusItem);
                    IMediaBundle mediaBundle = current as IMediaBundle;
                    if (mediaBundle != null)
                    {
                        foreach (IMedia current2 in mediaBundle.Medias)
                        {
                            IMediaStatusItem mediaStatusItem2 = this.container.Resolve<IMediaStatusItem>();
                            mediaStatusItem2.Media = current2;
                            mediaExtension = this.container.Resolve<IMediaExtension>(current2.Type);
                            if (mediaExtension != null && mediaExtension != null)
                            {
                                mediaStatusItem2.ImageSourceMediaKey = mediaExtension.GetMediaImageDefinitionKey(current);
                            }
                            mediaStatusItem.Child.Add(mediaStatusItem2);
                        }
                    }
                }
            }
            catch (Exception generaalException)
            {
                log.Error("CustomPlaceStatusViewModel:GetMediaStatusItem() : " + generaalException.ToString());
            }
        }

        private void media_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                string propertyName = e.PropertyName;
                if (propertyName != null && propertyName == "DR")
                {
                    this.UpdateMediaList();
                }
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel:media_PropertyChanged() :" + generalException.ToString());
            }
        }

        private void UpdateMediaList()
        {
            try
            {
                if (Application.Current.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(this.UpdateMediaList));
                    return;
                }
                this.mediaStatusItemList.Clear();
                this.GetMediaStatusItem();
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel:UpdateMediaList() : " + generalException.ToString());
            }
        }

        private void MediaLogOnCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaLogOnCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                new bool?(true);
                if (mediaStatusItem.Media.IsNeedToConfirmLoginInformation && mediaStatusItem.Media.IsPromptInformation)
                {
                    IMediaView mediaView = this.viewManager.InstantiateShell("RootMediaInformationRegion", "MediaInformationView", "mediaInformation", new System.Collections.Generic.Dictionary<string, object>
				{
					{
						"Medias",
						new System.Collections.Generic.List<IMedia>
						{
							mediaStatusItem.Media
						}
					}
				}).View as IMediaView;
                    Window window = mediaView as Window;
                    if (window != null)
                    {
                        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        window.Owner = Application.Current.MainWindow;
                        window.ShowDialog();
                    }
                    this.viewManager.RemoveShell("mediaInformation");
                }
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.LogOn();
                });
                thread.Start();
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaLogOnCommandHandler() : " + generalException.ToString());
            }
        }

        private void MediaLogOffCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaLogOffCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.LogOff();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaLogOffCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaStatusReadyCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusReadyCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.Ready();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusReadyCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaStatusNotReadyCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusNotReadyCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.NotReady();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusNotReadyCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaStatusNotReadyActionCodeCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusNotReadyActionCodeCommandHandler()");
                }
                object[] array = parameter as object[];
                if (array != null)
                {
                    IMediaStatusItem mediaStatusItem = (IMediaStatusItem)array[0];
                    IMedia media = mediaStatusItem.Media;
                    System.Threading.Thread thread = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            if (media.Name == "voice")
                            {
                                var requestAgentNotReady = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentNotReady.Create();
                                requestAgentNotReady.ThisDN = Settings.GetInstance().ThisDN;
                                MyActionCodeUtil actionCodeUtil = MyActionCodeManager.GetActionCodeUtil((string)array[1], (string)array[2], agent.NotReadyActionCodes);
                                media.DndOff();
                                requestAgentNotReady.Reasons = actionCodeUtil.Reasons;
                                requestAgentNotReady.Extensions = actionCodeUtil.Extensions;
                                requestAgentNotReady.AgentWorkMode = actionCodeUtil.WorkMode;
                                agent.FirstMediaVoice.Channel.Protocol.Send(requestAgentNotReady);
                            }
                            else
                            {
                                media.NotReady((string)array[1]);
                            }
                        }
                        catch (Exception generalException)
                        {
                            log.Error("Error occured while sending custom NR request at channel level " + generalException.Message);
                        }
                    });
                    thread.Start();
                }
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusNotReadyActionCodeCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaStatusDndOnCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusDndOnCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.DndOn();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusDndOnCommandHandler() :" + genralException.ToString());
            }
        }

        private void MediaStatusDndOffCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusDndOffCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.DndOff();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusDndOffCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaStatusNotReadyAfterCallWorkCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaStatusNotReadyAfterCallWorkCommandHandler()");
                }
                IMediaStatusItem mediaStatusItem = (IMediaStatusItem)parameter;
                IMedia media = mediaStatusItem.Media;
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    media.NotReadyAfterCallWork();
                });
                thread.Start();
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaStatusNotReadyAfterCallWorkCommandHandler() : " + genralException.ToString());
            }
        }

        private void MediaForwardCommandCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaForwardCommandCommandHandler()");
                }
                IForwardView forwardView = this.viewManager.InstantiateShell("RootForwardRegion", "ForwardView", "forward", null).View as IForwardView;
                if (forwardView != null && forwardView.Model != null)
                {
                    IMediaStatusItem mediaStatusItem = parameter as IMediaStatusItem;
                    if (mediaStatusItem != null)
                    {
                        forwardView.Model.MediaVoice = mediaStatusItem.Media;
                    }
                }
                Window window = forwardView as Window;
                if (window != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                }
                this.viewManager.RemoveShell("forward");
            }
            catch (Exception genralException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaForwardCommandCommandHandler() :" + genralException.ToString());
            }
        }

        private void MediaCancelForwardCommandCommandHandler(object parameter)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("MediaForwardCommandCommandHandler()");
                }
                ICancelForwardView cancelForwardView = this.viewManager.InstantiateShell("RootCancelForwardRegion", "CancelForwardView", "CancelForward", null).View as ICancelForwardView;
                if (cancelForwardView != null && cancelForwardView.Model != null)
                {
                    IMediaStatusItem mediaStatusItem = parameter as IMediaStatusItem;
                    if (mediaStatusItem != null)
                    {
                        cancelForwardView.Model.MediaVoice = mediaStatusItem.Media;
                    }
                }
                Window window = cancelForwardView as Window;
                if (window != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                }
                this.viewManager.RemoveShell("CancelForward");
            }
            catch (Exception generalException)
            {
                log.Error("CustomPlaceStatusViewModel:MediaCancelForwardCommandCommandHandler() :" + generalException.ToString());
            }
        }

        public void NotReady(string actionCode, IMediaStatusItem mediaStatusItem)
        {
            //if (!this.IsActionPossible())
            //{
            //    return;
            //}
            //try
            //{
            //    //if (this.log.IsInfoEnabled)
            //    //{
            //    //    this.log.Info("NotReady('" + actionCode + "')" + this.LogSet);
            //    //}
            //    //if (this.IsOutOfService)
            //    //{
            //    //    this.log.Info("The media is Out Of Service" + this.LogSet);
            //    //}
            //    //else if (this.IsLogOff)
            //    //{
            //    //    this.log.Info("The media is LogOff" + this.LogSet);
            //    //}
            //    //else

            //    {
            //        //mediaVoice=this.container.Resolve<IMediaVoice>();
            //        //IActionCodeManager actionCodeManager = this.container.Resolve<IActionCodeManager>();
            //        //IActionCodeUtil actionCodeUtil = CustomActionCodeManager.GetActionCodeUtil(actionCode, this.Agent.NotReadyActionCodes);
            //        ////this.DndOff();
            //        //System.Collections.Generic.Dictionary<string, object> dictionary = new System.Collections.Generic.Dictionary<string, object>();
            //        //dictionary["EnterpriseAgent"] =mediaVoice.EntrepriseAgent;
            //        //dictionary["Channel"] = mediaVoice.Channel;
            //        //dictionary["Queue"] = mediaVoice.Queue;
            //        //dictionary["WorkMode"] = Genesyslab.Enterprise.Model.Identity.WorkMode.Unknown;
            //        //if (actionCode != null)
            //        //{
            //        //    dictionary["WorkMode"] = (object)actionCodeUtil.WorkMode;
            //        //    dictionary["Reasons"] = (object)actionCodeUtil.Reasons;
            //        //    dictionary["Extensions"] = (object)actionCodeUtil.Extensions;
            //        //}
            //        //commandManager = this.container.Resolve<ICommandManager>();
            //        //this.commandManager.GetChainOfCommandByName("MediaVoiceNotReady").Execute(dictionary);
            //    }
            //    mediaVoice = this.container.Resolve<IMediaVoice>();
            //    IActionCodeUtil actionCodeUtil = CustomActionCodeManager.GetActionCodeUtil(actionCode, this.Agent.NotReadyActionCodes, this.container.Resolve<IActionCodeUtil>());
            //    // this.DndOff();
            //    Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //    dictionary["EnterpriseAgent"] = mediaVoice.EntrepriseAgent;
            //    dictionary["Channel"] = mediaVoice.Channel;
            //    dictionary["Queue"] = mediaVoice.Queue;
            //    dictionary["WorkMode"] = Genesyslab.Enterprise.Model.Identity.WorkMode.Unknown;
            //    if (actionCode != null)
            //    {
            //        dictionary["WorkMode"] = actionCodeUtil.WorkMode;
            //        dictionary["Reasons"] = actionCodeUtil.Reasons;
            //        dictionary["Extensions"] = actionCodeUtil.Extensions;
            //    }
            //    this.commandManager.GetChainOfCommandByName("MediaVoiceNotReady").Execute(dictionary);
            //}
            //catch (System.Exception exception)
            //{
            //    // this.log.Error("NotReady('" + actionCode + "', Exception " + this.LogSet, exception);
            //}
        }

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
    }
}