using BARAKVPNSERVER.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BARAKVPNSERVER.Data;
using BARAKVPNSERVER.Helpers;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;

namespace BARAKVPNSERVER.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IConfiguration _configuration;
        private readonly ICountryRepository _countryRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBarakVpnRepository _contextRepository;

        public UserRepository( IGroupRepository groupRepository, IConfiguration configuration, ICountryRepository countryRepository, IInvoiceRepository invoiceRepository, IBarakVpnRepository contextRepository)
        {
            _groupRepository = groupRepository;
            _configuration = configuration;
            _countryRepository = countryRepository;
            _invoiceRepository = invoiceRepository;
            _contextRepository = contextRepository;
        }
        public async Task<User> Login(string username, string password)
        {

            using (var _context = new DataContext())
            {
                var user = await _context.Users.Include(h => h.Group).FirstOrDefaultAsync(x => x.Username.ToLower() == username);

                if (user != null)
                {
                    if (!VerifyPasswordHashed(password, user.HashedPassword, user.SaltPassword))
                    {
                        return null;
                    }

                    user.Group = DecryptGroup(user.Group);
                    return user;

                }
                else
                {
                    return null;
                }
            }
            
        }

        public async Task<List<string>> Register(GroupWeb group)
        {
            List<string> Errors = new List<string>();

            try
            {

                if (await UserExists(group.Username))
                {
                    Errors.Add("Username Exists!");
                    return Errors;
                }

                Errors = CheckRegexes(group);

                if(Errors != null && Errors.Count > 0)
                {
                    return Errors;
                }

                Group newGroup = new Group
                {
                    TaxId = EncryptDecrypt.Encrypt(group.TaxId, _configuration.GetSection("AppSettings:cipher").Value),
                    City = EncryptDecrypt.Encrypt(group.City, _configuration.GetSection("AppSettings:cipher").Value),
                    Street = EncryptDecrypt.Encrypt(group.Street, _configuration.GetSection("AppSettings:cipher").Value),
                    PhoneNumber = EncryptDecrypt.Encrypt(group.PhoneNumber, _configuration.GetSection("AppSettings:cipher").Value),
                    Country = EncryptDecrypt.Encrypt(group.Country, _configuration.GetSection("AppSettings:cipher").Value),
                    CompanyName = EncryptDecrypt.Encrypt(group.CompanyName, _configuration.GetSection("AppSettings:cipher").Value),
                    ZipCode = EncryptDecrypt.Encrypt(group.ZipCode , _configuration.GetSection("AppSettings:cipher").Value),
                };
                newGroup.Email = group.Username.ToLower();
                var groupId = await _groupRepository.CreateGroup(newGroup);
                if (groupId > 0)
                {
                    var user = new User
                    {
                        Username = group.Username.ToLower(),
                        LastName = group.LastName,
                        FirstName = group.FirstName,
                        Admin = true,
                        Billing = true,
                        Active = false,
                        Confirmed = false,
                        GroupId = groupId
                    }
                        ;


                    byte[] passwordHash = null, passwordSalt = null;
                    CreatePasswordHash(group.Password, out passwordHash, out passwordSalt);
                    user.HashedPassword = passwordHash;
                    user.SaltPassword = passwordSalt;
                     _contextRepository.Add(user);
                    if(await _contextRepository.SaveAll())
                    {
                        Emails.SendEmailConfirmation(user.Username.ToLower(), Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:JWTKEY").Value));
                        var countryCode = _countryRepository.GetCountry(group.Country);
                        var invoiceId = InvoiceHelper.RegisterInvoice(group, countryCode, _configuration.GetSection("AppSettings:secretinvoice").Value, _configuration.GetSection("AppSettings:idinvoice").Value, groupId);
                        if(invoiceId != null)
                        {
                            var invoiceObj = new InvoiceDetail
                            {
                                GroupId = groupId,
                                Token = invoiceId
                            };

                           await _invoiceRepository.AddClient(invoiceObj);
                        }
                    }



                }
            }
            catch (Exception e)
            {
                Errors.Add("An Error Occoured");
            }
            return Errors;
        }

        public async Task<bool> UserExists(string username)
        {
            using (var _context = new DataContext())
            {
                return await _context.Users.AnyAsync(x => x.Username == username);
            }
                
        }

        private List<string> CheckRegexes(GroupWeb group)
        {
            var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
            var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
            var hasMinimum8Chars = new System.Text.RegularExpressions.Regex(@".{8,}");

            var errors = new List<string>();
            if (group.Password != null)
            {
                var isValidated = hasNumber.IsMatch(group.Password) && hasUpperChar.IsMatch(group.Password) && hasMinimum8Chars.IsMatch(group.Password);

                if (!isValidated)
                {
                    errors.Add("Password is not valid");
                }
            }
            else
            {
                errors.Add("You must provide Password");
            }
            if (group.FirstName == null || group.FirstName == "")
            {
                errors.Add("You must provide FirstName");
            }
            if (group.LastName == null || group.LastName == "")
            {
                errors.Add("You must provide LastName");
            }

            if (group.Username == null || group.Username == "")
            {

                errors.Add("You must provide Email");
            }
            else
            {
                try
                {
                    MailAddress m = new MailAddress(group.Username);

                }
                catch (FormatException)
                {
                    errors.Add("You must provide valid Email");

                }
            }

            if(group.Country == null || group.Country == "")
            {
                errors.Add("You must provide Contry");
            }
            else
            {
                var code = _countryRepository.GetCountry(group.Country);

                if(code == null)
                {
                    errors.Add("You must provide valid Contry");
                }
            }

            if(group.ZipCode == null)
            {
                group.ZipCode = "";
            }




            return errors;

        }


        private List<string> CheckRegexes(UserAdd user)
        {
            var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
            var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
            var hasMinimum8Chars = new System.Text.RegularExpressions.Regex(@".{8,}");

            var errors = new List<string>();
            if (user.Password != null)
            {
                var isValidated = hasNumber.IsMatch(user.Password) && hasUpperChar.IsMatch(user.Password) && hasMinimum8Chars.IsMatch(user.Password);

                if (!isValidated)
                {
                    errors.Add("Password is not valid");
                }
            }
            else
            {
                errors.Add("You must provide Password");
            }
            if (user.FirstName == null || user.FirstName == "")
            {
                errors.Add("You must provide FirstName");
            }
            if (user.LastName == null || user.LastName == "")
            {
                errors.Add("You must provide LastName");
            }

            if (user.Username == null || user.Username == "")
            {

                errors.Add("You must provide Email");
            }
            else
            {
                try
                {
                    MailAddress m = new MailAddress(user.Username);

                }
                catch (FormatException)
                {
                    errors.Add("You must provide valid Email");

                }
            }

            return errors;

        }
        public async Task<User> GetActiveAdminUser(int Id)
        {
            using (var _context = new DataContext())
            {
                var user = await _context.Users.Include(p => p.Group).FirstOrDefaultAsync(a => a.Id == Id && a.Active && a.Admin);
                if (user != null)
                {
                    user.Group = DecryptGroup(user.Group);
                }
                return user;
            }
            
            
        }


        public async Task<List<string>> AddUser(UserAdd user)
        {
            

            var errors = CheckRegexes(user);
            if(errors.IsNotEmpty())
            {
                return errors;
            }
            if (await UserExists(user.Username))
            {
                errors.Add("Username Exists!");
                return errors;
            }
            var userNew = new User
            {
                Username = user.Username.ToLower(),
                Admin = user.IsAdmin,
                Billing = user.IsBilling,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GroupId = user.GroupId
            };
            byte[] passwordHash = null, passwordSalt = null;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);
            userNew.HashedPassword = passwordHash;
            userNew.SaltPassword = passwordSalt;
            _contextRepository.Add(userNew);
            await _contextRepository.SaveAll();
            return errors;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHashed(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (!computedHash[i].Equals(passwordHash[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private Group DecryptGroup(Group group)
        {
            Group newGroup = new Group
            {
                TaxId = EncryptDecrypt.Decrypt(group.TaxId, _configuration.GetSection("AppSettings:cipher").Value),
                City = EncryptDecrypt.Decrypt(group.City, _configuration.GetSection("AppSettings:cipher").Value),
                Street = EncryptDecrypt.Decrypt(group.Street, _configuration.GetSection("AppSettings:cipher").Value),
                PhoneNumber = EncryptDecrypt.Decrypt(group.PhoneNumber, _configuration.GetSection("AppSettings:cipher").Value),
                Country = EncryptDecrypt.Decrypt(group.Country, _configuration.GetSection("AppSettings:cipher").Value),
                CompanyName = EncryptDecrypt.Decrypt(group.CompanyName, _configuration.GetSection("AppSettings:cipher").Value),
                ZipCode = EncryptDecrypt.Decrypt(group.ZipCode, _configuration.GetSection("AppSettings:cipher").Value),
            };
            return newGroup;
        }

        public async Task<List<UserAdd>> GetAllUsersByGroupId(int Id)
        {
            using (var _context = new DataContext())
            {
                List<UserAdd> ret = new List<UserAdd>();
                var users = await _context.Users.Where(a => a.GroupId == Id).ToListAsync();
                
                if (users != null)
                {
                    foreach(var user in users)
                    {
                        ret.Add(new UserAdd
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            IsAdmin = user.Admin,
                            IsBilling = user.Billing,
                            Username = user.Username,
                        });
                    }
                   
                }
                return ret;
            }
        }

        public async Task<List<string>> EditUser(User user, UserAdd userToEdit)
        {
            using (var _context = new DataContext())
            {
                List<string> ret = new List<string>();
                var userEdit = await _context.Users.Where(a => a.Username == userToEdit.Username).FirstOrDefaultAsync();

                if (user != null && user.GroupId == user.GroupId)
                {
                    userEdit.FirstName = userToEdit.FirstName;
                    userEdit.LastName = userToEdit.LastName;
                    userEdit.Billing = userToEdit.IsBilling;
                    userEdit.Admin = userToEdit.IsAdmin;
                    _contextRepository.Update(userEdit);
                    var x = await _contextRepository.SaveAll();
                }
                else
                {
                    ret.Add("Username not connected to group id");
                }
                return ret;
            }
        }

        public async Task<List<string>> DeleteUser(string username, User user)
        {
            using (var _context = new DataContext())
            {
                List<string> ret = new List<string>();
                var userEdit = await _context.Users.Where(a => a.Username == username).FirstOrDefaultAsync();

                if (user != null && user.GroupId == user.GroupId)
                {
                    _contextRepository.Delete(userEdit);
                    var x = await _contextRepository.SaveAll();
                }
                else
                {
                    ret.Add("Username not connected to group id");
                }
                return ret;
            }
        }
    }
}
