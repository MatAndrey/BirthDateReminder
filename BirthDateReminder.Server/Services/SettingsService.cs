using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BirthDateReminder.Server.Services
{
    public class SettingsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;

        public SettingsService(UserManager<ApplicationUser> userManager, IEmailService emailService, AppDbContext context, ImageService imageService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _context = context;
            _imageService = imageService;
        }

        public async Task<UserSettingsDto> GetUserSettingsAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");

            return new UserSettingsDto { Email = user.Email };
        }

        public async Task ChangePasswordAsync(ClaimsPrincipal userPrincipal, ChangePasswordDto dto)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!result.Succeeded) throw new Exception("Ошибка при изменении пароля");
        }

        public async Task ChangeEmailAsync(ClaimsPrincipal userPrincipal, ChangeEmailDto dto)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");

            var result = await _userManager.SetEmailAsync(user, dto.NewEmail);

            if (!result.Succeeded) throw new Exception("Ошибка при изменении пароля");
        }

        public async Task SendTestEmailAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");
            await _emailService.SendTestEmailAsync(user.Email);
        }

        public async Task DeleteAccountAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal) ?? throw new Exception("Пользователь не найден");

            var birthdays = _context.BirthdayItems.Where(b => b.OwnerId == user.Id);

            foreach (var birthday in birthdays)
            {
                if (!string.IsNullOrEmpty(birthday.ImagePath))
                {
                    _imageService.DeleteImage(birthday.ImagePath);
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if(!result.Succeeded) throw new Exception("Ошибка при удалении аккаунта");
        }
    }
}
