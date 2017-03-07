/*
 Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE 
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING 
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY, 
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL 
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A 
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS 
  *DERIVATIVES.
 */

using Genesyslab.Platform.Commons.Logging;

namespace Pointel.CIS.Desktop.Core.Util
{
     /// <summary>
    /// Comment: contains log objects for WDE
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class Log
    {
        #region Fields
        private static Log thisLogger = null;
        private ILogger Genlogger = null;
        #endregion

        #region Constructor
        private Log()
        {
        } 
        #endregion

        #region Log Level

        public void Error(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Error(message);
            }
        }

        public void Info(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Info(message);
            }
        }

        public void Warn(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Warn(message);
            }
        }

        public void Debug(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Debug(message);
            }

        }

        #endregion

        #region GetInstance
        public static Log GenInstance()
        {
            if (thisLogger == null)
            {
                thisLogger = new Log();
            }
            return thisLogger;
        }
        #endregion

        #region CreateLogger
        public Log CreateLogger(ILogger genLoggger)
        {
            this.Genlogger = genLoggger;
            return thisLogger;
        } 
        #endregion
    }
}
