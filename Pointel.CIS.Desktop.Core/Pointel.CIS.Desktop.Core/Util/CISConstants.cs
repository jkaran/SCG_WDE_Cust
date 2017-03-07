using System;

namespace Pointel.CIS.Desktop.Core.Util
{
    public class CISConstants
    {
        // Business Units.
        public static readonly String BUS_UNIT_SDGE = "SDGE";

        public static readonly String BUS_UNIT_SCG = "SCG";

        //private static readonly Properties ENV_VARS;

        public static readonly int DEFAULT_MAX_DISPOSITION_SELECTION = 3;

        //=========================== environment specific keys
        //Values need to exist in environment.properties file
        public static readonly String ENV_KEY_GAD_OPTIONS = "GAD_OPTIONS";

        public static readonly String ENV_KEY_GAD_OPTIONS_BUSINESS_UNIT = "GAD_OPTIONS_BUSINESS_UNIT";
        public static readonly String ENV_KEY_GAD_OPTIONS_CACS = "GAD_OPTIONS_CACS";
        public static readonly String ENV_KEY_GAD_OPTIONS_MAX_DCODE_SEL = "GAD_OPTIONS_MAX_DCODE_SEL";
        public static readonly String ENV_KEY_GAD_OPTIONS_CACS_URL = "SETTINGS_CACS_URL";
        public static readonly String ENV_KEY_GAD_OPTIONS_GROUP_NAME_SCG = "GAD_OPTIONS_BUSINESS_UNIT_VALUE_SCG";
        public static readonly String ENV_KEY_GAD_OPTIONS_GROUP_NAME_SDGE = "GAD_OPTIONS_BUSINESS_UNIT_VALUE_SDGE";
        public static readonly String ENV_KEY_GAD_OPTIONS_OUTBOUND = "GAD_OPTIONS_OUTBOUND";
        public static readonly String ENV_KEY_GROUP_DISPOSITION_CODE_PREFIX = "GROUP_DISPOSITION_CODE_PREFIX";
        public static readonly String ENV_KEY_SETTINGS_SUFFIX = "SETTINGS_SUFFIX";
        public static readonly String ENV_KEY_SETTINGS_GET_CUSTOMER_INFO_WS_URL = "SETTINGS_GET_CUSTOMER_INFO_WS_URL";
        public static readonly String ENV_KEY_SETTINGS_GET_CUSTOMER_INFO_WS_TIMEOUT = "SETTINGS_GET_CUSTOMER_INFO_WS_TIMEOUT";
        public static readonly String ENV_KEY_SETTINGS_GET_CUSTOMER_INFO_WS_RESULT_CODE_PREFIX = "SETTINGS_GET_CUSTOMER_INFO_WS_RESULT_CODE_PREFIX";
        public static readonly String ENV_KEY_SETTINGS_WS_GATEWAY_ACCOUNT_ID = "SETTINGS_WS_GATEWAY_ACCOUNT_ID";
        public static readonly String ENV_KEY_SETTINGS_WS_GATEWAY_ACCOUNT_PASSWORD = "SETTINGS_WS_GATEWAY_ACCOUNT_PASSWORD";
        public static readonly String ENV_KEY_SETTINGS_DISPOSITION_TAB_COLUMNS_COUNT = "SETTINGS_DISPOSITION_TAB_COLUMNS_COUNT";
        public static readonly String ENV_KEY_NOT_READY_CODE_SET_SUFFIX = "NOT_READY_CODE_SET_SUFFIX";
        public static readonly String ENV_KEY_MODULE_DISPCODE_MAPPINGS_SUFFIX = "MODULE_DISPCODE_MAPPINGS_SUFFIX";
        public static readonly String ENV_KEY_CALLER_GOAL_DISPCODE_MAPPINGS_SUFFIX = "CALLER_GOAL_DISPCODE_MAPPINGS_SUFFIX";
        public static readonly String ENV_KEY_IVR_COMPLETED_TXN_TRANSLATIONS_SUFFIX = "IVR_COMPLETED_TXN_TRANSLATIONS_SUFFIX";
        public static readonly String ENV_KEY_SETTINGS_ACCOUNT_CASH_INDICATOR = "ACCOUNT_CASH_INDICATOR";

