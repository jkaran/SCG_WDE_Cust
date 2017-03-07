﻿using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Pointel.CIS.Desktop.Core.Views.CustomReasonCode.GlobalMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Pointel.CustomGlobalStatusMenu.GlobalMenu
{
    class AfterCallWorkCommand  :ICommand
    {   
        IAgent agent;
       
        public AfterCallWorkCommand()
        {
          agent = CustomGlobalMenuView.agent;  
        }

        public bool CanExecute(object parameter)
        {
          return agent.IsItCapableAfterCallWork;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
           agent.NotReadyAfterCallWork();   
        }
    }
}