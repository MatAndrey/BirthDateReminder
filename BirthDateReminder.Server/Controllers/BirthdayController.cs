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
    public class BirthdayController : ControllerBase
    {
        private readonly BirthdayService _birthdayService;

        public BirthdayController(BirthdayService birthdayService)
        {
            _birthdayService = birthdayService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _birthdayService.GetUserBirthdaysAsync(User));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _birthdayService.GetBirthdayAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BirthdayDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var birthday = await _birthdayService.CreateBirthdayAsync(formData, User);
                return CreatedAtAction(nameof(Get), new { id = birthday.Id }, birthday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BirthdayDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                await _birthdayService.UpdateBirthdayAsync(formData, id);
                return NoContent();
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _birthdayService.DeleteBirthdayAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("reminder")]
        public async Task<IActionResult> CreateReminder([FromBody] ReminderDto dto)
        {
            try
            {
                var reminders = await _birthdayService.CreateReminderAsync(dto);
                return Ok(reminders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("reminder/{id}")]
        public async Task<IActionResult> CreateReminder(int id)
        {
            try
            {
                return Ok(await _birthdayService.DeleteReminderAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
