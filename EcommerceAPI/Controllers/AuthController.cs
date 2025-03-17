using EcommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            // Simulated user validation - replace with real user authentication
            if (model.Username == "admin" && model.Password == "password")
            {
                var token = GenerateJwtToken(model.Username, "Admin"); // Assign Admin Role
                return Ok(new { token });
            }
            else if (model.Username == "user" && model.Password == "password")
            {
                var token = GenerateJwtToken(model.Username, "User"); // Assign User Role
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)  // Assign Role
            };

            var key = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
