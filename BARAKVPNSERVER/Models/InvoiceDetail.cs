using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class InvoiceDetail
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string Token { get; set; }
    }
}
