using BirthDateReminder.Server.Dtos;
using BirthDateReminder.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BirthDateReminder.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string token = await _authService.LoginAsync(formData);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string token = await _authService.RegisterAsync(formData);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }   
    }
}
