using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BARAKVPNSERVER.Models;
using BARAKVPNSERVER.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BARAKVPNSERVER.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]GroupWeb group)
        {
            var res =  await _userRepository.Register(group);
            return Ok(res);
        }
         [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserForLogin user)
        {
            try
            {
                var res = await _userRepository.Login(user.Username, user.Password);
                if (res != null)
                {
                    if(!res.Confirmed)
                    {
                        return Ok(new { error = "Please Confirm Registration" });
                    }
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:token").Value);
                    var securityDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                            new Claim(ClaimTypes.NameIdentifier,res.Id.ToString()),
                    new Claim(ClaimTypes.Name, res.Username),
                    new Claim("Group", JsonConvert.SerializeObject(res.Group)),

                }),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                    };
                    res.HashedPassword = null;
                    res.SaltPassword = null;
                    var token = tokenHandler.CreateToken(securityDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);
                    return Ok(new { tokenString, res });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return BadRequest(500);
            }
            
        }

        [Authorize]

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] UserAdd user)
            {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            user.GroupId = userFromRepo.GroupId;
            var errors = await _userRepository.AddUser(user);
            return Ok(errors);


        }

        [Authorize]

        [HttpGet("GetAllUsersByGroup")]
        public async Task<IActionResult> GetAllUsersByGroup()
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var users = await _userRepository.GetAllUsersByGroupId(userFromRepo.GroupId);
            return Ok(users);


        }

        [Authorize]

        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] UserAdd user)
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var errors = await _userRepository.EditUser(userFromRepo, user);
            return Ok(errors);


        }

        [HttpPut("DeleteUser/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userRepository.GetActiveAdminUser(currentUserId);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var errors = await _userRepository.DeleteUser(username, userFromRepo);
            return Ok(errors);


        }


    }
}
