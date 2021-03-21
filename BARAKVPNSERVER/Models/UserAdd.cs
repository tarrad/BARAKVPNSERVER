using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Models
{
    public class UserAdd
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBilling { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public int GroupId { get; set; }
    }
}
