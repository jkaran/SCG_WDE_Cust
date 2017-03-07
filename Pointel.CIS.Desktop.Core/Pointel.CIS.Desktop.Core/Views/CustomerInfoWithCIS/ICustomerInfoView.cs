using Genesyslab.Desktop.Infrastructure;
using Pointel.CIS.Desktop.Core.Views.CustomerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pointel.CIS.Desktop.Core.Views.CustomerInfoWithCIS
{
    public interface ICustomerInfoView:IView
    {
        ICustomerInfoViewModel Model
        {
            get;
            set;
        }
    }
}
