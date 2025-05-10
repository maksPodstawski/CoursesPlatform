using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class CoursesPlatformContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<PurchasedCourses> PurchasedCourses { get; set; }
        public DbSet<Creator> Creators { get; set; }
        public DbSet<CourseSubcategory> CourseSubcategories { get; set; }
        
        public CoursesPlatformContext() { }
        
        public CoursesPlatformContext(DbContextOptions<CoursesPlatformContext> options) : base(options) { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=15432;Database=postgres;User Id=postgres;Password=postgres");
            }
        }
    }
}
