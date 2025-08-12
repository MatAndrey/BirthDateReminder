using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using BirthDateReminder.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BirthDateReminder.Server.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public ReminderService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendRemindersAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task SendRemindersAsync()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var reminders = await context.Reminders
                .Include(r => r.Birthday)
                .ToListAsync();

            var userIds = reminders.Select(r => r.Birthday.OwnerId).Distinct();
            var users = await userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            foreach (var reminder in reminders)
            {
                if (!users.TryGetValue(reminder.Birthday.OwnerId, out var user))
                    continue;

                DateOnly reminderDate = reminder.UnitsType switch
                {
                    UnitTypes.Day => reminder.Birthday.BirthDate.AddDays(-reminder.UnitsCount),
                    UnitTypes.Week => reminder.Birthday.BirthDate.AddDays(-reminder.UnitsCount * 7),
                    UnitTypes.Month => reminder.Birthday.BirthDate.AddMonths(-reminder.UnitsCount),
                    _ => reminder.Birthday.BirthDate
                };

                if (reminderDate.Month == today.Month && reminderDate.Day == today.Day)
                {
                    await emailService.SendReminerAsync(user.Email, reminder);
                }
            }


            var birthdays = await context.BirthdayItems
                    .Where(b => b.BirthDate.Day == today.Day && b.BirthDate.Month == today.Month)
                    .ToListAsync();

            foreach (var b in birthdays)
            {
                if (!users.TryGetValue(b.OwnerId, out var user))
                    continue;
                await emailService.SendTodayReminderAsync(user.Email, b.Name);
            }
        }
    }

}
