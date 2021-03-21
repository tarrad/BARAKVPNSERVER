using BARAKVPNSERVER.Data;
using BARAKVPNSERVER.Helpers;
using BARAKVPNSERVER.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IBarakVpnRepository _barakVpnRepository;
        public InvoiceRepository(IConfiguration configuration, IBarakVpnRepository barakVpnRepository)
        {
            _configuration = configuration;
            _barakVpnRepository = barakVpnRepository;
        }
        public async Task<int> AddClient(InvoiceDetail invoice)
        {

            using (var _context = new DataContext())
            {
                await _context.InvoiceDetails.AddAsync(invoice);
                return await _context.SaveChangesAsync();
            }
           
            
        }

        public async Task<InternalInvoiceResponse> CreateInvoice(CreateInvoiceDocumentReq invoice, User user)
        {
            var ret = new InternalInvoiceResponse();
            ret.IsSucces = true;
            using (var _context = new DataContext())
            {
                var clientInvoice = _context.InvoiceDetails.Where(a => a.GroupId == user.GroupId).FirstOrDefault();

                if (clientInvoice != null)
                {
                    var group = _context.Groups.Where(a => a.Id == user.Id).FirstOrDefault();
                    invoice.client = new Client();
                    invoice.client.id = clientInvoice.Token;
                    invoice.client.name = user.Username;
                    if (group != null && !group.Email.IsNotEmpty())
                    {
                        invoice.client.emails = new string[2];
                        invoice.client.emails[0] = group.Email;
                        invoice.client.emails[1] = user.Username;
                    }
                    var newInvice = InvoiceHelper.CreateInvoice(invoice, _configuration.GetSection("AppSettings:secretinvoice").Value, _configuration.GetSection("AppSettings:idinvoice").Value);
                    if (newInvice != null && newInvice.errorCode == 0)
                    {


                        InvoiceDocument toAdd = new InvoiceDocument
                        {
                            InvoiceId = newInvice.id,
                            Link = newInvice.url.en,
                            CreatedBy = user.Id,
                            CreationDate = DateTime.Now,
                            GroupId = user.GroupId,
                            Amount = invoice.income[0].price,
                            Currency = invoice.income[0].currency,
                            Description = invoice.description
                        };
                        
                        _barakVpnRepository.Add(toAdd);
                        await _barakVpnRepository.SaveAll();
                        ret.Id = newInvice.id;
                        ret.Url = newInvice.url.en;

                    }
                    else
                    {
                        if(newInvice != null)
                        {
                            ret.ErrorMessage = newInvice.errorMessage;
                            ret.IsSucces = false;

                        }
                        else
                        {
                            ret.IsSucces = false;
                        }
                    }



                }
                else
                {
                    ret.IsSucces = false;
                    ret.ErrorMessage = "User not assigned to group";
                }
                return ret;
            }
        }

        public async Task<List<InvoicesDocumentDTO>> GetInvoiceByGroupId(int groupId,int pageNo)
        {

            var ret = new List<InvoicesDocumentDTO>();
            if(pageNo < 1)
            {
                pageNo = 1;
            }
            using (var _context = new DataContext())   
            {
                var invoices  =  _context.InvoiceDocuments.Where(a => a.GroupId == groupId).Skip(10 * (pageNo-1)).Take(10).ToList();
               
                if(invoices != null && invoices.Count > 0)
                {
                    invoices = invoices.OrderByDescending(a => a.CreationDate).ToList();
                    var users = _context.Users.Where(a => a.GroupId == groupId).ToList();
                    if(users != null)
                    {
                        var usersDict = new Dictionary<int, string>();

                        foreach (var user in users)
                        {
                            usersDict[user.Id] = user.Username;
                        }

                        var noOfPages = _context.InvoiceDocuments.Where(a => a.GroupId == groupId).Count();
                        var divide = noOfPages / 10;
                        if(divide*10 < noOfPages)
                        {
                            divide++;
                        }
                        foreach (var invoice in invoices)
                        {

                            var toAdd = new InvoicesDocumentDTO
                            {
                                InvoiceId = invoice.InvoiceId,
                                CreationDate = invoice.CreationDate,
                                Link = invoice.Link,
                                Amount = invoice.Amount,
                                Currency = invoice.Currency,
                                Description = invoice.Description,
                                NumberOfPages = divide
                            };

                            if (usersDict.ContainsKey(invoice.CreatedBy))
                            {
                                toAdd.CreatedBy = usersDict[invoice.CreatedBy];
                            }
                            ret.Add(toAdd);
                        }
                    }

                }

                return ret;
            }

        }
    }
}
