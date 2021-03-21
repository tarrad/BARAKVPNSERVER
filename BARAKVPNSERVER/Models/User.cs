using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] SaltPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public bool Active { get; set; }
        public bool Admin { get; set; }
        public bool Billing { get; set; }
        public bool Confirmed { get; set; }
    }
}
