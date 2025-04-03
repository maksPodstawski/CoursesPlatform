using CoursesPlatformBackend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoursesPlatformBackend.Data
{
    public class PlatformDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

    }
}
