using CryptographyMailApi.Models;
using CryptographyMailApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptographyMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;

        public AuthController(AuthService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("E-posta ve şifre gereklidir.");
            }

            var (newUser, privateKey) = _authService.Register(request.Email, request.Password);
            var token = _tokenService.GenerateToken(newUser);
            return Ok(new { newUser.Email, PrivateKey = privateKey, Token = token });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            var authenticatedUser = _authService.Authenticate(request.Email, request.Password);
            if (authenticatedUser == null)
                return Unauthorized();

            var token = _tokenService.GenerateToken(authenticatedUser);

            return Ok(new { authenticatedUser.Email, Token = token });
        }
    }

    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
