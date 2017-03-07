using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Modules.Windows.Views.Common.CaseDataView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfoWithCIS
{
    public interface ICustomCaseDataView:IView
    {
        ICaseDataViewModel Model
        {
            get;
            set;
        }
    }
}
