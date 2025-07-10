using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using BirthDateReminder.Server.Models;
using BirthDateReminder.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BirthDateReminder.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BirthdayController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BirthdayController(AppDbContext context, ImageService imageService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(await _context.BirthdayItems.Where(el => el.OwnerId == user.Id).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(await _context.BirthdayItems.FirstOrDefaultAsync(el => el.OwnerId == user.Id && el.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BirthdayDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            string imageUrl = null;
            if (formData.Image != null && formData.Image.Length > 0)
            {
                try
                {
                    imageUrl = await _imageService.SaveImage(formData.Image);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

            var birthday = new BirthdayItem { BirthDate = formData.BirthDate, Name = formData.Name, ImagePath = imageUrl, OwnerId = user.Id };

            _context.BirthdayItems.Add(birthday);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = birthday.Id }, birthday);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BirthdayDto formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var birthday = await _context.BirthdayItems.FindAsync(id);
            if (birthday == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            string imageUrl = null;
            if (formData.Image != null && formData.Image.Length > 0)
            {
                try
                {
                    imageUrl = await _imageService.SaveImage(formData.Image);
                    if(birthday.ImagePath != null)
                        _imageService.DeleteImage(birthday.ImagePath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

            birthday.BirthDate = formData.BirthDate;
            birthday.Name = formData.Name;
            if(imageUrl != null)
            {
                birthday.ImagePath = imageUrl;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var birthday = await _context.BirthdayItems.FindAsync(id);
            if (birthday == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (birthday.OwnerId != user.Id)
            {
                return Forbid();
            }

            if (birthday.ImagePath != null)
                _imageService.DeleteImage(birthday.ImagePath);
            _context.BirthdayItems.Remove(birthday);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
