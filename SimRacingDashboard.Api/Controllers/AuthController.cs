using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimRacingDashboard.Api.Dtos;
using SimRacingDashboard.Api.Dtos.Auth;
using SimRacingDashboard.Api.Services;

namespace SimRacingDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email.ToLower(), request.Password);

            if (token == null)
                return Unauthorized(ApiResponse.Fail("Invalid username or password."));

            // Set JWT as HttpOnly, Secure cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse { Token = token }));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.RegisterAsync(request.Email.ToLower(), request.Username, request.Password);

            if (user == null)
                return BadRequest(ApiResponse.Fail("User already exists"));

            return Created();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized(ApiResponse.Fail("Missing token"));

            var user = await _authService.GetCurrentUserAsync(token);
            if (user == null)
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            return Ok(ApiResponse<UserResponse>.Ok(new UserResponse(user)));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (!Request.Cookies.ContainsKey("jwt"))
                return Ok(ApiResponse.Ok("Already logged out"));

            Response.Cookies.Delete("jwt");
            return Ok(ApiResponse.Ok("Logged out successfully"));
        }
    }
}
