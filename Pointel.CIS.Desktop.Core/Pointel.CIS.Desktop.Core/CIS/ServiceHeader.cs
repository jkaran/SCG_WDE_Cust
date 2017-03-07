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
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;
/// <summary>
/// Comment: Add Security Header for CIS WebService Requests
/// Last Modified: 13-Apr-2016
/// Created by: Pointel Inc
/// </summary>
public class SecurityHeader : MessageHeader
{
    #region Fields
    private readonly UsernameToken _usernameToken; 
    #endregion

    #region Properties
    public SecurityHeader(string username, string password)
    {
        _usernameToken = new UsernameToken(username, password);
    }

    public override string Name
    {
        get { return "Security"; }
    }

    public override string Namespace
    {
        get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
    } 
    #endregion

    #region Member Function
    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(UsernameToken));
        serializer.Serialize(writer, _usernameToken);
    } 
    #endregion
}


[XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
public class UsernameToken
{
    #region Constructor
    public UsernameToken()
    {
    }

    public UsernameToken(string username, string password)
    {
        Username = username;
        Password = new Password() { Value = password };
    } 
    #endregion

    #region Properties
    [XmlElement]
    public string Username { get; set; }

    [XmlElement]
    public Password Password { get; set; } 
    #endregion
}

public class Password
{
    #region Constructor
    public Password()
    {
        Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
    } 
    #endregion

    #region Properties
    [XmlAttribute]
    public string Type { get; set; }

    [XmlText]
    public string Value { get; set; } 
    #endregion
}