using System.Net.Mail;
using System.Net;
using BirthDateReminder.Server.Models;
using System.Text;

namespace BirthDateReminder.Server.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendTestEmailAsync(string to);
        Task SendReminerAsync(string userEmail, Reminder reminder);
        Task SendTodayReminderAsync(string userEmail, string name);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SendTestEmailAsync(string to)
        {
            string html = File.ReadAllText("./Templates/test_email.html");
            await SendEmailAsync(to, "Тестовое письмо", html);
        }

        public async Task SendTodayReminderAsync(string userEmail, string name)
        {
            string htmlTemplate = File.ReadAllText("./Templates/birthday_today_email_template.html");
            string finalHtml = htmlTemplate.Replace("{{Имя}}", name);

            await SendEmailAsync(userEmail, "Сегодня день рождения", finalHtml);
        }

        public async Task SendReminerAsync(string userEmail, Reminder reminder)
        {
            string htmlTemplate = File.ReadAllText("./Templates/birthday_email_template.html");
            string finalHtml = htmlTemplate
                .Replace("{{Имя}}", reminder.Birthday.Name)
                .Replace("{{Количество}}", reminder.UnitsCount.ToString())
                .Replace("{{Дней/Недель/Месяцев}}", ReminderToString(reminder.UnitsCount, reminder.UnitsType))
                .Replace("{{Ссылка}}", GenerateGoogleCalendarUrl($"День рождения {reminder.Birthday.Name}", reminder.Birthday.BirthDate));

            await SendEmailAsync(userEmail, "Напоминание о дне рождения", finalHtml);
        }

        private string GenerateGoogleCalendarUrl(string eventName, DateOnly eventDate)
        {
            string startDate = eventDate.ToString("yyyyMMdd");

            string endDate = eventDate.AddDays(1).ToString("yyyyMMdd");

            var url = new StringBuilder("https://www.google.com/calendar/render?action=TEMPLATE");

            url.Append($"&text={Uri.EscapeDataString(eventName)}");
            url.Append($"&dates={startDate}/{endDate}");

            url.Append("&ctz=UTC");

            return url.ToString();
        }


        private static string Declension(int value, string[] words) {
            value = Math.Abs(value) % 100;
            var num = value % 10;
            if (value > 10 && value< 20) return words[2];
            if (num > 1 && num< 5) return words[1];
            if (num == 1) return words[0];
            return words[2];
        }

        private static string ReminderToString(int reminderNum, UnitTypes reminderUnit) {
            switch (reminderUnit) {
                case UnitTypes.Day: return Declension(reminderNum, ["День", "Дня", "Дней"]);
                case UnitTypes.Week: return Declension(reminderNum, ["Неделя", "Недели", "Недель"]);
                case UnitTypes.Month: return Declension(reminderNum, ["Месяц", "Месяца", "Месяцев"]);
                default: return "";
            }
        }
    }
}
