using BirthDateReminder.Server.Data;
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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
            var users = await context.Users.ToListAsync();

            foreach (var user in users)
            {

                var birthdays = await context.BirthdayItems
                    .Where(b => b.OwnerId == user.Id &&
                        ((user.NotifyInBD && b.BirthDate.Month == today.Month && b.BirthDate.Day == today.Day) ||
                         (user.NotifyDayBefore && b.BirthDate.Month == tomorrow.Month && b.BirthDate.Day == tomorrow.Day)))
                    .ToListAsync();

                foreach (var b in birthdays)
                {
                    var subject = $"Напоминание: у {b.Name} день рождения!";
                    var body = $"Не забудь поздравить {b.Name} с днём рождения {b.BirthDate:dd.MM} 🎉";
                    await emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
        }
    }

}
