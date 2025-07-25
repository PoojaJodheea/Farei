
using FormRequest.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace FormRequest.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FormReqDb> FormReqDb { get; set; }
        public DbSet<Registry> Registry { get; set; } = default!;
        public DbSet<ThirdParty> ThirdParties{ get; set; }
        public DbSet<ITTreport> ITTreport { get; set; } = default!;



    }

    
}