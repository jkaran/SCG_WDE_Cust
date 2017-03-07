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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pointel.CIS.Desktop.Core.Util
{
    /// <summary>
    /// Comment: Encrypts the password to send with CIS Web Service Request
    /// Last Modified: 15-Apr-2016
    /// Created by: Pointel Inc
    /// </summary>
    public class Encryption
    {
        #region Consts
        /// <summary>
        /// Keep this inputkey very safe and prevent someone from decoding it some way!!
        /// </summary>
        internal const string Inputkey = "239087440-6346-4CF0-A2E8-671F9B6B9EA9";
        #endregion

        #region Rijndael Encryption

        /// <summary>
        /// Encrypt the given text and give the byte array back as a BASE64 string
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <returns>The encrypted text</returns>
        public static string EncryptRijndael(string text, string publicKey)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            RijndaelManaged aesAlgorithm = NewRijndaelManaged(publicKey);

            var encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        #endregion

        #region Rijndael Decryption
        /// <summary>
        /// Checks if a string is base64 encoded
        /// </summary>
        /// <param name="base64String">The base64 encoded string</param>
        /// <returns></returns>
        public static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        /// <summary>
        /// Decrypts the given text
        /// </summary>
        /// <param name="cipherText">The encrypted BASE64 text</param>
        /// <returns>De gedecrypte text</returns>
        public static string DecryptRijndael(string cipherText, string  publicKey)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");

            if (!IsBase64String(cipherText))
                throw new Exception("The cipherText input parameter is not base64 encoded");

            string text;

            RijndaelManaged aesAlgorithm = NewRijndaelManaged(publicKey);

            var decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);
            var cipher = Convert.FromBase64String(cipherText);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        text = srDecrypt.ReadToEnd();
                    }
                }
            }
            return text;
        }
        #endregion

        #region NewRijndaelManaged
        /// <summary>
        /// Create a new RijndaelManaged class and initialize it
        /// </summary>
        /// <param name="public Key">The pasword public key</param>
        /// <returns></returns>
        private static RijndaelManaged NewRijndaelManaged(string publicKey)
        {
            if (publicKey == null) throw new ArgumentNullException("Public Key");
            var publicBytes = Encoding.ASCII.GetBytes(publicKey);
            var key = new Rfc2898DeriveBytes(Inputkey, publicBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }
        #endregion
    }
}
