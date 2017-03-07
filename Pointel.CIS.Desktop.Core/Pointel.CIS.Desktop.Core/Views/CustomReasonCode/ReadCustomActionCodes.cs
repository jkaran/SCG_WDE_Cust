using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pointel.CIS.Desktop.Core.Views.CustomReasonCode
{
    public class ReadCustomActionCodes
    {
        public IList<CfgActionCode> ReadActionCodes(string channelName, IConfService confService)
        {
            try
            {
                if (!String.IsNullOrEmpty(channelName) && confService != null)
                {
                    CfgTenantQuery qTenant = new CfgTenantQuery();
                    qTenant.Name = "Resources";
                    CfgTenant tenant = confService.RetrieveObject<CfgTenant>(qTenant);
                    CfgActionCodeQuery qActionCode = new CfgActionCodeQuery();
                    qActionCode.TenantDbid = tenant.DBID;
                    return (IList<CfgActionCode>)confService.RetrieveMultipleObjects<CfgActionCode>(qActionCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}
