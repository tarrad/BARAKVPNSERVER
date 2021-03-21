using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class InvoicesDocumentDTO
    {
        public string InvoiceId { get; set; }
        public string Link { get; set; }
        public string CreatedBy { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int NumberOfPages {get;set;}
    }
}
