using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Genesyslab.Platform.Voice.Protocols.TServer;
using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Special;
using Pointel.CIS.Desktop.Core.Util;
using System;

namespace Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu
{
    internal class SetAgentLoginStatus
    {
        private ILogger logger = null;
        private static SetAgentLoginStatus loginStatus = null;
        private Settings settings = null;

        private SetAgentLoginStatus()
        {
            this.logger = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("SetAgentLoginStatus");
            this.settings = Settings.GetInstance();
        }

        public static SetAgentLoginStatus GetInstance()
        {
            if (loginStatus == null)
                loginStatus = new SetAgentLoginStatus();
            return loginStatus;
        }

        public void SetInitialVoiceMediaStatus()
        {
            try
            {
                this.logger.Info("Setting Agent Login Status for Voice Media....");
                CommonProperties cProperties = CommonProperties.Create();
                cProperties.UserEvent = 85;
                cProperties.ThisDN = Settings.GetInstance().ThisDN;
                if (this.settings.LoginStatusRequestAttribute.Contains("extensions"))
                {
                    KeyValueCollection extension = new KeyValueCollection();
                    extension.Add("ReasonCode", Convert.ToInt32(this.settings.LoginStatusReasoncode));
                    extension.Add(this.settings.LoginStatusReasonName, Convert.ToInt32(this.settings.LoginStatusReasoncode));
                    cProperties.Extensions = extension;
                }
                if (this.settings.LoginStatusRequestAttribute.Contains("reasons"))
                {
                    KeyValueCollection reasons = new KeyValueCollection();
                    reasons.Add("ReasonCode", Convert.ToInt32(this.settings.LoginStatusReasoncode));
                    reasons.Add(this.settings.LoginStatusReasonName, Convert.ToInt32(this.settings.LoginStatusReasoncode));
                    cProperties.Reasons = reasons;
                }
                cProperties.AgentWorkMode = GetWorkMode(this.settings.LoginStatusWorkMode);
                RequestDistributeUserEvent userEvent = RequestDistributeUserEvent.Create(Settings.GetInstance().ThisDN, cProperties);
                if (ContainerAccessPoint.Container.Resolve<IAgent>().FirstMediaVoice != null)
                {
                    ContainerAccessPoint.Container.Resolve<IAgent>().FirstMediaVoice.Channel.Protocol.Send(userEvent);
                }
                else
                    this.logger.Info("Voice Media is unavailable...");
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while sending UserEvent to TServer,  Exception :" + generalException.ToString());
            }
        }

        private AgentWorkMode GetWorkMode(string workmode)
        {
            switch (workmode)
            {
                case "aux-work":
                    return AgentWorkMode.AuxWork;

                case "after-call-work":
                    return AgentWorkMode.AfterCallWork;

                case "walk-away":
                    return AgentWorkMode.WalkAway;

                case "auto-in":
                    return AgentWorkMode.AutoIn;

                case "manual-in":
                    return AgentWorkMode.ManualIn;

                default:
                    return AgentWorkMode.Unknown;
            }
        }
    }
}