using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu;
using System;
using System.Windows.Input;

namespace Pointel.CustomGlobalStatusMenu.GlobalMenu
{
    internal class NotReadyActionCodeCommand : ICommand
    {
        private IAgent agent;
        private readonly ILogger log;

        public NotReadyActionCodeCommand()
        {
            agent = CustomGlobalMenuView.agent;
            log = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("NotReadyActionCodeCommand");
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67

        public void Execute(object parameter)
        {
            try
            {
                var reasonCode = (parameter as Array).GetValue(0).ToString();
                var reasonName = (parameter as Array).GetValue(1).ToString();
                if (!string.IsNullOrWhiteSpace(reasonCode))
                {
                    foreach (IMedia media in agent.Place.ListOfMedia)
                    {
                        if (media.Name == "voice")
                        {
                            try
                            {
                                var requestAgentNotReady = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentNotReady.Create();
                                requestAgentNotReady.ThisDN = Settings.GetInstance().ThisDN;
                                MyActionCodeUtil actionCodeUtil = MyActionCodeManager.GetActionCodeUtil(reasonCode, reasonName, agent.NotReadyActionCodes);
                                media.DndOff();
                                requestAgentNotReady.Reasons = actionCodeUtil.Reasons;
                                requestAgentNotReady.Extensions = actionCodeUtil.Extensions;
                                requestAgentNotReady.AgentWorkMode = actionCodeUtil.WorkMode;
                                agent.FirstMediaVoice.Channel.Protocol.Send(requestAgentNotReady);
                            }
                            catch (Exception generalException)
                            {
                                log.Error("Error Occurred on sending request to voice not ready, exception :" + generalException.ToString());
                            }
                        }
                        else
                        {
                            media.NotReady(reasonCode);
                        }
                    }
                }
                else
                {
                    log.Info("Not Ready action code is null");
                }
            }
            catch (Exception generalException)
            {
                log.Error("NotReadyActionCodeCommand: Error Occurred on sending request to agent not ready with reason code, exception :" + generalException.ToString());
            }
        }
    }
}