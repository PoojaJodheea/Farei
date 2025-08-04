
using Microsoft.AspNetCore.Identity;

namespace FormRequest.Models
{
    public class ApplicationUser : IdentityUser
    {
        public String? Type { get; set; }
        public String? Site { get; set; }
        public String? Supervisor { get; set; }

        public String? Dept { get; set; }
    }
}