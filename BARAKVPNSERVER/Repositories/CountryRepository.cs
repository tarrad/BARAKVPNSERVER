using BARAKVPNSERVER.Data;
using BARAKVPNSERVER.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public class CountryRepository : ICountryRepository
    {

        public CountryRepository()
        {
            
        }

        public string GetCountry(string country)
        {

            using (var _context = new DataContext())
            {
                var coun = _context.Countries.Where(a => a.Name == country).FirstOrDefault();

                if (coun != null)
                {
                    return coun.Code;
                }
                return null;
            }





        }
           


        

        public List<string> GetCountryAutoComplete(string term)
        {
            var list = new List<string>();
            if(term.IsNotEmpty())
            {

                using (var _context = new DataContext())
                {
                    var cont = _context.Countries.Where(c => EF.Functions.Like(c.Name, term + "%")).Take(5);
                    if (cont != null)
                    {
                        foreach (var c in cont)
                        {
                            list.Add(c.Name);
                        }
                    }
                }
               
            }
            return list;
        }

        public string GetCountryCode(string country)
        {
            throw new NotImplementedException();
        }
    }
}
