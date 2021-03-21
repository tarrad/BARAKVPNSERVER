using BARAKVPNSERVER.Data;
using BARAKVPNSERVER.Helpers;
using BARAKVPNSERVER.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IBarakVpnRepository _contextRepository;
        private readonly IConfiguration _configuration;


        public GroupRepository( ICountryRepository countryRepository, IConfiguration configuration, IBarakVpnRepository contextRepository)
        {
            _countryRepository = countryRepository;
            _configuration = configuration;
            _contextRepository = contextRepository;
        }
        public async Task<int> CreateGroup(Group group)
        {
            _contextRepository.Add(group);
            await _contextRepository.SaveAll();
            return group.Id;
        }

        public async Task<Group> GetGroupGroup(int id)
        {

            using (var _context = new DataContext())
            {
                return _context.Groups.Where(a => a.Id == id).FirstOrDefault();
            }
            
        }

        public async Task<List<string>> UpdateGroup(GroupDTO group, User user)
        {
            List<string> errors = new List<string>();

            try
            {
                if (group != null)
                {
                    var countryCode = _countryRepository.GetCountry(group.Country);
                    if (countryCode == null)
                    {
                        errors.Add("Incorrect Country");
                    }


                    if (group.TaxId == null || group.TaxId == "")
                    {
                        errors.Add("Missing TaxId");
                    }

                    if (errors.Count > 0)
                    {
                        return errors;
                    }



                    using (var _context = new DataContext())
                    {
                        var grpToUpdate = _context.Groups.Where(a => a.Id == user.GroupId).FirstOrDefault();
                        if (grpToUpdate != null)
                        {
                            grpToUpdate.TaxId = EncryptDecrypt.Encrypt(group.TaxId, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.City = EncryptDecrypt.Encrypt(group.City, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.Street = EncryptDecrypt.Encrypt(group.Street, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.PhoneNumber = EncryptDecrypt.Encrypt(group.PhoneNumber, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.Country = EncryptDecrypt.Encrypt(group.Country, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.CompanyName = EncryptDecrypt.Encrypt(group.CompanyName, _configuration.GetSection("AppSettings:cipher").Value);
                            grpToUpdate.ZipCode = EncryptDecrypt.Encrypt(group.ZipCode, _configuration.GetSection("AppSettings:cipher").Value);
                            _contextRepository.Update(grpToUpdate);
                            var x = await _contextRepository.SaveAll();
                        }





                    }


                }
            }
            catch
            {
                errors.Add("Internal Error");
            }
            return errors;


        }
    }
}
