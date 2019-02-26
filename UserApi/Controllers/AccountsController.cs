using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserApi.Data;

namespace UserApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private readonly UserDbContext _context;

        public AccountsController(UserDbContext context)
        {
            _context = context;
        }

        // POST: api/Accounts/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //바디로부터 파라미터를 객체로 받고, 그 객체는 뭘 넣어주던.. 암튼 그리고 객체내 넘버를 가지고 데이터베이스 자료와 비교후 해당되는 것 찾기
            var account = _context.Users.SingleOrDefault(m => m.Email.Equals(user.Email));

            if (account == null)
            {
                return NotFound("Id is not founded");
            }


            // Token Generation

            // Security Key
            // string securityKey = "top_secret";

            // Symmetric security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes("top secret"));
            
            // Signing Credentials
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims
            //var claims = new List<Claim>();
            //claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email)
            };

            // Token
            var token = new JwtSecurityToken(
                //issuer: "dev.team",
                //audience: "readers",
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddMinutes(30)        
            );
            
            var t = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = t });
        }



    }
}