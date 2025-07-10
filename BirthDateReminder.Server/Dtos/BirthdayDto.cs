using System.ComponentModel.DataAnnotations;

namespace BirthDateReminder.Server.Dtos
{
    public class BirthdayDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        public IFormFile? Image { get; set; }
    }
}
