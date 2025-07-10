using System.ComponentModel.DataAnnotations;

namespace BirthDateReminder.Server.Dtos
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
