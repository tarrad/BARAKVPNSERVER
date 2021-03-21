using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Helpers
{
    public class InternalInvoiceResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSucces { get; set; }
    }
}
