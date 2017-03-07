using Genesyslab.Desktop.Infrastructure;
using Pointel.CIS.Desktop.Core.NotePadView;

namespace Pointel.CIS.Desktop.Core.Views.NotePadView
{
    internal interface IConsultNotePadView : IView
    {
        IConsultNotepadViewModel model
        {
            get;
            set;
        }
    }
}