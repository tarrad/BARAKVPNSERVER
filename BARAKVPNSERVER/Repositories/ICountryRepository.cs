using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public interface ICountryRepository
    {
        public string GetCountryCode(string country);
        public string GetCountry(string country);
        public List<string> GetCountryAutoComplete(string term);
    }
}
