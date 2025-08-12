using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using BirthDateReminder.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BirthDateReminder.Server.Services
{
    public class BirthdayService
    {
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;
        public BirthdayService(AppDbContext context, ImageService imageService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }
        public async Task<List<BirthdayItem>> GetUserBirthdaysAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");
            return await _context.BirthdayItems.Where(el => el.OwnerId == user.Id).ToListAsync();
        }

        public async Task<BirthdayItem?> GetBirthdayAsync(int id)
        {
            return await _context.BirthdayItems.Include(b => b.Reminders).FirstOrDefaultAsync(el => el.Id == id);
        }

        public async Task<BirthdayItem> CreateBirthdayAsync(BirthdayDto formData, ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");

            string? imageUrl = null;
            if (formData.Image != null && formData.Image.Length > 0)
            {
                imageUrl = await _imageService.SaveImage(formData.Image);
            }

            var birthday = new BirthdayItem { 
                BirthDate = formData.BirthDate, 
                Name = formData.Name, 
                ImagePath = imageUrl, 
                OwnerId = user.Id };

            _context.BirthdayItems.Add(birthday);
            await _context.SaveChangesAsync();

            return birthday;
        }

        public async Task UpdateBirthdayAsync(BirthdayDto formData, int id)
        {
            BirthdayItem birthday = await _context.BirthdayItems.FindAsync(id) ?? throw new Exception("День рождения не найден");
            ApplicationUser user = await _userManager.FindByIdAsync(birthday.OwnerId) ?? throw new Exception("Польователь не найден");

            string? imageUrl = null;
            if (formData.Image != null && formData.Image.Length > 0)
            {
                try
                {
                    imageUrl = await _imageService.SaveImage(formData.Image);
                    if (birthday.ImagePath != null)
                        _imageService.DeleteImage(birthday.ImagePath);
                }
                catch (Exception)
                {
                    throw new Exception("Не удалось загрузить изображение");
                }
            }

            birthday.BirthDate = formData.BirthDate;
            birthday.Name = formData.Name;
            if (imageUrl != null)
            {
                birthday.ImagePath = imageUrl;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBirthdayAsync(int id)
        {
            BirthdayItem birthday = await _context.BirthdayItems.FindAsync(id) ?? throw new Exception("День рождения не найден");

            if (birthday.ImagePath != null)
                _imageService.DeleteImage(birthday.ImagePath);
            _context.BirthdayItems.Remove(birthday);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Reminder>> CreateReminderAsync(ReminderDto dto)
        {
            BirthdayItem birthday = await _context.BirthdayItems.FindAsync(int.Parse(dto.BirthdayId)) ?? throw new Exception("День рождения не найден");
            UnitTypes unitType = dto.ReminderUnit switch
            {
                "День" => UnitTypes.Day,
                "Неделя" => UnitTypes.Week,
                "Месяц" => UnitTypes.Month,
                _ => UnitTypes.Day
            };

            Reminder reminder = new Reminder { Birthday = birthday, UnitsCount = dto.ReminderNum, UnitsType = unitType };

            _context.Reminders.Add(reminder);
            birthday.Reminders.Add(reminder);

            await _context.SaveChangesAsync();

            return await _context.Reminders.Where(reminder => reminder.Birthday == birthday).ToListAsync();
        }

        public async Task<List<Reminder>> DeleteReminderAsync(int id)
        {
            Reminder reminder = await _context.Reminders.Include(r => r.Birthday).FirstOrDefaultAsync(el => el.Id == id) ?? throw new Exception("Напоминание не найдено");
            BirthdayItem birthday = reminder.Birthday;

            _context.Reminders.Remove(reminder);

            await _context.SaveChangesAsync();

            return await _context.Reminders.Where(reminder => reminder.Birthday.Id == birthday.Id).ToListAsync();
        }
    }
}
