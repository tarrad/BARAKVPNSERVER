using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class CreateInvoiceDocumentReq
    {
        public CreateInvoiceDocumentReq()
        {
            currency = "USD";
            date = DateTime.Now.ToString("yyyy-MM-dd");
            dueDate = DateTime.Now.ToString("yyyy-MM-dd");
            paymentRequestData = new PaymentRequestData();
            lang = "en";
            signed = true;
            attachment = true;
            rounding = true;
            type = 300;
        }
        public string description { get; set; }
        public string emailContent { get; set; }
        public int type { get; set; }
        public string date { get; set; }
        public string dueDate { get; set; }
        public string lang { get; set; }
        public string currency { get; set; }
        public int vatType { get; set; }
        public bool rounding { get; set; }
        public bool signed { get; set; }
        public bool attachment { get; set; }
        public int maxPayments { get; set; }
        public PaymentRequestData paymentRequestData { get; set; }
        public Client client { get; set; }
        public Income[] income { get; set; }
        public Payment[] payment { get; set; }
    }

    public class Payment
    {
        public Payment()
        {
            date = DateTime.Now.ToString("yyyy-MM-dd");
            currency = "USD";
            currencyRate = 1;
            type = 11;
        }
        public string date { get; set; }
        public int type { get; set; }
        public int price { get; set; }
        public string currency { get; set; }
        public int currencyRate { get; set; }
    }

    public class Client
    {
        public string id { get; set; }
        public string name { get; set; }
        public string[] emails { get; set; }
    }

    public class Income
    {
        public Income()
        {
            currency = "USD";
            vatType = 1; 
            currencyRate = 1;
        }

        public string catalogNum { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
        public string currency { get; set; }
        public int currencyRate { get; set; }
        public string itemId { get; set; }
        public int vatType { get; set; }
    }
    public class PaymentRequestData
    {
        public PaymentRequestData()
        {
            plugins = new Plugins[1];
            plugins[0] = new Plugins();
            maxPayments = 1;
        }
        public Plugins[] plugins { get; set; }
        public int maxPayments { get; set; }
    }
    public class Plugins
    {
        public Plugins()
        {
            id = "031e827a-c664-c1e4-d231-a80027271123";
            group = 100;
            type = 12010;
        }
        public string id { get; set; }
        public int group { get; set; }
        public int type { get; set; }

    }

    public class CreateInvoiceDocumentRes
    {
        public string id { get; set; }
        public int number { get; set; }
        public bool signed { get; set; }
        public string lang { get; set; }
        public LinkUrl url { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }    
    }

    public class LinkUrl
    {
        public string origin { get; set; }
        public string he { get; set; }
        public string en { get; set; }



    }
}
