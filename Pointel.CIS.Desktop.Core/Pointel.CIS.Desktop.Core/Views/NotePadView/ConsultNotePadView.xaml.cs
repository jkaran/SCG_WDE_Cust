using Genesyslab.Platform.Commons.Logging;
using Microsoft.Practices.Unity;
using Pointel.CIS.Desktop.Core.NotePadView;
using Pointel.CIS.Desktop.Core.Voice;
using System.Windows;
using System.Windows.Controls;

namespace Pointel.CIS.Desktop.Core.Views.NotePadView
{
    /// <summary>
    /// Interaction logic for ConsultNotePadView.xaml
    /// </summary>
    public partial class ConsultNotePadView : UserControl, IConsultNotePadView
    {
        private readonly IUnityContainer container;
        private readonly ILogger log;

        public IConsultNotepadViewModel model
        {
            get
            {
                return base.DataContext as IConsultNotepadViewModel;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public object Context
        {
            get;
            set;
        }

        public int SortIndex
        {
            get
            {
                return 0;
            }
        }

        public ConsultNotePadView(IUnityContainer container, IConsultNotepadViewModel viewModel, ILogger log)
        {
            this.container = container;
            this.model = viewModel;
            this.log = log.CreateChildLogger("ConsultNotePadView");
            this.log.Debug("ConsultNotePadView");
            this.InitializeComponent();
            //base.Width = double.NaN;
            //base.Height = double.NaN;
        }

        public void Create()
        {
            SubscribeAgentVoiceEvents.notepadVisiblityEvent += new ConsultNotepadVisiblityEvent(SubscribeAgentVoiceEvents_notepadEvent);
        }

        private void SubscribeAgentVoiceEvents_notepadEvent(bool enable, string notePadText)
        {
            if (enable)
            {
                CustomNotepad.Visibility = Visibility.Visible;
                if (!string.IsNullOrEmpty(notePadText))
                    this.txtEditor.SetText(notePadText);
                this.Height = double.NaN;
                this.Width = double.NaN;
            }
            else
            {
                CustomNotepad.Visibility = Visibility.Collapsed;
                this.Height = 0;
                this.Width = 0;
            }
        }

        public void Destroy()
        {
            this.log.Debug("ConsultNotePadView-destroy()");
            SubscribeAgentVoiceEvents.notepadVisiblityEvent -= new ConsultNotepadVisiblityEvent(SubscribeAgentVoiceEvents_notepadEvent);
        }
    }
}