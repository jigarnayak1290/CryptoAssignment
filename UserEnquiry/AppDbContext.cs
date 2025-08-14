using Microsoft.EntityFrameworkCore;
using UserEnquiry.Models;

namespace UserEnquiry
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UserInfo> UserInfos { get; set; }
    }
}