        //=========================== request parameters
        public static readonly String REQUEST_PARAM_INTERACTION_ID = "interaction_id";

        public static readonly String REQUEST_PARAM_DISPOSITION_CODE_KEY = "dcode_kvp_key";
        public static readonly String REQUEST_PARAM_DISPOSITION_CODE_VALUE = "dcode_kvp_value";

        //=========================== session keys
        public static readonly String SESSION_KEY_AGENT = "SESSION_KEY_AGENT";

        public static readonly String SESSION_KEY_AGENTSESSIONLISTENER = "SESSION_KEY_AGENTSESSIONLISTENER";
        public static readonly String SESSION_KEY_AGENT_SESSION_HISTORY = "SESSION_KEY_AGENT_SESSION_HISTORY";
        public static readonly String SESSION_KEY_AGENT_STATE_TREE_MAP = "SESSION_KEY_AGENT_STATE_TREE_MAP";
        public static readonly String SESSION_KEY_AGENT_STATES_LOADED = "SESSION_KEY_AGENT_STATES_LOADED";

        //=========================== inbound SDG&E KVP keys
        public static readonly String INBOUND_KVP_KEY_ACTUAL_CALL_TYPE = "ACTUAL_CALL_TYPE";

        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_1 = "IVR_DISPOSITION_1";
        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_2 = "IVR_DISPOSITION_2";
        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_3 = "IVR_DISPOSITION_3";
        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_1 = "IVR_DISPOSITION_DESC_1";
        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_2 = "IVR_DISPOSITION_DESC_2";
        public static readonly String INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_3 = "IVR_DISPOSITION_DESC_3";
        public static readonly String INBOUND_KVP_KEY_IVR_LAST_STATE = "IVR_LAST_STATE";
        public static readonly String INBOUND_KVP_KEY_IVR_ACCOUNT_NUM_R = "IVR_ACCOUNT_NUM_R";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_CITY_CODE = "IVR_AD_CITY_CODE";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_CRDNL_DIR = "IVR_AD_CRDNL_DIR";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_DIR_SX = "IVR_AD_DIR_SX";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_HSE_NO = "IVR_AD_HSE_NO";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_HSE_NO_MOD = "IVR_AD_HSE_NO_MOD";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_MODIFIER = "IVR_AD_MODIFIER";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_STR_NM = "IVR_AD_STR_NM";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_STR_SX = "IVR_AD_STR_SX";
        public static readonly String INBOUND_KVP_KEY_IVR_AD_SVC_ZIP = "IVR_AD_SVC_ZIP";
        public static readonly String INBOUND_KVP_KEY_IVR_CD_NPSO = "IVR_CD_NPSO";
        public static readonly String INBOUND_KVP_KEY_IVR_CD_RATE = "IVR_CD_RATE";
        public static readonly String INBOUND_KVP_KEY_IVR_FL_ASSIGN_ACCT = "IVR_FL_ASSIGN_ACCT";
        public static readonly String INBOUND_KVP_KEY_IVR_NM_COAP_1 = "IVR_NM_COAP_1";
        public static readonly String INBOUND_KVP_KEY_IVR_NM_COAP_2 = "IVR_NM_COAP_2";
        public static readonly String INBOUND_KVP_KEY_IVR_NM_CUST = "IVR_NM_CUST";
        public static readonly String INBOUND_KVP_KEY_IVR_TX_EMAIL_ADDR = "IVR_TX_EMAIL_ADDR";
        public static readonly String INBOUND_KVP_KEY_IVR_TX_HOME_AREACD = "IVR_TX_HOME_AREACD";
        public static readonly String INBOUND_KVP_KEY_IVR_TX_HOME_PHN_NO = "IVR_TX_HOME_PHN_NO";
        public static readonly String INBOUND_KVP_KEY_IVR_TX_PASSWORD = "IVR_TX_PASSWORD";
        public static readonly String INBOUND_KVP_KEY_IVR_CD_ACCT_CL = "IVR_CD_ACCT_CL";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_1 = "CSR_DISPOSITION_1";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_2 = "CSR_DISPOSITION_2";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_3 = "CSR_DISPOSITION_3";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_1 = "CSR_DISPOSITION_DESC_1";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_2 = "CSR_DISPOSITION_DESC_2";
        public static readonly String INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_3 = "CSR_DISPOSITION_DESC_3";
        public static readonly String INBOUND_KVP_KEY_CSR_OUTBOUND_DISPOSITION = "OUTBOUND_DISPOSITION";
        public static readonly String INBOUND_KVP_KEY_CSR_OUTBOUND_DISPOSITION_DESC = "OUTBOUND_DISPOSITION_DESC";
        public static readonly String INBOUND_KVP_KEY_CSR_IVR_PRESELECT_DISP = "CSR_IVR_PRESELECT_DISP";
        public static readonly String INBOUND_KVP_KEY_MODULE = "MODULE";
        public static readonly String INBOUND_KVP_KEY_CACS_ACCOUNTNUM = "CACS_ACCOUNTNUM";
        public static readonly String INBOUND_KVP_KEY_CACS_LOCATION = "CACS_LOCATION";
        public static readonly String INBOUND_KVP_KEY_CARE_PHRASE_OFFERED = "CARE_PHRASE_OFFERED";
        public static readonly String INBOUND_KVP_KEY_IVR_ADDRESS = "IVR_ADDRESS";
        public static readonly String INBOUND_KVP_KEY_IVR_PHONE_IND = "IVR_PHONE_IND";

