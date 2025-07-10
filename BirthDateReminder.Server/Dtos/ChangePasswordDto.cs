using System.ComponentModel.DataAnnotations;

namespace BirthDateReminder.Server.Dtos
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
