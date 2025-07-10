using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using BirthDateReminder.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace BirthDateReminder.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public SettingsController(AppDbContext context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSettings()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(new {user.Email, user.NotifyInBD, user.NotifyDayBefore});
        }

        [HttpPut("notifications")]
        public async Task<IActionResult> UpdateNotifications([FromBody] NotificationsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.NotifyDayBefore = dto.NotifyDayBefore;
            user.NotifyInBD = dto.NotifyInBD;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }

        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetEmailAsync(user, dto.NewEmail);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            await _emailService.SendEmailAsync(user.Email, "Тестовое письмо", "Так будет выглядеть напоминание о дне рождения");

            return Ok();
        }
    }
}
