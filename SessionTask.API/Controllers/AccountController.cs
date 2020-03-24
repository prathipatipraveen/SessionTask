using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SessionTask.DataAccess.Services;
using SessionTask.Models;
using SessionTask.Models.Helpers;
using SessionTask.Models.SignIn;

namespace SessionTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class AccountController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ISessionTaskRepository _sessionTaskRepository;
        
        public AccountController(IOptions<AppSettings> appSettings, ISessionTaskRepository sessionTaskRepository)
        {
            _appSettings = appSettings.Value;
            _sessionTaskRepository = sessionTaskRepository;

        }

        [HttpPost]
        public IActionResult SignIn(SignInRequest request)
        {
            var user = _sessionTaskRepository.AuthenticateUser(request.Username, request.Password);
            // return null if user not found
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" }); ;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var featurePermissions = JsonConvert.SerializeObject(user.Features);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim("Features", featurePermissions)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return Ok(user);
        }
    }
}
