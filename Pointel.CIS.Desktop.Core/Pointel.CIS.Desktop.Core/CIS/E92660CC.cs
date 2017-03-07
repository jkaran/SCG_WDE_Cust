﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



using System;
using System.Text;
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "file://target.files", ConfigurationName = "E92660CCPortType")]
public interface E92660CCPortType
{

    // CODEGEN: Generating message contract since the operation createScreenPopRequest is neither RPC nor document wrapped.
    [System.ServiceModel.OperationContractAttribute(Action = "urn:E92660CC", ReplyAction = "*")]
    [System.ServiceModel.XmlSerializerFormatAttribute()]
    createScreenPopRequestResponse createScreenPopRequest(createScreenPopRequestRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "urn:E92660CC", ReplyAction = "*")]
    System.Threading.Tasks.Task<createScreenPopRequestResponse> createScreenPopRequestAsync(createScreenPopRequestRequest request);
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.81.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Request/1.0")]
public partial class REQUEST
{

    private SCREEN_POP_REQUEST_DATA screen_pop_request_dataField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
    public SCREEN_POP_REQUEST_DATA screen_pop_request_data
    {
        get
        {
            return this.screen_pop_request_dataField;
        }
        set
        {
            this.screen_pop_request_dataField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.81.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Request/1.0")]
public partial class SCREEN_POP_REQUEST_DATA
{

    private short req_operation_cdField;

    private string req_channel_typeField;

    private string req_database_nameField;

    private long req_account_idField;

    private string req_check_digitField;

    private string req_key_typeField;

    private string req_language_codeField;

    #region ToString Method
    public override string ToString()
    {
        StringBuilder logString = new StringBuilder();
        try
        {
            logString.Append("\n********************ServiceRequest**************************\n");
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                logString.Append(String.Format("{0} = {1}\n", name, value));
            }
            logString.Append("\n*************************************************************");
        }
        catch (Exception)
        {
            logString.ToString();
        }
        return logString.ToString();
    }
    #endregion

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
    public short req_operation_cd
    {
        get
        {
            return this.req_operation_cdField;
        }
        set
        {
            this.req_operation_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
    public string req_channel_type
    {
        get
        {
            return this.req_channel_typeField;
        }
        set
        {
            this.req_channel_typeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
    public string req_database_name
    {
        get
        {
            return this.req_database_nameField;
        }
        set
        {
            this.req_database_nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
    public long req_account_id
    {
        get
        {
            return this.req_account_idField;
        }
        set
        {
            this.req_account_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
    public string req_check_digit
    {
        get
        {
            return this.req_check_digitField;
        }
        set
        {
            this.req_check_digitField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
    public string req_key_type
    {
        get
        {
            return this.req_key_typeField;
        }
        set
        {
            this.req_key_typeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
    public string req_language_code
    {
        get
        {
            return this.req_language_codeField;
        }
        set
        {
            this.req_language_codeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.81.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Response/1.0")]
public partial class SCREEN_POP_DATA
{

    private string screen_pop_home_phoneField;

    private string screen_pop_home_phone_extField;

    private string screen_pop_customer_nameField;

    private string screen_pop_spouse_nameField;

    private string screen_pop_cust_addressField;

    private long screen_pop_cust_idField;

    private string screen_pop_cust_ty_cdField;

    private long screen_pop_cf_idField;

    private string screen_pop_zip_codeField;

    private string screen_pop_mail_addressField;

    private string screen_pop_mail_cityField;

    private string screen_pop_mail_stateField;

    private string screen_pop_mail_zipField;

    private string screen_pop_mail_zip4Field;

    private string screen_pop_check_digitField;

    private string screen_pop_on_simple_payField;

    private string screen_pop_on_demand_payField;

    private decimal screen_pop_curr_bal_dueField;

    private string screen_pop_eligible_swField;

    private string screen_pop_baseField;

    private string screen_pop_ba_term_dtField;

    private string screen_pop_ba_paid_dtField;

    private string screen_pop_ba_open_dtField;

    private string screen_pop_ic_estb_dtField;

    private string screen_pop_overdue_countField;

    private string screen_pop_48hr_countField;

    private string screen_pop_over_one_yrField;

    private string screen_pop_off_greater6Field;

    private string screen_pop_read_ok_swField;

    private short screen_pop_nbr_of_dialsField;

    private string screen_pop_offer_amort_swField;

    private decimal screen_pop_ytd_varianceField;

    private string screen_pop_offer_recert_swField;

    private string screen_pop_lpp_swField;

    private string screen_pop_care_swField;

    private string screen_pop_med_baseline_swField;

    private string screen_pop_third_party_swField;

    private string screen_pop_ba_frgn_lng_cdField;

    private string screen_pop_cust_segmentField;

    private string screen_pop_core_aggr_bill_cdField;

    private string screen_pop_sba_cdField;

    private string screen_pop_ba_ty_cdField;

    private string screen_pop_cash_only_swField;

    private string screen_pop_ba_spcl_ldgr_swField;

    private int screen_pop_vc_ordr_qtyField;

    private int screen_pop_no_vc_ordr_qtyField;

    private string screen_pop_cf_ty_cdField;

    private short screen_pop_asc_bill_cyc_idField;

    private string screen_pop_ba_stat_cdField;

    private string screen_pop_ba_cls_desc_cdField;

    private string screen_pop_email_addrField;

    #region ToString Method
    public override string ToString()
    {
        StringBuilder logString = new StringBuilder();
        try
        {
            logString.Append("\n\tSCREEN_POP_DATA :\n");
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                logString.Append(String.Format("{0} = {1}\n\t", name, value));
            }
        }
        catch (Exception)
        {
            logString.ToString();
        }
        return logString.ToString();
    }
    #endregion

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
    public string screen_pop_home_phone
    {
        get
        {
            return this.screen_pop_home_phoneField;
        }
        set
        {
            this.screen_pop_home_phoneField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
    public string screen_pop_home_phone_ext
    {
        get
        {
            return this.screen_pop_home_phone_extField;
        }
        set
        {
            this.screen_pop_home_phone_extField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
    public string screen_pop_customer_name
    {
        get
        {
            return this.screen_pop_customer_nameField;
        }
        set
        {
            this.screen_pop_customer_nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
    public string screen_pop_spouse_name
    {
        get
        {
            return this.screen_pop_spouse_nameField;
        }
        set
        {
            this.screen_pop_spouse_nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
    public string screen_pop_cust_address
    {
        get
        {
            return this.screen_pop_cust_addressField;
        }
        set
        {
            this.screen_pop_cust_addressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
    public long screen_pop_cust_id
    {
        get
        {
            return this.screen_pop_cust_idField;
        }
        set
        {
            this.screen_pop_cust_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
    public string screen_pop_cust_ty_cd
    {
        get
        {
            return this.screen_pop_cust_ty_cdField;
        }
        set
        {
            this.screen_pop_cust_ty_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
    public long screen_pop_cf_id
    {
        get
        {
            return this.screen_pop_cf_idField;
        }
        set
        {
            this.screen_pop_cf_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 8)]
    public string screen_pop_zip_code
    {
        get
        {
            return this.screen_pop_zip_codeField;
        }
        set
        {
            this.screen_pop_zip_codeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 9)]
    public string screen_pop_mail_address
    {
        get
        {
            return this.screen_pop_mail_addressField;
        }
        set
        {
            this.screen_pop_mail_addressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 10)]
    public string screen_pop_mail_city
    {
        get
        {
            return this.screen_pop_mail_cityField;
        }
        set
        {
            this.screen_pop_mail_cityField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 11)]
    public string screen_pop_mail_state
    {
        get
        {
            return this.screen_pop_mail_stateField;
        }
        set
        {
            this.screen_pop_mail_stateField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 12)]
    public string screen_pop_mail_zip
    {
        get
        {
            return this.screen_pop_mail_zipField;
        }
        set
        {
            this.screen_pop_mail_zipField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 13)]
    public string screen_pop_mail_zip4
    {
        get
        {
            return this.screen_pop_mail_zip4Field;
        }
        set
        {
            this.screen_pop_mail_zip4Field = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 14)]
    public string screen_pop_check_digit
    {
        get
        {
            return this.screen_pop_check_digitField;
        }
        set
        {
            this.screen_pop_check_digitField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 15)]
    public string screen_pop_on_simple_pay
    {
        get
        {
            return this.screen_pop_on_simple_payField;
        }
        set
        {
            this.screen_pop_on_simple_payField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 16)]
    public string screen_pop_on_demand_pay
    {
        get
        {
            return this.screen_pop_on_demand_payField;
        }
        set
        {
            this.screen_pop_on_demand_payField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 17)]
    public decimal screen_pop_curr_bal_due
    {
        get
        {
            return this.screen_pop_curr_bal_dueField;
        }
        set
        {
            this.screen_pop_curr_bal_dueField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 18)]
    public string screen_pop_eligible_sw
    {
        get
        {
            return this.screen_pop_eligible_swField;
        }
        set
        {
            this.screen_pop_eligible_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 19)]
    public string screen_pop_base
    {
        get
        {
            return this.screen_pop_baseField;
        }
        set
        {
            this.screen_pop_baseField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 20)]
    public string screen_pop_ba_term_dt
    {
        get
        {
            return this.screen_pop_ba_term_dtField;
        }
        set
        {
            this.screen_pop_ba_term_dtField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 21)]
    public string screen_pop_ba_paid_dt
    {
        get
        {
            return this.screen_pop_ba_paid_dtField;
        }
        set
        {
            this.screen_pop_ba_paid_dtField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 22)]
    public string screen_pop_ba_open_dt
    {
        get
        {
            return this.screen_pop_ba_open_dtField;
        }
        set
        {
            this.screen_pop_ba_open_dtField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 23)]
    public string screen_pop_ic_estb_dt
    {
        get
        {
            return this.screen_pop_ic_estb_dtField;
        }
        set
        {
            this.screen_pop_ic_estb_dtField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 24)]
    public string screen_pop_overdue_count
    {
        get
        {
            return this.screen_pop_overdue_countField;
        }
        set
        {
            this.screen_pop_overdue_countField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 25)]
    public string screen_pop_48hr_count
    {
        get
        {
            return this.screen_pop_48hr_countField;
        }
        set
        {
            this.screen_pop_48hr_countField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 26)]
    public string screen_pop_over_one_yr
    {
        get
        {
            return this.screen_pop_over_one_yrField;
        }
        set
        {
            this.screen_pop_over_one_yrField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 27)]
    public string screen_pop_off_greater6
    {
        get
        {
            return this.screen_pop_off_greater6Field;
        }
        set
        {
            this.screen_pop_off_greater6Field = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 28)]
    public string screen_pop_read_ok_sw
    {
        get
        {
            return this.screen_pop_read_ok_swField;
        }
        set
        {
            this.screen_pop_read_ok_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 29)]
    public short screen_pop_nbr_of_dials
    {
        get
        {
            return this.screen_pop_nbr_of_dialsField;
        }
        set
        {
            this.screen_pop_nbr_of_dialsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 30)]
    public string screen_pop_offer_amort_sw
    {
        get
        {
            return this.screen_pop_offer_amort_swField;
        }
        set
        {
            this.screen_pop_offer_amort_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 31)]
    public decimal screen_pop_ytd_variance
    {
        get
        {
            return this.screen_pop_ytd_varianceField;
        }
        set
        {
            this.screen_pop_ytd_varianceField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 32)]
    public string screen_pop_offer_recert_sw
    {
        get
        {
            return this.screen_pop_offer_recert_swField;
        }
        set
        {
            this.screen_pop_offer_recert_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 33)]
    public string screen_pop_lpp_sw
    {
        get
        {
            return this.screen_pop_lpp_swField;
        }
        set
        {
            this.screen_pop_lpp_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 34)]
    public string screen_pop_care_sw
    {
        get
        {
            return this.screen_pop_care_swField;
        }
        set
        {
            this.screen_pop_care_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 35)]
    public string screen_pop_med_baseline_sw
    {
        get
        {
            return this.screen_pop_med_baseline_swField;
        }
        set
        {
            this.screen_pop_med_baseline_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 36)]
    public string screen_pop_third_party_sw
    {
        get
        {
            return this.screen_pop_third_party_swField;
        }
        set
        {
            this.screen_pop_third_party_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 37)]
    public string screen_pop_ba_frgn_lng_cd
    {
        get
        {
            return this.screen_pop_ba_frgn_lng_cdField;
        }
        set
        {
            this.screen_pop_ba_frgn_lng_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 38)]
    public string screen_pop_cust_segment
    {
        get
        {
            return this.screen_pop_cust_segmentField;
        }
        set
        {
            this.screen_pop_cust_segmentField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 39)]
    public string screen_pop_core_aggr_bill_cd
    {
        get
        {
            return this.screen_pop_core_aggr_bill_cdField;
        }
        set
        {
            this.screen_pop_core_aggr_bill_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 40)]
    public string screen_pop_sba_cd
    {
        get
        {
            return this.screen_pop_sba_cdField;
        }
        set
        {
            this.screen_pop_sba_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 41)]
    public string screen_pop_ba_ty_cd
    {
        get
        {
            return this.screen_pop_ba_ty_cdField;
        }
        set
        {
            this.screen_pop_ba_ty_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 42)]
    public string screen_pop_cash_only_sw
    {
        get
        {
            return this.screen_pop_cash_only_swField;
        }
        set
        {
            this.screen_pop_cash_only_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 43)]
    public string screen_pop_ba_spcl_ldgr_sw
    {
        get
        {
            return this.screen_pop_ba_spcl_ldgr_swField;
        }
        set
        {
            this.screen_pop_ba_spcl_ldgr_swField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 44)]
    public int screen_pop_vc_ordr_qty
    {
        get
        {
            return this.screen_pop_vc_ordr_qtyField;
        }
        set
        {
            this.screen_pop_vc_ordr_qtyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 45)]
    public int screen_pop_no_vc_ordr_qty
    {
        get
        {
            return this.screen_pop_no_vc_ordr_qtyField;
        }
        set
        {
            this.screen_pop_no_vc_ordr_qtyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 46)]
    public string screen_pop_cf_ty_cd
    {
        get
        {
            return this.screen_pop_cf_ty_cdField;
        }
        set
        {
            this.screen_pop_cf_ty_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 47)]
    public short screen_pop_asc_bill_cyc_id
    {
        get
        {
            return this.screen_pop_asc_bill_cyc_idField;
        }
        set
        {
            this.screen_pop_asc_bill_cyc_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 48)]
    public string screen_pop_ba_stat_cd
    {
        get
        {
            return this.screen_pop_ba_stat_cdField;
        }
        set
        {
            this.screen_pop_ba_stat_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 49)]
    public string screen_pop_ba_cls_desc_cd
    {
        get
        {
            return this.screen_pop_ba_cls_desc_cdField;
        }
        set
        {
            this.screen_pop_ba_cls_desc_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 50)]
    public string screen_pop_email_addr
    {
        get
        {
            return this.screen_pop_email_addrField;
        }
        set
        {
            this.screen_pop_email_addrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.81.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Response/1.0")]
public partial class SCREEN_POP_RESPONSE_DATA
{

    private short resp_scrn_pop_oper_cdField;

    private string resp_return_codeField;

    private string resp_error_msgField;

    private int resp_appl_error_cdField;

    private int resp_system_error_cdField;

    private long resp_account_idField;

    private SCREEN_POP_DATA screen_pop_dataField;

    #region ToString Method
    public override string ToString()
    {
        StringBuilder logString = new StringBuilder();
        try
        {
            logString.Append("\nSCREEN_POP_RESPONSE_DATA :");
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                logString.Append(String.Format("{0} = {1}\n", name, value));
            }
        }
        catch (Exception)
        {
            logString.ToString();
        }
        return logString.ToString();
    }
    #endregion

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
    public short resp_scrn_pop_oper_cd
    {
        get
        {
            return this.resp_scrn_pop_oper_cdField;
        }
        set
        {
            this.resp_scrn_pop_oper_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
    public string resp_return_code
    {
        get
        {
            return this.resp_return_codeField;
        }
        set
        {
            this.resp_return_codeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
    public string resp_error_msg
    {
        get
        {
            return this.resp_error_msgField;
        }
        set
        {
            this.resp_error_msgField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
    public int resp_appl_error_cd
    {
        get
        {
            return this.resp_appl_error_cdField;
        }
        set
        {
            this.resp_appl_error_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
    public int resp_system_error_cd
    {
        get
        {
            return this.resp_system_error_cdField;
        }
        set
        {
            this.resp_system_error_cdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
    public long resp_account_id
    {
        get
        {
            return this.resp_account_idField;
        }
        set
        {
            this.resp_account_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
    public SCREEN_POP_DATA screen_pop_data
    {
        get
        {
            return this.screen_pop_dataField;
        }
        set
        {
            this.screen_pop_dataField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.81.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Response/1.0")]
public partial class RESPONSE
{

    private SCREEN_POP_RESPONSE_DATA screen_pop_response_dataField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
    public SCREEN_POP_RESPONSE_DATA screen_pop_response_data
    {
        get
        {
            return this.screen_pop_response_dataField;
        }
        set
        {
            this.screen_pop_response_dataField = value;
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class createScreenPopRequestRequest
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Request/1.0", Order = 0)]
    public REQUEST REQUEST;

    public createScreenPopRequestRequest()
    {
    }

    public createScreenPopRequestRequest(REQUEST REQUEST)
    {
        this.REQUEST = REQUEST;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
public partial class createScreenPopRequestResponse
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://ns.sempra.com/OpEx/CC/eServices/ScreenPop/Response/1.0", Order = 0)]
    public RESPONSE RESPONSE;

    public createScreenPopRequestResponse()
    {
    }

    public createScreenPopRequestResponse(RESPONSE RESPONSE)
    {
        this.RESPONSE = RESPONSE;
    }
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface E92660CCPortTypeChannel : E92660CCPortType, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class E92660CCPortTypeClient : System.ServiceModel.ClientBase<E92660CCPortType>, E92660CCPortType
{

    public E92660CCPortTypeClient()
    {
    }

    public E92660CCPortTypeClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public E92660CCPortTypeClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public E92660CCPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public E92660CCPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    createScreenPopRequestResponse E92660CCPortType.createScreenPopRequest(createScreenPopRequestRequest request)
    {
        return base.Channel.createScreenPopRequest(request);
    }

    public RESPONSE createScreenPopRequest(REQUEST REQUEST)
    {
        createScreenPopRequestRequest inValue = new createScreenPopRequestRequest();
        inValue.REQUEST = REQUEST;
        createScreenPopRequestResponse retVal = ((E92660CCPortType)(this)).createScreenPopRequest(inValue);
        return retVal.RESPONSE;
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.Threading.Tasks.Task<createScreenPopRequestResponse> E92660CCPortType.createScreenPopRequestAsync(createScreenPopRequestRequest request)
    {
        return base.Channel.createScreenPopRequestAsync(request);
    }

    public System.Threading.Tasks.Task<createScreenPopRequestResponse> createScreenPopRequestAsync(REQUEST REQUEST)
    {
        createScreenPopRequestRequest inValue = new createScreenPopRequestRequest();
        inValue.REQUEST = REQUEST;
        return ((E92660CCPortType)(this)).createScreenPopRequestAsync(inValue);
    }
}
