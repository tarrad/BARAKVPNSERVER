using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class InvoiceDocument
    {
        public int Id { get; set; }
        public string InvoiceId { get; set; }
        public string Link { get; set; }
        public int CreatedBy { get; set; }
        public int GroupId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