        // Release 4: 03/07/2012 by Pritam
        public static readonly String INBOUND_KVP_KEY_CREDIT_PHRASE_OFFERED = "CREDIT_PHRASE_OFFERED";

        public static readonly String INBOUND_KVP_KEY_SAFE_ACCESS_PHRASE_OFFERED = "SAFE_ACCESS_PHRASE_OFFERED";

        public static readonly String[] INBOUND_KVP_LIST = new String[]{
		INBOUND_KVP_KEY_ACTUAL_CALL_TYPE, INBOUND_KVP_KEY_CACS_ACCOUNTNUM, INBOUND_KVP_KEY_CACS_LOCATION,
		INBOUND_KVP_KEY_IVR_DISPOSITION_1, INBOUND_KVP_KEY_IVR_DISPOSITION_2, INBOUND_KVP_KEY_IVR_DISPOSITION_3,
		INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_1, INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_2, INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_3,
		INBOUND_KVP_KEY_IVR_LAST_STATE, INBOUND_KVP_KEY_IVR_ACCOUNT_NUM_R, INBOUND_KVP_KEY_IVR_AD_CITY_CODE,
		INBOUND_KVP_KEY_IVR_AD_CRDNL_DIR, INBOUND_KVP_KEY_IVR_AD_DIR_SX, INBOUND_KVP_KEY_IVR_AD_HSE_NO,
		INBOUND_KVP_KEY_IVR_AD_HSE_NO_MOD, INBOUND_KVP_KEY_IVR_AD_MODIFIER, INBOUND_KVP_KEY_IVR_AD_STR_NM,
		INBOUND_KVP_KEY_IVR_AD_STR_SX, INBOUND_KVP_KEY_IVR_AD_SVC_ZIP, INBOUND_KVP_KEY_IVR_CD_NPSO,
		INBOUND_KVP_KEY_IVR_CD_RATE, INBOUND_KVP_KEY_IVR_FL_ASSIGN_ACCT, INBOUND_KVP_KEY_IVR_NM_COAP_1,
		INBOUND_KVP_KEY_IVR_NM_COAP_2, INBOUND_KVP_KEY_IVR_NM_CUST, INBOUND_KVP_KEY_IVR_TX_EMAIL_ADDR,
		INBOUND_KVP_KEY_IVR_TX_HOME_AREACD, INBOUND_KVP_KEY_IVR_TX_HOME_PHN_NO, INBOUND_KVP_KEY_IVR_TX_PASSWORD,
		INBOUND_KVP_KEY_IVR_CD_ACCT_CL, INBOUND_KVP_KEY_CSR_DISPOSITION_1, INBOUND_KVP_KEY_CSR_DISPOSITION_2,
		INBOUND_KVP_KEY_CSR_DISPOSITION_3, INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_1, INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_2,
		INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_3, INBOUND_KVP_KEY_CSR_IVR_PRESELECT_DISP, INBOUND_KVP_KEY_MODULE,
        INBOUND_KVP_KEY_CARE_PHRASE_OFFERED,
        INBOUND_KVP_KEY_IVR_ADDRESS,
        INBOUND_KVP_KEY_IVR_PHONE_IND,
        INBOUND_KVP_KEY_CREDIT_PHRASE_OFFERED,
        INBOUND_KVP_KEY_SAFE_ACCESS_PHRASE_OFFERED
	};

