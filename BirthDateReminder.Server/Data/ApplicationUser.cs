using Microsoft.AspNetCore.Identity;

namespace BirthDateReminder.Server.Data
{
    #nullable disable
    public class ApplicationUser : IdentityUser
    {
        public override string Email { get; set; } = default!;
    }
    #nullable restore
}
