using BARAKVPNSERVER.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Helpers
{
    public static class InvoiceHelper
    {


        public class AddCustomerRes
        {
            public string id { get; set; }
        }
        public class AddCustomerReq
        {
            public string name { get; set; }
            public bool active { get; set; }
            public string department { get; set; }
            public string taxId { get; set; }
            public int accountingKey { get; set; }
            public string city { get; set; }
            public string phone { get; set; }
            public string contactPerson { get; set; }
            public string address { get; set; }
            public int category { get; set; }
            public int subCategory { get; set; }
            public string country { get; set; }
        }
        public class GetTokenReq
        {
            public string secret { get; set; }
            public string id { get; set; }
        }
        public class GetTokenRes
        {
            public string token { get; set; }
        }
        public static string GetKey(string secret,string id)
        {

            try
            {
                var req = new GetTokenReq
                {
                    secret = secret,
                    id = id
                };
                var client = new RestClient("https://sandbox.d.greeninvoice.co.il");
                var request = new RestRequest("api/v1/account/token");
                request.AddJsonBody(req);
                request.AddHeader("Content-Type", "application/json");
                var response = client.Post(request);
                var content = response.Content; // Raw content as string
                var response2 = client.Post<GetTokenRes>(request);
                return response2.Data.token;
            }
            catch
            {
                return null;
            }
           
        }


        public static string RegisterInvoice(GroupWeb group, string countryCode, string secret, string id,int groupId)
        {

            try
            {

                var key = GetKey(secret, id);

                if(key != null)
                {
                    var client = new RestClient("https://sandbox.d.greeninvoice.co.il");
                    var request = new RestRequest("api/v1/clients");
                    var req = new AddCustomerReq
                    {
                        name = group.Username,
                        active = true,
                        department = "Customers",
                        taxId = group.TaxId,
                        accountingKey = groupId,
                        city = group.City,
                        address = group.Street,
                        category = 1,
                        subCategory = 101,
                        country = countryCode,
                        contactPerson = group.CompanyName,
                        phone = group.PhoneNumber
                    };

                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer "  + key);
                    request.AddJsonBody(req);
                    var response = client.Post(request);
                    var content = response.Content; // Raw content as string

                    if (content != null)
                    {
                        try
                        {
                            var result = JsonConvert.DeserializeObject<AddCustomerRes>(content);
                            return result.id;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return null;
                }
               
            }
            catch
            {
                return null;
               
            }
            return null;

        }



        public static CreateInvoiceDocumentRes CreateInvoice(CreateInvoiceDocumentReq requsetInvoice, string secret, string id)
        {

            try
            {

                var key = GetKey(secret, id);

                if (key != null)
                {
                    var client = new RestClient("https://sandbox.d.greeninvoice.co.il");
                    var request = new RestRequest("api/v1/documents");
                    

                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + key);
                    request.AddJsonBody(requsetInvoice);
                    var response = client.Post(request);
                    var content = response.Content; // Raw content as string

                    if (content != null)
                    {
                        try
                        {
                            var result = JsonConvert.DeserializeObject<CreateInvoiceDocumentRes>(content);
                            return result;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return null;
                }

            }
            catch
            {
                return null;

            }
            return null;

        }
    }
}
