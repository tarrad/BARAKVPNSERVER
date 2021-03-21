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
    public class GroupController : Controller
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICountryRepository _countryRepository;

        public GroupController(IGroupRepository groupRepository, IUserRepository userRepository, ICountryRepository countryRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
        }
        [HttpGet("GetGroup")]
        public async Task<IActionResult> GetGroup()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return NotFound("User not found with an id " + currentUserId);
            }
            else
            {
                userFromRepo.Group.Id = userFromRepo.GroupId;
                return Ok(userFromRepo.Group);
            }
        }

        [HttpPost("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup([FromBody] Group group)
        {
            
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
                if (userFromRepo == null || userFromRepo.GroupId != group.Id)
                {
                    return Unauthorized();
                }
            


            var errors = await _groupRepository.UpdateGroup(new GroupDTO(group), userFromRepo);
            return Ok(errors);

        }


        [HttpGet("GetCountry/{term}")]
        public async Task<IActionResult> GetCountry(string term)
        {
            var list = new List<string>();
            if (term.IsNotEmpty() && term.Length > 1)
            {
                list =  _countryRepository.GetCountryAutoComplete(term); 
            }
            return Ok(list);
        }



    }
}
