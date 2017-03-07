using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Voice.Protocols.TServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pointel.CIS.Desktop.Core.Util
{
    public class MyActionCodeUtil
    {
        public KeyValueCollection Extensions { get; set; }
        public string Name { get; set; }
        public KeyValueCollection Reasons { get; set; }
        public AgentWorkMode WorkMode { get; set; }
    }
}
