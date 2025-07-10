using Microsoft.AspNetCore.Identity;

namespace BirthDateReminder.Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool NotifyInBD { get; set; }
        public bool NotifyDayBefore { get; set; }
    }
}