        //=========================== inbound SCG KVP keys
        public static readonly String INBOUND_SCG_KVP_KEY_CALLER_GOAL = "CALLER_GOAL";  // Note:  This key is set by the IVR but is read-only for the desktop

        public static readonly String INBOUND_SCG_KVP_KEY_CIS_FOLDER = "CIS_FOLDER";

        public static readonly String INBOUND_SCG_KVP_KEY_RESP_ACCOUNT_ID = "RESP_ACCOUNT_ID";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_HOME_PHONE = "SCREEN_POP_HOME_PHONE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_HOME_PHONE_EXT = "SCREEN_POP_HOME_PHONE_EXT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CUSTOMER_NAME = "SCREEN_POP_CUSTOMER_NAME";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_SPOUSE_NAME = "SCREEN_POP_SPOUSE_NM";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_ADDRESS = "SCREEN_POP_CUST_ADDRESS";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_ID = "SCREEN_POP_CUST_ID";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_TY_CD = "SCREEN_POP_CUST_TY_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CF_ID = "SCREEN_POP_CF_ID";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_ZIP_CODE = "SCREEN_POP_ZIP_CODE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ADDRESS = "SCREEN_POP_MAIL_ADDRESS";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_CITY = "SCREEN_POP_MAIL_CITY";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_STATE = "SCREEN_POP_MAIL_STATE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ZIP = "SCREEN_POP_MAIL_ZIP";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MAIL_ZIP4 = "SCREEN_POP_MAIL_ZIP4";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CHECK_DIGIT = "SCREEN_POP_CHECK_DIGIT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_ON_SIMPLE_PAY = "SCREEN_POP_ON_SIMPLE_PAY";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_ON_DEMAND_PAY = "SCREEN_POP_ON_DEMAND_PAY";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CURR_BAL_DUE = "SCREEN_POP_CURR_BAL_DUE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_ELIGIBLE_SW = "SCREEN_POP_ELIGIBLE_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BASE = "SCREEN_POP_BASE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_TERM_DT = "SCREEN_POP_BA_TERM_DT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_PAID_DT = "SCREEN_POP_BA_PAID_DT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_OPEN_DT = "SCREEN_POP_BA_OPEN_DT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_IC_ESTB_DT = "SCREEN_POP_IC_ESTB_DT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_OVERDUE_COUNT = "SCREEN_POP_OVERDUE_COUNT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_48HR_COUNT = "SCREEN_POP_48HR_COUNT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_OVER_ONE_YR = "SCREEN_POP_OVER_ONE_YR";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_OFF_GREATER6 = "SCREEN_POP_OFF_GREATER6";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_READ_OK_SW = "SCREEN_POP_READ_OK_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_NBR_OF_DIALS = "SCREEN_POP_NBR_OF_DIALS";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_OFFER_AMORT_SW = "SCREEN_POP_OFFER_AMORT_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_YTD_VARIANCE = "SCREEN_POP_YTD_VARIANCE";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_OFFER_RECERT_SW = "SCREEN_POP_OFFER_RECERT_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_LPP_SW = "SCREEN_POP_LPP_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CARE_SW = "SCREEN_POP_CARE_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_MED_BASELINE_SW = "SCREEN_POP_MED_BASELINE_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_THIRD_PARTY_SW = "SCREEN_POP_THIRD_PARTY_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_FRGN_LNG_CD = "SCREEN_POP_BA_FRGN_LNG_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_SEGMENT = "SCREEN_POP_CUST_SEGMENT";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CORE_AGGR_BILL_CD = "SCREEN_POP_CORE_AGGR_BILL_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_SBA_CD = "SCREEN_POP_SBA_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_TY_CD = "SCREEN_POP_BA_TY_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CASH_ONLY_SW = "SCREEN_POP_CASH_ONLY_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_SPCL_LDGR_SW = "SCREEN_POP_BA_SPCL_LDGR_SW";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_VC_ORDR_QTY = "SCREEN_POP_VC_ORDR_QTY";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_NO_VC_ORDR_QTY = "SCREEN_POP_NO_VC_ORDR_QTY";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_CF_TY_CD = "SCREEN_POP_CF_TY_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_ASC_BILL_CYC_ID = "SCREEN_POP_ASC_BILL_CYC_ID";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_STAT_CD = "SCREEN_POP_BA_STAT_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_BA_CLS_DESC_CD = "SCREEN_POP_BA_CLS_DESC_CD";
        public static readonly String INBOUND_SCG_KVP_KEY_SCREEN_POP_EMAIL_ADDR = "SCREEN_POP_EMAIL_ADDR";

