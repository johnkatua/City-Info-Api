using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Claims;

namespace CityInfoAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public class AuthenticationRequestBody {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city) {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthController(IConfiguration config) {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authReqBody) {
            // validate username and password
            var user = ValidateUserCredentials(authReqBody.UserName, authReqBody.Password);

            if (user == null) {
                return Unauthorized();
            }

            // create a token
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // data present in the token
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("first_name", user.FirstName));
            claimsForToken.Add(new Claim("last_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                _config["Authentication:Issuer"],
                _config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow, // start time
                DateTime.UtcNow.AddHours(1), // expiry time
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password) {
            return new CityInfoUser(1, userName ?? "", "John", "Katua", "Nairobi");
        }
    }
}