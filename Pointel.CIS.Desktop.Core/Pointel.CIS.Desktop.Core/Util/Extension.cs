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
using Genesyslab.Platform.Commons.Collections;
using System.Collections.Generic;

namespace Pointel.CIS.Desktop.Core.Util
{ 
    /// <summary>
    /// Comment: Agent Voice Events Subscribed here
    /// Last Modified: 13-Apr-2016
    /// Created by: Pointel Inc
    /// </summary>
    static class Extension
    {
        #region Methods
        //extension         
        /// <summary>
        /// Checks Contains keys the and value.
        /// </summary>
        /// <param name="Dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool ContainskeyAndValue(this IDictionary<string, object> Dictionary, string key)
        {
            if (Dictionary.ContainsKey(key) && Dictionary[key] != null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Checks Contains keys the and value.
        /// </summary>
        /// <param name="KVP">The KVP.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool ContainskeyAndValue(this KeyValueCollection KVP, string key)
        {
            if (KVP.ContainsKey(key) && !string.IsNullOrWhiteSpace(KVP.GetAsString(key)))
                return true;
            else
                return false;
        } 
        #endregion
    }
}