        public static readonly String[] SCG_INBOUND_KVP_LIST = new String[] {
      INBOUND_KVP_KEY_ACTUAL_CALL_TYPE, INBOUND_SCG_KVP_KEY_RESP_ACCOUNT_ID,
      INBOUND_SCG_KVP_KEY_SCREEN_POP_CUSTOMER_NAME, INBOUND_SCG_KVP_KEY_SCREEN_POP_SPOUSE_NAME,
      INBOUND_SCG_KVP_KEY_SCREEN_POP_CUST_ADDRESS, INBOUND_SCG_KVP_KEY_SCREEN_POP_HOME_PHONE,
      INBOUND_KVP_KEY_CARE_PHRASE_OFFERED, INBOUND_KVP_KEY_MODULE,
      INBOUND_KVP_KEY_IVR_DISPOSITION_1, INBOUND_KVP_KEY_IVR_DISPOSITION_2, INBOUND_KVP_KEY_IVR_DISPOSITION_3,
      INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_1, INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_2, INBOUND_KVP_KEY_IVR_DISPOSITION_DESC_3,
      INBOUND_KVP_KEY_CSR_DISPOSITION_1, INBOUND_KVP_KEY_CSR_DISPOSITION_2, INBOUND_KVP_KEY_CSR_DISPOSITION_3,
      INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_1, INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_2, INBOUND_KVP_KEY_CSR_DISPOSITION_DESC_3,
      INBOUND_KVP_KEY_CSR_IVR_PRESELECT_DISP
    };

        //=========================== outbound KVP keys
        public static readonly String OUTBOUND_KVP_KEY_OUTBOUND_DISPOSITION = "OUTBOUND_DISPOSITION";

        public static readonly String[] OUTBOUND_KVP_LIST = new String[] {
                OUTBOUND_KVP_KEY_OUTBOUND_DISPOSITION
        };

        public static readonly String SCG_COLLECTIONS_NOTICEAMT_KVP = "NOTICEAMT";

        public static readonly String SCG_CALLTYPE_KVP = "CALLTYPE";

        public static readonly String SCG_MAILADDRESS1_KVP = "MAILADDRESS1";
        public static readonly String SCG_MAILADDRESS2_KVP = "MAILADDRESS2";
        public static readonly String SCG_MAILADDRESS3_KVP = "MAILADDRESS3";

        /**
	 * Return the environment value for the specified key.
	 * @param key Key.
	 * @return Value.
	 */
        //public static String getEnvironmentValue(String key)
        //{
        //    return ENV_VARS.getProperty(key);
        //}

        // Loads the environment variables.
        //static
        //{
        //    ENV_VARS = new Properties();
        //    try
        //    {
        //        ENV_VARS.load(CISConstants.class.getResourceAsStream(("environment.properties")));
        //        log.info("Environment specific variables loaded successfully");
        //    }
        //    catch(IOException ioe)
        //    {
        //        log.fatalError("IOException occurs while trying to load environment.properties", ioe);
        //    }
        //}
    }
}