using System.Collections.Generic;
using System.Linq;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {   
        [HttpPost]
        [Route("auth")]
        public IActionResult Auth([FromBody]Membership membership)
        {
            var accounts = GetAccounts();
            var account = accounts.FirstOrDefault(g => g.UserName.ToUpper() == membership?.UserName?.ToUpper() && g.Password == membership?.Password);
            if(account == null)
            {
                return BadRequest("User Name or Password is invalid");
            }

            var claims = new List<Claim>() 
            { 
                new Claim("sid", account.UserName),
                new Claim("firstName", account.FirstName),
                new Claim("lastName", account.LastName), 
            };
            
            return Ok(new AuthModel
            {
                JwtToken = JwtService.GenerateTokenForUser(claims.ToArray())
            });
        }

        private List<Membership> GetAccounts()
        {
            var json = System.IO.File.ReadAllText("membership.json");
            return JsonConvert.DeserializeObject<List<Membership>>(json);
        }
    }
}