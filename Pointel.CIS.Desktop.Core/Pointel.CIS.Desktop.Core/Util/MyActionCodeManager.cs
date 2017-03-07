using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Logging;
using Genesyslab.Platform.Voice.Protocols.TServer;
using System;
using System.Text.RegularExpressions;

namespace Pointel.CIS.Desktop.Core.Util
{
    internal class MyActionCodeManager
    {
        private static ILogger logger = null;

        static MyActionCodeManager()
        {
            logger = ContainerAccessPoint.Container.Resolve<ILogger>().CreateChildLogger("MyActionCodeManager");
        }

        // Genesyslab.Desktop.Modules.Core.Model.Agents.ActionCode.ActionCodeManager
        public static MyActionCodeUtil GetActionCodeUtil(string codeIn, string name, System.Collections.Generic.IList<CfgActionCode> list)
        {
            MyActionCodeUtil actionCodeUtil = new MyActionCodeUtil();
            KeyValueCollection reasons = new KeyValueCollection();
            KeyValueCollection extensions = new KeyValueCollection();
            try
            {
                CfgActionCode actionCode = null;
                foreach (CfgActionCode current in list)
                {
                    if (current.Code.Equals(codeIn) && current.Name.Equals(name))
                    {
                        logger.Info("NR Match Case " + current.Code + ":" + current.Name);
                        actionCode = current;
                        break;
                    }
                }

                actionCodeUtil.Name = "";
                if (actionCode != null)
                {
                    actionCodeUtil.Name = actionCode.Name;
                    string code = actionCode.Code;
                    string workMode = null;
                    string extensionBool = null;
                    string keyName = null;
                    string keyValue = null;
                    bool canAddReason = false;
                    bool canAddExtensions = false;
                    if (actionCode.UserProperties != null)
                    {
                        KeyValueCollection asKeyValueCollection = actionCode.UserProperties.GetAsKeyValueCollection("interaction-workspace");
                        if (asKeyValueCollection != null)
                        {
                            workMode = (string)asKeyValueCollection.Get("workmode");
                            extensionBool = (string)asKeyValueCollection.Get("extensions");
                            keyName = (string)asKeyValueCollection.Get("reason-extension-key");
                            keyValue = (string)asKeyValueCollection.Get("reason-extension-value");
                            string text3 = (string)asKeyValueCollection.Get("reason-extension-request-attribute");
                            if (!string.IsNullOrEmpty(text3))
                            {
                                if (text3.ToLower().Contains("reasons"))
                                {
                                    canAddReason = true;
                                }
                                if (text3.ToLower().Contains("extensions"))
                                {
                                    canAddExtensions = true;
                                }
                            }
                        }
                        else
                        {
                            canAddReason = true;
                            canAddExtensions = true;
                        }
                    }
                    else
                    {
                        canAddReason = true;
                        canAddExtensions = true;
                    }
                    object value3 = code;
                    if (string.IsNullOrEmpty(workMode) && string.IsNullOrEmpty(extensionBool))
                    {
                        string key;
                        if (string.IsNullOrEmpty(keyName))
                        {
                            key = actionCode.Name;
                        }
                        else
                        {
                            key = keyName;
                        }
                        if (keyValue != null && "name".Equals(keyValue))
                        {
                            value3 = actionCode.Name;
                        }
                        actionCodeUtil.WorkMode = AgentWorkMode.Unknown;
                        if (canAddReason)
                        {
                            reasons.Add(key, value3);
                            actionCodeUtil.Reasons = reasons;
                        }
                        if (canAddExtensions)
                        {
                            if (Regex.IsMatch(actionCode.Code, @"^\d+$"))
                                extensions.Add(key, Int32.Parse(actionCode.Code));
                            else
                                extensions.Add(key, actionCode.Code);

                            actionCodeUtil.Extensions = extensions;
                        }
                        return actionCodeUtil;
                    }
                    if ("aux-work".Equals(workMode, System.StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            string key;
                            if (string.IsNullOrEmpty(keyName))
                            {
                                key = "ReasonCode";
                            }
                            else
                            {
                                key = keyName;
                            }
                            if (keyValue != null && "name".Equals(keyValue))
                            {
                                value3 = actionCode.Name;
                            }
                            actionCodeUtil.WorkMode = AgentWorkMode.AuxWork;
                            if (canAddReason)
                            {
                                reasons.Add(key, actionCode.Code);
                                actionCodeUtil.Reasons = reasons;
                            }
                            if (canAddExtensions)
                            {
                                if (Regex.IsMatch(actionCode.Code, @"^\d+$"))
                                    extensions.Add(key, Int32.Parse(actionCode.Code));
                                else
                                    extensions.Add(key, actionCode.Code);
                                actionCodeUtil.Extensions = extensions;
                            }
                            var result = actionCodeUtil;
                            return result;
                        }
                        catch (System.Exception exception)
                        {
                            logger.Info("aux-work, apply a normal not ready  Exception", exception);
                            actionCodeUtil.WorkMode = AgentWorkMode.Unknown;
                            actionCodeUtil.Reasons = null;
                            actionCodeUtil.Extensions = null;
                            var result = actionCodeUtil;
                            return result;
                        }
                    }
                    if ("after-call-work".Equals(workMode, System.StringComparison.OrdinalIgnoreCase))
                    {
                        value3 = code;
                        string key;
                        if (string.IsNullOrEmpty(keyName))
                        {
                            key = "ReasonCode";
                        }
                        else
                        {
                            key = keyName;
                        }
                        if (keyValue != null && "name".Equals(keyValue))
                        {
                            value3 = actionCode.Name;
                        }

                        actionCodeUtil.WorkMode = AgentWorkMode.AfterCallWork;
                        if (canAddReason)
                        {
                            reasons.Add(key, value3);
                            actionCodeUtil.Reasons = reasons;
                        }
                        if (canAddExtensions)
                        {
                            if (Regex.IsMatch(actionCode.Code, @"^\d+$"))
                                extensions.Add(key, Int32.Parse(actionCode.Code));
                            else
                                extensions.Add(key, actionCode.Code);
                            actionCodeUtil.Extensions = extensions;
                        }
                        return actionCodeUtil;
                    }
                    if ("walk-away".Equals(workMode, System.StringComparison.OrdinalIgnoreCase))
                    {
                        value3 = code;
                        string key;
                        if (string.IsNullOrEmpty(keyName))
                        {
                            key = "ReasonCode";
                        }
                        else
                        {
                            key = keyName;
                        }
                        if (keyValue != null && "name".Equals(keyValue))
                        {
                            value3 = actionCode.Name;
                        }

                        actionCodeUtil.WorkMode = AgentWorkMode.WalkAway;
                        if (canAddReason)
                        {
                            reasons.Add(key, value3);
                            actionCodeUtil.Reasons = reasons;
                        }
                        if (canAddExtensions)
                        {
                            if (Regex.IsMatch(actionCode.Code, @"^\d+$"))
                                extensions.Add(key, Int32.Parse(actionCode.Code));
                            else
                                extensions.Add(key, actionCode.Code);
                            actionCodeUtil.Extensions = extensions;
                        }
                        return actionCodeUtil;
                    }
                    if ("true".Equals(extensionBool, System.StringComparison.OrdinalIgnoreCase))
                    {
                        KeyValueCollection keyValueCollection5 = new KeyValueCollection();
                        value3 = code;
                        string key;
                        if (string.IsNullOrEmpty(keyName))
                        {
                            key = "ReasonCode";
                        }
                        else
                        {
                            key = keyName;
                        }
                        if (keyValue != null && "name".Equals(keyValue))
                        {
                            value3 = actionCode.Name;
                        }
                        keyValueCollection5.Add(key, value3);
                        actionCodeUtil.WorkMode = AgentWorkMode.Unknown;
                        if (canAddReason)
                        {
                            actionCodeUtil.Reasons = keyValueCollection5;
                        }
                        if (canAddExtensions)
                        {
                            actionCodeUtil.Extensions = keyValueCollection5;
                        }
                        return actionCodeUtil;
                    }
                }
                actionCodeUtil.WorkMode = AgentWorkMode.Unknown;
                actionCodeUtil.Reasons = null;
                actionCodeUtil.Extensions = null;
                return actionCodeUtil;
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred, Exception :" + generalException.ToString());
            }
            return actionCodeUtil;
        }
    }
}