using Microsoft.EntityFrameworkCore;

namespace MinimalistArchitecture.Routes.User
{
    public class UserDb : DbContext
    {
        public UserDb(DbContextOptions<UserDb> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
    }
}
