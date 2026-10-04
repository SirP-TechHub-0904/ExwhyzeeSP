using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExwhyzeeSP.Core.Dtos.Paystack
{
    public class TransactionResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public DataLog data { get; set; }
    }
   
    public class Metadata
    {
        public string referrer { get; set; }

        [JsonProperty("custom-fields")]
        public IList<CustomField> CustomFields { get; set; }
    }

    public class History
    {
        public string type { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }


    public class Authorization
    {
        public string authorization_code { get; set; }
        public string card_type { get; set; }
        public string last4 { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string bin { get; set; }
        public string bank { get; set; }
        public string channel { get; set; }
        public string signature { get; set; }
        public bool reusable { get; set; }
        public string country_code { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public object customer_code { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }

    public class DataLog
    {
        public int id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public decimal amount { get; set; }
        public object message { get; set; }
        public string gateway_response { get; set; }
        public object paid_at { get; set; }
        public string created_at { get; set; }
        public string channel { get; set; }
        public string currency { get; set; }
        public string ip_address { get; set; }
        public Metadata metadata { get; set; }
        public object log { get; set; }
        public object fees { get; set; }
        public object fees_split { get; set; }
        public Authorization authorization { get; set; }
        public Customer customer { get; set; }
        public object plan { get; set; }
        public object order_id { get; set; }
        public object paidAt { get; set; }
        public string createdAt { get; set; }
        public int requested_amount { get; set; }
        public object pos_transaction_data { get; set; }
        public object source { get; set; }
        public object fees_breakdown { get; set; }
    }
}
