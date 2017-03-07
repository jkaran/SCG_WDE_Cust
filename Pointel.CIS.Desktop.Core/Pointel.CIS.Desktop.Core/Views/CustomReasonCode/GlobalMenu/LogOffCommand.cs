using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Platform.Commons.Logging;
using Pointel.CIS.Desktop.Core.Util;
using Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu;
using System;
using System.Windows.Input;

namespace Pointel.CustomGlobalStatusMenu.GlobalMenu
{
    internal class LogOffCommand : ICommand
    {
        private IAgent agent;
        private readonly ILogger log;

        public LogOffCommand()
        {
            agent = CustomGlobalMenuView.agent;
            log = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("LogOffCommand");
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
                if (parameter as string == "LogOff")
                    agent.LogOff();
                //for channel level
                else if ((parameter as Array).Length == 4)
                {
                    IMedia currentMedia = (parameter as Array).GetValue(3) as IMedia;
                    if (currentMedia != null)
                    {
                        if (currentMedia.Name == "voice")
                        {
                            //do logout with reason code
                            LogOffVoiceMedia(parameter);
                        }
                        else
                        {
                            currentMedia.LogOff();
                        }
                    }
                }
                // for global level
                else if ((parameter as Array).Length == 3)
                {
                    foreach (IMedia media in agent.Place.ListOfMedia)
                    {
                        if (media.Name == "voice")
                        {
                            //do logout with reason code
                            LogOffVoiceMedia(parameter);
                        }
                        else
                        {
                            media.LogOff();
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                log.Error("Error Occurred on sending request to Logoff, exception :" + generalException.ToString());
            }
        }

        private void LogOffVoiceMedia(object parameter)
        {
            try
            {
                var requestAgentLogout = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogout.Create(Settings.GetInstance().ThisDN);
                string code = (parameter as Array).GetValue(1).ToString();
                string name = (parameter as Array).GetValue(0).ToString();
                MyActionCodeUtil actionCodeUtil = MyActionCodeManager.GetActionCodeUtil(code, name, agent.NotReadyActionCodes);
                requestAgentLogout.Reasons = actionCodeUtil.Reasons;
                requestAgentLogout.Extensions = actionCodeUtil.Extensions;
                agent.FirstMediaVoice.Channel.Protocol.Send(requestAgentLogout);
            }
            catch (Exception generalException)
            {
                log.Error("Error Occurred on sending request to Logoff voice channel, exception :" + generalException.ToString());
            }
        }
    }
}