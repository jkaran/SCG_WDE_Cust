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

using System;
using System.Text.RegularExpressions;

namespace Pointel.CIS.Desktop.Core.Util
{
    /// <summary>
    /// Comment: Util the string for CIS Update
    /// Last Modified: 20-Jun-2016
    /// Created by: Pointel Inc
    /// </summary>
    public class StringUtils
    {
        public static String formatAccountNumberSCG(String accountNr, int cashIndicator)
        {
            if (string.IsNullOrEmpty(accountNr))
            {
                return "";
            }

            if (accountNr.Length == 10)
            {
                int checkDigit = mod10CheckDigit(accountNr, cashIndicator);
                return accountNr.Substring(0, 3) + " " + accountNr.Substring(3, 3) + " " + accountNr.Substring(6) + " " + checkDigit;
            }
            else if (accountNr.Length < 10)
            {
                if (Settings.GetInstance().EnableAccountNumberFormat)
                {
                    while (accountNr.Length < 10)
                    {
                        accountNr = "0" + accountNr;
                    }
                    int checkDigit = mod10CheckDigit(accountNr, cashIndicator);
                    return accountNr.Substring(0, 3) + " " + accountNr.Substring(3, 3) + " " + accountNr.Substring(6) + " " + checkDigit;
                }
                else
                {
                    return accountNr;
                }
            }
            else
            {
                return accountNr;
            }
        }

        /**
         * Performs MOD10 checksum.
         *
         * @param acctId ID to be checked
         * @param cashIndicator cash indicator
         * @return the check digit after mod10 calculation
         */

        public static int mod10CheckDigit(String acctId, int cashIndicator)
        {
            int n = 0;
            int sum = 0;
            bool alternate = true;
            if (!string.IsNullOrWhiteSpace(acctId) && Regex.IsMatch(acctId, @"^\d+$"))
            {
                for (int i = acctId.Length - 1; i >= 0; i--)
                {
                    n = int.Parse(acctId.Substring(i, 1));
                    if (alternate)
                    {
                        n *= 2;
                        if (n > 9)
                        {
                            n = (n % 10) + 1;
                        }
                    }
                    sum += n;
                    alternate = !alternate;
                }
            }
            sum += cashIndicator;
            return 10 * (int)Math.Ceiling(sum / 10d) - sum;
        }

        public static String formatPhoneNumberWithAreaCode(String phoneNum)
        {
            string phoneFormat = "###-###-####";
            if (!String.IsNullOrEmpty(phoneNum))
            {
                Regex regexObj = new Regex(@"[^\d]");
                string phoneNumber = regexObj.Replace(phoneNum, "");
                if (phoneNumber.Length == 10)
                {
                    return Convert.ToInt64(phoneNum).ToString(phoneFormat);
                }
            }
            return phoneNum;
        }

        public static String aniToPhoneNumber(String ani)
        {
            string[] aniFormat = { "###-###-####", "#-###-###-####" };
            if (!String.IsNullOrEmpty(ani))
            {
                // Verify that the ani is all digits.
                Regex regexObj = new Regex(@"[^\d]");
                string phoneNum = regexObj.Replace(ani, "");
                if (ani.Length == 10)
                {
                    return Convert.ToInt64(phoneNum).ToString(aniFormat[0]);
                }
                else if (ani.Length >= 11)
                {
                    return Convert.ToInt64(phoneNum).ToString(aniFormat[1]);
                }
            }
            return ani;
        }
    }
}