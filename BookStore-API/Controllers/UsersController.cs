using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Data;
using BookStore_API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            ILoggerService logger,
            IConfiguration config) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")] //required because there are 2 httpposts
        public async Task <IActionResult> Register([FromBody] UserDTO userDTO)
        {
            //userDTO will suit both Login and Register (username and password)
            //There are 2 HttpPost endpoints so need to route them properly
           
            var location = GetControllerActionNames();
            try
            {
                var username = userDTO.EmailAddress;
                var password = userDTO.Password;
                _logger.LogInfo($"{location}: Registration attempt from user {username} ");
                var user = new IdentityUser
                {
                    Email = username,
                    UserName = username
                };
                var result = await _userManager.CreateAsync(user: user,
                                                            password: password);
                if(result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"{location}: {error.Code} {error.Description}");
                    }
                    return InternalError($"{location}: {username} User Registration Attempt Failed");
                }
                await _userManager.AddToRoleAsync(user, "Customer");
                return Created("login", new { result.Succeeded });

            }
            catch(Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException} ");
            }
        }



        /// <summary>
        /// User Login endppoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task <IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var username = userDTO.EmailAddress;
                var password = userDTO.Password;

                _logger.LogInfo($"{location}: Login attempt from user {username} ");
                var result = await _signInManager.PasswordSignInAsync(username,password,false,false);
               
                if(result.Succeeded == true)
                {
                    _logger.LogInfo($"{location}: {username} Successfully Authenticated");
                    var user = await _userManager.FindByNameAsync(username);
                    var tokenString = await GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString });
                }
                _logger.LogInfo($"{location}: {username} Not Authenticated");

                return Unauthorized(userDTO);

            }
            catch (Exception e)
            {

                return InternalError($"{location}: {e.Message} {e.InnerException} ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //create claims we want to include in string - what is  your information - who are you claiming to be?
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);

            //Get role names  - just a list of strings, for each create claim
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType,r)));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                notBefore: null,
                expires: DateTime.Now.AddMinutes(5), //expires 5 minutes after it's issued
                signingCredentials: credentials);


            //return token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// What controller and action is making a given call
        /// Just to improve the log
        /// </summary>
        /// <returns></returns>
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        /// <summary>
        /// Present consistant error status message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ObjectResult InternalError(string message)
        {
            //https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
            _logger.LogError(message);
            return StatusCode(500, "Something mad happened here");  //Internal server error code
        }

    }
}
