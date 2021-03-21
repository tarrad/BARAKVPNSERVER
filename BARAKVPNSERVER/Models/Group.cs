using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class Group
    {

        
        public int Id {get; set;}
        public string  TaxId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string CompanyName { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }

    }
    public class GroupDTO
    {

        
        public string  TaxId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string CompanyName { get; set; }
        public string ZipCode { get; set; }

        public GroupDTO(Group g)
        {
            TaxId = g.TaxId;
            City = g.City;
            Street = g.Street;
            PhoneNumber = g.PhoneNumber;
            Country = g.Country;
            CompanyName = g.CompanyName;
            ZipCode = g.ZipCode;
        }

    }
}
