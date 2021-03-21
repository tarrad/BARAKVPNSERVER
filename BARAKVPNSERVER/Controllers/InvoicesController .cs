using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BARAKVPNSERVER.Helpers;
using BARAKVPNSERVER.Models;
using BARAKVPNSERVER.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BARAKVPNSERVER.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicesController(IInvoiceRepository invoiceRepository, IUserRepository userRepository, ICountryRepository countryRepository)
        {
            _userRepository = userRepository;
            _invoiceRepository = invoiceRepository;
        }



        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDocumentReq req)
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            if(req == null)
            {
                return BadRequest();
            }
            if (userFromRepo.Admin || userFromRepo.Billing)
            {
                var errors = await _invoiceRepository.CreateInvoice(req, userFromRepo);
                return Ok(errors);
            }
            else
            {
                return Unauthorized();
            }





        }

        [HttpGet("GetInvoicesByGroupId/{pageNo}")]
        public async Task<IActionResult> GetInvoicesByGroupId(int pageNo)
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }


            var ret = await _invoiceRepository.GetInvoiceByGroupId(userFromRepo.GroupId, pageNo);
            return Ok(ret);






        }
    }
}


